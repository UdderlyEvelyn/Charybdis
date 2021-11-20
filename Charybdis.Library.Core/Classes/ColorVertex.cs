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
    public struct ColorVertex : IVertex
    {
        private Vec3 _position;
        private Col3 _color;

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

        public Col3 Color
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
    }
}
