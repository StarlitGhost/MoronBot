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
    public class ModdTunes : Function
    {
        public ModdTunes()
        {
            Help = "m(odd)t(unes) - Tells you what Moddington's internet radio is currently playing.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(m(odd)?t(unes)?)$", RegexOptions.IgnoreCase))
            {
                Mpc mpc = new Mpc();

                var addresses = System.Net.Dns.GetHostAddresses("moddington.net");
                if (addresses.Length > 0)
                {
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

                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, output, message.ReplyTo) };
                }
                else
                {
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Moddington's internet radio seems to be down", message.ReplyTo) };
                }
            }
            
            return null;
        }
    }
}