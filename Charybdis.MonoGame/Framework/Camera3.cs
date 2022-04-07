using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ray = Microsoft.Xna.Framework.Ray;
using XNARectangle = Microsoft.Xna.Framework.Rectangle;
using Charybdis.Library.Core;

namespace Charybdis.MonoGame
{
    public class Camera3 : GameComponent
    {
        protected Vec3 _look = Vec3.Zero;
        protected Vec3 _position = Vec3.Zero;
        protected Vec3 _rotation = Vec3.Zero;

        protected BoundingFrustum _nearFrustum;
        protected BoundingFrustum _frustum;
        protected BoundingFrustum _farFrustum;

        public BoundingFrustum NearFrustum
        {
            get
            {
                return _nearFrustum;
            }
        }

        public BoundingFrustum Frustum
        {
            get
            {
                return _frustum;
            }
        }

        public BoundingFrustum FarFrustum
        {
            get
            {
                return _farFrustum;
            }
        }

        protected float _fov;
        public float FOV
        {
            get
            {
                return _fov;
            }
            set
            {
                _fov = value;
                if (FOV > 0 && Aspect > 0 && NearClip > 0 && FarClip > 0)
                {
                    Perspective = Matrix.CreatePerspectiveFieldOfView(FOV, Aspect, NearClip, FarClip);
                }
            }
        }

        protected float _nearClip;
        public float NearClip
        {
            get
            {
                return _nearClip;
            }
            set
            {
                _nearClip = value;
                if (FOV > 0 && Aspect > 0 && NearClip > 0 && FarClip > 0)
                {
                    Perspective = Matrix.CreatePerspectiveFieldOfView(FOV, Aspect, NearClip, FarClip);
                }
            }
        }
        protected float _farClip;
        public float FarClip
        {
            get
            {
                return _farClip;
            }
            set
            {
                _farClip = value;
                if (FOV > 0 && Aspect > 0 && NearClip > 0 && FarClip > 0)
                {
                    Perspective = Matrix.CreatePerspectiveFieldOfView(FOV, Aspect, NearClip, FarClip);
                }
            }
        }
        protected float _aspect;
        public float Aspect
        {
            get
            {
                return _aspect;
            }
            set
            {
                _aspect = value;
                if (FOV > 0 && Aspect > 0 && NearClip > 0 && FarClip > 0)
                {
                    Perspective = Matrix.CreatePerspectiveFieldOfView(FOV, Aspect, NearClip, FarClip);
                }
            }
        }

        public float X
        {
            get
            {
                return _position.X;
            }
        }
        public float Y
        {
            get
            {
                return _position.Y;
            }
        }
        public float Z
        {
            get
            {
                return _position.Z;
            }
        }
        public float Pitch
        {
            get
            {
                return _rotation.X;
            }
        }
        public float Yaw
        {
            get
            {
                return _rotation.Y;
            }
        }
        public float Roll
        {
            get
            {
                return _rotation.Z;
            }
        }

        public Vec3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                UpdateLookAt();
            }
        }

        public Vec3 Rotation
        {
            get
            {
                return _rotation;
            }

            set
            {
                _rotation = value;
                UpdateLookAt();
            }
        }

        public Vec3 Look
        {
            get
            {
                return _look;
            }
        }
        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(_position.ToXNA(), _look.ToXNA(), Vec3.Up.ToXNA());
            }
        }

        public Matrix Perspective;

        public void UpdateLookAt()
        {
            _look = _position + Vector3.Transform(Vec3.UnitZ.ToXNA(), Matrix.CreateRotationX(_rotation.X) * Matrix.CreateRotationY(_rotation.Y)).ToCharybdis();
            GenerateFrustum();
        }

        public Camera3(Game game) : base(game)
        {

        }

        public void GenerateFrustum()
        {
            _nearFrustum = new BoundingFrustum(Matrix.CreateOrthographic(4096, 4096, 0, 1000));
            _frustum = new BoundingFrustum(View * Perspective); 
            _farFrustum = new BoundingFrustum(Matrix.CreateOrthographic(1024, 1024, 1000, 10000));
        }

        //Sourced (including comments) from http://ghoscher.me/2010/12/09/xna-picking-tutorial-part-ii/
        public BoundingFrustum UnprojectRectangle(XNARectangle source, Viewport viewport)
        {
            //http://forums.create.msdn.com/forums/p/6690/35401.aspx , by "The Friggm"    
            // Many many thanks to him...     
            // Point in screen space of the center of the region selected    
            Vector2 regionCenterScreen = new Vector2(source.Center.X, source.Center.Y);
            // Generate the projection matrix for the screen region    
            Matrix regionProjMatrix = Perspective;
            // Calculate the region dimensions in the projection matrix. M11 is inverse of width, M22 is inverse of height.    
            regionProjMatrix.M11 /= ((float)source.Width / (float)viewport.Width);
            regionProjMatrix.M22 /= ((float)source.Height / (float)viewport.Height);
            // Calculate the region center in the projection matrix. M31 is horizonatal center.    
            regionProjMatrix.M31 = (regionCenterScreen.X - (viewport.Width / 2f)) / ((float)source.Width / 2f);
            // M32 is vertical center. Notice that the screen has low Y on top, projection has low Y on bottom.    
            regionProjMatrix.M32 = -(regionCenterScreen.Y - (viewport.Height / 2f)) / ((float)source.Height / 2f);
            return new BoundingFrustum(View * regionProjMatrix);
        }

        public Ray UnprojectPoint(Vector2 point, Viewport viewport)
        {
            Vector3 nearPoint = new Vector3(point, 0);
            Vector3 farPoint = new Vector3(point, 1);
            nearPoint = viewport.Unproject(nearPoint, Perspective, View, Matrix.Identity);
            farPoint = viewport.Unproject(farPoint, Perspective, View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }
    }
}
