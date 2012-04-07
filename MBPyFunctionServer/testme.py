from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

from enumType import enum

TargetTypes = enum('CHANNEL', 'USER')

class UserStruct:
    Hostmask = ''
    Name = ''
    User = ''

    def __init__(self, dct):
        self.__dict__ = dct

class FakeIRCMessage:
    User = UserStruct({'User':'','Name':'','Hostmask':''})
    TargetType = TargetTypes.CHANNEL
    Type = ''
    ReplyTo = ''
    MessageList = []
    MessageString = "<Jimmy> ducttape minecraft!"
    RawMessage = ''
    CTCP = False
    CTCPString = ''
    Parameters = ''
    ParameterList = ['wat']
    Command = 'responses'

import responses

print responses.Instantiate().GetResponse(FakeIRCMessage())
