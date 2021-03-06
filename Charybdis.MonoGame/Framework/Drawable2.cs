using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Charybdis.MonoGame
{
    public abstract class Drawable2 : IDrawable2
    {
        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Vec2.Zero);
        }
        public abstract void Draw(SpriteBatch spriteBatch, Vec2 offset);
        public Vec2 Position;
        public Vec2 Size;
        public bool DrawChildren = true;
        public List<Drawable2> Children = new List<Drawable2>();
        public Drawable2 Parent;
        public bool DrawMe = true;
    }
}