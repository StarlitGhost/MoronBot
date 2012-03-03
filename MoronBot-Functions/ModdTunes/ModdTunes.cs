using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;

using Libmpc;
using MBUtilities.Channel;

namespace Internet
{
    /// <summary>
    /// A Function which returns the currently playing song from Moddington's internet radio station.
    /// </summary>
    /// <suggester>Moddington</suggester>
    public class ModdTunes : Function
    {
        public ModdTunes()
        {
            Help = "m(odd)t(unes) - Tells you what Moddington's internet radio is currently playing.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            FuncInterface.CommandFormatMessageReceived += commandReceived;
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, "^(m(odd)?t(unes)?)$", RegexOptions.IgnoreCase))
                return;

            Mpc mpc = new Mpc();

            var addresses = System.Net.Dns.GetHostAddresses("moddington.net");
            if (addresses.Length == 0)
            {
                FuncInterface.SendResponse(ResponseType.Say, "Moddington's internet radio seems to be down", message.ReplyTo);
                return;
            }

            mpc.Connection = new MpcConnection(new IPEndPoint(addresses[0], 6600));

            var song = mpc.CurrentSong();
            string songMsg = "";
            if (song.HasTitle || song.HasArtist)
            {
                if (song.HasTitle) songMsg += song.Title;
                else songMsg += "<Unknown Title>";
                if (song.HasArtist) songMsg += " - " + song.Artist;
                else songMsg += "<Unknown Artist>";
            }
            else
            {
                songMsg += song.File;
            }

            var status = mpc.Status();
            TimeSpan elapsed = TimeSpan.FromSeconds(status.TimeElapsed);
            TimeSpan total = TimeSpan.FromSeconds(status.TimeTotal);
            string timeMsg = (elapsed.Hours > 0 ? elapsed.Hours + ":" : "") + elapsed.Minutes + ":" + elapsed.Seconds.ToString("D2") + "/" +
                (total.Hours > 0 ? total.Hours + ":" : "") + total.Minutes + ":" + total.Seconds.ToString("D2");

            string output = "Playing: " + songMsg +
                " [" + timeMsg + "] - Listen here: " + ChannelList.EvadeChannelLinkBlock(message, "http://moddington.net:8000/moddtunes.ogg");

            FuncInterface.SendResponse(ResponseType.Say, output, message.ReplyTo);
            return;
        }
    }
}