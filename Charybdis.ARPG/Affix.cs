using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.ARPG
{
    public class Affix : CharybdisObject
    {
        /// <summary>
        /// Modifiers that affect the target's stats.
        /// </summary>
        List<StatModifier> TargetModifiers = new List<StatModifier>();
        /// <summary>
        /// Effects that apply to the target.
        /// </summary>
        List<Effect> TargetEffects = new List<Effect>();
        /// <summary>
        /// Modifiers that affect the wielder's stats (for items).
        /// </summary>
        List<StatModifier> Modifiers = new List<StatModifier>();
        /// <summary>
        /// Effects that apply to the wielder (for items).
        /// </summary>
        List<Effect> Effects = new List<Effect>();

        public AffixType Type;

        public enum AffixType
        {
            /// <summary>
            /// This affix can only be on its own.
            /// </summary>
            Solo = 0,
            /// <summary>
            /// This affix goes at the beginning of the name of the target.
            /// </summary>
            Prefix = 1,
            /// <summary>
            /// This affix goes at the end of the name of the target.
            /// </summary>
            Suffix = 2,
            /// <summary>
            /// This affix can go at the beginning or end of the name of the target.
            /// </summary>
            Both = 3,
        }
    }
}
