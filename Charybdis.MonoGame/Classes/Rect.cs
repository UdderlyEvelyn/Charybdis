using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Charybdis.MonoGame
{
    [Serializable]
    public class Rect : ISerializable
    {
        public Vec2 Center
        {
            get
            {
                return Position + Extent;
            }
        }

        public Vec2 Extent
        {
            get
            {
                return Size / 2;
            }
        }

        public Vec2 Min
        {
            get
            {
                return Position;
            }
        }

        public Vec2 Max
        {
            get
            {
                return Position + Size;
            }
        }

        public float W
        {
            get
            {
                return Size.X;
            }
        }

        public float H
        {
            get
            {
                return Size.Y;
            }
        }

        public float X
        {
            get
            {
                return Position.X;
            }
        }

        public float Y
        {
            get
            {
                return Position.Y;
            }
        }

        public Vec2 Position = new Vec2(0, 0);
        public Vec2 Size = new Vec2(1, 1);    

        public Rect(float x, float y, float width, float height)
        {
            Position.X = x;
            Position.Y = y;
            Size.X = width;
            Size.Y = height;
        }

        public Rect(Vec2 position, Vec2 size)
        {
            Position = position;
            Size = size;
        }

        public float Radius
        {
            get
            {
                return Size.Average();
            }
        }

        protected Rect(SerializationInfo info, StreamingContext ctxt)
        {
            Position = (Vec2)info.GetValue("Position", typeof(Vec2));
            Size = (Vec2)info.GetValue("Size", typeof(Vec2));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Position", Position);
            info.AddValue("Size", Size);
        }

        public static Rect FromMinMax(Vec2 min, Vec2 max)
        {
            return new Rect(min, max - min);
        }

        public bool Contains(Vec2 v)
        {
            return ((v.X <= Max.X && v.X >= Min.X) && (v.Y <= Max.Y && v.Y >= Min.Y));
        }
    }
}
