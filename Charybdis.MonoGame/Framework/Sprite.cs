using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using GraphicsDevice = Microsoft.Xna.Framework.Graphics.GraphicsDevice;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;
using System.Runtime.CompilerServices;

namespace Charybdis.MonoGame
{
    public class Sprite : Drawable2
    {
        public Sprite(Texture2D data)
        {
            _data = data;
            Size = new Vec2(data.Width * Scale.X, data.Height * Scale.Y);
            DrawChildren = true;
            BoundingBox = new BoundingBox(new Microsoft.Xna.Framework.Vector3(Position.X, Position.Y, 0), new Microsoft.Xna.Framework.Vector3(Position.X + Size.X, Position.Y + Size.Y, 0));
        }

        public BoundingBox BoundingBox;
        
        public float Rotation = 0;

        public bool PositionBasedDepthEnabled = false;

        public float Depth = 0;

        public Vec2 Scale = Vec2.One;

        private Texture2D _data;

        public Microsoft.Xna.Framework.Graphics.SpriteEffects Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetPositionBasedDepth(Vec2 offset)
        {
            return Position.Distance(offset) + Depth;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vec2 offset)
        {
            if (!DrawMe)
                return; //Set to not draw, so abort the drawing process.        
            spriteBatch.Draw(_data,
                position: Position.ToXNA(),
                sourceRectangle: null,//new Microsoft.Xna.Framework.Rectangle(Position.Xi, Position.Yi, Size.Xi, Size.Yi),
                color: Tint.HasValue ? Tint.Value.ToXNA() : Microsoft.Xna.Framework.Color.White,
                rotation: Rotation,
                origin: offset.ToXNA(),
                scale: Scale.ToXNA(),
                effects: Effects,
                layerDepth: PositionBasedDepthEnabled ? GetPositionBasedDepth(offset) : Depth);
            if (DrawChildren)
                foreach (Drawable2 d2 in Children)
                    d2.Draw(spriteBatch, offset);
        }

        public Col4? Tint;
    }
}
