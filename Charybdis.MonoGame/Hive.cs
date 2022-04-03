//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Xna;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Content;
//using System.Diagnostics;
//using Charybdis.Library.Core;
//using Charybdis.Science;
//using Charybdis.MonoGame.Evolution;
//using System.Collections.Concurrent;
//using System.IO;
//using System.Runtime.CompilerServices;

//namespace Charybdis.MonoGame
//{
//    public class Hive : Game
//    {
//        MouseState previousMouseState;
//        MouseState activeMouseState;
//        Random random;
//        GraphicsDeviceManager gdm;
//        RasterizerState rasterizerState;
//        DepthStencilState depthStencilState;
//        SpriteBatch spriteBatch;
//        double seed;
//        Font font1;
//        Font font2;
//        int frameRate = 0;
//        int frameCounter = 0;
//        int simulationRate = 0;
//        int simulationCounter = 0;
//        double memUsage = 0;
//        TimeSpan elapsedTime = TimeSpan.Zero;
//        Vec3 movement = Vec3.Zero;
//        float POS89RADS;
//        float NEG89RADS;
//        Vec2 normal = Vec2.Zero;
//        List<Sprite> sprites = new List<Sprite>();
//        Vec2 velocity = Vec2.Zero;
//        Sprite cursor;
//        Border selectionBorder = new Border { Color = Col3.White };
//        Col4 uiPanelColor = new Col4(80, 80, 80);
//        Col4 uiBorderColor = Col4.White;
//        TextWindow infoWindow;
//        bool paused = false;
//        int deadCreatureCount = 0;
//        int eggCountAllTime = 0;
//        ulong updates = 0;
//        List<object> epochTracker = new List<object>();
//        bool render = true;
//        bool simulate = true;
//        //Label mostFitLabel;
//        Task simulationTask;
//        ulong injectionCount = 0;
//        ushort injectionRate = 0;
//        ushort injectionCounter = 0;
//        ulong spawnedCreatureCount = 0;
//        Label mostFitLabel;
//        Label longestLineageLabel;
//        Label oldestLabel;
//        Label smallestLabel;
//        Label largestLabel;
//        Vec2 graphOffset = new Vec2(250, 0);
//        Vec2 labelOffset = new Vec2(-500, 0);
//        double lastSeasonalTemperatureMultiplier = Globals.SeasonalTemperatureMultiplier;
//        //static FileStream logFile = File.OpenWrite("Charybdis-" + DateTime.Now.ToString("MMddyyyyhhmmss") + ".log");
//        //StreamWriter sw = new StreamWriter(logFile);
//        //bool log = false;

//        //public void Log(string line)
//        //{
//        //    if (log)
//        //        lock (logFile)
//        //            sw.WriteLine("[" + DateTime.Now.ToShortDateString() + " @ " + DateTime.Now.ToShortTimeString() + "] " + line);
//        //}

//        //public void LogMethodEntry([CallerMemberName] string caller = null)
//        //{
//        //    Log("Entering \"" + caller + "\"..");
//        //}

//        //public void LogMethodExit([CallerMemberName] string caller = null)
//        //{
//        //    Log("Exiting \"" + caller + "\"..");
//        //}

//        public Hive()
//        {
//            //LogMethodEntry();
//            random = new Random();
//            Window.Title = Console.Title = "Charybdis Kernel (2D)";
//            Window.IsBorderless = true;
//            gdm = new GraphicsDeviceManager(this);
//            seed = random.NextDouble();
//            Content.RootDirectory = "Data";
//            gdm.PreferredBackBufferWidth = Globals.Width;
//            gdm.PreferredBackBufferHeight = Globals.Height;
//            IsMouseVisible = false;
//            IsFixedTimeStep = Globals.DecoupleSimulationFromVisuals;
//            //LogMethodExit();
//        }

//        protected override void Initialize()
//        {
//            //LogMethodEntry();
//            POS89RADS = MathHelper.ToRadians(89);
//            NEG89RADS = MathHelper.ToRadians(-89);
//            Globals.GraphicsDevice = GraphicsDevice;
//            GraphicsDevice.Viewport = new Viewport(0, 0, Globals.Width, Globals.Height); //This is needed for release builds to show the borderless window properly (otherwise it renders the top left chunk across the screen and the rest is out of bounds).
//            spriteBatch = new SpriteBatch(GraphicsDevice);
//            rasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace, FillMode = FillMode.Solid };
//            depthStencilState = new DepthStencilState { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less };
//            XNA.Initialize2D(GraphicsDevice);
//            base.Initialize();
//            //LogMethodExit();
//        }

//        protected override void LoadContent()
//        {
//            //LogMethodEntry();
//            Globals.Font = font1 = GraphicsDevice.FontFromFile(Content.RootDirectory + "/Fonts/Nyala-12.fnt");
//            font1.Scale = 2f;
//            font2 = GraphicsDevice.FontFromFile(Content.RootDirectory + "/Fonts/Nyala-12.fnt");
//            font2.Scale = 1f;
//            var cursorTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Cursor21.png");
//            cursor = new Sprite { Data = cursorTexture, PredefinedBoundingRect = new BoundingRect(Vec2.Zero, 3), BoundingOffset = new Vec2(-1, -1) };
//            infoWindow = new TextWindow("", font1, uiBorderColor, uiPanelColor, Col4.White);
//            infoWindow.Parent = cursor;
//            cursor.Children.Add(infoWindow);

//            if (Globals.FitnessBasedEvolution)
//                mostFitLabel = new Label { Font = Globals.Font, Shadowed = true };
//            longestLineageLabel = new Label { Font = Globals.Font, Shadowed = true };
//            oldestLabel = new Label { Font = Globals.Font, Shadowed = true };
//            smallestLabel = new Label { Font = Globals.Font, Shadowed = true };
//            largestLabel = new Label { Font = Globals.Font, Shadowed = true };

//            //Log("Generating World..");

