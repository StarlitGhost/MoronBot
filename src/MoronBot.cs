#region File Information
/********************************************************************
    Name:		MoronBot
    Author:		Matthew Cox
    Created:	9/12/2009
    
    Purpose:	The main class for MoronBot.
*********************************************************************/
#endregion File Information

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using CwIRC;

namespace MoronBot
{
    /// <summary>
    /// Class to hold MoronBot's details and behaviours.
    /// </summary>
    class MoronBot
    {
        #region Variables

        BackgroundWorker worker;
        CwIRC.Interface cwIRC;

        string desiredNick, nick;
        /// <summary>
        /// Nickname of the Bot
        /// </summary>
        public string Nick
        {
            get
            {
                return nick;
            }
        }
        int nickUsedCount = 0;

        public struct Channel 
        {
            public string Name;
            public List<string> Users;
        }
        List<Channel> channels;
        public List<Channel> Channels
        {
            get { return channels; }
        }

        List<Functions.Function> functions = new List<Functions.Function>();
        List<Functions.Function> userListFunctions = new List<Functions.Function>();
        List<Functions.Function> regexFunctions = new List<Functions.Function>();
        List<Functions.Function> commandFunctions = new List<Functions.Function>();

        List<string> commandList = new List<string>();
        public List<string> CommandList
        {
            get { return commandList; }
        }
        Dictionary<string, string> helpLibrary = new Dictionary<string, string>();
        public Dictionary<string, string> HelpLibrary
        {
            get { return helpLibrary; }
        }

        public List<IRCResponse> MessageQueue = new List<IRCResponse>();

        #endregion Variables

