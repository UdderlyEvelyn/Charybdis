using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    [Flags]
    public enum Direction2D : int
    {
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
        UpLeft = Up | Left,
        DownLeft = Down | Left,
        UpRight = Up | Right,
        DownRight = Down | Right,
    }

    public enum Directionality2D
    {
        Orthogonal,
        Diagonal,
        All,
    }
    
    public static class ArrayExtensions
    {
        public static int GetIndex<T>(this Array2<T> array, T item)
            where T : class
        {
            return Array.IndexOf(array.Array, item);
        }

        public static int[] GetPosition<T>(this Array2<T> array, T item)
            where T : class
        {
            var addr = Maths.EuclideanAddress(Array.IndexOf(array.Array, item), array.Width);
            return new int[] { addr.Xi, addr.Yi };
        }

        public static void Fill<T>(this T[] array, Func<T> valueSource)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = valueSource();
        }

        public static bool HasNeighbor(this Array2<bool> data, int index, Directionality2D d = Directionality2D.All)
        {
            bool all = d.HasFlag(Directionality2D.All);
            if (all || d.HasFlag(Directionality2D.Orthogonal))
            {
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.Left))
                    return true;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.Right))
                    return true;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.Up))
                    return true;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.Down))
                    return true;
            }
            if (all || d.HasFlag(Directionality2D.Diagonal))
            {
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.UpLeft))
                    return true;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.UpRight))
                    return true;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.DownLeft))
                    return true;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.DownRight))
                    return true;
            }
            return false;
        }
        
        public static int NeighborCount(this Array2<bool> data, int index, Directionality2D d = Directionality2D.All)
        {
            int result = 0;
            bool all = d.HasFlag(Directionality2D.All);
            if (all || d.HasFlag(Directionality2D.Orthogonal))
            {
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.Left))
                    result++;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.Right))
                    result++;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.Up))
                    result++;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.Down))
                    result++;
            }
            if (all || d.HasFlag(Directionality2D.Diagonal))
            {
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.UpLeft))
                    result++;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.UpRight))
                    result++;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.DownLeft))
                    result++;
                if (data.HasNeighborWithValueInDirection(index, true, Direction2D.DownRight))
                    result++;
            }
            return result;
        }

        public static List<int> GetNeighborIndices(this Array2<bool> data, int index, Directionality2D d = Directionality2D.All)
        {
            List<int> result = new List<int>();
            bool all = d.HasFlag(Directionality2D.All);
            if (all || d.HasFlag(Directionality2D.Orthogonal))
            {
                int targetIndex = data.Shift(index, Direction2D.Left);
                if (targetIndex != INVALID_SHIFT_RESULT)
                    if (data.HasValueAt(targetIndex))
                        result.Add(targetIndex);
                targetIndex = data.Shift(index, Direction2D.Right);
                if (targetIndex != INVALID_SHIFT_RESULT)
                    if (data.HasValueAt(targetIndex))
                        result.Add(targetIndex);
                targetIndex = data.Shift(index, Direction2D.Up);
                if (targetIndex != INVALID_SHIFT_RESULT)
                    if (data.HasValueAt(targetIndex))
                        result.Add(targetIndex);
                targetIndex = data.Shift(index, Direction2D.Down);
                if (targetIndex != INVALID_SHIFT_RESULT)
                    if (data.HasValueAt(targetIndex))
                        result.Add(targetIndex);
            }
            if (all || d.HasFlag(Directionality2D.Diagonal))
            {
                int targetIndex = data.Shift(index, Direction2D.UpLeft);
                if (targetIndex != INVALID_SHIFT_RESULT)
                    if (data.HasValueAt(targetIndex))
                        result.Add(targetIndex);
                targetIndex = data.Shift(index, Direction2D.UpRight);
                if (targetIndex != INVALID_SHIFT_RESULT)
                    if (data.HasValueAt(targetIndex))
                        result.Add(targetIndex);
                targetIndex = data.Shift(index, Direction2D.DownLeft);
                if (targetIndex != INVALID_SHIFT_RESULT)
                    if (data.HasValueAt(targetIndex))
                        result.Add(targetIndex);
                targetIndex = data.Shift(index, Direction2D.DownRight);
                if (targetIndex != INVALID_SHIFT_RESULT)
                    if (data.HasValueAt(targetIndex))
                        result.Add(targetIndex);
            }
            return result;
        }

        public static bool OnlyNeighborOfNeighbor(this Array2<bool> data, int index, Direction2D d)
        {
            int neighborIndex = data.Shift(index, d);
            if (neighborIndex == INVALID_SHIFT_RESULT)
                throw new InvalidOperationException();
            return data.NeighborCount(neighborIndex) == 1;
        }

        public static bool HasNeighborWithValueInDirection<T>(this Array2<T> data, int index, T value, Direction2D d)
        {
            int targetIndex = data.Shift(index, d);
            return targetIndex == INVALID_SHIFT_RESULT ? false : data.HasValueAt(targetIndex, value);
        }

        public static bool HasValueAt(this Array2<bool> data, int index)
        {
            return data.HasValueAt(index, true);
        }

        public static bool HasValueAt<T>(this Array2<T> data, int index, T value)
        {
            return data.Get(index).Equals(value);
        }

        public static bool HasValueAt<T>(this Array2<T> data, int x, int y, T value)
        {
            return data.Get(x, y).Equals(value);
        }

        public const int INVALID_SHIFT_RESULT = -1;

        public static int Shift<T>(this Array2<T> data, int index, Direction2D d)
        {
            //Translate 1D coordinate to 2D coordinate according to array width.
            var i = Maths.EuclideanAddress(index, data.Width);
            //Separate variables to store the delta in case we run into a problem and don't want to apply it.
            int dX = 0;
            int dY = 0;
            //Modify 2D coordinates based on requested direction.
            if (d.HasFlag(Direction2D.Left))
            {
                if (i.X == 0)
                    return INVALID_SHIFT_RESULT;
                dX--;
            }
            if (d.HasFlag(Direction2D.Right))
            {
                if (i.X == data.Width - 1)
                    return INVALID_SHIFT_RESULT;
                dX++;
            }
            if (d.HasFlag(Direction2D.Up))
            {
                if (i.Y == 0)
                    return INVALID_SHIFT_RESULT;
                dY--;
            }
            if (d.HasFlag(Direction2D.Down))
            {
                if (i.Y == data.Height - 1)
                    return INVALID_SHIFT_RESULT;
                dY++;
            }
            return Maths.LinearAddress(i.Xi + dX, i.Yi + dY, data.Width); //Apply delta and translate back to 1D index.
        }

        public static Array2<byte> ToByteArray(this Array2<float> data)
        {
            Array2<byte> result = new Array2<byte>(data.Width, data.Height);
            for (int i = 0; i < data.Count; i++)
            {
                result.Set(i, (byte)(Math.Abs(data.Get(i)) * 255));
            }
            return result;
        }

        public static void ToConsole(this Array2<bool> data)
        {
            Console.WriteLine(data.Width + "x" + data.Height + " Array2<bool>");
            for (int i = 0; i < data.Count; i++)
                Console.Write(i % data.Width == 0 ? "\n" : (data.Get(i) ? "X " : "  "));
        }

        //public static void ToConsole<T>(this Array2<T> data)
        //{
        //    Console.WriteLine(data.Width + "x" + data.Height + " Array2<" + typeof(T).Name + ">");
        //    for (int i = 0; i < data.Count; i++)
        //        Console.Write(i % data.Width == 0 ? '\n' : (data.Get(i) ? 'X' : ' '));
        //}

        public static void ToConsole(this Array2<float> data)
        {
            Console.WriteLine(data.Width + "x" + data.Height + " Array2<float>");
            for (int y = 0; y < data.Height; y++)
            {
                Console.Write("\n");
                for (int x = 0; x < data.Width; x++)
                {
                    Console.Write(data.Get(x, y).ToString() + ' ');
                }
            }
        }

        public static void ToConsole(this Array2<byte> data)
        {
            Console.WriteLine(data.Width + "x" + data.Height + " Array2<byte>");
            for (int y = 0; y < data.Height; y++)
            {
                Console.Write("\n");
                for (int x = 0; x < data.Width; x++)
                {
                    Console.Write(data.Get(x, y).ToString() + ' ');
                }
            }
        }
    }
}
