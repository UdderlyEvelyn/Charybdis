using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Charybdis.MonoGame
{
    //Technically an equilateral triangle only.
    public class Triangle : Shape
    {
        protected override _shapeDrawStrategy _drawStrategy
        {
            get
            {
                return _shapeDrawStrategy.Polygonal;
            }
        }

        public Col4? FillColor { get; set; }

        public Triangle(float size)
        {
            Vec2 A = new Vec2(.5f * size, 0); //Top Middle
            Vec2 B = Vec2.PP * size; //Bottom Right
            Vec2 C = Vec2.ZP * size; //Bottom Left
            Segments = new List<LineSegment>
            {
                new LineSegment(A, B), //Right
                new LineSegment(B, C), //Bottom
                new LineSegment(C, A), //Left
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
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       