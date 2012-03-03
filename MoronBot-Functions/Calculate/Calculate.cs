using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

using Newtonsoft.Json;
using System.Web;

namespace Utility
{
    public class Calculate : Function
    {
        //http://www.google.com/ig/calculator?hl=en&q=sin(3)USD+in+GBP
        //{lhs: "sin(3) * U.S. dollar",rhs: "0.0908049727 British pounds",error: "",icc: true}

        public Calculate()
        {
            Help = "calc <expr> - Uses Google's calculation API to give you the result of <expr>";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, @"^(calc(ulate)?)$", RegexOptions.IgnoreCase))
                return;
            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "You didn't give an expression to calculate!", message.ReplyTo);
                return;
            }

            try
            {
                string query = HttpUtility.UrlEncode(message.Parameters);
                Stream responseStream = URL.SendToServer("http://www.google.com/ig/calculator?hl=en&q=" + query);
                string jsonResponse = URL.ReceiveFromServer(responseStream);

                Result result = JsonConvert.DeserializeObject<Result>(jsonResponse);
                if (result.rhs.Length > 0)
                {
                    FuncInterface.SendResponse(ResponseType.Say, "Result: " + result.rhs, message.ReplyTo);
                    return;
                }
                if (result.error.Length > 0)
                {
                    FuncInterface.SendResponse(ResponseType.Say, "Error: " + result.error, message.ReplyTo);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
                return;
            }
        }

        class Result
        {
            //{lhs: "sin(3) * U.S. dollar",rhs: "0.0908049727 British pounds",error: "",icc: true}
            public string lhs;
            public string rhs;
            public string error;
            public string icc;
        }
    }
}
