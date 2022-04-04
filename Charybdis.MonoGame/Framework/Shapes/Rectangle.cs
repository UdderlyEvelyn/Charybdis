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
    public class Rectangle : Shape
    {
        protected override _shapeDrawStrategy _drawStrategy
        {
            get
            {
                return _shapeDrawStrategy.Polygonal;
            }
        }

        public Col4? FillColor;

        public Rectangle(float width, float height) : this(new Vec2(width, height)) { }

        public Rectangle(Vec2 size)
        {
            Resize(size);
        }

        public void Resize(Vec2 size)
        {
            Size = size;
            Vec2 A = Vectors.Vec2.ZZ; //Top Left
            Vec2 B = Vectors.Vec2.PZ * size.X; //Top Right
            Vec2 C = size; //Bottom Right
            Vec2 D = Vectors.Vec2.ZP * size.Y; //Bottom Left
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
            spriteBatch.FillRectangle(new Rect(effectivePosition + Vec2.One, Size - Vec2.One), FillColor.Value);
            base.Draw(spriteBatch, offset);
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        