using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class FlattenedAggregateException : CustomException
    {
        internal FlattenedAggregateException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
