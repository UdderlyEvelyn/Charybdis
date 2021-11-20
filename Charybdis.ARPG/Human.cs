using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.ARPG
{
    public class Human : ARPGObject
    {
        public Human()
        {
            Stats = new StatCollection
            {
                { "Strength", new Stat { Name = "Strength Stat", Description = "How strong you are, lets you hold more/bigger things and use them better, too.", Owner = this } },
                { "Dexterity", new Stat { Name = "Dexterity Stat", Description = "How nimble you are, lets you do things faster and do more delicate things in general.", Owner = this } },
                { "Intelligence", new Stat { Name = "Intelligence Stat", Description = "Hao smart u am. But really, this is how smart you are, pretty straightforward.", Owner = this } },
                { "Willpower", new Stat { Name = "Willpower Stat", Description = "Your ability to implement your desires, changing the world or making things manifest.", Owner = this } },
            };
            Vitals = new VitalCollection
            {
                { "Health", new Vital { Name = "Health Vital", Description = "You need this to not be dead.", Owner = this } },
                { "Energy", new Vital { Name = "Energy Vital", Description = "You need this to do stuff.", Owner = this } },
                { "Decay", new Vital { Name = "Decay Vital", Description = "How decayed your body is, if it's completely decayed you'll be dead.. or.. dead-er..", Owner = this } }, //Zombie/"corruption" mechanic? ;3
            };
        }
    }
}
