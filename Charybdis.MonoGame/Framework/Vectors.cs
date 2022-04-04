using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Charybdis.MonoGame
{
    public static class Vectors
    {
        public static class Vec2
        {
            public static readonly Vector2 UnitX = new Vector2(1, 0);
            public static readonly Vector2 UnitY = new Vector2(0, 1);

            public static readonly Vector2 Left = new Vector2(-1, 0);
            public static readonly Vector2 Right = new Vector2(1, 0);
            public static readonly Vector2 Up = new Vector2(0, -1);
            public static readonly Vector2 Down = new Vector2(0, 1);

            public static readonly Vector2 Zero = new Vector2(0, 0);
            public static readonly Vector2 One = new Vector2(1, 1);

            public static readonly Vector2 NN = new Vector2(-1, -1);
            public static readonly Vector2 NP = new Vector2(-1, 1);
            public static readonly Vector2 PN = new Vector2(1, -1);
            public static readonly Vector2 PP = new Vector2(1, 1);

            public static readonly Vector2 ZN = new Vector2(0, -1);
            public static readonly Vector2 NZ = new Vector2(-1, 0);
            public static readonly Vector2 ZP = new Vector2(0, 1);
            public static readonly Vector2 PZ = new Vector2(1, 0);
            public static readonly Vector2 ZZ = new Vector2(0, 0);

            public static float Cross(Vector2 vector1, Vector2 vector2)
            {
                return vector1.Cross(vector2);
            }
        }

        public static class Vec3
        {
            public static readonly Vector3 Up = new Vector3(0, 1, 0);
            public static readonly Vector3 Down = new Vector3(0, -1, 0);
            public static readonly Vector3 UnitX = new Vector3(1, 0, 0);
            public static readonly Vector3 UnitY = new Vector3(0, 1, 0);
            public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);
            public static readonly Vector3 Zero = new Vector3(0, 0, 0);
            public static readonly Vector3 One = new Vector3(1, 1, 1);
            public static readonly Vector3 HHH = new Vector3(.5f, .5f, .5f);
            public static readonly Vector3 HZH = new Vector3(.5f, 0, .5f);

            public static readonly Vector3 ZZZ = new Vector3(0, 0, 0);
            public static readonly Vector3 ZZP = new Vector3(0, 0, 1);
            public static readonly Vector3 ZPZ = new Vector3(0, 1, 0);
            public static readonly Vector3 ZPP = new Vector3(0, 1, 1);
            public static readonly Vector3 PZZ = new Vector3(1, 0, 0);
            public static readonly Vector3 PZP = new Vector3(1, 0, 1);
            public static readonly Vector3 PPZ = new Vector3(1, 1, 0);
            public static readonly Vector3 PPP = new Vector3(1, 1, 1);
            public static readonly Vector3 ZZN = new Vector3(0, 0, -1);
            public static readonly Vector3 ZNZ = new Vector3(0, -1, 0);
            public static readonly Vector3 ZNN = new Vector3(0, 1, 1);
            public static readonly Vector3 NZZ = new Vector3(-1, 0, 0);
            public static readonly Vector3 NZN = new Vector3(-1, 0, -1);
            public static readonly Vector3 NNZ = new Vector3(-1, -1, 0);
            public static readonly Vector3 NNN = new Vector3(-1, -1, -1);
            public static readonly Vector3 NPN = new Vector3(-1, 1, -1);
            public static readonly Vector3 NPP = new Vector3(-1, 1, 1);
            public static readonly Vector3 PNP = new Vector3(1, -1, 1);
            public static readonly Vector3 PNN = new Vector3(1, -1, -1);
            public static readonly Vector3 ZPN = new Vector3(0, 1, -1);
            public static readonly Vector3 ZNP = new Vector3(0, -1, 1);
            public static readonly Vector3 PNZ = new Vector3(1, -1, 0);
            public static readonly Vector3 NPZ = new Vector3(-1, 1, 0);
            public static readonly Vector3 PZN = new Vector3(1, 0, -1);
            public static readonly Vector3 NZP = new Vector3(-1, 0, 1);
            public static readonly Vector3 PPN = new Vector3(1, 1, -1);
            public static readonly Vector3 NNP = new Vector3(-1, -1, 1);

            public static readonly Vector3 Front = new Vector3(0, 0, 1);
            public static readonly Vector3 Back = new Vector3(0, 0, -1);
            public static readonly Vector3 Top = new Vector3(0, 1, 0);
            public static readonly Vector3 Bottom = new Vector3(0, -1, 0);
            public static readonly Vector3 Left = new Vector3(-1, 0, 0);
            public static readonly Vector3 Right = new Vector3(1, 0, 0);
        }

        public static class Vec4
        {
            public static readonly Vector4 Zero = new Vector4(0);
            public static readonly Vector4 One = new Vector4(1);
        }
    }
}
