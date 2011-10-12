from MBFunctionInterface import BotMessage
from CwIRC import IRCResponse, ResponseType
from MBUtilities import Settings
from Function import Function

import re
import urllib, urllib2

class Instantiate(Function):
    Help = "wiki(pedia) <search term> - returns the top result for a given search term from wikipedia."
    
    def GetResponse(self, message):
        match = re.search("^wiki(pedia)?", message.Command, re.IGNORECASE)
        if match:
            article = message.Parameters
            article = urllib.quote(article)
            
            opener = urllib2.build_opener()
            opener.addheaders = [('User-agent', 'Mozilla/5.0')]
            
            try:
                page = opener.open("http://ajax.googleapis.com/ajax/services/search/web?v=1.0&q=site:en.wikipedia.org%20" + article)
                data = page.read()
                page.close()
                title = re.search("\"titleNoFormatting\":\"(?P<title>[^\"]+)", data).group('title').decode('utf-8')
                title = title[:-35]
                content = re.search("\"content\":\"(?P<content>[^\"]+)", data).group('content').decode('unicode-escape')
                content = re.sub("<.*?>", "", content)
                content = re.sub("\s+", " ", content)
                url = re.search("\"url\":\"(?P<url>[^\"]+)", data).group('url')
                replyText = title + " | " + content + " | " + url
                return IRCResponse(ResponseType.Say, replyText, message.ReplyTo)
            except:
                return IRCResponse(ResponseType.Say, "No Results!", message.ReplyTo)
        
        return