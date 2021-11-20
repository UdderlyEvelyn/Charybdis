using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Charybdis.Library.Core;

namespace Charybdis.Library.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TextureVertex : IVertex
    {
        private Vec3 _position;
        private Vec3 _normal;
        private Vec2 _textureCoordinate;

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
        public Vec3 Normal 
        {
            get
            {
                return _normal;
            }
            set
            {
                _normal = value;
            }
        }
        public Vec2 TextureCoordinate 
        {
            get
            {
                return _textureCoordinate;
            }
            set
            {
                _textureCoordinate = value;
            }
        }

        public TextureVertex(float x, float y, float z, float nx, float ny, float nz, float tx, float ty)
        {
            _position = new Vec3(x, y, z);
            _normal = new Vec3(nx, ny, nz);
            _textureCoordinate = new Vec2(tx, ty);
        }

        public TextureVertex(Vec3 v, Vec3 n, Vec2 t)
        {
            _position = v;
            _normal = n;
            _textureCoordinate = t;
        }

        public TextureVertex(float x, float y, float z, float tx, float ty)
        {
            _position = new Vec3(x, y, z);
            _normal = _position;
            _normal.Normalize();
            _textureCoordinate = new Vec2(tx, ty);
        }

        public TextureVertex(Vec3 v, Vec2 t)
        {
            _position = v;
            _normal = v;
            _normal.Normalize();
            _textureCoordinate = t;
        }
    }
}
