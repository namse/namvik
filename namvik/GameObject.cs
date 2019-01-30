using System;
using System.Collections.Generic;
using System.Linq;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using namvik.Tile;
using Color = Microsoft.Xna.Framework.Color;
using ContactListener = namvik.Contact.ContactListener;
using ContactManager = namvik.Contact.ContactManager;

namespace namvik
{
    public abstract class GameObject: ContactListener
    {
        protected bool HasMass = true;
        public Texture2D Texture;
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
        protected bool IsSeeLeft = true;

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

        public void Destroy()
        {
            Body.GetWorld().DestroyBody(Body);
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

        public virtual void Initialize(ContentManager content)
        {
            ContactManager.RegisterContactListener(this);
        }

        public virtual void Update(float dt)
        {
            if (HasMass && !IsOnGround)
            {
                var vy = Body.GetLinearVelocity().Y;
                vy += dt * 9.8f;
                Body.SetVelocityY(vy);
            }
        }

        public virtual void Draw(float dt, SpriteBatch spriteBatch)
        {
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
                IsSeeLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f);

            PolygonDefs.ForEach(polygonDef =>
            {
                polygonDef.Draw(Body.GetPosition(), spriteBatch, IsOnGround ? Color.GreenYellow : Color.Red);
            });
        }
    }
}
