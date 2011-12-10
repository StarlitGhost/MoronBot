from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re

class Instantiate(Function):
    Help = 'Guards against the Dutch'

    def GetResponse(self, message):
        match = re.search('([^a-zA-Z]|^)dutch([^a-zA-Z]|$)',
                          message.MessageString,
                          re.IGNORECASE)
        if match:
            return IRCResponse(ResponseType.Say,
                               'The Dutch, AGAIN!',
                               message.ReplyTo)