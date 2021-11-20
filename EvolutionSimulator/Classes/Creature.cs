using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Charybdis.MonoGame;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Charybdis.Neural;
using System.Collections.Concurrent;
using Charybdis.MonoGame;

namespace EvolutionSimulator
{
    public class Creature : GameObject2
    {
        public Drawable2 SelectionVisual = new Border(2) { Color = Col3.Green };

        public new bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                if (value)
                {
                    SelectionVisual.Parent = Visual;
                    Visual.Children.Add(SelectionVisual);
                }
                else
                {
                    SelectionVisual.Parent = null;
                    Visual.Children.Remove(SelectionVisual);
                }
                base.Selected = value;
            }
        }

        public bool Updating = false;

        public enum Inputs : int
        {
            Energy = 0,

            BiomassAtCurrentTile = 1,
            DifficultyAtCurrentTile = 2,

            BiomassInVision1 = 3,
            DifficultyInVision1 = 4,

            BiomassInVisionC = 5,
            DifficultyInVisionC = 6,

            BiomassInVisionL = 7,
            DifficultyInVisionL = 8,

            BiomassInVisionR = 9,
            DifficultyInVisionR = 10,

            NearbyCreatures = 11,
            Memory = 12,
            Sine = 13,
            Count = 14,
        }

        public enum Outputs : int
        {
            Direction = 0,
            Effort = 1,
            Memory = 2,
            FoodNoticed = 3,
            Breed = 4,
            Eat = 5,
            Count = 6,
        }
        //Old/Alternative Outputs

        private Random _random = new Random();
        public new Random Random
        {
            get
            {
                return _random;
            }
        }
        public Network Network;
        public double Energy;
        public double MaximumEnergy;
        private double _previousEnergy = 0;
        public MutableDouble MutationChance;
        public string Species;
        public float Direction;
        public MutableDouble Size;
        public ulong MaximumLifespan;
        public ulong MaturityThreshold;
        public double Momentum = 1;
        public ulong LastAte = 0;
        public double MetabolicRate;
        public ulong Hardship = 0;
        
        public Creature Parent = null;

        public const double ROTATION_COST_PER_STEP = .01; //.05 pre-1/12/19
        public const double MOVEMENT_COST_PER_STEP = .05; //.0005 pre-1/12/19

        private double _lastFoodNoticedThreshold = 0;

        private static int _bufferDepth = 8;

        private double[] _biomassBufferC = new double[_bufferDepth];
        private double[] _difficultyBufferC = new double[_bufferDepth];

        private double[] _biomassBufferL = new double[_bufferDepth];
        private double[] _difficultyBufferL = new double[_bufferDepth];

        private double[] _biomassBufferR = new double[_bufferDepth];
        private double[] _difficultyBufferR = new double[_bufferDepth];

        private double[] _outputDirectionBuffer = new double[_bufferDepth];
        private double[] _outputEffortBuffer = new double[_bufferDepth];
        private int _bufferIndex = 0;

        private int _historyIndex = 0;
        private static int _historyDepth = 200;
        public double[] DirectionDeltaHistory = new double[_historyDepth];
        public double[] DirectionHistory = new double[_historyDepth];
        public double[] EffortHistory = new double[_historyDepth];

        private double _totalMovement = 0;
        private ulong _childrenBirthed = 0;
        private double _energyAttained = 0;
        private double _energySpent = 0;

        private double _lastMemory = 0;

        private double _lastCreatureCount = 0;

        private double[] _lastInputs;
        private double[] _lastOutputs;

        public bool Dead { get; set; }
        public bool Busy { get; set; }
        public Tile CurrentTile { get; set; }
        public bool InEgg { get; set; }
        public double EggTime = 0;
        //Egg hatch threshold +/- up to 10% variation at random.
        public double RequiredEggTime { get; set; }

        public int BreedingEvents = 0;

        private Line visionLineC;
        private Line visionLineL;
        private Line visionLineR;
        private Line visionLineC2;
        private Line visionLineL2;
        private Line visionLineR2;

        private ulong _updates = 0;
        public ulong Updates
        {
            get
            {
                return _updates;
            }
        }
        public ulong Generation;

        public string Label
        {
            get
            {
                var pctMaxLifespan = (double)Updates / MaximumLifespan;
                var age = (Updates / Globals.YearLength).Round();
                var label = "Creature: " + Species.ToString() + "." + Generation.ToString() + "." + Size.Value.Round().ToString() + "." + ID.ToString() +
                            "\nSpecies: " + Species +
                            "\nAge: " + (Updates / Globals.YearLength).Round() + "/" + (MaximumLifespan / Globals.YearLength).Round() + " (" + (pctMaxLifespan * 100).Round() + "%)" +
                            "\nEnergy: " + Energy.Round(places: 0) + "/" + MaximumEnergy.Round(places: 0) + " (" + (Energy / MaximumEnergy * 100).Round() + "%)" +
                            "\nVisited Tiles: " + _visitedTileIDs.Count +
                            "\nFNT: " + (_lastFoodNoticedThreshold.Round() * 100) + "%" +
                            "\nSize: " + Size.Value.Round() +
                            "\nMetabolic Rate: " + MetabolicRate +
                            "\nBusy: " + Busy.ToString() +
                            "\nMomentum: " + Momentum.Round() +
                            "\nDirection: " + ((double)Direction).Round();// +
                                                                          //"\nBiomass Direction Buffer: " + _biomassDirectionBuffer.Select(d => d.Round()).Join(", ") + " -> " + _biomassDirectionBuffer.Average().Round() +
                                                                          //"\nBiomass Difference Buffer: " + _biomassDifferenceBuffer.Select(d => d.Round()).Join(", ") + " -> " + _biomassDifferenceBuffer.Average().Round() +
                                                                          //"\nOutput Direction Buffer: " + _outputDirectionBuffer.Select(d => d.Round()).Join(", ") + " -> " + _outputDirectionBuffer.Average().Round() +
                                                                          //"\nOutput Effort Buffer: " + _outputEffortBuffer.Select(d => d.Round()).Join(", ") + " -> " + _outputEffortBuffer.Average().Round() +
                if (Globals.ShowNeuralNetworkData)
                    label += "\n" + NeuralNetworkString;
                return label;
            }
        }

        public string NeuralNetworkString
        {
            get
            {
                if (Network == null)
                    return "NULL";
                string header = "Neural Network (" + Network.Neurons.Count + " Neurons, " + Network.Synapses.Array.Where(l => l != null).Sum(l => l.Count) + " Synapses)\n";
                List<string> inputLines = new List<string>();
                List<string> outputLines = new List<string>();
                for (int y = 0; y < Network.Height; y++)
                {
                    if (y < Network.InputCount)
                        inputLines.Add(((Inputs)y).GetName(true) + ": " + _lastInputs[y].Round());
                    if (y < Network.OutputCount)
                        outputLines.Add(_lastOutputs[y].Round() + " -> " + ((Outputs)y).GetName(true));
                }
                //int i = 0;
                return header + "Inputs:\n" + inputLines.Join("\n") + "\nOutputs:\n" + outputLines.Join("\n"); //Network.GetOutputPathString(i) + "=" + _lastOutputs[i].Round() + " (" + ((Outputs)i).GetName(true) + ")"; //lines.Join("\n");
            }
        }

        public int VisitedTiles
        {
            get
            {
                return _visitedTileIDs.Count;
            }
        }

        public Creature()
        {
            SelectionEnabled = true;
            Size = new MutableDouble(Globals.CreatureSize);
            Size.MutabilityMutationMultiplier = Globals.CreatureSizeMutabilityChangeMultiplier;
            MetabolicRate = Size.Value / Globals.MetabolicRateToSizeRatio;
            MetabolicRate -= MetabolicRate * Globals.MetabolicReliefPercentage;
            Direction = Maths.CirclePosition((float)Size.Value, (float)0d.Wrap(Random.NextDouble() * 360, 0, 360));
            MaturityThreshold = (ulong)(Globals.MaturityThresholdToSizeRatioThreshold * Size.Value).Round(places: 0);
            //lifespan from size (where x is size) = ((x / 16) ^ (16 / x) * 30000) + (80 * x)
            MaximumLifespan = (ulong)((Math.Pow(Size.Value / 16, 16 / Size.Value) * 30000) + (80 * Size.Value)).Round(places: 0);
            InEgg = false;
            Species = ID.ToString();
            Generation = 0;
            MaximumEnergy = Globals.EnergyToSizeRatio * Size.Value;
            Energy = Globals.StartingEnergyPercentage * MaximumEnergy;
            Visual = new Circle((float)(Size.Value / 2)) { Color = Col3.White };
            Network = new Network(Globals.NeuralNetworkWidth, Globals.NeuralNetworkHeight, (int)Inputs.Count, (int)Outputs.Count, .6, Math.Tanh);
            _lastInputs = new double[Network.InputCount];
            _lastOutputs = new double[Network.OutputCount];
            MutationChance = new MutableDouble(Globals.BaseMutationChance, Globals.BaseMutationChance);
            visionLineC2 = new Line(Position, Position) { Color = new Col3(128, 128, 128) };
            visionLineL2 = new Line(Position, Position) { Color = new Col3(0, 0, 128) };
            visionLineR2 = new Line(Position, Position) { Color = new Col3(128, 0, 0) };
            Visual.Children.Add(visionLineC2);
            Visual.Children.Add(visionLineL2);
            Visual.Children.Add(visionLineR2);
            visionLineC = new Line(Position, Position) { Color = Col3.White };
            visionLineL = new Line(Position, Position) { Color = Col3.Blue };
            visionLineR = new Line(Position, Position) { Color = Col3.Red };
            Visual.Children.Add(visionLineC);
            Visual.Children.Add(visionLineL);
            Visual.Children.Add(visionLineR);
            Visual.DrawChildren = true;
        }

        public Creature(Creature parent, bool egg)
        {
            SelectionEnabled = true;
            Parent = parent;
            Size = parent.Size.GetMutatedCopy();
            MetabolicRate = Size.Value / Globals.MetabolicRateToSizeRatio;
            MetabolicRate -= MetabolicRate * Globals.MetabolicReliefPercentage;
            MaturityThreshold = (ulong)(Globals.MaturityThresholdToSizeRatioThreshold * Size.Value).Round(places: 0);
            //lifespan from size (where x is size) = ((x / 16) ^ (16 / x) * 30000) + (80 * x)
            MaximumLifespan = (ulong)((Math.Pow(Size.Value / 16, 16 / Size.Value) * 30000) + (80 * Size.Value)).Round(places: 0);
            InEgg = egg;
            if (egg)
            {
                var eggHatchThreshold = Globals.EggHatchThresholdToSizeRatio * Size.Value;
                RequiredEggTime = eggHatchThreshold + (Random.NextDouble() * .1 * eggHatchThreshold * (Random.Chance(.5) ? -1 : 1));
            }
            Generation = parent.Generation + 1;
            Species = parent.Species;
            MaximumEnergy = Globals.EnergyToSizeRatio * Size.Value;
            Energy = Globals.StartingEnergyPercentage * MaximumEnergy;
            Visual = new Circle((float)(Size.Value / 2)) { Color = Col3.White };
            Network = parent.Network.Clone();
            MutationChance = parent.MutationChance.GetMutatedCopy();
            Network.Mutate(parent.MutationChance.Value);
            _lastInputs = new double[Network.InputCount];
            _lastOutputs = new double[Network.OutputCount];
            float spawnAngle = (float)0d.Wrap(Random.NextDouble() * 360, 0, 360);
            Direction = Maths.CirclePosition((float)parent.Size.Value, spawnAngle);
            Position = Globals.WorldWrapIfNecessary(parent.Position, Maths.CirclePointAtAngle((float)parent.Size.Value / 2, spawnAngle));
            visionLineC2 = new Line(Position, Position) { Color = new Col3(128, 128, 128) };
            visionLineL2 = new Line(Position, Position) { Color = new Col3(0, 0, 128) };
            visionLineR2 = new Line(Position, Position) { Color = new Col3(128, 0, 0) };
            Visual.Children.Add(visionLineC2);
            Visual.Children.Add(visionLineL2);
            Visual.Children.Add(visionLineR2);
            visionLineC = new Line(Position, Position) { Color = Col3.White };
            visionLineL = new Line(Position, Position) { Color = Col3.Blue };
            visionLineR = new Line(Position, Position) { Color = Col3.Red };
            Visual.Children.Add(visionLineC);
            Visual.Children.Add(visionLineL);
            Visual.Children.Add(visionLineR);
            Visual.DrawChildren = true;
        }

        public void MaybeEat(ArableTile at)
        {
            if (GetOutput(Outputs.Eat) > 0)
                lock (at)
                {
                    if (at.Biomass > 0 && (at.Biomass / at.MaximumBiomass) >= _lastFoodNoticedThreshold && Energy < MaximumEnergy)
                    {
                        Busy = true;
                        LastAte = Updates;
                        var biomassTransferred = Globals.BaseConsumptionAmount * (Size.Value / 16) * ((at.Biomass / at.MaximumBiomass) * at.Biomass) - (Globals.SharedTileConsumptionPenalty * Globals.GetNearbyCreatureCount(this));
                        if (biomassTransferred > at.Biomass)
                            biomassTransferred = at.Biomass;
                        SpendEnergy(Globals.EatingEnergyCost);
                        ReceiveEnergy(biomassTransferred);
                        at.Biomass -= biomassTransferred;
                    }
                }
        }

        public void ReceiveEnergy(double amount)
        {
            if (Dead)
                return;
            Energy += amount;
            if (Energy > MaximumEnergy || double.IsPositiveInfinity(Energy))
            {
                Energy = MaximumEnergy;
                amount = MaximumEnergy - Energy;
            }
            _energyAttained += amount;
            if (Globals.HardshipSystemEnabled)
                Hardship -= ((ulong)amount.Round(places: 0));
        }

        public void SpendEnergy(double amount)
        {
            if (Dead)
                return;
            Energy -= amount;
            if (Energy <= 0 || double.IsNegativeInfinity(Energy))
                Die(CurrentTile as ArableTile);
            _energySpent += amount;
            if (Globals.HardshipSystemEnabled)
                Hardship += ((ulong)amount.Round(places: 0));
        }

        List<ulong> _visitedTileIDs = new List<ulong>();

        public override void Update()
        {
            if (Updating)
                throw new Exception("Attempted to update creature #" + ID + " while it was already updating.");
            Updating = true;
            //try
            //{
            if (InEgg)
            {
                ////CurrentTile = Globals.FindTileForCreature(this);
                ////var at = CurrentTile as ArableTile;
                ////if (at != null)
                ////{
                ////    if (at.Temperature.Value * Globals.SeasonalTemperatureMultiplier < 32) //If it's freezing..
                ////        Die(null); //Egg dies - does not add its biomass to the tile (because it's toxic - I said so).
                ////}
                ////else //Egg is in water or on a mountain..
                ////    Die(null); //Egg dies.
                if (!double.IsNaN(Globals.SeasonalTemperatureMultiplier))
                    EggTime += Globals.SeasonalTemperatureMultiplier;
                if (EggTime >= RequiredEggTime) //If we've been in the egg long enough..
                    InEgg = false; //Hatch!
                else //Stay in egg.
                {
                    //Color egg more and more purple until it hatches by reducing green from a starting point of white.
                    ((Circle)Visual).Color = new Col3((float)(255 - EggTime / RequiredEggTime * 255 / 2), (float)(180 - EggTime / RequiredEggTime * 180), (float)(255 - EggTime / RequiredEggTime * 255 / 2));
                }
                Updating = false;
                return; //Don't perform creature activity.
            }

            if (!Dead)
            {
                #region Tile

                CurrentTile = Globals.FindTileForCreature(this);

                //Track visited tiles.
                if (!_visitedTileIDs.Contains(CurrentTile.ID))
                    _visitedTileIDs.Add(CurrentTile.ID);

                _updates++;

                SpendEnergy(MetabolicRate + (MetabolicRate * (1 - Globals.SeasonalTemperatureMultiplier)));

                Busy = !(LastAte - Updates > Globals.EatingCooldown);

                //var pctMaxLifespan = (double)Updates / MaximumLifespan;

                ArableTile at = CurrentTile as ArableTile;

                if (Updates >= MaximumLifespan)
                    Die(at);

                DeathTile dt = CurrentTile as DeathTile;

                if (dt != null)
                    Die(null);

                if (at != null)
                {
                    SetInput(Inputs.BiomassAtCurrentTile, at.BiomassRatio);
                    if (!Busy)
                        MaybeEat(at);
                }

                #endregion

                #region Neural Network

                //SetInput(Inputs.FamineSense, 1 - Globals.SeasonalTemperatureMultiplier);
                //SetInput(Inputs.LifespanPercentage, pctMaxLifespan);
                SetInput(Inputs.Sine, Math.Sin(Updates / (Globals.YearLength / 4)));
                SetInput(Inputs.NearbyCreatures, (double)Updates % Globals.TileSize == 0 ? (_lastCreatureCount = Globals.GetNearbyCreatureCount(this)) : _lastCreatureCount);
                //SetInput(Inputs.NearbyEggs, Globals.GetNearbyEggCount(this));
                //SetInput(Inputs.Rotation, _lastAngularMomentum);
                //SetInput(Inputs.Movement, _lastMovement);
                SetInput(Inputs.Energy, Energy / MaximumEnergy);
                SetInput(Inputs.Memory, _lastMemory);
                //SetInput(Inputs.MemoryB, _lastMemoryB);
                //SetInput(Inputs.MemoryC, _lastMemoryC);
                //SetInput(Inputs.MemoryD, _lastMemoryD);
                //SetInput(Inputs.DeltaE, (Energy - _previousEnergy) / MaximumEnergy);
                Network.Update(Updates);
                _lastMemory = GetOutput(Outputs.Memory);
                //_lastMemoryB = GetOutput(Outputs.MemoryB);
                //_lastMemoryC = GetOutput(Outputs.MemoryC);
                //_lastMemoryD = GetOutput(Outputs.MemoryD);
                _previousEnergy = Energy;
                _lastFoodNoticedThreshold = Math.Abs(GetOutput(Outputs.FoodNoticed) / 10);
                if (double.IsPositiveInfinity(_lastFoodNoticedThreshold))
                    _lastFoodNoticedThreshold = 1;

                #endregion

                ((Circle)Visual).Color = new Col3((float)Maths.Clamp(Energy / MaximumEnergy * 255 + 50, 0, 255), (float)Maths.Clamp(Energy / (MaximumEnergy * Globals.BreedingEnergyCostMultiplier) * 255 - 55, 0, 255), 0);//(float)(GetOutput(Outputs.Breed) != 0 && Updates > MaturityThreshold ? 40 : 0));

                #region Movement & Vision

                var inverseAdjustedFriction = (1 - Globals.Friction) * (Size.Value / 16);

                //AngularMomentum += GetOutput(Outputs.Direction);
                var lastDirection = Direction;
                _outputDirectionBuffer[_bufferIndex] = GetOutput(Outputs.Direction) * 360;
                var valid = _outputDirectionBuffer.Except(n => double.IsNaN(n) || double.IsInfinity(n));
                var desiredDirection = valid.Count() > 0 ? valid.Average() : 0; //Default to zero if no values.
                                                                                                                  //var desiredDirection = _outputDirectionBuffer.Except(n => !double.IsNaN(n)).Average();
                double directionDelta = 0;
                if (Math.Abs(desiredDirection - Direction) <= Math.Abs(Direction - desiredDirection))
                    directionDelta = Math.Sign(desiredDirection - Direction);// * .04;
                else
                    directionDelta = Math.Sign(Direction - desiredDirection);// * .04;
                Direction = Direction.Wrap((float)directionDelta, 0, 360);
                //DesiredDirectionHistory[_historyIndex] = desiredDirection;
                DirectionHistory[_historyIndex] = Direction;
                DirectionDeltaHistory[_historyIndex] = -directionDelta;
                SpendEnergy(ROTATION_COST_PER_STEP * Math.Abs(lastDirection - Direction) * (Busy ? Globals.BusyMovementCostMultiplier : 1));

                var effort = Math.Max(0, GetOutput(Outputs.Effort));
                _outputEffortBuffer[_bufferIndex] = effort;
                Momentum += (float)_outputEffortBuffer.Average() / (CurrentTile != null ? CurrentTile.MovementDivisor : 1);
                var delta = Maths.CirclePointAtAngle((float)Momentum, Direction);
                var movement = Math.Abs(delta.X) + Math.Abs(delta.Y);
                EffortHistory[_historyIndex] = movement;
                _totalMovement += (ulong)(((double)movement).Round(places: 0));
                Visual.Position = Position = Globals.WorldWrapIfNecessary(Position, delta.X, delta.Y);
                SpendEnergy(Momentum * (CurrentTile != null ? CurrentTile.MovementCostMultiplier : 1) * (Busy ? Globals.BusyMovementCostMultiplier : 1));
                Momentum -= Momentum * inverseAdjustedFriction;

                _historyIndex = _historyIndex.Wrap(1, 0, _historyDepth - 1);

                Vec2 visionTarget1 = Position + Maths.CirclePointAtAngle(1, Direction);
                var vTile1 = FindTileAtTarget(visionTarget1);
                var biomassInVision1 = vTile1 != null ? (vTile1 is ArableTile ? ((ArableTile)vTile1).BiomassRatio : 0) : 0;
                var difficultyInVision1 = vTile1 != null ? vTile1.MovementCostMultiplier : 1;

                Vec2 visionTarget = Position + Maths.CirclePointAtAngle((float)Globals.CentralVisionDistance, Direction);
                var vTileC = FindTileAtTarget(visionTarget);
                var biomassInVisionC = vTileC != null ? (vTileC is ArableTile ? ((ArableTile)vTileC).BiomassRatio : 0) : 0;
                var difficultyInVisionC = vTileC != null ? vTileC.MovementCostMultiplier : 1;
                Vec2 visionTargetL = Position + Maths.CirclePointAtAngle((float)Globals.PeripheralVisionDistance, Direction - Globals.PeripheralVisionAngle);
                var vTileL = FindTileAtTarget(visionTargetL);
                var biomassInVisionL = vTileL != null ? (vTileL is ArableTile ? ((ArableTile)vTileL).BiomassRatio : 0) : 0;
                var difficultyInVisionL = vTileL != null ? vTileL.MovementCostMultiplier : 1;
                Vec2 visionTargetR = Position + Maths.CirclePointAtAngle((float)Globals.PeripheralVisionDistance, Direction + Globals.PeripheralVisionAngle);
                var vTileR = FindTileAtTarget(visionTargetR);
                var biomassInVisionR = vTileR != null ? (vTileR is ArableTile ? ((ArableTile)vTileR).BiomassRatio : 0) : 0;
                var difficultyInVisionR = vTileR != null ? vTileR.MovementCostMultiplier : 1;

                Vec2 visionTarget2 = Position + Maths.CirclePointAtAngle((float)Globals.CentralFarVisionDistance, Direction);
                var vTileC2 = FindTileAtTarget(visionTarget2);
                var biomassInVisionC2 = vTileC2 != null ? (vTileC2 is ArableTile ? ((ArableTile)vTileC2).BiomassRatio : 0) : 0;
                var difficultyInVisionC2 = vTileC2 != null ? vTileC2.MovementCostMultiplier : 1;
                Vec2 visionTargetL2 = Position + Maths.CirclePointAtAngle((float)Globals.PeripheralFarVisionDistance, Direction - Globals.PeripheralFarVisionAngle);
                var vTileL2 = FindTileAtTarget(visionTargetL2);
                var biomassInVisionL2 = vTileL2 != null ? (vTileL2 is ArableTile ? ((ArableTile)vTileL2).BiomassRatio : 0) : 0;
                var difficultyInVisionL2 = vTileL2 != null ? vTileL2.MovementCostMultiplier : 1;
                Vec2 visionTargetR2 = Position + Maths.CirclePointAtAngle((float)Globals.PeripheralFarVisionDistance, Direction + Globals.PeripheralFarVisionAngle);
                var vTileR2 = FindTileAtTarget(visionTargetR2);
                var biomassInVisionR2 = vTileR2 != null ? (vTileR2 is ArableTile ? ((ArableTile)vTileR2).BiomassRatio : 0) : 0;
                var difficultyInVisionR2 = vTileR2 != null ? vTileR2.MovementCostMultiplier : 1;

                var biomass1 = biomassInVision1;
                var biomassC = biomassInVisionC * .75 + biomassInVisionC2 * .25;
                var biomassL = biomassInVisionL * .75 + biomassInVisionL2 * .25;
                var biomassR = biomassInVisionR * .75 + biomassInVisionR2 * .25;

                var difficulty1 = difficultyInVision1;
                var difficultyC = difficultyInVisionC * .75 + difficultyInVisionC2 * .25;
                var difficultyL = difficultyInVisionL * .75 + difficultyInVisionL2 * .25;
                var difficultyR = difficultyInVisionR * .75 + difficultyInVisionR2 * .25;

                _biomassBufferC[_bufferIndex] = biomassC;
                _biomassBufferL[_bufferIndex] = biomassL;
                _biomassBufferR[_bufferIndex] = biomassR;

                _difficultyBufferC[_bufferIndex] = difficultyC;
                _difficultyBufferL[_bufferIndex] = difficultyL;
                _difficultyBufferR[_bufferIndex] = difficultyR;

                SetInput(Inputs.BiomassInVision1, biomassInVision1);

                var biomassAverageC = _biomassBufferC.Average();
                var biomassAverageL = _biomassBufferL.Average();
                var biomassAverageR = _biomassBufferR.Average();

                SetInput(Inputs.BiomassInVisionC, biomassAverageC);
                SetInput(Inputs.BiomassInVisionL, biomassAverageL);
                SetInput(Inputs.BiomassInVisionR, biomassAverageR);

                SetInput(Inputs.DifficultyInVision1, difficultyInVision1);

                var difficultyAverageC = _difficultyBufferC.Average();
                var difficultyAverageL = _difficultyBufferL.Average();
                var difficultyAverageR = _difficultyBufferR.Average();

                SetInput(Inputs.DifficultyInVisionC, difficultyAverageC);
                SetInput(Inputs.DifficultyInVisionL, difficultyAverageL);
                SetInput(Inputs.DifficultyInVisionR, difficultyAverageR);

                _bufferIndex = _bufferIndex.Wrap(1, 0, _bufferDepth - 1);

                if (Globals.HardshipSystemEnabled)
                    Hardship += ((ulong)
                                (1 + (1 - (1 / CurrentTile.MovementDivisor)) +
                                (at != null ? 1 - at.BiomassRatio : 0) +
                                (1 - biomassInVision1) +
                                difficultyInVision1 +
                                //(1 - (((biomassAverageC * .50) + (biomassAverageL * .25) + (biomassAverageR * .25)) / 3)) +
                                //(((difficultyAverageC * .50)) + (difficultyAverageL * .25) + (difficultyAverageR * .25) / 3) +
                                0).Round(places: 0));

                #endregion

                #region Birth

                if (Updates > MaturityThreshold && GetOutput(Outputs.Breed) > 0) //Can only give birth if mature & willing.
                    if ((Energy > (MaximumEnergy * (Globals.BreedingEnergyCostMultiplier + .1))))// && (GetOutput(Outputs.Breed) + Globals.BreedingBias) > 0)
                    {
                        BreedingEvents++;
                        Busy = true;
                        SpendEnergy(MaximumEnergy * Globals.BreedingEnergyCostMultiplier);
                        for (byte i = 0; i < Globals.EggsPerSpawning; i++)
                        {
                            var c = new Creature(this, Globals.UseEggs);
                            Globals.NewlyBornCreatures.Add(c);
                        }
                        _childrenBirthed += Globals.EggsPerSpawning;
                    }

                #endregion

                visionLineC.A = Position;
                visionLineC.B = visionTarget;

                visionLineL.A = Position;
                visionLineL.B = visionTargetL;

                visionLineR.A = Position;
                visionLineR.B = visionTargetR;

                visionLineC2.A = Position;
                visionLineC2.B = visionTarget2;

                visionLineL2.A = Position;
                visionLineL2.B = visionTargetL2;

                visionLineR2.A = Position;
                visionLineR2.B = visionTargetR2;
            }
            Updating = false;
            //}
            //catch (Exception ex) 
            //{
            //    System.IO.File.AppendAllText(ID + ".log", "\nException encountered during update of this creature:\n" + ex.ToString());
            //}
            BoundingRect = new BoundingRect(Position, (float)Size.Value);
        }

        public void Die(ArableTile deathTile)
        {
            if (Dead)
                return; //Can't die twice.
            Dead = true;
            if (deathTile != null)
                lock (deathTile)
                    deathTile.Biomass += Energy * .33; //33% of creature's energy goes to tile, the rest is lost in some way (rising heat, gases, inedible material, etc.).
            Globals.NewlyDeadCreatures.Add(this);
        }

        public void SetInput(Inputs input, double value)
        {
            int inputIndex = (int)input;
            _lastInputs[inputIndex] = value;
            if (Network != null)
                Network.SetInput(inputIndex, value);
            else
                throw new Exception("The neural network has not been assigned yet.");
        }

        public double GetOutput(Outputs output)
        {
            int outputIndex = (int)output;
            if (Network != null)
                return (_lastOutputs[outputIndex] = Network.GetOutput(outputIndex));
            else
                throw new Exception("The neural network has not been assigned yet.");
        }

        //private double _lastProgenyScore = 0;
        //public double GetProgenyScore()
        //{
        //    if (Dead) //If it's dead..
        //        return _lastProgenyScore; //Return the last score when it was alive.
        //    else if (Globals.ProgenyTrackingEnabled)
        //    {
        //        if (BreedingEvents == 0) //If the creature didn't breed.
        //            return 0; //It gets no score.
        //        double total = 0;
        //        foreach (Creature c in Children)
        //        {
        //            if (!c.Dead || c.BreedingEvents > 0) //Child is alive or has bred.
        //                total++; //1 Point
        //            else //Child is dead or did not breed.
        //                total += .25; //1/4 Point
        //            total += c.GetProgenyScore(); //Add in recursive points.
        //        }
        //        return _lastProgenyScore = (total / (BreedingEvents * Globals.EggsPerSpawning)); //Divide points by number of eggs spawned, store in _lastProgenyScore for returning it if the creature dies.
        //    }
        //    else
        //        return BreedingEvents; //Score is equal to number of breeding events.
        //}

        public Tile FindTileAtTarget(Vec2 at)
        {
            Vec2 delta = at - Position;
            Vec2 eyeTarget = Globals.WorldWrapIfNecessary(Position, delta);
            return Globals.FindTileForPosition(eyeTarget);
        }

        private double _fitness = 0;
        public double Fitness
        {
            get
            {
                if (!Dead)
                    _recalculateFitness();
                return _fitness;
            }
        }

        private void _recalculateFitness()
        {
            if (_totalMovement < Globals.TileSize * 2)
                _fitness = 0;
            else
            {
                var lifespanLived = (double)_updates / (double)MaximumLifespan;
                //Globals.BaseTemperatureVariation + Math.Sin(updates / Globals.YearLength * Globals.SeasonFrequency) * Globals.SeasonAmplitudeMultiplier;
                //var seasonsLived = _updates / Globals.YearLength * Globals.SeasonToYearRatio;
                //var progenyFitness = (double)_childrenBirthed;
                var energyFitness = Math.Max(0, (_energyAttained - _energySpent)) / MaximumEnergy;
                _fitness = _visitedTileIDs.Count < 5 ? 0 : (_visitedTileIDs.Count / 1000d) + (energyFitness * 2) + (BreedingEvents * 100d);// + (2 * (_visitedTileIDs.Count / 3));
            }
        }

        public override string ToString()
        {
            return Species + "(" + Generation + ", " + BreedingEvents + "): " + Fitness.Round();
        }
    }
}