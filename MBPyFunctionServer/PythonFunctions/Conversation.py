from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re

class Instantiate(Function):
    Help = 'Responds to greetings and such'

    def GetResponse(self, message):
        match = re.search("^(?P<greeting>'?sup|hi(ya)?|hey|hello|'?lo|mornin[g']?|greetings|bonjour|salut|howdy|'?yo|o?hai|mojn|hej|dongs|ahoy( hoy)?|salutations|g'?day|bye|night)(there)?,?[ ]%s([^a-zA-Z0-9_\|`\[\]\^-]|$)" % CurrentNick,
                          message.MessageString,
                          re.IGNORECASE)
        if match:
            return IRCResponse(ResponseType.Say,
                               '%s %s' % (match.group('greeting'), message.User.Name),
                               message.ReplyTo)