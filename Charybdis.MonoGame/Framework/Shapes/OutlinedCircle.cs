using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Charybdis.MonoGame
{
    //Doesn't work right.
    public class OutlinedCircle : Shape
    {
        protected override _shapeDrawStrategy _drawStrategy
        {
            get
            {
                return _shapeDrawStrategy.Radial;
            }
        }

        public Col4? FillColor;

        private Circle _fillCircle = null;

        public OutlinedCircle(float radius)
        {
            Radius = radius;
            DirectionalBiasFunction = angle => Maths.CirclePointAtAngle(Radius.Value, angle);
            _fillCircle = new Circle(radius - .5f);
        }

        public override void Draw(SpriteBatch spriteBatch, Vec2 offset)
        {
            if (FillColor.HasValue)
            {
                _fillCircle.Color = FillColor.Value;
                _fillCircle.Draw(spriteBatch, offset);
            }
            base.Draw(spriteBatch, offset);
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           