using System;
using System.Collections.Generic;
using Box2DX.Collision;
using Box2DX.Common;

namespace namvik.Contact
{
    public struct ContactPoint
    {
        public Shape MyShape;
        public Shape OppositeShape;
        public Vec2 Position;
        public Vec2 Velocity;
        public Vec2 Normal; // Me -> Opposite
        public float Separation;
        public float Friction;
        public float Restitution;
        public ContactID Id;
    }

    internal enum CollisionEvent
    {
        Before,
        After,
    }

    class ContactManager: Box2DX.Dynamics.ContactListener
    {
        public static List<ContactListener> ContactListeners = new List<ContactListener>();

        public static void RegisterContactListener(ContactListener contactListener)
        {
            ContactListeners.Add(contactListener);
        }

        private void HandleCollisionEvent(CollisionEvent collisionEvent, Box2DX.Dynamics.ContactPoint point)
        {
            ContactListeners.ForEach(contactListener =>
            {
                var isShape1Me = contactListener == point.Shape1.GetBody().GetUserData();
                var isShape2Me = contactListener == point.Shape2.GetBody().GetUserData();
                var isNotMyBusiness = !isShape1Me && !isShape2Me;
                if (isNotMyBusiness)
                {
                    return;
                }

                var contactPoint = new ContactPoint
                {
                    Velocity = point.Velocity,
                    Normal = (isShape2Me ? -1 : 1) * point.Normal,
                    Position = point.Position,
                    Restitution = point.Restitution,
                    Friction = point.Friction,
                    Id = point.ID,
                    MyShape = isShape1Me ? point.Shape1 : point.Shape2,
                    OppositeShape = isShape1Me ? point.Shape2 : point.Shape1,
                    Separation = point.Separation,
                };

                switch (collisionEvent)
                {
                    case CollisionEvent.Before:
                        contactListener.OnCollisionBefore(contactPoint);
                        break;
                    case CollisionEvent.After:
                        contactListener.OnCollisionAfter(contactPoint);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(collisionEvent), collisionEvent, null);
                }
            });
        }

        public override void Add(Box2DX.Dynamics.ContactPoint point)
        {
            base.Add(point);

            HandleCollisionEvent(CollisionEvent.Before, point);
        }

        public override void Remove(Box2DX.Dynamics.ContactPoint point)
        {
            base.Remove(point);

            HandleCollisionEvent(CollisionEvent.After, point);
        }
    }
}
