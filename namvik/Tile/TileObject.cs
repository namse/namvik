using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace namvik.Tile
{
    enum TileObjectType
    {
        Unknown,
        Polygon,
        Point,
    }
    class TileObject
    {
        public float X;
        public float Y;
        public TileGroupName TileGroupName;
        public int Id;

        public TileObject(int id,float x, float y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public static TileObject Parse(XmlElement xmlElement)
        {
            var x = float.Parse(xmlElement.GetAttribute("x"));
            var y = float.Parse(xmlElement.GetAttribute("y"));
            var id = int.Parse(xmlElement.GetAttribute("id"));
            if (xmlElement.HasAttribute("width"))
            {
                var width = float.Parse(xmlElement.GetAttribute("width"));
                var height = float.Parse(xmlElement.GetAttribute("height"));
                var hasRotation = xmlElement.HasAttribute("rotation");
                var rotation = hasRotation ? float.Parse(xmlElement.GetAttribute("rotation")) : 0;
                if (xmlElement.HasAttribute("gid")) {
                    var gid = int.Parse(xmlElement.GetAttribute("gid"));
                    return new TileImageObject(id, x, y, width, height, gid);
                }
                return new TileRectangleObject(id, x, y, width, height, rotation);
            }
            TilePolygon polygon = null;
            TileObjectType tileObjectType = TileObjectType.Unknown;
            foreach (XmlElement item in xmlElement.ChildNodes)
            {
                switch (item.Name)
                {
                    case "polygon":
                        polygon = new TilePolygon();
                        polygon.Parse(item);
                        tileObjectType = TileObjectType.Polygon;
                        break;
                    case "point":
                        tileObjectType = TileObjectType.Point;
                        break;
                }
            }

            switch (tileObjectType)
            {
                case TileObjectType.Polygon:
                    return new TilePolygonObject(id, x, y, polygon);
                case TileObjectType.Point:
                    return new TilePointObject(id, x, y);
                default:
                    return new TileObject(id, x, y);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}