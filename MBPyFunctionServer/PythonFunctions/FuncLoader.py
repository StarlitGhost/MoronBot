from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import sys
from FunctionHandler import LoadFunction, UnloadFunction

class Instantiate(Function):
    Help = "Handles loading/unloading of python functions."

    def GetResponse(self, message):
        if message.Command == "pyload":
            path = message.ParameterList[0]
            try:
                loadType = LoadFunction(path)
                return IRCResponse(ResponseType.Say, "Python Function '" + path + "' " + loadType + "oaded!", message.ReplyTo)
            except Exception:
                return IRCResponse(ResponseType.Say, "Python Load Error: " + str(sys.exc_info()), message.ReplyTo)
        elif message.Command == "pyunload":
            path = message.ParameterList[0]
            try:
                success = UnloadFunction(path)
                if success:
                    return IRCResponse(ResponseType.Say, "Python Function '" + path + "' unloaded!", message.ReplyTo)
                else:
                    return IRCResponse(ResponseType.Say, "Python Function '" + path + "' not found", message.ReplyTo)
            except Exception, x:
                return IRCResponse(ResponseType.Say, "Python Unload Error: " + str(sys.exc_info()), message.ReplyTo)