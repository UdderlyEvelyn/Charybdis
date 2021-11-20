using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace Charybdis.Library.Core
{
    //Avoid using this in situations where the chunk might turn out to be invalid, because it incurs 
    //extra calculation of block coordinates which may not be usable if the chunk isn't valid 
    //(at least unless this is updated to handle it nicely in an alternate constructor or something).
    [DataContract, StructLayout(LayoutKind.Sequential)]
    public struct Coordinate
    {
        public Vec2 Chunk
        {
            get
            {
                return new Vec2(CX, CZ);
            }
        }

        public Vec3 Local
        {
            get
            {
                return new Vec3(X, Y, Z);
            }
        }

        [DataMember]
        public int X;
        [DataMember]
        public int Y;
        [DataMember]
        public int Z;
        [DataMember]
        public int CX;
        [DataMember]
        public int CZ;

        public static Coordinate From(Vec3 v)
        {
            return new Coordinate
            {
                CX = (int)Math.Floor(v.X / 16),
                CZ = (int)Math.Floor(v.Z / 16),
                X = (int)Math.Floor(v.X % 16),
                Y = (int)Math.Floor(v.Y),
                Z = (int)Math.Floor(v.Z % 16),
            };
        }
    }
}
