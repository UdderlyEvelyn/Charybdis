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
using Charybdis.MonoGame;

namespace Fortress
{
    public class FortressKernel : Game
    {
        double dayProgress = 0;
        static int minutesPerDay = 3;
        static int secondsPerDay = minutesPerDay * 60;
        static int tileSize = 32;
        static int worldTileWidth = 320;
        static int worldTileHeight = 240;
        static int width = worldTileWidth * tileSize; 
        static int height = worldTileHeight * tileSize;
        int viewportWidth = 1920;
        int viewportHeight = 1080;
        Vec2 viewportCursor = Vec2.Zero;
        Vec2 lastSelectedPosition = Vec2.Zero;
        float viewportSpeedX = (int)(1920 / 1080) * 10;
        float viewportSpeedY = 10;
        MouseState previousMouseState;
        MouseState activeMouseState;
        KeyboardState previousKeyboardState;
        Random random;
        GraphicsDeviceManager gdm;
        RasterizerState rasterizerState;
        DepthStencilState depthStencilState;
        SpriteBatch spriteBatch;
        double seed;
        Font returnOfGanon;
        Font smallFonts;
        int frameRate = 0;
        int frameCounter = 0;
        double memUsage = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        Vec3 movement = Vec3.Zero;
        Vec2 normal = Vec2.Zero;
        List<Sprite> sprites = new List<Sprite>();
        Vec2 velocity = Vec2.Zero;
        Texture2D gasTileTexture;
        Texture2D tileTexture;
        Texture2D geyserTexture;
        Sprite cursor;
        Border selectionBorder = new Border { Color = Col3.White };
        Col4 uiPanelColor = new Col4(80, 80, 80);
        Col4 uiBorderColor = Col4.White;
        TextWindow infoWindow;
        List<FortressObject> selection = new List<FortressObject>();
        List<FortressObject> previousSelection = new List<FortressObject>();
        Array2<Tile> world;
        DateTime lastUpdate = DateTime.Now;
        double lastSecondsSinceLastUpdate = 0;
        long updateCount = 0;
        bool paused = false;
        DateTime? pauseTime = null;
        Array2<float> densityField;
        SurroundingTiles st;
        bool drawUpdateTiles = false;
        Sprite xSprite;
        bool applyTemperatureColor = false;

