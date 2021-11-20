using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;

namespace Charybdis.MonoGame
{
    public abstract class Drawable2
    {
        public abstract void Draw(SpriteBatch spriteBatch, Vec2 offset);
        public Vec2 Position;
        public Vec2 Size;
        public bool DrawChildren = true;
        public List<Drawable2> Children = new List<Drawable2>();
        public Drawable2 Parent;
        public bool DrawMe = true;
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              