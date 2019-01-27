using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;

namespace namvik.Tile
{
    class TilePolygon
    {
        public List<Vector2> Points = new List<Vector2>();
        public void Parse(XmlElement xmlElement)
        {
            var points = xmlElement.GetAttribute("points");
            Points = points.Split(' ').Select((pointString) =>
            {
                var strings = pointString.Split(',');
                var x = float.Parse(strings[0]);
                var y = float.Parse(strings[1]);
                return new Vector2(x, y);
            }).ToList();
        }
    }
}