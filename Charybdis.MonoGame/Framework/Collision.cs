using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Charybdis.MonoGame
{
    public static class Collision
    {
        public static bool PointInPolygon(Vec2 point, List<Vec2> vertices)
        {
            int cn = 0;    // the  crossing number counter

            // loop through all edges of the polygon
            for (int i = 0; i < vertices.Count - 1; i++)
            {    // edge from V[i]  to V[i+1]
                if (((vertices[i].Y <= point.Y) && (vertices[i + 1].Y > point.Y))     // an upward crossing
                 || ((vertices[i].Y > point.Y) && (vertices[i + 1].Y <= point.Y)))
                { // a downward crossing
                  // compute  the actual edge-ray intersect x-coordinate
                    float vt = (point.Y - vertices[i].Y) / (vertices[i + 1].Y - vertices[i].Y);
                    if (point.X < vertices[i].X + vt * (vertices[i + 1].X - vertices[i].X)) // P.Xi < intersect
                        ++cn;   // a valid crossing of y=P.Yi right of P.Xi
                }
            }
            return (cn & 1) == 1;    // 0 if even (out), and 1 if  odd (in)
        }

        public static bool Within(this Vec2 point, Vec2 origin, Vec2 extent)
        {
            return
                point.X >= origin.X &&
                point.Y >= origin.Y &&
                point.X <= extent.X &&
                point.Y <= extent.Y;
        }

        public static bool Within (this Vec2 point, Rectangle r)
        {
            return point.Within(r.Position, r.Position + r.Size);
        }
    }
}
