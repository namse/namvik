using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace namvik
{
    public class FpsPrinter: GameObject
    {
        private SpriteFont _font;
        private int frameCount;
        private float _sumOfTime;
        private readonly int _maxFrameCount = 5;
        private int _fps;
        public override void Initialize(ContentManager content)
        {
            _font = content.Load<SpriteFont>("font/defaultFont");
        }

        public override void Update(float dt)
        {
            
        }

        public override void Draw(float dt, SpriteBatch spriteBatch)
        {
            frameCount += 1;
            _sumOfTime += dt;
            if (frameCount == _maxFrameCount)
            {
                _fps = (int)(frameCount / _sumOfTime);
                frameCount = 0;
                _sumOfTime = 0;
            }

            spriteBatch.DrawString(_font, _fps.ToString(), new Vector2(0, 0), Color.GreenYellow);
        }
    }
}
