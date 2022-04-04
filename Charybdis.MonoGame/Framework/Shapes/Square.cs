using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Charybdis.MonoGame
{
    public class Square : Shape
    {
        protected override _shapeDrawStrategy _drawStrategy
        {
            get
            {
                return _shapeDrawStrategy.Polygonal;
            }
        }

        public Col4? FillColor { get; set; }

        public Square(float size)
        {
            Vec2 A = Vectors.Vec2.ZZ; //Top Left
            Vec2 B = Vectors.Vec2.PZ * size; //Top Right
            Vec2 C = Vectors.Vec2.PP * size; //Bottom Right
            Vec2 D = Vectors.Vec2.ZP * size; //Bottom Left
            Segments = new List<LineSegment>
            {
                new LineSegment(A, B), //Top
                new LineSegment(B, C), //Right
                new LineSegment(C, D), //Bottom
                new LineSegment(D, A), //Left
            };
        }

        public override void Draw(SpriteBatch spriteBatch, Vec2 offset)
        {
            Vec2 effectivePosition = Position + (Parent != null ? Parent.Position : Vec2.Zero) + offset;
            if (FillColor.HasValue)
                spriteBatch.FillRectangle(new Rect(effectivePosition + Vec2.One, Size - Vec2.One), FillColor.Value);
            base.Draw(spriteBatch, offset);
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       