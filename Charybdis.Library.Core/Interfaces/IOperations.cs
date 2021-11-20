using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public interface IOperations<T>
        where T : class
    {
        Action<T> Update { get; }
        Action<T> Add { get; }
        Action<T> Delete { get; }
        Action Save { get; }
    }
}
