using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Charybdis.Science;
using Charybdis.MonoGame;

namespace Kolony
{
    public class Tile : KolonyObject
    {
        public Tile()
        {
            SelectionEnabled = true;
        }

        public Material Material;

        public void UpdateVisuals(SurroundingTiles st, bool applyTemperatureColor = false)
        {
            if (Visual == null)
            {
                Visual = new Sprite(Material.Texture)
                {
                    Position = Position,
                    Scale = new Vec2(1.1f, .9f),
                    Rotation = 0.78539816f,
                    Tint = applyTemperatureColor ? Col3.Average(Material.Color, Temperature.GetColor()) : Material.Color
                };
            }
            else
            {
                var s = Visual as Sprite;
                s.Tint = applyTemperatureColor ? Col3.Average(Material.Color, Temperature.GetColor()) : Material.Color;
            }
        }

        public override void Update()
        {
            /*Nothing to see here.*/
        }
    }
}