//            Globals.Heightmap = new Array2<byte>(Globals.WorldWidth, Globals.WorldHeight);
//            //Globals.Heightmap.RandomStatic();
//            Globals.Heightmap.Perlin(Globals.HeightmapPerlinOctaves, Globals.HeightmapMaximumHeight);
//            //Globals.Heightmap.Perlin();
//            //Noise.SimplexNoise(Globals.Heightmap, Noise.DefaultNoiseArgs);
//            //(130*sin(x/81)-10)        

//            //Noise.NormalizedNoise(Globals.Climate, seed, Noise.DefaultNoiseArgs);

//            Globals.World = new Array2<Tile>(Globals.WorldWidth, Globals.WorldHeight);
//            for (int y = 0; y < Globals.WorldHeight; y++)
//                for (int x = 0; x < Globals.WorldWidth; x++)
//                {
//                    Tile t;
//                    var height = Globals.Heightmap.Get(x, y);
//                    if (Globals.Random.Chance(.004)) //1 in 250 chance for a death tile.
//                        t = new DeathTile(Globals.TileSize, Globals.GridColor, height);
//                    else
//                    {
//                        if (height >= Globals.SteepHeightThreshold)
//                        {
//                            var st = new SteepTile(Globals.TileSize, Globals.GridColor, height)
//                            {
//                                Position = new Vec2(x * Globals.TileSize, y * Globals.TileSize),
//                            };
//                            Globals.AllGameObjects.Add(st);
//                            t = st;
//                        }
//                        else if (height <= Globals.Sealevel)
//                        {
//                            var wt = new WaterTile(Globals.TileSize, Globals.GridColor, height)
//                            {
//                                Position = new Vec2(x * Globals.TileSize, y * Globals.TileSize),
//                            };
//                            Globals.AllGameObjects.Add(wt);
//                            t = wt;
//                        }
//                        else
//                        {
//                            var at = new ArableTile(Globals.TileSize, Globals.GridColor, height)
//                            {
//                                Position = new Vec2(x * Globals.TileSize, y * Globals.TileSize),
//                            };
//                            at.Biomass = Maths.Clamp(at.MaximumBiomass * Globals.Random.NextDouble() * Globals.StartingBiomassPercentage, 0, at.MaximumBiomass);
//                            t = at;
//                            Globals.AllGameObjects.Add(at);
//                        }
//                    }
//                    Globals.World.Set(x, y, t);
//                    Globals.Tiles.Add(t);
//                    Globals.Visuals.Add(t.Visual);
//                }
//            //Log("World Generation Complete!");
//            //Log("Spawning Initial Population..");
//            for (int i = 0; i < Globals.CreatureInjectionCount; i++)
//            {
//                //Log("Spawning " + i + "/" + Globals.CreatureInjectionCount);
//                var newCreaturePosition = new Vec2(random.Next(1, Globals.WorldPixelWidth - 1), random.Next(1, Globals.WorldPixelHeight - 1));
//                while (!(Globals.FindTileForPosition(newCreaturePosition) is ArableTile)) //Make sure it gets a fair start.
//                    newCreaturePosition = new Vec2(random.Next(1, Globals.WorldPixelWidth - 1), random.Next(1, Globals.WorldPixelHeight - 1));
//                Globals.NewlyBornCreatures.Add(new Creature() { Position = newCreaturePosition });
//                spawnedCreatureCount++;
//            }
//            //Log("Spawning Complete!");
//            //mostFitLabel = new Label { Text = "MOST FIT", Font = Globals.Font, Shadowed = true };
//            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;
//            if (Globals.DecoupleSimulationFromVisuals)
//                simulationTask = Tasking.Do(SimulationLoop);
//            base.LoadContent();
//            //LogMethodExit();
//        }

//        protected override bool BeginDraw()
//        {
//            //LogMethodEntry();
//            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
//            //LogMethodExit();
//            return base.BeginDraw();
//        }

//        protected override void Draw(GameTime gameTime)
//        {
//            //LogMethodEntry();
//            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
//            GraphicsDevice.RasterizerState = rasterizerState;
//            GraphicsDevice.DepthStencilState = depthStencilState;
//            GraphicsDevice.BlendState = BlendState.AlphaBlend;

//            if (render)
//            {
//                //Log("Rendering..");
//                Globals.VisualsLock.EnterReadLock();
//                //int renderCount = 0;
//                foreach (var id2 in Globals.Visuals)
//                {
//                    //Log("Rendering " + ++renderCount + "/" + Globals.Visuals.Count);
//                    id2.Draw(spriteBatch);
//                }
//                Globals.VisualsLock.ExitReadLock();
//                //Log("Rendering Complete!");
//            }
//            else
//                spriteBatch.DrawShadowedString(font1, "RENDERING DISABLED FOR PERFORMANCE", new Vec2(Globals.Width / 2, Globals.Height / 2) - (font1.CalculateTextSize("RENDERING DISABLED FOR PERFORMANCE") / 2));

//            //Log("Updating Title Holder Labels..");      
//            Creature mostFit = null;
//            if (Globals.FitnessBasedEvolution)
//            {
//                mostFit = Globals.TitleHolders[Title.MostFit];
//                if (mostFit != null)
//                {
//                    mostFitLabel.Text = "Most Fit\n" + mostFit.Label;
//                    mostFitLabel.Position = mostFit.Visual.Position - (Globals.Font.CalculateTextSize(mostFitLabel.Text) / 2);
//                    mostFitLabel.Draw(spriteBatch);
//                    var directionGraphOrigin = mostFit.Visual.Position + graphOffset;
//                    var directionDeltaGraphOrigin = mostFit.Visual.Position + graphOffset + new Vec2(-100, 250);
//                    for (int i = 0; i < 199; i++)
//                    {
//                        float colorValue = (float)(mostFit.EffortHistory[i] * 255);

//                        new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(mostFit.DirectionHistory[i])),
//                                 directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(mostFit.DirectionHistory[i + 1])))
//                        { Color = new Col3(colorValue, 0, colorValue) }
//                        .Draw(spriteBatch);

