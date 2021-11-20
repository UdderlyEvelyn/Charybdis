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
    public struct ColorVertex : IVertexType, IVertex
    {
        [DataMember]
        private Vec3 _position;
        [DataMember]
        private Color _color;

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
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
        VertexDeclaration IVertex.VertexDeclaration { get { return VertexDeclaration; } }
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

        public ColorVertex(float x, float y, float z, float r, float g, float b, float a)
        {
            _position = new Vec3(x, y, z);
            _color = new Col4(r, g, b, a);
        }

        public ColorVertex(float x, float y, float z, float r, float g, float b)
        {
            _position = new Vec3(x, y, z);
            _color = new Col3(r, g, b);
        }

        public ColorVertex(Vec3 v, Col4 c)
        {
            _position = v;
            _color = c;
        }

        public static implicit operator VertexPositionColor(ColorVertex cv)
        {
            return new VertexPositionColor(cv.Position, cv.Color);
        }

        public static implicit operator ColorVertex(VertexPositionColor vpc)
        {
            return new ColorVertex(vpc.Position, vpc.Color);
        }
    }
}
