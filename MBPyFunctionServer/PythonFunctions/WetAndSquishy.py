from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re

class Instantiate(Function):
    Help = 'Responds to inappropriately combined adjectives'

    def GetResponse(self, message):
        match = re.search("([^a-zA-Z]|^)wet (and|&|'?n'?) squishy([^a-zA-Z]|$)",
                          message.MessageString,
                          re.IGNORECASE)
        if not match:
            match = re.search("([^a-zA-Z]|^)squishy (and|&|'?n'?) wet([^a-zA-Z]|$)",
                              message.MessageString,
                              re.IGNORECASE)

        if match:
            return IRCResponse(ResponseType.Say,
                                'GODDAMMIT, GET YOUR HAND OUT OF THERE',
                                message.ReplyTo)