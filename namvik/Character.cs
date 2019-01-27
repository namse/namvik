using System;
using System.Linq;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using namvik.Tile;
using Color = Microsoft.Xna.Framework.Color;
using Math = System.Math;

namespace namvik
{
    public class Character: GameObject
    {
        private readonly float _maxVelocity = 480f.ToMeter();
        private readonly float _accelerationX = 600f.ToMeter();
        private readonly float _maximumJumpHeight = 230f.ToMeter();

        private readonly float _lovelyzK = 0.3f;
        private bool _isStartingJump;

        public override void Initialize(ContentManager content)
        {
            Texture = content.Load<Texture2D>("sprite/character");
            Position = new Vector2(246.2743f, -1806.1f);
            MakeBox2DBoxWithTexture();
        }

        public override void Update(float dt)
        {
            if (!IsOnGround)
            {
                var vy = Body.GetLinearVelocity().Y;
                vy += dt * 9.8f;
                Body.SetVelocityY(vy);
                _isStartingJump = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (IsOnGround)
                {
                    //IsOnGround = false;
                    var velocity = Body.GetLinearVelocity();

                    var initialVy = (float)Math.Sqrt(20f * _maximumJumpHeight);

                    velocity.Y = - initialVy;
                    Body.SetLinearVelocity(velocity);

                    _isStartingJump = true;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                var isLeftMove = Keyboard.GetState().IsKeyDown(Keys.Left);
                var velocity = Body.GetLinearVelocity();
                var moveDirection = isLeftMove ? -1 : 1;

                if ((moveDirection < 0 && velocity.X > 0) || (moveDirection > 0 && velocity.X < 0))
                {
                    velocity.X = 0;
                }

                if (!IsOnGround || _isStartingJump)
                {
                    velocity.X += dt * moveDirection * _accelerationX;

                    if (Math.Abs(velocity.X) > _maxVelocity)
                    {
                        velocity.X = moveDirection * _maxVelocity;
                    }
                }
                else
                {
                    var contactPoint = ContactPoints.First(point => point.Value.Normal.Y < 0).Value;
                    var normal = contactPoint.Normal;
                    var gamma = Math.Atan2(-normal.Y, normal.X);
                    var theta = (Math.PI / 2) - gamma;

                    var acceleration = isLeftMove
                        ? _accelerationX * (-1 + _lovelyzK * Math.Sin(theta))
                        : _accelerationX * (1 + _lovelyzK * Math.Sin(theta));
                    
                    var aX = (float)(acceleration * Math.Cos(theta));
                    var aY = (float)(acceleration * Math.Sin(theta));

                    velocity.X += dt * aX;
                    velocity.Y += dt * aY;

                    if (Math.Abs(velocity.X) > _maxVelocity * Math.Cos(theta))
                    {
                        velocity.X = moveDirection * _maxVelocity * (float)Math.Cos(theta);
                    }
                    if (Math.Abs(velocity.Y) > _maxVelocity * Math.Sin(theta))
                    {
                        velocity.Y = moveDirection * _maxVelocity * (float)Math.Sin(theta);
                    }
                }
                Body.SetLinearVelocity(velocity);
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.Right))
            {
                var velocity = Body.GetLinearVelocity();

                if (velocity.X != 0)
                {
                    velocity.X = 0;
                    Body.SetLinearVelocity(velocity);
                }   
            }
        }

        public override void Draw(float dt, SpriteBatch spriteBatch)
        {
            Position = Body.GetPosition().ToVector2();

            //Console.WriteLine(_body.GetLinearVelocity().ToVector2());

            var maybeWorks = new Vector2((int)Position.X, (int)Position.Y);
            spriteBatch.Draw(Texture, maybeWorks, Color.White);
            //spriteBatch.Draw(_texture, Position, Color.White);

            PolygonDefs.ForEach(polygonDef =>
            {
                polygonDef.Draw(Body.GetPosition(), spriteBatch);
            });
        }
    }
}
