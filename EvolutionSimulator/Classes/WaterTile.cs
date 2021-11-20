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
    public class WaterTile : Tile
    {
        public override double MovementCostMultiplier
        {
            get
            {
                return 2;
            }
        }

        public override double MovementDivisor
        {
            get
            {
                return 1.25;
            }
        }

        public WaterTile(float size, Col4 color, byte height) : base(size, color)
        {
            Height = height;
            ((Square)Visual).FillColor = new Col3(0, 6.6f, (float)(Maths.Clamp((float)Height / (float)Globals.Sealevel * 135, 0, 50) + Math.Sin(Updates / 16) * 5));
        }

        public override void Update()
        {
            base.Update();
            ((Square)Visual).FillColor = new Col3(0, 6.6f, (float)(Maths.Clamp((float)Height / (float)Globals.Sealevel * 135, 0, 50) + Math.Sin(Updates / (ulong)Math.Max(1, Globals.SimulationFramesPerSecond)) * 5));
        }
    }
}
