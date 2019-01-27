using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace namvik
{
    class Monkey: GameObject
    {
        public static Monkey SpawnMonkey(ContentManager content, Vector2 position)
        {
            var monkey = new Monkey
            {
                Position = position,
            };
            monkey.Initialize(content);

            return monkey;
        }
        public override void Initialize(ContentManager content)
        {
            Texture = content.Load<Texture2D>("sprite/monkey");
            MakeBox2DBoxWithTexture();
        }

        public override void Update(float dt)
        {
            Console.WriteLine("monkey is coming");
        }
    }
}
