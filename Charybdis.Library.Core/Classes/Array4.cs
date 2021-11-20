using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charybdis.Library.Core
{
    /// <summary>
    /// Emulates a 4D array (2D array of 2D arrays) using a 1D array for efficiency/speed while providing the usability of a 4D array's logic.
    /// </summary>
    public class Array4<dataType>
    {
        public int rowWidth = 1;
        public int numRows = 1;
        public int cellWidth = 1;
        public int cellRows = 1;
        public Array2<Array2<dataType>>[] array;
        public dataType defaultValue;
        public int Count
        {
            get
            {
                return rowWidth * numRows;
            }
        }
        public int itemCount
        {
            get
            {
                return (rowWidth * cellWidth) * (numRows * cellRows);
            }
        }
        public int cellSize
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
        public Array4(int rowWidth, int numRows, int cellWidth, int cellRows, dataType defaultValue)
        {
            //Move our parameters into the classwide variables.
            this.rowWidth = rowWidth;
            this.numRows = numRows;
            this.cellRows = cellRows;
            this.cellWidth = cellWidth;
            this.defaultValue = defaultValue;
            this.array = new Array2<Array2<dataType>>[rowWidth * numRows];
        }

        public Array2<dataType> getCell(int x, int y)
        {
            //Return the value with the offset calculated based on our virtual array.
            return array[x + (y * rowWidth)] as Array2<dataType>;
        }

        public Array2<dataType> getCell(int i)
        {
            return array[i] as Array2<dataType>;
        }

        public dataType get(int x, int y, int cx, int cy)
        {
            return getCell(x, y).Get(cx, cy);
        }

        public void put(int x, int y, int cx, int cy, dataType value)
        {
            //Place the value using the offset calculated based on our virtual array.
            getCell(x, y).Put(cx, cy, value);
        }

        public void putCell(int x, int y, Array2<dataType> c)
        {
            Array2<dataType> cell = getCell(x, y);
            if (cell.Width == c.Width && cell.Height == c.Height)
            {
                cell.SetArray(c.Array);
            }
            else throw new ArgumentOutOfRangeException("c");
        }

        public void clear()
        {
            for (int i = 0; i < array.Count(); i++)
            {
                Array2<dataType> cell = getCell(i);
                cell.Fill(defaultValue);
            }
        }

        public void fill(dataType value, int xMin = 0, int xMax = 0, int yMin = 0, int yMax = 0)
        {
            if (xMax == 0) xMax = rowWidth;
            if (yMax == 0) yMax = numRows;
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    putCell(x, y, new Array2<dataType>(cellWidth, cellRows, value));
                }
            }
        }
    }
}
