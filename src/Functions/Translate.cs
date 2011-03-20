using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions
{
    class Translate : Function
    {
        public Translate(MoronBot moronBot)
        {
            Name = this.GetType().ToString().Split('.')[2];
            Help = "translate <sentence>\t- Translates the given sentence to English.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override IRCResponse GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(translate)$", RegexOptions.IgnoreCase))
            {
                if (message.Parameters.Length > 0)
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
                    return new IRCResponse(ResponseType.Say, translatedString, message.ReplyTo);
                }
                else
                {
                    return new IRCResponse(ResponseType.Say, "Translate what?", message.ReplyTo);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
