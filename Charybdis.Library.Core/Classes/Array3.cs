using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// Simulates a 3D array using a list of Array2s.
    /// </summary>
    public class Array3<T> : List<Array2<T>>
    {
        public Array3(int width, int height, int depth)
        {
            layers = depth;
            numRows = height;
            rowWidth = width;
            for (int z = 0; z < depth; z++)
                Add(new Array2<T>(width, height));
        }

        public T Get(int x, int y, int z)
        {
            return this[z].Get(x, y);
        }

        public void Set(int x, int y, int z, T value)
        {
            this[z].Set(x, y, value);
        }

        public T Get(Vec3 coordinates)
        {
            return this[coordinates.Zi].Get(coordinates.Xi, coordinates.Yi);
        }

        public void Set(Vec3 coordinates, T value)
        {
            this[coordinates.Zi].Set(coordinates.Xi, coordinates.Yi, value);
        }

        protected int rowWidth;
        public int Width
        {
            get
            {
                return rowWidth;
            }
        }

        protected int numRows;
        public int Height
        {
            get
            {
                return numRows;
            }
        }

        protected int layers;
        public int Depth
        {
            get
            {
                return layers;
            }
        }

        public int Count
        {
            get
            {
                return rowWidth * numRows * layers;
            }
        }
    }
}