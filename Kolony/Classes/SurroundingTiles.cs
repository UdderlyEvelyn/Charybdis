using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Kolony
{
    public class SurroundingTiles
    {
        public static int TileSize;
        public static int WorldTileWidth;
        public static int WorldTileHeight;

        public Cube TopLeft;
        public Cube TopRight;
        public Cube TopCenter;
        public Cube Left;
        public Cube Center;
        public Cube Right;
        public Cube BottomLeft;
        public Cube BottomCenter;
        public Cube BottomRight;

        public List<Cube> All;

        public int X;
        public int Y;

        public SurroundingTiles(Array2<Cube> world, Cube t)
        {
            //TL TC TR
            // L  C  R
            //BL BC BR
            Center = t;
            //Origin
            X = (int)(t.Position.X / TileSize);
            Y = (int)(t.Position.Y / TileSize);
            //Tile Potential
            bool TopCenterPossible = Y > 0;
            bool LeftPossible = X > 0;
            bool RightPossible = X < (WorldTileWidth - 1);
            bool BottomCenterPossible = Y < (WorldTileHeight - 1);
            bool TopLeftPossible = X > 0 && Y > 0;
            bool TopRightPossible = X < (WorldTileWidth - 1) && Y > 0;
            bool BottomLeftPossible = X > 0 && Y < (WorldTileHeight - 1);
            bool BottomRightPossible = X < (WorldTileWidth - 1) && Y < (WorldTileHeight - 1);
            //Find Orthogonal Tiles
            if (TopCenterPossible)
                TopCenter = world.Get(X, Y - 1);
            if (LeftPossible)
                Left = world.Get(X - 1, Y);
            if (RightPossible)
                Right = world.Get(X + 1, Y);
            if (BottomCenterPossible)
                BottomCenter = world.Get(X, Y + 1);
            //Find Diagonal Tiles
            if (TopLeftPossible)
                TopLeft = world.Get(X - 1, Y - 1);
            if (TopRightPossible)
                TopRight = world.Get(X + 1, Y - 1);
            if (BottomLeftPossible)
                BottomLeft = world.Get(X - 1, Y + 1);
            if (BottomRightPossible)
                BottomRight = world.Get(X + 1, Y + 1);
            //Build List
            All = new List<Cube>
                {
                    TopLeft,
                    TopRight,
                    TopCenter,
                    Left,
                    Right,
                    BottomLeft,
                    BottomRight,
                    BottomCenter,
                };
        }
    }
}