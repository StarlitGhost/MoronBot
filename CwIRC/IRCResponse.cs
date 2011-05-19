namespace CwIRC
{
    public enum ResponseType
    {
        Say,
        Do,
        Notice,
        Raw,

    }

    public class IRCResponse
    {
        ResponseType responseType;
        public ResponseType Type
        {
            get { return responseType; }
        }

        string responseString;
        public string Response
        {
            get { return responseString; }
        }

        string responseTarget;
        public string Target
        {
            get { return responseTarget; }
        }

        public IRCResponse(ResponseType type, string response, string target)
        {
            responseType = type;
            responseString = response;
            responseTarget = target;
        }
    }
}
