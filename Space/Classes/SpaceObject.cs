using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Charybdis.MonoGame;

namespace Space
{
    public abstract class SpaceObject : GameObject2
    {
        public double Mass { get; set; }
        /// <summary>
        /// The velocity of the object in two dimensions.
        /// </summary>
        public Vec2 Velocity { get; set; }
        /// <summary>
        /// The object that this object defines its position as relative to.
        /// </summary>
        public SpaceObject Anchor { get; set; }
        /// <summary>
        /// Creation date of this object (likely more useful as in-game-world time rather than real world, but it depends on the game).
        /// </summary>
        public DateTime Creation { get; set; }
    }
}
