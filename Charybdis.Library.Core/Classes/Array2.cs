using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class ClassArray2<T> : Array2<T>
        where T : class
    {
        public override T Get(int x, int y)
        {
            try
            {
                return array[rowWidth * y + x];
            }
            catch (Exception) //Totally shouldn't be doing this. :D
            {
                return null;
            }
            //catch (IndexOutOfRangeException)
            //{
            //    return null;
            //}
            //catch (ArgumentOutOfRangeException)
            //{
            //    return null;
            //}
        }

        public ClassArray2(int width, int height) : base(width, height)
        {

        }

        public ClassArray2(int width, int height, T defaultValue) : base(width, height, defaultValue)
        {

        }

        public ClassArray2(int width, int height, Func<T> defaultValueGenerator) : base(width, height, defaultValueGenerator)
        {

        }

        public ClassArray2(int width, int height, Func<int, T> indexDependentDefaultValueGenerator) : base(width, height, indexDependentDefaultValueGenerator)
        {

        }
    }

    public class Array2<T>
    {
        public Random Random = new Random();

        protected T[] array;

        protected int rowWidth = 0;
        protected int numRows = 0;

        public T[] Array
        {
            get
            {
                return array;
            }
        }

        public void SetArray(T[] array)
        {
            this.array = array;
        }

        public int Width
        {
            get
            {
                return rowWidth;
            }
        }

        public int Height
        {
            get
            {
                return numRows;
            }
        }

        public int Count
        {
            get
            {
                return array.Length;
            }
        }

        public Array2(int width, int height)
        {
            array = new T[width * height];
            rowWidth = width;
            numRows = height;
        }

        public Array2(int width, int height, T defaultValue)
        {
            array = new T[width * height];
            rowWidth = width;
            numRows = height;
            Fill(defaultValue);
        }

        public Array2(int width, int height, Func<T> defaultValueGenerator)
        {
            array = new T[width * height];
            rowWidth = width;
            numRows = height;
            Fill(defaultValueGenerator);
        }

        public Array2(int width, int height, Func<int, T> indexDependentDefaultValueGenerator)
        {
            array = new T[width * height];
            rowWidth = width;
            numRows = height;
            Fill(indexDependentDefaultValueGenerator);
        }

        public bool InRange(int i)
        {
            return i.Inside(array.GetLowerBound(0), array.GetUpperBound(0));
        }

        public bool InRange(int x, int y)
        {
            if (x < 0) return false;
            if (y < 0) return false;
            if (x > rowWidth - 1) return false;
            if (y > numRows - 1) return false;
            return true;
        }

        public Array2<T> Fill(Func<int, T> indexDependentValueGenerator)
        {
            for (int i = 0; i < Count; i++) Set(i, indexDependentValueGenerator(i));
            return this;
        }

        public Array2<T> Fill(Func<T> valueGenerator)
        {
            for (int i = 0; i < Count; i++) Set(i, valueGenerator());
            return this;
        }

        public Array2<T> Fill(T value)
        {
            for (int i = 0; i < Count; i++) Set(i, value);
            return this;
        }

        public List<T> GetColumn(int x)
        {
            List<T> result = new List<T>();
            for (int y = 0; y < Height; y++)
                result.Add(Get(x, y));
            return result;
        }

        public List<T> GetRow(int y)
        {
            List<T> result = new List<T>();
            for (int x = 0; x < Width; x++)
                result.Add(Get(x, y));
            return result;
        }

        public virtual T Get(int x, int y)
        {
            return array[rowWidth * y + x];
        }

        public T Get(int i)
        {
            return array[i];
        }

        public void Put(int x, int y, T value)
        {
            Set(x, y, value);
        }

        public void Set(int x, int y, T value)
        {
             array[rowWidth * y + x] = value;
        }

        public void Put(int i, T value)
        {
            Set(i, value);
        }

        public void Set(int i, T value)
        {
            array[i] = value;
        }

        public T Largest
        {
            get
            {
                return array.Max();
            }
        }

        public T Smallest
        {
            get
            {
                return array.Min();
            }
        }

        public Array2<R> Select<R>(Func<T, R> selector)
        {
            var result = new Array2<R>(Width, Height);
            for (int i = 0; i < array.Length; i++)
                result.array[i] = selector(array[i]);
            return result;
        }

        public static Array2<byte> Stitch(Array2<byte> topLeft, Array2<byte> topRight, Array2<byte> bottomLeft, Array2<byte> bottomRight)
        {
            if (typeof(T) != typeof(byte)) throw new Exception("The \"Stitch\" function only works with Array2<byte>.");
            if ((topLeft.Width == topRight.Width) && (bottomLeft.Width == bottomRight.Width) && (topLeft.Width == bottomLeft.Width) && (topLeft.Height == topRight.Height) && (bottomLeft.Height == bottomRight.Height) && (topLeft.Height == bottomLeft.Height))
            {
                int width = topLeft.Width * 2;
                int height = topLeft.Height * 2;
                Array2<byte> result = new Array2<byte>(width, height);
                for (int y = 0; y < height; y++)
                {
                    if (y < height / 2) //Top
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (x < width / 2) //Left
                            {
                                result.Set(x, y, topLeft.Get(x, y));
                            }
                            else if (x >= width / 2) //Right
                            {
                                result.Set(x, y, topRight.Get(x - width / 2, y));
                            }
                        }
                    }
                    else if (y >= height / 2) //Bottom
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (x < width / 2) //Left
                            {
                                result.Set(x, y, bottomLeft.Get(x, y - height / 2));
                            }
                            else if (x >= width / 2) //Right
                            {
                                result.Set(x, y, bottomRight.Get(x - width / 2, y - height / 2));
                            }
                        }
                    }
                }
                return result;
            }
            else throw new Exception("The arrays to be stitched must have the same dimensions.");
        }
    }
}
