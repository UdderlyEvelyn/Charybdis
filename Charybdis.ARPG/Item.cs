using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.ARPG
{
    public abstract class Item : ARPGObject
    {
        public class RequirementCollection : Dictionary<string, uint>, IDictionary<string, uint>
        {
            public uint this[string statName, params StatModifier[] modifiers]
            {
                get
                {
                    var bonus = modifiers.Where(sm => sm.TargetName == statName && sm.Amount != 0).Sum(sm => (sm.IsNegative ? -sm.Amount : sm.Amount));
                    var bonusPercentage = modifiers.Where(sm => sm.TargetName == statName && sm.Percentage != 0).Sum(sm => sm.Percentage);
                    var originalValue = base[statName];
                    var modifiedValue = (uint)(originalValue + bonus);
                    modifiedValue += (uint)Math.Round(modifiedValue * bonusPercentage);
                    return modifiedValue;
                }
            }

            public uint this[string statName, IEnumerable<StatModifier> modifiers]
            {
                get
                {
                    var bonus = modifiers.Where(sm => sm.TargetName == statName && sm.Amount != 0).Sum(sm => (sm.IsNegative ? -sm.Amount : sm.Amount));
                    var bonusPercentage = modifiers.Where(sm => sm.TargetName == statName && sm.Percentage != 0).Sum(sm => sm.Percentage);
                    var originalValue = base[statName];
                    var modifiedValue = (uint)(originalValue + bonus);
                    modifiedValue += (uint)Math.Round(modifiedValue * bonusPercentage);
                    return modifiedValue;
                }
            }
        }

        public bool CheckRequirements(ARPGObject ao, bool ignoreStatsNotPresent)
        {
            foreach (var requirement in Requirements)
            {
                if (ao.Stats.ContainsKey(requirement.Key))
                {
                    if (ao.GetStat(requirement.Key) < requirement.Value)
                        return false;
                }
                else if (!ignoreStatsNotPresent)
                    return false;
            }
            return true;
        }

        public RequirementCollection Requirements;
        public abstract EquipSlot EquipSlot { get; set; }
    }
}
