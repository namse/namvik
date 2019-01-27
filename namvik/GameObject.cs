using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace namvik
{
    public abstract class GameObject : ContactListener
    {
        protected bool HasMass;
        protected Texture2D Texture;
        public Vector2 Position;
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

        public abstract void Initialize(ContentManager content);
        public abstract void Update(float dt);
        public abstract void Draw(float dt, SpriteBatch spriteBatch);
    }
}