//                        //new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(mostFit.DesiredDirectionHistory[i])),
//                        //         directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(mostFit.DesiredDirectionHistory[i + 1])))
//                        //{ Color = new Col3(colorValue, colorValue, colorValue) }
//                        //.Draw(spriteBatch);

//                        new Line(directionDeltaGraphOrigin + new Vec2(i, (float)(mostFit.DirectionDeltaHistory[i])),
//                                 directionDeltaGraphOrigin + new Vec2(i + 1, (float)(mostFit.DirectionDeltaHistory[i + 1])))
//                        { Color = new Col3(colorValue, 0, colorValue) }
//                        .Draw(spriteBatch);
//                    }
//                    new Line(directionGraphOrigin + Vec2.NZ * 4, directionGraphOrigin + Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch);
//                    new Line(directionGraphOrigin + Vec2.ZN * 4, directionGraphOrigin + Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch);
//                    new Line(directionDeltaGraphOrigin + Vec2.NZ * 4, directionDeltaGraphOrigin + Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch);
//                    new Line(directionDeltaGraphOrigin + Vec2.ZN * 4, directionDeltaGraphOrigin + Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch);
//                }
//            }
//            //Longest Lineage Title Label
//            var longestLineage = Globals.TitleHolders[Title.LongestLineage];
//            if (longestLineage != null)
//            {
//                longestLineageLabel.Text = "Longest Lineage\n" + longestLineage.Label;
//                longestLineageLabel.Position = longestLineage.Visual.Position - (Globals.Font.CalculateTextSize(longestLineageLabel.Text) / 2);
//                longestLineageLabel.Draw(spriteBatch);
//                var directionGraphOrigin = longestLineage.Visual.Position + graphOffset;
//                var directionDeltaGraphOrigin = longestLineage.Visual.Position + graphOffset + new Vec2(-100, 250);
//                for (int i = 0; i < 199; i++)
//                {
//                    float colorValue = (float)(longestLineage.EffortHistory[i] * 255);

//                    new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(longestLineage.DirectionHistory[i])),
//                             directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(longestLineage.DirectionHistory[i + 1])))
//                    { Color = new Col3(colorValue, 0, colorValue) }
//                    .Draw(spriteBatch);

//                    //new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(longestLineage.DesiredDirectionHistory[i])),
//                    //         directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(longestLineage.DesiredDirectionHistory[i + 1])))
//                    //{ Color = new Col3(colorValue, colorValue, colorValue) }
//                    //.Draw(spriteBatch);

//                    new Line(directionDeltaGraphOrigin + new Vec2(i, (float)(longestLineage.DirectionDeltaHistory[i])),
//                             directionDeltaGraphOrigin + new Vec2(i + 1, (float)(longestLineage.DirectionDeltaHistory[i + 1])))
//                    { Color = new Col3(colorValue, 0, colorValue) }
//                    .Draw(spriteBatch);
//                }
//                new Line(directionGraphOrigin + Vec2.NZ * 4, directionGraphOrigin + Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionGraphOrigin + Vec2.ZN * 4, directionGraphOrigin + Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionDeltaGraphOrigin + Vec2.NZ * 4, directionDeltaGraphOrigin + Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionDeltaGraphOrigin + Vec2.ZN * 4, directionDeltaGraphOrigin + Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch);
//            }
//            //Oldest Title Label
//            var oldest = Globals.TitleHolders[Title.Oldest];
//            if (oldest != null)
//            {
//                oldestLabel.Text = "Oldest\n" + oldest.Label;
//                oldestLabel.Position = oldest.Visual.Position - (Globals.Font.CalculateTextSize(oldestLabel.Text) / 2);
//                oldestLabel.Draw(spriteBatch);
//                var directionGraphOrigin = oldest.Visual.Position + graphOffset;
//                var directionDeltaGraphOrigin = oldest.Visual.Position + graphOffset + new Vec2(-100, 250);
//                for (int i = 0; i < 199; i++)
//                {
//                    float colorValue = (float)(oldest.EffortHistory[i] * 255);

//                    new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(oldest.DirectionHistory[i])),
//                             directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(oldest.DirectionHistory[i + 1])))
//                    { Color = new Col3(colorValue, 0, colorValue) }
//                    .Draw(spriteBatch);

//                    //new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(oldest.DesiredDirectionHistory[i])),
//                    //         directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(oldest.DesiredDirectionHistory[i + 1])))
//                    //{ Color = new Col3(colorValue, colorValue, colorValue) }
//                    //.Draw(spriteBatch);

//                    new Line(directionDeltaGraphOrigin + new Vec2(i, (float)(oldest.DirectionDeltaHistory[i])),
//                             directionDeltaGraphOrigin + new Vec2(i + 1, (float)(oldest.DirectionDeltaHistory[i + 1])))
//                    { Color = new Col3(colorValue, 0, colorValue) }
//                    .Draw(spriteBatch);
//                }
//                new Line(directionGraphOrigin + Vec2.NZ * 4, directionGraphOrigin + Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionGraphOrigin + Vec2.ZN * 4, directionGraphOrigin + Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionDeltaGraphOrigin + Vec2.NZ * 4, directionDeltaGraphOrigin + Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionDeltaGraphOrigin + Vec2.ZN * 4, directionDeltaGraphOrigin + Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch);
//            }
//            else
//                oldestLabel.DrawMe = false;
//            //Smallest Title Label
//            var smallest = Globals.TitleHolders[Title.Smallest];
//            if (smallest != null)
//            {
//                smallestLabel.Text = "Smallest\n" + smallest.Label;
//                smallestLabel.Position = smallest.Visual.Position - (Globals.Font.CalculateTextSize(smallestLabel.Text) / 2);
//                smallestLabel.Draw(spriteBatch);
//                var directionGraphOrigin = smallest.Visual.Position + graphOffset;
//                var directionDeltaGraphOrigin = smallest.Visual.Position + graphOffset + new Vec2(-100, 250);
//                for (int i = 0; i < 199; i++)
//                {
//                    float colorValue = (float)(smallest.EffortHistory[i] * 255);

