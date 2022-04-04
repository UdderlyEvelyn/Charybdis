using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Infinitum
{
    public class StoneTile : Tile
    {
        public StoneTile(Vec2 position) : base(Material.Stone, position)
        {

        }
    }
}
