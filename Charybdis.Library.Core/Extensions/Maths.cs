using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public static class Maths
    {
        public const double H = 6.6260693e-34; //Planck's Constant (Joule-Seconds)
        public const double K = 1.380658e-23; //Boltzmann's Constant
        public const double C = 299792458; //Speed of Light In Vacuum (Meters Per Second)
        public const double C_SQ = C * C; //Speed of Light Squared (precalculated to avoid repeatedly calculating it)

        public static double InverseSquare(double value, double distance, double receivingSurfaceArea)
        {
            return value / receivingSurfaceArea;
            //I = P/A = P/4pir^2
        }

        //No damn idea how this works, but I blindly rewrote it for C#. :D
        //http://www.spectralcalc.com/blackbody/CalculatingBlackbodyRadianceV2.pdf
        //Outputs W m^-2 sr^-1 Hz^-1 units..
        public static double SpectralRadiance(double sigma, double temperature)
        {
            double c1 = H * C / K;
            double x = c1 * 100 * sigma / temperature;
            double x2 = x * x;
            double x3 = x2 * x;
            int iterations = (int)Math.Max(2 + 20 / x, 512); //Maybe original source made a mistake and this should be a "min" function?
            double sum = 0;
            for (int n = 1; n < iterations; n++)
            {
                double dn = 1 / n;
                sum += Math.Exp(-n * x) * (x3 + (3 * x2 + 6 * (x + dn) * dn) * dn) * dn;
            }
            double c2 = 2 * H * C_SQ;
            return c2 * Math.Pow(temperature / c1, 4) * sum;
        }

        public static double SphereArea(double radius)
        {
            return 4 * Math.PI * (radius * radius);
        }

        public static float Saturate(float f)
        {
            if (f > 1) return 1;
            if (f < 0) return 0;
            return f;
        }

        #region Clamp

        public static double Clamp(double d, double min, double max)
        {
            return Math.Max(Math.Min(d, max), min);
        }

        public static float Clamp(float f, float min, float max)
        {
            return Math.Max(Math.Min(f, max), min);
        }

        public static int Clamp(int i, int min, int max)
        {
            return Math.Max(Math.Min(i, max), min);
        }

        public static byte Clamp(byte b, byte min, byte max)
        {
            return Math.Max(Math.Min(b, max), min);
        }

        public static Vec2 Clamp(Vec2 v, int min, int max)
        {
            return new Vec2(Clamp(v.X, min, max), Clamp(v.Y, min, max));
        }

        public static Vec3 Clamp(Vec3 v, float min, float max)
        {
            return new Vec3(Clamp(v.X, min, max), Clamp(v.Y, min, max), Clamp(v.Z, min, max));
        }

        #endregion

        #region LinearAddress

        public static double LinearAddress(double x, double y, int width)
        {
            return Math.Max((int)y * width + (int)x, 0);
        }

        public static float LinearAddress(float x, float y, int width)
        {
            return Math.Max((int)y * width + (int)x, 0);
        }

        public static int LinearAddress(int x, int y, int width)
        {
            return Math.Max(y * width + x, 0);
        }

        public static byte LinearAddress(byte x, byte y, int width)
        {
            return (byte)Math.Max(y * width + x, 0);
        }

        public static float LinearAddress(Vec2 v, int width)
        {
            return Math.Max(v.Y * width + v.X, 0);
        }

        #endregion

        #region EuclideanAddress

        public static Vec2 EuclideanAddress(int linearAddress, int width)
        {
            int x = linearAddress % width;
            int y = (linearAddress - x) / width;
            return new Vec2(x, y);
        }

        #endregion

        public static double DegToRad(this double angle)
        {
            return angle * (Math.PI / 180);
        }

        public static double RadToDeg(this double angle)
        {
            return angle * (180 / Math.PI);
        }

        #region Inside

        public static bool Inside(this float f, float min, float max)
        {
            return f > min && f < max;
        }

        public static bool Inside(this double d, double min, double max)
        {
            return d > min && d < max;
        }

        public static bool Inside(this int i, int min, int max)
        {
            return i > min && i < max;
        }

        public static bool Inside(this byte b, byte min, byte max)
        {
            return b > min && b < max;
        }

        public static bool Inside(this Vec2 v, float min, float max)
        {
            return v.X > min && v.X < max && v.Y > min && v.Y < max;
        }

        public static bool Inside(this Vec3 v, float min, float max)
        {
            return v.X > min && v.X < max && v.Y > min && v.Y < max && v.Z > min && v.Z < max;
        }

        public static bool Inside(this Vec2 v, Vec2 min, Vec2 max)
        {
            return v.X > min.X && v.X < max.X && v.Y > min.Y && v.Y < max.Y;
        }

        public static bool Inside(this Vec3 v, Vec3 min, Vec3 max)
        {
            return v.X > min.X && v.X < max.X && v.Y > min.Y && v.Y < max.Y && v.Z > min.Z && v.Z < max.Z;
        }

        #endregion

        #region Outside

        public static bool Outside(this float f, float min, float max)
        {
            return !f.Inside(min, max);
        }

        public static bool Outside(this double d, double min, double max)
        {
            return !d.Inside(min, max);
        }

        public static bool Outside(this int i, int min, int max)
        {
            return !i.Inside(min, max);
        }

        public static bool Outside(this byte b, byte min, byte max)
        {
            return !b.Inside(min, max);
        }

        public static bool Outside(this Vec2 v, float min, float max)
        {
            return !v.Inside(min, max);
        }

        public static bool Outside(this Vec3 v, float min, float max)
        {
            return !v.Inside(min, max);
        }

        public static bool Outside(this Vec2 v, Vec2 min, Vec2 max)
        {
            return !v.Inside(min, max);
        }

        public static bool Outside(this Vec3 v, Vec3 min, Vec3 max)
        {
            return !v.Inside(min, max);
        }

        #endregion

        public static double GetHypotenuse(double a, double b)
        {
            return Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }

        #region Circle Math

        public static Vec2 CirclePointAtAngle(float radius, float angle)
        {
            var radians = DegToRad(angle);
            return new Vec2(radius * (float)Math.Cos(radians), radius * (float)Math.Sin(radians));
        }

        public static float CirclePosition(float radius, float angle)
        {
            var radians = DegToRad(angle);
            return (float)Math.Sin(radians) * radius;
        }

        public static float CircleRadialAngle(float radius, float x)
        {
            return (float)Math.Asin(x / radius);
        }

        #endregion

        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public static double SigmoidB(double x)
        {
            return 1 / Math.Exp(-x+5);
        }

        public static double? Error(double value, double? expected = null)
        {
            if (expected.HasValue)
                return -Math.Pow(Math.Abs(value - expected.Value), 2);
            else
                return null;
        }

        public static float GetBellCurveValue(float x, float min, float max, float avg)
        {
            //min + ((max - min) * sin(x / avg))
            return (float)(min + ((max - min) * Math.Sin(x / avg)));
        }

        public static float Lerp(float min, float max, float percent)
        {
            return min + ((max - min) * percent);
        }
    }
}