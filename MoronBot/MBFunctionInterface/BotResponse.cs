namespace MBFunctionInterface
{
    public class BotResponse : CwIRC.IRCResponse
    {
        public BotResponse(CwIRC.ResponseType type, string response, string target) : base(type, response, target)
        {

        }
    }
}
