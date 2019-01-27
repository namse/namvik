using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace namvik.Tile
{
    class TileSet
    {
        private int _firstGid;
        private List<Tile> _tileList = new List<Tile>();

        public void Parse(XmlElement xmlElement, ContentManager content)
        {
            _firstGid = int.Parse(xmlElement.GetAttribute("firstgid"));

            foreach (XmlElement childElement in xmlElement.ChildNodes)
            {
                if (childElement.Name == "tile")
                {
                    var tile = new Tile(_firstGid);
                    tile.Parse(childElement, content);
                    _tileList.Add(tile);
                }
            }

        }
    }
}