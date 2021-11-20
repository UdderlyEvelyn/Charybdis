using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Charybdis.Science;
using Charybdis.MonoGame;

namespace Fortress
{
    public class Tile : FortressObject
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
                    Position = (Visual as Sprite).Position,
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