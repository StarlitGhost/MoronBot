from enumType import enum

TargetTypes = enum('CHANNEL', 'USER')

class UserStruct:
    Hostmask = ''
    Name = ''
    User = ''

    def __init__(self, dct):
        self.__dict__ = dct

class IRCMessage:
    User = UserStruct({'User':'','Name':'','Hostmask':''})
    TargetType = TargetTypes.CHANNEL
    Type = ''
    ReplyTo = ''
    MessageList = []
    MessageString = ''
    RawMessage = ''
    CTCP = False
    CTCPString = ''
    Parameters = ''
    ParameterList = []
    Command = ''

    def __init__(self, json):
        self.__dict__ = json
        self.User = UserStruct(self.User)
