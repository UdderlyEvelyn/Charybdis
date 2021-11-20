using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace Charybdis.Library.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Col3 : IEquatable<Col3>
    {
        public float R;
        public float G;
        public float B;

        public Col3(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public static readonly Col3 White = new Col3(255, 255, 255);
        public static readonly Col3 Red = new Col3(255, 0, 0);
        public static readonly Col3 Blue = new Col3(0, 0, 255);
        public static readonly Col3 Green = new Col3(0, 255, 0);
        public static readonly Col3 Black = new Col3(0, 0, 0);
        public static readonly Col3 MagicPink = new Col3(255, 0, 255);
        public static readonly Col3 Yellow = new Col3(0, 255, 255);
        public static readonly Col3 DarkGray = new Col3(146, 146, 146);
        
        public override string ToString()
        {
            return R.ToString() + ", " + G.ToString() + ", " + B.ToString();
        }

        public bool Equals(Col3 other)
        {
            return R == other.R && 
                   G == other.G && 
                   B == other.B;
        }

        public bool Equals(Col4 other)
        {
            if (other.A != 255)
                return false;
            else
                return R == other.R &&
                       G == other.G &&
                       B == other.B;
        }

        public override int GetHashCode()
        {
            //This may not be the best way, just trying for simplicity and speed. This assumes 
            //color values are 0-255, which they should be but is not currently enforced. 
            //-Y 12/23/17
            return ((byte)R).GetHashCode() & ((byte)G).GetHashCode() & ((byte)B).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static bool operator ==(Col3 a, Col3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Col3 a, Col3 b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Col3 a, Col4 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Col3 a, Col4 b)
        {
            return !a.Equals(b);
        }

        public static Col3 operator *(Col3 c, float f)
        {
            return new Col3(c.R * f, c.G * f, c.B * f);
        }

        public static Col3 operator /(Col3 c, float f)
        {
            return new Col3(c.R / f, c.G / f, c.B / f);
        }

        public static Col3 operator +(Col3 c, float f)
        {
            return new Col3(c.R + f, c.G + f, c.B + f);
        }

        public static Col3 operator -(Col3 c, float f)
        {
            return new Col3(c.R - f, c.G - f, c.B - f);
        }

        public static Col3 operator *(Col3 c, double d)
        {
            return new Col3((float)(c.R * d), (float)(c.G * d), (float)(c.B * d));
        }

        public static Col3 operator /(Col3 c, double d)
        {
            return new Col3((float)(c.R / d), (float)(c.G / d), (float)(c.B / d));
        }

        public static Col3 operator +(Col3 c, double d)
        {
            return new Col3((float)(c.R + d), (float)(c.G + d), (float)(c.B + d));
        }

        public static Col3 operator -(Col3 c, double d)
        {
            return new Col3((float)(c.R - d), (float)(c.G - d), (float)(c.B - d));
        }

        public static Col3 Average(Col3 c, Col3 c2)
        {
            return new Col3((c.R + c2.R) / 2, (c.G + c2.G) / 2, (c.B + c2.B) / 2);
        }

        public static Col4 Average(Col3 c, Col4 c2)
        {
            return new Col4((c.R + c2.R) / 2, (c.G + c2.G) / 2, (c.B + c2.B) / 2, c2.A);
        }
    }
}
