from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re

class Instantiate(Function):
    Help = 'Responds to DuctTape being a dick in minecraft'

    def GetResponse(self, message):
        match = re.search('([^a-zA-Z]|^)minecraft([^a-zA-Z]|$)',
                          message.MessageString,
                          re.IGNORECASE)
        ductMatch = re.search('([^a-zA-Z]|^)(?P<duc>duc[kt]tape)([^a-zA-Z]|$)',
                          message.MessageString,
                          re.IGNORECASE)
        if match:
            if ductMatch:
                return IRCResponse(ResponseType.Say,
                               'Just saying, %s is a dick in Minecraft' % ductMatch.group('duc'),
                               message.ReplyTo)