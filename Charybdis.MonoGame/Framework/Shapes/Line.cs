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
    public class Line : Shape
    {
        public Line(Vec2 a, Vec2 b)
        {
            Segments = new List<LineSegment> { new LineSegment(a, b) };
        }

        public Vec2 A
        {
            get
            {
                return Segments[0].A;
            }
            set
            {
                Segments[0].A = value;
            }
        }

        public Vec2 B
        {
            get
            {
                return Segments[0].B;
            }
            set
            {
                Segments[0].B = value;
            }
        }

        protected override _shapeDrawStrategy _drawStrategy
        {
            get
            {
                return _shapeDrawStrategy.Polygonal;
            }
        }
    }
}
