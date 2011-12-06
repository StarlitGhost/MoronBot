from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re

class Instantiate(Function):
    Help = 'Responds to bees'

    def GetResponse(self, message):
        match = re.search('([^a-zA-Z]|^)bee+s?([^a-zA-Z]|$)',
                          message.MessageString,
                          re.IGNORECASE)
        if match:
            return IRCResponse(ResponseType.Say,
                               'BEES?! AAAARRGGHGHFLFGFGFLHL',
                               message.ReplyTo)
