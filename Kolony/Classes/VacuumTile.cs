using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Charybdis.Science;

namespace Kolony
{
    public class VacuumTile : Tile
    {
        public VacuumTile()
        {
            Material = Material.Vacuum;
            Temperature = TemperatureF.AbsoluteZero;
        }
    }
}
