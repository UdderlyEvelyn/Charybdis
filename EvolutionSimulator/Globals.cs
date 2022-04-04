using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Charybdis.MonoGame;
using System.Collections.Concurrent;
using System.Threading;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace EvolutionSimulator
{
    public class Globals : Charybdis.MonoGame.Globals
    {        
        public static ReaderWriterLockSlim CreaturesLock = new ReaderWriterLockSlim();
        public static List<Creature> Creatures = new List<Creature>();
        public static ConcurrentBag<Creature> NewlyBornCreatures = new ConcurrentBag<Creature>();
        public static ConcurrentBag<Creature> NewlyDeadCreatures = new ConcurrentBag<Creature>();
        public static ConcurrentDictionary<Title, Creature> TitleHolders = new ConcurrentDictionary<Title, Creature>();
        public static ReaderWriterLockSlim MostSuccessfulCreaturesLock = new ReaderWriterLockSlim();
        public static List<Creature> MostSuccessfulCreatures = new List<Creature>();
        public static List<Tile> Tiles = new List<Tile>();

        public static Array2<Tile> World;
        public static int TileSize = 48; //64
        public static int WorldWidth = (int)Math.Floor((double)Width / TileSize);
        public static int WorldHeight = (int)Math.Floor((double)Height / TileSize);
        public static int WorldPixelWidth = WorldWidth * TileSize;
        public static int WorldPixelHeight = WorldHeight * TileSize;

        public static int CreatureSize = 24;//TileSize / 4; //16
        public static double CreatureSizeMutabilityChangeMultiplier = .001;
        public static int MinimumCreaturesInWorld = 1;
        public static int SuccessfulCreatureHistoryLength = 10; //20
        public static int CreatureInjectionCount = SuccessfulCreatureHistoryLength; //SuccessfulCreatureHistoryLength * 5; //20
        public static bool BiomassRefillOnInjection = CreatureInjectionCount >= MinimumCreaturesInWorld;
        public static int StartingCreatureCount = SuccessfulCreatureHistoryLength * 5; //MinimumCreaturesInWorld
        public static int SuccessfulCreatureHistoryDisplayLength = SuccessfulCreatureHistoryLength; //20
        public static int SuccessfulCreatureHistoryCullingThreshold = SuccessfulCreatureHistoryLength / 2; //10
        public static bool SuccessfulCreatureSpeciesGenerationExampleReduction = false;
        public static int SuccessfulCreatureSpeciesGenerationExampleLimit = SuccessfulCreatureHistoryLength / 4; //2 at default history length of 20.
        public static double MedianFitness = 0;
        public static double LowerHalfMeanFitness = 0;
        public static double PercentageBelowLowerHalfMeanFitnessDeletionThreshold = .2;
        public static bool HardshipSystemEnabled = false;
        public static bool ProgenyTrackingEnabled = false;
        public static bool FitnessBasedEvolution = true;

        public static Array2<byte> Heightmap;
        public static int HeightmapPerlinOctaves = 3;
        public static byte HeightmapMaximumHeight = 255;
        public static byte Sealevel = 70;
        public static byte SteepHeightThreshold = 180;
        public static byte SteepSpace = (byte)(255 - SteepHeightThreshold);
        public static byte MinimumSteepColor = 140;
        public static byte RemainingSteepColorSpace = (byte)(255 - MinimumSteepColor);

        public static double YearLength = 10000;
        public static double LifespanToSizeRatio = 1875;
        public static double MaturityThresholdToSizeRatioThreshold = .01;
        public static double BaseTemperatureVariation = .75; //Y-Intercept
        public static double SeasonFrequency = Math.PI; //Slope
        public static double SeasonToYearRatio = .25;
        //This is the output of the seasonal variation function that takes in the above variables.
        public static double SeasonalTemperatureMultiplier = BaseTemperatureVariation;
        public static float MinimumAmbientTemperature = 32;
        public static float AverageAmbientTemperature = 65;
        public static float MaximumAmbientTemperature = 100;

        public static double Friction = .80;

        public static double StartingBiomassPercentage = 100;
        public static double MetabolicRateToSizeRatio = 16000; //16000
        public static double MetabolicReliefPercentage = .25;
        public static double EatingEnergyCost = .02;

        public static double MinimumCreatureSize = 4;
        public static double MaximumCreatureSize = 72;
        public static double EnergyToSizeRatio = 62.5;
        public static double StartingEnergyPercentage = .1;
        public static double BusyMovementCostMultiplier = 2;
        public static double RegrowthMultiplier = .000125;//.00005; //.000125; //.0005;
        public static double BaseConsumptionAmount = .25;
        public static ulong EatingCooldown = 50;
        public static double SharedTileConsumptionPenalty = 2;

        public static double BreedingEnergyCostMultiplier = .6; //.8
        public static double EggHatchThresholdToSizeRatio = 9.375;
        public static bool UseEggs = true;
        public static byte EggsPerSpawning = (byte)Math.Max(1, (BreedingEnergyCostMultiplier / StartingEnergyPercentage).Round(places: 0));

        public static int NeuralNetworkHiddenLayers = 2;
        public static int NeuralNetworkWidth = 2 + NeuralNetworkHiddenLayers; //Input + Hidden + Output
        public static int NeuralNetworkHeight = Math.Max((int)Creature.Inputs.Count, (int)Creature.Outputs.Count);
        //public static int MaximumMutationEvents = (int)((NeuralNetworkWidth * NeuralNetworkHeight * 2.1).Round(places: 0));
        public static double BaseMutationChance = .15;//.15;

        //public static bool IndividualsCanLearn = true;

        public static bool ShowNeuralNetworkData = false;
        public static bool ShowTitleHolderGraphs = false;
        public static bool ShowTitleHolderLabels = true;

        public static bool ParallelNeuralNetworkUpdateMethod
        {
            get
            {
                return Charybdis.Neural.Globals.ParallelNetworkUpdateMethod;
            }
            set
            {
                Charybdis.Neural.Globals.ParallelNetworkUpdateMethod = value;
            }
        }

        public static float PeripheralVisionAngle = 45f;
        public static float PeripheralFarVisionAngleMultiplier = .75f;
        public static float PeripheralFarVisionAngle = PeripheralVisionAngle * PeripheralFarVisionAngleMultiplier;
        public static double FarVisionDistanceMultiplier = 1.75;
        public static double PeripheralVisionDistance = (int)((TileSize * 1.2).Round(places: 0));
        public static double PeripheralFarVisionDistance = (int)((PeripheralVisionDistance * FarVisionDistanceMultiplier).Round(places: 0));
        public static double CentralVisionDistance = (int)((TileSize * 1.1).Round(places: 0));
        public static double CentralFarVisionDistance = (int)((CentralVisionDistance * FarVisionDistanceMultiplier).Round(places: 0));

        public static Tile FindTileForCreature(Creature c)
        {
            return FindTileForPosition(c.Position);
        }

        public static Tile FindTileForPosition(Vec2 v)
        {
            try
            {
                var nearestX = (int)Math.Floor(v.X / TileSize);
                var nearestY = (int)Math.Floor(v.Y / TileSize);
                //The last position in a row or column will try to access a tile that doesn't exist because it maps to the nearest TileSize as the one that would exist if the grid kept going.
                if (nearestX == WorldWidth) //This one is probably silent and wraps around to stealing from the next tile.
                    nearestX--;
                if (nearestY == WorldHeight)
                    nearestY--;
                return World.Get(nearestX, nearestY);
            }
            catch
            {
                return null;
            }
        }

        public static Vec2 WorldWrapIfNecessary(Vec2 position, float deltaX, float deltaY)
        {
            var newX = position.X.Wrap(deltaX, 0, WorldPixelWidth);
            var newY = position.Y.Wrap(deltaY, 0, WorldPixelHeight);
            return new Vec2(newX, newY);
        }

        public static Vec2 WorldWrapIfNecessary(Vec2 position, Vec2 delta)
        {
            return WorldWrapIfNecessary(position, delta.X, delta.Y);
        }

        public static Vec2 WorldClamp(Vec2 position)
        {
            return new Vec2(Maths.Clamp(position.X, 0, WorldPixelWidth), Maths.Clamp(position.Y, 0, WorldPixelHeight));
        }

        public static IEnumerable<Creature> GetNearbyCreatures(Creature creature, bool inEgg = false)
        {
            CreaturesLock.EnterReadLock();
            var result = Creatures.Where(c => c.InEgg == inEgg && c != creature && c.Position.Distance(creature.Position) < TileSize);
            CreaturesLock.ExitReadLock();
            return result;
        }

        public static int GetNearbyCreatureCount(Creature creature)
        {
            CreaturesLock.EnterReadLock();
            var result = Creatures.Count(c => !c.InEgg && c != creature && c.Position.Distance(creature.Position) < TileSize);
            CreaturesLock.ExitReadLock();
            return result;
        }

        public static int GetNearbyEggCount(Creature creature)
        {
            CreaturesLock.EnterReadLock();
            var result = Creatures.Count(c => c.InEgg && c != creature && c.Position.Distance(creature.Position) < TileSize);
            CreaturesLock.ExitReadLock();
            return result;
        }
    }

    public enum Title : int
    {
        Oldest,
        Largest,
        Smallest,
        LongestLineage,
        MostFit,
        Explorer,
    }
}
