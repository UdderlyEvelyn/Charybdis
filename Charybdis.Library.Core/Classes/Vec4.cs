using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace Charybdis.Library.Core
{
    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct Vec4 : ISerializable
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public static readonly Vec4 Zero = new Vec4(0);
        public static readonly Vec4 One = new Vec4(1);

        public Vec4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vec4(Vec3 v, float w)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }

        public Vec4(float f)
        {
            X = f;
            Y = f;
            Z = f;
            W = f;
        }

        private Vec4(SerializationInfo info, StreamingContext ctxt)
        {
            X = info.GetSingle("X");
            Y = info.GetSingle("Y");
            Z = info.GetSingle("Z");
            W = info.GetSingle("W");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Z", Z);
            info.AddValue("W", W);
        }

        public Vec4 Floor()
        {
            return new Vec4((float)Math.Floor(X), (float)Math.Floor(Y), (float)Math.Floor(Z), (float)Math.Floor(W));
        }

        public Vec4 Round()
        {
            return new Vec4((float)Math.Round(X, MidpointRounding.AwayFromZero), (float)Math.Round(Y, MidpointRounding.AwayFromZero), (float)Math.Round(Z, MidpointRounding.AwayFromZero), (float)Math.Round(W, MidpointRounding.AwayFromZero));
        }

        public Vec4 Ceiling()
        {
            return new Vec4((float)Math.Ceiling(X), (float)Math.Ceiling(Y), (float)Math.Ceiling(Z), (float)Math.Ceiling(W));
        }

        public static float Dot(Vec4 v, Vec4 v2)
        {
            return v.X * v2.X + v.Y * v2.Y + v.Z * v2.Z + v.W * v2.W;
        }

        public Vec4 SwapPairs(Vec4 v)
        {
            return new Vec4(v.Z, v.W, v.X, v.Y);
        }

        public Vec4 SwapPairs()
        {
            return new Vec4(Z, W, X, Y);
        }

        public float Dot(Vec4 v)
        {
            return Dot(this, v);
        }

        public static float Dot(Vec4 v, Vec3 v2)
        {
            return v.X * v2.X + v.Y * v2.Y + v.Z * v2.Z + v.W;
        }

        public float Dot(Vec3 v)
        {
            return Dot(this, v);
        }

        public static Vec4 operator &(Vec4 v, uint u)
        {
            return new Vec4((uint)v.X & u, (uint)v.Y & u, (uint)v.Z & u, (uint)v.W & u);
        }

        public static Vec4 ToVec4(Vec3 v)
        {
            return new Vec4(v.X, v.Y, v.Z, 1);
        }

        public Vec3 ToVec3()
        {
            return new Vec3(X, Y, Z);
        }

        public static Vec4 operator *(Vec4 v1, Vec4 v2)
        {
            return new Vec4(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
        }

        public static Vec4 operator +(Vec4 v1, Vec4 v2)
        {
            return new Vec4(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
        }

        public static Vec4 operator -(Vec4 v1, Vec4 v2)
        {
            return new Vec4(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
        }

        public static Vec4 operator *(Vec4 v, float f)
        {
            return new Vec4(v.X * f, v.Y * f, v.Z * f, v.W * f);
        }

        public static Vec4 operator +(Vec4 v, float f)
        {
            return new Vec4(v.X + f, v.Y + f, v.Z + f, v.W + f);
        }

        public static Vec4 operator -(Vec4 v, float f)
        {
            return new Vec4(v.X - f, v.Y - f, v.Z - f, v.W - f);
        }

        public static Vec4 operator /(Vec4 v, float f)
        {
            return new Vec4(v.X / f, v.Y / f, v.Z / f, v.W / f);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec4)
                return this == (Vec4)obj;
            else return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
        }
        public static bool operator !=(Vec4 v, Vec4 v2)
        {
            return !(v == v2);
        }
        public static bool operator ==(Vec4 v, Vec4 v2)
        {
            if (v.X != v2.X) return false;
            if (v.Y != v2.Y) return false;
            if (v.Z != v2.Z) return false;
            if (v.W != v2.W) return false;
            return true;
        }
    }
}
