using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using Charybdis.MonoGame.Framework;

namespace Charybdis.MonoGame.Datatypes
{
    [DataContract, StructLayout(LayoutKind.Sequential)]
    public struct LitTextureVertex : IVertexType, IVertex
    {
        [DataMember]
        private Vec3 _position;
        [DataMember]
        private Vec3 _normal;
        [DataMember]
        private Vec2 _textureCoordinate;
        [DataMember]
        private float _ambientOcclusion;
        [DataMember]
        private float _reflectivity;
        //[DataMember]
        //private float _transparencyEnable;

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
        /*public float TransparencyEnable
        {
            get
            {
                return _transparencyEnable;
            }
            set
            {
                _transparencyEnable = value;
            }
        }*/

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
        VertexDeclaration IVertex.VertexDeclaration { get { return VertexDeclaration; } }
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3)
            //new VertexElement(40, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 4)
        );

        public LitTextureVertex(float x, float y, float z, float nx, float ny, float nz, float tx, float ty, float ao, float r)//, bool transparency)
        {
            _position = new Vec3(x, y, z);
            _normal = new Vec3(nx, ny, nz);
            _textureCoordinate = new Vec2(tx, ty);
            _ambientOcclusion = ao;
            _reflectivity = r;
            //_transparencyEnable = transparency ? 1 : 0;
        }

        public LitTextureVertex(Vec3 v, Vec3 n, Vec2 t, float ao, float r)//, bool transparency)
        {
            _position = v;
            _normal = n;
            _textureCoordinate = t;
            _ambientOcclusion = ao;
            _reflectivity = r;
            //_transparencyEnable = transparency ? 1 : 0;
        }
    }
}
