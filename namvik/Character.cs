using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Math = System.Math;

namespace namvik
{
    public class Character
    {
        private Texture2D _texture;
        public Vector2 Position;
        private Body _body;
        private readonly float _maxVelocityX = 5f;
        private readonly float _accelerationX = 50f;
        public void Initialize(ContentManager content)
        {
            _texture = content.Load<Texture2D>("sprite/character");
            Position = new Vector2(246.2743f, -1506.1f);

            var bodyDef = new BodyDef
            {
                Position = Position.ToVec2(),
                FixedRotation = true,
            };

            _body = Map.World.CreateBody(bodyDef);

            var polygonDef = new PolygonDef();

            var hx = (_texture.Width / 2f).ToMeter();
            var hy = (_texture.Height / 2f).ToMeter();
            polygonDef.SetAsBox(hx, hy, center: new Vec2(hx, hy), angle: 0);

            polygonDef.Density = 0.1f;
            polygonDef.Friction = 0.1f;

            _body.CreateShape(polygonDef);

            _body.SetMassFromShapes();
        }

        public void Update(float dt)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                //_body.SetLinearVelocity(new Vec2(0f, 10));
                var isGround = _body.GetLinearVelocity().Y == 0;
                if (isGround)
                {
                    var velocity = _body.GetLinearVelocity();

                    var height = 190f.ToMeter();
                    var initialVy = (float)Math.Sqrt(20f * height);

                    velocity.Y = - initialVy;
                    _body.SetLinearVelocity(velocity);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                var isLeft = Keyboard.GetState().IsKeyDown(Keys.A);
                var accelerationX = isLeft ? -_accelerationX : _accelerationX;

                var velocity = _body.GetLinearVelocity();

                velocity.X += dt * accelerationX;

                if (Math.Abs(velocity.X) > _maxVelocityX)
                {
                    velocity.X = isLeft ? -_maxVelocityX : _maxVelocityX;
                }
                _body.SetLinearVelocity(velocity);
            }

            if (Keyboard.GetState().IsKeyUp(Keys.D) && Keyboard.GetState().IsKeyUp(Keys.A))
            {
                var velocity = _body.GetLinearVelocity();
                velocity.X = 0;
                _body.SetLinearVelocity(velocity);
            }

            Position = _body.GetPosition().ToVector2();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }
    }
}