        #region Constructor & Destructor
        /// <summary>
        /// Constructor for MoronBot.
        /// This is where: 
        ///  - The server-listening process starts (BackgroundWorker).
        ///  - All of the Bot's Functions are loaded (so add new ones in here).
        ///  - Settings is initialized from xml.
        ///  - The initial connection to the server is done.
        /// </summary>
        public MoronBot()
        {
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.WorkerReportsProgress = true;

            // Bot Functions
            functions.Add(new Functions.Bot.Join(this));
            functions.Add(new Functions.Bot.Leave(this));
            functions.Add(new Functions.Bot.Nick(this));
            functions.Add(new Functions.Bot.Ignore(this)); functions.Add(new Functions.Bot.Unignore(this));
            functions.Add(new Functions.Bot.Say(this));

            // Automatic Functions
            functions.Add(new Functions.Automatic.Conversation(this));
            functions.Add(new Functions.Automatic.KrozeStalker(this));
            functions.Add(new Functions.Automatic.RandomKicker(this));
            functions.Add(new Functions.Automatic.URLFollow(this));

            // Internet Functions
            functions.Add(new Functions.Internet.Bitly(this));
            functions.Add(new Functions.Internet.Google(this));
            functions.Add(new Functions.Internet.NowPlaying(this)); functions.Add(new Functions.Internet.NowPlayingRegister(this));
            functions.Add(new Functions.Internet.RSSChecker(this));
            functions.Add(new Functions.Internet.Translate(this));

            // GitHub Functions
            functions.Add(new Functions.GitHub.LastCommit(this));
            functions.Add(new Functions.GitHub.Source(this));

            // Fun Functions
            functions.Add(new Functions.Fun.Dice(this));
            functions.Add(new Functions.Fun.MM(this));
            functions.Add(new Functions.Fun.Welch(this));

            // Utility Functions
            functions.Add(new Functions.Utility.Countdown(this)); functions.Add(new Functions.Utility.AddEvent(this)); functions.Add(new Functions.Utility.RemoveEvent(this)); functions.Add(new Functions.Utility.Upcoming(this));
            functions.Add(new Functions.Utility.Log(this));
            functions.Add(new Functions.Utility.Tell(this)); functions.Add(new Functions.Utility.TellAuto(this));
            functions.Add(new Functions.Utility.Time(this));

            functions.Add(new Functions.Commands(this));

            foreach (Functions.Function f in functions)
            {
                commandList.Add(f.Name);
                helpLibrary.Add(f.Name, f.Help);
                switch (f.Type)
                {
                    case Functions.Types.Command:
                        commandFunctions.Add(f);
                        break;
                    case Functions.Types.Regex:
                        regexFunctions.Add(f);
                        break;
                    case Functions.Types.UserList:
                        userListFunctions.Add(f);
                        break;
                }
            }

            if (!LoadXML("settings.xml"))
            {
                SaveXML("settings.xml");
            }
            desiredNick = Settings.Instance.Nick;
            nick = Settings.Instance.Nick;
            Settings.Instance.CurrentNick = nick;

            channels = new List<Channel>();

            cwIRC = new CwIRC.Interface();
            cwIRC.Connect(Settings.Instance.Server, Settings.Instance.Port);
            cwIRC.NICK(desiredNick);
            cwIRC.USER(desiredNick, "Meh", "Whatever", "MoronBot 0.1.6");
            cwIRC.SendData("PASS mOrOnBoTuS");

            cwIRC.JOIN(Settings.Instance.Channel);

            worker.RunWorkerAsync();
        }
        /// <summary>
        /// Destructor for MoronBot.
        /// Closes the connection to the IRC Server.
        /// </summary>
        ~MoronBot()
        {
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
            Program.form.txtIRC_Update(p_target + " <" + desiredNick + "> " + p_message);
            cwIRC.PRIVMSG(p_message, p_target);
        }
        /// <summary>
        /// Sends the specified notice to the specified channel or user (Sends the NOTICE message).
        /// </summary>
        /// <param name="p_message">The notice to send.</param>
        /// <param name="p_target">The channel or user to send the notice to.</param>
        public void Notice(string p_message, string p_target)
        {
            Program.form.txtIRC_Update(p_target + " [" + desiredNick + "] " + p_message);
            cwIRC.NOTICE(p_message, p_target);
        }
        /// <summary>
        /// Sends the specified 'action' message to the specified channel or user (Sends the PRIVMSG message, with ctcp ACTION command).
        /// </summary>
        /// <param name="p_action">The ACTION message to send.</param>
        /// <param name="p_target">The channel or user to send the ACTION message to.</param>
        public void Do(string p_action, string p_target)
        {
            Program.form.txtIRC_Update(p_target + " " + desiredNick + " " + p_action);
            char ctcpChar = Convert.ToChar((byte)1);
            cwIRC.PRIVMSG(ctcpChar + "ACTION " + p_action + ctcpChar, p_target);
        }

