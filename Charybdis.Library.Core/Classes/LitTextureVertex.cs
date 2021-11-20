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
    public struct LitTextureVertex : IVertex
    {
        private Vec3 _position;
        private Vec3 _normal;
        private Vec2 _textureCoordinate;
        private float _ambientOcclusion;
        private float _reflectivity;

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
        public float AmbientOcclusion
        {
            get
            {
                return _ambientOcclusion;
            }
            set
            {
                _ambientOcclusion = value;
            }
        }
        public float Reflectivity
        {
            get
            {
                return _reflectivity;
            }
            set
            {
                _reflectivity = value;
            }
        }

        public LitTextureVertex(float x, float y, float z, float nx, float ny, float nz, float tx, float ty, float ao, float r)
        {
            _position = new Vec3(x, y, z);
            _normal = new Vec3(nx, ny, nz);
            _textureCoordinate = new Vec2(tx, ty);
            _ambientOcclusion = ao;
            _reflectivity = r;
        }

        public LitTextureVertex(Vec3 v, Vec3 n, Vec2 t, float ao, float r)
        {
            _position = v;
            _normal = n;
            _textureCoordinate = t;
            _ambientOcclusion = ao;
            _reflectivity = r;
        }
    }
}
