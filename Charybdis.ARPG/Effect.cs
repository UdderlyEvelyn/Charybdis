using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.ARPG
{
    public class Effect : CharybdisEffect<ARPGObject>
    {
        public uint RemainingLifetime;
        public EffectType Type;

        public Effect()
        {
            RemainingLifetime = Lifetime;
        }
    }

    public enum EffectType
    {
        Resist,
        DamageOverTime,
    }
}
