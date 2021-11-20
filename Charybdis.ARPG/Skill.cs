using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.ARPG
{
    public class Skill<T>
        where T : ARPGObject
    {
        public string Name;
        public List<Effect> Effects = new List<Effect>();
    }
}
