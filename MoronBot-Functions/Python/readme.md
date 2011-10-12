## Examples
Here is an example function that replies to people when the say hello to the bot:
```python
from MBFunctionInterface import BotMessage		# The messages you will be recieving
from CwIRC import IRCResponse, ResponseType		# What you will be sending back to the server
from MBUtilities import Settings			# The bot's settings module
from Function import Function				# The 'Function' class template

# You have to call your class 'Instantiate', and derive from Function,
# otherwise the bot won't be able to load or use it!
class Instantiate(Function):
  # The function's help text - not used yet, but fill it out anyway for when I get round to it :)
  Help = "A function that replies to people who greet the bot"

  # This is the function the bot will call to pass you messages from the IRC server
  def GetResponse(self, message):
    if message.MessageString == "hello" + Settings.Instance.CurrentNick:
      return IRCResponse( \
        ResponseType.Say, \
        "hello " + message.User.Name + "!", \
        message.ReplyTo)
    else:
      return
```

Here is an example of a command function, that makes the bot say what you ask it to:
```python
from MBFunctionInterface import BotMessage
from CwIRC import IRCResponse, ResponseType
from MBUtilities import Settings
from Function import Function

class Instantiate(Function):
  Help = "say <text> - makes the bot say the specified text"

  def GetResponse(self, message):
    if message.Command == "say":
      return IRCResponse(ResponseType.Say, message.Parameters, message.ReplyTo)
    else:
      return
```

You may wish to use Python's regular expressions module 're' to do more advanced matching on messages and commands.
Feel free to ask me about this if you have any problems! :D

## Reference
Now here's a complete reference to all the non-python things that you have available to you.

The format is:
```
from <library>					<-- How you import it
  import <module/class/whatever>		<-/
    <instance/constructor/static class>		<-- How you access it in your code
      <data/functions/parameter descriptions>	<-- What you can do with it/how it is used
```
### MoronBot Function Interface
Defines the BotMessage class, which is the class that is passed to your Functions.
```
from MBFunctionInterface
  import BotMessage
    message - read-only object holding the message you want to do stuff based on
      .MessageString (string)
        - the message text as it appears in an IRC client
      .Command (string)
        - the command part of the message, if it has one (from '|say text', it would contain 'say')
      .Parameters (string)
        - .MessageString with the .Command cut off the beggining
      .ParameterList (list of strings)
        - .Parameters split by spaces
      .User (struct)
        .Name (string)
          - the nick of the user who sent the message.
            This is what you should use instead of .ReplyTo if you want to send a PM
        .User (string)
          - the user part of the hostmask string (you *probably* don't want this)
        .Hostmask (string)
          - the 'hostmask' part of the hostmask string (you *probably* don't want this)
      .ReplyTo (string)
        - the channel (or user if it was a private message) that any responses should be sent back to
      .TargetType (enum)
        .CHANNEL
          - the message is from a channel
        .USER
          - the message is from a user
      .CTCP (bool)
        - whether or not the message is a CTCP message
          (actions are, in addition to the things you'd normally think of like VERSION)
      .CTCPString (string)
        - the .MessageString with the CTCP character cut off
      .MessageList (list of strings)
        - the raw irc message, split by spaces (you *probably* don't want this)
```
### CwIRC (Communicate with IRC)
Contains the IRCResponse class, which is what Functions should return.
You can return none, 1, or as many as you like in a Python list.
```
from CwIRC
  import IRCResponse, ResponseType
    IRCResponse(type, text, target)
      type is one of:
        ResponseType.Say
          - send as a normal message
        ResponseType.Do
          - send as an action message
        ResponseType.Notice
          - send as a notice
        ResponseType.Raw
          - send as a raw irc command
            (you'll need to know the IRC protocol for this one, and target becomes useless)
      text is the (string) message that you wish to send
      target (string) is where the message will be sent
        (usually message.ReplyTo, or message.User.Name to PM the person who sent the message)
```
### MoronBot Utilities
A library containing bot information, channel information, and a Logger for error logging.
```
from MBUtilities
  import Settings - read-only bot settings/details that you may need
    Settings.Instance
      .Server (string)
        - the server the bot initially connected to
      .Channel (string)
        - the bot's 'home' channel
      .CurrentNick (string)
        - the bot's nickname
      .AppPath (string)
        - the path to the directory the bot's executable is running from
      .FunctionPath (string)
        - the path to the bot's function directory (AppPath + "Functions")
      .DataPath (string)
        - the path to the bot's data directory (AppPath + "Data")
      .LogPath (string)
        - the path to the bot's logs directory (AppPath + "logs")
      .ErrorFile (string)
        - the path to the bot's error log (handy for debug output too) (LogPath + "errors.txt")

  .Channel import ChannelList - functions to check channel and user modes
    ChannelList
      .ChannelHasMode(channel, mode)
        - checks if 'channel' (string) has the mode 'mode' (single character)
      .UserIsAnyOp(nick, channel)
        - checks if 'nick' is any kind of op in 'channel'
      .UserIsFounder(nick, channel)
      .UserIsSop(nick, channel)
      .UserIsOp(nick, channel)
      .UserIsHop(nick, channel)
      .UserIsVoiced(nick, channel)

  import Logger - for logging strings to text files
    Logger
      .Write(text, filepath)
        - appends 'text' to the file at 'filepath'
          used with ErrorFile from Settings.Instance usually, eg;
            Logger.Write("my debug message", Settings.Instance.ErrorFile)
```