using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using namvik.GameObject.Item;
using ContactPoint = namvik.Contact.ContactPoint;
using Math = System.Math;

namespace namvik.GameObject
{
    public class Character : BaseGameObject
    {
        private readonly float _maxVelocity = 480f.ToMeter();
        private readonly float _accelerationX = 600f.ToMeter();
        private readonly float _defaultJumpHeight = 230f.ToMeter();

        private readonly float _lovelyzK = 0.3f;
        private bool _isStartingJump;
        private float _timeSinceNotGround;
        public bool CanIJump => !_isStartingJump && (IsOnGround || _timeSinceNotGround < 0.10f);

        private readonly List<BaseItem> _items = new List<BaseItem>();

        public Character() : this(null)
        {
        }

        public Character(BaseGameObject parent): base(parent)
        {
            categoryBits = CategoryBits.Character;
            maskBits = MaskBits.Character;
            Position = new Vector2(246.2743f, -1806.1f);
            MakeBox2DBoxWithTexture();
        }

        public override void OnCollisionBefore(ContactPoint point)
        {
            base.OnCollisionBefore(point);

            if (point.OppositeShape.FilterData.GroupIndex == ContactGroupIndex.Monster)
            {
                if (point.OppositeShape.GetBody().GetUserData() is BaseGameObject gameObject)
                {
                    gameObject.IsDead = true;
                    JumpAfterKillMonster();
                }
            }
        }

        public void JumpAfterKillMonster()
        {
            Jump(0.5f * _defaultJumpHeight);
        }

        public void Jump(float height)
        {
            var velocity = Body.GetLinearVelocity();

            var vYForHeight = (float)Math.Sqrt(2f * 9.8f * height);

            velocity.Y = -vYForHeight;
            Body.SetLinearVelocity(velocity);
        }

        public void AddItem(BaseItem item)
        {
            _items.Add(item);
            item.OnLooted();
        }

        public void OnHit()
        {
            if (_items.Count <= 0)
            {
                return;
            }

            var item = _items.First();
            item.OnDestroyed();
            DetachChild(item);
            _items.Remove(item);
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (!IsOnGround)
            {
                if (CanIJump)
                {
                    _isStartingJump = false;
                }
                _timeSinceNotGround += dt;
            }

            if (IsOnGround)
            {
                _isStartingJump = false;
                _timeSinceNotGround = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (CanIJump)
                {
                    Jump(_defaultJumpHeight);

                    _isStartingJump = true;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                var isLeftMove = Keyboard.GetState().IsKeyDown(Keys.A);
                IsSeeingLeft = isLeftMove;
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
                else if (IsOnGround)
                {
                    var contactPoint = ContactPoints.First(point => point.Normal.Y > 0);
                    var normal = contactPoint.Normal;

                    var rotateAngle = (isLeftMove ? 1 : -1) * (float)Math.PI / 2f;
                    var directionVector = normal.Rotate(rotateAngle);

                    var acceleration = _accelerationX * (1 + _lovelyzK * directionVector.Y);
                    var speedVector = acceleration * directionVector;

                    velocity += dt * speedVector;

                    if (Math.Abs(velocity.X) > Math.Abs(_maxVelocity * directionVector.X))
                    {
                        velocity.X = _maxVelocity * directionVector.X;
                    }
                    if (Math.Abs(velocity.Y) > Math.Abs(_maxVelocity * directionVector.Y))
                    {
                        velocity.Y = _maxVelocity * directionVector.Y;
                    }
                }
                Body.SetLinearVelocity(velocity);
            }

            if (Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.Right))
            {
                var velocity = Body.GetLinearVelocity();

                if (velocity.X != 0)
                {
                    velocity.X = 0;
                    Body.SetLinearVelocity(velocity);
                }
            }
        }
    }
}
