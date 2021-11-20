using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class DataRetrievalException : CustomException
    {
        public override bool Display
        {
            get
            {
                return true; //Display Data Retrieval errors to the user.
            }
        }
        public DataRetrievalException(string message) : base(message) { }

        public DataRetrievalException(string message, Exception innerException) : base(message, innerException) { }
    }
}
