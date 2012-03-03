using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

using MBFunctionInterface;
using CwIRC;

using Newtonsoft.Json;
using MBUtilities;

namespace PythonInterface
{
    public class PythonInterface : Function
    {
        public PythonInterface()
        {
            Help = "A C# function which passes messages to other functions written in python.";
            Type = Types.Regex;
            AccessLevel = AccessLevels.Anyone;
            
            FuncInterface.NickChanged += OnNickChanged;
            FuncInterface.AnyMessageReceived += anyMessageReceived;
        }

        void anyMessageReceived(object sender, BotMessage message)
        {
            string url = "http://localhost:8080/message";
            
            string json = JsonConvert.SerializeObject(message);
            
            try
            {
                Stream responseStream = URL.SendToServer(url, json);
                string jsonResponse = URL.ReceiveFromServer(responseStream);
                
                List<BotResponse> responses = JsonConvert.DeserializeObject<List<BotResponse>>(jsonResponse);

                FuncInterface.SendResponses(responses);
                return;
            }
            catch (System.Exception /*ex*/)
            {
                //FuncInterface.SendResponse(ResponseType.Say, "PythonInterface Exception: " + ex.Message, message.ReplyTo);
                return;
            }
        }
        
        void OnNickChanged(object o, string newNick)
        {
            string url = "http://localhost:8080/nickchange";
            try
            {
                URL.SendToServer(url, newNick);
            }
            catch (System.Exception /*ex*/)
            {
                return;
            }
        }
    }
}
