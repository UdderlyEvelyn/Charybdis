/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Charybdis.MonoGame.Datatypes;

namespace Charybdis.MonoGame.Framework
{
    public class GLCamera
    {
        const float MIN_TILT = 1;
        const float MAX_TILT = 89;

        //private GameWindow _window;

        private Vec3 _right = Vec3.Right;
        private Vec3 _up = Vec3.Up;
        private Vec3 _look = Vec3.UnitZ;
        private Vec3 _position = new Vec3(0, 100, 0);

        private float _xRot = 3.14f / 4;
        private float _yRot = 0;
        private float _zRot = 0;

        private Matrix _perspective;
        private Frustum _frustum;

        public Frustum Frustum
        {
            get
            {
                return _frustum;
            }
        }        

        public float CamX
        {
            get
            {
                return _position.X;
            }
            set
            {
                _position.X = value;
            }
        }
        public float CamY
        {
            get
            {
                return _position.Y;
            }
            set
            {
                _position.Y = value;
            }
        }
        public float CamZ
        {
            get
            {
                return _position.Z;
            }
            set
            {
                _position.Z = value;
            }
        }
        public float CamRotX
        {
            get
            {
                return _xRot;
            }
            set
            {
                _xRot = value;
            }
        }
        public float CamRotY
        {
            get
            {
                return _yRot;
            }
            set
            {
                _yRot = value;
            }
        }
        public float CamRotZ
        {
            get
            {
                return _zRot;
            }
            set
            {
                _zRot = value;
            }
        }
        public Vec3 Position
        {
            get
            {
                return -_position;
            }
            set
            {
                _position = -value;
            }
        }
        public Vec3 Look
        {
            get
            {
                return -_look;
            }
        }
        public Vec3 Up
        {
            get
            {
                return _up;
            }
        }
        public Vec3 Right
        {
            get
            {
                return _right;
            }
        }
        private float _fov;
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
                    _perspective = Matrix.CreatePerspectiveFieldOfView(FOV, Aspect, NearClip, FarClip);
            }
        }

        private float _nearClip;
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
                    _perspective = Matrix.CreatePerspectiveFieldOfView(FOV, Aspect, NearClip, FarClip);
            }
        }
        private float _farClip;
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
                    _perspective = Matrix.CreatePerspectiveFieldOfView(FOV, Aspect, NearClip, FarClip);
            }
        }
        private float _aspect;
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
                    _perspective = Matrix.CreatePerspectiveFieldOfView(FOV, Aspect, NearClip, FarClip);
            }
        }

        private Matrix _viewMatrix;
        public Matrix View
        {
            get
            {
                return _viewMatrix;
            }
        }

        public void RecalculateViewMatrix()
        {
            //Reset
            _right = Vec3.Right;
            _up = Vec3.Up;
            _look = Vec3.UnitZ;
            //Create
            Matrix yawMatrix = Matrix.CreateFromAxisAngle(_up, (float)Common.degToRad(_yRot));
            _look = Vector3.Transform(_look, yawMatrix);
            _right = Vector3.Transform(_right, yawMatrix);
            Matrix pitchMatrix = Matrix.CreateFromAxisAngle(_right, (float)Common.degToRad(_xRot));
            _look = Vector3.Transform(_look, pitchMatrix);
            _up = Vector3.Transform(_up, pitchMatrix);
            Matrix rollMatrix = Matrix.CreateFromAxisAngle(_look, (float)Common.degToRad(_zRot));
            _look = Vector3.Transform(_look, rollMatrix);
            _right = Vector3.Transform(_right, rollMatrix);
            Matrix viewMatrix = Matrix.Identity;
            viewMatrix.M11 = _right.X;
            viewMatrix.M12 = _up.X;
            viewMatrix.M13 = _look.X;
            viewMatrix.M21 = _right.Y;
            viewMatrix.M22 = _up.Y;
            viewMatrix.M23 = _look.Y;
            viewMatrix.M31 = _right.Z;
            viewMatrix.M32 = _up.Z;
            viewMatrix.M33 = _look.Z;
            viewMatrix.M41 = Vector3.Dot(_position, _right);
            viewMatrix.M42 = Vector3.Dot(_position, _up);
            viewMatrix.M43 = Vector3.Dot(_position, _look);
            _viewMatrix = viewMatrix;
            //_frustum = Frustum.FromMatrix(_viewMatrix.Inverted() * _perspective);
            _frustum = Frustum.FromMatrices(_viewMatrix.Inverted(), _perspective);
            //_frustum = Frustum.FromMatrix2(_viewMatrix.Inverted());
        }

        public Matrix Orientation;

        public Matrix Perspective
        {
            get
            {
                return _perspective; 
            }
        }

        public Camera(GameWindow window)
        {
            _window = window;
            //Reset
            _right = Vec3.Right;
            _up = Vec3.Up;
            _look = Vec3.UnitZ;
            //Create
            Matrix yawMatrix = Matrix.CreateRotationY(0);
            _look = Vector3.TransformNormal(_look, yawMatrix);
            _right = Vector3.TransformNormal(_right, yawMatrix);
            Matrix pitchMatrix = Matrix.CreateRotationX(0);
            _look = Vector3.TransformNormal(_look, pitchMatrix);
            _up = Vector3.TransformNormal(_up, pitchMatrix);
            Matrix rollMatrix = Matrix.CreateRotationZ(0);
            _look = Vector3.TransformNormal(_look, rollMatrix);
            _right = Vector3.TransformNormal(_right, rollMatrix);
            Orientation = Matrix.Identity;
            Orientation.M11 = _right.X;
            Orientation.M12 = _up.X;
            Orientation.M13 = _look.X;
            Orientation.M21 = _right.Y;
            Orientation.M22 = _up.Y;
            Orientation.M23 = _look.Y;
            Orientation.M31 = _right.Z;
            Orientation.M32 = _up.Z;
            Orientation.M33 = _look.Z;
            Orientation.M41 = Vector3.Dot(_position, _right);
            Orientation.M42 = Vector3.Dot(_position, _up);
            Orientation.M43 = Vector3.Dot(_position, _look);
        }

        public void UpdateLook()
        {
            Matrix view = View;            
        }

        private float moveSpeed = 1f;
        private float rotationSpeed = 1.5f;
        private float mouseSpeed = 8f;

        public void TurnLeft(float amount = 1, bool mouse = false)
        {
            if (_yRot > 0)
                _yRot -= amount * (mouse ? mouseSpeed : rotationSpeed);
            else _yRot = 360;
        }

        public void TurnRight(float amount = 1, bool mouse = false)
        {
            if (_yRot < 360)
                //if ((_yRot + amount * (mouse ? mouseSpeed : rotationSpeed)) > 0)
                    _yRot += amount * (mouse ? mouseSpeed : rotationSpeed);
                //else _yRot = 359;
            else _yRot = 0;
        }

        public void Turn(float amount)
        {
            TurnRight(-amount * Aspect, true);
        }

        public void TiltUp(float amount = 1, bool mouse = false)
        {
            if (_xRot < 89 || mouse)
                _xRot += amount * (mouse ? mouseSpeed : rotationSpeed);
            else _xRot = 89;
        }

        public void TiltDown(float amount = 1, bool mouse = false)
        {
            if (_xRot > -89)
                _xRot -= amount * (mouse ? mouseSpeed : rotationSpeed);
            else _xRot = -89;
        }

        public void Tilt(float amount)
        {
            if (_xRot - amount * mouseSpeed > 89) _xRot = 89;
            else if (_xRot - amount * mouseSpeed < -89) _xRot = -89;
            else _xRot -= amount * mouseSpeed;
        }

        public void MoveForward()
        {
            _position += (_look * moveSpeed);
        }

        public void MoveBackward()
        {
            _position -= (_look * moveSpeed);
        }

        public void MoveLeft()
        {
            _position += (_right * moveSpeed);
        }

        public void MoveRight()
        {
            _position -= (_right * moveSpeed);
        }

        public void MoveUp()
        {
            _position += (Vec3.Up * moveSpeed);
        }

        public void MoveDown()
        {
            _position -= (Vec3.Up * moveSpeed);
        }
    }
}
*/