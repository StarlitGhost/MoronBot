from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re
import subprocess

class Instantiate(Function):
	Help = 'Checks the log for the last time someone mentioned a given word or phrase'
	def GetResponse(self, message):
		if message.Type != 'PRIVMSG':
			return
			
		if (len(message.MessageString.strip().split()) > 1) and (message.MessageString.strip().split().pop(0) == "|lastmention" or message.MessageString.strip().split().pop(0) == "|lastmentioned"):
			proc = subprocess.Popen(['/usr/bin/php','/opt/moronbot/loggrep.php',"\""+message.MessageString.replace("|lastmention ",'').replace("|lastmentioned ",'').replace("\"","\\\"").replace("\n","\\\n")+"\"",message.ReplyTo,"mention"], stdout=subprocess.PIPE)
			output = proc.stdout.read()		
			return IRCResponse(ResponseType.Say,output, message.ReplyTo)
		if (len(message.MessageString.strip().split()) > 1) and (message.MessageString.strip().split().pop(0) == "|lastsaid"):
			proc = subprocess.Popen(['/usr/bin/php','/opt/moronbot/loggrep.php',"\""+message.MessageString.replace("|lastsaid ",'').replace("\"","\\\"").replace("\n","\\\n")+"\"",message.ReplyTo,"mentionnottoday"],stdout=subprocess.PIPE)
			output = proc.stdout.read()		
			return IRCResponse(ResponseType.Say,output, message.ReplyTo)
