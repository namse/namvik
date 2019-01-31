using System;
using System.Collections.Generic;
using Box2DX.Collision;

namespace namvik.Contact
{
    public abstract class ContactListener
    {
        private readonly Dictionary<(ContactID, Shape), ContactPoint> _contactPointDictionary = new Dictionary<(ContactID, Shape), ContactPoint>();

        protected IEnumerable<ContactPoint> ContactPoints => _contactPointDictionary.Values;

        private void AddContactPoint(ContactPoint point)
        {
            var key = (point.Id, point.OppositeShape);

            if (!_contactPointDictionary.ContainsKey(key))
            {
                _contactPointDictionary.Add(key, point);
            }
        }

        private void RemoveContactPoint(ContactPoint point)
        {
            var key = (point.Id, point.OppositeShape);
            
            if (_contactPointDictionary.ContainsKey(key))
            {
                _contactPointDictionary.Remove(key);
            }
        }

        public virtual void OnCollisionBefore(ContactPoint point)
        {
            AddContactPoint(point);
        }

        public virtual void OnCollisionAfter(ContactPoint point)
        {
            RemoveContactPoint(point);
        }
    }
}