using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace namvik
{
    public class Character
    {
        private Texture2D _texture;
        private Vector2 _position;
        public void Initialize(ContentManager content)
        {
            _texture = content.Load<Texture2D>("sprite/character");
        }

        public void Update()
        {
            var moveVector = new Vector2();

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                moveVector += new Vector2(0, -1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                moveVector += new Vector2(0, 1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                moveVector += new Vector2(-1, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                moveVector += new Vector2(1, 0);
            }

            if (moveVector.Length() > 0)
            {
                moveVector.Normalize();
            }

            moveVector *= 10;
            _position += moveVector;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, Color.White);
        }
    }
}
