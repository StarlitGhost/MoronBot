/********************************************************************
    Name:		MoronBot
    Author:		Matthew Cox
    Created:	9/12/2009
    
    Purpose:	The main class for MoronBot.
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;
using MBUtilities.Channel;

namespace MoronBot
{
    /// <summary>
    /// Class to hold MoronBot's details and behaviours.
    /// </summary>
    public class MoronBot
    {
        #region Variables

        /// <summary>
        /// The CwIRC Interface that is used to send and receive messages to and from an IRC server
        /// </summary>
        CwIRC.Interface cwIRC;

        /// <summary>
        /// Nickname of the Bot
        /// </summary>
        string Nick
        {
            get { return Settings.Instance.CurrentNick; }
            set
            {
                MBEvents.OnNickChanged(this, value);
                Settings.Instance.CurrentNick = value;
            }
        }
        /// <summary>
        /// Used to set the bot's nickname if the one in the settings file is already in use
        /// </summary>
        int nickUsedCount = 0;

        /// <summary>
        /// A List containing all of the Functions that are activated based on usernames
        /// </summary>
        internal List<IFunction> UserListFunctions = new List<IFunction>();
        /// <summary>
        /// A List containing all of the Functions that are activated based upon regex parsing of the message text
        /// </summary>
        internal List<IFunction> RegexFunctions = new List<IFunction>();
        /// <summary>
        /// A List containing all of the Functions that are activated based on the 'standard' command syntaxes '|(function)', 'botname function'
        /// </summary>
        internal List<IFunction> CommandFunctions = new List<IFunction>();

        /// <summary>
        /// A List of the names of Functions loaded
        /// </summary>
        List<string> commandList = new List<string>();
        public List<string> CommandList
        {
            get { return commandList; }
        }

        /// <summary>
        /// A List containing functions that have been unloaded
        /// </summary>
        List<IFunction> unloadedList = new List<IFunction>();

        /// <summary>
        /// A map of function names -> their associated help text
        /// </summary>
        Dictionary<string, string> helpLibrary = new Dictionary<string, string>();
        public Dictionary<string, string> HelpLibrary
        {
            get { return helpLibrary; }
        }

        /// <summary>
        /// A queue of messages to send to the IRC server
        /// </summary>
        List<IRCResponse> MessageQueue = new List<IRCResponse>();
        /// <summary>
        /// A lock to synchronize message queue operations
        /// </summary>
        readonly object queueSync = new object();
        /// <summary>
        /// A lock to synchronize message processing
        /// </summary>
        readonly object messageSync = new object();

        System.Timers.Timer connectionTimer = new System.Timers.Timer();

        #endregion Variables

        #region Constructor & Destructor
        /// <summary>
        /// Constructor for MoronBot.
        /// This is where: 
        ///  - All of the Bot's Functions are loaded.
        ///  - Settings is initialized from xml.
        ///  - The initial connection to the server is made.
        /// </summary>
        public MoronBot()
        {
            if (!LoadXML("settings.xml"))
            {
                SaveXML("settings.xml");
            }

            LoadFunctions();

            PluginLoader.WatchDirectory(Settings.Instance.FunctionPath, FuncDirChanged);

            Nick = Settings.Instance.Nick;

            cwIRC = CwIRC.Interface.Instance;

            cwIRC.MessageReceived += CwIRC_MessageReceived;
            cwIRC.Connect(Settings.Instance.Server, Settings.Instance.Port);
            cwIRC.NICK(Nick);
            cwIRC.USER(Nick, "Nope", "Whatever", "MoronBot 0.1.6");
            cwIRC.SendData("PASS mOrOnBoTuS");
            Say("identify mOrOnBoTuS", "NickServ");

            cwIRC.JOIN(Settings.Instance.Channel);

            connectionTimer.Elapsed += new System.Timers.ElapsedEventHandler(connectionTimer_Elapsed);
            connectionTimer.Interval = 120000;
            connectionTimer.Enabled = true;
        }
        /// <summary>
        /// Destructor for MoronBot.
        /// Closes the connection to the IRC Server.
        /// </summary>
        ~MoronBot()
        {
            SaveXML("settings.xml");
            cwIRC.Disconnect();
        }
        #endregion Constructor & Destructor

        #region Basic Operations
        
        /// <summary>
        /// Sends the specified message to the specified channel or user (Sends the PRIVMSG message).
        /// </summary>
        /// <param name="p_message">The message to send.</param>
        /// <param name="p_target">The channel or user to send the message to.</param>
        public void Say(string p_message, string p_target)
        {
            Log("<" + Nick + "> " + p_message, p_target);
            cwIRC.PRIVMSG(p_message, p_target);
        }
        /// <summary>
        /// Sends the specified notice to the specified channel or user (Sends the NOTICE message).
        /// </summary>
        /// <param name="p_message">The notice to send.</param>
        /// <param name="p_target">The channel or user to send the notice to.</param>
        public void Notice(string p_message, string p_target)
        {
            Log("[" + Nick + "] " + p_message, p_target);
            cwIRC.NOTICE(p_message, p_target);
        }
        /// <summary>
        /// Sends the specified 'action' message to the specified channel or user (Sends the PRIVMSG message, with ctcp ACTION command).
        /// </summary>
        /// <param name="p_action">The ACTION message to send.</param>
        /// <param name="p_target">The channel or user to send the ACTION message to.</param>
        public void Do(string p_action, string p_target)
        {
            Log("*" + Nick + " " + p_action + "*", p_target);
            char ctcpChar = Convert.ToChar((byte)1);
            cwIRC.PRIVMSG(ctcpChar + "ACTION " + p_action + ctcpChar, p_target);
        }

        /// <summary>
        /// Sends the given IRCResponse to the server, using the method specified by the IRCResponse's ResponseType.
        /// </summary>
        /// <param name="response">The IRCResponse to send to the server.</param>
        /// <returns>Whether or not the send was successful (actually whether or not the given response is valid).</returns>
        bool Send(IRCResponse response)
        {
            if (response == null)
                return false;

            switch (response.Type)
            {
                case ResponseType.Say:
                    Say(response.Response, response.Target);
                    break;
                case ResponseType.Do:
                    Do(response.Response, response.Target);
                    break;
                case ResponseType.Notice:
                    Notice(response.Response, response.Target);
                    break;
                case ResponseType.Raw:
                    cwIRC.SendData(response.Response);
                    break;
            }
            return true;
        }

        /// <summary>
        /// Sends all IRCResponses in the message queue to the server.
        /// </summary>
        void SendQueue()
        {
            if (MessageQueue.Count == 0)
                return;

            lock (queueSync)
            {
                foreach (IRCResponse response in MessageQueue)
                {
                    Send(response);
                    System.Threading.Thread.Sleep(100);
                }
                MessageQueue.Clear();
            }
        }

        void Log(string data, string fileName)
        {
            DateTime date = DateTime.Now.IsDaylightSavingTime() ? DateTime.UtcNow.AddHours(1.0) : DateTime.UtcNow;

            string timeData = date.ToString(@"[HH:mm] ") + data;
            MBEvents.OnNewFormattedIRC(this, fileName + " " + timeData);

            string fileDate = date.ToString(@" yyyy-MM-dd");
            string filePath = Path.Combine(Settings.Instance.LogPath,
                string.Format(Settings.Instance.Server + fileDate + @"{0}" + fileName + @".txt", Path.DirectorySeparatorChar));
            Logger.Write(timeData, filePath.ToLowerInvariant());
        }
        
        #endregion Basic Operations

        #region Message Processing
        
        /// <summary>
        /// Processes messages from the server. Most of the main 'bot' functions are in here.
        /// NOTE: Should probably be split off into separate modules, for easier modification.
        /// FURTHER-NOTE: Have now split off all of the bot's main functions (the ones listed by |commands), should maybe continue with the rest.
        /// </summary>
        /// <param name="p_message">The message received from the server.</param>
        void ProcessMessage(BotMessage message)
        {
            if (message.Type == "PING")
            {
                cwIRC.SendData("PONG " + message.MessageString);
                return;
            }

            string parameter = message.MessageList[2].TrimStart(':');

            string logText = "";

            switch (message.Type)
            {
                // First, a load of message types we don't care about
                case "NOTICE":
                case "001": // Welcome
                case "002": // Server you're on, IRC server software version
                case "003": // Server creation date
                case "004": // Server name, version, user modes, channel modes
                case "005": // Stuff supported by the server
                case "251": // Number of users on the servers
                case "252": // Number of OPs on the servers
                case "253": // Number of unregistered connections
                case "254": // Number of channels formed
                case "255": // Number of local connections, I think
                case "265": // Number of local users
                case "266": // Number of global users
                case "315": // End of WHO reply
                case "329": // Channel creation time
                case "333": // Who set the current topic, and when
                case "366": // End of NAMES reply
                case "372": // MOTD line
                case "375": // Start of MOTD
                case "451": // Bot not marked as registered yet
                case "474": // Bot banned from channel
                    break;
                case "010": // Server full, connect to another
                    cwIRC.Disconnect();
                    cwIRC.Connect(message.MessageList[3], Int32.Parse(message.MessageList[4]));
                    cwIRC.NICK(Nick);
                    cwIRC.USER(Nick, "Nope", "Whatever", "MoronBot 0.1.6");
                    break;
                case "324": // Channel modes
                    ChannelList.Parse324(message);
                    break;
                case "332": // Current topic
                    ChannelList.Parse332(message);
                    break;
                case "352": // WHO reply
                    ChannelList.Parse352(message);
                    break;
                case "353": // NAMES reply
                    ChannelList.Parse353(message);
                    break;
                case "376": // End of MOTD (Used as 'Nick Accepted')
                    Nick = message.MessageList[2];
                    cwIRC.JOIN(Settings.Instance.Channel);
                    break;
                case "433": // Nick In Use
                    nickUsedCount++;
                    Nick = Settings.Instance.Nick + nickUsedCount;
                    cwIRC.NICK(Nick);
                    break;
                case "NICK":
                    List<string> channels = ChannelList.ParseNICK(message);

                    if (message.User.Name == Nick)
                    {
                        Nick = parameter;
                    }

                    logText = message.User.Name + " is now known as " + parameter;
                    foreach (string chan in channels)
                        Log(logText, chan.ToLowerInvariant());
                    break;
                case "JOIN":
                    ChannelList.ParseJOIN(message);

                    if (message.User.Name == Nick)
                    {
                        cwIRC.SendData("MODE " + message.MessageList[2].TrimStart(':'));
                    }

                    //cwIRC.SendData("WHO " + message.MessageList[2].TrimStart(':'));
                    //cwIRC.SendData("NAMES " + message.MessageList[2].TrimStart(':'));

                    Log(" >> " + message.User.Name + " joined " + parameter, parameter.ToLowerInvariant());
                    break;
                case "PART":
                    ChannelList.ParsePART(message, message.User.Name == Nick);

                    string partMsg = (message.MessageList.Count > 3 ? ", message: " + String.Join(" ", message.MessageList.ToArray(), 3, message.MessageList.Count - 3).Substring(1) : "");
                    logText = " << " + message.User.Name + " left " + parameter + partMsg;

                    Log(logText, parameter.ToLowerInvariant());
                    break;
                case "QUIT":
                    List<string> quittedChannels = ChannelList.ParseQUIT(message);

                    string quitMsg = (message.MessageList.Count > 2 ? ", message: " + String.Join(" ", message.MessageList.ToArray(), 2, message.MessageList.Count - 2).Substring(1) : "");
                    logText = " << " + message.User.Name + " quit" + quitMsg;
                    foreach (string chan in quittedChannels)
                        Log(logText, chan.ToLowerInvariant());
                    break;
                case "KICK":
                    ChannelList.ParsePART(message, message.MessageList[3] == Nick);
                    if (message.MessageList[3] == Nick)
                    {
                        cwIRC.JOIN(message.MessageList[2]);
                    }

                    string kickMsg = (message.MessageList.Count > 4 ? ", message: " + String.Join(" ", message.MessageList.ToArray(), 4, message.MessageList.Count - 4).Substring(1) : "");
                    logText = "!<< " + message.User.Name + " kicked " + message.MessageList[3] + kickMsg;

                    Log(logText, parameter.ToLowerInvariant());
                    break;
                case "MODE":
                    ChannelList.ParseMODE(message);
                    
                    string setter = message.User.Name.TrimStart(':');
                    string modes = message.MessageList[3].TrimStart(':');
                    string targets = "";

                    string channel = message.MessageList[2].ToLowerInvariant();
                    if (channel.StartsWith("#"))
                    {
                        if (message.MessageList.Count > 4)
                        {
                            for (int i = 4; i < message.MessageList.Count; ++i)
                            {
                                if (i < message.MessageList.Count - 1)
                                    targets += message.MessageList[i] + " ";
                                else
                                    targets += message.MessageList[i];
                            }
                        }
                        else
                        {
                            targets = message.MessageList[2];
                        }
                    }

                    Log("# " + setter + " set mode: " + modes + " " + targets, channel.ToLowerInvariant());
                    break;
                case "TOPIC":
                    ChannelList.ParseTOPIC(message);

                    Log("# " + message.User.Name + " changed the topic to: " + message.MessageString, message.ReplyTo);
                    break;
                case "PRIVMSG": // User messages
                    char ctcpChar = Convert.ToChar((byte)1);
                    string action = ctcpChar + "ACTION ";
                    if (message.MessageString.StartsWith(action))
                    {
                        Log("*" + message.User.Name + " " + message.MessageString.Replace(action, "").TrimEnd(ctcpChar) + "*", message.ReplyTo);
                    }
                    else
                    {
                        Log("<" + message.User.Name + "> " + message.MessageString, message.ReplyTo);
                    }

                    ExecuteFunctionList(UserListFunctions, message);
                    SendQueue();

                    if (Settings.Instance.IgnoreList.Contains(message.User.Name.ToUpper()))
                        return;

                    ExecuteFunctionList(RegexFunctions, message);
                    SendQueue();

                    Match match = Regex.Match(message.MessageString, @"^(\||" + Nick + @"(,|:)?[ ])", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        ExecuteFunctionList(CommandFunctions, message);
                        SendQueue();

                        // Intrinsic functions
                        // These are here because they are either too linked with the bot to extract,
                        // or too simple to be worth making a Function dll for.
                        if (message.User.Name == Settings.Instance.Owner)
                        {
                            if (Regex.IsMatch(message.Command, "^(pass)$", RegexOptions.IgnoreCase))
                            {
                                cwIRC.SendData("PASS mOrOnBoTuS");
                            }
                            else if (Regex.IsMatch(message.Command, "^(unload)$", RegexOptions.IgnoreCase))
                            {
                                UnloadFunction(message);
                            }
                            else if (Regex.IsMatch(message.Command, "^(load)$", RegexOptions.IgnoreCase))
                            {
                                LoadFunction(message);
                            }
                        }
                    }

                    SendQueue();
                    break;
                default:
                    Log(message.RawMessage, "-unknown");
                    break;
            }
        }

        /// <summary>
        /// Executes all of the functions in a given list, passing the given message to all of them
        /// Also sends any responses that any of them generate.
        /// </summary>
        /// <param name="funcList">The list of functions (List<Functions.Function>) to execute.</param>
        /// <param name="message">The BotMessage to pass to each function.</param>
        /// <returns>Whether or not any of the functions generated IRCResponses.</returns>
        bool ExecuteFunctionList(List<IFunction> funcList, BotMessage message)
        {
            List<IRCResponse> responses = new List<IRCResponse>();

            foreach (IFunction f in funcList)
            {
                List<IRCResponse> funcResponses = null;
                try
                {
                    switch (f.AccessLevel)
                    {
                        case AccessLevels.Anyone:
                            funcResponses = f.GetResponse(message);
                            if (funcResponses != null)
                                responses.AddRange(funcResponses);
                            break;
                        case AccessLevels.UserList:
                            if (f.AccessList.Contains(message.User.Name.ToLowerInvariant()))
                            {
                                funcResponses = f.GetResponse(message);
                                if (funcResponses != null)
                                    responses.AddRange(funcResponses);
                            }
                            break;
                    }

                    // For the Apathy function
                    if (responses.Count > 0 && responses[responses.Count - 1] == null)
                        break;
                }
                catch (Exception e)
                {
                    Logger.Write(e.Message + "\n" + e.StackTrace, Settings.Instance.ErrorFile);
                }
            }

            if (responses.Count == 0)
                return false;

            lock (queueSync)
                MessageQueue.AddRange(responses);

            return true;
        }
        
        #endregion Message Processing

        #region IRC Message Receiver
        void CwIRC_MessageReceived(object sender, string message)
        {
            BotMessage botMessage = new BotMessage(message, Nick);
            connectionTimer.Interval = 120000;

            lock (messageSync)
            {
                MBEvents.OnNewRawIRC(this, botMessage.ToString());
                ProcessMessage(botMessage);
            }
        }

        void connectionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            cwIRC.Disconnect();

            cwIRC.Connect(Settings.Instance.Server, Settings.Instance.Port);
            cwIRC.NICK(Nick);
            cwIRC.USER(Nick, "Nope", "Whatever", "MoronBot 0.1.6");

            cwIRC.SendData("PASS mOrOnBoTuS");
            Say("identify mOrOnBoTuS", "NickServ");

            cwIRC.JOIN(Settings.Instance.Channel);
        }
        #endregion IRC Message Receiver

        #region Settings File
        
        /// <summary>
        /// Loads settings into a Settings object from an XML file
        /// </summary>
        /// <param name="p_fileLocation">The location of the settings file to load from</param>
        /// <param name="p_settings">The Settings object to load the settings into</param>
        /// <returns>true if load succeeded, false if it failed</returns>
        bool LoadXML(string fileLocation)
        {
            if (!File.Exists(fileLocation))
                return false;

            XmlSerializer deserializer = new XmlSerializer(Settings.Instance.GetType());
            StreamReader streamReader = new StreamReader(fileLocation);
            Settings.Assign((Settings)deserializer.Deserialize(streamReader));
            streamReader.Close();

            return true;
        }

        /// <summary>
        /// Saves settings to an XML file from a Settings object
        /// </summary>
        /// <param name="p_fileLocation">The location of the settings file to save to</param>
        /// <param name="settings">The Settings object to save the settings from</param>
        void SaveXML(string fileLocation)
        {
            XmlSerializer serializer = new XmlSerializer(Settings.Instance.GetType());
            StreamWriter streamWriter = new StreamWriter(fileLocation);
            serializer.Serialize(streamWriter, Settings.Instance);
            streamWriter.Close();
        }
        #endregion Settings File

        #region Function Loading
        
        void LoadFunctions()
        {
            CommandFunctions.Clear();
            RegexFunctions.Clear();
            UserListFunctions.Clear();

            List<IFunction> functions = new List<IFunction>();
            functions.AddRange(PluginLoader.GetPlugins<IFunction>(Settings.Instance.FunctionPath));

            functions.Add(new Functions.Commands());

            foreach (IFunction f in functions)
            {
                if (Settings.Instance.ExcludedFunctions.FindIndex(s => s.ToLowerInvariant() == f.Name.ToLowerInvariant()) >= 0)
                {
                    unloadedList.Add(f);
                    continue;
                }

                if (commandList.Contains(f.Name))
                    continue;

                LoadFunction(f);
            }
        }

        void FuncDirChanged(object sender, FileSystemEventArgs e)
        {
            //LoadFunctions();
        }

        void LoadFunction(IFunction func)
        {
            commandList.Add(func.Name);
            helpLibrary.Add(func.Name, func.Help);
            switch (func.Type)
            {
                case Types.Command:
                    CommandFunctions.Add(func);
                    break;
                case Types.Regex:
                    RegexFunctions.Add(func);
                    break;
                case Types.UserList:
                    UserListFunctions.Add(func);
                    break;
            }
        }

        void LoadFunction(BotMessage message)
        {
            if (message.ParameterList.Count > 0)
                foreach (string funcName in message.ParameterList)
                {
                    int index = unloadedList.FindIndex(f => f.Name.ToLowerInvariant() == funcName.ToLowerInvariant());
                    if (index >= 0)
                    {
                        LoadFunction(unloadedList[index]);
                        unloadedList.RemoveAt(index);
                        MessageQueue.Add(new IRCResponse(ResponseType.Say, "Function \"" + funcName + "\" loaded!", message.ReplyTo));
                    }
                    else
                    {
                        MessageQueue.Add(new IRCResponse(ResponseType.Say, "Function \"" + funcName + "\" not found!", message.ReplyTo));
                    }
                }
            else
                MessageQueue.Add(new IRCResponse(ResponseType.Say, "You didn't specify a function to load!", message.ReplyTo));
        }

        void UnloadFunction(BotMessage message)
        {
            if (message.ParameterList.Count > 0)
                foreach (string funcName in message.ParameterList)
                {
                    if (UnloadFromFunctionList(funcName, CommandFunctions) ||
                        UnloadFromFunctionList(funcName, RegexFunctions) ||
                        UnloadFromFunctionList(funcName, UserListFunctions))
                        MessageQueue.Add(new IRCResponse(ResponseType.Say, "Function \"" + funcName + "\" unloaded!", message.ReplyTo));
                    else
                        MessageQueue.Add(new IRCResponse(ResponseType.Say, "Function \"" + funcName + "\" not found!", message.ReplyTo));
                }
            else
                MessageQueue.Add(new IRCResponse(ResponseType.Say, "You didn't specify a function to unload!", message.ReplyTo));
        }

        bool UnloadFromFunctionList(string funcName, List<IFunction> funcList)
        {
            int index = funcList.FindIndex(cf => cf.Name.ToLowerInvariant() == funcName.ToLowerInvariant());
            if (index >= 0)
            {
                unloadedList.Add(funcList[index]);
                funcList.RemoveAt(index);
                string actualName = commandList.Find(s => s.IndexOf(funcName, StringComparison.InvariantCultureIgnoreCase) == 0);
                commandList.Remove(actualName);
                helpLibrary.Remove(actualName);
                return true;
            }
            return false;
        }
        #endregion Function Loading
    }
}
