using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Charybdis.MonoGame
{
    public abstract class GameObject2 : GameObject
    {
        /// <summary>
        /// An object used for collision detection.
        /// </summary>
        public BoundingRect BoundingRect;

        /// <summary>
        /// Visual component of this object.
        /// </summary>
        public Drawable2 Visual;

        /// <summary>
        /// Position of this object in the game world.
        /// </summary>
        public Vec2 Position
        {
            get
            {
                if (Visual != null)
                    return Visual.Position;
                else
                    return Vec2.NN;
            }
            set
            {
                if (Visual != null)
                    Visual.Position = value;
            }
        }

        /// <summary>
        /// Update method for this game object.
        /// </summary>
        public abstract void Update();
    }
}