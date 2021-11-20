using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    public class Quadtree
    {
        public Vec3 Position;

        public Quadtree Parent;

        public Quadtree TopLeft;
        public Quadtree TopRight;
        public Quadtree BottomLeft;
        public Quadtree BottomRight;

        public List<Quadtree> Quadtrees
        {
            get
            {
                return new List<Quadtree>
                {
                    TopLeft,
                    TopRight,
                    BottomLeft,
                    BottomRight,
                };
            }
        }

        public Box Box;
    }
}
