using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using namvik.Tile;
using Color = Microsoft.Xna.Framework.Color;
using ContactListener = namvik.Contact.ContactListener;
using ContactManager = namvik.Contact.ContactManager;

namespace namvik
{
    public abstract class GameObject: ContactListener
    {
        public readonly GameObject Parent;
        public readonly List<GameObject> Children = new List<GameObject>();
        public static List<GameObject> GameObjects = new List<GameObject>();

        protected bool HasMass = true;
        public readonly Texture2D Texture;
        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                Body?.SetXForm(value.ToVec2(), Body.GetAngle());
            }
        }
        private bool _isSeeingLeft;

        protected bool IsSeeingLeft
        {
            get => _isSeeingLeft;
            set
            {
                _isSeeingLeft = value;
                Children.ForEach(child => child.IsSeeingLeft = value);
            }
        }


        protected Body Body;

        protected bool IsOnGround
        {
            get
            {
                return ContactPoints.Count() != 0 && ContactPoints.Any(contactPoint =>
                        contactPoint.Normal.Y > 0 && contactPoint.OppositeShape.FilterData.GroupIndex != ContactGroupIndex.Monster);
            }
        }
        protected readonly List<PolygonDef> PolygonDefs = new List<PolygonDef>();
        protected Shape MainBodyShape;
        
        public bool IsDead;

        protected GameObject(GameObject parent)
        {
            GameObjects.Add(this);

            Parent = parent;
            parent?.Children.Add(this);

            _isSeeingLeft = Parent?.IsSeeingLeft ?? false;

            ContactManager.RegisterContactListener(this);
            Texture = TextureLoader.GetTexture(GetType());
        }

        public void Destroy()
        {
            Body?.GetWorld().DestroyBody(Body);

            Parent?.Children.Remove(this);

            Children.ForEach(child => child.Destroy());
        }

        protected virtual void MakeBox2DBoxWithTexture()
        {
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
        }

        public virtual void Update(float dt)
        {
            if (Body != null && HasMass && !IsOnGround)
            {
                var vy = Body.GetLinearVelocity().Y;
                vy += dt * 9.8f;
                Body.SetVelocityY(vy);
            }
        }

        public virtual void Draw(float dt, SpriteBatch spriteBatch)
        {
            if (Body != null)
            {
                DrawByBody(dt, spriteBatch);
            }
            else
            {
                DrawByParentPosition(dt, spriteBatch);
            }
        }

        private void DrawByParentPosition(float dt, SpriteBatch spriteBatch)
        {
            var position = Parent.Position;

            var integerPosition = new Vector2((int)position.X, (int)position.Y);

            spriteBatch.Draw(
                Texture,
                integerPosition,
                null,
                Color.White,
                0,
                Vector2.Zero,
                Vector2.One,
                IsSeeingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f);
        }

        private void DrawByBody(float dt, SpriteBatch spriteBatch)
        {
            if (Body is null)
            {
                throw new Exception("DrawByBody must be called when GameObject has Body");
            }

            Position = Body.GetPosition().ToVector2();

            var integerPosition = new Vector2((int)Position.X, (int)Position.Y);

            spriteBatch.Draw(
                Texture,
                integerPosition,
                null,
                Color.White,
                0,
                Vector2.Zero,
                Vector2.One,
                IsSeeingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f);

            PolygonDefs.ForEach(polygonDef =>
            {
                polygonDef.Draw(Body.GetPosition(), spriteBatch, IsOnGround ? Color.GreenYellow : Color.Red);
            });
        }
    }
}
