using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Charybdis.MonoGame.Framework.UI
{
    public class Button : TextWindow
    {
        public Action Action { get; set; }

        //public event OnClickEventHandler OnClick;

        //public delegate void OnClickEventHandler(Button b, OnClickEventArgs e);
        //public class OnClickEventArgs : EventArgs
        //{
        //    public Button Button;
        //    public MouseButton MouseButton;
        //}

        public Button(string text, Font font, Col4 borderColor, Col4 fillColor, Col4 textColor, Col4? textShadowColor = null, float padding = 6) : base(text, font, borderColor, fillColor, textColor, textShadowColor, padding)
        {

        }
    }

    //public enum MouseButton
    //{
    //    Left,
    //    Middle,
    //    Right,
    //}
}
