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
    public struct Col4
    {
        public float A;
        public float R;
        public float G;
        public float B;

        public Col4(float r, float g, float b, float a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static implicit operator Col3(Col4 c)
        {
            return new Col3(c.R, c.G, c.B);
        }

        public static implicit operator Col4(Col3 c)
        {
            return new Col4(c.R, c.G, c.B, 255);
        }

        public static readonly Col4 White = Col3.White;
        public static readonly Col4 Red = Col3.Red;
        public static readonly Col4 Blue = Col3.Blue;
        public static readonly Col4 Green = Col3.Green;
        public static readonly Col4 Black = Col3.Black;

        public override string ToString()
        {
            return R.ToString() + ", " + G.ToString() + ", " + B.ToString() + ", " + A.ToString();
        }

        public bool Equals(Col4 other)
        {
            return R == other.R &&
                   G == other.G &&
                   B == other.B &&
                   A == other.A;
        }

        public bool Equals(Col3 other)
        {
            if (A != 255)
                return false;
            else
                return R == other.R &&
                       G == other.G &&
                       B == other.B;
        }

        public override int GetHashCode()
        {
            //This may not be the best way, just trying for simplicity and speed. This assumes color values are 0-255, 
            //which they should be but is not currently enforced. The first one is a multiplication because it's the alpha,
            //and it's subtracted from 255 so that full color is multiplying by zero, so that it can be compared with Col3.
            //-Y 12/23/17
            return (255 - ((byte)A).GetHashCode()) * (((byte)R).GetHashCode() & ((byte)G).GetHashCode() & ((byte)B).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static bool operator ==(Col4 a, Col4 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Col4 a, Col4 b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Col4 a, Col3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Col4 a, Col3 b)
        {
            return !a.Equals(b);
        }

        public static Col4 Average(Col4 c, Col3 c2)
        {
            return new Col4((c.R + c2.R) / 2, (c.G + c2.G) / 2, (c.B + c2.B) / 2, c.A);
        }

        public static Col4 Average(Col4 c, Col4 c2)
        {
            return new Col4((c.R + c2.R) / 2, (c.G + c2.G) / 2, (c.B + c2.B) / 2, (c.A + c2.A) / 2);
        }
    }
}
