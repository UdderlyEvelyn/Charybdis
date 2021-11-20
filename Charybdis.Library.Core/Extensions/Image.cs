using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Charybdis.Library.Core
{
    public static class ImageLibrary
    {
        public static void SaveImage(this Array2<byte> data, string filename)
        {
            Bitmap bitmap = new Bitmap(data.Width, data.Height);
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    int h = (int)data.Get(x, y);
                    bitmap.SetPixel(x, y, Color.FromArgb(h, h, h));
                }
            }
            bitmap.Save(filename, ImageFormat.Png);
        }

        public static void SaveTexture(this Array2<byte> data, string filename, float colorRatio = .7f)
        {
            Bitmap bitmap = new Bitmap(data.Width, data.Height);
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    float heightRatio = 1 - colorRatio;
                    byte value = Maths.Clamp(data.Get(x, y), (byte)0, (byte)255);
                    int r = (value > 150 && value <= 200) ? 180 : (value > 200 ? value : 0);
                    int g = (value > 150 && value <= 200) ? 180 : (value > 200 ? 255 : 100);
                    int b = (value <= 90 ? 255 : (value > 150 && value <= 200) ? 180 : (value > 200 ? 255 : 0));
                    bitmap.SetPixel(x, y, Color.FromArgb((int)(r * colorRatio + value * heightRatio), (int)(g * colorRatio + value * heightRatio), (int)(b * colorRatio + value * heightRatio)));
                }
            }
            bitmap.Save(filename, ImageFormat.Png);
        }

        public static Color HeightColor(this byte h, float colorRatio = .7f)
        {
            float heightRatio = 1 - colorRatio;
            byte value = Maths.Clamp(h, (byte)0, (byte)255);
            int r = (value > 150 && value <= 200) ? 180 : (value > 200 ? value : 0);
            int g = (value > 150 && value <= 200) ? 180 : (value > 200 ? 255 : 100);
            int b = (value <= 90 ? 255 : (value > 150 && value <= 200) ? 180 : (value > 200 ? 255 : 0));
            return Color.FromArgb((int)(r * colorRatio + value * heightRatio), (int)(g * colorRatio + value * heightRatio), (int)(b * colorRatio + value * heightRatio));
        }

        public static void PlotFunctionToImage(this Func<double, double> f, string filename, int width, int height, Color color, double accuracy = .005, bool interpolate = false)
        {
            PlotFunctionToBitmap(f, width, height, color, accuracy, interpolate).Save(filename, ImageFormat.Png);
        }

        public static Bitmap DrawAxisToBitmap(this Bitmap b, Color c)
        {
            int width = b.Width;
            int height = b.Height;
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            for (int y = 0; y < height; y++)
            {
                b.SetPixel(halfWidth, y, c);
            }
            for (int x = 0; x < width; x++)
            {
                b.SetPixel(x, halfHeight, c);
            }
            return b;
        }

        public static Bitmap PlotFunctionToBitmap(this Func<double, double> f, int width, int height, Color color, double accuracy = .005, bool interpolate = false, Bitmap existingBitmap = null)
        {
            int halfWidth = width / 2;
            int halfHeight = height / 2;
            Bitmap bitmap = new Bitmap(width, height);
            if (existingBitmap != null)
            {
                bitmap = existingBitmap;
                if (bitmap.Width != width || bitmap.Height != height) throw new ArgumentException("The existing bitmap passed to this function does not have dimensions matching the other arguments.");
            }
            else bitmap.DrawAxisToBitmap(Color.Black);
            double oldValue = double.NaN;
            for (double x = -halfWidth + accuracy; x < halfWidth - accuracy; x += accuracy)
            {
                double value = Math.Max(0, Math.Min(height, (((int)f(x) / height) + halfHeight)));
                if (value > 0 && value < height - 1) bitmap.SetPixel((int)x + halfWidth, (int)value, color);
                if (!double.IsNaN(oldValue) && interpolate)
                {
                    for (double i = 0; i <= 1; i += accuracy)
                    {
                        if (!(oldValue > halfHeight && value < halfHeight)) //Console.WriteLine("FAILBOAT AT X=" + x + " (value=" + (int)Interpolation.LinearInterpolate(oldValue, value, i) + ")!");
                            bitmap.SetPixel((int)x + halfWidth, Math.Max(0, Math.Min(height - 1, (int)Interpolation.LinearInterpolate(oldValue, value, i))), color);
                    }
                }
                oldValue = value;
            }
            return bitmap;
        }

        public static Bitmap PlotFunctionsToBitmap(this Dictionary<Func<double, double>, Color> list, int width, int height, double accuracy = .005, bool interpolate = false)
        {
            Bitmap b = new Bitmap(width, height);
            b.DrawAxisToBitmap(Color.Black);
            foreach (KeyValuePair<Func<double, double>, Color> kvp in list)
            {
                PlotFunctionToBitmap(kvp.Key, width, height, kvp.Value, accuracy, interpolate, b);
            }
            return b;
        }

        public static void PlotFunctionsToImage(this Dictionary<Func<double, double>, Color> list, int width, int height, string filename, double accuracy = .005, bool interpolate = false)
        {
            Bitmap b = new Bitmap(width, height);
            b.DrawAxisToBitmap(Color.Black);
            foreach (KeyValuePair<Func<double, double>, Color> kvp in list)
            {
                PlotFunctionToBitmap(kvp.Key, width, height, kvp.Value, accuracy, interpolate, b);
            }
            b.Save(filename, ImageFormat.Png);
        }

        public static void TrySetPixel(this Bitmap b, int x, int y, Color c)
        {
            try
            {
                b.SetPixel(x, y, c);
            }
            catch
            {
                Console.WriteLine("Failed pixel set at " + x + ", " + y + " in " + b.Width + "x" + b.Height + " image.");
            }
        }
    }
}
