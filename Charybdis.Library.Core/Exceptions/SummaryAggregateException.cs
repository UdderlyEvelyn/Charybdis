using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class SummaryAggregateException : CustomException
    {
        internal SummaryAggregateException(string message, Exception innerException, bool fatal)
            : base(message, innerException)
        { }
    }
}
