using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Kolony
{
    public abstract class Creature : DestroyableObject
    {
        public Pathing2<Cube> Pathing = new Pathing2<Cube>();
        public Action<Cube> TileInteraction = null;

        public Creature(Array2<Cube> world, Vec2 startingPosition, Func<Cube, bool> passabilityCheck)
        {
            Pathing.PassabilityCheck = passabilityCheck;
            Pathing.World = world;
        }

        public override void Update()
        {
            UpdateCount++;
        }

        public ulong UpdateCount = 0;
    }
}