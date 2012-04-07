import sys
import web
import GlobalVars

try:
    import json
except ImportError:
    import simplejson as json

import os
abspath = os.path.abspath(__file__)
dname = os.path.dirname(abspath)
os.chdir(dname)

from IRCResponse import IRCResponse, ResponseType
from IRCMessage import IRCMessage

from FunctionHandler import AutoLoadFunctions
from GlobalVars import functions

class MessageHandler:
    def POST(self, name=None):
        data = web.data()
        jsonData = json.loads(data)
        message = IRCMessage(jsonData)
        
        if message.MessageString is not None:
            message.MessageString = message.MessageString.encode('ascii', 'xmlcharrefreplace')
            
        #print message.__dict__
            
        print ( '%s <%s> %s' % (message.ReplyTo,
                                message.User.Name,
                                message.MessageString) )
        
        responses = []
        
        for (name, func) in functions.items():
            try:
                response = func.GetResponse(message)
                if response is None:
                    continue
                if hasattr(response, '__iter__'):
                    for r in response:
                        responses.append( r.__dict__ )
                else:
                    responses.append( response.__dict__ )
            except Exception:
                msg = IRCResponse(ResponseType.Say,
                                  ("Python Execution Error in '%s': %s" %
                                   (name, str( sys.exc_info() ))),
                                  message.ReplyTo)
                responses.append( msg.__dict__ )
        
        return json.dumps(responses)

class BotDetailHandler:
    def POST(self, path=None):
        if path is not 'nickchange':
            return
        
        newNick = web.data()
        print 'nickchange received: ' + newNick
        GlobalVars.CurrentNick = newNick

urls = (
	'/message', 'MessageHandler',
	'/(nickchange)', 'BotDetailHandler'
)

if __name__ == '__main__':
    AutoLoadFunctions()
    app = web.application(urls, globals(), True)
    app.run()
