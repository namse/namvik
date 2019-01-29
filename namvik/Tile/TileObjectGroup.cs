using System.Collections.Generic;
using System.Xml;

namespace namvik.Tile
{
    public enum TileGroupName
    {
        Unknown,
        Background,
        Collision,
        MonsterSpawn,
    }
    class TileObjectGroup
    {
        public string Name;
        public TileGroupName TileGroupName;
        public List<TileObject> TileObjects = new List<TileObject>();

        public void Parse(XmlElement xmlElement)
        {
            Name = xmlElement.GetAttribute("name");
            TileGroupName = ParseTileGroupName(Name);
            foreach (XmlElement childElement in xmlElement.ChildNodes)
            {
                if (childElement.Name == "object")
                {
                    var tileObject = TileObject.Parse(childElement);
                    TileObjects.Add(tileObject);

                    tileObject.TileGroupName = ParseTileGroupName(Name);
                }
            }
        }

        private TileGroupName ParseTileGroupName(string value)
        {
            switch (value)
            {
                case "background":
                    return TileGroupName.Background;
                case "collision":
                    return TileGroupName.Collision;
                case "monsterSpawn":
                    return TileGroupName.MonsterSpawn;
                default:
                    return TileGroupName.Unknown;
            }
        }
    }
}