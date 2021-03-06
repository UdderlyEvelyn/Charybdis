using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.MonoGame;
using Charybdis.Library.Core;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Kolony
{
    public class CubeCoordinates
    {
        private static Dictionary<Vec2, CubeCoordinates> precalculatedCoordinatesPerCubeSize = new();

        public static CubeCoordinates Get(Vec2 position, Vec2 size, Vec2 scale, Vec2 origin)
        {
            var scaledSize = size * scale;
            CubeCoordinates precalculated = null;
            if (!precalculatedCoordinatesPerCubeSize.ContainsKey(scaledSize))
                precalculated = precalculatedCoordinatesPerCubeSize[scaledSize] = new CubeCoordinates(scaledSize, origin);
            else
                precalculated = precalculatedCoordinatesPerCubeSize[scaledSize];
            return new CubeCoordinates(position, precalculated);
        }

        public float X;
        public float Y;
        public float W;
        public float H;

        public Vec2 TopLeft;
        public Vec2 TopRight;
        public Vec2 BottomLeft;
        public Vec2 BottomRight;

        public Vec2 Center;

        public Vec2 TopFaceTop;
        public Vec2 TopFaceLeft;
        public Vec2 TopFaceRight;
        public Vec2 TopFaceBottom;

        public List<Vec2> TopFaceVertices;

        public Vec2 TopFaceCenter;

        public Vec2 LeftFaceTopLeft;
        public Vec2 LeftFaceTopRight;
        public Vec2 LeftFaceBottomLeft;
        public Vec2 LeftFaceBottomRight;

        public List<Vec2> LeftFaceVertices;

        public Vec2 LeftFaceCenter;

        public Vec2 RightFaceTopLeft;
        public Vec2 RightFaceTopRight;
        public Vec2 RightFaceBottomLeft;
        public Vec2 RightFaceBottomRight;

        public List<Vec2> RightFaceVertices;

        public Vec2 RightFaceCenter;

        public List<Vec2> CubeBorderVertices;

        /// <summary>
        /// Explicitly generates the coordinates for a specific size, position, and origin.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public CubeCoordinates(Vec2 position, Vec2 size, Vec2 origin)
        {
            X = position.X - origin.X;
            Y = position.Y - origin.Y;
            W = size.X;
            H = size.Y;
            Center = new Vec2(X + (W * .5f), Y + (H * .5f));
            BottomRight = new Vec2(X + W, Y + H);
            TopLeft = position;
            TopRight = new Vec2(BottomRight.X, Y);
            BottomLeft = new Vec2(X, BottomRight.Y);
            TopFaceTop = new Vec2(Center.X, Y);
            TopFaceLeft = new Vec2(X, Y + (H * .25f));
            TopFaceCenter = new Vec2(TopFaceTop.X, TopFaceLeft.Y);
            TopFaceRight = new Vec2(X + W, TopFaceLeft.Y);
            TopFaceBottom = Center;
            LeftFaceTopLeft = TopFaceLeft;
            RightFaceTopRight = TopFaceRight;
            LeftFaceTopRight = Center;
            RightFaceTopLeft = Center;
            LeftFaceBottomLeft = new Vec2(X, Y + (H * .75f));
            LeftFaceBottomRight = new Vec2(Center.X, BottomRight.Y);
            RightFaceBottomLeft = LeftFaceBottomRight;
            RightFaceBottomRight = new Vec2(BottomRight.X, LeftFaceBottomLeft.Y);
            LeftFaceCenter = new Vec2(X + (W * .25f), Y + (H * .625f));
            RightFaceCenter = new Vec2(X + (W * .75f), LeftFaceCenter.Y);
            TopFaceVertices = new List<Vec2>
            {
                TopFaceLeft, TopFaceTop, TopFaceRight, TopFaceBottom, TopFaceLeft
            };
            LeftFaceVertices = new List<Vec2>
            {
                LeftFaceTopLeft, LeftFaceTopRight, LeftFaceBottomRight, LeftFaceBottomLeft, LeftFaceTopLeft
            };
            RightFaceVertices = new List<Vec2>
            {
                RightFaceTopLeft, RightFaceTopRight, RightFaceBottomRight, RightFaceBottomLeft, RightFaceTopLeft
            };
            CubeBorderVertices = new List<Vec2>
            {
                TopFaceTop, TopFaceRight, RightFaceBottomRight, RightFaceBottomLeft, LeftFaceBottomLeft, TopFaceLeft, TopFaceTop
            };
        }

        /// <summary>
        /// Generates the generic coordinates for a given cube size and origin.
        /// </summary>
        /// <param name="size"></param>
        public CubeCoordinates(Vec2 size, Vec2 origin) : this(Vec2.Zero, size, origin)
        {

        }

        /// <summary>
        /// Applies the position to the precalculated offsets to produce cube coordinates.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="precalculatedOffsets"></param>
        public CubeCoordinates(Vec2 position, CubeCoordinates precalculatedOffsets)
        {
            X = precalculatedOffsets.X + position.X;
            Y = precalculatedOffsets.Y + position.Y;
            W = precalculatedOffsets.W;
            H = precalculatedOffsets.H;
            Center = precalculatedOffsets.Center + position;
            BottomRight = precalculatedOffsets.BottomRight + position;
            TopLeft = position;
            TopRight = precalculatedOffsets.TopRight + position;
            BottomLeft = precalculatedOffsets.BottomLeft + position;
            TopFaceTop = precalculatedOffsets.TopFaceTop + position;
            TopFaceLeft = precalculatedOffsets.TopFaceLeft + position;
            TopFaceCenter = new Vec2(TopFaceTop.X, TopFaceLeft.Y);
            TopFaceRight = precalculatedOffsets.TopFaceRight + position;
            TopFaceBottom = Center;
            LeftFaceTopLeft = TopFaceLeft;
            RightFaceTopRight = TopFaceRight;
            LeftFaceTopRight = Center;
            RightFaceTopLeft = Center;
            LeftFaceBottomLeft = precalculatedOffsets.LeftFaceBottomLeft + position;
            LeftFaceBottomRight = new Vec2(Center.X, BottomRight.Y);
            RightFaceBottomLeft = LeftFaceBottomRight;
            RightFaceBottomRight = new Vec2(BottomRight.X, LeftFaceBottomLeft.Y);
            LeftFaceCenter = precalculatedOffsets.LeftFaceCenter + position;
            RightFaceCenter = precalculatedOffsets.RightFaceCenter + position;
            TopFaceVertices = new List<Vec2>
            {
                TopFaceLeft, TopFaceTop, TopFaceRight, TopFaceBottom, TopFaceLeft
            };
            LeftFaceVertices = new List<Vec2>
            {
                LeftFaceTopLeft, LeftFaceTopRight, LeftFaceBottomRight, LeftFaceBottomLeft, LeftFaceTopLeft
            };
            RightFaceVertices = new List<Vec2>
            {
                RightFaceTopLeft, RightFaceTopRight, RightFaceBottomRight, RightFaceBottomLeft, RightFaceTopLeft
            };
            CubeBorderVertices = new List<Vec2>
            {
                TopFaceTop, TopFaceRight, RightFaceBottomRight, RightFaceBottomLeft, LeftFaceBottomLeft, TopFaceLeft, TopFaceTop
            };
        }

        public override string ToString()
        {
            return "Cube Coordinates for " + X + ", " + Y + " (" + W + "x" + H + ")\n" +
                   "Center: " + Center + "\n" +
                   "Top Face Top: " + TopFaceTop + "\n" +
                   "Top Face Left: " + TopFaceLeft + "\n" +
                   "Top Face Right: " + TopFaceRight + "\n" +
                   "Top Face Bottom: " + TopFaceBottom + "\n" +
                   "Top Face Center: " + TopFaceCenter + "\n" +
                   "Left Face Top Left: " + LeftFaceTopLeft + "\n" +
                   "Left Face Bottom Left: " + LeftFaceBottomLeft + "\n" +
                   "Left Face Top Right: " + LeftFaceTopRight + "\n" +
                   "Left Face Bottom Right: " + LeftFaceBottomRight + "\n" +
                   "Left Face Center: " + LeftFaceCenter + "\n" +
                   "Right Face Top Left: " + RightFaceTopLeft + "\n" +
                   "Right Face Bottom Left: " + RightFaceBottomLeft + "\n" +
                   "Right Face Top Right: " + RightFaceTopRight + "\n" +
                   "Right Face Bottom Right: " + RightFaceBottomRight + "\n" +
                   "Right Face Center: " + RightFaceCenter;
        }

        public bool WithinTopFaceAlt(Vec2 point)
        {
            return Collision.PointInPolygon(point, TopFaceVertices);
        }

        public bool WithinLeftFace(Vec2 point)
        {
            return Collision.PointInPolygon(point, LeftFaceVertices);
        }

        public bool WithinRightFace(Vec2 point)
        {
            return Collision.PointInPolygon(point, RightFaceVertices);
        }

        public bool WithinCube(Vec2 point)
        {
            return Collision.PointInPolygon(point, CubeBorderVertices);
        }

        //This does the same thing TransformToTopFaceBottomRightQuadrantRelativeSpace does built in,
        //and then lerps the Y value based on the X value between the possible X value range, and checks if the Y value falls in that valid range.
        //This is cheaper than doing the traditional ray test, but it requires mirror symmetry on both X and Y axes - works for the top face.
        public bool WithinTopFace(Vec2 point)
        {
            return MathF.Abs(point.Y - TopFaceCenter.Y) <= Maths.Lerp(TopFaceCenter.Y - TopFaceTop.Y, 0, MathF.Abs(point.X - TopFaceCenter.X) / (TopFaceRight.X - TopFaceCenter.X));
        }

        //This changes the coordinates to be relative to the center of the top face, and the Abs calls make it so that it doesn't differentiate between quadrants. Making each Abs result negative would make it be top left quadrant but that would be more expensive.
        //The reason to do this is to effectively fold the coordinate space of the top face vertically and horizontally and only have to work with a single triangle.
        //public Vec2 TransformToTopFaceBottomRightQuadrantRelativeSpace(Vec2 point)
        //{
        //    return new Vec2(MathF.Abs(point.X - TopFaceCenter.X), MathF.Abs(point.Y - TopFaceCenter.Y));
        //}
    }
}