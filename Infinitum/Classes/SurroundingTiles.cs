using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;

namespace Infinitum
{
    public class SurroundingTiles
    {
        public static int TileSize;
        public static int WorldTileWidth;
        public static int WorldTileHeight;

        public Tile TopLeft;
        public Tile TopRight;
        public Tile TopCenter;
        public Tile Left;
        public Tile Center;
        public Tile Right;
        public Tile BottomLeft;
        public Tile BottomCenter;
        public Tile BottomRight;

        public List<Tile> All;

        public int X;
        public int Y;

        public SurroundingTiles(Array2<Tile> world, Tile t)
        {
            //TL TC TR
            // L  C  R
            //BL BC BR
            Center = t;
            //Origin
            X = (int)(t.Visual.Position.X / TileSize);
            Y = (int)(t.Visual.Position.Y / TileSize);
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
            All = new List<Tile>
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