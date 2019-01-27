using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace namvik.Tile
{
    class TileImage
    {
        public string Source;
        public Texture2D Texture;
        private static readonly Dictionary<int, Texture2D> Texture2DMap = new Dictionary<int, Texture2D>();
        private readonly int _gid;
        public TileImage(int gid)
        {
            _gid = gid;
        }
        public void Parse(XmlElement xmlElement, ContentManager content)
        {
            Source = xmlElement.GetAttribute("source");
            Console.WriteLine(Source);
            Texture = LoadTexture(content, Source);

            Texture2DMap.Add(_gid, Texture);
        }

        private static Texture2D LoadTexture(ContentManager content, string source)
        {
            var sourceWithoutExtension = source.Replace(".png", "");
            return content.Load<Texture2D>($"map/{sourceWithoutExtension}");
            //return content.Load<Texture2D>($"map/map1");
        }

        public static Texture2D GetTexture(int gid)
        {
            return Texture2DMap.TryGetValue(gid, out var value) ? value : null;
        }
    }
}