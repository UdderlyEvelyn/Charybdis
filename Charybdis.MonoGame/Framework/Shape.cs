using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;

namespace Charybdis.MonoGame
{
    public abstract class Shape : Drawable2
    {
        public float? Radius { get; set; }

        private List<LineSegment> _segments = new List<LineSegment>();
        public List<LineSegment> Segments
        {
            get
            {
                return _segments;
            }
            set
            {
                Vertices = value.SelectMany(ls => ls.Points).ToList(); //If you edit the line segments and expect them to update the vertices list, this line will be a problem.
                _segments = value;
                Size = _calculateSize(); //Recalculate the size since we've changed the line segments.
            }
        }

        public List<Vec2> Vertices;

        public Func<float, Vec2> DirectionalBiasFunction;

        public Col4 Color;

        protected abstract _shapeDrawStrategy _drawStrategy { get; }

        public override void Draw(SpriteBatch spriteBatch, Vec2 offset)
        {
            if (!DrawMe)
                return; //Set to not draw, so abort the drawing process.
            Vec2 effectivePosition = Position + (Parent != null ? Parent.Position : Vec2.Zero) + offset;
            switch (_drawStrategy)
            {
                case _shapeDrawStrategy.Polygonal:
                    foreach (LineSegment seg in Segments)
                        spriteBatch.DrawLine(effectivePosition + seg.A, effectivePosition + seg.B, Color);
                    break;
                case _shapeDrawStrategy.Radial:
                    Vec2 _lastPoint = DirectionalBiasFunction(360);
                    for (int t = 0; t < 360; t++)
                        spriteBatch.DrawLine(effectivePosition + _lastPoint, effectivePosition + DirectionalBiasFunction(t), Color);
                    break;
            }
            if (DrawChildren)
                lock (Children)
                    foreach (Drawable2 d2 in Children)
                        d2.Draw(spriteBatch, offset);
        }

        protected enum _shapeDrawStrategy
        {
            Polygonal,
            Radial,
        }

        //Keep in mind that for polygonal shapes this is the size of a rectangle that encompasses all points, not precise bounds.
        private Vec2 _calculateSize()
        {
            switch (_drawStrategy)
            {
                case _shapeDrawStrategy.Polygonal:
                    return new Vec2(
                        Math.Max(
                            Segments.Max(s => s.A.X),
                            Segments.Max(s => s.B.X)
                            ) -
                        Math.Min(
                            Segments.Min(s => s.A.X),
                            Segments.Min(s => s.B.X)
                            ),
                        Math.Max(
                            Segments.Max(s => s.A.Y),
                            Segments.Max(s => s.B.Y)
                            ) -
                        Math.Min(
                            Segments.Min(s => s.A.Y),
                            Segments.Min(s => s.B.Y)
                            )
                        );
                case _shapeDrawStrategy.Radial:
                    return new Vec2(Radius.Value * 2);
                default:
                    return Vec2.Zero;
            }
        }
    }
}
