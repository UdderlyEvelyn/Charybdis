using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.ARPG
{
    public abstract class DamageType
    {
        public abstract bool OverTime { get; }
        public abstract List<DamageType> AssociatedDamageTypes { get; }
        public abstract string Name { get; }
        public abstract List<CharybdisEffect<ARPGObject>> Effects { get; }
    }
}