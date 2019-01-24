using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;

namespace namvik
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
    class Tile
    {
        public int Id;
        public List<TileImage> Images = new List<TileImage>();
        private int _firstGid;
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
                }
            }
        }
    }

    class TileObject
    {
        public float X;
        public float Y;

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
                if (xmlElement.HasAttribute("gid")) {
                    var gid = int.Parse(xmlElement.GetAttribute("gid"));
                    return new TileImageObject(x, y, width, height, gid);
                }
                return new TileRectangleObject(x, y, width, height);
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

    class TileRectangleObject: TileObject
    {
        public float Width;
        public float Height;

        public TileRectangleObject(float x, float y, float width, float height) : base(x, y)
        {
            Width = width;
            Height = height;
        }
    }

    class TileImageObject : TileRectangleObject
    {
        public readonly int Gid;

        public TileImageObject(float x, float y, float width, float height, int gid) : base(x, y, width, height)
        {
            Gid = gid;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            var texture = TileImage.GetTexture(Gid);
            spriteBatch.Draw(texture, new Vector2(X, Y), Color.White);
        }
    }


    class TilePolygonObject : TileObject
    {
        public TilePolygon Polygon;
        public TilePolygonObject(float x, float y, TilePolygon polygon): base(x, y) {
            X = x;
            Y = y;
            Polygon = polygon;
        }
    }

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
                var y = float.Parse(strings[0]);
                return new Vector2(x, y);
            }).ToList();
        }
    }



    public class Map
    {
        private List<TileSet> _tileSetList = new List<TileSet>();
        private List<TileObjectGroup> _tileObjectGroups = new List<TileObjectGroup>();

        public void Initialize(ContentManager content)
        {
            var doc = new XmlDocument();
            var reader = new XmlTextReader("Content/map/map1.tmx");
            doc.Load(reader);

            if (!(doc.ChildNodes[1] is XmlElement mapXmlElement))
            {
                throw new Exception("cannot find map xml");
            }
            foreach (XmlElement childNode in mapXmlElement.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "tileset":
                        var tileSet = new TileSet();
                        tileSet.Parse(childNode, content);
                        _tileSetList.Add(tileSet);
                        break;
                    case "objectgroup":
                        var tileObjectGroup = new TileObjectGroup();
                        tileObjectGroup.Parse(childNode);
                        _tileObjectGroups.Add(tileObjectGroup);
                        break;
                }
            }
            Console.WriteLine(_tileSetList);
        }

        public void Draw(Camera2D camera, SpriteBatch spriteBatch)
        {
            _tileObjectGroups.ForEach(tileObjectGroup =>
            {
                tileObjectGroup.TileObjects.ForEach(tileObject =>
                {
                    tileObject.Draw(spriteBatch);
                });
            });
        }
    }
}