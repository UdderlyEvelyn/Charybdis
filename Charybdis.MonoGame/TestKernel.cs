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

//namespace Charybdis.MonoGame
//{
//    public class TestKernel : Game
//    {
//        MouseState previousMouseState;
//        Random random;
//        GraphicsDeviceManager gdm;
//        RasterizerState rasterizerState;
//        DepthStencilState depthStencilState;
//        SpriteBatch spriteBatch;
//        //double seed;
//        Font consolas = new Font("Consolas");
//        int frameRate = 0;
//        int frameCounter = 0;
//        int memUsage = 0;
//        TimeSpan elapsedTime = TimeSpan.Zero;
//        Vec3 movement = Vec3.Zero;
//        float POS89RADS;
//        float NEG89RADS;
//        Vec2 normal = Vec2.Zero;
//        Sprite sprite;
//        Texture2D wallTexture;
//        List<Sprite> sprites = new List<Sprite>();
//        Vec2 velocity = Vec2.Zero;
//        Texture2D redBox;
//        int triX = 0;
//        int triY = 0;
        
//        public TestKernel()
//        {
//            random = new Random();
//            Window.Title = Console.Title = "Charybdis Kernel (2D)"; 
//            gdm = new GraphicsDeviceManager(this);
//            //seed = random.NextDouble();
//            Content.RootDirectory = "Data";
//            gdm.PreferredBackBufferWidth = Globals.Width;
//            gdm.PreferredBackBufferHeight = Globals.Height;
//            IsMouseVisible = false;
//            IsFixedTimeStep = true;
//        }

//        protected override void Initialize()
//        {
//            POS89RADS = MathHelper.ToRadians(89);
//            NEG89RADS = MathHelper.ToRadians(-89);
//            Globals.GraphicsDevice = GraphicsDevice;
//            spriteBatch = new SpriteBatch(GraphicsDevice);
//            rasterizerState = new RasterizerState { CullMode = CullMode.CullClockwiseFace, FillMode = FillMode.Solid };
//            depthStencilState = new DepthStencilState { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less };
//            base.Initialize();
//        }

//        protected override void LoadContent()
//        {
//            consolas.Texture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Fonts/Consolas36.png");
//            consolas.LoadMetrics(Content.RootDirectory + "/Fonts/Consolas36.xml");
//            consolas.Scale = .45f;
//            sprite = new Sprite();
//            sprite.Data = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Sponge.png");
//            sprite.Position = new Vec2(384, 256);
//            sprite.Scale = 2;
//            sprites.Add(sprite);
//            sprite.GenerateBoundingRect();
//            wallTexture = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Stone.png");
//            //Noise.NoiseArgs noiseArgs = Noise.DefaultNoiseArgs;
//            Array2<byte> maze = new Array2<byte>(Globals.Width, Globals.Height, 0).SimplexNoise(random.NextDouble());
//            for (int y = 0; y < Globals.Height; y += 33)
//            {
//                for (int x = 0; x < Globals.Width; x += 33)
//                {
//                    byte val = 0;
//                    try
//                    {
//                        val = maze.Get(x, y);
//                    }
//                    catch { /*Pass*/ }
//                    if (val >= 100)
//                    //if (random.Next(0, 3) == 0)
//                    {
//                        if (x > 350 && x < 450 && y > 230 && y < 330) continue; //skip start position of player sprite
//                        Sprite s = new Sprite { Data = wallTexture };
//                        s.Scale = 2;
//                        s.Position = new Vec2(x, y);
//                        sprites.Add(s);
//                    }                    
//                }
//            }
//            redBox = GraphicsDevice.Texture2DFromFile(Content.RootDirectory + "/Textures/Red.png");
//            base.LoadContent();
//        }

//        protected override bool BeginDraw()
//        {
//            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
//            return base.BeginDraw();
//        }

//        protected override void Draw(GameTime gameTime)
//        {
//            frameCounter++;
//            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
//            GraphicsDevice.RasterizerState = rasterizerState;
//            GraphicsDevice.DepthStencilState = depthStencilState;
//            GraphicsDevice.BlendState = BlendState.Opaque;
//            foreach (Sprite s in sprites) spriteBatch.DrawSprite(s);
//            spriteBatch.Draw(redBox, new Microsoft.Xna.Framework.Rectangle(triX, triY, 2, 2), Color.White);
//            spriteBatch.DrawShadowedString(consolas, 
//                "Charybdis2D Kernel - " + frameRate + "FPS - " + memUsage + "MB" +
//                "\nPos: " + sprite.Position +
//                "", Vec2.Zero);
//            base.Draw(gameTime);
//        }

