from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re

class Instantiate(Function):
    Help = 'Responds to incorrect windmill assumptions'

    def GetResponse(self, message):
        match = re.search('([^a-zA-Z]|^)windmills?([^a-zA-Z]|$)',
                          message.MessageString,
                          re.IGNORECASE)
        coolMatch = re.search('([^a-zA-Z]|^)cool([^a-zA-Z]|$)',
                              message.MessageString,
                              re.IGNORECASE)

        if match and coolMatch:
            return [IRCResponse(ResponseType.Say,
                                 'WINDMILLS DO NOT WORK THAT WAY!',
                                 message.ReplyTo),
					 IRCResponse(ResponseType.Say,
					             'GOODNIGHT!',
								 message.ReplyTo)]