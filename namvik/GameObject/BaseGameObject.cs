using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using namvik.Tile;
using Color = Microsoft.Xna.Framework.Color;
using ContactListener = namvik.Contact.ContactListener;
using ContactManager = namvik.Contact.ContactManager;

namespace namvik.GameObject
{
    public abstract class BaseGameObject: ContactListener
    {
        public readonly BaseGameObject Parent;
        public readonly List<BaseGameObject> Children = new List<BaseGameObject>();
        public static List<BaseGameObject> GameObjects = new List<BaseGameObject>();

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
        protected Color _color = Color.White;
        protected CategoryBits categoryBits;
        protected MaskBits maskBits = MaskBits.Unknown;

        protected BaseGameObject(BaseGameObject parent)
        {
            GameObjects.Add(this);

            Parent = parent;
            parent?.Children.Add(this);

            _isSeeingLeft = Parent?.IsSeeingLeft ?? false;

            ContactManager.RegisterContactListener(this);
            Texture = TextureLoader.GetTexture(GetType());
        }

        protected void DetachChild(BaseGameObject child)
        {
            Children.Remove(child);
        }

        public void Destroy()
        {
            Body?.GetWorld().DestroyBody(Body);

            Parent?.DetachChild(this);

            Children.ForEach(child => child.Destroy()); // TODO FIX BUG HERE!! ALREADY CHILD HAS BEEN REMOVED ON DetachDChild!!
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
            polygonDef.Filter.CategoryBits = (ushort)categoryBits;
            polygonDef.Filter.MaskBits = (ushort)maskBits;

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

            if (HasMass && Body != null)
            {
                Position = Body.GetPosition().ToVector2();
            }
            else
            {
                Position = Parent.Position;
            }
        }

        public virtual void PreDraw(float dt, GraphicsDevice device, Camera2D camera)
        {

        }

        public virtual void Draw(float dt, SpriteBatch spriteBatch)
        {
            DrawTexture(dt, spriteBatch, Texture);
            DrawPolygons(dt, spriteBatch);
        }

        protected void DrawTexture(float dt, SpriteBatch spriteBatch, Texture2D texture)
        {
            if (texture is null)
            {
                return;
            }

            var integerPosition = new Vector2((int)Position.X, (int)Position.Y);

            spriteBatch.Draw(
                texture,
                integerPosition,
                null,
                _color,
                0,
                Vector2.Zero,
                Vector2.One,
                IsSeeingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f);
        }

        private void DrawPolygons(float dt, SpriteBatch spriteBatch)
        {
            PolygonDefs.ForEach(polygonDef =>
            {
                polygonDef.Draw(Body.GetPosition(), spriteBatch, IsOnGround ? Color.GreenYellow : Color.Red);
            });
        }
    }
}
