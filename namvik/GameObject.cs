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
using namvik.Tile;
using Color = Microsoft.Xna.Framework.Color;

namespace namvik
{
    public abstract class GameObject : ContactListener
    {
        protected bool HasMass;
        protected Texture2D Texture;
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

        protected Body Body;

        protected bool IsOnGround
        {
            get
            {
                return ContactPoints.Count != 0 && ContactPoints.Values.Any(contactPoint => contactPoint.Normal.Y < 0);
            }
        }
        protected readonly List<PolygonDef> PolygonDefs = new List<PolygonDef>();
        protected Shape MainBodyShape;
        protected readonly Dictionary<uint, ContactPoint> ContactPoints = new Dictionary<uint, ContactPoint>();

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
            Map.World.SetContactListener(this);
        }
        public abstract void Update(float dt);

        public virtual void Draw(float dt, SpriteBatch spriteBatch)
        {
            Position = Body.GetPosition().ToVector2();

            var integerPosition = new Vector2((int)Position.X, (int)Position.Y);
            spriteBatch.Draw(Texture, integerPosition, Color.White);

            PolygonDefs.ForEach(polygonDef =>
            {
                polygonDef.Draw(Body.GetPosition(), spriteBatch);
            });
        }
    }
}
