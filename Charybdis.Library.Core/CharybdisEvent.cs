using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public abstract class CharybdisEvent<T> : CharybdisObject
        where T : CharybdisObject
    {
        protected List<CharybdisEffect<T>> _effects { get; set; }
    }
}
