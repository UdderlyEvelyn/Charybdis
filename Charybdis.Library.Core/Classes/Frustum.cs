using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// Custom Frustum class that is platform independent (would require minor tweaks for matrix type and col/row majority), but not used in the case of MonoGame due to worse performance than the native classes.
    /// </summary>
    public class Frustum
    {
        public float FOV;
        public float Aspect;

        public Plane Left;
        public Plane Right;
        public Plane Top;
        public Plane Bottom;
        public Plane Near;
        public Plane Far;

        public Frustum Normalized;

        public bool Intersects(Vec3 v)
        {
            if (!(Far.Contains(v) && Near.Contains(v))) return false;
            if (!(Left.Contains(v) && Right.Contains(v))) return false;
            if (!(Top.Contains(v) && Bottom.Contains(v))) return false;
            return true; 
        }

        public bool Contains(Box b)
        {
            if (!(Far.Check(b) && Near.Check(b))) return false;
            if (!(Left.Check(b) && Right.Check(b))) return false;
            if (!(Top.Check(b) && Bottom.Check(b))) return false;
            return true;             
        }

        public bool Contains(Vec3 v)
        {
            if (!(Far.Check(v) && Near.Check(v))) return false;
            if (!(Left.Check(v) && Right.Check(v))) return false;
            if (!(Top.Check(v) && Bottom.Check(v))) return false;
            return true;           
        }

        /*public static Frustum FromMatrix(Matrix m)
        {
            Frustum f = new Frustum
            {
                Left = new Plane(m.M14 - m.M11, m.M24 - m.M21, m.M34 - m.M31, m.M44 - m.M41),
                Right = new Plane(m.M14 + m.M11, m.M24 + m.M21, m.M34 + m.M31, m.M44 + m.M41),
                Top = new Plane(m.M14 + m.M12, m.M24 + m.M22, m.M34 + m.M32, m.M44 + m.M42),
                Bottom = new Plane(m.M14 - m.M12, m.M24 - m.M22, m.M34 - m.M32, m.M44 - m.M42),
                Near = new Plane(m.M14 - m.M13, m.M24 - m.M23, m.M34 - m.M33, m.M44 - m.M43),
                Far = new Plane(m.M14 + m.M13, m.M24 + m.M23, m.M34 + m.M33, m.M44 + m.M43),
            };
            f.Normalized = new Frustum
            {
                Left = f.Left.Normalize(),
                Right = f.Right.Normalize(),
                Top = f.Top.Normalize(),
                Bottom = f.Bottom.Normalize(),
                Near = f.Near.Normalize(),
                Far = f.Far.Normalize(),
            };
            return f;
        }*/
    }
}
