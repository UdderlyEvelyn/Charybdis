//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Charybdis.MonoGame
//{
//    public class ShadowCamera : Camera
//    {
//        private RenderTarget2D _renderTarget;
//        public RenderTarget2D RenderTarget
//        {
//            get
//            {
//                return _renderTarget;
//            }
//        }

//        private Texture2D _shadowMap;
//        public Texture2D ShadowMap
//        {
//            get
//            {
//                float[] data = new float[_renderTarget.Width * _renderTarget.Height];
//                _renderTarget.GetData<float>(data);
//                _shadowMap.SetData<float>(data);
//                return _shadowMap;
//            }
//        }

//        public ShadowCamera(Game game) : base(game)
//        {
//            _renderTarget = new RenderTarget2D(Globals.GraphicsDevice, Globals.Width, Globals.Height, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
//            _shadowMap = new Texture2D(Globals.GraphicsDevice, Globals.Width, Globals.Height);
//        }
//    }
//}
