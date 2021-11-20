using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Runtime.InteropServices;
using XNARectangle = Microsoft.Xna.Framework.Rectangle;
using DrawingRectangle = System.Drawing.Rectangle;

namespace Charybdis.MonoGame
{
    public static class Texture2DExtensions
    {
        /// <summary>
        /// The BGR to RGB color matrix used to switch the blue and red colors from an image
        /// </summary>
        private static ColorMatrix _BgrToRgbColorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {0, 0, 1, 0, 0},
            new float[] {0, 1, 0, 0, 0},
            new float[] {1, 0, 0, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        /// <summary>
        /// My SaveAsPng function.
        /// </summary>
        /// <param name="stream">The stream to write the png to.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public static void MySaveAsPng(this Texture2D thisTexture2D, Stream stream, int width, int height)
        {
            var pixelData = new byte[thisTexture2D.Width * thisTexture2D.Height /**GraphicsExtensions.Size(thisTexture2D.Format)*/];
            thisTexture2D.GetData(pixelData);
            Bitmap bitmap = new Bitmap(thisTexture2D.Width, thisTexture2D.Height, PixelFormat.Format32bppArgb);
            BitmapData bmData = bitmap.LockBits(new DrawingRectangle(0, 0, thisTexture2D.Width, thisTexture2D.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            Marshal.Copy(pixelData, 0, bmData.Scan0, 4 * thisTexture2D.Width * thisTexture2D.Height);
            bitmap.UnlockBits(bmData);

            // Switch from BGR encoding to RGB
            using (ImageAttributes ia = new ImageAttributes())
            {
                ia.SetColorMatrix(_BgrToRgbColorMatrix);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(bitmap, new DrawingRectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, ia);
                }
            }

            bitmap.Save(stream, ImageFormat.Png);
        }

        //public enum Rotation : int
        //{
        //    Right = 90, //X = Y, Y = Height - X
        //    Left = -90, //X = Width - Y, Y = X
        //    Flip = 180, //X = Width - Y, Y = Height - X
        //}

        //public static Texture2D Rotate(this Texture2D thisTexture2D, Rotation rotation)
        //{
        //    Texture2D output = new Texture2D(thisTexture2D.GraphicsDevice, rotation != Rotation.Flip ? thisTexture2D.Height : thisTexture2D.Width, rotation != Rotation.Flip ? thisTexture2D.Width : thisTexture2D.Height);
        //    for (int y = 0; y < thisTexture2D.Height; y++)
        //    {
        //        for (int x = 0; x < thisTexture2D.Width; x++)
        //        {
        //            var tx = x;
        //            var ty = y;
        //            switch (rotation)
        //            {
        //                case Rotation.Right:
        //                    tx = y;
        //                    ty = output.Height - x;
        //                    break;
        //                case Rotation.Left:
        //                    tx = output.Width - y;
        //                    ty = x;
        //                    break;
        //                case Rotation.Flip:
        //                    tx = output.Width - y;
        //                    ty = output.Height - x;
        //                    break;
        //            }
        //            output.SetColor((byte)tx, (byte)ty, thisTexture2D.GetColor((byte)x, (byte)y));
        //        }
        //    }
        //    return output;
        //}

        public static Texture2D Clone(this Texture2D source)
        {
            Texture2D target = new Texture2D(source.GraphicsDevice, source.Width, source.Height);
            Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[source.Width * source.Height];
            source.GetData<Microsoft.Xna.Framework.Color>(data);
            target.SetData<Microsoft.Xna.Framework.Color>(data);
            return target;
        }
    }
}
