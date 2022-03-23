using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

namespace Kolony
{
    public class Material
    {
        public string Name;
        public float Density;
        public Col3 Color;
        public Texture2D TopTexture;
        public Texture2D SideTexture;

        public static Material Stone = new Material
        {
            Density = 4,
            Color = Col3.White,
            Name = "Stone",
        };
    }
}
