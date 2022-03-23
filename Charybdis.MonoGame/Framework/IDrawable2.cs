using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;

namespace Charybdis.MonoGame
{
    public interface IDrawable2
    {
        void Draw(SpriteBatch spriteBatch, Vec2 offset);
    }
}
