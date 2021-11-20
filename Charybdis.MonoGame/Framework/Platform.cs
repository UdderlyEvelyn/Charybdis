using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.MonoGame
{
    public static class Platform
    {
        public enum APIs
        {
            OpenTK,
            SharpDX,
        }

        private static APIs _api;
        public static APIs API
        {
            get
            {
                return _api;
            }
            set
            {
                switch (value)
                {
                    case APIs.OpenTK:
                        OpenGL = true;
                        DirectX = false;
                        break;
                    case APIs.SharpDX:
                        OpenGL = false;
                        DirectX = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(message: "Unknown API assigned to Platform.API.", innerException: null);
                }
                _api = value;
            }
        }

        public static bool DirectX = false;
        public static bool OpenGL = false;
    }
}
