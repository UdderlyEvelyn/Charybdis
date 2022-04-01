using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public static class Collision2
    {
        //public static bool Contains(this IList<LineSegment> segments, Vec2 v)
        //{
        //    //Cheap test to make sure it's not WAY out of bounds before doing more intensive math.
        //    var allX = segments.Select(x => x.A.X).Union(segments.Select(x => x.B.X));
        //    var allY = segments.Select(x => x.A.Y).Union(segments.Select(x => x.B.Y));
        //    float minX = allX.Min();
        //    float minY = allY.Min();
        //    float maxX = allX.Max();
        //    float maxY = allY.Max();
        //    Rect approximate = new Rect(minX, minY, maxX - minX, maxY - minY);
        //    if (!approximate.Contains(v))
        //        return false;

        //    Vec2 center = approximate.Center;

        //    bool currentResult = false;

        //    foreach (var s in segments)
        //    {
        //        bool left = s.A.X < center.X && s.B.X < center.X;
        //        bool right = s.A.X > center.X && s.B.X > center.X;
        //        bool top = s.A.Y < center.Y && s.B.Y < center.Y;
        //        bool bottom = s.A.Y > center.Y && s.B.Y > center.Y;

        //        if (top)
        //        { //Wrong because it's checking if it's within the Y range of the segment not if it's greater than it in general.. 
        //            if (s.A.Y > s.B.Y)
        //            {
        //                if (v.Y.IsBetween(s.B.Y, s.A.Y))
        //                    currentResult = true;
        //            }
        //            else if (s.B.Y > s.A.Y)
        //            {
        //                if (v.Y.IsBetween(s.A.Y, s.B.Y))
        //                    currentResult = true;
        //            }
        //            else if (v.Y >= s.A.Y)
        //                currentResult = true;
        //        }
        //    }
        //}

        //Winding Number
        public static bool PolygonContains(IList<Vec2> polygonVertices, Vec2 v)
        {
            Vec2 leftVertex;
            Vec2 rightVertex;
            bool inside = false;

            if (polygonVertices.Count < 3) //If it's a point or a line then it can't be inside (unless it's on the line or equal to the point), so just return false.
                return inside;
            else if (polygonVertices.Count == 4)
            {
                int leftVertices = 0;
                int rightVertices = 0;
                int upVertices = 0;
                int downVertices = 0;
                foreach (var pv in polygonVertices)
                {
                    if (pv.X < v.X)
                        leftVertices++;
                    else if (pv.X > v.X)
                        rightVertices++;
                    if (pv.Y < v.Y)
                        upVertices++;
                    else if (pv.Y > v.Y)
                        downVertices++;
                }
                if (leftVertices != 2 ||
                    rightVertices != 2 ||
                    upVertices != 2 ||
                    downVertices != 2)
                    return false;
                else
                    return true;
            }

            Vec2 previousVertex = new Vec2(polygonVertices[polygonVertices.Count - 1].X, polygonVertices[polygonVertices.Count - 1].Y); //Start with last vertex to account for last->first line segment.

            for (int i = 0; i < polygonVertices.Count; i++) //Iterate through count of vertices for an index..
            {
                Vec2 currentVertex = new Vec2(polygonVertices[i].X, polygonVertices[i].Y); //Get next vertex.

                if (currentVertex.X > previousVertex.X) //If the current vertex is further right than the old one..
                {
                    leftVertex = previousVertex;
                    rightVertex = currentVertex;
                }
                else //The current vertex is further left than the old one.
                {
                    leftVertex = currentVertex; //Set v1 to the new one.
                    rightVertex = previousVertex; //Set v2 to the old one.
                }

                if ((currentVertex.X < v.X) == (v.X <= previousVertex.X) && //If the point is between (inclusive) the two vertices we're looking at presently..
                   ((long)v.Y - (long)leftVertex.Y) * (long)(rightVertex.X - leftVertex.X) < ((long)rightVertex.Y - (long)leftVertex.Y) * (long)(v.X - leftVertex.X)) //Is it on the proper side of the line..?
                   /*X,Y=0--------3----5-7--------X=N
                    *1                  cv 
                    *2                 / |
                    *3               /   |
                    *4             /___v_| 
                    *5           pv 
                    *Y=N
                    */
                    //(((4-5=-1) * (7-3=4) = -4) < ((1-5=-4) * (5-3=2) = -8)) = false
                    //Difference In Point And Left Y * Right Triangle Width < Right Triangle Height * Difference In Point And Left X
                    //Don't understand the logic here, but it's winding number algorithm related I think.. -^
                {
                    inside = !inside; //Toggle
                }

                previousVertex = currentVertex; //New->Old and next loop.
            }

            return inside; //Return
        }

        // Converted for C# by UdderlyEvelyn 2017
        // From http://geomalgorithms.com/a03-_inclusion.html

        // Copyright 2000 softSurfer, 2012 Dan Sunday
        // This code may be freely used and modified for any purpose
        // providing that this copyright notice is included with it.
        // SoftSurfer makes no warranty for this code, and cannot be held
        // liable for any real or imagined damage resulting from its use.
        // Users of this code must verify correctness for their application.


        // a Vec2 is defined by its coordinates {int x, y;}
        //===================================================================


        // isLeft(): tests if a point is Left|On|Right of an infinite line.
        //    Input:  three points P0, P1, and P2
        //    Return: >0 for P2 left of the line through P0 and P1
        //            =0 for P2  on the line
        //            <0 for P2  right of the line
        //    See: Algorithm 1 "Area of Triangles and Polygons"
        public static int isLeft(Vec2 P0, Vec2 P1, Vec2 P2)
        {
            return ((P1.Xi - P0.Xi) * (P2.Yi - P0.Yi)
                    - (P2.Xi - P0.Xi) * (P1.Yi - P0.Yi));
        }
        //===================================================================


        // cn_PnPoly(): crossing number test for a point in a polygon
        //      Input:   P = a point,
        //               V[] = vertex points of a polygon V[n+1] with V[n]=V[0]
        //      Return:  0 = outside, 1 = inside
        // This code is patterned after [Franklin, 2000]
        public static int cn_PnPoly(Vec2 P, List<Vec2> V, int n)
        {
            int cn = 0;    // the  crossing number counter

            // loop through all edges of the polygon
            for (int i = 0; i < n; i++)
            {    // edge from V[i]  to V[i+1]
                if (((V[i].Yi <= P.Yi) && (V[i + 1].Yi > P.Yi))     // an upward crossing
                 || ((V[i].Yi > P.Yi) && (V[i + 1].Yi <= P.Yi)))
                { // a downward crossing
                  // compute  the actual edge-ray intersect x-coordinate
                    float vt = (float)(P.Yi - V[i].Yi) / (V[i + 1].Yi - V[i].Yi);
                    if (P.Xi < V[i].Xi + vt * (V[i + 1].Xi - V[i].Xi)) // P.Xi < intersect
                        ++cn;   // a valid crossing of y=P.Yi right of P.Xi
                }
            }
            return (cn & 1);    // 0 if even (out), and 1 if  odd (in)

        }
        //===================================================================

        public static bool PointInPolygon(Vec2 point, List<Vec2> vertices)
        {
            int cn = 0;    // the  crossing number counter

            // loop through all edges of the polygon
            for (int i = 0; i < vertices.Count - 1; i++)
            {    // edge from V[i]  to V[i+1]
                if (((vertices[i].Yi <= point.Yi) && (vertices[i + 1].Yi > point.Yi))     // an upward crossing
                 || ((vertices[i].Yi > point.Yi) && (vertices[i + 1].Yi <= point.Yi)))
                { // a downward crossing
                  // compute  the actual edge-ray intersect x-coordinate
                    float vt = (point.Y - vertices[i].Y) / (vertices[i + 1].Y - vertices[i].Y);
                    if (point.X < vertices[i].X + vt * (vertices[i + 1].X - vertices[i].X)) // P.Xi < intersect
                        ++cn;   // a valid crossing of y=P.Yi right of P.Xi
                }
            }
            return (cn & 1) == 1;    // 0 if even (out), and 1 if  odd (in)
        }


        // wn_PnPoly(): winding number test for a point in a polygon
        //      Input:   P = a point,
        //               V[] = vertex points of a polygon V[n+1] with V[n]=V[0]
        //      Return:  wn = the winding number (=0 only when P is outside)
        public static int wn_PnPoly(Vec2 P, List<Vec2> V, int n)
        {
            int wn = 0;    // the  winding number counter

            // loop through all edges of the polygon
            for (int i = 0; i < n; i++)
            {   // edge from V[i] to  V[i+1]
                if (V[i].Yi <= P.Yi)
                {          // start y <= P.Yi
                    if (V[i + 1].Yi > P.Yi)      // an upward crossing
                        if (isLeft(V[i], V[i + 1], P) > 0)  // P left of  edge
                            ++wn;            // have  a valid up intersect
                }
                else
                {                        // start y > P.Yi (no test needed)
                    if (V[i + 1].Yi <= P.Yi)     // a downward crossing
                        if (isLeft(V[i], V[i + 1], P) < 0)  // P right of  edge
                            --wn;            // have  a valid down intersect
                }
            }
            return wn;
        }
        //===================================================================
    }

    public static class Collision3
    {
        public static List<ICollisionObject3> CollisionObjects = new List<ICollisionObject3>();

        /// <summary>
        /// Detect collisions along a ray.
        /// </summary>
        /// <param name="r">the ray to check</param>
        /// <param name="limit">maximum distance to take objects into account from</param>
        /// <param name="increment">the amount of distance along the ray to move between each check</param>
        /// <returns>the first ICollisionObject in the collision system that was encountered</returns>
        public static ICollisionObject3 Cast(this Ray3 r, float limit, float increment = 1f)
        {
            IEnumerable<ICollisionObject3> cobjs = CollisionObjects.Where(co => co.Position.FastDistance(r.Position) < limit * limit); for (float f = 0; f < limit; f += increment)
            {
                foreach (ICollisionObject3 co in cobjs)
                {
                    if (co.Position.Distance(r.Position + r.Direction * f) <= co.CollisionRadius)
                        return co;
                }
            }
            return null;
        }

        /// <summary>
        /// Detect collisions at a fixed distance along a ray.
        /// </summary>
        /// <param name="r">the ray to check</param>
        /// <param name="distance">the distance along the ray to check</param>
        /// <returns>the first ICollisionObject in the collision system that was encountered</returns>
        public static ICollisionObject3 Offset(this Ray3 r, float distance)
        {
            try
            {
                return CollisionObjects.First(co => co.Position.Distance(r.Position + r.Direction * distance) < co.CollisionRadius);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Find the closest CollisionObject to this point.
        /// </summary>
        /// <param name="v">the point to check</param>
        /// <param name="limit">the maximum distance away that objects are taken into account</param>
        /// <returns>the closest ICollisionObject in the system that was within the limit</returns>
        public static ICollisionObject3 Closest(this Vec3 v, float limit)
        {
            return CollisionObjects.Where(cobj => cobj.Position.FastDistance(v) < limit * limit).OrderBy(cobj => cobj.Position.FastDistance(v)).FirstOrDefault();
        }

        /// <summary>
        /// Find the closest CollisionObject to this point, ignoring the height.
        /// </summary>
        /// <param name="v">the point to check</param>
        /// <param name="limit">the maximum distance away that objects are taken into account</param>
        /// <returns>the closest ICollisionObject to the X/Z coordinates of the vector</returns>
        public static ICollisionObject3 ClosestXZ(this Vec3 v, float limit)
        {
            return CollisionObjects.Where(cobj => cobj.Position.FastDistanceXZ(v) < limit * limit).OrderBy(cobj => cobj.Position.FastDistanceXZ(v)).FirstOrDefault();
        }

        /// <summary>
        /// Find the closest CollisionObject to this point, ignoring the height.
        /// </summary>
        /// <param name="v">the point to check</param>
        /// <param name="limit">the maximum distance away that objects are taken into account</param>
        /// <returns>the closest ICollisionObject to the coordinates of the vector (along the 3d X/Z plane)</returns>
        public static ICollisionObject3 ClosestXZ(this Vec2 v, float limit)
        {
            return CollisionObjects.Where(cobj => cobj.Position.FastDistanceXZ(v) < limit * limit).OrderBy(cobj => cobj.Position.FastDistanceXZ(v)).FirstOrDefault();
        }

        //Just realized this is pointless, since finding the single closest isn't too slow, and we can translate that back to the grid and grab data from there.
        //Note that this is VERY SLOW - could be improved, most likely.
        /// <summary>
        /// Find the closest (count) CollisionObjects to this point, ignoring the height (SLOW!)
        /// </summary>
        /// <param name="v">the point to check</param>
        /// <param name="limit">the maximum distance away that objects are taken into account</param>
        /// <param name="count">the number of objects to return</param>
        /// <returns>an IEnumerable of ICollisionObject containing the closest (count) CollisionObjects to the point</returns>
        public static IEnumerable<ICollisionObject3> ClosestXZ(this Vec3 v, float limit, int count)
        {
            return CollisionObjects.Where(cobj => cobj.Position.FastDistanceXZ(v) < limit * limit).OrderBy(cobj => cobj.Position.FastDistanceXZ(v)).Take(count);
        }
    }

    public enum State : byte
    {
        Inside = 0,
        Outside = 1,
        Intersect = 2,
    }
}
