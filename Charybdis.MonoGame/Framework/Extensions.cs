using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Charybdis.MonoGame
{
    public static class Extensions
    {
        public static float Cross(this Vector2 vector1, Vector2 vector2)
        {
            return vector1.X * vector2.Y - vector1.Y * vector2.X;
        }

        public static Vector3 Cross(this Vector3 vector1, Vector3 vector2)
        {
            return Vector3.Cross(vector1, vector2);
        }

        public static float Distance(this Vector2 vector1, Vector2 vector2)
        {
            return Vector2.Distance(vector1, vector2);
        }

        public static float Average(this Vector2 vector)
        {
            return (vector.X + vector.Y) / 2;
        }
    }
}
