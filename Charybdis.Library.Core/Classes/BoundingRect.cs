using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    public class BoundingRect : CharybdisObject, ICollisionObject2
    {        
        private Vec2 _position;
        private Vec2 _originalPosition;
        private float _collisionRadius;
        public float Size = 1;
        protected float _halfSize = .5f;

        public Vec2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public Vec2 OriginalPosition
        {
            get
            {
                return _originalPosition;
            }
            set
            {
                _originalPosition = value;
            }
        }

        public float CollisionRadius
        {
            get
            {
                return _collisionRadius;
            }
            set
            {
                _collisionRadius = value;
            }
        }

        public Vec2 Min
        {
            get
            {
                return _position - _halfSize;
            }
        }

        public Vec2 Max
        {
            get
            {
                return _position + _halfSize;
            }
        }
        
        public BoundingRect(Vec2 position, float size)
        {
            _position = position;
            _originalPosition = position;
            Size = size;
            _collisionRadius = (float)(size + size * .3);
        }

        public BoundingRect(Vec2 min, Vec2 max)
        {
            float xDiff = max.X - min.X;
            float yDiff = max.Y - min.Y;
            //if (!(xDiff == yDiff)) throw new ArgumentException("The differences in X and Y of the two Vec2 arguments must be equal (i.e., they must form a square).");
            _position = min;
            _originalPosition = min;
            Size = xDiff;
            _halfSize = Size / 2;
            _collisionRadius = (float)(Size + Size * .3);
        }

        public bool Contains(Vec2 point)
        {
            return Min.X <= point.X && point.X <= Max.X && Min.Y <= point.Y && point.Y <= Max.Y;
        }
    }
}