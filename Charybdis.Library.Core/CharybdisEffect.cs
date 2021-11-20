using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public abstract class CharybdisEffect<T> : CharybdisObject
        where T : CharybdisObject
    {
        public uint Lifetime { get; set; }
        public virtual Action<T>[] Actions { get; set; }
        public virtual List<CharybdisModifier<T>> Modifiers { get; set; }
    }
}
