using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Charybdis.Science;
using Charybdis.MonoGame;

namespace Infinitum
{
    public abstract class Tile : DestroyableObject
    {
        public Tile(Material material, Vec2 position)
        {
            SelectionEnabled = true;
            Material = material;
            Visual = new Sprite(Material.Texture)
            {
                Position = position,
                Tint = Material.Color
            };
        }

        public Material Material;

        public override void Update()
        {
            /*Nothing to see here.*/
        }
    }
}