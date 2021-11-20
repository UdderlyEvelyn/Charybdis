using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class ClientException : CustomException
    {
        public override bool Display
        {
            get
            {
                return true; //ClientExceptions should always be displayed to the user as they indicate an issue with connectivity or authentication.
            }
        }

        public int StatusCode { get; protected set; }

        public ClientException(string message) : base(message) { }

        public ClientException(string message, Exception innerException) : base(message, innerException) { }

        public ClientException(string message, int statusCode) : base(message) { StatusCode = statusCode; }

        public ClientException(string message, Exception innerException, int statusCode) : base(message, innerException) { StatusCode = statusCode; }

        public static ClientException FromWrappedMessage(string wrappedMessage, string prefix = "", string suffix = "")
        {
            return new ClientException("A client exception has occurred.\n\n" + prefix + GetMessageString(wrappedMessage) + suffix, new Exception(wrappedMessage));
        }

        public static ClientException FromWrappedMessage(string wrappedMessage, Exception innerException, string prefix = "", string suffix = "")
        {
            return new ClientException("A client exception has occurred.\n\n" + prefix + GetMessageString(wrappedMessage) + suffix, new Exception(wrappedMessage, innerException));
        }

        public static ClientException FromWrappedMessage(string wrappedMessage, int statusCode, string prefix = "", string suffix = "")
        {
            string msg = GetMessageString(wrappedMessage);
            if (msg.Contains("☼DISPLAY")) //If it is explicitly flagged for display..
                return new ClientException("A client exception has occurred with status code " + statusCode + ".\n\n" + prefix + msg.Substring(8) + suffix, new Exception(wrappedMessage), statusCode);
            if (statusCode.ToString()[0] == '5') //If it's a 5xx error (server-side) without a display override magic string..
                return new ClientException("A client exception has occurred with status code " + statusCode + ".", new Exception(prefix + GetMessageString(wrappedMessage) + suffix, new Exception(wrappedMessage)), statusCode);
            else //It is non-5xx and not explicitly flagged for display.
                return new ClientException("A client exception has occurred with status code " + statusCode + ".\n\n" + prefix + msg + suffix, new Exception(wrappedMessage), statusCode);
        }

        public static ClientException FromWrappedMessage(string wrappedMessage, int statusCode, Exception innerException, string prefix = "", string suffix = "")
        {
            string msg = GetMessageString(wrappedMessage);
            if (msg.Contains("☼DISPLAY")) //If it is explicitly flagged for display..
                return new ClientException("A client exception has occurred with status code " + statusCode + ".\n\n" + prefix + msg.Substring(8) + suffix, new Exception(wrappedMessage, innerException), statusCode);
            if (statusCode.ToString()[0] == '5') //If it's a 5xx error (server-side) without a display override magic string..
                return new ClientException("A client exception has occurred with status code " + statusCode + ".", new Exception(prefix + GetMessageString(wrappedMessage) + suffix, new Exception(wrappedMessage, innerException)), statusCode);
            else //It is non-5xx and not explicitly flagged for display.
                return new ClientException("A client exception has occurred with status code " + statusCode + ".\n\n" + prefix + msg + suffix, new Exception(wrappedMessage, innerException), statusCode);
        }

        private static string GetMessageString(string wrappedMessage)
        {
            try
            {
                //Could still use some tweaking to remove "the statement has been terminated", and odd escape sequences.
                var tmp = wrappedMessage.Split("</m:message>");
                var tmp2 = tmp
                    .Where(s => s.Contains("<m:message xml:lang=\"en-US\">")) //Get messages with language specified..
                    .Select(s => s.Substring(s.IndexOf("<m:message xml:lang=\"en-US\">") + 28))
                    .Concat(tmp
                        .Where(s => s.Contains("<m:message>")) //Add messages with no language specified..
                        .Select(s => s.Substring(s.IndexOf("<m:message>") + 11)));
                var tmp3 = tmp2.Where(s => s.Contains("conflict occurred")); //Special case..
                if (tmp3.Count() > 0) //If special case..
                    return tmp3.Join('\n'); //Return list of matches.
                else
                    return tmp2.Join('\n'); //Return full list.
                    //return wrappedMessage.Between("<m:message xml:lang=\"en-US\">", "</m:message>"); //Old code, just grabs single message with language specified.
            }
            catch (Exception e)
            {
                e.ThrowIfNot<IndexOutOfRangeException, ArgumentException>();
                if (wrappedMessage.Contains("context has changed since the database was created")) //Special case..
                    return "☼DISPLAYThe model used by the service does not match the database you are trying to access."; //Friendly error.
                return "Unable to parse the message returned from the service - it was \"" + wrappedMessage + "\"";
            }
        }
    }
}