        public FortressKernel()
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
            GraphicsDevice.Viewport = new Viewport(0, 0, Globals.Width, Globals.Height); //This is needed for release builds to show the borderless window properly (otherwise it renders the top left chunk across the screen and the rest is out of bounds).
            spriteBatch = new SpriteBatch(GraphicsDevice);
            rasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace, FillMode = FillMode.Solid };
            depthStencilState = new DepthStencilState { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less };
            XNA.Initialize2D(GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            returnOfGanon = GraphicsDevice.FontFromFile(Content.RootDirectory + "/Fonts/ReturnOfGanon-12.fnt");
            returnOfGanon.Scale = .5f;
            smallFonts = GraphicsDevice.FontFromFile(Content.RootDirectory + "/Fonts/SmallFonts-32.fnt");
            smallFonts.Scale = .5f;
            returnOfGanon.BackupFont = smallFonts;
            var cursorTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Cursor21.png");
            cursor = new Sprite(cursorTexture); //, PredefinedBoundingRect = new BoundingRect(Vec2.Zero, 3), BoundingOffset = new Vec2(-1, -1) };
            infoWindow = new TextWindow("", returnOfGanon, uiBorderColor, uiPanelColor, Col4.White);
            //infoWindow.Parent = cursor;
            //cursor.Children.Add(infoWindow);

            Material.Water.FreezesInto = Material.WaterIce;
            Material.Water.BoilsInto = Material.Steam;
            Material.WaterIce.MeltsInto = Material.Water;
            Material.Steam.CondensesInto = Material.Water;

            SurroundingTiles.TileSize = tileSize;
            SurroundingTiles.WorldTileWidth = worldTileWidth;
            SurroundingTiles.WorldTileHeight = worldTileHeight;

            Console.Write("Generating Temperature Field..");
            Array2<float> temperatureField = new Array2<float>(worldTileWidth, worldTileHeight,0).NormalizedNoise(seed, Noise.DefaultNoiseArgs);
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Generating Matter Field..");
            Array2<float> matterField = new Array2<float>(worldTileWidth, worldTileHeight, 0).NormalizedNoise(seed + 2, Noise.DefaultNoiseArgs);
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Generating Density Field..");
            densityField = new Array2<float>(worldTileWidth, worldTileHeight, 0).NormalizedNoise(seed + 3, Noise.DefaultNoiseArgs);
            world = new Array2<Tile>(worldTileWidth, worldTileHeight);
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Setting Up Textures From \"" + Content.RootDirectory + "/Textures\"..");
            tileTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Tile32.png");
            gasTileTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/GasTile32.png");
            geyserTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Geyser32.png");
            Scroble.Texture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Scroble.png");
            xSprite = new Sprite(GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/X32.png"));
            Material.Gas.Texture = gasTileTexture;
            Material.Liquid.Texture = gasTileTexture;
            Material.Permanent.Texture = geyserTexture;
            Material.Stone.Texture = tileTexture;
            Material.Vacuum.Texture = geyserTexture;
            Material.Water.Texture = gasTileTexture;
            Material.WaterIce.Texture = tileTexture;
            Material.Steam.Texture = gasTileTexture;
            Material.Dioxygen.Texture = gasTileTexture;
            Material.Hydrogen.Texture = gasTileTexture;
            Material.LiquidHydrogen.Texture = gasTileTexture;
            Material.LiquidOxygen.Texture = gasTileTexture;
            Material.SolidHydrogen.Texture = tileTexture;
            Material.SolidOxygen.Texture = tileTexture;
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Spawning Tiles..");
            int i = 0;
            for (int y = 0; y < worldTileHeight; y++)
            {
                for (int x = 0; x < worldTileWidth; x++)
                {
                    double temperature = ((double)(temperatureField.Get(x, y) * 100)).Round();
                    double density = Math.Exp(densityField.Get(x, y) / 2);
                    Tile tile = null;
                    if (y == 0) //First layer of the world
                    {
                        tile = new SpaceTile
                        {
                            Visual = new Sprite(geyserTexture) { Position = new Vec2(x * tileSize, 0) },
                            Random = random,
                            Mass = 0
                        };
                    }
                    else
                    {
                        if (random.Chance(.00001) && y > 15) //1% Chance, but don't put geysers and random space tiles too close to the top, pointless/boring.
                        {
                            if (random.Chance(.1)) //10% Chance
                            {
                                tile = new SpaceTile
                                {
                                    Visual = new Sprite(geyserTexture) { Position = new Vec2(x * tileSize, y * tileSize) },
                                    Random = random,
                                    Mass = 0
                                };
                            }
                            else //90% Chance
                            {
                                tile = new GeyserTile
                                {
                                    Temperature = new TemperatureF(400),
                                    Visual = new Sprite(geyserTexture) { Position = new Vec2(x * tileSize, y * tileSize) },
                                    Random = random,
                                    Mass = 0
                                };
                            }
                        }
                        else if (density <= .09)
                        {
                            tile = new Tile()
                            {
                                Material = Material.Hydrogen,
                                Visual = new Sprite(gasTileTexture) { Position = new Vec2(x * tileSize, y * tileSize) },
                                Random = random,
                                Temperature = new TemperatureF(temperature),
                                Mass = ((double)(matterField.Get(x, y) * 500)).Round(),
                            };
                        }
                        else if (density <= 1.2)
                        {
                            tile = new Tile()
                            {
                                Material = Material.Dioxygen,
                                Visual = new Sprite(gasTileTexture) { Position = new Vec2(x * tileSize, y * tileSize) },
                                Random = random,
                                Temperature = new TemperatureF(temperature),
                                Mass = ((double)(matterField.Get(x, y) * 500)).Round(),
                            };
                        }
                        else if (density <= 1.25)
                        {
                            tile = new Tile()
                            {
                                Material = Material.Steam,
                                Visual = new Sprite(gasTileTexture) { Position = new Vec2(x * tileSize, y * tileSize) },
                                Random = random,
                                Temperature = Material.Steam.CondensationPoint + temperature,
                                Mass = ((double)(matterField.Get(x, y) * 500)).Round(),
                            };
                        }
                        else
                        {
                            tile = new Tile()
                            {
                                Material = Material.Stone,
                                Visual = new Sprite(tileTexture) { Position = new Vec2(x * tileSize, y * tileSize) },
                                Random = random,
                                Temperature = new TemperatureF(temperature),
                                Mass = ((double)(matterField.Get(x, y) * 1000)).Round(),
                            };
                        }
                    }
                    world.Put(x, y, tile);
                    Globals.AllGameObjects.Add(tile);
                    Globals.Visuals.Add(tile.Visual);
                    i++;
                }
            }
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Launching Background Thread(s)...");
            Tasking.Do(UpdateLoop);
            Console.Write("Done!");
            Console.WriteLine();
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

            int objectsDrawn = 0;
            Vec2 viewportMax = new Vec2(viewportCursor.X + viewportWidth, viewportCursor.Y + viewportHeight);

            Globals.VisualsLock.EnterReadLock();

            foreach (var d2 in Globals.Visuals)
                if (d2.Position.Within(viewportCursor - tileSize, viewportMax))
                {
                    d2.Draw(spriteBatch, viewportCursor);
                    objectsDrawn++;
                }

            Globals.VisualsLock.ExitReadLock();
            
            spriteBatch.DrawShadowedString(returnOfGanon,
                "Charybdis2D Kernel - " + frameRate + "FPS - " + memUsage + "MB" + "\n" +
                "World Size: " + worldTileWidth + "x" + worldTileHeight + "\n" +
                "Viewport Cursor: " + viewportCursor.ToString() + "\n" +
                "Viewport Max: " + viewportMax.ToString() + "\n" +
                "Local Cursor: " + cursor.Position.ToString() + "\n" +
                "Absolute Cursor: " + (cursor.Position + viewportCursor).ToString() + "\n" +
                "Objects Drawn: " + objectsDrawn + "\n" +
                "Selection Count: " + selection.Count + "\n" +
                "Day Progress: " + (((double)dayProgress / (double)secondsPerDay) * 100).Round() + "%\n" +
                "Update Count: " + updateCount + (paused ? " PAUSED (" + (pauseTime.Value - lastUpdate).TotalSeconds.Round() + ")" : "") + "\n" +
                "Last Update: " + lastUpdate.TimeOfDay.ToString() + "\n" +
                "Seconds Since Last Update: " + lastSecondsSinceLastUpdate + "\n" +
                "Mark Update Tiles: " + drawUpdateTiles + "\n" +
                "Apply Temperature Color: " + applyTemperatureColor + "\n" +
                "", Vec2.Zero);
            if (selection.Count > 0)
            {
                infoWindow.Position = new Vec2(cursor.Position.X + cursor.Size.X + 3, cursor.Position.Y);
                Tile t = selection[0] as Tile;
                if (t != null)
                {
                    var tilePosition = t.Visual.Position / tileSize;
                    infoWindow.Text =
                        "Material: " + t.Material.Name + "\n" +
                        "Phase: " + t.Material.Phase.GetName() + "\n" +
                        "Position: " + tilePosition + "\n" +
                        "Mass (Kg): " + t.Mass + "\n" +
                        "Temp (F): " + t.Temperature + "\n" +
                        "Tint: " + (t.Visual as Sprite).Tint.ToString();
                    spriteBatch.DrawLine(infoWindow.Position + (infoWindow.Size / 2), t.Visual.Position + (tileSize / 2) - viewportCursor, Col4.White, 3);
                    infoWindow.Draw(spriteBatch, Vec2.Zero);
                }
            }
            spriteBatch.DrawSprite(cursor); //Draw cursor last so it's always on top.
            if (selection.Count == 0)
            {
                int cursorTileX = (cursor.Position.Xi + viewportCursor.Xi) / tileSize;
                int cursorTileY = (cursor.Position.Yi + viewportCursor.Yi) / tileSize;
                if (!(cursorTileX > world.Width || cursorTileY > world.Height || cursorTileX < 0 || cursorTileY < 0))
                {
                    Tile t = world.Get(cursorTileX, cursorTileY);
                    spriteBatch.DrawShadowedString(returnOfGanon, cursor.Position.ToString() + "\n" + cursorTileX + ", " + cursorTileY + (t != null ? "\n" + t.Temperature.ToString() : "") + "\n" + densityField.Get(cursorTileX, cursorTileY), new Vec2(cursor.Position.X, cursor.Position.Y + tileSize));
                }
            }
            base.Draw(gameTime);
        }

