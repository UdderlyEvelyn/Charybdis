using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using Charybdis.MonoGame;

namespace Kolony
{
    public class Material
    {
        public string Name;
        public float Density;
        public Texture2D Texture;

        public Material(string name, float density, Texture2D texture = null)
        {
            Name = name;
            Density = density;
            Texture = texture;
        }

        public static Material Air = new Material("Air", 0);

        public static Material Stone = new Material("Stone", 4);

        public static Material Soil = new Material("Soil", 2);

        public static Material Sand = new Material("Sand", 1);

        public static Material Grey = new Material("Grey", 999);
    }
}