//                    new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(smallest.DirectionHistory[i])),
//                             directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(smallest.DirectionHistory[i + 1])))
//                    { Color = new Col3(colorValue, 0, colorValue) }
//                    .Draw(spriteBatch);

//                    //new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(smallest.DesiredDirectionHistory[i])),
//                    //         directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(smallest.DesiredDirectionHistory[i + 1])))
//                    //{ Color = new Col3(colorValue, colorValue, colorValue) }
//                    //.Draw(spriteBatch);

//                    new Line(directionDeltaGraphOrigin + new Vec2(i, (float)(smallest.DirectionDeltaHistory[i])),
//                             directionDeltaGraphOrigin + new Vec2(i + 1, (float)(smallest.DirectionDeltaHistory[i + 1])))
//                    { Color = new Col3(colorValue, 0, colorValue) }
//                    .Draw(spriteBatch);
//                }
//                new Line(directionGraphOrigin + Vec2.NZ * 4, directionGraphOrigin + Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionGraphOrigin + Vec2.ZN * 4, directionGraphOrigin + Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionDeltaGraphOrigin + Vec2.NZ * 4, directionDeltaGraphOrigin + Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionDeltaGraphOrigin + Vec2.ZN * 4, directionDeltaGraphOrigin + Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch);
//            }
//            //Largest Title Label
//            var largest = Globals.TitleHolders[Title.Largest];
//            if (largest != null)
//            {
//                largestLabel.Text = "Largest\n" + largest.Label;
//                largestLabel.Position = largest.Visual.Position - (Globals.Font.CalculateTextSize(largestLabel.Text) / 2);
//                largestLabel.Draw(spriteBatch);
//                var directionGraphOrigin = largest.Visual.Position + graphOffset;
//                var directionDeltaGraphOrigin = largest.Visual.Position + graphOffset + new Vec2(-100, 250);
//                for (int i = 0; i < 199; i++)
//                {
//                    float colorValue = (float)(largest.EffortHistory[i] * 255);

//                    new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(largest.DirectionHistory[i])),
//                             directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(largest.DirectionHistory[i + 1])))
//                    { Color = new Col3(colorValue, 0, colorValue) }
//                    .Draw(spriteBatch);

//                    //new Line(directionGraphOrigin + Maths.CirclePointAtAngle(i, (float)(largest.DesiredDirectionHistory[i])),
//                    //         directionGraphOrigin + Maths.CirclePointAtAngle(i + 1, (float)(largest.DesiredDirectionHistory[i + 1])))
//                    //{ Color = new Col3(colorValue, colorValue, colorValue) }
//                    //.Draw(spriteBatch);

//                    new Line(directionDeltaGraphOrigin + new Vec2(i, (float)(largest.DirectionDeltaHistory[i])),
//                             directionDeltaGraphOrigin + new Vec2(i + 1, (float)(largest.DirectionDeltaHistory[i + 1])))
//                    { Color = new Col3(colorValue, 0, colorValue) }
//                    .Draw(spriteBatch);
//                }
//                new Line(directionGraphOrigin + Vec2.NZ * 4, directionGraphOrigin + Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionGraphOrigin + Vec2.ZN * 4, directionGraphOrigin + Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionDeltaGraphOrigin + Vec2.NZ * 4, directionDeltaGraphOrigin + Vec2.PZ * 4) { Color = Col3.White }.Draw(spriteBatch);
//                new Line(directionDeltaGraphOrigin + Vec2.ZN * 4, directionDeltaGraphOrigin + Vec2.ZP * 4) { Color = Col3.White }.Draw(spriteBatch);
//            }

