using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace Charybdis.Library.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2
    {
        public float X;
        public float Y;

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vec2(float f)
        {
            X = f;
            Y = f;
        }

        public Vec2(SerializationInfo info, StreamingContext ctxt)
        {
            X = info.GetSingle("X");
            Y = info.GetSingle("Y");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("X", X);
            info.AddValue("Y", Y);
        }
        
        public override string ToString()
        {
            return X.ToString() + ", " + Y.ToString();
        }

        public Vec2 Flip()
        {
            return new Vec2(Y, X);
        }

        public float Distance(Vec2 v)
        {
            return Math.Abs(X - v.X) + Math.Abs(Y - v.Y);
        }

        public Vec2 AngularMovement(double angle, float distance)
        {
            var rads = Maths.DegToRad(angle);
            return this + (new Vec2((float)Math.Cos(rads), (float)Math.Sin(rads)).Normalize() * distance);
        }

        public Vec2 Project(Vec2 v, float distance)
        {
            return Project(this, v, distance);
        }

        public static Vec2 Project(Vec2 origin, Vec2 target, float distance)
        {
            var delta = target - origin;
            return Project(origin, delta, distance);
        }

        public static Vec2 Project(Vec2 origin, float deltaX, float deltaY, float distance)
        {
            return origin + new Vec2(deltaX, deltaY).Normalize() * distance;
        }

        public static readonly Vec2 UnitX = new Vec2(1, 0);
        public static readonly Vec2 UnitY = new Vec2(0, 1);

        public static readonly Vec2 Left = new Vec2(-1, 0);
        public static readonly Vec2 Right = new Vec2(1, 0);
        public static readonly Vec2 Up = new Vec2(0, -1);
        public static readonly Vec2 Down = new Vec2(0, 1);

        public static readonly Vec2 Zero = new Vec2(0, 0);
        public static readonly Vec2 One = new Vec2(1, 1);

        public static readonly Vec2 NN = new Vec2(-1, -1);
        public static readonly Vec2 NP = new Vec2(-1, 1);
        public static readonly Vec2 PN = new Vec2(1, -1);
        public static readonly Vec2 PP = new Vec2(1, 1);

        public static readonly Vec2 ZN = new Vec2(0, -1);
        public static readonly Vec2 NZ = new Vec2(-1, 0);
        public static readonly Vec2 ZP = new Vec2(0, 1);
        public static readonly Vec2 PZ = new Vec2(1, 0);
        public static readonly Vec2 ZZ = new Vec2(0, 0);
        
        public int Xi
        {
            get
            {
                return (int)X;
            }
        }

        public int Yi
        {
            get
            {
                return (int)Y;
            }
        }

        public static bool operator <(Vec2 v, Vec2 v2)
        {
            return v.X + v.Y < v2.X + v2.Y;
        }

        public static bool operator >(Vec2 v, Vec2 v2)
        {
            return v.X + v.Y > v2.X + v2.Y;
        }

        public static Vec2 operator +(Vec2 v, Vec2 v2)
        {
            return new Vec2(v.X + v2.X, v.Y + v2.Y);
        }

        public static Vec2 operator -(Vec2 v, Vec2 v2)
        {
            return new Vec2(v.X - v2.X, v.Y - v2.Y);
        }

        public static bool operator ==(Vec2 v, Vec2 v2)
        {
            if (v.X != v2.X) return false;
            if (v.Y != v2.Y) return false;
            return true;
        }

        public static bool operator !=(Vec2 v, Vec2 v2)
        {
            return !(v == v2);
        }

        public static Vec2 operator *(Vec2 v, Vec2 v2)
        {
            return new Vec2(v.X * v2.X, v.Y * v2.Y);
        }

        public static Vec2 operator *(Vec2 v, int i)
        {
            return new Vec2(v.X * i, v.Y * i);
        }

        public static Vec2 operator /(Vec2 v, Vec2 v2)
        {
            return new Vec2(v.X / v2.X, v.Y / v2.Y);
        }

        public static Vec2 operator /(Vec2 v, int i)
        {
            return new Vec2(v.X / i, v.Y / i);
        }

        public static Vec2 operator -(Vec2 v, int i)
        {
            return new Vec2(v.X - i, v.Y - i);
        }

        public static Vec2 operator +(Vec2 v, int i)
        {
            return new Vec2(v.X + i, v.Y + i);
        }

        public static Vec2 operator *(Vec2 v, float i)
        {
            return new Vec2(v.X * i, v.Y * i);
        }

        public static Vec2 operator /(Vec2 v, float i)
        {
            return new Vec2(v.X / i, v.Y / i);
        }

        public static Vec2 operator -(Vec2 v, float i)
        {
            return new Vec2(v.X - i, v.Y - i);
        }

        public static Vec2 operator +(Vec2 v, float i)
        {
            return new Vec2(v.X + i, v.Y + i);
        }

        public static Vec2 operator *(Vec2 v, double i)
        {
            return new Vec2((float)(v.X * i), (float)(v.Y * i));
        }

        public static Vec2 operator /(Vec2 v, double i)
        {
            return new Vec2((float)(v.X / i), (float)(v.Y / i));
        }

        public static Vec2 operator -(Vec2 v, double i)
        {
            return new Vec2((float)(v.X - i), (float)(v.Y - i));
        }

        public static Vec2 operator +(Vec2 v, double i)
        {
            return new Vec2((float)(v.X + i), (float)(v.Y + i));
        }

        public static Vec2 operator -(Vec2 v)
        {
            return new Vec2(-v.X, -v.Y);
        }

        public Vec2 Floor()
        {
            return new Vec2((float)Math.Floor(X), (float)Math.Floor(Y));
        }

        public Vec2 Ceil()
        {
            return new Vec2((float)Math.Ceiling(X), (float)Math.Ceiling(Y));
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        public Vec2 Average(Vec2 v)
        {
            return new Vec2((X + v.X) / 2, (Y + v.Y) / 2);
        }

        public float Average()
        {
            return (X + Y) / 2;
        }

        public Vec2 Normalize()
        {
            double length = Length;
            return new Vec2((float)(this.X / length), (float)(this.Y / length));
        }

        public Vec2 Abs()
        {
            return new Vec2((float)Math.Abs(this.X), (float)Math.Abs(this.Y));
        }

        public static Vec2 Abs(Vec2 v)
        {
            return new Vec2((float)Math.Abs(v.X), (float)Math.Abs(v.Y));
        }

        public static float Dot(Vec2 v, Vec2 v2)
        {
            return v.X * v2.X + v.Y * v2.Y;
        }

        public float Dot(Vec2 v)
        {
            return Dot(this, v);
        }

        public static float Cross(Vec2 v, Vec2 v2)
        {
            return v.X * v2.Y - v.Y * v2.X;
        }

        public float Cross(Vec2 v)
        {
            return Cross(this, v);
        }

        public static Vec2 LeftOrtho(Vec2 v)
        {
            return new Vec2(v.Y, -v.X);
        }

        public Vec2 LeftOrtho()
        {
            return LeftOrtho(this);
        }

        public static Vec2 RightOrtho(Vec2 v)
        {
            return new Vec2(-v.Y, v.X);
        }

        public Vec2 RightOrtho()
        {
            return RightOrtho(this);
        }

        public Vec2 Project(Vec2 v)
        {
            float dot = this.Dot(v);
            float len = this.X * this.X + this.Y * this.Y;
            return new Vec2(dot / len * this.X, dot / len * this.Y);
        }

        public static Vec2 Project(Vec2 v, Vec2 axis)
        {
            return axis.Project(v);
        }

        public Vec2 UnitProject(Vec2 v)
        {
            //This check prevents mistakes using this, but it also kinda negates the point which is that this is faster in cases where it can be used.
            /*Vec2 n = this.Normalize().Abs();
            int valuedAxes = 0;
            if (n.X > 0) valuedAxes++;
            if (n.Y > 0) valuedAxes++;
            if (valuedAxes > 1) throw new ArgumentOutOfRangeException("This is not a unit vector.", (Exception)null);*/
            float dot = this.Dot(v);
            return new Vec2(dot * this.X, dot * this.Y);
        }

        public static Vec2 UnitProject(Vec2 v, Vec2 axis)
        {
            return axis.UnitProject(v);
        }

        public static Vec2 Normal(Vec2 v1, Vec2 v2)
        {
            Vec2 result = v2 - v1;
            return result.Normalize();
        }

        public Vec2 Normal(Vec2 v)
        {
            return Normal(this, v);
        }
       
        public override bool Equals(object obj)
        {
            if (obj is Vec2)
                return this == (Vec2)obj;
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
