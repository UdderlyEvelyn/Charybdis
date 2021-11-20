using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Science;
using Charybdis.Library.Core;
using Charybdis.MonoGame;

namespace EvolutionSimulator
{
    public class ArableTile : Tile
    {
        public const double TEMPERATURE_THRESHOLD_F = 60;
        public double Biomass { get; set; }
        public Temperature Temperature { get; set; }
        public double BiomassRatio
        {
            get
            {
                return Biomass / MaximumBiomass;
            }
        }
        public double BiomassPercentage
        {
            get
            {
                return Biomass / MaximumBiomass * 100;
            }
        }
        //public string LabelString
        //{
        //    get
        //    {
        //        return /*Height + "\n" + Temperature.Value.Round().ToString() + "\n" + (Temperature.Value * Globals.SeasonalTemperatureMultiplier).Round() + "\n" +*/ BiomassPercentage + " %\n" + LastGrowth.Round();
        //    }
        //}
        public double LastGrowth { get; set; }

        public double MaximumBiomass { get; set; }

        public ArableTile(float size, Col4 color) : base(size, color)
        {
            //Label l1 = new Label { DataSource = this, BindingProperty = typeof(ArableTile).GetProperty("LabelString"), Font = Globals.Font, Parent = Visual, Shadowed = true, Offset = new Vec2(0, -Globals.TileSize) };
            //Visual.Children.Add(l1);
        }

        public ArableTile(float size, Col4 color, byte height)
            : this(size, color)
        {
            Height = height;
            //temperature adjusted by height = 32 + ((100-32)*sin(x/65))
            Temperature = new Temperature(Maths.GetBellCurveValue(Height, Globals.MinimumAmbientTemperature, Globals.MaximumAmbientTemperature, Globals.AverageAmbientTemperature), Temperature.TemperatureScale.F);
            //bonus maximum biomass by season (where x is season mult) = ((x/(2^x))-.2)
            var scaledTemperature = Temperature.Value / 64 * Globals.TileSize;
            MaximumBiomass = scaledTemperature + (scaledTemperature / Math.Pow(2, Globals.SeasonalTemperatureMultiplier) - .4);
            LastGrowth = Biomass = MaximumBiomass;
        }

        public override void Update()
        {
            base.Update();
            //bonus maximum biomass by season (where x is season mult) = ((x/(2^x))-.2)
            var scaledTemperature = Temperature.Value / 64 * Globals.TileSize;
            MaximumBiomass = scaledTemperature + (scaledTemperature / Math.Pow(2, Globals.SeasonalTemperatureMultiplier) - .4);
            Biomass = Maths.Clamp(Biomass, 0, MaximumBiomass);
            if (Biomass < MaximumBiomass)
            {
                LastGrowth = Math.Max(.001, Temperature.Value * Globals.SeasonalTemperatureMultiplier * Globals.RegrowthMultiplier / 64 * Globals.TileSize);
                Biomass += LastGrowth;
            }
            //Update color based on temperature and biomass content.
            var r = (float)Maths.Clamp(10 + (Globals.SeasonalTemperatureMultiplier >= .75 ? MaximumBiomass / 255 * 60 : MaximumBiomass / 255 * 40), 0, 60);
            var g = (float)Maths.Clamp(Biomass / MaximumBiomass * (Globals.SeasonalTemperatureMultiplier * 135), 30, (Globals.SeasonalTemperatureMultiplier * 135));
            var b = (float)Maths.Clamp(10 + (Globals.SeasonalTemperatureMultiplier < .75 ? MaximumBiomass / 255 * 60 : MaximumBiomass / 255 * 40), 0, 60);
            ((Square)Visual).FillColor = new Col3(r, g, b);
        }
    }
}