//            //Log("Counting Creatures & Eggs..");
//            Globals.CreaturesLock.EnterReadLock();
//            var creatureCount = Globals.Creatures.Count(c => !c.InEgg);
//            var eggCount = Globals.Creatures.Count(c => c.InEgg);
//            Globals.CreaturesLock.ExitReadLock();
//            //Log("Creating Display Text..");
//            var temperatureMultiplierChangeSign = Math.Sign(Globals.SeasonalTemperatureMultiplier - lastSeasonalTemperatureMultiplier);
//            string display =
//                "Charybdis2D Kernel - " + frameRate + "FPS - " + memUsage + "MB" +
//                "\nSimulation Updates/Sec: " + simulationRate + " (" + (Globals.DecoupleSimulationFromVisuals ? "Decoupled" : "Bound To Framerate") + ")" +
//                "\nParallel Update Loop: " + (Globals.ParallelUpdateLoop ? "Enabled" : "Disabled") +
//                "\nFitness-Based Evolution: " + (Globals.FitnessBasedEvolution ? "Enabled" : "Disabled") +
//                "\nHardship System: " + (Globals.HardshipSystemEnabled ? "Enabled" : "Disabled") +
//                "\nInjections/Sec: " + injectionRate + " (Total: " + injectionCount + ")" +
//                "\nEpoch: " + epochTracker.Count +
//                "\nYear: " + ((double)updates / Globals.YearLength).Round() +
//                "\nSeason: " + (Globals.SeasonalTemperatureMultiplier >= Globals.BaseTemperatureVariation ? "Summer" : "Winter") + " (" + (Globals.SeasonalTemperatureMultiplier.Round() * 100) + "% " + (temperatureMultiplierChangeSign > 0 ? "And Rising" : (temperatureMultiplierChangeSign < 0 ? "And Falling" : "Stagnant")) + ")" +
//                "\nWorld: " + Globals.WorldWidth + "x" + Globals.WorldHeight + " (Tiles), " + Globals.WorldPixelWidth + "x" + Globals.WorldPixelHeight + "px" +
//                "\nCreatures: " + creatureCount;
//            if (Globals.UseEggs)
//                display += "\nEggs: " + eggCount + " (All Time: " + eggCountAllTime + ")";
//            display +=
//                "\nDead: " + deadCreatureCount +
//                "\nLongest Lineage: " + (longestLineage != null ? (longestLineage.Generation + " (" + longestLineage.Species + ")") : "N/A") +
//                "\nOldest: " + (oldest != null ? (((double)oldest.Updates / Globals.YearLength).Round() + " (" + (((double)oldest.Updates / (double)oldest.MaximumLifespan).Round() * 100) + "%, Generation " + oldest.Generation + ", Species " + oldest.Species + ")") : "N/A") +
//                "\nLargest: " + (largest != null ? (largest.Size.Value.Round() + " (Size Mutability: " + largest.Size.Mutability.Round() + ", Species " + largest.Species + ")") : "N/A") +
//                "\nSmallest: " + (smallest != null ? (smallest.Size.Value.Round() + " (Size Mutability: " + smallest.Size.Mutability.Round() + ", Species " + smallest.Species + ")") : "N/A");
//            if (Globals.FitnessBasedEvolution)
//            {
//                display +=
//                "\n" +
//                "\nFitness Board" +
//                "\n-------------" +
//                "\nMedian: " + Globals.MedianFitness.Round() +
//                "\nLower Half Mean: " + Globals.LowerHalfMeanFitness.Round() +
//                "\nCulled At/Below: " + (Globals.LowerHalfMeanFitness - (Globals.LowerHalfMeanFitness * Globals.PercentageBelowLowerHalfMeanFitnessDeletionThreshold)).Round() + " (When 20 On Board)" +
//                "\n-------------" +
//                "\nLiving: " + (mostFit != null ? mostFit.ToString() : "N/A") +
//                "";
//                Globals.MostSuccessfulCreaturesLock.EnterReadLock();
//                for (int i = 0; i < Globals.SuccessfulCreatureHistoryDisplayLength; i++)
//                    display += "\n" + (i + 1) + ": " + (Globals.MostSuccessfulCreatures.Count > i ? Globals.MostSuccessfulCreatures[i].ToString() : "N/A");
//                Globals.MostSuccessfulCreaturesLock.ExitReadLock();
//            }
//            spriteBatch.DrawShadowedString(font1, display, Vec2.Zero);
//            if (paused)
//                spriteBatch.DrawShadowedString(font1, "PAUSED", new Vec2(Globals.Width / 2, Globals.Height / 2) - (font1.CalculateTextSize("PAUSED") / 2));
//            spriteBatch.DrawSprite(cursor); //Draw cursor last so it's always on top.
//            lastSeasonalTemperatureMultiplier = Globals.SeasonalTemperatureMultiplier;
//            base.Draw(gameTime);
//            //LogMethodExit();
//        }

//        protected override void EndDraw()
//        {
//            //LogMethodEntry();
//            spriteBatch.End();
//            base.EndDraw();
//            //LogMethodExit();
//        }

//        protected override void Update(GameTime gameTime)
//        {
//            //LogMethodEntry();
//            elapsedTime += gameTime.ElapsedGameTime;
//            if (elapsedTime > TimeSpan.FromSeconds(1))
//            {
//                elapsedTime -= TimeSpan.FromSeconds(1);
//                frameRate = frameCounter;
//                frameCounter = 0;
//                Globals.SimulationFramesPerSecond = simulationRate = simulationCounter;
//                simulationCounter = 0;
//                injectionRate = injectionCounter;
//                injectionCounter = 0;
//                memUsage = Math.Round((double)GC.GetTotalMemory(false) / 1024 / 1024, 2);
//                //Log("Stats At " + DateTime.Now.ToShortTimeString() + ": " + frameRate + "FPS, " + simulationRate + "SFPS, " + memUsage + "MB");
//                //lock (logFile)
//                //    sw.Flush();
//            }
//            KeyboardHandler(gameTime);
//            MouseHandler(gameTime);
//            if (paused)
//            {
//                //Log("Paused.");
//                base.Update(gameTime);
//                //LogMethodExit();
//                return;
//            }
//            if (simulate)
//            {
//                if (!Globals.DecoupleSimulationFromVisuals) //If we're performing this on the same thread..
//                {
//                    //Log("Processing Single Simulation Frame On Same Thread..");
//                    PerformSimulationFrame(); //Do one simulation frame.
//                }
//                else if (simulationTask != null && simulationTask.Status != TaskStatus.Running) //We're simulating on a separate thread, so make sure threads are running and restart them if not (and mention it in the console if they faulted).
//                {
//                    //Log("Simulation Task Was Not Running, Restarting It!");
//                    Console.WriteLine("Simulation thread status was " + simulationTask.Status.GetName(false) + ", restarting..\n\nException:\n" + simulationTask.Exception.ToString());
//                    simulationTask = Tasking.Do(SimulationLoop);
//                }
//            }
//            base.Update(gameTime);
//            //LogMethodExit();
//        }

//        private Comparison<Creature> _creatureFitnessComparison = new Comparison<Creature>(
//            (c1, c2) =>
//            {
//                if (c1.Fitness > c2.Fitness) return -1;
//                else if (c2.Fitness > c1.Fitness) return 1;
//                else return 0;
//            });

//        public void SimulationLoop()
//        {
//            //LogMethodEntry();
//            while (simulate)
//                PerformSimulationFrame();
//            //LogMethodExit();
//        }

