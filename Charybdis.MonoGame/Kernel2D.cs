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
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Charybdis.MonoGame
{
    public class Kernel2D : Game
    {
        string display = "";
        DateTime executionStart = DateTime.Now;
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
        ulong updates = 0;
        bool render = true;
        Vec2 graphOffset = new Vec2(250, 0);
        Vec2 labelOffset = new Vec2(-500, 0);

        public Kernel2D() : base()
        {
            random = new Random();
            Window.Title = Console.Title = "Charybdis Kernel (2D)";
            Window.IsBorderless = true;
            gdm = new GraphicsDeviceManager(this);
            seed = random.NextDouble();
            Content.RootDirectory = "Data";
            gdm.PreferredBackBufferWidth = Globals.Width;
            gdm.PreferredBackBufferHeight = Globals.Height;
            IsMouseVisible = false;
            IsFixedTimeStep = Globals.DecoupleSimulationFromVisuals;
        }

        protected override void Initialize()
        {
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

            #endregion

            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.SustainedLowLatency;

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

                foreach (var id2 in Globals.Visuals)
                    id2.Draw(spriteBatch, Vec2.Zero);

                Globals.VisualsLock.ExitReadLock();
            }
            else
                spriteBatch.DrawShadowedString(font1, "RENDERING DISABLED FOR PERFORMANCE", new Vec2(Globals.Width / 2, Globals.Height / 2) - (font1.CalculateTextSize("RENDERING DISABLED FOR PERFORMANCE") / 2));

            display =
                "Charybdis2D Kernel - " + frameRate + "FPS - " + Globals.MemoryUsed + "MB" +
                "\nRuntime: " + (DateTime.Now - executionStart).TotalMinutes.Round(places: 0) + " Minutes" +
                "";

            spriteBatch.DrawShadowedString(font1, display, Vec2.Zero);
            if (paused)
                spriteBatch.DrawShadowedString(font1, "PAUSED", new Vec2(Globals.Width / 2, Globals.Height / 2) - (font1.CalculateTextSize("PAUSED") / 2));
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
            if (elapsedTime > sec1)
            {
                elapsedTime -= sec1;
                frameRate = frameCounter;
                frameCounter = 0;
                Globals.MemoryUsed = (int)Math.Round((double)GC.GetTotalMemory(false) / 1024 / 1024, 0);
            }
            elapsedTime5 += gameTime.ElapsedGameTime;
            if (elapsedTime5 > sec5)
            {
                elapsedTime5 -= sec5;
            }
            KeyboardHandler(gameTime);
            MouseHandler(gameTime);
            if (paused)
            {
                base.Update(gameTime);
                return;
            }

            base.Update(gameTime);
        }

        protected virtual void LeftClickHandler(Vec2 cursorPosition)
        {

        }

        protected virtual void RightClickHandler(Vec2 cursorPosition)
        {

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
                LeftClickHandler(cursor.Position);
            }
            if (previousMouseState.RightButton == ButtonState.Released && activeMouseState.RightButton == ButtonState.Pressed) //Handle right mouse press when it was previously released (to respond only once to a click).
            {
                //Right Click
                RightClickHandler(cursor.Position);
            }
        }

        protected Dictionary<Keys, Action<GameTime>> keyHandlers = new Dictionary<Keys, Action<GameTime>>();

        public void KeyboardHandler(GameTime gameTime)
        {
            //Keyboard Handling
            KeyboardState ks = Keyboard.GetState();
            foreach (Keys k in ks.GetPressedKeys())
            {
                if (keyHandlers.ContainsKey(k))
                    keyHandlers[k](gameTime);
            }
        }
    }
}
