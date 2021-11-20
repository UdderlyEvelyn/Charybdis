using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public static class Cube
    {
        public static Vertex[] Vertices = new Vertex[8] 
        {
            new Vertex(Vec3.NNN, Col3.Green), //0
            new Vertex(Vec3.NNP, Col3.Green), //1
            new Vertex(Vec3.NPN, Col3.Green), //2
            new Vertex(Vec3.NPP, Col3.Green), //3
            new Vertex(Vec3.PNN, Col3.Green), //4
            new Vertex(Vec3.PNP, Col3.Green), //5
            new Vertex(Vec3.PPN, Col3.Green), //6
            new Vertex(Vec3.PPP, Col3.Green), //7
        };

        public static Index[] Indices = new Index[12]
        {
            new Index(0, 1, 3),
            new Index(6, 0, 2),
            new Index(5, 0, 4),
            new Index(6, 4, 0),
            new Index(0, 3, 2),
            new Index(5, 1, 0),
            new Index(3, 1, 5),
            new Index(7, 4, 6),
            new Index(4, 7, 5),
            new Index(7, 6, 2),
            new Index(7, 2, 3),
            new Index(7, 3, 5),
        };

        public static Vertex[] Positioned(Vec3 position, Col3 color)
        {
            return new Vertex[8] 
            {
                new Vertex(Vec3.NNN + position, color), //0
                new Vertex(Vec3.NNP + position, color), //1
                new Vertex(Vec3.NPN + position, color), //2
                new Vertex(Vec3.NPP + position, color), //3
                new Vertex(Vec3.PNN + position, color), //4
                new Vertex(Vec3.PNP + position, color), //5
                new Vertex(Vec3.PPN + position, color), //6
                new Vertex(Vec3.PPP + position, color), //7
            };
        }
    }
}
