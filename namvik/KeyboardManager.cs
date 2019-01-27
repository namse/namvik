using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace namvik
{
    using Callback = Action<Keys>;

    class KeyInfo
    {
        public List<Callback> Callbacks = new List<Callback>();
        public bool IsKeyUp = true;
    }

    public static class KeyboardManager
    {
        private static readonly Dictionary<Keys, KeyInfo> KeyInfoDictionary = new Dictionary<Keys, KeyInfo>();
        public static void OnKeyPress(Keys key, Callback callback)
        {
            if (!KeyInfoDictionary.TryGetValue(key, out var keyInfo))
            {
                keyInfo = new KeyInfo();
                KeyInfoDictionary.Add(key, keyInfo);
            }
            keyInfo.Callbacks.Add(callback);
        }

        public static void Update()
        {
            foreach (var keyValuePair in KeyInfoDictionary)
            {
                var key = keyValuePair.Key;
                var keyInfo = keyValuePair.Value;

                var isTriggerOn = keyInfo.IsKeyUp && Keyboard.GetState().IsKeyDown(key);

                if (isTriggerOn)
                {
                    keyInfo.Callbacks.ForEach(callback => callback.Invoke(key));
                    keyInfo.IsKeyUp = false;
                }

                if (Keyboard.GetState().IsKeyUp(key))
                {
                    keyInfo.IsKeyUp = true;
                }
            }
        }
    }
}
