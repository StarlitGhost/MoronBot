using System;

namespace MBUtilities
{
	public delegate void StringEventHandler(object sender, string text);
	public delegate void VoidEventHandler(object sender);

	public static class Events
	{
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

