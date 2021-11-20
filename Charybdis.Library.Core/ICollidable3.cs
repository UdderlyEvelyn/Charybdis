using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public interface ICollidable3
    {
        Vec3 Position { get; set; }
        BoundingCube BoundingCube { get; set; }
    }
}
