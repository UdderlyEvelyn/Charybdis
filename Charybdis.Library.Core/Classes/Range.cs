using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class Range
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public bool Contains(double value)
        {
            return value >= Min && value <= Max;
        }

        public Range(double min, double max)
        {
            Min = min;
            Max = max;
        }
    }
}
