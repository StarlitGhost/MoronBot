using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CwIRC;

namespace MBFunctionInterface
{
    public delegate void BotMessageEventhandler(object sender, BotMessage message);
    public delegate void StringEventHandler(object sender, string text);
    public delegate void VoidEventHandler(object sender);

    public static class FuncInterface
    {
        static Queue<BotResponse> messageQueue = new Queue<BotResponse>();
        /// <summary>
        /// A lock to synchronize message queue operations
        /// </summary>
        static readonly object queueSync = new object();

        public static event StringEventHandler NickChanged;
        public static void OnNickChanged(object sender, string nick)
        {
            if (NickChanged != null)
                NickChanged(sender, nick);
        }

        public static event BotMessageEventhandler AnyMessageReceived;
        public static void OnAnyMessageReceived(object sender, BotMessage message)
        {
            if (AnyMessageReceived != null)
                AnyMessageReceived(sender, message);
        }

        public static event BotMessageEventhandler PRIVMSGReceived;
        public static void OnPRIVMSGReceived(object sender, BotMessage message)
        {
            if (PRIVMSGReceived != null)
                PRIVMSGReceived(sender, message);
        }

        public static event BotMessageEventhandler NonPRIVMSGReceived;
        public static void OnNonPRIVMSGReceived(object sender, BotMessage message)
        {
            if (NonPRIVMSGReceived != null)
                NonPRIVMSGReceived(sender, message);
        }

        public static event BotMessageEventhandler CommandFormatMessageReceived;
        public static void OnCommandFormatMessageReceived(object sender, BotMessage message)
        {
            if (CommandFormatMessageReceived != null)
                CommandFormatMessageReceived(sender, message);
        }

        public static void SendResponse(ResponseType type, string response, string target)
        {
            SendResponse(new BotResponse(type, response, target));
        }

        public static void SendResponse(BotResponse response)
        {
            lock (queueSync)
            {
                messageQueue.Enqueue(response);
            }
        }

        public static void SendResponses(ResponseType type, List<string> responses, string target)
        {
            lock (queueSync)
            {
                foreach (string response in responses)
                {
                    messageQueue.Enqueue(new BotResponse(type, response, target));
                }
            }
        }

        public static void SendResponses(List<BotResponse> responses)
        {
            lock (queueSync)
            {
                foreach (var response in responses)
                    messageQueue.Enqueue(response);
            }
        }

        public static BotResponse GetResponse()
        {
            lock (queueSync)
            {
                if (messageQueue.Count > 0)
                {
                    BotResponse response = messageQueue.Dequeue();
                    if (response == null)
                        messageQueue.Clear();

                    return response;
                }
                else
                    return null;
            }
        }
    }
}
