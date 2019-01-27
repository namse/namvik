using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Collision;
using Box2DX.Common;
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

            var polygonDef = new PolygonDef();

            var hx = (24f).ToMeter();
            var hy = (3.5f).ToMeter();
            var center = new Vec2(hx + 50f.ToMeter(), 0);
            polygonDef.SetAsBox(hx, hy, center, angle: 0);

            polygonDef.Density = 1f;
            polygonDef.Friction = 0f;
            polygonDef.Restitution = 0f;
            //polygonDef.IsSensor = true;
            polygonDef.Filter.GroupIndex = ContactGroupIndex.Monster;

            Body.CreateShape(polygonDef);
            PolygonDefs.Add(polygonDef);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}
