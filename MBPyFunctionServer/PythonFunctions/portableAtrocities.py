from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re

class Instantiate(Function):
    Help = 'Responds to pokemon'

    def GetResponse(self, message):
        match = re.search('([^a-zA-Z]|^)pokemon([^a-zA-Z]|$)',
                          message.MessageString,
                          re.IGNORECASE)
        if match:
			return IRCResponse(ResponseType.Say,
                          'Portable Atrocities! Must be encapsulated en masse!',
                           message.ReplyTo)