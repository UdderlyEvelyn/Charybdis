using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core.Datatypes
{
    public class Polygon
    {
        public List<Vec2> Points { get; set; }

        public Polygon()
        {
            Points = new List<Vec2>();
        }
    }
}
