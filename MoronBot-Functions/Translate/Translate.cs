using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Internet
{
    public class Translate : Function
    {
        public Translate()
        {
            Help = "translate <sentence> - Translates the given sentence to English.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(translate)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    string translatedString;
                    try
                    {
                        string translateTerm = message.Parameters.Replace('"', ' ');
                        translatedString = Gapi.Language.Translator.Translate(translateTerm, Gapi.Language.Language.English);
                    }
                    catch (Gapi.Core.GapiException ex)
                    {
                        string filePath = string.Format(@".{0}logs{0}errors.txt", Path.DirectorySeparatorChar);
                        Logger.Write(ex.ToString(), filePath);
                        translatedString = "Couldn't work out what language you're using.";
                    }
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, translatedString, message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Translate what?", message.ReplyTo) };
                }
            }
            else
            {
                return null;
            }
        }
    }
}
