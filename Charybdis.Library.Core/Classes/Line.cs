using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    public class Line : CharybdisObject, ICollisionObject2
    {
        private Vec2 _position;
        private Vec2 _originalPosition;
        private Vec2 _direction;
        private float _collisionRadius = 0f;

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

        public Vec2 Direction
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

        public Line(Vec2 position, Vec2 direction)
        {
            Position = position;
            Direction = direction;
            DrawMe = false;
        }
    }
}
