using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    public class LineSegment : CharybdisObject
    {
        private Vec2 _a;

        public Vec2 A
        {
            get
            {
                return _a;
            }
            set
            {
                if (Points.Contains(_a))
                    Points.Remove(_a);
                _a = value;
                Points.Add(_a);                
            }
        }

        private Vec2 _b;

        public Vec2 B
        {
            get
            {
                return _b;
            }
            set
            {
                if (Points.Contains(_b))
                    Points.Remove(_b);
                _b = value;
                Points.Add(_b);
            }
        }
        
        public List<Vec2> Points { get; set; }

        public LineSegment(Vec2 a, Vec2 b)
        {
            Points = new List<Vec2>();
            A = a;
            B = b;
        }
    }
}
