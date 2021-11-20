using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Science;
using Charybdis.Library.Core;
using Charybdis.MonoGame;

namespace EvolutionSimulator
{
    public class DeathTile : Tile
    {
        public override double MovementCostMultiplier
        {
            get
            {
                return 100;
            }
        }

        public override double MovementDivisor
        {
            get
            {
                return 100;
            }
        }

        public DeathTile(float size, Col4 color, byte height) : base(size, color)
        {
            Height = height;
            ((Square)Visual).FillColor = Col3.Black;
        }
    }
}
