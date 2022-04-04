using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Charybdis.MonoGame
{
    public class TextWindow : Window
    {
        public Col4 TextColor;
        public Col4? TextShadowColor;
        public Font Font;
        public float Padding;

        private string _text = null;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                Resize(Font.CalculateTextSize(value) + new Vec2(Padding * 2));
            }
        }

        public TextWindow(string text, Font font, Col4 borderColor, Col4 fillColor, Col4 textColor, Col4? textShadowColor = null, float padding = 6)
            : base(Vec2.One, borderColor, fillColor)
        {
            Font = font;
            TextColor = textColor;
            TextShadowColor = textShadowColor;
            Padding = padding;
            Text = text;
        }

        public override void Draw(SpriteBatch spriteBatch, Vec2 offset)
        {
            if (!DrawMe)
                return; //Abort if not set to draw.

            if (Font == null)
                return; //Can't draw if we don't have a font to draw in! Not an error, though, so just quietly don't continue this.

            Vec2 effectivePosition = Position + (Parent != null ? Parent.Position : Vec2.Zero) + offset;            

            base.Draw(spriteBatch, offset);
            if (TextShadowColor.HasValue)
                spriteBatch.DrawShadowedString(Font, Text, effectivePosition +  new Vec2(Padding), TextColor, Vec2.One, TextShadowColor.Value);
            else
                spriteBatch.DrawString(Font, Text, effectivePosition + new Vec2(Padding), TextColor);

            if (DrawChildren)
                foreach (var child in Children)
                    child.Draw(spriteBatch, offset);
        }
    }
}