        protected override void EndDraw()
        {
            spriteBatch.End();
            base.EndDraw();
        }

        protected override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
            memUsage = Math.Round((double)GC.GetTotalMemory(false) / 1024 / 1024, 2);
            KeyboardHandler(gameTime);
            MouseHandler(gameTime);
            base.Update(gameTime);
        }

        private void UpdateLoop()
        {
            while (true)
            {
                if (paused)
                {
                    //Visual Update Only
                    for (int y = 0; y < worldTileHeight; y++)
                        for (int x = 0; x < worldTileWidth; x++)
                        {
                            Tile t = world.Get(x, y);
                            t.UpdateVisuals(new SurroundingTiles(world, t), applyTemperatureColor);
                        }
                }
                else
                {
                    DateTime thisUpdate = DateTime.Now;
                    double secondsSinceLastUpdate = (thisUpdate - lastUpdate).TotalSeconds;
                    if (pauseTime.HasValue)
                    {
                        double secondsSincePaused = (pauseTime.Value - lastUpdate).TotalSeconds;
                        if (secondsSincePaused > 1) //If it's been paused for more than a second..
                            secondsSinceLastUpdate = 1; //Treat it as a second.
                        else
                            secondsSinceLastUpdate = secondsSincePaused; //Use the fraction of a second.
                    }
                    secondsSinceLastUpdate = 1;
                    lastSecondsSinceLastUpdate = secondsSinceLastUpdate;
                    dayProgress += secondsSinceLastUpdate;
                    if (dayProgress >= secondsPerDay)
                        dayProgress -= secondsPerDay;
                    lastUpdate = thisUpdate;
                    updateCount++;
                    for (int y = 0; y < worldTileHeight; y++)
                        for (int x = 0; x < worldTileWidth; x++)
                        {
                            //Update each object as necessary.. 
                            Tile t = world.Get(x, y);
                            lock (t)
                            {
                                st = new SurroundingTiles(world, t);
                                if (updateCount == 1) //First Update Loop
                                    t.UpdateVisuals(st); //Make sure we get a color so if temperatures don't change someplace we're not stuck.
                                #region Space Tiles
                                SpaceTile space = t as SpaceTile;
                                if (space != null)
                                {
                                    //Temperature Data Based From https://sciencing.com/temperatures-outer-space-around-earth-20254.html
                                    /*Excerpt:
                                        This solar radiation heats the space near Earth to 393.15 kelvins (120 degrees Celsius or 248 degrees Fahrenheit) or higher, while shaded objects plummet to temperatures lower than 173.5 kelvins (minus 100 degrees Celsius or minus 148 degrees Fahrenheit).
                                     */
                                    //((sin(x*pi*2)+1)/2) * range + start
                                    space.Temperature.Value = ((Math.Sin((dayProgress / secondsPerDay) * Math.PI * 2) + 1) / 2) * 396 - 148; //-148 is minimum temperature, 396 is difference between upper (248) and minimum.
                                    (space.Visual as Sprite).Tint = space.Temperature.GetColor();
                                }
                                #endregion
                                #region Heat Dispersion
                                //Heat Dispersion
                                if (t.Material.DispersesHeat) //If the current tile's material can disperse heat.
                                {
                                    #region Differences
                                    //Differences
                                    Dictionary<string, double> differences = new Dictionary<string, double>();
                                    if (st.TopLeft != null)
                                    {
                                        var dTL = t.Temperature.Value - st.TopLeft.Temperature.Value;
                                        if (dTL > 0)
                                            differences.Add("TL", dTL);
                                    }
                                    if (st.TopCenter != null)
                                    {
                                        var dTC = t.Temperature.Value - st.TopCenter.Temperature.Value;
                                        if (dTC > 0)
                                            differences.Add("TC", dTC);
                                    }
                                    if (st.TopRight != null)
                                    {
                                        var dTR = t.Temperature.Value - st.TopRight.Temperature.Value;
                                        if (dTR > 0)
                                            differences.Add("TR", dTR);
                                    }
                                    if (st.Left != null)
                                    {
                                        var dL = t.Temperature.Value - st.Left.Temperature.Value;
                                        if (dL > 0)
                                            differences.Add("L", dL);
                                    }
                                    if (st.Right != null)
                                    {
                                        var dR = t.Temperature.Value - st.Right.Temperature.Value;
                                        if (dR > 0)
                                            differences.Add("R", dR);
                                    }
                                    if (st.BottomLeft != null)
                                    {
                                        var dBL = t.Temperature.Value - st.BottomLeft.Temperature.Value;
                                        if (dBL > 0)
                                            differences.Add("BL", dBL);
                                    }
                                    if (st.BottomCenter != null)
                                    {
                                        var dBC = t.Temperature.Value - st.BottomCenter.Temperature.Value;
                                        if (dBC > 0)
                                            differences.Add("BC", dBC);
                                    }
                                    if (st.BottomRight != null)
                                    {
                                        var dBR = t.Temperature.Value - st.BottomRight.Temperature.Value;
                                        if (dBR > 0)
                                            differences.Add("BR", dBR);
                                    }
                                    #endregion
                                    #region Determination & Execution
                                    //Determination & Execution
                                    var sorted = differences.OrderByDescending(d => d.Value);
                                    foreach (var kvp in sorted)
                                    {
                                        switch (kvp.Key)
                                        {
                                            case "L":
                                                if (kvp.Value > 0)
                                                {
                                                    if (st.Left.Material.AbsorbsHeat)
                                                    {
                                                        var xl = kvp.Value * ((t.Material.HeatConductivity + st.Left.Material.HeatConductivity) / 2) * secondsSinceLastUpdate / differences.Count;
                                                        if (st.Left.Material.GainsHeat)
                                                        {
                                                            st.Left.Temperature.Value += xl;
                                                            if (st.Left.Material.MeltsInto != null && st.Left.Temperature.Value >= st.Left.Material.MeltingPoint.Value)
                                                                st.Left.Material = st.Left.Material.MeltsInto;
                                                            else if (st.Left.Material.BoilsInto != null && st.Left.Temperature.Value >= st.Left.Material.BoilingPoint.Value)
                                                                st.Left.Material = st.Left.Material.BoilsInto;
                                                            st.Left.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                        if (t.Material.LosesHeat)
                                                        {
                                                            t.Temperature.Value -= xl;
                                                            if (t.Material.FreezesInto != null && t.Temperature.Value <= t.Material.FreezingPoint.Value)
                                                                t.Material = t.Material.FreezesInto;
                                                            else if (t.Material.CondensesInto != null && t.Temperature.Value <= t.Material.CondensationPoint.Value)
                                                                t.Material = t.Material.CondensesInto;
                                                            t.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                    }
                                                }
                                                break;
                                            case "R":
                                                if (kvp.Value > 0)
                                                {
                                                    if (st.Right.Material.AbsorbsHeat)
                                                    {
                                                        var xr = kvp.Value * ((t.Material.HeatConductivity + st.Right.Material.HeatConductivity) / 2) * secondsSinceLastUpdate / differences.Count; ;
                                                        if (st.Right.Material.GainsHeat)
                                                        {
                                                            st.Right.Temperature.Value += xr;
                                                            if (st.Right.Material.MeltsInto != null && st.Right.Temperature.Value >= st.Right.Material.MeltingPoint.Value)
                                                                st.Right.Material = st.Right.Material.MeltsInto;
                                                            else if (st.Right.Material.BoilsInto != null && st.Right.Temperature.Value >= st.Right.Material.BoilingPoint.Value)
                                                                st.Right.Material = st.Right.Material.BoilsInto;
                                                            st.Right.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                        if (t.Material.LosesHeat)
                                                        {
                                                            t.Temperature.Value -= xr;
                                                            if (t.Material.FreezesInto != null && t.Temperature.Value <= t.Material.FreezingPoint.Value)
                                                                t.Material = t.Material.FreezesInto;
                                                            else if (t.Material.CondensesInto != null && t.Temperature.Value <= t.Material.CondensationPoint.Value)
                                                                t.Material = t.Material.CondensesInto;
                                                            t.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                    }
                                                }
                                                break;
                                            case "TC":
                                                if (kvp.Value > 0)
                                                {
                                                    if (st.TopCenter.Material.AbsorbsHeat)
                                                    {
                                                        var xtc = kvp.Value * ((t.Material.HeatConductivity + st.TopCenter.Material.HeatConductivity) / 2) * secondsSinceLastUpdate / differences.Count; ;
                                                        if (st.TopCenter.Material.GainsHeat)
                                                        {
                                                            st.TopCenter.Temperature.Value += xtc;
                                                            if (st.TopCenter.Material.MeltsInto != null && st.TopCenter.Temperature.Value >= st.TopCenter.Material.MeltingPoint.Value)
                                                                st.TopCenter.Material = st.TopCenter.Material.MeltsInto;
                                                            else if (st.TopCenter.Material.BoilsInto != null && st.TopCenter.Temperature.Value >= st.TopCenter.Material.BoilingPoint.Value)
                                                                st.TopCenter.Material = st.TopCenter.Material.BoilsInto;
                                                            st.TopCenter.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                        if (t.Material.LosesHeat)
                                                        {
                                                            t.Temperature.Value -= xtc;
                                                            if (t.Material.FreezesInto != null && t.Temperature.Value <= t.Material.FreezingPoint.Value)
                                                                t.Material = t.Material.FreezesInto;
                                                            else if (t.Material.CondensesInto != null && t.Temperature.Value <= t.Material.CondensationPoint.Value)
                                                                t.Material = t.Material.CondensesInto;
                                                            t.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                    }
                                                }
                                                break;
                                            case "BC":
                                                if (kvp.Value > 0)
                                                {
                                                    if (st.BottomCenter.Material.AbsorbsHeat)
                                                    {
                                                        var xbc = kvp.Value * ((t.Material.HeatConductivity + st.BottomCenter.Material.HeatConductivity) / 2) * secondsSinceLastUpdate / differences.Count; ;
                                                        if (st.BottomCenter.Material.GainsHeat)
                                                        {
                                                            st.BottomCenter.Temperature.Value += xbc;
                                                            if (st.BottomCenter.Material.MeltsInto != null && st.BottomCenter.Temperature.Value >= st.BottomCenter.Material.MeltingPoint.Value)
                                                                st.BottomCenter.Material = st.BottomCenter.Material.MeltsInto;
                                                            else if (st.BottomCenter.Material.BoilsInto != null && st.BottomCenter.Temperature.Value >= st.BottomCenter.Material.BoilingPoint.Value)
                                                                st.BottomCenter.Material = st.BottomCenter.Material.BoilsInto;
                                                            st.BottomCenter.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                        if (t.Material.LosesHeat)
                                                        {
                                                            t.Temperature.Value -= xbc;
                                                            if (t.Material.FreezesInto != null && t.Temperature.Value <= t.Material.FreezingPoint.Value)
                                                                t.Material = t.Material.FreezesInto;
                                                            else if (t.Material.CondensesInto != null && t.Temperature.Value <= t.Material.CondensationPoint.Value)
                                                                t.Material = t.Material.CondensesInto;
                                                            t.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                    }
                                                }
                                                break;
                                            case "TL":
                                                if (kvp.Value > 0)
                                                {
                                                    if (st.TopLeft.Material.AbsorbsHeat)
                                                    {
                                                        var xtl = kvp.Value * ((t.Material.HeatConductivity + st.TopLeft.Material.HeatConductivity) / 2) * secondsSinceLastUpdate / differences.Count;
                                                        if (st.TopLeft.Material.GainsHeat)
                                                        {
                                                            st.TopLeft.Temperature.Value += xtl;
                                                            if (st.TopLeft.Material.MeltsInto != null && st.TopLeft.Temperature.Value >= st.TopLeft.Material.MeltingPoint.Value)
                                                                st.TopLeft.Material = st.TopLeft.Material.MeltsInto;
                                                            else if (st.TopLeft.Material.BoilsInto != null && st.TopLeft.Temperature.Value >= st.TopLeft.Material.BoilingPoint.Value)
                                                                st.TopLeft.Material = st.TopLeft.Material.BoilsInto;
                                                            st.TopLeft.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                        if (t.Material.LosesHeat)
                                                        {
                                                            t.Temperature.Value -= xtl;
                                                            if (t.Material.FreezesInto != null && t.Temperature.Value <= t.Material.FreezingPoint.Value)
                                                                t.Material = t.Material.FreezesInto;
                                                            else if (t.Material.CondensesInto != null && t.Temperature.Value <= t.Material.CondensationPoint.Value)
                                                                t.Material = t.Material.CondensesInto;
                                                            t.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                    }
                                                }
                                                break;
                                            case "TR":
                                                if (kvp.Value > 0)
                                                {
                                                    if (st.TopRight.Material.AbsorbsHeat)
                                                    {
                                                        var xtr = kvp.Value * ((t.Material.HeatConductivity + st.TopRight.Material.HeatConductivity) / 2) * secondsSinceLastUpdate / differences.Count;
                                                        if (st.TopRight.Material.GainsHeat)
                                                        {
                                                            st.TopRight.Temperature.Value += xtr;
                                                            if (st.TopRight.Material.MeltsInto != null && st.TopRight.Temperature.Value >= st.TopRight.Material.MeltingPoint.Value)
                                                                st.TopRight.Material = st.TopRight.Material.MeltsInto;
                                                            else if (st.TopRight.Material.BoilsInto != null && st.TopRight.Temperature.Value >= st.TopRight.Material.BoilingPoint.Value)
                                                                st.TopRight.Material = st.TopRight.Material.BoilsInto;
                                                            st.TopRight.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                        if (t.Material.LosesHeat)
                                                        {
                                                            t.Temperature.Value -= xtr;
                                                            if (t.Material.FreezesInto != null && t.Temperature.Value <= t.Material.FreezingPoint.Value)
                                                                t.Material = t.Material.FreezesInto;
                                                            else if (t.Material.CondensesInto != null && t.Temperature.Value <= t.Material.CondensationPoint.Value)
                                                                t.Material = t.Material.CondensesInto;
                                                            t.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                    }
                                                }
                                                break;
                                            case "BL":
                                                if (kvp.Value > 0)
                                                {
                                                    if (st.BottomLeft.Material.AbsorbsHeat)
                                                    {
                                                        var xbl = kvp.Value * ((t.Material.HeatConductivity + st.BottomLeft.Material.HeatConductivity) / 2) * secondsSinceLastUpdate / differences.Count;
                                                        if (st.BottomLeft.Material.GainsHeat)
                                                        {
                                                            st.BottomLeft.Temperature.Value += xbl;
                                                            if (st.BottomLeft.Material.MeltsInto != null && st.BottomLeft.Temperature.Value >= st.BottomLeft.Material.MeltingPoint.Value)
                                                                st.BottomLeft.Material = st.BottomLeft.Material.MeltsInto;
                                                            else if (st.BottomLeft.Material.BoilsInto != null && st.BottomLeft.Temperature.Value >= st.BottomLeft.Material.BoilingPoint.Value)
                                                                st.BottomLeft.Material = st.BottomLeft.Material.BoilsInto;
                                                            st.BottomLeft.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                        if (t.Material.LosesHeat)
                                                        {
                                                            t.Temperature.Value -= xbl;
                                                            if (t.Material.FreezesInto != null && t.Temperature.Value <= t.Material.FreezingPoint.Value)
                                                                t.Material = t.Material.FreezesInto;
                                                            else if (t.Material.CondensesInto != null && t.Temperature.Value <= t.Material.CondensationPoint.Value)
                                                                t.Material = t.Material.CondensesInto;
                                                            t.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                    }
                                                }
                                                break;
                                            case "BR":
                                                if (kvp.Value > 0)
                                                {
                                                    if (st.BottomRight.Material.AbsorbsHeat)
                                                    {
                                                        var xbr = kvp.Value * ((t.Material.HeatConductivity + st.BottomRight.Material.HeatConductivity) / 2) * secondsSinceLastUpdate / differences.Count;
                                                        if (st.BottomRight.Material.GainsHeat)
                                                        {
                                                            st.BottomRight.Temperature.Value += xbr;
                                                            if (st.BottomRight.Material.MeltsInto != null && st.BottomRight.Temperature.Value >= st.BottomRight.Material.MeltingPoint.Value)
                                                                st.BottomRight.Material = st.BottomRight.Material.MeltsInto;
                                                            else if (st.BottomRight.Material.BoilsInto != null && st.BottomRight.Temperature.Value >= st.BottomRight.Material.BoilingPoint.Value)
                                                                st.BottomRight.Material = st.BottomRight.Material.BoilsInto;
                                                            st.BottomRight.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                        if (t.Material.LosesHeat)
                                                        {
                                                            t.Temperature.Value -= xbr;
                                                            if (t.Material.FreezesInto != null && t.Temperature.Value <= t.Material.FreezingPoint.Value)
                                                                t.Material = t.Material.FreezesInto;
                                                            else if (t.Material.CondensesInto != null && t.Temperature.Value <= t.Material.CondensationPoint.Value)
                                                                t.Material = t.Material.CondensesInto;
                                                            t.UpdateVisuals(st, applyTemperatureColor);
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                                #endregion
                                #endregion
                                #region Gas/Liquid Shifting
                                if (t.Material.Phase == Material.MaterialPhase.Gas || t.Material.Phase == Material.MaterialPhase.Liquid)
                                {
                                    bool tcGasOrLiquid = st.TopCenter != null ? st.TopCenter.Material.Phase == Material.MaterialPhase.Gas || st.TopCenter.Material.Phase == Material.MaterialPhase.Liquid : false;
                                    bool bcGasOrLiquid = st.BottomCenter != null ? st.BottomCenter.Material.Phase == Material.MaterialPhase.Gas || st.BottomCenter.Material.Phase == Material.MaterialPhase.Liquid : false;
                                    if (tcGasOrLiquid && (t.Material.Density < st.TopCenter.Material.Density || (t.Material.Density == st.TopCenter.Material.Density && t.Temperature.Value > st.TopCenter.Temperature.Value))) //T heavier or equal density but hotter than TC (move T up)
                                    {
                                        world.Put(x, y, st.TopCenter);
                                        world.Put(x, y - 1, t);
                                        var oldTilePos = t.Visual.Position;
                                        t.Visual.Position = st.TopCenter.Visual.Position;
                                        st.TopCenter.Visual.Position = oldTilePos;
                                        st.TopCenter.UpdateVisuals(st, applyTemperatureColor);
                                        t.UpdateVisuals(st, applyTemperatureColor);
                                    }
                                    else if (bcGasOrLiquid && (t.Material.Density > st.BottomCenter.Material.Density || (t.Material.Density == st.BottomCenter.Material.Density && t.Temperature.Value < st.BottomCenter.Temperature.Value))) //BC lighter or equal density but hotter than T (move BC up)
                                    {
                                        world.Put(x, y, st.BottomCenter);
                                        world.Put(x, y + 1, t);
                                        var oldTilePos = t.Visual.Position;
                                        t.Visual.Position = st.BottomCenter.Visual.Position;
                                        st.BottomCenter.Visual.Position = oldTilePos;
                                        st.BottomCenter.UpdateVisuals(st, applyTemperatureColor);
                                        t.UpdateVisuals(st, applyTemperatureColor);
                                    }
                                    else
                                    {
                                        bool lGasOrLiquid = st.Left != null ? st.Left.Material.Phase == Material.MaterialPhase.Gas || st.Left.Material.Phase == Material.MaterialPhase.Liquid : false;
                                        bool rGasOrLiquid = st.Right != null ? st.Right.Material.Phase == Material.MaterialPhase.Gas || st.Right.Material.Phase == Material.MaterialPhase.Liquid : false;
                                        if ((lGasOrLiquid && !rGasOrLiquid) || (lGasOrLiquid && random.Chance(.5))) //If left is the only option or a 50% chance.
                                        {
                                            //Swap with tile to the left.
                                            world.Put(x, y, st.Left);
                                            world.Put(x - 1, y, t);
                                            var oldTilePos = t.Visual.Position;
                                            t.Visual.Position = st.Left.Visual.Position;
                                            st.Left.Visual.Position = oldTilePos;
                                            st.Left.UpdateVisuals(st, applyTemperatureColor);
                                            t.UpdateVisuals(st, applyTemperatureColor);
                                        }
                                        else if (rGasOrLiquid) //If right is an option.
                                        {
                                            //Swap with tile to the right.
                                            world.Put(x, y, st.Right);
                                            world.Put(x + 1, y, t);
                                            var oldTilePos = t.Visual.Position;
                                            t.Visual.Position = st.Right.Visual.Position;
                                            st.Right.Visual.Position = oldTilePos;
                                            st.Right.UpdateVisuals(st, applyTemperatureColor);
                                            t.UpdateVisuals(st, applyTemperatureColor);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    Globals.AllGameObjectsLock.EnterReadLock();
                    foreach (Creature c in Globals.AllGameObjects.OfType<Creature>())
                        c.Update();
                    Globals.AllGameObjectsLock.ExitReadLock();
                }
            }
        }

        public void MouseHandler(GameTime gameTime)
        {
            //Mouse Handling
            previousMouseState = activeMouseState;
            activeMouseState = Mouse.GetState();
            if (activeMouseState.Position.X < 0 || activeMouseState.Position.Y < 0 || activeMouseState.Position.X > width || activeMouseState.Position.Y > height) //If mouse isn't in window..
                return; //Don't handle mouse.
            cursor.Position = new Vec2(activeMouseState.Position.X, activeMouseState.Position.Y);
            if (previousMouseState.LeftButton == ButtonState.Released && activeMouseState.LeftButton == ButtonState.Pressed) //Handle left mouse press when it was previously released (to respond only once to a click).
            {
                previousSelection = selection;
                selection.Clear();
                selectionBorder.Parent = null;
                foreach (var gameObject in previousSelection)
                    if (gameObject.Visual.Children.Contains(selectionBorder))
                        gameObject.Visual.Children.Remove(selectionBorder);
                Tile t = world.Get((cursor.Position.Xi + viewportCursor.Xi) / tileSize, (cursor.Position.Yi + viewportCursor.Yi) / tileSize);
                if (t != null)
                {
                    if (t.SelectionEnabled)
                    {
                        t.Selected = true;
                        selection.Add(t);
                        t.Visual.Children.Add(selectionBorder);
                        selectionBorder.Parent = t.Visual;
                    }
                }
                //foreach (var gameObject in allGameObjects.Where(x => x.SelectionEnabled))
                //{
                //    if ((cursor.Position + viewportCursor).Within(gameObject.Visual.Position, gameObject.Visual.Position + gameObject.Visual.Size))
                //    {
                //        gameObject.Selected = true;
                //        selection.Add(gameObject);
                //        gameObject.Visual.Children.Add(selectionBorder);
                //        selectionBorder.Parent = gameObject.Visual;
                //        break; //We only allow one selection (at the moment - nothing breaks or anything).
                //    }
                //}
            }
            if (previousMouseState.RightButton == ButtonState.Released && activeMouseState.RightButton == ButtonState.Pressed) //Handle right mouse press when it was previously released (to respond only once to a click).
            {
                previousSelection = selection;
                selection.Clear();
                foreach (var gameObject in previousSelection)
                    if (gameObject.Visual.Children.Contains(selectionBorder))
                        gameObject.Visual.Children.Remove(selectionBorder);
                selectionBorder.Parent = null;
            }
        }

        public void KeyboardHandler(GameTime gameTime)
        {
            //Keyboard Handling
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
                this.Exit();

            var dDown = ks.IsKeyDown(Keys.Down);
            var dUp = ks.IsKeyDown(Keys.Up);
            var uDown = ks.IsKeyUp(Keys.Down);
            var uUp = ks.IsKeyUp(Keys.Up);
            var dLeft = ks.IsKeyDown(Keys.Left);
            var dRight = ks.IsKeyDown(Keys.Right);
            var uLeft = ks.IsKeyUp(Keys.Left);
            var uRight = ks.IsKeyUp(Keys.Right);
            if (dDown && !dUp)
            {
                if (viewportCursor.Y + viewportHeight < height)
                    viewportCursor.Y += viewportSpeedY;
            }
            if (dUp && !dDown)
            {
                if (viewportCursor.Y > 0)
                    viewportCursor.Y -= viewportSpeedY;
            }
            if (dRight && !dLeft)
            {
                if (viewportCursor.X + viewportWidth < width)
                    viewportCursor.X += viewportSpeedX;
            }
            if (dLeft && !dRight)
            {
                if (viewportCursor.X > 0)
                    viewportCursor.X -= viewportSpeedX;
            }
            if (ks.IsKeyDown(Keys.Space) && previousKeyboardState != null && previousKeyboardState.IsKeyUp(Keys.Space))
            {
                paused = !paused;
                if (paused)
                    pauseTime = DateTime.Now;
                else
                    pauseTime = null;
            }
            if (ks.IsKeyDown(Keys.U) && previousKeyboardState != null && previousKeyboardState.IsKeyUp(Keys.U))
            {
                drawUpdateTiles = !drawUpdateTiles;
            }
            if (ks.IsKeyDown(Keys.T) && previousKeyboardState != null && previousKeyboardState.IsKeyUp(Keys.T))
            {
                applyTemperatureColor = !applyTemperatureColor;
            }
            if (ks.IsKeyDown(Keys.C) && previousKeyboardState != null && previousKeyboardState.IsKeyUp(Keys.C))
            {
                if (selection.Count == 1)
                {
                    Tile t = selection[0] as Tile;
                    if (t != null)
                    {
                        var s = new Scroble(world, t.Visual.Position, delegate (Tile x) { return x.Material.Phase == Material.MaterialPhase.Solid; });
                        Globals.AllGameObjectsLock.EnterWriteLock();
                        Globals.AllGameObjects.Add(s);
                        Globals.AllGameObjectsLock.ExitWriteLock();
                        Globals.VisualsLock.EnterWriteLock();
                        Globals.Visuals.Add(s.Visual);
                        Globals.VisualsLock.ExitWriteLock();
                    }
                }
            }
            if (ks.IsKeyDown(Keys.G))
            {
                if (selection.Count == 1)
                {
                    Tile t = selection[0] as Tile;
                    if (t != null)
                    {
                        var pos = t.Visual.Position;
                        Tile n = new GeyserTile { Temperature = new TemperatureF(400), Visual = new Sprite(geyserTexture) { Position = pos }, Random = t.Random, Mass = t.Mass };
                        world.Put((int)(pos.X / tileSize), (int)(pos.Y / tileSize), n);
                        Globals.AllGameObjectsLock.EnterWriteLock();
                        Globals.AllGameObjects.Remove(t);
                        Globals.AllGameObjects.Add(n);
                        Globals.AllGameObjectsLock.ExitWriteLock();
                        //Clear Selection
                        previousSelection = selection;
                        selection.Clear();
                        foreach (var gameObject in previousSelection)
                            if (gameObject.Visual.Children.Contains(selectionBorder))
                                gameObject.Visual.Children.Remove(selectionBorder);
                        selectionBorder.Parent = null;
                    }
                }
            }
            if (ks.IsKeyDown(Keys.V))
            {
                if (selection.Count == 1)
                {
                    Tile t = selection[0] as Tile;
                    if (t != null)
                    {
                        var pos = t.Visual.Position;
                        Tile n = new VacuumTile { Visual = new Sprite(geyserTexture) { Position = pos }, Random = t.Random, Mass = 0 };
                        world.Put((int)(pos.X / tileSize), (int)(pos.Y / tileSize), n);
                        Globals.AllGameObjectsLock.EnterWriteLock();
                        Globals.AllGameObjects.Remove(t);
                        Globals.AllGameObjects.Add(n);
                        Globals.AllGameObjectsLock.ExitWriteLock();
                        //Clear Selection
                        previousSelection = selection;
                        selection.Clear();
                        foreach (var gameObject in previousSelection)
                            if (gameObject.Visual.Children.Contains(selectionBorder))
                                gameObject.Visual.Children.Remove(selectionBorder);
                        selectionBorder.Parent = null;
                    }
                }
            }

            /*if (ks.IsKeyDown(Keys.W))
                velocity.Y = -1;
            if (ks.IsKeyDown(Keys.S))
                velocity.Y = 1;
            if (ks.IsKeyDown(Keys.A))
                velocity.X = -1;
            if (ks.IsKeyDown(Keys.D))
                velocity.X = 1;
            sprite.GenerateBoundingRect();
            if (ks.IsKeyDown(Keys.G))
                GC.Collect();
            if (ks.IsKeyDown(Keys.Home))
                if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
            Keys[] pressedKeys = ks.GetPressedKeys();
            if (ks.IsKeyDown(Keys.OemTilde))
            {
                if (ks.IsKeyDown(Keys.LeftShift)) rasterizerState.FillMode = FillMode.Solid;
                else rasterizerState.FillMode = FillMode.WireFrame;
            }
            if (ks.IsKeyDown(Keys.T))
                if (ks.IsKeyDown(Keys.LeftShift)) Globals.Test = false;
                else Globals.Test = true;

            if (velocity != Vec2.Zero)
            {
                Vec2 oldPos = sprite.Position;
                sprite.Position = sprite.Position + velocity;
                foreach (Sprite s in sprites.Except(new List<Sprite> { sprite }))
                {*/
            #region subtriangle detection (commented out)
            //n++;
            /*if (sprite.BoundingRect.Intersects(s.BoundingRect))
            {
                float minX = s.BoundingRect.Min.X;
                float minY = s.BoundingRect.Min.Y;
                float maxX = s.BoundingRect.Max.X;
                float maxY = s.BoundingRect.Max.Y;
                Vec2 center = new Vec2(sprite.Position.X + sprite.Data.Width / 2, sprite.Position.Y + sprite.Data.Height / 2);
                Vec2 TL = new Vec2(minX, minY);
                Vec2 TR = new Vec2(maxX, minY);
                Vec2 BL = new Vec2(minX, maxY);
                Vec2 BR = new Vec2(maxX, maxY);
                float midY = minY + (maxY - minY) / 2;
                float midX = minX + (maxX - minX) / 2;
                Vec2 C = new Vec2(midX, midY);
                Vec2 TM = new Vec2(midX, minY);
                Vec2 BM = new Vec2(midX, maxY);
                Vec2 LM = new Vec2(minX, midY);
                Vec2 RM = new Vec2(maxX, midY);
                if (center.X > minX && center.X < midX)
                {
                    //left half
                    if (center.Y > minY && center.Y < midY)
                    {
                        //top left
                        if (Vec2InTriangle(center, C, TM, TL))
                        {
                            //upper triangle
                            Vec2 tri = GetCentroid(new List<Vec2> { C, TM, TL });
                            triX = (int)tri.X;
                            triY = (int)tri.Y;
                        }
                        else
                        {
                            //lower triangle
                            Vec2 tri = GetCentroid(new List<Vec2> { C, LM, TL });
                            triX = (int)tri.X;
                            triY = (int)tri.Y;
                        }
                    }
                    else
                    {
                        //bottom left                                
                        if (Vec2InTriangle(center, C, BM, BL))
                        {
                            //lower triangle
                            Vec2 tri = GetCentroid(new List<Vec2> { C, BM, BL });
                            triX = (int)tri.X;
                            triY = (int)tri.Y;
                        }
                        else
                        {
                            //upper triangle
                            Vec2 tri = GetCentroid(new List<Vec2> { C, LM, BL });
                            triX = (int)tri.X;
                            triY = (int)tri.Y;
                        }
                    }
                }
                else
                {
                    //right half
                    if (center.Y > minY && center.Y < midY)
                    {
                        //top right
                        if (Vec2InTriangle(center, C, TM, TR))
                        {
                            //upper triangle
                            Vec2 tri = GetCentroid(new List<Vec2> { C, TM, TR });
                            triX = (int)tri.X;
                            triY = (int)tri.Y;
                        }
                        else
                        {
                            //lower triangle
                            Vec2 tri = GetCentroid(new List<Vec2> { C, RM, TR });
                            triX = (int)tri.X;
                            triY = (int)tri.Y;
                        }
                    }
                    else
                    {
                        //bottom right
                        if (Vec2InTriangle(center, C, BM, BR))
                        {
                            //lower triangle
                            Vec2 tri = GetCentroid(new List<Vec2> { C, BM, BR });
                            triX = (int)tri.X;
                            triY = (int)tri.Y;
                        }
                        else
                        {
                            //upper triangle
                            Vec2 tri = GetCentroid(new List<Vec2> { C, RM, BR });
                            triX = (int)tri.X;
                            triY = (int)tri.Y;
                        }
                    }
                }

            }*/
            //tw.Flush();
            #endregion
            //Resolve(sprite, s, oldPos);
            //velocity = Vec2.Zero;
            //}
            //}

            previousKeyboardState = ks;
        }

        //void Resolve(Sprite a, Sprite b, Vec2 oldPos, int depth = 0)
        //{
        //    if (++depth > 9) return; //abort if too deep
        //    Vec2 diff = (b.Position - a.Position);
        //    float adx = (float)Math.Abs(diff.X);
        //    float ady = (float)Math.Abs(diff.Y);
        //    if (adx > ady)
        //    {
        //        if (a.BoundingBox.Intersects(b.BoundingBox))
        //            a.Position = new Vec2(oldPos.X, a.Position.Y);
        //        if (a.BoundingBox.Intersects(b.BoundingBox))
        //            a.Position = new Vec2(a.Position.X, oldPos.Y);
        //    }
        //    else if (adx < ady)
        //    {
        //        if (a.BoundingBox.Intersects(b.BoundingBox))
        //            a.Position = new Vec2(a.Position.X, oldPos.Y);
        //        if (a.BoundingBox.Intersects(b.BoundingBox))
        //            a.Position = new Vec2(oldPos.X, a.Position.Y);
        //    }
        //    Resolve(a, b, oldPos, depth);
        //}

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
            float c1 = Vec2.Cross(c - b, v - b);
            float c2 = Vec2.Cross(c - b, a - b);
            float c3 = Vec2.Cross(c - a, v - a);
            float c4 = Vec2.Cross(b - c, a - c);
            float c5 = Vec2.Cross(b - a, v - a);
            float c6 = Vec2.Cross(b - a, c - a);
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
    }
}
