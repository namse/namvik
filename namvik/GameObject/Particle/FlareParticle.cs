using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace namvik.GameObject.Particle
{
    public class FlareParticle: BaseGameObject
    {
        public FlareParticle(BaseGameObject parent) : base(parent)
        {
            HasMass = false;
        }
    }
}
