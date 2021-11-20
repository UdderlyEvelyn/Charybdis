using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.MonoGame.Datatypes;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Charybdis.MonoGame.Framework;
using Charybdis.Libraries;

namespace Charybdis.Libraries
{
    public static class OGL
    {
        //In the old TerrainGenerator, each array cell translates to two triangles and a heightmap point, 
        //with the triangles morphing to create the relevant height, and the scale controlling how big the triangles were/how far apart the points were.
        //Two large triangles are added to form a "base" for the mesh to avoid wrap-around of points.
        //
        //In this system, each array cell translates to a point directly, and triangles are formed from actual points.
        //
        //Going to rework the new to work like the old did, because that offered more control and easier hit detection.
        /*public static Mesh ToIndexedMesh(this Array2<byte> data, string name = "")
        {
            //Make Mesh
            Mesh mesh = new Mesh { Name = name, CollisionBoxes = new List<Box>() };
            //Make Visual Object
            Vec3[] vertices;
            uint[] indices;
            List<Vec3> vectors = data.ToIndexedVertexData(out vertices, out indices);
            mesh.Vertices = vertices;
            mesh.Indices = indices;
            //Make Physics Object
            mesh.SetPosition(mesh.Position);
            foreach (Vec3 v in vectors)
            {
                Box b = new Box(v, 1f);
                mesh.CollisionBoxes.Add(b);
                Collision.CollisionObjects.Add(b);
            }
            //Return
            return mesh;
        }*/

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

