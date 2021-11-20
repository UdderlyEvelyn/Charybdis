using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Science
{
    public static class Gravity
    {
        const double G = 6.6726e-11;

        public static double Between(double mass1, double mass2, double distance)
        {
            return (G * mass1 * mass2) / (distance * distance);
        }

        public static double Surface(double mass, double radius)
        {
            return G * mass / radius;
        }

        public static double Escape(double mass, double distanceToCenter)
        {
            return Math.Sqrt(2 * G * mass / distanceToCenter);
        }
    }
}
