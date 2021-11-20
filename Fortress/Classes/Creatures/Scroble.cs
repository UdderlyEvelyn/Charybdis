using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Science;
using Charybdis.MonoGame;

namespace Fortress
{
    public class Scroble : Creature
    {
        public static Texture2D Texture;
        public static readonly double MaxCalories = 1000;
        public double Calories = MaxCalories;
        public static readonly double ActionCalories = 200;
        public static readonly double HungryCalories = 650;
        public static readonly double MetabolismCalories = 2.5;
        public Array2<Tile> World;

        public Scroble(Array2<Tile> world, Vec2 startingPosition, Func<Tile, bool> passabilityCheck)
            : base(world, startingPosition, passabilityCheck)
        {
            World = world;
            Visual = new Sprite(Texture) { Position = startingPosition };
            Temperature = new TemperatureF(80);

            TileInteraction = delegate(Tile t)
            {
                if (t.Material == Material.Water) //If it's in water..
                {
                    lock (t)
                    {
                        t.Material = Material.Dioxygen; //It performs electrolysis, burns the hydrogen for calories, and ejects the oxygen.
                        Calories += t.Mass; //Absorb calories.
                        Temperature.Value += .1 * t.Mass; //Heat up commensurate with hydrogen use (10% mass of water used).
                        t.Mass -= t.Mass * .02; //2% Waste.
                    }
                }
                else
                {
                    var st = new SurroundingTiles(World, t);
                    var nearWater = st.All.Where(tile => tile.Material == Material.Water).OrderByDescending(tile => tile.Mass);
                    if (nearWater.Count() > 0)
                        Visual.Position = nearWater.First().Visual.Position; //Go to largest mass of nearby water.
                    else //Wander toward cold since heat rises and water tends to settle on bottom.
                        Visual.Position = st.All.OrderBy(tile => tile.Temperature.Value).First().Visual.Position;
                }
            };
        }

        public override void Update()
        {
            Calories -= MetabolismCalories;
            if (Calories <= HungryCalories) //If Hungry..
            {
                (Visual as Sprite).Tint = Col3.Red; //Tint Red, Show Hunger.
                TileInteraction(World.Get(Visual.Position.Xi / 32, Visual.Position.Yi / 32));
            }
            else
            {
                (Visual as Sprite).Tint = Col3.Green; //Tint Green, Show Contentment.
            }
            base.Update();
        }
    }
}