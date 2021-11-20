using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    public class BoundingCube : CharybdisObject, ICollisionObject3
    {        
        private Vec3 _position;
        private Vec3 _originalPosition;
        private float _collisionRadius;
        public float Size = 1;
        protected float _halfSize = .5f;

        public Vec3 Position
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

        public Vec3 OriginalPosition
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

        public Vec3 Min
        {
            get
            {
                return _position - _halfSize;
            }
        }

        public Vec3 Max
        {
            get
            {
                return _position + _halfSize;
            }
        }
        
        public BoundingCube(Vec3 position, float size)
        {
            _position = position;
            _originalPosition = position;
            Size = size;
            _collisionRadius = (float)(size + size * .3);
        }

        public BoundingCube(Vec3 min, Vec3 max)
        {
            float xDiff = max.X - min.X;
            float yDiff = max.Y - min.Y;
            float zDiff = max.Z - min.Z;
            if (!(xDiff == yDiff && xDiff == zDiff)) throw new ArgumentException("The differences in X, Y, and Z of the two Vec3 arguments must be equal (i.e., they must form a cube).");
            _position = min;
            _originalPosition = min;
            Size = xDiff;
            _halfSize = Size / 2;
            _collisionRadius = (float)(Size + Size * .3);
        }
    }
}
