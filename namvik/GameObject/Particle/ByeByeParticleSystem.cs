using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace namvik.GameObject.Particle
{
    class ByeByeParticleSystem: BaseGameObject
    {
        private List<ByeByeParticle> _byeByeParticles = new List<ByeByeParticle>();
        private static readonly Random Random = new Random();
        public ByeByeParticleSystem(BaseGameObject parent) : base(parent)
        {
            HasMass = false;
        }

        public void MakeParticlesWithTexture(Texture2D texture)
        {
            var colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);


            var center = new Vector2(texture.Width / 2, texture.Height / 2);

            for (var i = 0; i < 100; i += 1)
            {
                var x = Random.Next(0, texture.Width);
                var y = Random.Next(0, texture.Height);
                var index = y * texture.Width + x;
                var color = colors[index];
                if (color.A == 0)
                {
                    continue;
                }

                var rotationSpeed = (float)(Math.PI * Random.NextDouble() * 2);
                var length = (float)Random.NextDouble() * 7 + 3;
                var byeByeParticle = new ByeByeParticle(this, new Vector2(x, y), center, rotationSpeed, length);
                _byeByeParticles.Add(byeByeParticle);
            }
        }
    }
}
