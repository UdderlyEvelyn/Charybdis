using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Charybdis.Library.Core
{
    public class Model
    {
        private static long _id;

        public long ID { get; set; }
        public string Name { get; set; }
        public LitTextureVertex[] Vertices { get; set; }
        public Index[] Indices { get; set; }
        public List<Instance> Instances { get; set; }
        public string TexturePath { get; set; }

        public Model(LitTextureVertex[] vertices, Index[] indices, string name = "")
        {
            ID = Interlocked.Increment(ref _id);
            Name = name;
            Vertices = vertices;
            Indices = indices;
            Instances = new List<Instance>();
        }

        public Instance Instantiate(Vec3 position, float size = 1f, float yaw = 0, float pitch = 0, float roll = 0)
        {
            Instance i = new Instance(this, position, size, yaw, pitch, roll);
            Instances.Add(i);
            return i;
        }
    }
}
