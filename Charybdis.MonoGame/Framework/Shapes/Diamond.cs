using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.MonoGame
{
    public class Diamond : Shape
    {
        protected override _shapeDrawStrategy _drawStrategy
        {
            get
            {
                return _shapeDrawStrategy.Polygonal;
            }
        }

        public Diamond(float size)
        {
            float halfSize = size / 2;
            Vec2 A = Vec2.ZP * halfSize; //Left
            Vec2 B = Vec2.PZ * halfSize; //Top
            Vec2 C = new Vec2(size, halfSize); //Right
            Vec2 D = new Vec2(halfSize, size); //Bottom
            Segments = new List<LineSegment>
            {
                new LineSegment(A, B), //Top Left
                new LineSegment(B, C), //Top Right
                new LineSegment(C, D), //Bottom Right
                new LineSegment(D, A), //Bottom Left
            };
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                