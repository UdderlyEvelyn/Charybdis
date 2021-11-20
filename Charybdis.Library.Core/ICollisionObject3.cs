using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public interface ICollisionObject3
    {
        Vec3 Position { get; set; }
        Vec3 OriginalPosition { get; set; }
        float CollisionRadius { get; set; }
    }
}