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
        private readonly float _accelerationX = 60f.ToMeter();
        private bool _isOnGround;
        private readonly float _maximumJumpHeight = 230f.ToMeter();
        private List<PolygonDef> _polygonDefs = new List<PolygonDef>();
        private float _friction = 0.8f;
        private Shape _mainBodyShape;

        private bool shouldUpdateVeolictyX;
        private float nextVelocityX;

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

            polygonDef.Density = 0.001f;
            polygonDef.Friction = _friction;

            _mainBodyShape = _body.CreateShape(polygonDef);
            _polygonDefs.Add(polygonDef);

            CreateNonFrictionBorder();

            _body.SetMassFromShapes();

            Map.World.SetContactListener(this);
        }

        public void CreateNonFrictionBorder()
        {
            var hx = (_texture.Width / 2f).ToMeter();
            var halfPixel = 0.5f.ToMeter();
            var hy = (_texture.Height / 2f).ToMeter() - halfPixel;

            CreateNonFrictionBody(halfPixel, hy, center: new Vec2(hx * 2 + halfPixel, hy));
            CreateNonFrictionBody(halfPixel, hy, center: new Vec2(-halfPixel, hy));
        }

        public void CreateNonFrictionBody(float hx, float hy, Vec2 center)
        {
            var polygonDef = new PolygonDef();
            polygonDef.SetAsBox(hx, hy, center: center, angle: 0);
            polygonDef.Density = 0f;
            polygonDef.Friction = 0f;

            _body.CreateShape(polygonDef);
            _polygonDefs.Add(polygonDef);
        }


        public override void Add(ContactPoint point)
        {
            base.Add(point);

            var isMyCollision = point.Shape1 == _mainBodyShape || point.Shape2 == _mainBodyShape;
            if (!isMyCollision)
            {
                return;
            }

            var opposite = point.Shape1.GetBody() == _body ? point.Shape2.GetBody() : point.Shape1.GetBody();

            var userData = opposite.GetUserData();
            if (userData is TileObject tileObject && tileObject.TileGorup == TileGroup.Collision)
            {
                if (point.Normal.Y < 0)
                {
                    _isOnGround = true;
                    //var position = _body.GetPosition();
                    //position -= point.Normal * point.Separation;
                    //_body.SetXForm(position, _body.GetAngle());

                    shouldUpdateVeolictyX = true;
                    nextVelocityX = _body.GetLinearVelocity().X;
                }
            }
        }

        public void Update(float dt)
        {
            if (shouldUpdateVeolictyX)
            {
                _body.SetVelocityX(nextVelocityX);
                shouldUpdateVeolictyX = false;
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
                var isLeft = Keyboard.GetState().IsKeyDown(Keys.Left);

                var velocity = _body.GetLinearVelocity();

                var dax = (_accelerationX + _friction * _body.GetWorld().Gravity.Y);
                if (isLeft)
                {
                    dax = -dax;
                }

                velocity.X += dt * dax;

                if (Math.Abs(velocity.X) > _maxVelocityX)
                {
                    velocity.X = isLeft ? -_maxVelocityX : _maxVelocityX;
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
