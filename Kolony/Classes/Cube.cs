using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Charybdis.Science;
using Charybdis.MonoGame;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Kolony
{
    public class Cube : KolonyObject
    {
        public float TMP_Density = 0;
        public Vec3 GridPosition;

        public Cube()
        {
            SelectionEnabled = true;
        }

        public Vec3 WorldPosition
        {
            get
            {
                return new Vec3(Position, Depth);
            }
        }

        public float Depth;

        public Material Material;

        public override void Update()
        {
            /*Nothing to see here.*/
        }

        public CubeCoordinates Coordinates;
    }
}