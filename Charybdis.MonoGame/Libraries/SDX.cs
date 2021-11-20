using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Charybdis.MonoGame.Datatypes;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using SharpDX.Toolkit.Input;
using System.Runtime.InteropServices;
using BoundingBox = Charybdis.Datatypes.BoundingBox;
using Charybdis.MonoGame.Framework;
using Model = Charybdis.Datatypes.Model;
using SharpDX.Direct3D11;
/*using Jitter;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;*/


namespace Charybdis.Libraries
{
    public static class SDX
    {
        public static Vector3 ToVector3(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector3 ToVector3(this Vec3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vec3 ToVec3(this Vector3 v)
        {
            return new Vec3(v.X, v.Y, v.Z);
        }

        /*public static JVector ToJVector(this Vector3 vector)
        {
            return new JVector(vector.X, vector.Y, vector.Z);
        }

        public static JVector ToJVector(this Vector4 vector)
        {
            return new JVector(vector.X, vector.Y, vector.Z);
        }

        public static Vector3 ToVector3(this JVector vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }*/

        public static void DrawIndexed(this GraphicsDevice graphicsDevice, Mesh mesh)
        {
            graphicsDevice.SetVertexBuffer(mesh.VertexBuffer);
            graphicsDevice.SetIndexBuffer(mesh.IndexBuffer, true);
            graphicsDevice.SetVertexInputLayout(mesh.VertexInputLayout);
            graphicsDevice.DrawIndexed(PrimitiveType.TriangleList, mesh.IndexBuffer.ElementCount);
        }

        public static SharpDX.Direct3D11.Buffer CreateBuffer<T>(Device device, ResourceUsage resourceUsage, BindFlags bindFlags, params T[] items) where T : struct
        {
            var len = items.Length * Marshal.SizeOf(typeof(T));
            SharpDX.Direct3D11.Buffer buffer;
            DataStream stream;
            switch (resourceUsage)
            {
                case ResourceUsage.Dynamic:
                    buffer = new SharpDX.Direct3D11.Buffer(device, len, resourceUsage, bindFlags, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
                    device.ImmediateContext.MapSubresource(buffer, MapMode.WriteDiscard, MapFlags.None, out stream);
                    WriteToStream(stream, items);
                    device.ImmediateContext.UnmapSubresource(buffer, 0);
                    break;
                default:
                    stream = new DataStream(len, true, true);
                    WriteToStream(stream, items);
                    buffer = new SharpDX.Direct3D11.Buffer(device, stream, len, resourceUsage, bindFlags, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
                    break;
            }
            return buffer;
        }

        public static void WriteToStream<T>(DataStream stream, IEnumerable<T> items) where T : struct
        {
            foreach (T item in items)
            {
                stream.Write(item);
            }
            stream.Position = 0;
        }

        public static void UpdateDynamicBuffer<T>(Device device, SharpDX.Direct3D11.Buffer buffer, IEnumerable<T> items) where T : struct
        {
            DataStream stream;
            device.ImmediateContext.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None, out stream);
            WriteToStream(stream, items);
            device.ImmediateContext.UnmapSubresource(buffer, 0);
            stream.Dispose();
        }

        /// <summary>
        /// Helper Method to get the vertex and index List from the model.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="model"></param>
        /*public static void ExtractData(out List<JVector> vertices, out List<TriangleVertexIndices> indices, Buffer<VertexPositionColor> VertexBuffer, Buffer<int> IndexBuffer, Matrix view)
        {
            vertices = VertexBuffer.GetData<Vector3>().Select(x => Vector3.Transform(x, Matrix.Invert(view)).ToJVector()).ToList();
            short[] s = IndexBuffer.GetData<short>();
            TriangleVertexIndices[] tvi = new TriangleVertexIndices[IndexBuffer.ElementCount / 3];
            for (int i = 0; i != tvi.Length; ++i)
            {
                tvi[i].I0 = s[i * 3 + 0];
                tvi[i].I1 = s[i * 3 + 1];
                tvi[i].I2 = s[i * 3 + 2];
            }
            indices = tvi.ToList();
        }*/

        public static VisualGroup ToCubes(this Cell data)
        {
            VisualGroup vg = new VisualGroup();
            vg.SetPosition(new Vec3(data.Width * data.X, 0, data.Height * data.Y));
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    Globals.CubeModel.Instances.Add(new Instance(new Vec3(x + (data.X * data.Width), value, y + (data.Y * data.Height)), 1f, 0, 0, 0));
                }
            }
            return vg;
        }

        public static VisualGroup ToCubes(this Array2<byte> data)
        {
            VisualGroup vg = new VisualGroup();
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    Cube c = new Cube();
                    c.RandomColor();
                    c.SetPosition(x, value, y);
                    BoundingBox b = new BoundingBox(c.Position, 1f) { Owner = c };
                    Collision.CollisionObjects.Add(b);
                    vg.AddVisual(c);
                }
            }
            return vg;
        }

