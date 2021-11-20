using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortress
{
    public abstract class DestroyableObject : FortressObject
    {
        public double Health { get; set; }
    }
}
