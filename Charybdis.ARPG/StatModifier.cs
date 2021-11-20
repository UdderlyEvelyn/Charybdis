using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.ARPG
{
    public class StatModifier : CharybdisModifier<ARPGObject>
    {
        public string TargetName;
        public uint Amount;
        public float Percentage;
        public bool IsNegative;

        /*public override void Apply(ARPGObject target)
        {
            if (target.Stats.ContainsKey(TargetName))
            { 
                uint originalValue = target.Stats[TargetName];
                uint newValue = originalValue;
                if (Amount != 0)
                {
                    if (IsNegative)
                        newValue -= Amount;
                    else
                        newValue += Amount;
                }
                if (Percentage != 0)
                {
                    uint percentageAmount = (uint)Math.Round(Percentage * newValue, 0);
                    if (IsNegative)
                        newValue -= percentageAmount;
                    else
                        newValue += percentageAmount;                        
                }
            }
        }

        public override void Remove(ARPGObject target)
        {

        }*/
    }
}