        public static VisualGroup ToCubes(this Cell data)
        {
            VisualGroup vg = new VisualGroup();
            vg.SetPosition(new Vec3(data.Width * data.X, 0, data.Height * data.Y));
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    Cube c = new Cube();
                    c.SetPosition(x + (data.X * data.Width), value, y + (data.Y * data.Height));
                    BoundingBox b = new BoundingBox(c.Position, 1f) { Owner = c };
                    Collision.CollisionObjects.Add(b);
                    vg.AddVisual(c);
                }
            }
            return vg;
        }

        public static TexturedMesh ToTexturedMesh(this Array2<byte> data, string name = "")
        {
            //Make Texture
            string texturePath = "Data/Texture-" + (name == "" ? "TEMP-" + DateTime.Now.Ticks : name) + ".png";
            data.SaveTexture(texturePath);
            //Make Mesh
            TexturedMesh mesh = new TexturedMesh { Name = name, CollisionBoxes = new List<BoundingBox>(), Texture = new Texture(texturePath), Width = data.Width, Height = data.Height };
            //Make Visual Object
            Vec3[] vertices;
            Vec2[] texcoords;
            uint[] indices;
            List<Vec3> vectors = data.ToTexturedIndexedVertexData(out vertices, out texcoords, out indices);
            mesh.Vertices = vertices.ToList();
            mesh.TextureCoordinates = texcoords;
            mesh.Indices = indices.ToList();
            //Make Physics Object
            mesh.SetPosition(new Vec3(0, 0, 0));
            foreach (Vec3 v in vectors)
            {
                BoundingBox b = new BoundingBox(v, 1f) { Owner = mesh };
                mesh.CollisionBoxes.Add(b);
                Collision.CollisionObjects.Add(b);
            }
            //Return
            return mesh;
        }

        public static TexturedMesh ToTexturedMesh(this Cell data, string name = "")
        {
            TexturedMesh tm = (data as Array2<byte>).ToTexturedMesh(name);
            tm.SetPosition(new Vec3(data.X * data.Width, 0, data.Y * data.Height));
            return tm;
        }

        public static Mesh ToMesh(this Array2<byte> data, string name = "")
        {
            //Make Mesh
            Mesh mesh = new Mesh { Name = name, CollisionBoxes = new List<BoundingBox>(), Width = data.Width, Height = data.Height };
            //Make Visual Object
            Vec3[] vertices;
            uint[] indices;
            List<Vec3> vectors = data.ToIndexedVertexData(out vertices, out indices);
            mesh.Vertices = vertices.ToList();
            mesh.Indices = indices.ToList(); 
            //Make Physics Object
            mesh.SetPosition(new Vec3(0, 0, 0));
            foreach (Vec3 v in vectors)
            {
                BoundingBox b = new BoundingBox(v, 1f) { Owner = mesh };
                mesh.CollisionBoxes.Add(b);
                Collision.CollisionObjects.Add(b);
            }
            //Return
            return mesh;
        }

        public static List<Vec3> ToIndexedVertexData(this Array2<byte> data, out Vec3[] vertices, out uint[] indices)
        {
            List<Vec3> vectors = new List<Vec3>();
            int indexCount = (data.Width - 1) * (data.Height - 1) * 6;
            vertices = new Vec3[data.Count];
            indices = new uint[indexCount];
            //colors = new float[data.Count * 3];
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    Vec3 pos = new Vec3(x, value, y);//Vec3.Transform(new Vector3(x, value, -y), Globals.Camera.Orientation).ToVector3();
                    vectors.Add(pos);
                    vertices[x + y * data.Width] = pos; 
                    /*value = Maths.Clamp(value, (byte)0, (byte)255);
                    int r = (value > 150 && value <= 200) ? 180 : (value > 200 ? value : 0);
                    int g = (value > 150 && value <= 200) ? 180 : (value > 200 ? 255 : 100);
                    int b = (value <= 90 ? 255 : (value > 150 && value <= 200) ? 180 : (value > 200 ? 255 : 0));
                    colors[x + y * data.Width] = (float)r;
                    colors[x + y * data.Width + 1] = (float)g;
                    colors[x + y * data.Width + 2] = (float)b;*/
                }
            }

            uint counter = 0;
            for (uint y = 0; y < data.Height - 1; y++)
            {
                for (uint x = 0; x < data.Width - 1; x++)
                {
                    uint ll = (uint)(x + y * data.Width);
                    uint lr = (uint)((x + 1) + y * data.Width);
                    uint tl = (uint)(x + (y + 1) * data.Width);
                    uint tr = (uint)((x + 1) + (y + 1) * data.Width);

                    indices[counter++] = tl;
                    indices[counter++] = lr;
                    indices[counter++] = ll;

                    indices[counter++] = tl;
                    indices[counter++] = tr;
                    indices[counter++] = lr;
                }
            }
            return vectors;
        }

        /*public static List<Vec3> ToTexturedIndexedVertexData(this Array2<byte> data, out Vec3[] vertices, out Vec2[] texcoords, out uint[] indices)
        {
            List<Vec3> vectors = new List<Vec3>();
            int indexCount = (data.Width - 1) * (data.Height - 1) * 6;
            vertices = new Vec3[data.Count];
            texcoords = new Vec2[data.Count];
            indices = new uint[indexCount];
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    Vec3 pos = new Vec3(x, value, -y);
                    vectors.Add(pos);
                    vertices[x + y * data.Width] = pos;
                    float X = Maths.Clamp(x, -1, 1);
                    float Y = Maths.Clamp(-y, -1, 1);
                    texcoords[x + y * data.Width] = new Vec2(X * data.Width , Y * data.Height);
                    //texcoords[x + y * data.Width] = new Vec2(x / data.Width, -y / data.Height);
                    /*for (int tri = 0; tri < 6; tri++)
                    {
                        int aX = x + ((tri == 1 || tri == 2 || tri == 5) ? 1 : 0);
                        int aY = y + ((tri == 1 || tri == 2 || tri == 5) ? 1 : 0);
                        //vertices[x + y * data.Width] = new Vec3(aX, value, -aY);
                        texcoords[x + y * data.Width] = new Vec2(aX / data.Width, aY / data.Height);
                    }*/
                /*}
            }

            uint counter = 0;
            for (uint y = 0; y < data.Height - 1; y++)
            {
                for (uint x = 0; x < data.Width - 1; x++)
                {
                    uint ll = (uint)(x + y * data.Width);
                    uint lr = (uint)((x + 1) + y * data.Width);
                    uint tl = (uint)(x + (y + 1) * data.Width);
                    uint tr = (uint)((x + 1) + (y + 1) * data.Width);

                    indices[counter++] = tl;
                    indices[counter++] = lr;
                    indices[counter++] = ll;

                    indices[counter++] = tl;
                    indices[counter++] = tr;
                    indices[counter++] = lr;
                }
            }
            return vectors;
        }*/

        public static List<Vec3> ToTexturedIndexedVertexData(this Array2<byte> data, out Vec3[] vertices, out Vec2[] texcoords, out uint[] indices)
        {
            List<Vec3> vectors = new List<Vec3>();
            int indexCount = (data.Width - 1) * (data.Height - 1) * 6;
            vertices = new Vec3[data.Count];
            texcoords = new Vec2[data.Count];
            indices = new uint[indexCount];
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    Vec3 pos = new Vec3(x, value, y);
                    vectors.Add(pos);
                    vertices[x + y * data.Width] = pos;
                    texcoords[x + y * data.Width] = new Vec2((float)((double)x / data.Width), (float)((double)y / data.Height));
                    //if (texcoords[x + y * data.Width].Outside(-1, 1)) Console.WriteLine("#" + (x + y * data.Width) + "[" + x + "," + y + "]: " + texcoords[x + y * data.Width].ToString());
                    //texcoords[x + y * data.Width] = new Vec2((float)Maths.Clamp((double)x / data.Width, -1, 1), (float)Maths.Clamp((double)y / data.Height, -1, 1)); //Softens axis artifacts?
                    //Console.WriteLine("#" + (x + y * data.Width) + "[" + x + "," + y + "]: " + texcoords[x + y * data.Width].ToString());
                }
            }

            uint counter = 0;
            for (uint y = 0; y < data.Height - 1; y++)
            {
                for (uint x = 0; x < data.Width - 1; x++)
                {
                    uint ll = (uint)(x + y * data.Width);
                    uint lr = (uint)((x + 1) + y * data.Width);
                    uint tl = (uint)(x + (y + 1) * data.Width);
                    uint tr = (uint)((x + 1) + (y + 1) * data.Width);

                    indices[counter++] = tl;
                    indices[counter++] = lr;
                    indices[counter++] = ll;

                    indices[counter++] = tl;
                    indices[counter++] = tr;
                    indices[counter++] = lr;
                }
            }
            return vectors;
        }

        public static List<Vec3> ToVertexData(this Array2<byte> data)
        {
            List<Vec3> vectors = new List<Vec3>();
            for (int x = 0; x < data.Width; x++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    byte value = data.Get(x, y);
                    Vec3 ll = new Vec3(x, value, y);
                    /*Vec3 lr = new Vec3(x + 1, value, y);
                    Vec3 tl = new Vec3(x, value, y + 1);
                    Vec3 tr = new Vec3(x + 1, value, y + 1);
                    vectors.Add(tl);
                    vectors.Add(lr);*/
                    vectors.Add(ll);
                    /*vectors.Add(tl);
                    vectors.Add(tr);
                    vectors.Add(lr);*/
                }
            }
            return vectors;
        }

        /*public static List<Vec3> ToIndexedVertexData(this Array2<byte> data, out Vec3[] vertices, out uint[] indices, float scale = 100)
        {
            int indexCount = (data.Width - 1) * (data.Height - 1) * 6;
            float width = data.Width * scale;
            float length = data.Height * scale;
            float dx = width / (data.Width - 1);
            float dz = length / (data.Height - 1);
            List<Vec3> vectors = new List<Vec3>();
            vertices = new Vec3[data.Count];
            indices = new uint[indexCount];
            List<uint> indexList = new List<uint>();
            for (int i = 0; i < data.Count; i++)
            {
                float x = 0, y = 0, z = 0;
                if (i % data.Width == 0) z = i / data.Width;
                x = Math.Max(i - (z * data.Width), 0);
                y = data.Get(i);
                float xA = x * dx; //+x_start
                float zA = z * dz; //+z_start
                vectors.Add(vertices[i] = new Vec3(xA, y, zA));
                //skip x == 0 or z == 0..?
                //if (x == 0 || z == 0) continue;
                indexList.Add((uint)Maths.LinearAddress(x, z, data.Width));
                indexList.Add((uint)Maths.LinearAddress(x - 1, z, data.Width));
                indexList.Add((uint)Maths.LinearAddress(x - 1, z - 1, data.Width));
                indexList.Add((uint)Maths.LinearAddress(x, z, data.Width));
                indexList.Add((uint)Maths.LinearAddress(x - 1, z - 1, data.Width));
                indexList.Add((uint)Maths.LinearAddress(x, z - 1, data.Width));
            }
            indexList.Add((uint)Maths.LinearAddress(data.Width - 1, data.Height - 1, data.Width));
            indexList.Add((uint)Maths.LinearAddress(data.Width - 1, 0, data.Width));
            indexList.Add((uint)Maths.LinearAddress(0, 0, data.Width));
            indexList.Add((uint)Maths.LinearAddress(data.Width - 1, data.Height - 1, data.Width));
            indexList.Add((uint)Maths.LinearAddress(0, 0, data.Width));
            indexList.Add((uint)Maths.LinearAddress(0, data.Height - 1, data.Width));
            indices = indexList.ToArray();
            return vectors;
        }*/

    }
}
