using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Fortress
{
    public abstract class Creature : DestroyableObject
    {
        public Pathing2<Tile> Pathing = new Pathing2<Tile>();
        public Action<Tile> TileInteraction = null;

        public Creature(Array2<Tile> world, Vec2 startingPosition, Func<Tile, bool> passabilityCheck)
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