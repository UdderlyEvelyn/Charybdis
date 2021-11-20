using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    public static class Vector
    {
        #region LINQ Extension Methods

        #region Sum

        public static Vec2 Sum(this IEnumerable<Vec2> e)
        {
            return new Vec2(e.Sum(v => v.X), e.Sum(v => v.Y));
        }

        public static Vec3 Sum(this IEnumerable<Vec3> e)
        {
            return new Vec3(e.Sum(v => v.X), e.Sum(v => v.Y), e.Sum(v => v.Z));
        }

        public static Vec4 Sum(this IEnumerable<Vec4> e)
        {
            return new Vec4(e.Sum(v => v.X), e.Sum(v => v.Y), e.Sum(v => v.Z), e.Sum(v => v.W));
        }

        #endregion

        #endregion
    }
}
