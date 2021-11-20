using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public class OffsetRect : Rect
    {
        public OffsetRect(Vec2 position, Vec2 size)
            : base(position, size)
        {

        }

        public OffsetRect(int x, int y, int width, int height)
            : base(x, y, width, height)
        {

        }

        public Vec2 DestOffset { get; set; }
        public Vec2 DestPadding { get; set; }
    }
}
