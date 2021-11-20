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
    public class Tile : GameObject2
    {
        public virtual double MovementCostMultiplier
        {
            get
            {
                return 1;
            }
        }
        public virtual double MovementDivisor
        {
            get
            {
                return 1;
            }
        }

        public byte Height { get; set; }

        private ulong _updates = 0;
        public ulong Updates
        {
            get
            {
                return _updates;
            }
        }

        public Tile(float size, Col4 color)
        {
            Visual = new Square(size) { Color = color };
        }

        public override void Update()
        {
            Visual.Position = Position;
            _updates++;
        }
    }
}
