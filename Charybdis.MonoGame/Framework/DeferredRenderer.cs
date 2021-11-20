using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;

namespace Charybdis.MonoGame
{
    public static class DeferredRenderer
    {
        public static GraphicsDevice GraphicsDevice;
        public static RenderTarget2D ColorTarget;
        public static RenderTarget2D NormalTarget;
        public static RenderTarget2D DepthTarget;
        public static RenderTarget2D LightTarget;
        public static RenderTarget2D RenderTarget;
        public static SamplerState Sampler = new SamplerState
        {
            Filter = TextureFilter.Point,
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp
        };

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            ColorTarget = new RenderTarget2D(graphicsDevice, graphicsDevice.DisplayMode.Width, graphicsDevice.DisplayMode.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            NormalTarget = new RenderTarget2D(graphicsDevice, graphicsDevice.DisplayMode.Width, graphicsDevice.DisplayMode.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            DepthTarget = new RenderTarget2D(graphicsDevice, graphicsDevice.DisplayMode.Width, graphicsDevice.DisplayMode.Height, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
            LightTarget = new RenderTarget2D(graphicsDevice, graphicsDevice.DisplayMode.Width, graphicsDevice.DisplayMode.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            RenderTarget = new RenderTarget2D(graphicsDevice, graphicsDevice.DisplayMode.Width, graphicsDevice.DisplayMode.Height, false, SurfaceFormat.Color, DepthFormat.None);
        }

        #region Vertices

        public static TextureVertex[] Vertices = new TextureVertex[6]
                {
                    new TextureVertex(Vec3.NPZ, Vec3.PNN, Vec2.PP), //Z
                    new TextureVertex(Vec3.PNZ, Vec3.NPN, Vec2.ZZ), //X
                    new TextureVertex(Vec3.NNZ, Vec3.PPN, Vec2.PZ), //Y
                    new TextureVertex(Vec3.NPZ, Vec3.PNN, Vec2.PP), //Z
                    new TextureVertex(Vec3.PPZ, Vec3.NNN, Vec2.ZP), //W
                    new TextureVertex(Vec3.PNZ, Vec3.NPN, Vec2.ZZ), //X
                };

        #endregion

        public static void PreDraw()
        {
            GraphicsDevice.Textures[0] = RenderTarget;
            GraphicsDevice.SamplerStates[0] = Sampler;
        }

        //public static void Draw()
        //{
        //    GraphicsDevice.DrawUserPrimitives<TextureVertex>(PrimitiveType.TriangleList, Vertices, 0, 2);
        //}
    }
}
