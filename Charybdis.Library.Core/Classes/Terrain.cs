using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class Terrain
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Array2<Instance[]> Positions { get; set; }

        public Terrain(int width, int height, Vertex[] vertices, Index[] indices)
        {
            Width = width;
            Height = height;
            Positions = new Array2<Instance[]>(width, height);
        }

        public void Fill(Instance[] instances)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Positions.Set(x, y, instances.Where(i => i.Position.X == x && i.Position.Z == y).OrderBy(i => i.Position.Y).ToArray());
                }
            }
        }

        public List<Instance> GetInstances()
        {
            List<Instance> instances = new List<Instance>();
            foreach (Instance[] ia in Positions.Array)
            {
                instances.AddRange(ia);
            }
            return instances;
        }
    }
}
