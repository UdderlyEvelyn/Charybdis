using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charybdis.Library.Core
{
    public static class Interpolation
    {
        public static double CubicInterpolate(double pre0, double n0, double pre1, double n1, double a)
        {
            double p = (n1 - pre1) - (pre0 - n0);
            double q = (pre0 - n0) - p;
            double r = pre1 - pre0;
            double s = n0;
            double a2 = a * a;
            return (p * a2 * a) + (q * a2) + (r * a) + s;
        }

        public static double LinearInterpolate(double n0, double n1, double a)
        {
            return ((1.0 - a) * n0) + (a * n1);
        }

        public static double SCurve3(double a)
        {
            return (a * a * (3.0 - 2.0 * a));
        }

        public static double SCurve5(double a)
        {
            double a3 = a * a * a;
            double a4 = a3 * a;
            double a5 = a4 * a;
            return (6.0 * a5) - (15.0 * a4) + (10.0 * a3);
        }

        //This works but not amazingly.
        /*public static Array2<byte> TileHorizontal(this Array2<byte> data, int addedWidth)
        {
            //Copy the original array into a new one with the new dimensions.
            Array2<byte> result = new Array2<byte>(data.Width + addedWidth, data.Height);
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    result.Set(x, y, data.Get(x, y));
                }
            }

            //Create the new region.
            for (int y = 0; y < data.Height; y++)
            {
                byte start = data.Get(data.Width - 1, y);
                byte end = data.Get(0, y);
                double lastValue = start;
                double change = (end - start) / addedWidth;
                for (int dX = 0; dX < addedWidth; dX++)
                {
                    change = (end - lastValue) / (addedWidth - (dX - Globals.Random.NextDouble()));
                    lastValue += change + (change * Globals.Random.NextDouble());
                    double d = SCurve5(Globals.Random.NextDouble());
                    //Set the current point to noise from this new point applied at a random % of strength to the point and then re-applied at 5% strength to that point's original value again.
                    result.Set(data.Width + dX, y, (byte)((lastValue * .95 + (lastValue * (1 - d) + (Noise.GetOctaveNoise(data.Width + dX, y, new Noise.NoiseArgs()) * d) / 2) * .05)));
                }
            }

            //Blur only the new area and then replace the values with the new (blurred) ones.
            Array2<byte> blur = result.WeightedBlur(1, data.Width, 0);
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = data.Width; x < data.Width + addedWidth; x++)
                {
                    result.Set(x, y, blur.Get(x, y));
                }
            }
            
            return result;
        }*/
    }
}
