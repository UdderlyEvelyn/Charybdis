using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework;
using XNABoundingBox = Microsoft.Xna.Framework.BoundingBox;
using CharybdisBoundingBox = Charybdis.Library.Core.BoundingRect;

namespace Charybdis.MonoGame
{
    /// <summary>
    /// XNA to Charybdis and Vice Versa Conversions
    /// </summary>
    public static class Conversion
    {
        #region Vectors

        public static Vector2 ToXNA(this Vec2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vec2 ToCharybdis(this Vector2 v)
        {
            return new Vec2(v.X, v.Y);
        }

        public static Vector3 ToXNA(this Vec3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vec3 ToCharybdis(this Vector3 v)
        {
            return new Vec3(v.X, v.Y, v.Z);
        }
        
        public static Vector4 ToXNA(this Vec4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Vec4 ToCharybdis(this Vector4 v)
        {
            return new Vec4(v.X, v.Y, v.Z, v.W);
        }

        #endregion

        #region Color

        public static Color ToXNA(this Col3 c)
        {
            //return new Color(c.R, c.G, c.B);
            return Color.FromNonPremultiplied((int)c.R, (int)c.G, (int)c.B, 255);
        }

        public static Color ToXNA(this Col4 c)
        {
            //return new Color(c.R, c.G, c.B, c.A);
            return Color.FromNonPremultiplied((int)c.R, (int)c.G, (int)c.B, (int)c.A);
        }

        public static Col4 ToCharybdis(this Color c)
        {
            return new Col4(c.R, c.G, c.B, c.A);
        }

        #endregion

        #region Shapes

        public static Microsoft.Xna.Framework.Rectangle ToXNA(this Rect r)
        {
            return new Microsoft.Xna.Framework.Rectangle((int)r.X, (int)r.Y, (int)r.W, (int)r.H);
        }

        public static Rect ToCharybdis(this Microsoft.Xna.Framework.Rectangle r)
        {
            return new Rect(r.X, r.Y, r.Width, r.Height);
        }

        #endregion

        #region Bounding

        public static XNABoundingBox ToXNA(this CharybdisBoundingBox bb)
        {
            return new XNABoundingBox(new Vector3(bb.Min.ToXNA(), 0),  new Vector3(bb.Max.ToXNA(), 0));
        }

        public static XNABoundingBox ToXNA(this BoundingCube bb)
        {
            return new XNABoundingBox(bb.Min.ToXNA(), bb.Max.ToXNA());
        }

        public static BoundingCube ToCharybdis3(this XNABoundingBox bb)
        {
            return new BoundingCube(bb.Min.ToCharybdis(), bb.Max.ToCharybdis());
        }

        public static CharybdisBoundingBox ToCharybdis(this XNABoundingBox bb)
        {
            return new CharybdisBoundingBox(new Vec2(bb.Min.X, bb.Min.Y), new Vec2(bb.Max.X, bb.Max.Y));
        }

        #endregion
    }
}