using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.MonoGame.Datatypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Charybdis.MonoGame.Framework
{
    public class Scene
    {
        public GraphicsDevice GraphicsDevice { get; private set; }
        public Camera Camera { get; private set; }
        public World World { get; set; }
        public int ChunkViewDistance = 4;
        public bool Menu = false;

        public Scene(GraphicsDevice device, Camera camera)
        {
            GraphicsDevice = device;
            Camera = camera;
        }

        public void Draw(Chunk.Layer layer, BoundingFrustum clipFrustum, Vec3 clipPosition, GameTime gameTime = null)
        {
            if (!Menu) World.UpdateLoadedChunks(Camera.Position, ChunkViewDistance);
            List<Chunk> tmpChunks = World.LoadedChunks.OrderByDescending(c => c.Position.DistanceXZ(clipPosition)).ToList();
            foreach (Chunk c in tmpChunks)
            {
                //if (clipFrustum.Contains(c.BoundingBox) != ContainmentType.Disjoint)
                //{
                    if (c.Dirty) c.Build();
                    if (layer == Chunk.Layer.Opaque)
                    {
                        if (c.VertexBuffer.VertexCount > 0)
                        {
                            Globals.ChunkRenderCount++;
                            lock (c.VertexBuffer)
                            {
                                GraphicsDevice.SetVertexBuffer(c.VertexBuffer);
                                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, c.VertexBuffer.VertexCount / 3);
                            }
                        }
                    }
                    else if (layer == Chunk.Layer.Transparent)
                    {
                        if (c.TransparencyVertexBuffer.VertexCount > 0)
                        {
                            lock (c.TransparencyVertexBuffer)
                            {
                                GraphicsDevice.SetVertexBuffer(c.TransparencyVertexBuffer);
                                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, c.TransparencyVertexBuffer.VertexCount / 3);
                            }
                        }
                    }
                    else if (layer == Chunk.Layer.Liquid)
                    {
                        if (c.LiquidVertexBuffer.VertexCount > 0)
                        {
                            lock (c.LiquidVertexBuffer)
                            {
                                GraphicsDevice.SetVertexBuffer(c.LiquidVertexBuffer);
                                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, c.LiquidVertexBuffer.VertexCount / 3);
                            }
                        }
                    }
                //}
            }
        }
    }
}
