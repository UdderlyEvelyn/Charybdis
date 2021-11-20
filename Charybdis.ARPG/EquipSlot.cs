using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.ARPG
{
    public class EquipSlot
    {
        public string Name { get; private set; }

        public static EquipSlot None = new EquipSlot { Name = "N/A" };
    }
}