//        protected override void EndDraw()
//        {
//            spriteBatch.End();
//            base.EndDraw();
//        }

//        protected override void Update(GameTime gameTime)
//        {
//            elapsedTime += gameTime.ElapsedGameTime;
//            if (elapsedTime > TimeSpan.FromSeconds(1))
//            {
//                elapsedTime -= TimeSpan.FromSeconds(1);
//                frameRate = frameCounter;
//                frameCounter = 0;
//            }
//            memUsage = (int)Math.Ceiling((double)GC.GetTotalMemory(false) / 1024 / 1024);
//            KeyboardHandler(gameTime);
//            /*Vec3 old = camera.Position;
//            Vec3 move = camera.PreviewMove(camera.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * camera.MoveSpeed);
//            if (!scene.BlockAt(new Vec3(move.X, move.Y - 1, move.Z)))
//            {
//                camera.Velocity.Y += -.1f;
//            }
//            if (scene.BlockAt(move))
//            {
//                Vec3 n = Vec3.Zero;
//                float xEntry = 0;
//                float zEntry = 0;
//                if (camera.Velocity.X > 0)
//                {
//                    xEntry = move.X - (camera.Position.X + 1);
//                }
//                else
//                {
//                    xEntry = (move.X + 1) - camera.Position.X;
//                }
//                if (camera.Velocity.Z > 0)
//                {
//                    zEntry = move.Z - (camera.Position.Z + 1);
//                }
//                else
//                {
//                    zEntry = (move.Z + 1) - camera.Position.Z;
//                }
//                if (xEntry > zEntry)
//                {
//                    if (xEntry < 0)
//                    {
//                        n.X = 1;
//                        n.Z = 0;
//                    }
//                    else
//                    {
//                        n.Z = -1;
//                        n.Z = 0;
//                    }
//                }
//                else
//                {
//                    if (zEntry < 0)
//                    {
//                        n.X = 0;
//                        n.Z = 1;
//                    }
//                    else
//                    {
//                        n.X = 0;
//                        n.Z = -1;
//                    }
//                }
//                normal = new Vec2(n.X, n.Z);
//                float mag = (float)Math.Sqrt(camera.Velocity.X * camera.Velocity.X + camera.Velocity.Z * camera.Velocity.Z);
//                float dot = camera.Velocity.X * n.Z + camera.Velocity.Z * n.X;
//                if (dot > 0) dot = 1;
//                else if (dot < 0) dot = -1;
//                camera.Velocity = new Vec3(dot * n.Y * mag, 0, dot * n.X * mag);
//            }
//            camera.Move(camera.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * camera.MoveSpeed);
//            camera.Velocity = Vec3.Zero;

//            if (!scene.Chunks.Any(c => c.InRange((int)Math.Floor(camera.Position.X), (int)Math.Floor(camera.Position.Y), (int)Math.Floor(camera.Position.Z)))) camera.SetPosition(old, camera.Rotation);*/
//            MouseHandler(gameTime);
//            base.Update(gameTime);
//        }

//        public void MouseHandler(GameTime gameTime)
//        {
//            //Mouse Handling
//            MouseState ms = Mouse.GetState();
//            if (ms != previousMouseState)
//            {
//                previousMouseState = ms;
//            }
//        }

//        public void KeyboardHandler(GameTime gameTime)
//        {
//            //Keyboard Handling
//            KeyboardState ks = Keyboard.GetState();
//            if (ks.IsKeyDown(Keys.Escape))
//                this.Exit();
//            if (ks.IsKeyDown(Keys.W))
//                velocity.Y = -1;
//            if (ks.IsKeyDown(Keys.S))
//                velocity.Y = 1;
//            if (ks.IsKeyDown(Keys.A))
//                velocity.X = -1;
//            if (ks.IsKeyDown(Keys.D))
//                velocity.X = 1;
//            sprite.GenerateBoundingRect();
//            if (ks.IsKeyDown(Keys.G))
//                GC.Collect();
//            if (ks.IsKeyDown(Keys.Home))
//                if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
//            Keys[] pressedKeys = ks.GetPressedKeys();
//            if (ks.IsKeyDown(Keys.OemTilde))
//            {
//                if (ks.IsKeyDown(Keys.LeftShift)) rasterizerState.FillMode = FillMode.Solid;
//                else rasterizerState.FillMode = FillMode.WireFrame;
//            }

