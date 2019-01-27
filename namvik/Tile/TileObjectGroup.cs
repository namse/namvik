using System.Collections.Generic;
using System.Xml;

namespace namvik.Tile
{
    public enum TileGroup
    {
        Collision,
    }
    class TileObjectGroup
    {
        public string Name;
        public List<TileObject> TileObjects = new List<TileObject>();

        public void Parse(XmlElement xmlElement)
        {
            Name = xmlElement.GetAttribute("name");
            foreach (XmlElement childElement in xmlElement.ChildNodes)
            {
                if (childElement.Name == "object")
                {
                    var tileObject = TileObject.Parse(childElement);
                    TileObjects.Add(tileObject);

                    if (Name == "collision")
                    {
                        tileObject.TileGroup = TileGroup.Collision;
                    }
                }
            }
        }
    }
}