        public static Mesh ToMesh(this Array2<byte> data, GraphicsDevice gd, string name = "")
        {
            //Make Mesh
            Mesh mesh = new Mesh { Name = name, GraphicsDevice = gd, CollisionBoxes = new List<Charybdis.Datatypes.BoundingBox>() };
            //Make Visual Object
            List<Vector3> vectors = data.ToIndexedVertexData(mesh.GraphicsDevice, out mesh.VertexBuffer, out mesh.IndexBuffer, out mesh.VertexInputLayout);
            //Make Physics Object
            mesh.SetPosition(mesh.Position);
            foreach (Vector3 v in vectors)
            {
                Charybdis.Datatypes.BoundingBox b = new Charybdis.Datatypes.BoundingBox(v.ToVec3(), 1f);
                mesh.CollisionBoxes.Add(b);
                Collision.CollisionObjects.Add(b);
            }
            //Return
            return mesh;
        }

        public static List<Vector3> ToIndexedVertexData(this Array2<byte> data, GraphicsDevice gd, out Buffer<VertexPositionColor> vertexBuffer, out Buffer<int> indexBuffer, out VertexInputLayout inputLayout)
        {
            List<Vector3> vectors = new List<Vector3>();
            int indexCount = (data.Width - 1) * (data.Height - 1) * 6;
            vertexBuffer = Buffer.Vertex.New<VertexPositionColor>(gd, data.Count, global::SharpDX.Direct3D11.ResourceUsage.Dynamic);
            indexBuffer = Buffer.Index.New<int>(gd, indexCount);
            inputLayout = VertexInputLayout.FromBuffer(0, vertexBuffer);
            VertexPositionColor[] vertices = new VertexPositionColor[data.Count];
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    Vector3 pos = Vector3.Transform(new Vector3(x, value, -y), Globals.Camera.Orientation).ToVector3();
                    vectors.Add(pos);
                    vertices[x + y * data.Width].Position = pos;
                    byte r = (value > 150 && value <= 200) ? (byte)180 : (value > 200 ? value : (byte)0);
                    byte g = (value > 150 && value <= 200) ? (byte)180 : (value > 200 ? (byte)255 : (byte)100);
                    byte b = (value <= 90 ? (byte)255 : (value > 150 && value <= 200) ? (byte)180 : (value > 200 ? (byte)255 : (byte)0));
                    vertices[x + y * data.Width].Color = new Color(r, g, b);
                }
            }
            vertexBuffer.SetData(vertices);
            int[] indices = new int[indexCount];
            int counter = 0;
            for (int y = 0; y < data.Height - 1; y++)
            {
                for (int x = 0; x < data.Width - 1; x++)
                {
                    int ll = x + y * data.Width;
                    int lr = (x + 1) + y * data.Width;
                    int tl = x + (y + 1) * data.Width;
                    int tr = (x + 1) + (y + 1) * data.Width;

                    indices[counter++] = tl;
                    indices[counter++] = lr;
                    indices[counter++] = ll;

                    indices[counter++] = tl;
                    indices[counter++] = tr;
                    indices[counter++] = lr;
                }
            }
            indexBuffer.SetData(indices);
            return vectors;
        }
    }
}
