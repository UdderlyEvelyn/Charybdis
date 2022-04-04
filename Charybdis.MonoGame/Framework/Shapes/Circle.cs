using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Charybdis.MonoGame
{
    public class Circle : Shape
    {
        protected override _shapeDrawStrategy _drawStrategy
        {
            get
            {
                return _shapeDrawStrategy.Radial;
            }
        }

        public Circle(float radius)
        {
            Radius = radius;
            DirectionalBiasFunction = angle => Maths.CirclePointAtAngle(Radius.Value, angle).ToXNA();
        }
    }
}
