using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Charybdis.Library.Core;
using Charybdis.Science;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using Charybdis.MonoGame;
using Charybdis.MonoGame.Framework;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace EvolutionSimulator
{
    public class EvolutionSimulator : Game
    {
        DateTime simulationStart = DateTime.Now;
        DateTime lastInjection = DateTime.Now;
        MouseState previousMouseState;
        MouseState activeMouseState;
        Random random;
        GraphicsDeviceManager gdm;
        RasterizerState rasterizerState;
        DepthStencilState depthStencilState;
        SpriteBatch spriteBatch;
        double seed;
        Font font1;
        Font font2;
        int frameRate = 0;
        int frameCounter = 0;
        int simulationRate = 0;
        int simulationCounter = 0;
        int simulationRate5 = 0;
        int simulationCounter5 = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        TimeSpan elapsedTime5 = TimeSpan.Zero;
        TimeSpan sec1 = TimeSpan.FromSeconds(1);
        TimeSpan sec5 = TimeSpan.FromSeconds(5);
        Vec3 movement = Vec3.Zero;
        Vec2 normal = Vec2.Zero;
        List<Sprite> sprites = new List<Sprite>();
        Vec2 velocity = Vec2.Zero;
        Sprite cursor;
        Border selectionBorder = new Border { Color = Col3.White };
        Col4 uiPanelColor = new Col4(80, 80, 80);
        Col4 uiBorderColor = Col4.White;
        TextWindow infoWindow;
        bool paused = false;
        int deadCreatureCount = 0;
        int eggCountAllTime = 0;
        ulong updates = 0;
        List<object> epochTracker = new List<object>();
        bool render = true;
        bool simulate = true;
        Task simulationTask;
        ulong injectionCount = 0;
        ushort injectionRate = 0;
        ushort injectionCounter = 0;
        ulong spawnedCreatureCount = 0;
        Label mostFitLabel;
        Label longestLineageLabel;
        Label oldestLabel;
        Label smallestLabel;
        Label largestLabel;
        Vec2 graphOffset = new Vec2(250, 0);
        Vec2 labelOffset = new Vec2(-500, 0);
        double lastSeasonalTemperatureMultiplier = Globals.SeasonalTemperatureMultiplier;

        public EvolutionSimulator()
        {
            gdm = new GraphicsDeviceManager(this);
            random = new Random();
            Window.Title = Console.Title = "Charybdis Kernel (2D)";
            Window.IsBorderless = true;
            seed = random.NextDouble();
            Content.RootDirectory = "Data";
            gdm.PreferredBackBufferWidth = Globals.Width;
            gdm.PreferredBackBufferHeight = Globals.Height;
            IsMouseVisible = false;
            IsFixedTimeStep = Globals.DecoupleSimulationFromVisuals;
        }

        protected override void Initialize()
        {
            Window.Position = Point.Zero;
            Globals.GraphicsDevice = GraphicsDevice;
            GraphicsDevice.Viewport = new Viewport(0,0 , Globals.Width, Globals.Height); //This is needed for release builds to show the borderless window properly (otherwise it renders the top left chunk across the screen and the rest is out of bounds).
            spriteBatch = new SpriteBatch(GraphicsDevice);
            rasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace, FillMode = FillMode.Solid };
            depthStencilState = new DepthStencilState { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less };
            XNA.Initialize2D(GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Set up title holder spots programmatically.
            var titles = Enum.GetValues(typeof(Title)).Cast<Title>();
            foreach (var title in titles)
                Globals.TitleHolders.TryAdd(title, null);

            #region Prep UI Bits

            Globals.Font = font1 = GraphicsDevice.FontFromFile(Content.RootDirectory + "/Fonts/Consolas-12.fnt");
            font1.Scale = 1f;
            font2 = GraphicsDevice.FontFromFile(Content.RootDirectory + "/Fonts/Consolas-12.fnt");
            font2.Scale = .5f;
            var cursorTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Cursor21.png");
            cursor = new Sprite(cursorTexture);
            infoWindow = new TextWindow("", font1, uiBorderColor, uiPanelColor, Col4.White) { Parent = cursor, Position = cursor.Size * new Vec2(1.1f) };
            infoWindow.DrawMe = false; //Temporary, trying to figure out why it won't stop drawing when a check for .Text being null is put in place. -UdderlyEvelyn 1/25/19
            cursor.Children.Add(infoWindow);

            if (Globals.FitnessBasedEvolution)
                mostFitLabel = new Label { Font = Globals.Font, Shadowed = true };
            longestLineageLabel = new Label { Font = Globals.Font, Shadowed = true };
            oldestLabel = new Label { Font = Globals.Font, Shadowed = true };
            smallestLabel = new Label { Font = Globals.Font, Shadowed = true };
            largestLabel = new Label { Font = Globals.Font, Shadowed = true };

            #endregion

            #region World Generation

            Globals.Heightmap = new Array2<byte>(Globals.WorldWidth, Globals.WorldHeight);
            Globals.Heightmap.Perlin(Globals.HeightmapPerlinOctaves, Globals.HeightmapMaximumHeight);

            Globals.World = new Array2<Tile>(Globals.WorldWidth, Globals.WorldHeight);
            for (int y = 0; y < Globals.WorldHeight; y++)
                for (int x = 0; x < Globals.WorldWidth; x++)
                {
                    Tile t;
                    var height = Globals.Heightmap.Get(x, y);
                    //if (Globals.Random.Chance(.004)) //1 in 250 chance for a death tile.
                    //    t = new DeathTile(Globals.TileSize, Globals.GridColor, height);
                    //else
                    //{
                        if (height >= Globals.SteepHeightThreshold)
                        {
                            var st = new SteepTile(Globals.TileSize, Globals.GridColor, height)
                            {
                                Position = new Vec2(x * Globals.TileSize, y * Globals.TileSize),
                            };
                            Globals.AllGameObjects.Add(st);
                            t = st;
                        }
                        else if (height <= Globals.Sealevel)
                        {
                            var wt = new WaterTile(Globals.TileSize, Globals.GridColor, height)
                            {
                                Position = new Vec2(x * Globals.TileSize, y * Globals.TileSize),
                            };
                            Globals.AllGameObjects.Add(wt);
                            t = wt;
                        }
                        else
                        {
                            var at = new ArableTile(Globals.TileSize, Globals.GridColor, height)
                            {
                                Position = new Vec2(x * Globals.TileSize, y * Globals.TileSize),
                            };
                            at.Biomass = Maths.Clamp(at.MaximumBiomass * Globals.Random.NextDouble() * Globals.StartingBiomassPercentage, 0, at.MaximumBiomass);
                            t = at;
                            Globals.AllGameObjects.Add(at);
                        }
                    //}
                    Globals.World.Set(x, y, t);
                    Globals.Tiles.Add(t);
                    Globals.Visuals.Add(t.Visual);
                }

            #endregion

            #region Spawn Initial Population

            for (int i = 0; i < Globals.StartingCreatureCount; i++)
            {
                //Log("Spawning " + i + "/" + Globals.CreatureInjectionCount);
                var newCreaturePosition = new Vec2(random.Next(1, Globals.WorldPixelWidth - 1), random.Next(1, Globals.WorldPixelHeight - 1));
                while (!(Globals.FindTileForPosition(newCreaturePosition) is ArableTile)) //Make sure it gets a fair start.
                    newCreaturePosition = new Vec2(random.Next(1, Globals.WorldPixelWidth - 1), random.Next(1, Globals.WorldPixelHeight - 1));
                Globals.NewlyBornCreatures.Add(new Creature() { Position = newCreaturePosition });
                spawnedCreatureCount++;
            }
            lastInjection = DateTime.Now;

            #endregion

            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
            if (Globals.DecoupleSimulationFromVisuals)
                simulationTask = Tasking.Do(SimulationLoop);
            base.LoadContent();
        }

        protected override bool BeginDraw()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            return base.BeginDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            frameCounter++;

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.DepthStencilState = depthStencilState;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            if (render)
            {
                Globals.VisualsLock.EnterReadLock();

                foreach (var d2 in Globals.Visuals)
                    d2.Draw(spriteBatch, Vec2.Zero);

                Globals.VisualsLock.ExitReadLock();
            }
            else
                spriteBatch.DrawShadowedString(font1, "RENDERING DISABLED FOR PERFORMANCE", new Vec2(Globals.Width / 2, Globals.Height / 2) - (font1.CalculateTextSize("RENDERING DISABLED FOR PERFORMANCE") / 2));

            var mostFit = Globals.TitleHolders[Title.MostFit];
            var longestLineage = Globals.TitleHolders[Title.LongestLineage];
            var oldest = Globals.TitleHolders[Title.Oldest];
            var smallest = Globals.TitleHolders[Title.Smallest];
            var largest = Globals.TitleHolders[Title.Largest];
            
            if (render && Globals.ShowTitleHolderLabels)
            {
                //Most Fit Title Label
                if (Globals.FitnessBasedEvolution && mostFit != null)
                {
                    mostFitLabel.Text = "Most Fit\n" + mostFit.Label;
                    _drawCreatureLabel(mostFit, mostFitLabel);
                }
                //Longest Lineage Title Label
                if (longestLineage != null)
                {
                    longestLineageLabel.Text = "Longest Lineage\n" + longestLineage.Label;
                    _drawCreatureLabel(longestLineage, longestLineageLabel);
                }
                //Oldest Title Label
                if (oldest != null)
                {
                    oldestLabel.Text = "Oldest\n" + oldest.Label;
                    _drawCreatureLabel(oldest, oldestLabel);
                }
                //Smallest Title Label
                if (smallest != null)
                {
                    smallestLabel.Text = "Smallest\n" + smallest.Label;
                    _drawCreatureLabel(smallest, smallestLabel);
                }
                //Largest Title Label
                if (largest != null)
                {
                    largestLabel.Text = "Largest\n" + largest.Label;
                    _drawCreatureLabel(largest, largestLabel);
                }
            }
            
            Globals.CreaturesLock.EnterReadLock();
            var creatureCount = Globals.Creatures.Count(c => !c.InEgg);
            var eggCount = Globals.Creatures.Count(c => c.InEgg);
            Globals.CreaturesLock.ExitReadLock();

            var temperatureMultiplierChangeSign = Math.Sign(Globals.SeasonalTemperatureMultiplier - lastSeasonalTemperatureMultiplier);

            string display =
                "Charybdis2D Kernel - " + frameRate + "FPS - " + Globals.MemoryUsed + "MB" +
                "\nSimulation Time: " + (DateTime.Now - simulationStart).TotalMinutes.Round(places: 0) + " Minutes" +
                (Globals.DecoupleSimulationFromVisuals ? ("\nSimulation Thread Status: " + simulationTask.Status.GetName(false)) : "") +
                "\nSimulation Updates/Sec: " + simulationRate + " (" + (Globals.DecoupleSimulationFromVisuals ? "Decoupled" : "Bound To Framerate") + ")" +
                "\nSimulation Updates/Sec (5 Second Average): " + ((double)simulationRate5 / 5).Round() +
                "\nParallel Update Loop: " + (Globals.ParallelUpdateLoop ? "Enabled" : "Disabled") +
                "\nParallel Neural Network Update Method: " + (Globals.ParallelNeuralNetworkUpdateMethod ? "Enabled" : "Disabled") +
                "\nFitness-Based Evolution: " + (Globals.FitnessBasedEvolution ? "Enabled" : "Disabled") +
                "\nHardship System: " + (Globals.HardshipSystemEnabled ? "Enabled" : "Disabled") +
                "\nInjections/Sec: " + injectionRate + " (Total: " + injectionCount + ")" +
                "\nSince Last Injection: " + (DateTime.Now - lastInjection).TotalMinutes.Round() + " Minutes" +
                "\nEpoch: " + epochTracker.Count +
                "\nYear: " + ((double)updates / Globals.YearLength).Round() +
                "\nSeason: " + (Globals.SeasonalTemperatureMultiplier >= Globals.BaseTemperatureVariation ? "Summer" : "Winter") + " (" + (Globals.SeasonalTemperatureMultiplier.Round() * 100) + "% " + (temperatureMultiplierChangeSign > 0 ? "And Rising" : (temperatureMultiplierChangeSign < 0 ? "And Falling" : "Stagnant")) + ")" +
                "\nWorld: " + Globals.WorldWidth + "x" + Globals.WorldHeight + " (Tiles), " + Globals.WorldPixelWidth + "x" + Globals.WorldPixelHeight + "px" +
                "\nCreatures: " + creatureCount + " (Eggs: " + (Globals.UseEggs ? (eggCount + ", All Time: " + eggCountAllTime) : "Disabled") + ")" +
                "\nSelection: " + Selector.Count +
                "\nMinimum Creatures In World: " + Globals.MinimumCreaturesInWorld +
                "\nDead: " + deadCreatureCount +
                "\nLongest Lineage: " + (longestLineage != null ? (longestLineage.Generation + " (" + longestLineage.Species + ")") : "N/A") +
                "\nOldest: " + (oldest != null ? (((double)oldest.Updates / Globals.YearLength).Round() + " (" + (((double)oldest.Updates / (double)oldest.MaximumLifespan).Round() * 100) + "%, Generation " + oldest.Generation + ", Species " + oldest.Species + ")") : "N/A") +
                "\nSize Range: " + ((smallest != null && largest != null) ? smallest.Size.Value.Round() + "-" + largest.Size.Value.Round() : "N/A") +
                "";
            if (Globals.FitnessBasedEvolution)
            {
                display +=
                "\n" +
                "\nFitness Board" +
                "\n-------------" +
                "\nMedian: " + Globals.MedianFitness.Round() +
                "\nLower Half Mean: " + Globals.LowerHalfMeanFitness.Round() +
                "\nCulled At/Below: " + (Globals.LowerHalfMeanFitness - (Globals.LowerHalfMeanFitness * Globals.PercentageBelowLowerHalfMeanFitnessDeletionThreshold)).Round() + " (When " + Globals.SuccessfulCreatureHistoryCullingThreshold + "+ On Board)" +
                "\n-------------" +
                "\nPLACEMENT: ID(GENERATION, BREEDING EVENTS): FITNESS SCORE" +
                "\n-------------" +
                "\nLiving: " + (mostFit != null ? mostFit.ToString() : "N/A") +
                "";
                Globals.MostSuccessfulCreaturesLock.EnterReadLock();
                for (int i = 0; i < Globals.SuccessfulCreatureHistoryDisplayLength; i++)
                    display += "\n" + (i + 1) + ": " + (Globals.MostSuccessfulCreatures.Count > i ? Globals.MostSuccessfulCreatures[i].ToString() : "N/A");
                Globals.MostSuccessfulCreaturesLock.ExitReadLock();
            }
            spriteBatch.DrawShadowedString(font1, display, Vec2.Zero);
            if (paused)
                spriteBatch.DrawShadowedString(font1, "PAUSED", new Vec2(Globals.Width / 2, Globals.Height / 2) - (font1.CalculateTextSize("PAUSED") / 2));
            spriteBatch.DrawSprite(cursor); //Draw cursor last so it's always on top.
            lastSeasonalTemperatureMultiplier = Globals.SeasonalTemperatureMultiplier;
            base.Draw(gameTime);
        }

        private void _drawCreatureLabel(Creature c, Label l)
        {
            l.Draw(spriteBatch, c.Visual.Position - (Globals.Font.CalculateTextSize(l.Text) / 2));
            if (Globals.ShowTitleHolderGraphs)
            {
                var directionGraphOrigin = c.Visual.Position + graphOffset;
                var directionDeltaGraphOrigin = c.Visual.Position + graphOffset + new Vec2(-100, 250);
                for (int i = 0; i < 199; i++)
                {
                    float colorValue = (float)(c.EffortHistory[i] * 255);

                    new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(c.DirectionHistory[i])).ToXNA(),
                             directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(c.DirectionHistory[i + 1])).ToXNA())
                    { Color = new Col3(colorValue, 0, colorValue) }
                    .Draw(spriteBatch, Vec2.Zero);

                    new Line(directionDeltaGraphOrigin + new Vec2(i, (float)(c.DirectionDeltaHistory[i])),
                             directionDeltaGraphOrigin + new Vec2(i + 1, (float)(c.DirectionDeltaHistory[i + 1])))
                    { Color = new Col3(colorValue, 0, colorValue) }
                    .Draw(spriteBatch, Vec2.Zero);
                }
                new Line(directionGraphOrigin + Vectors.Vec2.NZ * 4, directionGraphOrigin + Vectors.Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch, Vec2.Zero);
                new Line(directionGraphOrigin + Vectors.Vec2.ZN * 4, directionGraphOrigin + Vectors.Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch, Vec2.Zero);
                new Line(directionDeltaGraphOrigin + Vectors.Vec2.NZ * 4, directionDeltaGraphOrigin + Vectors.Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch, Vec2.Zero);
                new Line(directionDeltaGraphOrigin + Vectors.Vec2.ZN * 4, directionDeltaGraphOrigin + Vectors.Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch, Vec2.Zero);
            }
        }

        protected override void EndDraw()
        {
            spriteBatch.End();
            base.EndDraw();
        }

        protected override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > sec1)
            {
                elapsedTime -= sec1;
                frameRate = frameCounter;
                frameCounter = 0;
                Globals.SimulationFramesPerSecond = simulationRate = simulationCounter;
                simulationCounter = 0;
                injectionRate = injectionCounter;
                injectionCounter = 0;
                Globals.MemoryUsed = (int)Math.Round((double)GC.GetTotalMemory(false) / 1024 / 1024, 0);
            }
            elapsedTime5 += gameTime.ElapsedGameTime;
            if (elapsedTime5 > sec5)
            {
                elapsedTime5 -= sec5;
                simulationRate5 = simulationCounter5;
                simulationCounter5 = 0;
            }
            KeyboardHandler(gameTime);
            MouseHandler(gameTime);
            if (paused)
            {
                base.Update(gameTime);
                return;
            }
            if (simulate)
            {
                if (!Globals.DecoupleSimulationFromVisuals) //If we're performing this on the same thread..
                    PerformSimulationFrame(); //Do one simulation frame.
                else if (simulationTask != null && simulationTask.Status != TaskStatus.Running) //We're simulating on a separate thread, so make sure threads are running and restart them if not (and mention it in the console if they faulted).
                {
                    if (simulationTask.Status == TaskStatus.Faulted)
                    {
                        Console.WriteLine("Simulation thread status was " + simulationTask.Status.GetName(false) + "\n\nException:\n" + simulationTask.Exception.ToString());
                        simulate = false;
                    }
                    else
                    {
                        Console.WriteLine("Simulation thread status was " + simulationTask.Status.GetName(false) + ", restarting..\n");
                        simulationTask = Tasking.Do(SimulationLoop);
                    }
                }
            }
            base.Update(gameTime);
        }

        private Comparison<Creature> _creatureFitnessComparison = new Comparison<Creature>(
            (c1, c2) =>
            {
                if (c1.Fitness > c2.Fitness) return -1;
                else if (c2.Fitness > c1.Fitness) return 1;
                else return 0;
            });

        public void SimulationLoop()
        {
            while (simulate)
                PerformSimulationFrame();
        }

        public void PerformSimulationFrame()
        {
            if (paused)
                return;
            simulationCounter5++;
            simulationCounter++;
            updates++;
            //Hand-tuned this..
            //OLD: https://www.graphsketch.com/?eqn1_color=1&eqn1_eqn=.75%20-%20(sin(x%2F160)%20%2F%202)&eqn2_color=2&eqn2_eqn=&eqn3_color=3&eqn3_eqn=&eqn4_color=4&eqn4_eqn=&eqn5_color=5&eqn5_eqn=&eqn6_color=6&eqn6_eqn=&x_min=-100&x_max=1000&y_min=-.15&y_max=1.5&x_tick=20&y_tick=.05&x_label_freq=3&y_label_freq=3&do_grid=0&do_grid=1&bold_labeled_lines=0&bold_labeled_lines=1&line_width=4&image_w=850&image_h=525
            //https://www.graphsketch.com/?eqn1_color=1&eqn1_eqn=.75%20%2B%20sin(x%20%2F%2010000%20*%20pi)%20*%20.25&eqn2_color=2&eqn2_eqn=.75&eqn3_color=3&eqn3_eqn=&eqn4_color=4&eqn4_eqn=&eqn5_color=5&eqn5_eqn=&eqn6_color=6&eqn6_eqn=&x_min=-500&x_max=10000&y_min=-.1&y_max=1&x_tick=1000&y_tick=.1&x_label_freq=1&y_label_freq=1&do_grid=0&do_grid=1&bold_labeled_lines=0&bold_labeled_lines=1&line_width=4&image_w=850&image_h=525
            Globals.SeasonalTemperatureMultiplier = Globals.BaseTemperatureVariation + Math.Sin(updates / Globals.YearLength * Globals.SeasonFrequency) * Globals.SeasonToYearRatio;
            if (updates == ulong.MaxValue) //If we've run out of ulong..
            {
                epochTracker.Add(new object()); //Add an "epoch" object.
                updates = 0; //Reset the count.
            }
            bool injectionHappened = false;
            //Spawn new creatures if necessary.
            Globals.CreaturesLock.EnterReadLock();
            int livingCreatureCount = Globals.Creatures.Where(c => !c.Dead).Count();
            Globals.CreaturesLock.ExitReadLock();
            if (livingCreatureCount + Globals.NewlyBornCreatures.Count < Globals.MinimumCreaturesInWorld)
            {
                Parallel.For(0, Globals.CreatureInjectionCount, i =>
                {
                    var newCreaturePosition = new Vec2(random.Next(1, Globals.WorldPixelWidth - 1), random.Next(1, Globals.WorldPixelHeight - 1));
                    while (!(Globals.FindTileForPosition(newCreaturePosition) is ArableTile)) //Make sure it gets a fair start.
                        newCreaturePosition = new Vec2(random.Next(1, Globals.WorldPixelWidth - 1), random.Next(1, Globals.WorldPixelHeight - 1));

                    if (Globals.MostSuccessfulCreatures.Count > i)
                        Globals.NewlyBornCreatures.Add(new Creature(Globals.MostSuccessfulCreatures[i], false) { Position = newCreaturePosition });
                    else
                        Globals.NewlyBornCreatures.Add(new Creature() { Position = newCreaturePosition });
                });
                injectionHappened = true;
                injectionCounter++;
                injectionCount++;
                lastInjection = DateTime.Now;
            }

            //Store current titleholders in a variable which will also track best fits while we iterate.
            var mostFit = Globals.TitleHolders[Title.MostFit];
            var longestLineage = Globals.TitleHolders[Title.LongestLineage];
            var oldest = Globals.TitleHolders[Title.Oldest];
            var smallest = Globals.TitleHolders[Title.Smallest];
            var largest = Globals.TitleHolders[Title.Largest];
            var explorer = Globals.TitleHolders[Title.Explorer];
           
            //Update creatures in parallel.
            Globals.AllGameObjectsLock.EnterReadLock();
            if (Globals.ParallelUpdateLoop)
            {
                var parallelResult = Parallel.ForEach(Globals.AllGameObjects, igo =>
                {
                    lock (igo)
                        igo.Update();
                    if (igo is Creature c && !c.InEgg)
                    {
                        if (Globals.FitnessBasedEvolution)
                            if ((mostFit == null || (mostFit.Dead || c.Fitness > mostFit.Fitness) && mostFit != c))
                                mostFit = c;
                        if ((longestLineage == null || (longestLineage.Dead || c.Generation > longestLineage.Generation) && longestLineage != c))
                            longestLineage = c;
                        if ((oldest == null || (oldest.Dead || c.Updates > oldest.Updates) && oldest != c))
                            oldest = c;
                        if ((smallest == null || (smallest.Dead || c.Size.Value < smallest.Size.Value) && smallest != c))
                            smallest = c;
                        if ((largest == null || (largest.Dead || c.Size.Value > largest.Size.Value) && largest != c))
                            largest = c;
                        if ((explorer == null || (explorer.Dead || c.VisitedTiles > explorer.VisitedTiles) && explorer != c))
                            explorer = c;
                    }
                    if (Globals.BiomassRefillOnInjection && injectionHappened)
                    {
                        if (igo is ArableTile at)
                            at.Biomass = at.MaximumBiomass;
                    }
                });
                while (!parallelResult.IsCompleted)
                {
                    //Wait for completion.
                }
            }
            else
            {
                //Update creatures linearly. This hasn't been used in a long time, lol!
                foreach (GameObject2 go in Globals.AllGameObjects)
                {
                    lock (go)
                        go.Update();
                    if (go is Creature c && !c.InEgg)
                    {
                        if (Globals.FitnessBasedEvolution)
                            if ((mostFit == null || (mostFit.Dead || c.Fitness > mostFit.Fitness) && mostFit != c))
                                mostFit = c;
                        if ((longestLineage == null || (longestLineage.Dead || c.Generation > longestLineage.Generation) && longestLineage != c))
                            longestLineage = c;
                        if ((oldest == null || (oldest.Dead || c.Updates > oldest.Updates) && oldest != c))
                            oldest = c;
                        if ((smallest == null || (smallest.Dead || c.Size.Value < smallest.Size.Value) && smallest != c))
                            smallest = c;
                        if ((largest == null || (largest.Dead || c.Size.Value > largest.Size.Value) && largest != c))
                            largest = c;
                        if ((explorer == null || (explorer.Dead || c.VisitedTiles > explorer.VisitedTiles) && explorer != c))
                            explorer = c;
                    }
                    if (Globals.BiomassRefillOnInjection && injectionHappened)
                        if (go is ArableTile at)
                            at.Biomass = at.MaximumBiomass;
                }
            }
            Globals.AllGameObjectsLock.ExitReadLock();

            //Update title holders.
            Globals.TitleHolders[Title.LongestLineage] = longestLineage;
            Globals.TitleHolders[Title.Oldest] = oldest;
            Globals.TitleHolders[Title.Smallest] = smallest;
            Globals.TitleHolders[Title.Largest] = largest;
            Globals.TitleHolders[Title.Explorer] = explorer;
            if (Globals.FitnessBasedEvolution)
            {
                Globals.TitleHolders[Title.MostFit] = mostFit;
                //Add most fit to the board, re-sort and remove least fit.
                if (mostFit != null && mostFit.Fitness > 0)
                {
                    Globals.MostSuccessfulCreaturesLock.EnterWriteLock();
                    bool creaturesAddedToMostFitBoard = false;
                    if (!Globals.MostSuccessfulCreatures.Contains(mostFit))
                    {
                        Globals.MostSuccessfulCreatures.Add(mostFit);
                        creaturesAddedToMostFitBoard = true;
                    }
                    var lowestBoardValue = Globals.MostSuccessfulCreatures.Min(c => c.Fitness);

                    Globals.CreaturesLock.EnterReadLock();
                    foreach (Creature c in Globals.Creatures)
                    {
                        if (c.Fitness > lowestBoardValue)
                            if (!Globals.MostSuccessfulCreatures.Contains(c) && c.Fitness > 0)
                            {
                                Globals.MostSuccessfulCreatures.Add(c);
                                creaturesAddedToMostFitBoard = true;
                            }
                    }
                    Globals.CreaturesLock.ExitReadLock();

                    if (creaturesAddedToMostFitBoard)
                    {
                        Globals.MostSuccessfulCreatures.Sort(_creatureFitnessComparison);
                        //Cull Extraneous Species Examples
                        IEnumerable<Creature> toRemove = null;
                        if (Globals.SuccessfulCreatureSpeciesGenerationExampleReduction)
                        {
                            toRemove =
                                Globals.MostSuccessfulCreatures
                                .Where(c => c.Dead) //Only cull dead.
                                .GroupBy(c => c.Species + "." + c.Generation)
                                .Select(g => new
                                {
                                    Count = g.Count(),
                                    Creatures = g
                                })
                                .Where(x => x.Count > Globals.SuccessfulCreatureSpeciesGenerationExampleLimit)
                                .SelectMany(x =>
                                    x.Creatures
                                    .OrderByDescending(c => c.Fitness)
                                    .Skip(Globals.SuccessfulCreatureSpeciesGenerationExampleLimit)
                                );
                        }

                        //Bring Out Your Dead!
                        int deadCount = Globals.MostSuccessfulCreatures.Count(c => c.Dead);
                        if (deadCount > Globals.SuccessfulCreatureHistoryLength)
                        {
                            Globals.MostSuccessfulCreatures.RemoveAt(deadCount - 1);
                            deadCount = Globals.MostSuccessfulCreatures.Count(c => c.Dead);
                        }
                        Globals.MedianFitness = deadCount > 0 ? Globals.MostSuccessfulCreatures.Where(c => c.Dead).Median(c => c.Fitness) : 0;
                        Globals.LowerHalfMeanFitness = deadCount > 0 ? Globals.MostSuccessfulCreatures.Where(c => c.Dead && c.Fitness <= Globals.MedianFitness).Average(c => c.Fitness) : 0;
                        if (deadCount >= Globals.SuccessfulCreatureHistoryCullingThreshold)
                            Globals.MostSuccessfulCreatures.RemoveAll(c => c.Fitness == 0 || (c.Dead && ((toRemove != null && toRemove.Contains(c)) || c.Fitness <= Globals.LowerHalfMeanFitness - (Globals.LowerHalfMeanFitness * Globals.PercentageBelowLowerHalfMeanFitnessDeletionThreshold))));
                    }
                    Globals.MostSuccessfulCreaturesLock.ExitWriteLock();
                }
            }
            //Remove creatures that died.
            while (!Globals.NewlyDeadCreatures.IsEmpty)
            {
                if (Globals.NewlyDeadCreatures.TryTake(out Creature c))
                {
                    Globals.AllGameObjectsLock.EnterWriteLock();
                    Globals.AllGameObjects.Remove(c);
                    Globals.AllGameObjectsLock.ExitWriteLock();
                    Globals.CreaturesLock.EnterWriteLock();
                    Globals.Creatures.Remove(c);
                    Globals.CreaturesLock.ExitWriteLock();
                    Globals.VisualsLock.EnterWriteLock();
                    Globals.Visuals.Remove(c.Visual);
                    Globals.VisualsLock.ExitWriteLock();
                    deadCreatureCount++;
                }
            }
            //Add creatures that were born.
            while (!Globals.NewlyBornCreatures.IsEmpty)
            {
                if (Globals.NewlyBornCreatures.TryTake(out Creature c))
                {
                    Globals.AllGameObjectsLock.EnterWriteLock();
                    Globals.AllGameObjects.Add(c);
                    Globals.AllGameObjectsLock.ExitWriteLock();
                    Globals.CreaturesLock.EnterWriteLock();
                    Globals.Creatures.Add(c);
                    Globals.CreaturesLock.ExitWriteLock();
                    Globals.VisualsLock.EnterWriteLock();
                    Globals.Visuals.Add(c.Visual);
                    Globals.VisualsLock.ExitWriteLock();
                    if (c.InEgg)
                        eggCountAllTime++;
                }
            }
        }

        public void MouseHandler(GameTime gameTime)
        {
            //Mouse Handling
            previousMouseState = activeMouseState;
            activeMouseState = Mouse.GetState();
            cursor.Position = new Vec2(activeMouseState.Position.X, activeMouseState.Position.Y);
            if (previousMouseState.LeftButton == ButtonState.Released && activeMouseState.LeftButton == ButtonState.Pressed) //Handle left mouse press when it was previously released (to respond only once to a click).
            {
                //Left Click
                Globals.CreaturesLock.EnterReadLock();
                //For some reason the bounding box is sometimes null, shouldn't need to check, it's a workaround for that.. -UdderlyEvelyn 1/7/19
                Selector.SelectMany(Globals.Creatures.Where(c => c.BoundingRect != null && c.BoundingRect.Contains(cursor.Position)));
                Globals.CreaturesLock.ExitReadLock();
            }
            if (previousMouseState.RightButton == ButtonState.Released && activeMouseState.RightButton == ButtonState.Pressed) //Handle right mouse press when it was previously released (to respond only once to a click).
            {
                //Right Click
            }
        }

        public void KeyboardHandler(GameTime gameTime)
        {
            //Keyboard Handling
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
            {
                simulate = false;
                Exit();
            }
            if (ks.IsKeyDown(Keys.P))
                paused = !paused;
            if (ks.IsKeyDown(Keys.L))
                Globals.ParallelUpdateLoop = !Globals.ParallelUpdateLoop;
            if (ks.IsKeyDown(Keys.R))
                render = !render;
            if (ks.IsKeyDown(Keys.K))
            {
                lock (Globals.Creatures)
                {
                    var result = Parallel.ForEach(Globals.Creatures.Where(c => !c.Dead), c => c.Die(null));
                    while (!result.IsCompleted)
                    {
                        /*Wait*/
                    }
                    deadCreatureCount = 0;
                }
            }
            if (ks.IsKeyDown(Keys.D))
                Globals.DecoupleSimulationFromVisuals = !Globals.DecoupleSimulationFromVisuals;
            if (ks.IsKeyDown(Keys.N))
                Globals.ShowNeuralNetworkData = !Globals.ShowNeuralNetworkData;
            if (ks.IsKeyDown(Keys.U))
                Globals.ParallelNeuralNetworkUpdateMethod = !Globals.ParallelNeuralNetworkUpdateMethod;
            if (ks.IsKeyDown(Keys.G))
                Globals.ShowTitleHolderGraphs = !Globals.ShowTitleHolderGraphs;
            if (ks.IsKeyDown(Keys.L))
                Globals.ShowTitleHolderLabels = !Globals.ShowTitleHolderLabels;
        }

        #region Helper Methods From The Past, Consider Moving Or Removing

        void Resolve(Sprite a, Sprite b, Vec2 oldPos, int depth = 0)
        {
            if (++depth > 9) return; //abort if too deep
            Vec2 diff = (b.Position - a.Position);
            float adx = Math.Abs(diff.X);
            float ady = Math.Abs(diff.Y);
            if (adx > ady)
            {
                if (a.BoundingBox.Intersects(b.BoundingBox))
                    a.Position = new Vec2(oldPos.X, a.Position.Y);
                if (a.BoundingBox.Intersects(b.BoundingBox))
                    a.Position = new Vec2(a.Position.X, oldPos.Y);
            }
            else if (adx < ady)
            {
                if (a.BoundingBox.Intersects(b.BoundingBox))
                    a.Position = new Vec2(a.Position.X, oldPos.Y);
                if (a.BoundingBox.Intersects(b.BoundingBox))
                    a.Position = new Vec2(oldPos.X, a.Position.Y);
            }
            Resolve(a, b, oldPos, depth);
        }

        //http://gamedev.stackexchange.com/questions/68460/calculating-wall-angle-and-sliding-in-2d
        Vec2 FindIntersection(Vec2 player, Vec2 motion, Vec2 wall1, Vec2 wall2)
        {
            return new Vec2(
                -(motion.X * (wall1.X * wall2.Y - wall1.Y * wall2.X)
                + motion.X * player.Y * (wall2.X - wall1.X) + motion.Y * player.X
                * (wall1.X - wall2.X)) / (motion.X * (wall1.Y - wall2.Y)
                + motion.Y * (wall2.X - wall1.X)),

                -(motion.Y * (wall1.X * wall2.Y - wall1.Y * wall2.X)
                + motion.X * player.Y * (wall2.Y - wall1.Y) + motion.Y * player.X
                * (wall1.Y - wall2.Y)) / (motion.X * (wall1.Y - wall2.Y)
                + motion.Y * (wall2.X - wall1.X))
            );
        }

        //http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
        public static bool Vec2InTriangle(Vec2 v, Vec2 a, Vec2 b, Vec2 c)
        {
            var s = a.Y * c.X - a.X * c.Y + (c.Y - a.Y) * v.X + (a.X - c.X) * v.Y;
            var t = a.X * b.Y - a.Y * b.X + (a.Y - b.Y) * v.X + (b.X - a.X) * v.Y;

            if ((s < 0) != (t < 0))
                return false;

            var A = -b.Y * c.X + a.Y * (c.X - b.X) + a.X * (b.Y - c.Y) + b.X * c.Y;
            if (A < 0.0)
            {
                s = -s;
                t = -t;
                A = -A;
            }
            return s > 0 && t > 0 && (s + t) < A;
        }

        public bool PointInTriangle(Vec2 v, Vec2 a, Vec2 b, Vec2 c)
        {
            float c1 = Vectors.Vec2.Cross(c - b, v - b);
            float c2 = Vectors.Vec2.Cross(c - b, a - b);
            float c3 = Vectors.Vec2.Cross(c - a, v - a);
            float c4 = Vectors.Vec2.Cross(b - c, a - c);
            float c5 = Vectors.Vec2.Cross(b - a, v - a);
            float c6 = Vectors.Vec2.Cross(b - a, c - a);
            bool test1 = c1 * c2 >= 0;
            bool test2 = c3 * c4 >= 0;
            bool test3 = c5 * c6 >= 0;
            return test1 && test2 && test3;
        }

        /// <summary>
        /// Method to compute the centroid of a polygon. This does NOT work for a complex polygon.
        /// </summary>
        /// <param name="poly">points that define the polygon</param>
        /// <returns>centroid point, or PointF.Empty if something wrong</returns>
        /// http://stackoverflow.com/questions/9815699/how-to-calculate-centroid
        public static Vec2 GetCentroid(List<Vec2> poly)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                float temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                accumulatedArea += temp;
                centerX += (poly[i].X + poly[j].X) * temp;
                centerY += (poly[i].Y + poly[j].Y) * temp;
            }

            if (accumulatedArea < 1E-7f)
                return Vec2.Zero;  // Avoid division by zero

            accumulatedArea *= 3f;
            return new Vec2(centerX / accumulatedArea, centerY / accumulatedArea);
        }

        #endregion
    }
}
