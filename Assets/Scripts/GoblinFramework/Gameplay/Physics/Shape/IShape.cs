using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Shape
{
    public interface IShape
    {
        public Fixed64 area { get; }
    }
}
