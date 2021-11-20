using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.MonoGame.Libraries;

namespace Charybdis.MonoGame.Datatypes
{
    public class Cell : Array2<byte>
    {
        private int _x = 0;
        private int _y = 0;
        private int _projectedWidth = 0;
        private int _projectedHeight = 0;

        public int X
        {
            get
            {
                return _x;
            }
            private set
            {
                _x = value;
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }
            private set
            {
                _y = value;
            }
        }

        private double _seed = 0;

        public double Seed
        {
            get
            {
                return _seed;
            }
            private set
            {
                _seed = value;
            }
        }

        public Cell(double seed, int width, int height, int x = 0, int y = 0, int projectedWidth = 0, int projectedHeight = 0) : base(width, height)
        {
            _x = x;
            _y = y;
            _seed = seed;
            _projectedWidth = projectedWidth;
            _projectedHeight = projectedHeight;
        }

        public Cell Simplex()
        {
            return this.SimplexNoise(_seed, null, _x * Width, _y * Height, _projectedWidth, _projectedHeight) as Cell;
        }

        public Cell Blur(int passes = 1)
        {
            Cell c = new Cell(this.CheapBlur(passes), Seed, X, Y, _projectedWidth, _projectedHeight);
            c.numRows = numRows;
            c.rowWidth = rowWidth;
            c.Random = Random;
            return c;
        }

        public Cell WBlur(int passes = 1)
        {
            Cell c = new Cell(this.WeightedBlur(passes), Seed, X, Y, _projectedWidth, _projectedHeight);
            c.numRows = numRows;
            c.rowWidth = rowWidth;
            c.Random = Random;
            return c;
        }

        public Cell LBlur()
        {
            Cell c = new Cell(this.LayeredBlur(), Seed, X, Y, _projectedWidth, _projectedHeight);
            c.numRows = numRows;
            c.rowWidth = rowWidth;
            c.Random = Random;
            return c;
        }

        public Cell(Array2<byte> data, double seed, int x = 0, int y = 0, int projectedWidth = 0, int projectedHeight = 0) : base(data.Width, data.Height)
        {
            _x = x;
            _y = y;
            _seed = seed;
            _projectedWidth = projectedWidth;
            _projectedHeight = projectedHeight;
            array = data.Array;
        }
    }
}
