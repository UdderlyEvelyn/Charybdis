using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Charybdis.MonoGame
{
    public class Window : Rectangle
    {
        public Window(float width, float height, Col4 borderColor, Col4 fillColor)
            : base(width, height)
        {
            Color = borderColor;
            FillColor = fillColor;
        }

        public Window(Vec2 size, Col4 borderColor, Col4 fillColor)
            : base(size)
        {
            Color = borderColor;
            FillColor = fillColor;
        }
    }
}
