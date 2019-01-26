using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace namvik
{
    public class FpsPrinter: GameObject
    {
        private int _frameCount;
        private float _sumOfTime;
        private readonly int _maxFrameCount = 5;
        private int _fps;
        public override void Initialize(ContentManager content)
        {
        }

        public override void Update(float dt)
        {
            
        }

        public override void Draw(float dt, SpriteBatch spriteBatch)
        {
            _frameCount += 1;
            _sumOfTime += dt;
            if (_frameCount == _maxFrameCount)
            {
                _fps = (int)(_frameCount / _sumOfTime);
                _frameCount = 0;
                _sumOfTime = 0;
            }

            spriteBatch.DrawString(Game1.DefaultFont, _fps.ToString(), new Vector2(0, 0), Color.GreenYellow);
        }
    }
}
