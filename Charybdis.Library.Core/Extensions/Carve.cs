using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public static class Carve
    {
        public static Array2<byte> Automata(this Array2<byte> data, int generations = 10, byte threshold = 200, byte value = 200, bool lessThan = false)
        {
            for (int g = 0; g < generations; g++)
            {
                for (int y = 0; y < data.Height; y++)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        int total = data.Get(x, y);
                        if (x > 0)
                        {
                            total += data.Get(x - 1, y);
                        }
                        if (x < data.Width - 1)
                        {
                            total += data.Get(x + 1, y);
                        }
                        if (y > 0)
                        {
                            total += data.Get(x, y - 1);
                        }
                        if (y < data.Height - 1)
                        {
                            total += data.Get(x, y + 1);
                        }
                        if (lessThan)
                        {
                            if (total / 5 <= threshold) data.Set(x, y, value);
                        }
                        else if (total / 5 >= threshold) data.Set(x, y, value);

                    }
                }
            }
            return data;
        }
    }
}
