using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

using Newtonsoft.Json;

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
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, @"^(calc(ulate)?)$", RegexOptions.IgnoreCase))
                return null;
            if (message.ParameterList.Count == 0)
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't give an expression to calculate!", message.ReplyTo) };

            try
            {
                string query = HttpUtility.UrlEncode(message.Parameters);
                Stream responseStream = URL.SendToServer("http://www.google.com/ig/calculator?hl=en&q=" + query);
                string jsonResponse = URL.ReceiveFromServer(responseStream);

                Result result = JsonConvert.DeserializeObject<Result>(jsonResponse);
                if (result.rhs.Length > 0)
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Result: " + result.rhs, message.ReplyTo) };
                if (result.error.Length > 0)
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Calculation Error or Unsupported Operations", message.ReplyTo) };

                return null;
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
                return null;
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
