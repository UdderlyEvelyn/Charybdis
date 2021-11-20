using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class AuthenticationException : CustomException
    {
        public override bool Display
        {
            get
            {
                return true; //Display Authentication errors to the user.
            }
        }

        public AuthenticationException(string message) : base(message) { }

        public AuthenticationException(string message, Exception innerException) : base(message, innerException) { }

        public AuthenticationException(string message, bool fatal)
            : base(message)
        {
            Fatal = fatal;
        }

        public AuthenticationException(string message, Exception innerException, bool fatal) 
            : base(message, innerException) 
        {
            Fatal = fatal;
        }
    }
}
