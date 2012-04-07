'''
Created on Dec 20, 2011

@author: Tyranic-Moron
'''

from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re

class Instantiate(Function):
    Help = 'Responds with a link to the chat stats webpage'

    def GetResponse(self, message):
        if message.Type != 'PRIVMSG' or not message.MessageString[:1] == '|':
            return
        
        match = re.search('^stats$', message.Command, re.IGNORECASE)
        if not match:
            return
        
        return IRCResponse(ResponseType.Say,
                           'sss: http://stats.fugiman.com | pisg: http://silver.amazon.fooproject.net/pisg/desertbus.html',
                           message.ReplyTo)
