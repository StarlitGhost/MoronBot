from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re
import subprocess

class Instantiate(Function):
	Help = 'Finds a nick\'s last message'
	def GetResponse(self, message):
		if message.Type != 'PRIVMSG':
			return
			
		if (len(message.MessageString.strip().split()) > 1) and (message.MessageString.strip().split().pop(0) == "|lastseen"):
			proc = subprocess.Popen(['/usr/bin/php','/opt/moronbot/loggrep.php',message.MessageString.strip().split().pop(1),message.ReplyTo], stdout=subprocess.PIPE)
			output = proc.stdout.read()		
			return IRCResponse(ResponseType.Say,output, message.ReplyTo)
		if (len(message.MessageString.strip().split()) > 1) and (message.MessageString.strip().split().pop(0) == "|lastsaw"):
			proc = subprocess.Popen(['/usr/bin/php','/opt/moronbot/loggrep.php',message.MessageString.strip().split().pop(1),message.ReplyTo,"sawed"], stdout=subprocess.PIPE)
			output = proc.stdout.read()		
			return IRCResponse(ResponseType.Say,output, message.ReplyTo)
