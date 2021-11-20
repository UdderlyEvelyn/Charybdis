using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Charybdis.Science;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.MonoGame;

namespace Fortress
{
    public abstract class FortressObject : GameObject2
    {
        /// <summary>
        /// The temperature of the object.
        /// </summary>
        public Temperature Temperature;

        /// <summary>
        /// The mass of the object.
        /// </summary>
        public double Mass;

        /// <summary>
        /// The velocity of the object in three dimensions.
        /// </summary>
        public Vec2 Velocity;
    }
}