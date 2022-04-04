using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Charybdis.MonoGame;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace EvolutionSimilator
{
    public class TerrainLine : GameObject2
    {
        private Line _line;

        public TerrainLine(Vec2 a, Vec2 b)
        {
            _line = new Line(a, b);
            Visual = _line;
        }

        public Vec2 A
        {
            get
            {
                return _line.A;
            }
            set
            {
                _line.A = value;
            }
        }

        public Vec2 B
        {
            get
            {
                return _line.B;
            }
            set
            {
                _line.B = value;
            }
        }

        public override void Update()
        {
            /*Nothing to do here.*/
        }
    }
}
