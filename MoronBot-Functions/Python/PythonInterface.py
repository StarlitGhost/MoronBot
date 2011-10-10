import sys

from MBFunctionInterface import BotMessage
from CwIRC import IRCResponse, ResponseType

import Function

functions = {}

def ProcessMessage(message):
	responses = []
	
	if message.Command == "pyload":
		path = message.ParameterList[0]
		try:
			loadType = LoadFunction(path)
			responses.append( IRCResponse(ResponseType.Say, "Python Function '" + path + "' " + loadType + "oaded!", message.ReplyTo) )
		except Exception, x:
			responses.append( IRCResponse(ResponseType.Say, str(sys.exc_info()), "Tyranic-Moron" ) )
			#responses.append( IRCResponse(ResponseType.Say, "Error loading Python Function '" + path + "': " + x.message, message.ReplyTo) )
	elif message.Command == "pyunload":
		path = message.ParameterList[0]
		try:
			success = UnloadFunction(path)
			if success:
				responses.append( IRCResponse(ResponseType.Say, "Python Function '" + path + "' unloaded!", message.ReplyTo) )
			else:
				responses.append( IRCResponse(ResponseType.Say, "Python Function '" + path + "' not found", message.ReplyTo) )
		except Exception, x:
			responses.append( IRCResponse(ResponseType.Say, "Error unloading Python Function '" + path + "': " + x.message, message.ReplyTo) )
	else:
		for name, func in functions.iteritems():
			try:
				responses.append( func.GetResponse(message) )
			except Exception, x:
				responses.append( IRCResponse(ResponseType.Say, "Error executing Python Function '" + name + "':" + x.message, message.User.Name) )
	
	return responses

def LoadFunction(path, loadAs=""):
	loadType = "l"
	name = path
	src = __import__(name)
	if loadAs != "":
		name = loadAs
	if name in functions:
		loadType = "rel"
	reload(src)
		
	func = src.Instantiate()
		
	functions.update({name:func})
		
	return loadType

def UnloadFunction(name):
	success = 1
	if name in functions.keys():
		del functions[name]
	else:
		success = 0
	
	return success
