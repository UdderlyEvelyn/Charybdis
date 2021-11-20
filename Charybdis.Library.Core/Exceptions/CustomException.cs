using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// All custom exceptions should inherit from this. Only add things to this class directly if they should affect ALL custom exceptions.
    /// </summary>
    public abstract class CustomException : Exception
    {
        public CustomException(string message) : base(message) { }

        public CustomException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Provides a hint as to whether an error handler should display the message of this exception to the user or not (up to the error handler to take this into account or not).
        /// </summary>
        public virtual bool Display { get; protected set; }


        private bool _fatal = true;
        /// <summary>
        /// Indicates the severity of the error - the error handler can choose how to respond, but in most cases you'd want to clean up and shut down the application.
        /// </summary>
        public virtual bool Fatal 
        { 
            get
            {
                return _fatal;
            }
            protected set
            {
                _fatal = value;
            }
        }
    }
}
