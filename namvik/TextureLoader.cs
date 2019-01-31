using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace namvik
{
    public static class TextureLoader
    {
        private static readonly Dictionary<Type,  Texture2D> _textureMap = new Dictionary<Type, Texture2D>();

        public static void LoadTexture(Type type, ContentManager contentManager, string assetName)
        {
            var texture = contentManager.Load<Texture2D>(assetName);
            _textureMap.Add(type, texture);
        }

        public static Texture2D GetTexture(Type type)
        {
            return _textureMap.TryGetValue(type, out var texture) ? texture : null;
        }
    }
}
