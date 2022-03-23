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
        double dayProgress = 0;
        static int minutesPerDay = 3;
        static int secondsPerDay = minutesPerDay * 60;
        static int tileSize = 32;
        static int worldTileWidth = 320;
        static int worldTileHeight = 240;
        static int worldTileCount = worldTileWidth * worldTileHeight;
        static int width = worldTileWidth * tileSize; 
        static int height = worldTileHeight * tileSize;
        static int size = width * height;
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
        SpriteBatch cubeTopSpriteBatch;
        SpriteBatch cubeLeftSpriteBatch;
        SpriteBatch cubeRightSpriteBatch;
        double seed;
        Font returnOfGanon;
        Font smallFonts;
        int frameRate = 0;
        int frameCounter = 0;
        double memUsage = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        Texture2D tileTexture;
        Sprite cursor;
        Border selectionBorder = new Border { Color = Col3.White };
        Col4 uiPanelColor = new Col4(80, 80, 80);
        Col4 uiBorderColor = Col4.White;
        TextWindow infoWindow;
        List<KolonyObject> selection = new List<KolonyObject>();
        List<KolonyObject> previousSelection = new List<KolonyObject>();
        Array2<Cube> world;
        DateTime lastUpdate = DateTime.Now;
        double lastSecondsSinceLastUpdate = 0;
        long updateCount = 0;
        bool paused = false;
        DateTime? pauseTime = null;
        Array2<float> densityField;
        bool drawUpdateTiles = false;
        bool applyTemperatureColor = false;

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
            cubeTopSpriteBatch = new SpriteBatch(GraphicsDevice);
            cubeLeftSpriteBatch = new SpriteBatch(GraphicsDevice);
            cubeRightSpriteBatch = new SpriteBatch(GraphicsDevice);
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

            SurroundingTiles.TileSize = tileSize;
            SurroundingTiles.WorldTileWidth = worldTileWidth;
            SurroundingTiles.WorldTileHeight = worldTileHeight;

            //Console.Write("Generating Temperature Field..");
            //Array2<float> temperatureField = new Array2<float>(worldTileWidth, worldTileHeight,0).NormalizedNoise(seed, Noise.DefaultNoiseArgs);
            //Console.Write("Done!");
            //Console.WriteLine();
            Console.Write("Generating Matter Field..");
            Array2<float> matterField = new Array2<float>(worldTileWidth, worldTileHeight, 0).NormalizedNoise(seed + 2, Noise.DefaultNoiseArgs);
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Generating Density Field..");
            densityField = new Array2<float>(worldTileWidth, worldTileHeight, 0).NormalizedNoise(seed + 3, Noise.DefaultNoiseArgs);
            world = new Array2<Cube>(worldTileWidth, worldTileHeight);
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Setting Up Textures From \"" + Content.RootDirectory + "/Textures\"..");
            tileTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Tile32.png");
            //gasTileTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/GasTile32.png");
            //geyserTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Geyser32.png");
            //Scroble.Texture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Scroble.png");
            //xSprite = new Sprite(GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/X32.png"));
            //Material.Gas.Texture = gasTileTexture;
            //Material.Liquid.Texture = gasTileTexture;
            //Material.Permanent.Texture = geyserTexture;
            Material.Stone.TopTexture = tileTexture;
            Material.Stone.SideTexture = tileTexture;
            //Material.Vacuum.Texture = geyserTexture;
            //Material.Water.Texture = gasTileTexture;
            //Material.WaterIce.Texture = tileTexture;
            //Material.Steam.Texture = gasTileTexture;
            //Material.Dioxygen.Texture = gasTileTexture;
            //Material.Hydrogen.Texture = gasTileTexture;
            //Material.LiquidHydrogen.Texture = gasTileTexture;
            //Material.LiquidOxygen.Texture = gasTileTexture;
            //Material.SolidHydrogen.Texture = tileTexture;
            //Material.SolidOxygen.Texture = tileTexture;
            Console.Write("Done!");
            Console.WriteLine();
            Console.Write("Spawning Tiles..");
            //for (int y = 0; y < worldTileHeight; y++)
            //{
            //    for (int x = 0; x < worldTileWidth; x++)
            //    {
            //        //double temperature = ((double)(temperatureField.Get(x, y) * 100)).Round();
            //        //double density = Math.Exp(densityField.Get(x, y) / 2);
            //        Cube tile = null;
            //        //Tile gen logic by density would go here..
            //        tile = new Cube()
            //        {
            //            Material = Material.Stone,
            //            Visual = new SpriteGroup 
            //            { 
            //                new Sprite(tileTexture) { Position = new Vec2(x * tileSize, y * tileSize - (tileSize / 3)), Tint = Col3.White },
            //                new Sprite(tileTexture) { Position = new Vec2(x * tileSize - (tileSize / 3), y * tileSize + (tileSize / 3)), Tint = Col3.Blue },
            //                new Sprite(tileTexture) { Position = new Vec2(x * tileSize + (tileSize / 3), y * tileSize + (tileSize / 3)), Tint = Col3.Red },
            //            },
            //            Random = random,
            //            Temperature = new TemperatureF(60),
            //            Mass = ((double)(matterField.Get(x, y) * 1000)).Round(),
            //        };
            //        world.Put(x, y, tile);
            //        Globals.AllGameObjects.Add(tile);
            //    }
            //}
            int x = 10;
            int y = 10;
            Cube tile = new Cube()
            {
                Material = Material.Stone,
                Visual = new SpriteGroup
                        {
                            new Sprite(tileTexture) { Position = new Vec2(x * tileSize, y * tileSize), Tint = Col3.White },
                            new Sprite(tileTexture) { Position = new Vec2(x * tileSize, y * tileSize), Tint = Col3.Blue },
                            new Sprite(tileTexture) { Position = new Vec2(x * tileSize, y * tileSize), Tint = Col3.Red },
                        },
                Random = random,
                Temperature = new TemperatureF(60),
                Mass = ((double)(matterField.Get(x, y) * 1000)).Round(),
            };
            world.Put(x, y, tile);
            Globals.AllGameObjects.Add(tile);
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
            cubeTopSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: Matrix.CreateRotationZ(0.78539816f) * Matrix.CreateScale(new Vector3(1.1f, .9f, 1f)));
            cubeLeftSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: Matrix.CreateRotationZ(0.78539816f) * Matrix.CreateScale(new Vector3(.75f, 1.1f, 1f)));
            cubeRightSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: Matrix.CreateRotationZ(-0.78539816f) * Matrix.CreateScale(new Vector3(.75f, 1f, 1f)));
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

            foreach (var go2 in Globals.AllGameObjects)
                if (go2.Visual != null && go2.Visual.Position.Within(viewportCursor - tileSize, viewportMax))
                {
                    if (go2 is Cube cube)
                    {
                        var spriteGroup = cube.Visual as SpriteGroup;
                        spriteGroup[0].Draw(cubeTopSpriteBatch, viewportCursor);
                        spriteGroup[1].Draw(cubeLeftSpriteBatch, viewportCursor);
                        spriteGroup[2].Draw(cubeRightSpriteBatch, viewportCursor);
                    }
                    else
                        go2.Visual.Draw(spriteBatch, viewportCursor);
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
                Cube t = selection[0] as Cube;
                if (t != null)
                {
                    var tilePosition = t.Visual.Position / tileSize;
                    infoWindow.Text =
                        "Material: " + t.Material.Name + "\n" +
                        "Position: " + tilePosition + "\n" +
                        "Mass (Kg): " + t.Mass + "\n" +
                        "Temp (F): " + t.Temperature;
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
                    Cube t = world.Get(cursorTileX, cursorTileY);
                    spriteBatch.DrawShadowedString(returnOfGanon, cursor.Position.ToString() + "\n" + cursorTileX + ", " + cursorTileY + (t != null ? "\n" + t.Temperature.ToString() : "") + "\n" + densityField.Get(cursorTileX, cursorTileY), new Vec2(cursor.Position.X, cursor.Position.Y + tileSize));
                }
            }
            base.Draw(gameTime);
        }

        protected override void EndDraw()
        {
            cubeRightSpriteBatch.End();
            cubeLeftSpriteBatch.End();
            cubeTopSpriteBatch.End();
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
                Cube t = world.Get((cursor.Position.Xi + viewportCursor.Xi) / tileSize, (cursor.Position.Yi + viewportCursor.Yi) / tileSize);
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