//        public void PerformSimulationFrame()
//        {
//            //LogMethodEntry();
//            if (paused)
//                return;
//            simulationCounter++;
//            updates++;
//        //Hand-tuned this..
//        //OLD: https://www.graphsketch.com/?eqn1_color=1&eqn1_eqn=.75%20-%20(sin(x%2F160)%20%2F%202)&eqn2_color=2&eqn2_eqn=&eqn3_color=3&eqn3_eqn=&eqn4_color=4&eqn4_eqn=&eqn5_color=5&eqn5_eqn=&eqn6_color=6&eqn6_eqn=&x_min=-100&x_max=1000&y_min=-.15&y_max=1.5&x_tick=20&y_tick=.05&x_label_freq=3&y_label_freq=3&do_grid=0&do_grid=1&bold_labeled_lines=0&bold_labeled_lines=1&line_width=4&image_w=850&image_h=525
//        //https://www.graphsketch.com/?eqn1_color=1&eqn1_eqn=.75%20%2B%20sin(x%20%2F%2010000%20*%20pi)%20*%20.25&eqn2_color=2&eqn2_eqn=.75&eqn3_color=3&eqn3_eqn=&eqn4_color=4&eqn4_eqn=&eqn5_color=5&eqn5_eqn=&eqn6_color=6&eqn6_eqn=&x_min=-500&x_max=10000&y_min=-.1&y_max=1&x_tick=1000&y_tick=.1&x_label_freq=1&y_label_freq=1&do_grid=0&do_grid=1&bold_labeled_lines=0&bold_labeled_lines=1&line_width=4&image_w=850&image_h=525
//            //Log("Updating Seasonal Temperature Modifier..");
//            Globals.SeasonalTemperatureMultiplier = Globals.BaseTemperatureVariation + Math.Sin(updates / Globals.YearLength * Globals.SeasonFrequency) * Globals.SeasonToYearRatio;
//            if (updates == ulong.MaxValue)
//            {
//                //Log("New Epoch (" + epochTracker.Count + ")!");
//                epochTracker.Add(new object());
//                updates = 0;
//            }
//            bool injectionHappened = false;
//            //Spawn new creatures if necessary.
//            if (Globals.Creatures.Where(c => !c.Dead).Count() + Globals.NewlyBornCreatures.Count < Globals.MinimumCreaturesInWorld)
//            {
//                //Log("Injecting Creatures..");
//                for (int i = 0; i < Globals.CreatureInjectionCount; i++)
//                {
//                    /*int j = i;
//                    if (j < 0)
//                        j = 0;*/
//                    //Log("Injecting Creature " + i + "/" + Globals.CreatureInjectionCount + "..");
//                    var newCreaturePosition = new Vec2(random.Next(1, Globals.WorldPixelWidth - 1), random.Next(1, Globals.WorldPixelHeight - 1));
//                    while (!(Globals.FindTileForPosition(newCreaturePosition) is ArableTile)) //Make sure it gets a fair start.
//                        newCreaturePosition = new Vec2(random.Next(1, Globals.WorldPixelWidth - 1), random.Next(1, Globals.WorldPixelHeight - 1));
//                    if (Globals.MostSuccessfulCreatures.Count > i)
//                        Globals.NewlyBornCreatures.Add(new Creature(Globals.MostSuccessfulCreatures[i], false) { Position = newCreaturePosition });
//                    else
//                        Globals.NewlyBornCreatures.Add(new Creature() { Position = newCreaturePosition });
//                }
//                injectionHappened = true;
//                injectionCounter++;
//                injectionCount++;
//                //deadCreatureCount = 0;
//                //Log("Injection Complete!");
//            }
//            //Store current ones in a variable which will also track best fits while we iterate.
//            var mostFit = Globals.TitleHolders[Title.MostFit];
//            var longestLineage = Globals.TitleHolders[Title.LongestLineage];
//            var oldest = Globals.TitleHolders[Title.Oldest];
//            var smallest = Globals.TitleHolders[Title.Smallest];
//            var largest = Globals.TitleHolders[Title.Largest];
//            //If there's more than 16 creatures (not counting eggs) use parallelism here, otherwise probably faster not to.
//            //Globals.ParallelUpdateLoop = Globals.Creatures.Count(c => !c.InEgg) > 16;                    
//            Globals.AllGameObjectsLock.EnterReadLock();
//            if (Globals.ParallelUpdateLoop)
//            {
//                //Log("Entering Parallel Update Loop..");
//                var parallelResult = Parallel.ForEach(Globals.AllGameObjects, igo =>
//                {
//                    ((IUpdateable)igo).Update();
//                    var c = igo as Creature;
//                    if (c != null && !c.InEgg)
//                    {
//                        if (Globals.FitnessBasedEvolution)
//                            if ((mostFit == null || (mostFit.Dead || c.Fitness > mostFit.Fitness) && mostFit != c))
//                                mostFit = c;
//                        if ((longestLineage == null || (longestLineage.Dead || c.Generation > longestLineage.Generation) && longestLineage != c))
//                            longestLineage = c;
//                        if ((oldest == null || (oldest.Dead || c.Updates > oldest.Updates) && oldest != c))
//                            oldest = c;
//                        if ((smallest == null || (smallest.Dead || c.Size.Value < smallest.Size.Value) && smallest != c))
//                            smallest = c;
//                        if ((largest == null || (largest.Dead || c.Size.Value > largest.Size.Value) && largest != c))
//                            largest = c;
//                    }
//                    if (Globals.BiomassRefillOnInjection && injectionHappened)
//                    {
//                        var at = igo as ArableTile;
//                        if (at != null)
//                            at.Biomass = at.MaximumBiomass;
//                    }
//                });
//                while (!parallelResult.IsCompleted)
//                {
//                    //Wait for completion.
//                }
//                //Log("Exiting Parallel Update Loop!");
//            }
//            else
//            {
//                //Log("Entering Linear Update Loop..");
//                foreach (IUpdateable iu in Globals.AllGameObjects)
//                {
//                    iu.Update();
//                    var c = iu as Creature;
//                    if (c != null)
//                    {
//                        if (Globals.FitnessBasedEvolution)
//                            if ((mostFit == null || (mostFit.Dead || c.Fitness > mostFit.Fitness) && mostFit != c))
//                                mostFit = c;
//                        if ((longestLineage == null || (longestLineage.Dead || c.Generation > longestLineage.Generation) && longestLineage != c))
//                            longestLineage = c;
//                        if ((oldest == null || (oldest.Dead || c.Updates > oldest.Updates) && oldest != c))
//                            oldest = c;
//                        if ((smallest == null || (smallest.Dead || c.Size.Value < smallest.Size.Value) && smallest != c))
//                            smallest = c;
//                        if ((largest == null || (largest.Dead || c.Size.Value > largest.Size.Value) && largest != c))
//                            largest = c;
//                    }
//                    if (Globals.BiomassRefillOnInjection && injectionHappened)
//                    {
//                        var at = iu as ArableTile;
//                        if (at != null)
//                            at.Biomass = at.MaximumBiomass;
//                    }
//                }
//                //Log("Exiting Linear Update Loop!");
//            }
//            Globals.AllGameObjectsLock.ExitReadLock();
//            //Update title holders.
//            Globals.TitleHolders[Title.LongestLineage] = longestLineage;
//            Globals.TitleHolders[Title.Oldest] = oldest;
//            Globals.TitleHolders[Title.Smallest] = smallest;
//            Globals.TitleHolders[Title.Largest] = largest;
//            if (Globals.FitnessBasedEvolution)
//            {
//                Globals.TitleHolders[Title.MostFit] = mostFit;
//                //Add most fit to the board, re-sort and remove least fit.
//                if (mostFit != null && mostFit.Fitness > 0)
//                {
//                    Globals.MostSuccessfulCreaturesLock.EnterWriteLock();
//                    if (!Globals.MostSuccessfulCreatures.Contains(mostFit))
//                    {
//                        Globals.MostSuccessfulCreatures.Add(mostFit);
//                        Globals.MostSuccessfulCreatures.Sort(_creatureFitnessComparison);
//                        if (Globals.MostSuccessfulCreatures.Count > Globals.SuccessfulCreatureHistoryLength)
//                            Globals.MostSuccessfulCreatures.RemoveAt(Globals.MostSuccessfulCreatures.Count - 1);
//                        var deadSuccessfulCreatureCount = Globals.MostSuccessfulCreatures.Count(c => c.Dead);
//                        Globals.MedianFitness = deadSuccessfulCreatureCount > 0 ? Globals.MostSuccessfulCreatures.Where(c => c.Dead).Median(c => c.Fitness) : 0;
//                        Globals.LowerHalfMeanFitness = deadSuccessfulCreatureCount > 0 ? Globals.MostSuccessfulCreatures.Where(c => c.Dead && c.Fitness <= Globals.MedianFitness).Average(c => c.Fitness) : 0;
//                        if (deadSuccessfulCreatureCount >= Globals.SuccessfulCreatureHistoryCullingThreshold)
//                            /*if (*/Globals.MostSuccessfulCreatures.RemoveAll(c => c.Dead && c.Fitness <= Globals.LowerHalfMeanFitness - (Globals.LowerHalfMeanFitness * Globals.PercentageBelowLowerHalfMeanFitnessDeletionThreshold));// == 0)
//                                /*if (Globals.Random.Chance(Globals.ChanceToRemoveExtraCreatureFromBottomOfHistory))
//                                    Globals.MostSuccessfulCreatures.RemoveAt(Globals.MostSuccessfulCreatures.Count - 1);*/
//                    }
//                    Globals.MostSuccessfulCreaturesLock.ExitWriteLock();
//                }
//            }
//            bool requiresCleanup = false;
//            //Remove creatures that died.
//            while (!Globals.NewlyDeadCreatures.IsEmpty)
//            {
//                //Log("Removing Dead Creatures (" + Globals.NewlyDeadCreatures.Count + ")..");
//                Creature c = null;
//                if (Globals.NewlyDeadCreatures.TryTake(out c))
//                {
//                    requiresCleanup = true;
//                    Globals.AllGameObjectsLock.EnterWriteLock();
//                    Globals.AllGameObjects.Remove(c);
//                    Globals.AllGameObjectsLock.ExitWriteLock();
//                    Globals.CreaturesLock.EnterWriteLock();
//                    Globals.Creatures.Remove(c);
//                    Globals.CreaturesLock.ExitWriteLock();
//                    Globals.VisualsLock.EnterWriteLock();
//                    Globals.Visuals.Remove(c.Visual);
//                    Globals.VisualsLock.ExitWriteLock();
//                    deadCreatureCount++;
//                }
//                //Log("Removing Dead Creatures Complete!");
//            }
//            //Add creatures that were born.
//            while (!Globals.NewlyBornCreatures.IsEmpty)
//            {
//                //Log("Adding New Creatures (" + Globals.NewlyBornCreatures.Count + ")..");
//                Creature c = null;
//                if (Globals.NewlyBornCreatures.TryTake(out c))
//                {
//                    requiresCleanup = true;
//                    Globals.AllGameObjectsLock.EnterWriteLock();
//                    Globals.AllGameObjects.Add(c);
//                    Globals.AllGameObjectsLock.ExitWriteLock();
//                    Globals.CreaturesLock.EnterWriteLock();
//                    Globals.Creatures.Add(c);
//                    Globals.CreaturesLock.ExitWriteLock();
//                    Globals.VisualsLock.EnterWriteLock();
//                    Globals.Visuals.Add(c.Visual);
//                    Globals.VisualsLock.ExitWriteLock();
//                    if (c.InEgg)
//                        eggCountAllTime++;
//                }
//                //Log("Adding New Creatures Complete!");
//            }
//            if (requiresCleanup)
//                GC.Collect();
//            //LogMethodExit();
//        }

