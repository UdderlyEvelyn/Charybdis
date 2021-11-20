using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public interface ICollisionObject2
    {
        Vec2 Position { get; set; }
        Vec2 OriginalPosition { get; set; }
        float CollisionRadius { get; set; }
    }
}