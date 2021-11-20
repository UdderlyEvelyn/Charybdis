using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    public class Plane
    {
        public float A;
        public float B;
        public float C;
        public float D;

        public Plane(float a, float b, float c, float d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public float Distance(Vec3 v)
        {
            return A * v.X + B * v.Y + C * v.Z + D;
        }

        public bool Contains(Vec3 v)
        {
            return Distance(v) > 0;
        }

        public bool Contains(Vec2 v)
        {
            return A * v.X + C * v.Y + D > 0;
        }

        public Plane Normalize()
        {
            float magnitude = (float)Math.Sqrt(A * A + B * B + C * C);
            A /= magnitude;
            B /= magnitude;
            C /= magnitude;
            D /= magnitude;
            return this;
        }

        public Plane Abs()
        {
            return new Plane(Math.Abs(A), Math.Abs(B), Math.Abs(C), Math.Abs(D));
        }


        //http://fgiesen.wordpress.com/2010/10/17/view-frustum-culling/
        //http://www.reedbeta.com/blog/2013/12/28/on-vector-math-libraries/ <-- Look at this (and browse the site..)
        public bool Check(Box box)
        {
            return box.Center.Dot(ToVec3()) + box.Extent.Dot(Abs().ToVec3()) > -D;
        }

        public bool Check(Vec3 v)
        {
            return Vec3.Dot(v, ToVec3()) > -D;
        }

        public static implicit operator Vec4(Plane p)
        {
            return new Vec4(p.A, p.B, p.C, p.D);
        }

        public static implicit operator Plane(Vec4 v)
        {
            return new Plane(v.X, v.Y, v.Z, v.W);
        }

        /// <summary>
        /// Discards the "D" component of Plane p and returns a Vec3.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vec3 ToVec3()
        {
            return new Vec3(A, B, C);
        }
    }
}
