using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.ARPG
{
    public class ARPGObject : CharybdisObject
    {
        public DamageCollection Damage;
        public ResistCollection Resists;
        public StatCollection Stats;
        public VitalCollection Vitals;
        public List<Effect> CurrentEffects;
        public List<Effect> DissipatingEffects = new List<Effect>();

        public uint GetStat(string statName)
        {
            if (Stats.ContainsKey(statName))
                return Stats[statName, CurrentEffects.Select(e => e.Modifiers).OfType<StatModifier>().Where(m => m.TargetName == statName)];
            else
                return 0;
        }

        public uint Hurt(DamageType dt, uint amount, uint lifetime = 0)
        {
            if (Vitals.ContainsKey("Health"))
            {
                if (!dt.OverTime)
                {
                    float resistance = 0;
                    if (Resists != null && Resists.ContainsKey(dt)) resistance += Resists[dt];
                    uint damage = amount - (uint)Math.Round(amount * resistance, 0);
                    Vitals["Health"].Value.Current -= damage;
                    return damage;
                }
                else CurrentEffects.Add(new Effect { Description = dt.Name + " Damage Over Time", Owner = this, Type = EffectType.DamageOverTime, Lifetime = lifetime });
            }
            return 0;
        }

        public void Tick()
        {
            //Apply current effects, update lifetimes, and remove ones that have ended.
            foreach (Effect e in CurrentEffects)
            {
                if (e.RemainingLifetime > 0)
                {
                    if (e.Actions != null && e.Actions.Count() > 0)
                    {
                        foreach (var a in e.Actions)
                        {
                            a(this);
                        }
                    }
                    e.RemainingLifetime--;
                }
                else DissipatingEffects.Add(e);
            }

            if (DissipatingEffects.Count > 0)
            {
                foreach (Effect e in DissipatingEffects)
                {
                    CurrentEffects.Remove(e);
                }
                DissipatingEffects.Clear();
            }
        }

        public class StatCollection : Dictionary<string, Stat>, IDictionary<string, Stat>
        {
            public uint this[string statName, params StatModifier[] modifiers]
            {
                get
                {
                    var bonus = modifiers.Where(sm => sm.TargetName == statName && sm.Amount != 0).Sum(sm => (sm.IsNegative ? -sm.Amount : sm.Amount));
                    var bonusPercentage = modifiers.Where(sm => sm.TargetName == statName && sm.Percentage != 0).Sum(sm => sm.Percentage);
                    var originalValue = base[statName].Value;
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
                    var originalValue = base[statName].Value;
                    var modifiedValue = (uint)(originalValue + bonus);
                    modifiedValue += (uint)Math.Round(modifiedValue * bonusPercentage);
                    return modifiedValue;
                }
            }

            public uint GetBaseStat(string statName)
            {
                if (base.ContainsKey(statName))
                    return base[statName].Value;
                else
                    return 0;
            }

            public void ChangeBaseStat(string statName, uint newValue)
            {
                if (base.ContainsKey(statName))
                    base[statName].Value = newValue;
            }
        }
        public class VitalCollection : Dictionary<string, Vital>, IDictionary<string, Vital> { }
        public class DamageCollection : Dictionary<DamageType, uint>, IDictionary<DamageType, uint> { }
        public class ResistCollection : Dictionary<DamageType, uint>, IDictionary<DamageType, uint> { }
    }
}
