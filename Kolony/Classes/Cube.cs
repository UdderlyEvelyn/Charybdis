﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Charybdis.Library.Core;
using Charybdis.Science;
using Charybdis.MonoGame;

namespace Kolony
{
    public class Cube : KolonyObject
    {
        public Cube()
        {
            SelectionEnabled = true;
        }

        public Material Material;

        public override void Update()
        {
            /*Nothing to see here.*/
        }
    }
}