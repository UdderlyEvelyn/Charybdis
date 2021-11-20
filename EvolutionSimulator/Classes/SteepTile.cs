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
    public class SteepTile : Tile
    {
        public override double MovementCostMultiplier
        {
            get
            {
                return 8;
            }
        }

        public override double MovementDivisor
        {
            get
            {
                return 4;
            }
        }

        private float _randomRedHue = (float)(Globals.Random.NextDouble() * 20 + (Globals.MinimumSteepColor - 20));
        private float _randomBlueHue = (float)(Globals.Random.NextDouble() * 20 + (Globals.MinimumSteepColor - 20));

        public SteepTile(float size, Col4 color, byte height) : base(size, color)
        {
            Height = height;
        }

        public override void Update()
        {
            var steepSpaceHeight = Height - Globals.SteepHeightThreshold;
            var pctThroughSteepSpace = (double)steepSpaceHeight / (double)Globals.SteepSpace;
            var colorValue = (float)Math.Min((pctThroughSteepSpace - .15), 255);
            var r = _randomRedHue + Globals.RemainingSteepColorSpace * colorValue;
            var g = Globals.MinimumSteepColor + Globals.RemainingSteepColorSpace * colorValue;
            var b = _randomBlueHue + Globals.RemainingSteepColorSpace * colorValue;
            ((Square)Visual).FillColor = new Col3(r, g, b);
            base.Update();
        }
    }
}
