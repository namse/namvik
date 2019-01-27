using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace namvik.Tile
{
    class TileObject
    {
        public float X;
        public float Y;
        public TileGroup TileGroup;

        public TileObject(float x, float y)
        {
            X = x;
            Y = y;
        }
        public TileObject(TileObject tileObject)
        {
            X = tileObject.X;
            Y = tileObject.Y;
        }
        public static TileObject Parse(XmlElement xmlElement)
        {
            var x = float.Parse(xmlElement.GetAttribute("x"));
            var y = float.Parse(xmlElement.GetAttribute("y"));
            if (xmlElement.HasAttribute("width"))
            {
                var width = float.Parse(xmlElement.GetAttribute("width"));
                var height = float.Parse(xmlElement.GetAttribute("height"));
                var hasRotation = xmlElement.HasAttribute("rotation");
                var rotation = hasRotation ? float.Parse(xmlElement.GetAttribute("rotation")) : 0;
                if (xmlElement.HasAttribute("gid")) {
                    var gid = int.Parse(xmlElement.GetAttribute("gid"));
                    return new TileImageObject(x, y, width, height, gid);
                }
                return new TileRectangleObject(x, y, width, height, rotation);
            }
            TilePolygon polygon = null;
            foreach (XmlElement item in xmlElement.ChildNodes)
            {
                if (item.Name == "polygon")
                {
                    polygon = new TilePolygon();
                    polygon.Parse(item);
                }
            }
            if (polygon != null)
            {
                return new TilePolygonObject(x, y, polygon);
            }


            return new TileObject(x, y);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}