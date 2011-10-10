from MBFunctionInterface import BotMessage
from CwIRC import IRCResponse, ResponseType
from MBUtilities import Settings
from Function import Function

import re

class Instantiate(Function):
	Help = "I unno lol"

	def GetResponse(self, message):
		match = re.search("^(ahoy there|oh hai)[ ]" + Settings.Instance.CurrentNick, message.MessageString, re.IGNORECASE)
		if match:
			return IRCResponse(ResponseType.Say, match.group(1) + " " + message.User.Name + "!", message.ReplyTo)
		return