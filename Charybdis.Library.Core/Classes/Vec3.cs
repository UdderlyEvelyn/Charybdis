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
    public struct Vec3 : ISerializable
    {
        public float X;
        public float Y;
        public float Z;
        
        public static readonly Vec3 Up = new Vec3(0, 1, 0);
        public static readonly Vec3 Down = new Vec3(0, -1, 0);
        public static readonly Vec3 UnitX = new Vec3(1, 0, 0);
        public static readonly Vec3 UnitY = new Vec3(0, 1, 0);
        public static readonly Vec3 UnitZ = new Vec3(0, 0, 1);
        public static readonly Vec3 Zero = new Vec3(0, 0, 0);
        public static readonly Vec3 One = new Vec3(1, 1, 1);
        public static readonly Vec3 HHH = new Vec3(.5f, .5f, .5f);
        public static readonly Vec3 HZH = new Vec3(.5f, 0, .5f);

        public static readonly Vec3 ZZZ = new Vec3(0, 0, 0);
        public static readonly Vec3 ZZP = new Vec3(0, 0, 1);
        public static readonly Vec3 ZPZ = new Vec3(0, 1, 0);
        public static readonly Vec3 ZPP = new Vec3(0, 1, 1);
        public static readonly Vec3 PZZ = new Vec3(1, 0, 0);
        public static readonly Vec3 PZP = new Vec3(1, 0, 1);
        public static readonly Vec3 PPZ = new Vec3(1, 1, 0);
        public static readonly Vec3 PPP = new Vec3(1, 1, 1);
        public static readonly Vec3 ZZN = new Vec3(0, 0, -1);
        public static readonly Vec3 ZNZ = new Vec3(0, -1, 0);
        public static readonly Vec3 ZNN = new Vec3(0, 1, 1);
        public static readonly Vec3 NZZ = new Vec3(-1, 0, 0);
        public static readonly Vec3 NZN = new Vec3(-1, 0, -1);
        public static readonly Vec3 NNZ = new Vec3(-1, -1, 0);
        public static readonly Vec3 NNN = new Vec3(-1, -1, -1);
        public static readonly Vec3 NPN = new Vec3(-1, 1, -1);
        public static readonly Vec3 NPP = new Vec3(-1, 1, 1);
        public static readonly Vec3 PNP = new Vec3(1, -1, 1);
        public static readonly Vec3 PNN = new Vec3(1, -1, -1);
        public static readonly Vec3 ZPN = new Vec3(0, 1, -1);
        public static readonly Vec3 ZNP = new Vec3(0, -1, 1);
        public static readonly Vec3 PNZ = new Vec3(1, -1, 0);
        public static readonly Vec3 NPZ = new Vec3(-1, 1, 0);
        public static readonly Vec3 PZN = new Vec3(1, 0, -1);
        public static readonly Vec3 NZP = new Vec3(-1, 0, 1);
        public static readonly Vec3 PPN = new Vec3(1, 1, -1);
        public static readonly Vec3 NNP = new Vec3(-1, -1, 1);

        public static readonly Vec3 Front = new Vec3(0, 0, 1);
        public static readonly Vec3 Back = new Vec3(0, 0, -1);
        public static readonly Vec3 Top = new Vec3(0, 1, 0);
        public static readonly Vec3 Bottom = new Vec3(0, -1, 0);
        public static readonly Vec3 Left = new Vec3(-1, 0, 0);
        public static readonly Vec3 Right = new Vec3(1, 0, 0);

        /* 
            Vec3.ZZP, //front
            Vec3.ZZN, //back
            Vec3.ZPZ, //top
            Vec3.ZNZ, //bottom
            Vec3.NZZ, //left
            Vec3.PZZ, //right
        */
            
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

        public int Zi
        {
            get
            {
                return (int)Z;
            }
        }
        
        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3(float f)
        {
            X = f;
            Y = f;
            Z = f;
        }

        public Vec3(Vec2 v, float z)
        {
            X = v.X;
            Y = v.Y;
            Z = z;
        }

        private Vec3(SerializationInfo info, StreamingContext ctxt)
        {
            X = info.GetSingle("X");
            Y = info.GetSingle("Y");
            Z = info.GetSingle("Z");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Z", Z);
        }

        public static Vec3 Clamp(Vec3 v, float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            return new Vec3(Maths.Clamp(v.X, minX, maxX), Maths.Clamp(v.Y, minY, maxY), Maths.Clamp(v.Z, minZ, maxZ));
        }

        public Vec3 Clamp(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            return Clamp(this, minX, maxX, minY, maxY, minZ, maxZ);
        }

        public static float Dot(Vec3 v, Vec3 v2)
        {
            return v.X * v2.X + v.Y * v2.Y + v.Z * v2.Z;
        }

        public float Dot(Vec3 v)
        {
            return Dot(this, v);
        }

        public static Vec3 Cross(Vec3 v, Vec3 v2)
        {
            float x = v.Y * v2.Z - v.Z * v2.Y;
            float y = v.Z * v2.X - v.X * v2.Z;
            float z = v.X * v2.Y - v.Y * v2.X;
            return new Vec3(x, y, z);
        }

        public Vec3 Cross(Vec3 v)
        {
            return Cross(this, v);
        }

        public static float[] Unpack(Vec3 v)
        {
            return new float[3] { v.X, v.Y, v.Z };
        }

        public static float Distance(Vec3 v1, Vec3 v2)
        {
            float x = v1.X - v2.X;
            x *= x;
            float y = v1.Y - v2.Y;
            y *= y;
            float z = v1.Z - v2.Z;
            z *= z;
            return (float)Math.Sqrt(x + y + z);
        }

        public float Distance(Vec3 v)
        {
            float x = this.X - v.X;
            x *= x;
            float y = this.Y - v.Y;
            y *= y;
            float z = this.Z - v.Z;
            z *= z;
            return (float)Math.Sqrt(x + y + z);
        }

        public static float FastDistance(Vec3 v1, Vec3 v2)
        {
            float x = v1.X - v2.X;
            x *= x;
            float y = v1.Y - v2.Y;
            y *= y;
            float z = v1.Z - v2.Z;
            z *= z;
            return x + y + z;
        }

        public float FastDistance(Vec3 v)
        {
            float x = this.X - v.X;
            x *= x;
            float y = this.Y - v.Y;
            y *= y;
            float z = this.Z - v.Z;
            z *= z;
            return x + y + z;
        }

        public static float DistanceXZ(Vec3 v1, Vec3 v2)
        {
            float x = v1.X - v2.X;
            x *= x;
            float z = v1.Z - v2.Z;
            z *= z;
            return (float)Math.Sqrt(x + z);
        }

        public float DistanceXZ(Vec3 v)
        {
            float x = this.X - v.X;
            x *= x;
            float z = this.Z - v.Z;
            z *= z;
            return (float)Math.Sqrt(x + z);
        }

        public static float FastDistanceXZ(Vec3 v1, Vec3 v2)
        {
            float x = v1.X - v2.X;
            x *= x;
            float z = v1.Z - v2.Z;
            z *= z;
            return x + z;
        }

        public float FastDistanceXZ(Vec3 v)
        {
            float x = this.X - v.X;
            x *= x;
            float z = this.Z - v.Z;
            z *= z;
            return x + z;
        }

        public static float DistanceXZ(Vec3 v1, Vec2 v2)
        {
            float x = v1.X - v2.X;
            x *= x;
            float z = v1.Z - v2.Y;
            z *= z;
            return (float)Math.Sqrt(x + z);
        }

        public float DistanceXZ(Vec2 v)
        {
            float x = this.X - v.X;
            x *= x;
            float z = this.Z - v.Y;
            z *= z;
            return (float)Math.Sqrt(x + z);
        }

        public static float FastDistanceXZ(Vec3 v1, Vec2 v2)
        {
            float x = v1.X - v2.X;
            x *= x;
            float z = v1.Z - v2.Y;
            z *= z;
            return x + z;
        }

        public float FastDistanceXZ(Vec2 v)
        {
            float x = this.X - v.X;
            x *= x;
            float z = this.Z - v.Y;
            z *= z;
            return x + z;
        }

        public Vec3 Average(Vec3 v)
        {
            return new Vec3((this.X + v.X) / 2, (this.Y + v.Y) / 2, (this.Z + v.Z) / 2);
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            }
        }

        public Vec3 Abs()
        {
            return new Vec3((float)Math.Abs(this.X), (float)Math.Abs(this.Y), (float)Math.Abs(this.Z));
        }

        public Vec3 Normalize()
        {
            double length = Length;
            return new Vec3((float)(this.X / length), (float)(this.Y / length), (float)(this.Z / length));
        }

        public static void Normalize(Vec3 v)
        {
            double length = v.Length;
            v.X = (float)(v.X / length);
            v.Y = (float)(v.Y / length);
            v.Z = (float)(v.Z / length);
        }

        public float Average()
        {
            return (this.X + this.Y + this.Z) / 3;
        }

        public static Vec3 Normal(Vec3 v1, Vec3 v2)
        {
            Vec3 result = v2 - v1;
            return result.Normalize();
        }

        public Vec3 Normal(Vec3 v)
        {
            return Normal(this, v);
        }

        public static Vec3 Sign(Vec3 v)
        {
            return new Vec3(Math.Sign(v.X), Math.Sign(v.Y), Math.Sign(v.Z));
        }

        public Vec3 Sign()
        {
            return Sign(this);
        }

        public Vec3 Floor()
        {
            return new Vec3((float)Math.Floor(X), (float)Math.Floor(Y), (float)Math.Floor(Z));
        }

        public Vec3 Ceiling()
        {
            return new Vec3((float)Math.Ceiling(X), (float)Math.Ceiling(Y), (float)Math.Ceiling(Z));
        }

        public Vec3 Round(int decimals = 0)
        {
            return new Vec3((float)Math.Round(X, decimals), (float)Math.Round(Y, decimals), (float)Math.Round(Z, decimals));
        }

        public static Vec3 operator ^(Vec3 v1, Vec3 v2)
        {
            return new Vec3((short)v1.X ^ (short)v2.X, (short)v1.Y ^ (short)v2.Y, (short)v1.Z ^ (short)v2.Z);
        }

        /// <summary>
        /// Discards W component of v2.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vec3 operator ^(Vec3 v1, Vec4 v2)
        {
            return new Vec3((short)v1.X ^ (short)v2.X, (short)v1.Y ^ (short)v2.Y, (short)v1.Z ^ (short)v2.Z);
        }

        public static Vec3 operator ^(Vec4 v1, Vec3 v2)
        {
            return new Vec3((short)v1.X ^ (short)v2.X, (short)v1.Y ^ (short)v2.Y, (short)v1.Z ^ (short)v2.Z);
        }

        public static Vec3 operator -(Vec3 v)
        {
            return new Vec3(-v.X, -v.Y, -v.Z);
        }

        public static Vec3 operator -(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vec3 operator +(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vec3 operator *(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        public static Vec3 operator /(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        }

        public static Vec3 operator -(Vec3 v, float f)
        {
            return new Vec3(v.X - f, v.Y - f, v.Z - f);
        }

        public static Vec3 operator -(float f, Vec3 v)
        {
            return new Vec3(v.X - f, v.Y - f, v.Z - f);
        }

        public static Vec3 operator +(Vec3 v, float f)
        {
            return new Vec3(v.X + f, v.Y + f, v.Z + f);
        }

        public static Vec3 operator *(Vec3 v, float f)
        {
            return new Vec3(v.X * f, v.Y * f, v.Z * f);
        }

        public static Vec3 operator *(float f, Vec3 v)
        {
            return new Vec3(v.X * f, v.Y * f, v.Z * f);
        }

        public static Vec3 operator *(Vec3 v, double d)
        {
            return new Vec3((float)(v.X * d), (float)(v.Y * d), (float)(v.Z * d));
        }

        public static Vec3 operator /(Vec3 v, float f)
        {
            return new Vec3(v.X / f, v.Y / f, v.Z / f);
        }
        
        public static Vec3 operator %(Vec3 v, int i)
        {
            return new Vec3(v.X % i, v.Y % i, v.Z % i);
        }

        /// <summary>
        /// Subtracts the X/Y of a Vec2 from the X/Z of a Vec3.
        /// </summary>
        /// <param name="v">The Vec3 to subtract from</param>
        /// <param name="v2">The Vec2 to subtract</param>
        /// <returns>Vec3 result</returns>
        public static Vec3 operator -(Vec3 v, Vec2 v2)
        {
            return new Vec3(v.X - v2.X, v.Y, v.Z - v2.Y);
        }

        /// <summary>
        /// Adds the X/Y from a Vec2 to the X/Z of a Vec3.
        /// </summary>
        /// <param name="v">Vec3 to add to</param>
        /// <param name="v2">Vec2 to add</param>
        /// <returns>Vec3 result</returns>
        public static Vec3 operator +(Vec3 v, Vec2 v2)
        {
            return new Vec3(v.X + v2.X, v.Y, v.Z + v2.Y);
        }

        /// <summary>
        /// Multiplies the X/Z of a Vec3 by the X/Y of a Vec2.
        /// </summary>
        /// <param name="v">Vec3 to multiply</param>
        /// <param name="v2">Vec2 to multiply by</param>
        /// <returns>Vec3 result</returns>
        public static Vec3 operator *(Vec3 v, Vec2 v2)
        {
            return new Vec3(v.X * v2.X, v.Y, v.Z * v2.Y);
        }

        /// <summary>
        /// Divides the X/Z of a Vec3 by the X/Y of a Vec2.
        /// </summary>
        /// <param name="v">Vec3 to divide</param>
        /// <param name="v2">Vec2 to divide by</param>
        /// <returns>Vec3 result</returns>
        public static Vec3 operator /(Vec3 v, Vec2 v2)
        {
            return new Vec3(v.X / v2.X, v.Y, v.Z / v2.Y);
        }

        public static bool operator ==(Vec3 v, Vec3 v2)
        {
            if (v.X != v2.X) return false;
            if (v.Y != v2.Y) return false;
            if (v.Z != v2.Z) return false;
            return true;
        }

        public static bool operator ==(Vec3 v, Vec2 v2)
        {
            if (v.X != v2.X) return false;
            if (v.Z != v2.Y) return false;
            return true;
        }

        public static bool operator !=(Vec3 v, Vec3 v2)
        {
            return !(v == v2);
        }

        public static bool operator !=(Vec3 v, Vec2 v2)
        {
            return !(v == v2);
        }

        /// <summary>
        /// Converts a Vec3 to a Vec2, discarding Y and treating X/Z as X/Y.
        /// </summary>
        /// <param name="v">Vec3 to convert</param>
        /// <returns>Vec2</returns>
        public static implicit operator Vec2(Vec3 v)
        {
            return new Vec2(v.X, v.Z);
        }

        /// <summary>
        /// Converts a Vec2 into a Vec3, treating the coordinates as X/Z with Y set to 0.
        /// </summary>
        /// <param name="v">Vec2 to convert</param>
        /// <returns>Vec3</returns>
        public static implicit operator Vec3(Vec2 v)
        {
            return new Vec3(v.X, 0, v.Y);
        }

        public override string ToString()
        {
            return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec3)
                return this == (Vec3)obj;
            else return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() / 3 + Y.GetHashCode() / 3 + Z.GetHashCode() / 3;
        }
    }
}
