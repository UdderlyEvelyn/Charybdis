using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;

namespace Charybdis.MonoGame
{
    public class Camera2
    {
        public Vec2 Position;
        public float Zoom = 1f;
        //public float Rotation = 0f;
        public int Width;
        public int Height;
        public float HalfWidth;
        public float HalfHeight;
        public float AspectRatio;
        public float FOV;
        public float MinZ = 1f;
        public float MaxZ = 2048f;
        public Matrix Translation;
        public Matrix InverseTranslation;

        //These are in screenspace.
        public Vec2 CullingPosition;
        public Vec2 CullingExtent;
        public float CullingPaddingFactor = .15f;

        public Camera2(int width, int height)
        {
            Width = width;
            Height = height;
            HalfWidth = width * .5f;
            HalfHeight = height * .5f;
            Position = Vec2.Zero;
            Update();
        }

        public void Update()
        {
            CullingPosition = new Vec2(-CullingPaddingFactor);
            CullingExtent = new Vec2(Width + Width * CullingPaddingFactor, Height + Height * CullingPaddingFactor);
            Translation = Matrix.CreateTranslation((int)-Position.X, (int)-Position.Y, 0) //Handles shifting things into view with the position of the camera.
                                                                                          //* Matrix.CreateRotationZ(Rotation) //Handles rotation.
                        * Matrix.CreateScale(new Vec3(Zoom, Zoom, 1f)) //Handles zoom via scaling.
                        * Matrix.CreateTranslation(new Vec3(HalfWidth, HalfHeight, 0)); //Accounts for camera center vs. viewport center (otherwise zooming comes from the camera position in the top-left corner).
            InverseTranslation = Matrix.Invert(Translation);
        }

        public Vec2 WorldToScreen(Vec2 position)
        {
            return Vec2.Transform(position, Translation);
        }

        public Vec2 ScreenToWorld(Vec2 position)
        {
            return Vec2.Transform(position, InverseTranslation);
        }

        public bool WithinView(Vec2 worldPosition)
        {
            return WorldToScreen(worldPosition).Within(CullingPosition, CullingExtent);
        }

        //public void Update()
        //{
        //    View = Matrix.CreateLookAt(new Vec3(0, 0, Z), Vec3.Zero, Vec3.Down); //X/Y Positive should be down and to the right, so Up = Down.
        //    Matrix.CreatePerspectiveFieldOfView(FOV, AspectRatio, MinZ, MaxZ, out Projection);
        //}
    }
}