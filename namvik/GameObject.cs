using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace namvik
{
    public abstract class GameObject
    {
        public abstract void Initialize(ContentManager content);
        public abstract void Update(float dt);
        public abstract void Draw(float dt, SpriteBatch spriteBatch);
    }
}
