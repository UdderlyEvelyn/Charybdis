using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charybdis.Library.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Charybdis.MonoGame
{
    public class GameObject : CharybdisObject
    {
        /// <summary>
        /// Randomness source for this object - may or may not be shared with others.
        /// </summary>
        public Random Random;

        /// <summary>
        /// Whether this object may be selected by the user.
        /// </summary>
        public bool SelectionEnabled = false;

        /// <summary>
        /// Whether this object is selected by the user.
        /// </summary>
        public bool Selected = false;
    }
}