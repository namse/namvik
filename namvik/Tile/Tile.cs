using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace namvik.Tile
{
    class Tile
    {
        public int Id;
        public List<TileImage> Images = new List<TileImage>();
        private readonly int _firstGid;
        public Tile(int firstGid)
        {
            _firstGid = firstGid;
        }

        public void Parse(XmlElement xmlElement, ContentManager content)
        {
            Id = int.Parse(xmlElement.GetAttribute("id"));
            foreach (XmlElement childElement in xmlElement.ChildNodes)
            {
                if (childElement.Name == "image")
                {
                    var image = new TileImage(_firstGid + Id);
                    image.Parse(childElement, content);
                    Images.Add(image);
                }
            }
        }
    }
}