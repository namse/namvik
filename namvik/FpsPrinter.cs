using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace namvik
{
    public class FpsPrinter: GameObject
    {
        private int _drawCount;
        private float _sumDrawTime;
        private readonly int _maxFrameCount = 5;
        private int _updateCount;
        private float _sumUpdateTime;
        private int _drawsPerSecond;
        private int _updatesPerSecond;
        public override void Initialize(ContentManager content)
        {
        }

        public override void Update(float dt)
        {
            _updateCount += 1;
            _sumUpdateTime += dt;
            if (_updateCount == _maxFrameCount)
            {
                _updatesPerSecond = (int)(_updateCount / _sumUpdateTime);
                _updateCount = 0;
                _sumUpdateTime = 0;
            }
        }

        public override void Draw(float dt, SpriteBatch spriteBatch)
        {
            _drawCount += 1;
            _sumDrawTime += dt;
            if (_drawCount == _maxFrameCount)
            {
                _drawsPerSecond= (int)(_drawCount / _sumDrawTime);
                _drawCount = 0;
                _sumDrawTime = 0;
            }

            spriteBatch.DrawString(Game1.DefaultFont, $"{_drawsPerSecond} | {_updatesPerSecond}", new Vector2(0, 0), Color.GreenYellow);
        }
    }
}
