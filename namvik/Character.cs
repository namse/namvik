using System;
using System.Collections.Generic;
using System.Xml;
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
        private readonly float _maxVelocityX = 480f.ToMeter();
        private readonly float _accelerationX = 600f.ToMeter();
        private bool _isOnGround;
        private readonly float _maximumJumpHeight = 230f.ToMeter();
        private readonly List<PolygonDef> _polygonDefs = new List<PolygonDef>();
        private Shape _mainBodyShape;
        private readonly Dictionary<uint, ContactPoint> _contactPoints = new Dictionary<uint, ContactPoint>();

        public void Initialize(ContentManager content)
        {
            _texture = content.Load<Texture2D>("sprite/character");
            Position = new Vector2(246.2743f, -1806.1f);

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

            polygonDef.Density = 0.001f;
            polygonDef.Friction = 0f;
            polygonDef.Restitution = 0f;

            _mainBodyShape = _body.CreateShape(polygonDef);
            _polygonDefs.Add(polygonDef);

            _body.SetMassFromShapes();

            _body.SetUserData(this);

            Map.World.SetContactListener(this);
        }

        public override void Remove(ContactPoint point)
        {
            base.Remove(point);

            if (_contactPoints.ContainsKey(point.ID.Key))
            {
                _contactPoints.Remove(point.ID.Key);
            }
        }

        public override void Add(ContactPoint point)
        {
            base.Add(point);

            var isMyCollision = point.Shape1 == _mainBodyShape || point.Shape2 == _mainBodyShape;
            if (!isMyCollision)
            {
                return;
            }

            if (!_contactPoints.ContainsKey(point.ID.Key))
            {
                _contactPoints.Add(point.ID.Key, point);
            }

            var opposite = point.Shape1.GetBody() == _body ? point.Shape2.GetBody() : point.Shape1.GetBody();

            var userData = opposite.GetUserData();
            if (userData is TileObject tileObject && tileObject.TileGorup == TileGroup.Collision)
            {
                if (point.Normal.Y < 0)
                {
                    _isOnGround = true;
                }
            }
        }

        public void Update(float dt)
        {
            if (_contactPoints.Count == 0)
            {
                var vy = _body.GetLinearVelocity().Y;
                vy += dt * 9.8f;
                _body.SetVelocityY(vy);
            }

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
                var moveDirection = Keyboard.GetState().IsKeyDown(Keys.Left) ? -1 : 1;
                var velocity = _body.GetLinearVelocity();

                if ((moveDirection < 0 && velocity.X > 0) || (moveDirection > 0 && velocity.X < 0))
                {
                    velocity.X = 0;
                }
                
                velocity.X += dt * moveDirection * _accelerationX;

                if (Math.Abs(velocity.X) > _maxVelocityX)
                {
                    velocity.X = moveDirection * _maxVelocityX;
                }
                _body.SetLinearVelocity(velocity);
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.Right))
            {
                var velocity = _body.GetLinearVelocity();

                if (velocity.X != 0)
                {
                    velocity.X = 0;
                    _body.SetLinearVelocity(velocity);
                }   
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Position = _body.GetPosition().ToVector2();

            //Console.WriteLine(_body.GetLinearVelocity().ToVector2());

            var maybeWorks = new Vector2((int)Position.X, (int)Position.Y);
            spriteBatch.Draw(_texture, maybeWorks, Color.White);
            //spriteBatch.Draw(_texture, Position, Color.White);

            _polygonDefs.ForEach(polygonDef =>
            {
                polygonDef.Draw(_body.GetPosition(), spriteBatch);
            });
        }
    }
}
