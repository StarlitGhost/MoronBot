from IRCMessage import IRCMessage
from IRCResponse import IRCResponse, ResponseType
from Function import Function
from GlobalVars import *

import re
import datetime
import subprocess

class Instantiate(Function):
    Help = 'Gives you a log link'
    
    def GetResponse(self, message):
        if message.Type != 'PRIVMSG' or not message.MessageString[:1] == '|':
            return
        
        match = re.search('^log$', message.Command, re.IGNORECASE)
        if not match:
            return
        
        date = datetime.datetime.utcnow();
        if len(message.MessageString.strip().split()) > 1 and message.MessageString.strip().split().pop(1).replace("-","").isdigit():
            date += datetime.timedelta(days = int(message.MessageString.strip().split().pop(1)))
        
        proc = subprocess.Popen(['/usr/bin/php','/opt/moronbot/getloghash.php',message.ReplyTo+"-"+ date.strftime('%Y%m%d')], stdout=subprocess.PIPE)
        hash = proc.stdout.read()
        if hash == "Not found":
            output = "I don't have that log."
        else:
            output = "Log for " + date.strftime('%Y/%m/%d') + ": http://stats.fugiman.com/logs/?l=" + hash
            return IRCResponse(ResponseType.Say, output, message.ReplyTo)
