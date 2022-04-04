using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Charybdis.Science;
using Charybdis.MonoGame;
using Charybdis.Library.Core.Classes;
using Range = Charybdis.Library.Core.Range;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Space
{
    public class Star : SpaceObject
    {
        public List<SpaceObject> Orbitals = new List<SpaceObject>();

        public Star(Vec2 position, Drawable2 visual, Random r)
        {
            SelectionEnabled = true;
            Name = GenerateName(r);
            Temperature = new Temperature(Maths.Clamp(r.NextGaussian(6000, 33000), 1000, 40000));
            Visual = visual;
            Position = position;
            var spriteVisual = Visual as Sprite;
            if (spriteVisual != null)
                spriteVisual.Tint = Temperature.GetColor();
            var extent = visual.Position + visual.Size;
            BoundingRect = new (visual.Position.ToPoint(), new Microsoft.Xna.Framework.Point((int)extent.X, (int)extent.Y));
        }

        private Temperature _temperature;
        public Temperature Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                _starSpectralClass = _spectralClassTemperatureRanges.Single(sctr => sctr.Key.Contains(Temperature.Value.Round(0))).Value;
            }
        }

        public Element FusionProduct { get; set; }

        private Dictionary<Range, StarSpectralClass> _spectralClassTemperatureRanges = new Dictionary<Range, StarSpectralClass>
        {
            { new Range(0, 3500), StarSpectralClass.M },
            { new Range(3501, 5000), StarSpectralClass.K },
            { new Range(5001, 6000), StarSpectralClass.G },
            { new Range(6001, 7500), StarSpectralClass.F },
            { new Range(7501, 11000), StarSpectralClass.A },
            { new Range(11001, 25000), StarSpectralClass.B },
            { new Range(25001, 40000), StarSpectralClass.O }, //Technically doesn't end at 40000K, that's just the color func's limit.
        };

        private StarSpectralClass? _starSpectralClass = null;
        public StarSpectralClass SpectralClass
        {
            get
            {
                if (!_starSpectralClass.HasValue)
                    _starSpectralClass = _spectralClassTemperatureRanges.Single(sctr => sctr.Key.Contains(Temperature.Value)).Value;
                return _starSpectralClass.Value;
            }
        }

        private static List<string> _usedNames = new List<string>();

        public static string GenerateName(Random r)
        {
            var prefixChance = r.NextDouble();
            var suffixChance = r.NextDouble();
            var vowelSyllableChance = r.NextDouble();
            var greekChance = r.NextDouble();

            string s = (greekChance > .7 ? Data.GreekLetters.ElementAt(r.Next(0, Data.GreekLetters.Count - 1)).Name + " " : "") + //Greek Prefix
                                                                                                                                  //Generated Name
                       Processing.Capitalize((prefixChance > .5 ? Data.ConsonantSyllables.ElementAt(r.Next(0, Data.ConsonantSyllables.Count - 1)) : "") + (vowelSyllableChance > .5 ? Data.VowelSyllables.ElementAt(r.Next(0, Data.VowelSyllables.Count - 1)) : Data.Vowels.ElementAt(r.Next(0, Data.Vowels.Count - 1)).ToString()) + (suffixChance > .5 ? Data.ConsonantSyllables.ElementAt(r.Next(0, Data.ConsonantSyllables.Count - 1)) : ""));

            if (_usedNames.Contains(s))
                return GenerateName(r);
            else
            {
                _usedNames.Add(s);
                return s;
            }
        }

        public override void Update()
        {

        }
    }

    public enum StarSpectralClass
    {
        O,
        B,
        A,
        F,
        G,
        K,
        M,
    }
}
