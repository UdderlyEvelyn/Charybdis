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

namespace Kolony
{
    public class KolonyKernel : Game
    {
        bool cubeCoordinateDebug = false;
        double dayProgress = 0;
        static int minutesPerDay = 3;
        static int secondsPerDay = minutesPerDay * 60;
        static int tileSize = 32;
        //static int tileHalfSize = tileSize / 2;
        static int worldWidth = 16;
        static int worldHeight = 16;
        static int worldDepth = 5;
        static int worldCountPerLayer = worldWidth * worldHeight;
        static int worldCount = worldCountPerLayer * worldDepth;
        //static int width = worldTileWidth * tileSize; 
        //static int height = worldTileHeight * tileSize;
        //static int size = width * height;
        int viewportWidth = 1920;
        int viewportHeight = 1080;
        Vec2 viewportCursor = Vec2.Zero;
        //Vec2 lastSelectedPosition = Vec2.Zero;
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
        //Texture2D tileTexture;
        Texture2D stoneCubeTexture;
        Sprite cursor;
        //Border selectionBorder = new Border { Color = Col3.White };
        Col4 uiPanelColor = new Col4(80, 80, 80);
        Col4 uiBorderColor = Col4.White;
        TextWindow infoWindow;
        List<KolonyObject> selection = new List<KolonyObject>();
        //List<KolonyObject> previousSelection = new List<KolonyObject>();
        Array3<Cube> world;
        DateTime lastUpdate = DateTime.Now;
        double lastSecondsSinceLastUpdate = 0;
        long updateCount = 0;
        bool paused = false;
        DateTime? pauseTime = null;
        Array2<float> heightmap;
        bool drawUpdateTiles = false;
        bool applyTemperatureColor = false;
        //Cube cubeAtCursor = null;
        static Vec2 cubeScale = new Vec2(2);
        static Vec2 halfCubeScale = cubeScale / 2;
        static Vec2 cubeSize;

