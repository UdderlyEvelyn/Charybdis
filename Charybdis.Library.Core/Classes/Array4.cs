using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// This is a lie, it uses an Array2 of Array2s.
    /// Emulates a 4D array (2D array of 2D arrays) using a 1D array for efficiency/speed while providing the usability of a 4D array's logic.
    /// </summary>
    public class Array4<T>
    {
        public int rowWidth = 1;
        public int numRows = 1;
        public int cellWidth = 1;
        public int cellRows = 1;
        public Array2<Array2<T>>[] array;
        public T defaultValue;
        public int Count
        {
            get
            {
                return rowWidth * numRows;
            }
        }
        public int ItemCount
        {
            get
            {
                return (rowWidth * cellWidth) * (numRows * cellRows);
            }
        }
        public int CellSize
        {
            get
            {
                return cellWidth * cellRows;
            }
        }

        /// <summary>
        /// Emulated 4D array optimized for speed.
        /// </summary>
        /// <param name="rowWidth">Virtual width of the encapsulating 2D array's rows (X).</param>
        /// <param name="numRows">Virtual number of rows in the encapsulating 2D array (Y).</param>
        /// <param name="cellRows">Virtual number of rows in each cell (nested 2D array).</param>
        /// <param name="cellWidth">Virtual number of values in each cell (nested 2D array) row.</param>
        /// <param name="defaultValue">The value each item will be initialized with.</param>
        public Array4(int rowWidth, int numRows, int cellWidth, int cellRows, T defaultValue)
        {
            //Move our parameters into the classwide variables.
            this.rowWidth = rowWidth;
            this.numRows = numRows;
            this.cellRows = cellRows;
            this.cellWidth = cellWidth;
            this.defaultValue = defaultValue;
            this.array = new Array2<Array2<T>>[rowWidth * numRows];
        }

        public Array2<T> GetCell(int x, int y)
        {
            //Return the value with the offset calculated based on our virtual array.
            return array[x + (y * rowWidth)] as Array2<T>;
        }

        public Array2<T> GetCell(int i)
        {
            return array[i] as Array2<T>;
        }

        public T Get(int x, int y, int cx, int cy)
        {
            return GetCell(x, y).Get(cx, cy);
        }

        public void Set(int x, int y, int cx, int cy, T value)
        {
            //Place the value using the offset calculated based on our virtual array.
            GetCell(x, y).Set(cx, cy, value);
        }

        public void SetCell(int x, int y, Array2<T> c)
        {
            Array2<T> cell = GetCell(x, y);
            if (cell.Width == c.Width && cell.Height == c.Height)
            {
                cell.SetArray(c.Array);
            }
            else throw new ArgumentOutOfRangeException("c");
        }

        public void Clear()
        {
            for (int i = 0; i < array.Count(); i++)
            {
                Array2<T> cell = GetCell(i);
                cell.Fill(defaultValue);
            }
        }

        public void Fill(T value, int xMin = 0, int xMax = 0, int yMin = 0, int yMax = 0)
        {
            if (xMax == 0) xMax = rowWidth;
            if (yMax == 0) yMax = numRows;
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    SetCell(x, y, new Array2<T>(cellWidth, cellRows, value));
                }
            }
        }
    }
}