//        public void MouseHandler(GameTime gameTime)
//        {
//            //LogMethodEntry();
//            //Mouse Handling
//            previousMouseState = activeMouseState;
//            activeMouseState = Mouse.GetState();
//            cursor.Position = new Vec2(activeMouseState.Position.X, activeMouseState.Position.Y);
//            if (previousMouseState.LeftButton == ButtonState.Released && activeMouseState.LeftButton == ButtonState.Pressed) //Handle left mouse press when it was previously released (to respond only once to a click).
//            {
//                //Left Click
//            }
//            if (previousMouseState.RightButton == ButtonState.Released && activeMouseState.RightButton == ButtonState.Pressed) //Handle right mouse press when it was previously released (to respond only once to a click).
//            {
//                //Right Click
//            }
//            //LogMethodExit();
//        }

//        public void KeyboardHandler(GameTime gameTime)
//        {
//            //LogMethodEntry();
//            //Keyboard Handling
//            KeyboardState ks = Keyboard.GetState();
//            if (ks.IsKeyDown(Keys.Escape))
//            {
//                simulate = false;
//                Exit();
//            }
//            if (ks.IsKeyDown(Keys.P))
//                paused = !paused;
//            if (ks.IsKeyDown(Keys.L))
//                Globals.ParallelUpdateLoop = !Globals.ParallelUpdateLoop;
//            if (ks.IsKeyDown(Keys.R))
//                render = !render;
//            if (ks.IsKeyDown(Keys.K))
//            {
//                lock (Globals.Creatures)
//                {
//                    var result = Parallel.ForEach(Globals.Creatures.Where(c => !c.Dead), c => c.Die(null));
//                    while (!result.IsCompleted)
//                    {
//                        /*Wait*/
//                    }
//                    deadCreatureCount = 0;
//                }
//            }
//            //LogMethodExit();
//        }

