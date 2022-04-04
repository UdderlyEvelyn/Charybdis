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
    public class Border : Shape
    {
        public Border(float width = 1, BorderType type = BorderType.AroundParent)
        {
            Width = width;
            Type = type;
        }

        public float Width = 1;
        public BorderType Type = BorderType.AroundParent;
        private Vec2? _oldParentPosition = null;

        protected override _shapeDrawStrategy _drawStrategy
        {
            get
            {
                return _shapeDrawStrategy.Polygonal;
            }
        }

        //This is overridden only because it needs to recalculate its segments on every draw if the parent (or children, depending on border type) has moved.
        public override void Draw(SpriteBatch spriteBatch, Vec2 offset)
        {
            if (!DrawMe) //This is also done in the base class, but this aborts it sooner avoiding more work.
                return; //Set to not draw, so abort the drawing process.
            switch (Type)
            {
                case BorderType.AroundParent:
                    if (Parent == null)
                        return; //ABORT, can't draw a border around a non-existent parent.
                    if (Parent.Position != _oldParentPosition)
                    {
                        _oldParentPosition = Parent.Position;
                        float left = -Width + offset.X;
                        float right = Parent.Size.X + Width + offset.X;
                        float top = -Width + offset.Y;
                        float bottom = Parent.Size.Y + Width + offset.Y;
                        Vec2 a = new Vec2(left, top);
                        Vec2 b = new Vec2(right, top);
                        Vec2 c = new Vec2(right, bottom);
                        Vec2 d = new Vec2(left, bottom);
                        Segments.Clear();
                        Segments.Add(new LineSegment(a, b));
                        Segments.Add(new LineSegment(b, c));
                        Segments.Add(new LineSegment(c, d));
                        Segments.Add(new LineSegment(d, a));
                    }
                    base.Draw(spriteBatch, offset);
                    break;
                case BorderType.AroundChildren:
                    throw new NotImplementedException("Implement this later!");
            }
        }
    }

    public enum BorderType
    {
        AroundParent,
        AroundChildren,
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            