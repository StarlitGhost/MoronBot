#region File Information
/********************************************************************
    Name:		cwIRC
    Author:		Matthew Cox
    Created:	9/12/2009
    
    Purpose:	Manages connections to IRC servers,
                and the sending and receiving of data to them.
*********************************************************************/
#endregion File Information

using System.ComponentModel;
using System.IO;
using System.Net.Sockets;

namespace CwIRC
{
    public delegate void StringEventHandler(object sender, string text);
    public delegate void VoidEventHandler(object sender);

    /// <summary>
    /// Class to manage connections to IRC servers, and the sending and receiving of data to them.
    /// </summary>
    public class Interface
    {
        #region Singleton rubbish
        static Interface instance = null;
        static readonly object padlock = new object();

        public static Interface Instance
        {
            get 
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Interface();
                    }
                    return instance;
                }
            }
        }

        public static void Assign(Interface face)
        {
            lock (padlock)
            {
                instance = face;
            }
        }
        #endregion Singleton rubbish

        TcpClient connection;
        NetworkStream networkStream;
        StreamReader streamReader;
        StreamWriter streamWriter;

        BackgroundWorker worker;

        public event StringEventHandler MessageReceived;
        protected virtual void OnMessageReceived(string message)
        {
            if (MessageReceived != null)
                MessageReceived(this, message);
        }

        Interface()
        {
            worker = new BackgroundWorker();

            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.WorkerReportsProgress = true;

            //sqlite = new SQLiteConnection("Data Source=/Data/database.db");
        }

        ~Interface()
        {
            Disconnect();
        }

        /// <summary>
        /// Connects to the specified IRC server, on the specified port.
        /// </summary>
        /// <param name="p_server">The IRC server to connect to.</param>
        /// <param name="p_port">The port of the IRC server to connect to.</param>
        /// <returns>Whether or not the connection attempt was successful.</returns>
        public bool Connect(string p_server, int p_port)
        {
            try
            {
                connection = new TcpClient(p_server, p_port);
            }
            catch
            {
                return false;
            }
            networkStream = connection.GetStream();
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);

            worker.RunWorkerAsync();

            return true;
        }
        /// <summary>
        /// Disconnects from the IRC server that the current instance of this class is connected to.
        /// </summary>
        public void Disconnect()
        {
            streamReader.Close();
            streamWriter.Close();
            networkStream.Close();
            connection.Close();

            worker.CancelAsync();
        }

        /// <summary>
        /// Gets a message from the downstream IRC message buffer.
        /// </summary>
        /// <returns>The message that was fetched from the downstream IRC message buffer.</returns>
        public string GetData()
        {
            return streamReader.ReadLine();
        }

        /// <summary>
        /// Puts a message on the upstream IRC message buffer.
        /// </summary>
        /// <param name="p_string">The message to put on the upstream IRC message buffer.</param>
        public void SendData(string p_string)
        {
            streamWriter.WriteLine(p_string + "\r\n");
            streamWriter.Flush();
        }

        /// <summary>
        /// Sends the NICK message to the IRC server, with the specified desired nickname.
        /// </summary>
        /// <param name="p_name">The desired nickname.</param>
        public void NICK(string p_name)
        {
            SendData("NICK " + p_name);
        }
        /// <summary>
        /// Sends the USER message to the IRC server, with the specified name, host name, server name, and real name.
        /// </summary>
        /// <param name="p_name">The name to send.</param>
        /// <param name="p_hostname">The host name to send (usually ignored by the server).</param>
        /// <param name="p_servername">The server name to send (usually ignored by the server).</param>
        /// <param name="p_realname">The 'real' name to send.</param>
        public void USER(string p_name, string p_hostname, string p_servername, string p_realname)
        {
            SendData("USER " + p_name + " " + p_hostname + " " + p_servername + " :" + p_realname);
        }
        /// <summary>
        /// Sends the JOIN message to the IRC server, with the specified channel to join.
        /// </summary>
        /// <param name="p_channel">The channel to join.</param>
        public void JOIN(string p_channel)
        {
            if (!p_channel.StartsWith("#"))
                p_channel = "#" + p_channel;
            SendData("JOIN " + p_channel);
        }
        /// <summary>
        /// Sends the PART message to the IRC server, to leave the specified channel with the specified leaving message.
        /// </summary>
        /// <param name="p_channel">The channel to leave.</param>
        public void PART(string p_channel, string p_partMessage)
        {
            SendData("PART #" + p_channel + " :" + p_partMessage);
        }
        /// <summary>
        /// Sends the PRIVMSG message to the IRC server, with the specified message and target channel.
        /// </summary>
        /// <param name="p_message">The message to send.</param>
        /// <param name="p_target">The channel (or user) to send the message to.</param>
        public void PRIVMSG(string p_message, string p_target)
        {
            SendData("PRIVMSG " + p_target + " :" + p_message);
        }
        /// <summary>
        /// Sends the NOTICE message to the IRC server, with the specified message and target channel.
        /// </summary>
        /// <param name="p_message">The message to send.</param>
        /// <param name="p_target">The channel (or user) to send the message to.</param>
        public void NOTICE(string p_message, string p_target)
        {
            SendData("NOTICE " + p_target + " :" + p_message);
        }
        /// <summary>
        /// Sends the QUIT message to the IRC server, with the specified quit message.
        /// </summary>
        /// <param name="p_quitMessage">The quit message.</param>
        public void QUIT(string p_quitMessage)
        {
            SendData("QUIT :" + p_quitMessage);
        }

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
                while ((inMessage = GetData()) != null)
                {
                    worker.ReportProgress(0, inMessage);
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
            OnMessageReceived(e.UserState.ToString());
        }
    }
}