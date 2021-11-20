using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public static class Miscellaneous
    {
        /// <summary>
        /// Changes every value equal to "from" to be equal to "to".
        /// </summary>
        /// <param name="data"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Array2<byte> Swap(this Array2<byte> data, byte from, byte to)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data.Get(i) == from) data.Set(i, to);
            }
            return data;
        }

        /// <summary>
        /// Modifies every value that meets the "threshold" by "offset".
        /// </summary>
        /// <param name="data"></param>
        /// <param name="threshold"></param>
        /// <param name="offset"></param>
        /// <param name="lessThan"></param>
        /// <returns></returns>
        public static Array2<byte> Bias(this Array2<byte> data, byte threshold, byte offset, bool lessThan = false, bool subtract = false)
        {
            for (int i = 0; i < data.Count; i++)
            {
                byte b = data.Get(i);
                if (lessThan)
                {
                    if (b <= threshold)
                    {
                        if (subtract)
                            data.Set(i, (byte)(b - offset));
                        else
                            data.Set(i, (byte)(b + offset));
                    }
                }
                else
                {
                    if (b >= threshold)
                    {
                        if (subtract)
                            data.Set(i, (byte)(b - offset));
                        else
                            data.Set(i, (byte)(b + offset));
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// Modifies every value within the range by "offset".
        /// </summary>
        public static Array2<byte> BiasRange(this Array2<byte> data, byte min, byte max, byte offset, bool subtract = false)
        {
            for (int i = 0; i < data.Count; i++)
            {
                byte b = data.Get(i);
                if (b >= min && b <= max)
                {
                    if (subtract)
                        data.Set(i, (byte)(b - offset));
                    else
                        data.Set(i, (byte)(b + offset));
                }
            }
            return data;
        }
    }
}
