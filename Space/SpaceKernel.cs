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
using Charybdis.MonoGame.Framework;
using Rectangle = Charybdis.MonoGame.Rectangle;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Space
{
    public class SpaceKernel : Game
    {
        MouseState previousMouseState;
        MouseState activeMouseState;
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
        Texture2D starTexture;
        Sprite cursor;
        List<Star> stars = new List<Star>();
        Border selectionBorder = new Border { Color = Col3.White };
        Col4 uiPanelColor = new Col4(80, 80, 80);
        Col4 uiBorderColor = Col4.White;
        TextWindow infoWindow;
        List<GameObject2> selection = new List<GameObject2>();
        List<GameObject2> previousSelection = new List<GameObject2>();
        GameObject2 lastClickTarget = null;

        public SpaceKernel()
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
            cursor = new Sprite(cursorTexture);
            infoWindow = new TextWindow("", returnOfGanon, uiBorderColor, uiPanelColor, Col4.White);
            starTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Star32.png");
            Array2<byte> field = new Array2<byte>(Globals.Width, Globals.Height, 0).SimplexNoise(seed);
            //int starCount = 0;
            for (int y = 0; y < Globals.Height; y += starTexture.Width)
            {
                for (int x = 0; x < Globals.Width; x += starTexture.Height)
                {
                    byte val = 0;
                    try
                    {
                        val = field.Get(x, y);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    if (/*starCount < 1 &&*/ val > 175 && random.Chance(.1))
                    {
                        //starCount++;
                        Sprite sprite = new Sprite(starTexture);
                        Star star = new Star(new Vec2(x, y), sprite, random);
                        int orbitalCount = random.Next(15);
                        List<Drawable2> planetVisuals = new List<Drawable2>();
                        for (int i = 0; i < orbitalCount; i++)
                        {
                            var planetIcon = new Rectangle(16, 16) { FillColor = new Col3(80, 80, 80) };
                            planetVisuals.Add(planetIcon);
                            Planet p = new Planet(star, planetIcon, random);
                            star.Orbitals.Add(p);
                            sprite.Children.Add(planetIcon);
                        }
                        sprite.Children = new List<Drawable2>
                        {
                            new Label
                            {
                                Font = returnOfGanon,
                                BindingProperty = typeof(Star).GetProperty("Name"),
                                Parent = star.Visual,
                                DataSource = star,
                                Shadowed = false,
                            }
                        };
                        Globals.AllGameObjectsLock.EnterWriteLock();
                        Globals.AllGameObjects.Add(star);
                        Globals.AllGameObjectsLock.ExitWriteLock();
                    }
                }
            }
            base.LoadContent();
        }

        protected override bool BeginDraw()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            return base.BeginDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            frameCounter++;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.DepthStencilState = depthStencilState;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Globals.AllGameObjectsLock.EnterReadLock();
            foreach (var gameObject in Globals.AllGameObjects)
                gameObject.Visual.Draw(spriteBatch, Vec2.Zero);
            Globals.AllGameObjectsLock.ExitReadLock();
            spriteBatch.DrawShadowedString(returnOfGanon,
                "Charybdis2D Kernel - " + frameRate + "FPS - " + memUsage + "MB" +
                "\nSelection Count: " + selection.Count +
                "", Vec2.Zero);
            if (selection.Count > 0)
            {
                infoWindow.Position = new Vec2(cursor.Position.X + cursor.Size.X + 3, cursor.Position.Y);
                Star s = selection[0] as Star;
                infoWindow.Text =
                    "Star" +
                    "\nName: " + s.Name +
                    "\nClass: " + s.SpectralClass +
                    "\nTemp (K): " + s.Temperature +
                    "\nOrbitals: " + s.Orbitals.Count;
                //    "\nPlanets (" + s.Orbitals.Count + "): ";
                //foreach (Planet p in s.Orbitals.OfType<Planet>())
                //    infoWindow.Text += "\n\n" + p.Name +
                //                       "\n\t- Temp (K): " + p.Temperature +
                //                       "\n\t- Mass (kg): " + p.Mass +
                //                       "\n\t- Radius (km): " + p.Radius;
                infoWindow.Draw(spriteBatch, Vec2.Zero);
            }
            spriteBatch.DrawSprite(cursor); //Draw cursor last so it's always on top.
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
            Globals.AllGameObjectsLock.EnterReadLock();
            foreach (GameObject2 go2 in Globals.AllGameObjects)
            {
                go2.Update();
            }
            Globals.AllGameObjectsLock.ExitReadLock();
            base.Update(gameTime);
        }

        public void MouseHandler(GameTime gameTime)
        {
            //Mouse Handling
            previousMouseState = activeMouseState;
            activeMouseState = Mouse.GetState();
            cursor.Position = new Vec2(activeMouseState.Position.X, activeMouseState.Position.Y);
            if (previousMouseState.LeftButton == ButtonState.Released && activeMouseState.LeftButton == ButtonState.Pressed) //Handle left mouse press when it was previously released (to respond only once to a click).
            {
                //Handle UI First
                //foreach (var x in )
                //If hits UI then don't do the rest, can't select THROUGH the UI.
                previousSelection = selection;
                selection.Clear();
                if (selectionBorder.Parent != null)
                    selectionBorder.Parent.Children.Remove(selectionBorder);
                selectionBorder.Parent = null;
                Globals.AllGameObjectsLock.EnterReadLock();
                foreach (var gameObject in Globals.AllGameObjects.Where(x => x.SelectionEnabled))
                {
                    if (gameObject.BoundingRect.Contains(new Vec2(activeMouseState.Position.X, activeMouseState.Position.Y)))
                    {

                        gameObject.Selected = true;
                        selection.Add(gameObject);
                        gameObject.Visual.Children.Add(selectionBorder);
                        selectionBorder.Parent = gameObject.Visual;
                        if (lastClickTarget == gameObject)
                        {
                            //Was already clicked on last time and is now clicked again, therefore activate the object.
                        }
                        lastClickTarget = gameObject;
                        break; //We only allow one selection (at the moment - nothing breaks or anything).
                    }
                }
                Globals.AllGameObjectsLock.ExitReadLock();
            }
        }

        public void KeyboardHandler(GameTime gameTime)
        {
            //Keyboard Handling
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
                this.Exit();
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
        }

        void Resolve(Sprite a, Sprite b, Vec2 oldPos, int depth = 0)
        {
            if (++depth > 9) return; //abort if too deep
            Vec2 diff = (b.Position - a.Position);
            float adx = (float)Math.Abs(diff.X);
            float ady = (float)Math.Abs(diff.Y);
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
    }
}
