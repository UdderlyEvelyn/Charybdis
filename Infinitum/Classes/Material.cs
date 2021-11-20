using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Science;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using Charybdis.Library.Core;

namespace Infinitum
{
    public class Material
    {
        //All blocks are assumed to be 600mm thick for purposes of heat conductivity/insulation calculations.
        public string Name;
        public float Density;
        public Temperature SublimationPoint = null;
        public Temperature BoilingPoint = null;
        public Material BoilsInto = null;
        public Temperature MeltingPoint = null;
        public Material MeltsInto = null;
        public Temperature CondensationPoint = null;
        public Material CondensesInto = null;
        public Temperature FreezingPoint = null;
        public Material FreezesInto = null;
        public float HeatConductivity = 1; //Default to 1.
        public MaterialPhase Phase = MaterialPhase.None;
        public Texture2D Texture;
        public Col3 Color;
        /// <summary>
        /// Whether the material can put out heat.
        /// </summary>
        public bool DispersesHeat = true;
        /// <summary>
        /// Whether the material loses heat when it puts heat out.
        /// </summary>
        public bool LosesHeat = true;
        /// <summary>
        /// Whether the material gains heat when it absorbs heat from another tile.
        /// </summary>
        public bool GainsHeat = true;
        /// <summary>
        /// Whether the material can absorb heat from another tile.
        /// </summary>
        public bool AbsorbsHeat = true;

        public static readonly Material Stone = new Material
        {
            Phase = MaterialPhase.Solid,
            MeltingPoint = new TemperatureF(2000),
            Name = "Ambiguous Stone",
            Density = 4,
            HeatConductivity = .2f,
            Color = new Col3(80, 80, 80),
        };

        public static readonly Material Permanent = new Material
        {
            Name = "Permanent",
            Density = 666,
            LosesHeat = false,
            AbsorbsHeat = false,
            GainsHeat = false,
            Color = Col3.White,
        };

        public static readonly Material Gas = new Material
        {
            Phase = MaterialPhase.Gas,
            Name = "Mystery Gas",
            Density = 1,
            HeatConductivity = .7f, //Needs to be per-gas-element/molecule, but for now this is reasonable.
            Color = Col3.MagicPink,
        };

        public static readonly Material Vacuum = new Material
        {
            Name = "Vacuum",
            Density = 0,
            GainsHeat = false,
            LosesHeat = false,
            Color = Col3.Black,
        };

        public static readonly Material Liquid = new Material
        {
            Phase = MaterialPhase.Liquid,
            Name = "Mystery Liquid",
            Density = 1.5f,
            HeatConductivity = .65f,
            Color = Col3.Yellow,
        };

        #region Water
        public static readonly Material Water = new Material
        {
            Phase = MaterialPhase.Liquid,
            Name = "Water",
            Density = 1000f, //g/L
            HeatConductivity = .85f, //Bullshit Number
            BoilingPoint = new TemperatureF(212),
            BoilsInto = Steam,
            FreezingPoint = new TemperatureF(32),        
            FreezesInto = WaterIce,
            Color = Col3.Blue,
        };

        public static readonly Material Steam = new Material
        {
            Phase = MaterialPhase.Gas,
            Name = "Steam",
            Density = 900f, //Bullshit Number
            HeatConductivity = .65f, //Bullshit Number
            CondensationPoint = new TemperatureF(211),
            CondensesInto = Water,
            Color = new Col3(Col3.Blue.R - 20, Col3.Blue.G - 20, Col3.Blue.B - 20),
        };

        public static readonly Material WaterIce = new Material
        {
            Phase = MaterialPhase.Solid,
            Name = "Water Ice",
            Density = 950f, //Bullshit Number
            HeatConductivity = .45f, //Bullshit Number
            MeltingPoint = new TemperatureF(33),
            MeltsInto = Water,
            Color = new Col3(Col3.Blue.R - 40, Col3.Blue.G - 40, Col3.Blue.B - 40),
        };
        #endregion

        #region Hydrogen
        public static readonly Material Hydrogen = new Material
        {
            Phase = MaterialPhase.Gas,
            Name = "Hydrogen",
            Density = .08988f,
            HeatConductivity = .45f, //Bullshit Number
            CondensationPoint = new TemperatureF(-423.181),
            CondensesInto = LiquidHydrogen,
            Color = new Col3(Col3.Red.R - 20, Col3.Red.G - 20, Col3.Red.B - 20),
        };

        public static readonly Material LiquidHydrogen = new Material
        {
            Phase = MaterialPhase.Liquid,
            Name = "Liquid Hydrogen",
            Density = .09f,
            HeatConductivity = .65f, //Bullshit Number
            BoilingPoint = new TemperatureF(-423.182),
            BoilsInto = Hydrogen,
            FreezingPoint = new TemperatureF(-434.5),
            FreezesInto = SolidHydrogen,
            Color = Col3.Red,
        };

        public static readonly Material SolidHydrogen = new Material
        {
            Phase = MaterialPhase.Solid,
            Name = "Solid Hydrogen",
            Density = .1f, //g/L
            HeatConductivity = .25f, //Bullshit Number
            MeltingPoint = new TemperatureF(-434.49),
            MeltsInto = LiquidHydrogen,
            Color = new Col3(Col3.Red.R - 40, Col3.Red.G - 40, Col3.Red.B - 40),
        };
        #endregion

        #region Oxygen
        public static readonly Material Dioxygen = new Material
        {
            Phase = MaterialPhase.Gas,
            Name = "Oxygen", //Dioxygen is a name nobody will recognize at a glance.
            Density = 1.429f, //g/L
            HeatConductivity = .55f, //Bullshit Number
            CondensationPoint = new TemperatureF(-361.81),
            CondensesInto = LiquidOxygen,
            Color = new Col3(255, 255, 255),
        };

        public static readonly Material LiquidOxygen = new Material
        {
            Phase = MaterialPhase.Liquid,
            Name = "Liquid Oxygen",
            Density = 1141f, //g/L
            HeatConductivity = .65f, //Bullshit Number
            BoilingPoint = new TemperatureF(-297.332),
            BoilsInto = Dioxygen,
            FreezingPoint = new TemperatureF(-361.83),
            FreezesInto = SolidOxygen,
            Color = new Col3(200, 200, 200),
        };

        public static readonly Material SolidOxygen = new Material
        {
            Phase = MaterialPhase.Solid,
            Name = "Solid Oxygen",
            Density = 1652f, //Bullshit Number
            HeatConductivity = .45f, //Bullshit Number
            MeltingPoint = new TemperatureF(-361.82),
            MeltsInto = LiquidOxygen,
            Color = new Col3(150, 150, 150),
        };
        #endregion

        #region CarbonDioxide

        #endregion

        #region CarbonMonoxide

        #endregion

        public enum MaterialPhase
        {
            Plasma,
            Gas,
            Liquid,
            Solid,
            None,
        }
    }
}