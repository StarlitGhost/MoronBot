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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;
using MBUtilities.Channel;

using System.Windows.Forms;

namespace MoronBot
{
    /// <summary>
    /// Class to hold MoronBot's details and behaviours.
    /// </summary>
    public class MoronBot
    {
        #region Variables

        BackgroundWorker worker;
        CwIRC.Interface cwIRC;

        string desiredNick;
        /// <summary>
        /// Nickname of the Bot
        /// </summary>
        public string Nick
        {
            get
            {
                return Settings.Instance.CurrentNick;
            }
            set 
            {
                Program.form.Text = value;
                Settings.Instance.CurrentNick = value;
            }
        }
        int nickUsedCount = 0;

        public List<IFunction> UserListFunctions = new List<IFunction>();
        public List<IFunction> RegexFunctions = new List<IFunction>();
        public List<IFunction> CommandFunctions = new List<IFunction>();

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

            LoadFunctions();

            PluginLoader.WatchDirectory(Settings.Instance.FunctionPath, FuncDirChanged);

            if (!LoadXML("settings.xml"))
            {
                SaveXML("settings.xml");
            }
            desiredNick = Settings.Instance.Nick;
            Nick = desiredNick;

            cwIRC = CwIRC.Interface.Instance;
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
            Log("<" + desiredNick + "> " + p_message, p_target);
            cwIRC.PRIVMSG(p_message, p_target);
        }
        /// <summary>
        /// Sends the specified notice to the specified channel or user (Sends the NOTICE message).
        /// </summary>
        /// <param name="p_message">The notice to send.</param>
        /// <param name="p_target">The channel or user to send the notice to.</param>
        public void Notice(string p_message, string p_target)
        {
            Log("[" + desiredNick + "] " + p_message, p_target);
            cwIRC.NOTICE(p_message, p_target);
        }
        /// <summary>
        /// Sends the specified 'action' message to the specified channel or user (Sends the PRIVMSG message, with ctcp ACTION command).
        /// </summary>
        /// <param name="p_action">The ACTION message to send.</param>
        /// <param name="p_target">The channel or user to send the ACTION message to.</param>
        public void Do(string p_action, string p_target)
        {
            Log("*" + desiredNick + " " + p_action + "*", p_target);
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

        void Log(string data, string fileName)
        {
            DateTime date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "GMT Standard Time");

            string timeData = date.ToString(@"[HH:mm] ") + data;
            Program.form.txtIRC_Update(fileName + " " + timeData);

            string fileDate = date.ToString(@" yyyy-MM-dd");
            Logger.Write(timeData, @".\logs\" + Settings.Instance.Server + fileDate + @"\" + fileName + @".txt");
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
                return;
            }

            string parameter = message.MessageList[2].TrimStart(':');

            string logText = "";

            switch (message.Type)
            {
                case "010": // Server full, connect to another
                    cwIRC.Connect(message.MessageList[3], Int32.Parse(message.MessageList[4]));
                    cwIRC.NICK(desiredNick);
                    cwIRC.USER(desiredNick, "Meh", "Whatever", "MoronBot 0.1.6");
                    break;
                case "324": // Channel modes
                    ChannelList.Parse324(message);
                    break;
                case "332": // Current Topic
                    ChannelList.Parse332(message);
                    break;
                case "333": // Who set the current topic, and when
                    break;
                case "352":
                    ChannelList.Parse352(message);
                    break;
                case "353": // User List
                    ChannelList.Parse353(message);
                    Program.form.RefreshListBox();
                    break;
                case "376": // End of MOTD (Used as 'Nick Accepted')
                    Nick = message.MessageList[2];
                    cwIRC.JOIN(Settings.Instance.Channel);
                    break;
                case "433": // Nick In Use
                    nickUsedCount++;
                    Nick = desiredNick + nickUsedCount;
                    cwIRC.NICK(desiredNick + nickUsedCount);
                    break;
                case "NICK":
                    if (message.User.Name == Nick)
                    {
                        Nick = parameter;
                    }
                    Program.form.RefreshListBox();
                    Log(message.User.Name + " is now known as " + parameter, parameter);
                    break;
                case "JOIN":
                    ChannelList.ParseJOIN(message);

                    if (message.User.Name == Nick)
                    {
                        cwIRC.SendData("MODE " + message.MessageList[2].TrimStart(':'));
                    }

                    cwIRC.SendData("WHO " + message.MessageList[2].TrimStart(':'));

                    Log(message.User.Name + " joined " + parameter, parameter);
                    break;
                case "PART":
                    ChannelList.ParsePART(message);

                    if (message.User.Name == Nick)
                    {
                    }

                    logText = message.User.Name + " left " + parameter + " message: " + String.Join(" ", message.MessageList.ToArray(), 3, message.MessageList.Count - 3).TrimStart(':');

                    Log(logText, parameter);
                    break;
                case "QUIT":
                    ChannelList.ParseQUIT(message);

                    logText = message.User.Name + " quit, message: " + String.Join(" ", message.MessageList.ToArray(), 2, message.MessageList.Count - 2);

                    //Log(logText, parameter);
                    break;
                case "KICK":
                    if (message.MessageList[3] == Nick)
                    {
                        cwIRC.JOIN(message.MessageList[2]);
                    }

                    logText = message.User.Name + " kicked " + message.MessageList[3];

                    Log(logText, parameter);
                    break;
                case "MODE":
                    string setter = message.User.Name.TrimStart(':');
                    string modes = message.MessageList[3].TrimStart(':');
                    string targets = "";
                    if (message.MessageList.Count > 4)
                    {
                        for (int i = 4; i < message.MessageList.Count; ++i)
                        {
                            if (i < message.MessageList.Count - 1)
                            {
                                targets += message.MessageList[i] + " ";
                            }
                            else
                            {
                                targets += message.MessageList[i];
                            }
                        }
                    }
                    else
                    {
                        targets = message.MessageList[2];
                    }
                    string channel = message.MessageList[2];
                    Log("# " + setter + " set mode: " + modes + " " + targets, channel);
                    break;
                case "TOPIC":
                    Log("# " + message.User.Name + " changed the topic to: " + message.MessageString, message.ReplyTo);
                    break;
                case "NOTICE":
                case "001":
                case "002":
                case "003":
                case "004":
                case "005":
                case "251":
                case "252":
                case "253":
                case "254":
                case "255":
                case "265":
                case "266":
                case "375":
                case "372":
                case "366":
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

                        if (Regex.IsMatch(message.Command, "^(pass)$", RegexOptions.IgnoreCase))
                        {
                            cwIRC.SendData("PASS mOrOnBoTuS");
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
            foreach (IFunction f in funcList)
            {
                List<IRCResponse> responses = null;
                switch (f.AccessLevel)
                {
                    case AccessLevels.Anyone:
                        responses = f.GetResponse(message);
                        break;
                    case AccessLevels.UserList:
                        if (f.AccessList.Contains(message.User.Name))
                        {
                            responses = f.GetResponse(message);
                        }
                        break;
                }
                if (responses != null && responses.Count > 0)
                {
                    MessageQueue.AddRange(responses);
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
            BotMessage message = new BotMessage(e.UserState.ToString(), Nick);
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

        void LoadFunctions()
        {
            List<IFunction> functions = new List<IFunction>();

            functions.AddRange(PluginLoader.GetPlugins<IFunction>(Settings.Instance.FunctionPath));

            functions.Add(new Functions.Commands());

            foreach (IFunction f in functions)
            {
                if (!commandList.Contains(f.Name))
                {
                    commandList.Add(f.Name);
                    helpLibrary.Add(f.Name, f.Help);
                    switch (f.Type)
                    {
                        case Types.Command:
                            CommandFunctions.Add(f);
                            break;
                        case Types.Regex:
                            RegexFunctions.Add(f);
                            break;
                        case Types.UserList:
                            UserListFunctions.Add(f);
                            break;
                    }
                }
            }
        }

        void FuncDirChanged(object sender, FileSystemEventArgs e)
        {
            System.Threading.Thread.Sleep(500);
            LoadFunctions();
        }
    }
}