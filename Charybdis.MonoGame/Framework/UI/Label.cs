using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Vec2 = Microsoft.Xna.Framework.Vector2;
using Vec3 = Microsoft.Xna.Framework.Vector3;

namespace Charybdis.MonoGame
{
    public class Label : Drawable2
    {
        public Label()
        {
            Padding = 3;
        }

        public Vec2? Offset;
        public bool Shadowed;
        public object DataSource;
        public PropertyInfo BindingProperty;
        public FieldInfo BindingField;
        public float Padding;

        private string _lastText;
        private string _text;
        public string Text
        {
            get
            {
                if (!string.IsNullOrEmpty(_text))
                    return _text;
                else if (DataSource != null)
                {
                    if (BindingProperty != null)
                    {
                        var value = BindingProperty.GetValue(DataSource);
                        if (value == null)
                        {
                            if (_lastText != "NULL")
                                Size = Font.CalculateTextSize("NULL");
                            return "NULL";
                        }
                        else
                        {
                            string stringValue = value.ToString();
                            if (_lastText != stringValue)
                                Size = Font.CalculateTextSize(stringValue);
                            return _lastText = stringValue;
                        }
                    }
                    else if (BindingField != null)
                    {
                        var value = BindingField.GetValue(DataSource);
                        if (value == null)
                        {
                            if (_lastText != "NULL")
                                Size = Font.CalculateTextSize("NULL");
                            return _lastText = "NULL";
                        }
                        else
                        {
                            string stringValue = value.ToString();
                            if (_lastText != stringValue)
                                Size = Font.CalculateTextSize(stringValue);
                            return _lastText = stringValue;
                        }
                    }
                    else return "";
                }
                else
                    return "";
            }
            set
            {
                _text = value;
                if (string.IsNullOrEmpty(Text) || Font == null)
                    Size = Vec2.Zero;
                else
                    Size = Font.CalculateTextSize(Text);
            }
        }
        public Font Font { get; set; }

        public override void Draw(SpriteBatch spriteBatch, Vec2 offset)
        {
            if (!DrawMe)
                return; //Abort if not set to draw.

            if (Font == null)
                return; //Can't draw if we don't have a font to draw in! Not an error, though, so just quietly don't continue this.

            if (Shadowed)
            {
                if (Parent != null)
                    //Draw under the parent, centered on the X axis.
                    spriteBatch.DrawShadowedString(Font, Text, new Vec2(Parent.Position.X + offset.X + (Parent.Size.X / 2) - (Size.X / 2), Parent.Position.Y + offset.Y + Parent.Size.Y + Padding) + (Offset != null ? Offset.Value : Vec2.Zero));
                else
                    //Draw at its set position.
                    spriteBatch.DrawShadowedString(Font, Text, Position + offset);
            }
            else
            {
                if (Parent != null)
                    //Draw under the parent, centered on the X axis.
                    spriteBatch.DrawString(Font, Text, new Vec2(Parent.Position.X + offset.X + (Parent.Size.X / 2) - (Size.X / 2), Parent.Position.Y + offset.Y + Parent.Size.Y + Padding) + (Offset != null ? Offset.Value : Vec2.Zero));
                else
                    //Draw at its set position.
                    spriteBatch.DrawString(Font, Text, Position + offset);
            }

            if (DrawChildren)
                foreach (var child in Children)
                    child.Draw(spriteBatch, offset);
        }

        //public string DebugInfo
        //{
        //    get
        //    {
        //        string inPosEq = Parent.Position.X.ToString() + " + (" + Parent.Size.X + " / 2) - (" + Size.X + " / 2), " + Parent.Position.Y + " + " + Parent.Size.Y + " + " + Padding;
        //        Vec2 inPos = new Vec2(Parent.Position.X + (Parent.Size.X / 2) - (Size.X / 2), Parent.Position.Y + Parent.Size.Y + Padding);
        //        string drawPosEq = inPos.X + ", " + inPos.Y + " + ((YO + YP) * " + Font.Scale + ") + (LineIndex * rectH * " + Font.Scale + "))";
        //        float x = inPos.X + XO;
        //        float y = inPos.Y + ((YO + YP) * Font.Scale) + (LineIndex * rectH * Font.Scale);
        //        (int)(position.X + xOffset), (int)(position.Y + ((r.DestOffset.Y + r.DestPadding.Y) * font.Scale) + (line * r.H * font.Scale))
        //        xOffset += (int)(((r.W + r.DestOffset.X) * font.Scale) + (font.Spacing * (r.W + r.DestPadding.X) * font.Scale));
        //    }
        //}
    }
}
