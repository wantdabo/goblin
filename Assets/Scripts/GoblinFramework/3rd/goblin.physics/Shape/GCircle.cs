﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;

namespace GoblinFramework.Physics.Shape
{
    public struct GCircle
    {
        public TSVector2 position;
        public FP radius;

        public GCircle(TSVector2 position, FP radius)
        {
            this.position = position;
            this.radius = radius;
        }
    }
}
