using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class Quad
    {
        public TextureVertex[] Vertices = new TextureVertex[6]
        {
            new TextureVertex(Vec3.PPZ, Vec3.ZZN, Vec2.PP), //z
            new TextureVertex(Vec3.PNZ, Vec3.ZZN, Vec2.PZ), //y
            new TextureVertex(Vec3.NNZ, Vec3.ZZN, Vec2.ZZ), //x
            new TextureVertex(Vec3.PPZ, Vec3.ZZN, Vec2.PP), //z
            new TextureVertex(Vec3.NNZ, Vec3.ZZN, Vec2.ZZ), //x
            new TextureVertex(Vec3.NPZ, Vec3.ZZN, Vec2.ZP), //w
        };
    }
}
