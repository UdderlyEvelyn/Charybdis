using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Charybdis.Science;
using Charybdis.MonoGame;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Space
{
    class Planet : SpaceObject
    {
        public const double EARTH_MASS_KILOGRAMS = 5.972e24;
        public const double JUPITER_MASS_KILOGRAMS = 317.83 * EARTH_MASS_KILOGRAMS;

        public Temperature Temperature { get; set; }
        public double Radius { get; set; }

        public Planet(Star hostStar, Drawable2 visual, Random r)
        {
            Anchor = hostStar;
            Name = GenerateName(r);
            Mass = r.NextDouble() * 28.5 * JUPITER_MASS_KILOGRAMS; //Most massive exoplanet is 28.5 Jupiter masses.
            Radius = r.NextDouble() * 6.9; //Largest exoplanet is a puffy gas giant at 6.9 Jupiter radii.
            Position = new Vec2((float)(hostStar.Position.X + Scale.ParsecsToSolarPixels(r.NextDouble())),
                                (float)(hostStar.Position.Y + Scale.ParsecsToSolarPixels(r.NextDouble())));
            Temperature = new Temperature(Maths.InverseSquare(hostStar.Temperature.Value, hostStar.Position.Distance(Position), Maths.SphereArea(Radius) / 2));
            Visual = visual;

            var spriteVisual = Visual as Sprite;
            if (spriteVisual != null)
                spriteVisual.Tint = Temperature.GetColor();
        }

        //private static List<string> _usedNames = new List<string>();

        private static ulong temporaryPlanetCounter = 0;
        public static string GenerateName(Random r)
        {
            return "Unnamed Planet " + ++temporaryPlanetCounter;
        }

        public override void Update()
        {

        }
    }
}
