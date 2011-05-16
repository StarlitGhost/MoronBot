using System.Text.RegularExpressions;

using CwIRC;
using System.Collections.Generic;

namespace MoronBot.Functions.Internet
{
    class Translate : Function
    {
        public Translate()
        {
            Help = "translate <sentence>\t- Translates the given sentence to English.";
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
                    catch (Gapi.Core.GapiException e)
                    {
                        Program.form.txtProgLog_Update(e.ToString());
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