//            if (velocity != Vec2.Zero)
//            {
//                Vec2 oldPos = sprite.Position;
//                sprite.Position = sprite.Position + velocity;
//                foreach (Sprite s in sprites.Except(new List<Sprite> { sprite }))
//                {
//                    #region subtriangle detection (commented out)
//                    //n++;
//                    /*if (sprite.BoundingRect.Intersects(s.BoundingRect))
//                    {
//                        float minX = s.BoundingRect.Min.X;
//                        float minY = s.BoundingRect.Min.Y;
//                        float maxX = s.BoundingRect.Max.X;
//                        float maxY = s.BoundingRect.Max.Y;
//                        Vec2 center = new Vec2(sprite.Position.X + sprite.Data.Width / 2, sprite.Position.Y + sprite.Data.Height / 2);
//                        Vec2 TL = new Vec2(minX, minY);
//                        Vec2 TR = new Vec2(maxX, minY);
//                        Vec2 BL = new Vec2(minX, maxY);
//                        Vec2 BR = new Vec2(maxX, maxY);
//                        float midY = minY + (maxY - minY) / 2;
//                        float midX = minX + (maxX - minX) / 2;
//                        Vec2 C = new Vec2(midX, midY);
//                        Vec2 TM = new Vec2(midX, minY);
//                        Vec2 BM = new Vec2(midX, maxY);
//                        Vec2 LM = new Vec2(minX, midY);
//                        Vec2 RM = new Vec2(maxX, midY);
//                        if (center.X > minX && center.X < midX)
//                        {
//                            //left half
//                            if (center.Y > minY && center.Y < midY)
//                            {
//                                //top left
//                                if (Vec2InTriangle(center, C, TM, TL))
//                                {
//                                    //upper triangle
//                                    Vec2 tri = GetCentroid(new List<Vec2> { C, TM, TL });
//                                    triX = (int)tri.X;
//                                    triY = (int)tri.Y;
//                                }
//                                else
//                                {
//                                    //lower triangle
//                                    Vec2 tri = GetCentroid(new List<Vec2> { C, LM, TL });
//                                    triX = (int)tri.X;
//                                    triY = (int)tri.Y;
//                                }
//                            }
//                            else
//                            {
//                                //bottom left                                
//                                if (Vec2InTriangle(center, C, BM, BL))
//                                {
//                                    //lower triangle
//                                    Vec2 tri = GetCentroid(new List<Vec2> { C, BM, BL });
//                                    triX = (int)tri.X;
//                                    triY = (int)tri.Y;
//                                }
//                                else
//                                {
//                                    //upper triangle
//                                    Vec2 tri = GetCentroid(new List<Vec2> { C, LM, BL });
//                                    triX = (int)tri.X;
//                                    triY = (int)tri.Y;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            //right half
//                            if (center.Y > minY && center.Y < midY)
//                            {
//                                //top right
//                                if (Vec2InTriangle(center, C, TM, TR))
//                                {
//                                    //upper triangle
//                                    Vec2 tri = GetCentroid(new List<Vec2> { C, TM, TR });
//                                    triX = (int)tri.X;
//                                    triY = (int)tri.Y;
//                                }
//                                else
//                                {
//                                    //lower triangle
//                                    Vec2 tri = GetCentroid(new List<Vec2> { C, RM, TR });
//                                    triX = (int)tri.X;
//                                    triY = (int)tri.Y;
//                                }
//                            }
//                            else
//                            {
//                                //bottom right
//                                if (Vec2InTriangle(center, C, BM, BR))
//                                {
//                                    //lower triangle
//                                    Vec2 tri = GetCentroid(new List<Vec2> { C, BM, BR });
//                                    triX = (int)tri.X;
//                                    triY = (int)tri.Y;
//                                }
//                                else
//                                {
//                                    //upper triangle
//                                    Vec2 tri = GetCentroid(new List<Vec2> { C, RM, BR });
//                                    triX = (int)tri.X;
//                                    triY = (int)tri.Y;
//                                }
//                            }
//                        }

//                    }*/
//                    //tw.Flush();
//                    #endregion
//                    Resolve(sprite, s, oldPos);
//                    velocity = Vec2.Zero;
//                }
//            }
//        }

//        void Resolve(Sprite a, Sprite b, Vec2 oldPos, int depth = 0)
//        {
//            if (++depth > 9) return; //abort if too deep
//            Vec2 diff = (b.Position - a.Position);
//            float adx = (float)Math.Abs(diff.X);
//            float ady = (float)Math.Abs(diff.Y);
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
