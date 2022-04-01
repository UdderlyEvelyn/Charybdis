using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    public class Ray3 : CharybdisObject, ICollisionObject3
    {
        private Vec3 _position;
        private Vec3 _originalPosition;
        private Vec3 _direction;
        private float _collisionRadius = 0f;

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

        public Vec3 Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
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

        public bool DrawMe { get; set; }

        public Ray3(Vec3 position, Vec3 direction)
        {
            Position = position;
            Direction = direction;
            DrawMe = false;
        }
    }
}
