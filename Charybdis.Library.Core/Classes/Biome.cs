using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class Biome
    {
        public string Name { get; set; }
        public float Reflectivity { get; set; }
        public float Temperature { get; set; }
        public float Moisture { get; set; }
        public float ColorBias { get; set; }
    }
}
