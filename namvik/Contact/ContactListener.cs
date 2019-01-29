using System.Collections.Generic;

namespace namvik.Contact
{
    public abstract class ContactListener
    {
        private readonly Dictionary<uint, ContactPoint> _contactPointDictionary = new Dictionary<uint, ContactPoint>();

        protected IEnumerable<ContactPoint> ContactPoints => _contactPointDictionary.Values;

        private void AddContactPoint(ContactPoint point)
        {
            var key = point.Id.Key;

            if (!_contactPointDictionary.ContainsKey(key))
            {
                _contactPointDictionary.Add(key, point);
            }
        }

        private void RemoveContactPoint(ContactPoint point)
        {
            var key = point.Id.Key;

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