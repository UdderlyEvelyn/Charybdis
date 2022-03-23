using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using GraphicsDevice = Microsoft.Xna.Framework.Graphics.GraphicsDevice;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;

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
        
        public float? Rotation;

        public Vec2 Scale = Vec2.One;

        private Texture2D _data;

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vec2 offset)
        {
            if (!DrawMe)
                return; //Set to not draw, so abort the drawing process.            
            var pos = (Position - offset).ToXNA();
            if (Rotation.HasValue)
#pragma warning disable CS0618 // Type or member is obsolete
                spriteBatch.Draw(_data,
                    position: pos,
                    color: Tint.HasValue ? Tint.Value.ToXNA() : Microsoft.Xna.Framework.Color.White,
                    rotation: Rotation.Value,
                    scale: Scale.ToXNA(),
                    origin: pos + (Size / 2).ToXNA());
#pragma warning restore CS0618 // Type or member is obsolete
            else
#pragma warning disable CS0618 // Type or member is obsolete
                spriteBatch.Draw(_data,
                    position: pos,
                    color: Tint.HasValue ? Tint.Value.ToXNA() : Microsoft.Xna.Framework.Color.White,
                    scale: Scale.ToXNA(),
                    origin: pos + (Size / 2).ToXNA());
#pragma warning restore CS0618 // Type or member is obsolete
            if (DrawChildren)
                foreach (Drawable2 d2 in Children)
                    d2.Draw(spriteBatch, offset);
        }

        public Col4? Tint;
    }
}
