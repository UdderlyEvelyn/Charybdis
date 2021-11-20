using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public static class Blend
    {
        public static Array2<byte> Average(this Array2<byte> target, Array2<byte> data, float weight = .6f)
        {
            for (int i = 0; i < target.Count; i++)
            {
                target.Set(i, (byte)(target.Get(i) * (1 - weight) + data.Get(i) * weight));
            }
            return target;
        }
    }
}
