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
			
			MBEvents.NickChanged += OnNickChanged;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
			string url = "http://localhost:8080/message";
			
            string json = JsonConvert.SerializeObject(message);
			
			try
			{
				Stream responseStream = SendToServer(url, json);
				string jsonResponse = ReceiveFromServer(responseStream);
				
				List<IRCResponse> ircResponses = JsonConvert.DeserializeObject<List<IRCResponse>>(jsonResponse);
	            
	            return ircResponses;
			}
			catch (System.Exception /*ex*/)
			{
				//return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "PythonInterface Exception: " + ex.Message, message.ReplyTo) };
				return null;
			}
        }
		
		void OnNickChanged(object o, string newNick)
		{
			string url = "http://localhost:8080/nickchange";
			try
			{
				SendToServer(url, newNick);
			}
			catch (System.Exception /*ex*/)
			{
				return;
			}
		}
		
		Stream SendToServer(string url, string text)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/json; charset=utf-8";
			StreamWriter writer = new StreamWriter(request.GetRequestStream());
			writer.Write(text);
			writer.Close();
			
			HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
			return webResponse.GetResponseStream();
		}
		
		string ReceiveFromServer(Stream responseStream)
		{
			Encoding encode = Encoding.UTF8;
			StreamReader stream = new StreamReader(responseStream, encode);
			
			StringBuilder sb = new StringBuilder();
			sb.Append(stream.ReadToEnd());
			return sb.ToString();
		}
    }
}