        /// <summary>
        /// Sends the given IRCResponse to the server, using the method specified by the IRCResponse's ResponseType.
        /// </summary>
        /// <param name="response">The IRCResponse to send to the server.</param>
        /// <returns>Whether or not the send was successful (actually whether or not the given response is valid).</returns>
        public bool Send(IRCResponse response)
        {
            if (response != null)
            {
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
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sends all IRCResponses in the message queue to the server.
        /// </summary>
        void SendQueue()
        {
            if (MessageQueue.Count > 0)
            {
                foreach (IRCResponse response in MessageQueue)
                {
                    Send(response);
                    System.Threading.Thread.Sleep(1700);
                }
                MessageQueue.Clear();
            }
        }
        #endregion Basic Operations

        /// <summary>
        /// Processes messages from the server. Most of the main 'bot' functions are in here.
        /// NOTE: Should probably be split off into separate modules, for easier modification.
        /// FURTHER-NOTE: Have now split off all of the bot's main functions (the ones listed by |commands), should maybe continue with the rest.
        /// </summary>
        /// <param name="p_message">The message received from the server.</param>
        public void ProcessMessage(BotMessage message)
        {
            if (message.Type == "PING")
            {
                cwIRC.SendData("PONG " + message.MessageString);
            }
            else
            {
                if (message.Type != "PRIVMSG")
                {
                    string parameter = message.MessageList[2].TrimStart(':');
                    
                    // Server full, connect to another
                    if (message.Type == "010")
                    {
                        cwIRC.Connect(message.MessageList[3], Int32.Parse(message.MessageList[4]));
                        cwIRC.NICK(desiredNick);
                        cwIRC.USER(desiredNick, "Meh", "Whatever", "MoronBot 0.1.6");
                    }

                    // User List
                    if (message.Type == "353")
                    {
                        Channel newChannel = new Channel();
                        newChannel.Name = message.MessageList[4];
                        List<string> Users = new List<string>();
                        for (int i = 5; i < message.MessageList.Count; i++)
                        {
                            if (message.MessageList[i].Length > 0)
                            {
                                Users.Add(message.MessageList[i].TrimStart(':'));
                            }
                        }
                        Users.Sort();
                        newChannel.Users = Users;
                        channels.Add(newChannel);
                        Program.form.RefreshListBox();
                    }

                    // Nick accepted?
                    if (message.Type == "376")
                    {
                        cwIRC.JOIN(Settings.Instance.Channel);
                    }

                    // Nick In Use
                    if (message.Type == "433")
                    {
                        nickUsedCount++;
                        cwIRC.NICK(desiredNick + nickUsedCount);
                        return;
                    }

                    #region Messages in Channel

                    #region Nick Change
                    if (message.Type == "NICK")
                    {
                        Program.form.txtIRC_Update(message.User.Name + " is now known as " + parameter);
                        if (message.User.Name == nick)
                        {
                            nick = parameter;
                            Settings.Instance.CurrentNick = nick;
                            Program.form.Text = nick;
                        }
                        Program.form.RefreshListBox();
                        return;
                    }
                    #endregion Nick Change

                    #region Join
                    if (message.Type == "JOIN")
                    {
                        if (message.User.Name != nick)
                        {
                            for (int i = 0; i < channels.Count; i++)
                            {
                                if (channels[i].Name == message.MessageList[2].TrimStart(':'))
                                {
                                    if (!channels[i].Users.Contains(message.User.Name))
                                        channels[i].Users.Add(message.User.Name);
                                }
                            }
                            Program.form.RefreshListBox();
                        }
                        Program.form.txtIRC_Update(message.User.Name + " joined " + parameter);
                        return;
                    }
                    #endregion Join

                    #region Leave
                    if (message.Type == "PART")
                    {
                        if (message.User.Name == nick)
                        {
                            for (int i = channels.Count - 1; i >= 0 ; i--)
                            {
                                if (channels[i].Name == parameter)
                                {
                                    channels.RemoveAt(i);
                                    //Program.form.RefreshListBox();
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < channels.Count ; i++)
                            {
                                if (channels[i].Name == message.MessageList[2].TrimStart(':'))
                                {
                                    channels[i].Users.Remove(message.User.Name);
                                }
                            }
                        }
                        Program.form.txtIRC_Update(message.User.Name + " left " + parameter + " message: " +  String.Join(" ", message.MessageList.ToArray(), 3, message.MessageList.Count - 3).TrimStart(':'));
                        return;
                    }
                    #endregion Leave

                    #region Quit
                    if (message.Type == "QUIT")
                    {
                        for (int i = 0; i < channels.Count; i++)
                        {
                            if (channels[i].Name == message.MessageList[2].TrimStart(':'))
                            {
                                channels[i].Users.Remove(message.User.Name);
                            }
                        }
                        Program.form.txtIRC_Update(message.User.Name + " quit, message: " + String.Join(" ", message.MessageList.ToArray(), 2, message.MessageList.Count - 2));
                    }
                    #endregion Quit

                    if (message.Type == "KICK")
                    {
                        if (message.MessageList[3] == nick)
                        {
                            for (int i = channels.Count - 1; i >= 0; i--)
                            {
                                if (channels[i].Name == parameter)
                                {
                                    channels.RemoveAt(i);
                                    //Program.form.RefreshListBox();
                                }
                            }
                            cwIRC.JOIN(message.MessageList[2]);
                        }
                    }

                    #endregion Messages in Channel
                }
                else
                {
                    Program.form.txtIRC_Update(message.ReplyTo + " <" + message.User.Name + "> " + message.MessageString);

                    ExecuteFunctionList(userListFunctions, message);
                    SendQueue();

                    if (Settings.Instance.IgnoreList.Contains(message.User.Name))
                        return;

                    ExecuteFunctionList(regexFunctions, message);
                    SendQueue();

                    #region Bot Commands
                    Match match = Regex.Match(message.MessageString, "^(\\||" + nick + "(,|:)?[ ])", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        ExecuteFunctionList(commandFunctions, message);
                        SendQueue();

                        if (Regex.IsMatch(message.Command, "^(pass)$", RegexOptions.IgnoreCase))
                        {
                            cwIRC.SendData("PASS mOrOnBoTuS");
                        }
                    }
                    #endregion Bot Commands

                    SendQueue();
                }
            }
        }

        /// <summary>
        /// Executes all of the functions in a given list, passing the given message to all of them
        /// Also sends any responses that any of them generate.
        /// </summary>
        /// <param name="funcList">The list of functions (List<Functions.Function>) to execute.</param>
        /// <param name="message">The BotMessage to pass to each function.</param>
        /// <returns>Whether or not any of the functions generated IRCResponses.</returns>
        bool ExecuteFunctionList(List<Functions.Function> funcList, BotMessage message)
        {
            foreach (Functions.Function f in funcList)
            {
                switch (f.AccessLevel)
                {
                    case Functions.AccessLevels.Anyone:
                        f.GetResponse(message, this);
                        break;
                    case Functions.AccessLevels.UserList:
                        if (f.AccessList.Contains(message.User.Name))
                        {
                            f.GetResponse(message, this);
                        }
                        break;
                }
            }
            return false;
        }

        #region Background Worker
        /// <summary>
        /// Gets new messages from the IRC server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string inMessage;
            while (true)
            {
                while ((inMessage = cwIRC.GetData()) != null)
                {
                    worker.ReportProgress(0, inMessage);
                    Console.WriteLine(inMessage);
                }
            }
        }
        /// <summary>
        /// Called when a new message is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BotMessage message = new BotMessage(e.UserState.ToString());
            Program.form.txtProgLog_Update(message.ToString());
            ProcessMessage(message);
        }
        /// <summary>
        /// Called when the worker is finished... should possibly remove, can't imagine a situation where it'd be needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        #endregion Background Worker

        #region settings.xml
        /// <summary>
        /// Loads settings into a Settings object from an XML file
        /// </summary>
        /// <param name="p_fileLocation">The location of the settings file to load from</param>
        /// <param name="p_settings">The Settings object to load the settings into</param>
        /// <returns>true if load succeeded, false if it failed</returns>
        public bool LoadXML(string fileLocation)
        {
            if (File.Exists(fileLocation))
            {
                XmlSerializer deserializer = new XmlSerializer(Settings.Instance.GetType());
                StreamReader streamReader = new StreamReader(fileLocation);
                Settings.Assign((Settings)deserializer.Deserialize(streamReader));
                streamReader.Close();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves settings to an XML file from a Settings object
        /// </summary>
        /// <param name="p_fileLocation">The location of the settings file to save to</param>
        /// <param name="settings">The Settings object to save the settings from</param>
        public void SaveXML(string fileLocation)
        {
            XmlSerializer serializer = new XmlSerializer(Settings.Instance.GetType());
            StreamWriter streamWriter = new StreamWriter(fileLocation);
            serializer.Serialize(streamWriter, Settings.Instance);
            streamWriter.Close();
        }
        #endregion settings.xml
    }
}