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
        private readonly float _maxVelocityX = 480f.ToMeter();
        private readonly float _accelerationX = 600f.ToMeter();
        private readonly float _maximumJumpHeight = 230f.ToMeter();

        public override void Initialize(ContentManager content)
        {
            Texture = content.Load<Texture2D>("sprite/character");
            Position = new Vector2(246.2743f, -1806.1f);

            var bodyDef = new BodyDef
            {
                Position = Position.ToVec2(),
                FixedRotation = true,
            };

            Body = Map.World.CreateBody(bodyDef);

            var polygonDef = new PolygonDef();

            var hx = (Texture.Width / 2f).ToMeter();
            var hy = (Texture.Height / 2f).ToMeter();
            polygonDef.SetAsBox(hx, hy, center: new Vec2(hx, hy), angle: 0);

            polygonDef.Density = 0.001f;
            polygonDef.Friction = 0f;
            polygonDef.Restitution = 0f;

            MainBodyShape = Body.CreateShape(polygonDef);
            PolygonDefs.Add(polygonDef);

            Body.SetMassFromShapes();

            Body.SetUserData(this);

            Map.World.SetContactListener(this);
        }

        public override void Remove(ContactPoint point)
        {
            base.Remove(point);

            if (ContactPoints.ContainsKey(point.ID.Key))
            {
                ContactPoints.Remove(point.ID.Key);
            }
        }

        public override void Add(ContactPoint point)
        {
            base.Add(point);

            var isMyCollision = point.Shape1 == MainBodyShape || point.Shape2 == MainBodyShape;
            if (!isMyCollision)
            {
                return;
            }

            if (!ContactPoints.ContainsKey(point.ID.Key))
            {
                ContactPoints.Add(point.ID.Key, point);
            }

            var opposite = point.Shape1.GetBody() == Body ? point.Shape2.GetBody() : point.Shape1.GetBody();

            var userData = opposite.GetUserData();
            if (userData is TileObject tileObject && tileObject.TileGroup == TileGroup.Collision)
            {
                if (point.Normal.Y < 0)
                {
                    IsOnGround = true;
                }
            }
        }

        public override void Update(float dt)
        {
            if (ContactPoints.Count == 0)
            {
                var vy = Body.GetLinearVelocity().Y;
                vy += dt * 9.8f;
                Body.SetVelocityY(vy);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (IsOnGround)
                {
                    IsOnGround = false;
                    var velocity = Body.GetLinearVelocity();

                    var initialVy = (float)Math.Sqrt(20f * _maximumJumpHeight);

                    velocity.Y = - initialVy;
                    Body.SetLinearVelocity(velocity);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                var moveDirection = Keyboard.GetState().IsKeyDown(Keys.Left) ? -1 : 1;
                var velocity = Body.GetLinearVelocity();

                if ((moveDirection < 0 && velocity.X > 0) || (moveDirection > 0 && velocity.X < 0))
                {
                    velocity.X = 0;
                }
                
                velocity.X += dt * moveDirection * _accelerationX;

                if (Math.Abs(velocity.X) > _maxVelocityX)
                {
                    velocity.X = moveDirection * _maxVelocityX;
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
