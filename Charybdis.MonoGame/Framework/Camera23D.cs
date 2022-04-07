/*
 * This version of Camera2 lives in 3D space, and actually uses 3D height and math to zoom, it works fine, but this complicates rendering and I was unable to sort that out easily so am trying
 * another approach. Leaving this here commented out in case I decide to revisit, or to use this for another project in the future. -UdderlyEvelyn 4/7/22
 * 
 */

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Xna.Framework;
//using Vec2 = Microsoft.Xna.Framework.Vector2;
//using Vec3 = Microsoft.Xna.Framework.Vector3;
//using Microsoft.Xna.Framework.Graphics;
//using Charybdis.Library.Core;

//namespace Charybdis.MonoGame
//{
//    public class Camera2
//    {
//        public Vec2 Position;
//        private float z;

//        public float Z
//        {
//            get => z;
//            set
//            {
//                z = value;
//                Maths.Clamp(z, MinZ, MaxZ);
//                Update();
//            }
//        }
//        public float AspectRatio;
//        public float FOV;
//        public Matrix View;
//        public Matrix Projection;
//        private float defaultZForResolution;
//        public float MinZ = 1f;
//        public float MaxZ = 2048f;
//        private Viewport viewport;

//        public Camera2(Viewport viewport)
//        {
//            this.viewport = viewport;
//            var height = viewport.Height;
//            Position = new Vec2(viewport.X, viewport.Y);
//            AspectRatio = (float)viewport.Width / height;
//            FOV = MathHelper.PiOver2;
//            Z = defaultZForResolution = -(.5f * height) / MathF.Tan(.5f * FOV);
//            Update();
//        }

//        public void Update()
//        {
//            View = Matrix.CreateLookAt(new Vec3(0, 0, Z), Vec3.Zero, Vec3.Down); //X/Y Positive should be down and to the right, so Up = Down.
//            Matrix.CreatePerspectiveFieldOfView(FOV, AspectRatio, MinZ, MaxZ, out Projection);
//        }
//    }
//}