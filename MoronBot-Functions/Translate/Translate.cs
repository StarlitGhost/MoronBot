using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

namespace Internet
{
    /// <summary>
    /// A Function which returns the english translation of a given sentence from Google Translate.
    /// No longer works because Google restricted access to the translation API.
    /// </summary>
    public class Translate : Function
    {
        public Translate()
        {
            Help = "translate <sentence> - Translates the given sentence to English.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(translate)$", RegexOptions.IgnoreCase))
                return;

            if (message.ParameterList.Count == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Translate what?", message.ReplyTo);
                return;
            }

            string translatedString;
            try
            {
                string translateTerm = message.Parameters.Replace('"', ' ');
                translatedString = Gapi.Language.Translator.Translate(translateTerm, Gapi.Language.Language.English);
            }
            catch (Gapi.Core.GapiException ex)
            {
                Logger.Write(ex.ToString(), Settings.Instance.ErrorFile);
                translatedString = "Couldn't work out what language you're using.";
            }
            catch (System.Net.WebException ex)
            {
                Logger.Write(ex.ToString(), Settings.Instance.ErrorFile);
                translatedString = "Google Translate appears to be down.";
            }
            FuncInterface.SendResponse(ResponseType.Say, translatedString, message.ReplyTo);
            return;
        }
    }
}
