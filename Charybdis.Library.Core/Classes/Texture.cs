using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Charybdis.Library.Core
{
    public class Texture
    {
        public int Width 
        {
            get
            {
                return Bitmap.Width;
            }
        }
        public int Height 
        {
            get
            {
                return Bitmap.Height;
            }
        }
        public Bitmap Bitmap { get; private set; }
        public int Handle { get; set; }
        private BitmapData _lock;

        public static Texture FromFile(string sourcePath)
        {
            Texture t = new Texture();
            if (System.IO.File.Exists(sourcePath))
            {
                try
                {
                    t.Bitmap = new Bitmap(sourcePath);
                    return t;
                }
                catch
                {
                    return null;
                }
            }
            else return null;
        }

        public IntPtr Lock()
        {
            if (_lock == null) return (_lock = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb)).Scan0;
            else return _lock.Scan0;
        }

        public void Unlock()
        {
            try
            {
                Bitmap.UnlockBits(_lock);
            }
            catch
            {
                //Pass
            }
        }
    }
}
