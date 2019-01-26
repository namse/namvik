using System;
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
    public class Character: ContactListener
    {
        private Texture2D _texture;
        public Vector2 Position;
        private Body _body;
        private readonly float _maxVelocityX = 2f;
        private readonly float _accelerationX = 8f;
        private bool _isOnGround;
        private readonly float _maximumJumpHeight = 220f.ToMeter();
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
            polygonDef.Friction = 0.0f;
            polygonDef.Restitution = 0;

            _body.CreateShape(polygonDef);

            _body.SetMassFromShapes();

            Map.World.SetContactListener(this);
        }


        public override void Add(ContactPoint point)
        {
            base.Add(point);

            var isMyCollision = point.Shape1.GetBody() == _body || point.Shape2.GetBody() == _body;
            if (!isMyCollision)
            {
                return;
            }

            var opposite = point.Shape1.GetBody() == _body ? point.Shape2.GetBody() : point.Shape1.GetBody();

            var userData = opposite.GetUserData();
            if (userData is TileObject tileObject && tileObject.TileGorup == TileGroup.Collision)
            {
                Console.WriteLine(point.Normal.ToVector2());

                if (point.Normal.Y < 0)
                {
                    Console.WriteLine("hit the ground!");
                    _isOnGround = true;
                }
            }
        }

        public void Update(float dt)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (_isOnGround)
                {
                    _isOnGround = false;
                    var velocity = _body.GetLinearVelocity();

                    var initialVy = (float)Math.Sqrt(20f * _maximumJumpHeight);

                    velocity.Y = - initialVy;
                    _body.SetLinearVelocity(velocity);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                var isLeft = Keyboard.GetState().IsKeyDown(Keys.Left);
                var accelerationX = isLeft ? -_accelerationX : _accelerationX;

                var velocity = _body.GetLinearVelocity();

                velocity.X += dt * accelerationX;

                if (Math.Abs(velocity.X) > _maxVelocityX)
                {
                    velocity.X = isLeft ? -_maxVelocityX : _maxVelocityX;
                }
                _body.SetLinearVelocity(velocity);
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.Right))
            {
                var velocity = _body.GetLinearVelocity();
                velocity.X = 0;
                _body.SetLinearVelocity(velocity);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Position = _body.GetPosition().ToVector2();
            //Console.WriteLine(Position.Y);
            var maybeWorks = new Vector2((int)Position.X, (int)Position.Y);
            spriteBatch.Draw(_texture, maybeWorks, Color.White);
            //spriteBatch.Draw(_texture, Position, Color.White);
        }
    }
}
