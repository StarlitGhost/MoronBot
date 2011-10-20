from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re

class Instantiate(Function):
    Help = "Responds to people who greet the bot"

    def GetResponse(self, message):
        match = re.search("^(ahoy there)[ ]" + CurrentNick, message.MessageString, re.IGNORECASE)
        if match:
            return IRCResponse(ResponseType.Say, match.group(1) + " " + message.User.Name + "!", message.ReplyTo)
        return