//        void Resolve(Sprite a, Sprite b, Vec2 oldPos, int depth = 0)
//        {
//            if (++depth > 9) return; //abort if too deep
//            Vec2 diff = (b.Position - a.Position);
//            float adx = Math.Abs(diff.X);
//            float ady = Math.Abs(diff.Y);
//            if (adx > ady)
//            {
//                if (a.BoundingBox.Intersects(b.BoundingBox))
//                    a.Position = new Vec2(oldPos.X, a.Position.Y);
//                if (a.BoundingBox.Intersects(b.BoundingBox))
//                    a.Position = new Vec2(a.Position.X, oldPos.Y);
//            }
//            else if (adx < ady)
//            {
//                if (a.BoundingBox.Intersects(b.BoundingBox))
//                    a.Position = new Vec2(a.Position.X, oldPos.Y);
//                if (a.BoundingBox.Intersects(b.BoundingBox))
//                    a.Position = new Vec2(oldPos.X, a.Position.Y);
//            }
//            Resolve(a, b, oldPos, depth);
//        }

//        //http://gamedev.stackexchange.com/questions/68460/calculating-wall-angle-and-sliding-in-2d
//        Vec2 FindIntersection(Vec2 player, Vec2 motion, Vec2 wall1, Vec2 wall2)
//        {
//            return new Vec2(
//                -(motion.X * (wall1.X * wall2.Y - wall1.Y * wall2.X)
//                + motion.X * player.Y * (wall2.X - wall1.X) + motion.Y * player.X
//                * (wall1.X - wall2.X)) / (motion.X * (wall1.Y - wall2.Y)
//                + motion.Y * (wall2.X - wall1.X)),

//                -(motion.Y * (wall1.X * wall2.Y - wall1.Y * wall2.X)
//                + motion.X * player.Y * (wall2.Y - wall1.Y) + motion.Y * player.X
//                * (wall1.Y - wall2.Y)) / (motion.X * (wall1.Y - wall2.Y)
//                + motion.Y * (wall2.X - wall1.X))
//            );
//        }

//        //http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
//        public static bool Vec2InTriangle(Vec2 v, Vec2 a, Vec2 b, Vec2 c)
//        {
//            var s = a.Y * c.X - a.X * c.Y + (c.Y - a.Y) * v.X + (a.X - c.X) * v.Y;
//            var t = a.X * b.Y - a.Y * b.X + (a.Y - b.Y) * v.X + (b.X - a.X) * v.Y;

//            if ((s < 0) != (t < 0))
//                return false;

//            var A = -b.Y * c.X + a.Y * (c.X - b.X) + a.X * (b.Y - c.Y) + b.X * c.Y;
//            if (A < 0.0)
//            {
//                s = -s;
//                t = -t;
//                A = -A;
//            }
//            return s > 0 && t > 0 && (s + t) < A;
//        }

//        public bool PointInTriangle(Vec2 v, Vec2 a, Vec2 b, Vec2 c)
//        {
//            float c1 = Vec2.Cross(c - b, v - b);
//            float c2 = Vec2.Cross(c - b, a - b);
//            float c3 = Vec2.Cross(c - a, v - a);
//            float c4 = Vec2.Cross(b - c, a - c);
//            float c5 = Vec2.Cross(b - a, v - a);
//            float c6 = Vec2.Cross(b - a, c - a);
//            bool test1 = c1 * c2 >= 0;
//            bool test2 = c3 * c4 >= 0;
//            bool test3 = c5 * c6 >= 0;
//            return test1 && test2 && test3;
//        }

//        /// <summary>
//        /// Method to compute the centroid of a polygon. This does NOT work for a complex polygon.
//        /// </summary>
//        /// <param name="poly">points that define the polygon</param>
//        /// <returns>centroid point, or PointF.Empty if something wrong</returns>
//        /// http://stackoverflow.com/questions/9815699/how-to-calculate-centroid
//        public static Vec2 GetCentroid(List<Vec2> poly)
//        {
//            float accumulatedArea = 0.0f;
//            float centerX = 0.0f;
//            float centerY = 0.0f;

//            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
//            {
//                float temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
//                accumulatedArea += temp;
//                centerX += (poly[i].X + poly[j].X) * temp;
//                centerY += (poly[i].Y + poly[j].Y) * temp;
//            }

//            if (accumulatedArea < 1E-7f)
//                return Vec2.Zero;  // Avoid division by zero

//            accumulatedArea *= 3f;
//            return new Vec2(centerX / accumulatedArea, centerY / accumulatedArea);
//        }
//    }
//}
