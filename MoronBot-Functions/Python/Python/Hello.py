from MBFunctionInterface import BotMessage
from CwIRC import IRCResponse, ResponseType
from MBUtilities import Settings
from Function import Function

import re

class Instantiate(Function):
	Help = "Responds to people who greet the bot"

	def GetResponse(self, message):
		match = re.search("^(hello|ahoy there|oh hai)[ ]" + Settings.Instance.CurrentNick, message.MessageString, re.IGNORECASE)
		if match:
			return IRCResponse(ResponseType.Say, match.group(1) + " " + message.User.Name + "!", message.ReplyTo)
		return