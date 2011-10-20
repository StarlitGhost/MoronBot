using System;

using CwIRC;

namespace MBUtilities
{
	public static class MBEvents
	{
        public static event StringEventHandler NickChanged;
        public static void OnNickChanged(object sender, string nick)
        {
            if (NickChanged != null)
                NickChanged(sender, nick);
        }

        public static event StringEventHandler NewRawIRC;
        public static void OnNewRawIRC(object sender, string text)
        {
            if (NewRawIRC != null)
                NewRawIRC(sender, text);
        }

        public static event StringEventHandler NewFormattedIRC;
        public static void OnNewFormattedIRC(object sender, string text)
        {
            if (NewFormattedIRC != null)
                NewFormattedIRC(sender, text);
        }
		
        public static event VoidEventHandler ChannelListModified;
        public static void OnChannelListModified()
        {
            if (ChannelListModified != null)
                ChannelListModified(new object());
        }

        public static event VoidEventHandler UserListModified;
        public static void OnUserListModified()
        {
            if (UserListModified != null)
                UserListModified(new object());
        }
	}
}

