using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.MonoGame
{
    public static class Collision
    {
        public static bool Within(this Vec2 point, Vec2 origin, Vec2 extent)
        {
            return
                point.X >= origin.X &&
                point.Y >= origin.Y &&
                point.X <= extent.X &&
                point.Y <= extent.Y;
        }

        public static bool Within (this Vec2 point, BoundingRect r)
        {
            return point.Within(r.Min, r.Max);
        }
    }
}
