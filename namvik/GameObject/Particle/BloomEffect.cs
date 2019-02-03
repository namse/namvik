using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace namvik.GameObject.Particle
{
    public class BloomEffect : BaseGameObject
    {
        private readonly List<ByeByeParticle> _particles;
        private readonly Texture2D _texture = new Texture2D(
            Game1.Device,
            Game1.Graphics.PreferredBackBufferWidth,
            Game1.Graphics.PreferredBackBufferHeight);
        public BloomEffect(BaseGameObject parent, List<ByeByeParticle> particles) : base(parent)
        {
            HasMass = false;

            _particles = particles;
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            var colors = new Color[_texture.Width * _texture.Height];
            for (var i = 0; i < colors.Length; i += 1)
            {
                colors[i] = Color.Transparent;
            }

            _particles.ForEach(particle =>
            {
                var origin = Game1.CenterOfScreen - new Vector2(Game1.Graphics.PreferredBackBufferWidth,
                                 Game1.Graphics.PreferredBackBufferHeight) / 2f;
                var vertexes = particle.GetVertexes().Select(vertex => vertex - origin);

                var previousVertex = vertexes.Last();
                foreach (var vertex in vertexes)
                {
                    DrawLineToTexture(previousVertex, vertex, (x0, y0) =>
                    {
                        var step = 1;
                        for (var x = x0 - step; x <= x0 + step; x += 1)
                        {
                            if (x < 0 || x >= _texture.Width)
                            {
                                continue;
                            }
                            var index = y0 * _texture.Width + x;
                            if (index < 0 || index >= _texture.Width * _texture.Height)
                            {
                                continue;
                            }
                            colors[index] = new Color(1f, 1f, 1f, 1f - Math.Abs(x0 - x) / (step + 1));
                        }
                    });
                    previousVertex = vertex;
                }
            });
            _texture.SetData(colors);
        }

        private void DrawLineToTexture(Vector2 vector0, Vector2 vector1, Action<int, int> action)
        {
            var (x0, y0) = vector0;
            var (x1, y1) = vector1;

            if (x0 > x1)
            {
                var temp = x1;
                x1 = x0;
                x0 = temp;

                temp = y1;
                y1 = y0;
                y0 = temp;
            }

            var deltaX = x1 - x0;

            if (deltaX == 0)
            {
                for (var y = (int)Math.Min(y0, y1); y <= Math.Max(y0, y1); y += 1)
                {
                    action((int)x0, y);
                }
            }
            else
            {
                var deltaY = y1 - y0;
                var deltaErr = Math.Abs(deltaY / deltaX);
                var error = 0.0; // No error at start
                var y = (int)y0;
                for (var x = (int)x0; x <= x1; x += 1)
                {
                    action(x, y);

                    error = error + deltaErr;
                    if (error >= 0.5)
                    {
                        y = y + Math.Sign(deltaY) * 1;
                        error = error - 1.0;
                    }
                }
            }
        }

        public override void Draw(float dt, SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate);
            spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, transformMatrix: Game1.Camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
        }
    }
}