        public KolonyKernel()
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
            Console.Write("Generating Matter Field..");
            Array2<float> matterField = new Array2<float>(worldWidth, worldHeight, 0).NormalizedNoise(seed + 2, Noise.DefaultNoiseArgs);
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Generating Density Field..");
            heightmap = new Array2<float>(worldWidth, worldHeight, 0).NormalizedNoise(seed + 3, Noise.DefaultNoiseArgs);
            //densityField = new Array2<float>(worldWidth, worldHeight, 0).NormalizedNoise(seed + 3, Noise.DefaultNoiseArgs);
            world = new Array3<Cube>(worldWidth, worldHeight, worldDepth);
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Setting Up Textures From \"" + Content.RootDirectory + "/Textures\"..");
            Material.Stone.Texture = stoneCubeTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/greyCube.png");
            cubeSize = stoneCubeTexture.Bounds.Size.ToVector2().ToCharybdis();
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Spawning Cubes..");
            //for (int i = 0; i < worldTileCount; i++)
            for (int y = 0; y < worldHeight; y++)
            {
                for (int x = 0; x < worldWidth; x++)
                {
                    for (int z = 0; z < worldDepth; z++)
                    {
                        //double temperature = ((double)(temperatureField.Get(x, y) * 100)).Round();
                        //double density = Math.Exp(densityField.Get(x, y) / 2);
                        Cube cube = null;
                        //Tile gen logic by density would go here..

                        var pos = GridToWorld(new Vec2(x, y), z);
                        var textureBounds = Material.Stone.Texture.Bounds;
                        var sprite = new Sprite(Material.Stone.Texture, pos, cubeScale);
                        //,
                        //(((((float)y * (float)worldWidth + (float)x)) / (float)worldCountPerLayer)));// * .9f)
                        //    + 
                        //    (((float)z / (float)worldDepth)) * .1f);
                        //float maxHeightAtXY = heightmap.Get(x, y) * (float)worldDepth;
                        int depthColor = (int)((((float)worldDepth - (float)z) / (float)worldDepth) * 255f);
                        sprite.Tint = new Col4(depthColor, depthColor, depthColor);
                        cube = new Cube()
                        {
                            Depth = worldDepth - z,
                            Position = pos,
                            Material = Material.Stone,
                            Visual = sprite,
                            Random = random,
                            Temperature = new TemperatureF(60),
                            Mass = ((double)(matterField.Get(x, y) * 1000)).Round(),
                            //BoundingRect = new BoundingRect(pos, new Vec2(textureBounds.X * 2, textureBounds.Y * 2)),
                            Coordinates = CubeCoordinates.Get(pos, cubeSize, cubeScale, sprite.Origin),
                        };
                        world.Set(x, y, z, cube);
                        Globals.AllGameObjects.Add(cube);
                        if (worldDepth - z > heightmap.Get(x, y) * worldDepth) //If this position doesn't have enough height..
                        {
                            sprite.Tint = new Col4(255, 255, 255,0); //Invisible.
                            cube.SelectionEnabled = false; //Unselectable.
                        }
                    }
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

        public Vec2 GridToWorld(Vec2 position, float z)
        {
            var x = position.X * cubeSize.X * halfCubeScale.X;
            var y = position.Y * cubeSize.Y * halfCubeScale.Y;
            var halfSizeY = cubeSize.Y * cubeScale.Y / 2;
            var halfSizeX = cubeSize.X * cubeScale.X / 2;
            //var halfSizeY = cubeSize.Y * halfCubeScale.Y / 2;
            return new Vec2(x, y + (x/2) + (halfSizeY * z));
        }

        public Vec2 WorldToGrid(Vec2 position)
        {
            var x = position.X / (cubeSize.X * halfCubeScale.X);
            var y = (position.Y - (position.X / 2)) / (cubeSize.Y * halfCubeScale.Y);
            return new Vec2(x, y);
        }

        protected override bool BeginDraw()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);//, DepthStencilState.Default);
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

            //Globals.VisualsLock.EnterReadLock();

            //var viewportBounds = new BoundingRect(viewportCursor - tileSize, viewportMax);
            for (int y = 0; y < worldHeight; y++)
                for (int x = 0; x < worldWidth; x++)
                    for (int z = worldDepth - 1; z >= 0; z--)
                    {
                        var c = world.Get(x, y, z);
                        if (c.Visual.Position.Within(viewportCursor, viewportMax))
                        {
                            c.Visual.Draw(spriteBatch, viewportCursor);
                            objectsDrawn++;
                        }
                    }
            //foreach (var go2 in Globals.AllGameObjects)
            //    if (go2.Visual != null && go2.Visual.Position.Within(viewportCursor, viewportMax))
            //    {
            //        go2.Visual.Draw(spriteBatch, viewportCursor);
            //        objectsDrawn++;
            //    }
            if (cubeCoordinateDebug)
                foreach (var go2 in Globals.AllGameObjects)
                    if (go2.Visual != null && go2.Visual.Position.Within(viewportCursor, viewportMax))
                    {
                        var cube = go2 as Cube;
                        spriteBatch.DrawLine(go2.Visual.Position - viewportCursor, cursor.Position, Col4.Green, 2, 1);
                        spriteBatch.DrawLine(cube.Coordinates.TopFaceCenter - viewportCursor, cursor.Position, Col4.Purple, 2, 1);
                        spriteBatch.DrawLine(cube.Coordinates.LeftFaceCenter - viewportCursor, cursor.Position, Col4.Blue, 2, 1);
                        spriteBatch.DrawLine(cube.Coordinates.RightFaceCenter - viewportCursor, cursor.Position, Col4.Red, 2, 1);
                    }

            //Globals.VisualsLock.ExitReadLock();
            
            spriteBatch.DrawShadowedString(returnOfGanon,
                "Charybdis2D Kernel - " + frameRate + "FPS - " + memUsage + "MB" + "\n" +
                "World Size: " + worldWidth + "x" + worldHeight + "\n" +
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
                Cube c = selection[0] as Cube;
                if (c != null)
                {
                    var sprite = (Sprite)c.Visual;
                    var worldCursorPosition = cursor.Position + viewportCursor;
                    infoWindow.Text =
                        "Material: " + c.Material.Name + "\n" +
                        "Position: " + c.Position + "\n" +
                        "World Position: " + c.WorldPosition + "\n" +
                        "Sprite Depth: " + sprite.Depth + "\n" +
                        "Sprite Tint: " + sprite.Tint + "\n" +
                        "Mass (Kg): " + c.Mass + "\n" +
                        "Temp (F): " + c.Temperature;// + "\n\n" +

                        //"Mouse In Top Face: " + c.Coordinates.WithinTopFace(worldCursorPosition) + " (alt " + c.Coordinates.WithinTopFaceAlt(worldCursorPosition) + ")\n" +
                        //"Mouse In Left Face: " + c.Coordinates.WithinLeftFace(worldCursorPosition) + "\n" +
                        //"Mouse In Right Face: " + c.Coordinates.WithinRightFace(worldCursorPosition) + "\n" +
                        //"Mouse In Cube: " + c.Coordinates.WithinCube(worldCursorPosition) + "\n\n" +

                        //c.Coordinates;

                    //var scaledSize = c.Visual.Size * ((Sprite)c.Visual).Scale;
                    //The Y/2 is cube-specific to point at center of top face.
                    //var lineTarget = c.Visual.Position + new Vec2(scaledSize.X, scaledSize.Y / 2) / 2;
                    spriteBatch.DrawLine(infoWindow.Position + (infoWindow.Size / 2), c.Coordinates.TopFaceCenter - viewportCursor, Col4.White, 3);
                    //Below is what it was before adjusting for isometry or cubes.
                    //spriteBatch.DrawLine(infoWindow.Position + (infoWindow.Size / 2), c.Visual.Position + (c.Visual.Size / 2) - viewportCursor, Col4.White, 3);
                    infoWindow.Draw(spriteBatch, Vec2.Zero);
                }
            }
            spriteBatch.DrawSprite(cursor); //Draw cursor last so it's always on top.
            //if (selection.Count == 0)
            //{
            //    var cursorTile = WorldToGrid(cursor.Position + viewportCursor);
            //    if (!(cursorTile.X > world.Width || cursorTile.Y > world.Height || cursorTile.X < 0 || cursorTile.Y < 0))
            //    {
            //        cubeAtCursor = world.Get(cursorTile.Xi, cursorTile.Yi);
            //        spriteBatch.DrawShadowedString(returnOfGanon, cursor.Position.ToString() + "\n" + cursorTile.X + ", " + cursorTile.Y + (cubeAtCursor != null ? "\n" + cubeAtCursor.Temperature.ToString() : "") + "\n" + densityField.Get(cursorTile.Xi, cursorTile.Yi), new Vec2(cursor.Position.X, cursor.Position.Y + cursor.Size.Y));
            //    }
            //    else 
            //        cubeAtCursor = null;
            //}
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
                    //for (int y = 0; y < worldTileHeight; y++)
                    //    for (int x = 0; x < worldTileWidth; x++)
                    //    {
                    //        Cube t = world.Get(x, y);
                    //    }
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
                    //for (int i = 0; i < worldTileCount; i++)
                    //{
                    //    //Update each object as necessary.. 
                    //    Cube t = world.Array[i];
                    //    lock (t)
                    //    {
                    //        st = new SurroundingTiles(world, t);
                    //        if (updateCount == 1) //First Update Loop
                    //            t.UpdateVisuals(); //Make sure we get a color so if temperatures don't change someplace we're not stuck.
                    //    }
                    //}
                    //Globals.AllGameObjectsLock.EnterReadLock();
                    //foreach (Creature c in Globals.AllGameObjects.OfType<Creature>())
                    //    c.Update();
                    //Globals.AllGameObjectsLock.ExitReadLock();
                }
            }
        }

        public void MouseHandler(GameTime gameTime)
        {
            //Mouse Handling
            previousMouseState = activeMouseState;
            activeMouseState = Mouse.GetState();
            //if (activeMouseState.Position.X < 0 || activeMouseState.Position.Y < 0 || activeMouseState.Position.X > width || activeMouseState.Position.Y > height) //If mouse isn't in window..
            //    return; //Don't handle mouse.
            cursor.Position = new Vec2(activeMouseState.Position.X, activeMouseState.Position.Y);
            if (previousMouseState.LeftButton == ButtonState.Released && activeMouseState.LeftButton == ButtonState.Pressed) //Handle left mouse press when it was previously released (to respond only once to a click).
            {
                foreach (var gameObject in selection)
                {
                    (gameObject.Visual as Sprite).DrawAlternateTint = false;
                    gameObject.Selected = false;
                }
                selection.Clear();
                //selectionBorder.Parent = null;
                //foreach (var gameObject in previousSelection)
                //    if (gameObject.Visual.Children.Contains(selectionBorder))
                //        gameObject.Visual.Children.Remove(selectionBorder);
                //var cursorPos = new Vec2(cursor.Position.Xi + viewportCursor.Xi, cursor.Position.Yi + viewportCursor.Yi);
                //var cursorWorldPos = GridToWorld(cursorPos);
                //MessageBox.Show("Cursor Info",
                //    "Cursor Position: " + cursorPos + "\n" +
                //    "Cursor World Position: " + cursorWorldPos, new string[] { "OK" });
                //var worldPos = GridToWorld(cursorPos);
                //Cube c = Globals.AllGameObjects.OfType<Cube>().Where(cube => cube.BoundingRect.Contains(cursorWorldPos)).FirstOrNull();
                //if (c != null)
                //{
                //    if (c.SelectionEnabled)
                //    {
                //        c.Selected = true;
                //        selection.Add(c);
                //        c.Visual.Children.Add(selectionBorder);
                //        selectionBorder.Parent = c.Visual;
                //    }
                //}
                //var cursorGridPosition = WorldToGrid(cursor.Position + viewportCursor);
                //var c = world.Get(cursorGridPosition.Xi, cursorGridPosition.Yi);
                foreach (var c in Globals.AllGameObjects.OfType<Cube>().Where(x => x.SelectionEnabled).OrderByDescending(x => x.Depth))
                {
                    //var sprite = (Sprite)c.Visual;
                    //var scaledSize = c.Visual.Size * sprite.Scale;
                    //The Y/2 is cube-specific to point at top face instead of actual center.
                    //var cubeExtent = c.Visual.Position + new Vec2(scaledSize.X, scaledSize.Y / 2);
                    //if ((cursor.Position + viewportCursor).Within(c.Visual.Position, cubeExtent))
                    if (c.Coordinates.WithinTopFace(cursor.Position + viewportCursor))
                    {
                        c.Selected = true;
                        ((Sprite)c.Visual).DrawAlternateTint = true;
                        selection.Add(c);
                        //gameObject.Visual.Children.Add(selectionBorder);
                        //selectionBorder.Parent = gameObject.Visual;
                        break; //We only allow one selection (at the moment - nothing breaks or anything).
                    }
                }
            }
            if (previousMouseState.RightButton == ButtonState.Released && activeMouseState.RightButton == ButtonState.Pressed) //Handle right mouse press when it was previously released (to respond only once to a click).
            {
                foreach (var gameObject in selection)
                {
                    gameObject.Selected = false;
                    (gameObject.Visual as Sprite).DrawAlternateTint = false;
                }   
                selection.Clear();
                //foreach (var gameObject in previousSelection)
                //    if (gameObject.Visual.Children.Contains(selectionBorder))
                //        gameObject.Visual.Children.Remove(selectionBorder);
                //selectionBorder.Parent = null;
            }
            if (previousMouseState.MiddleButton == ButtonState.Released && activeMouseState.MiddleButton == ButtonState.Pressed) //Middle mouse button
            {
                if (selection.Count > 0)
                {
                    Cube c = (Cube)selection[0];
                    MessageBox.Show("Cube Stats",
                                    "Position: " + c.Position + "\n" +
                                    "Visual Position: " + c.Visual.Position + "\n" +
                                    "WorldToGrid Position: " + WorldToGrid(c.Position) + "\n" +
                                    "WorldToGrid Visual.Position: " + WorldToGrid(c.Visual.Position) + "\n" +

                        "Cursor In Top Face: " + c.Coordinates.WithinTopFace(cursor.Position)
                                    , new string[] { "OK" });
                }
                else MessageBox.Show("Cube Stats", "No cube selected.", new string[] { "OK" });
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
                //if (viewportCursor.Y + viewportHeight < height)
                    viewportCursor.Y += viewportSpeedY;
            }
            if (dUp && !dDown)
            {
                //if (viewportCursor.Y > 0)
                    viewportCursor.Y -= viewportSpeedY;
            }
            if (dRight && !dLeft)
            {
                //if (viewportCursor.X + viewportWidth < width)
                    viewportCursor.X += viewportSpeedX;
            }
            if (dLeft && !dRight)
            {
                //if (viewportCursor.X > 0)
                    viewportCursor.X -= viewportSpeedX;
            }
            if (ks.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
            {
                paused = !paused;
                if (paused)
                    pauseTime = DateTime.Now;
                else
                    pauseTime = null;
            }
            if (ks.IsKeyDown(Keys.U) && previousKeyboardState.IsKeyUp(Keys.U))
                drawUpdateTiles = !drawUpdateTiles;
            if (ks.IsKeyDown(Keys.T) && previousKeyboardState.IsKeyUp(Keys.T))
                applyTemperatureColor = !applyTemperatureColor;
            if (ks.IsKeyDown(Keys.C) && previousKeyboardState.IsKeyUp(Keys.C))
                cubeCoordinateDebug = !cubeCoordinateDebug;
            if (ks.IsKeyDown(Keys.R) && previousKeyboardState.IsKeyUp(Keys.R)) //Select a random cube that isn't the one that's already selected.
            {
                Cube old = null;
                if (selection.Count > 0)
                {
                    old = (Cube)selection[0];
                    old.Selected = false;
                    (old.Visual as Sprite).DrawAlternateTint = false;
                }
                selection.Clear();
                var c = Globals.AllGameObjects.OfType<Cube>().Where(x => x.SelectionEnabled && (old == null || (old != null && x != old))).Random(1).First();
                c.Selected = true;
                ((Sprite)c.Visual).DrawAlternateTint = true;
                selection.Add(c);
            }

            //if (ks.IsKeyDown(Keys.C) && previousKeyboardState != null && previousKeyboardState.IsKeyUp(Keys.C))
            //{
            //    if (selection.Count == 1)
            //    {
            //        Cube t = selection[0] as Cube;
            //        if (t != null)
            //        {
            //            var s = new Scroble(world, t.Visual.Position, delegate (Cube x) { return x.Material.Phase == Material.MaterialPhase.Solid; });
            //            Globals.AllGameObjectsLock.EnterWriteLock();
            //            Globals.AllGameObjects.Add(s);
            //            Globals.AllGameObjectsLock.ExitWriteLock();
            //            Globals.VisualsLock.EnterWriteLock();
            //            Globals.Visuals.Add(s.Visual);
            //            Globals.VisualsLock.ExitWriteLock();
            //        }
            //    }
            //}
            previousKeyboardState = ks;
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
