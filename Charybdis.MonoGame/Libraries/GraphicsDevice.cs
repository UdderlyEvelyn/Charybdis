using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Charybdis.MonoGame
{
    public static class GraphicsDeviceExtensions
    {
        public static Texture2D Texture2DFromFile(this GraphicsDevice gd, string path)
        {
            using (var f = File.OpenRead(path))
                return Texture2D.FromStream(gd, f);
        }

        public static Font FontFromFile(this GraphicsDevice gd, string path, bool treatBlackAsTransparent = true)
        {
            Font f = new Font(path);
            f.Texture = gd.Texture2DFromFile(f.TexturePath);
            if (treatBlackAsTransparent)
                f.Texture.ReplaceColor(Color.Black, Color.Transparent);
            return f;
        }
    }
}
