using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Color = Microsoft.Xna.Framework.Color;
using Math = System.Math;

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

                    if (Name == "collision")
                    {
                        tileObject.TileGorup = TileGroup.Collision;
                    }
                }
            }
        }
    }

    public enum TileGroup
    {
        Collision,
    }


    class TileObject
    {
        public float X;
        public float Y;
        public TileGroup TileGorup;

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

    class TileRectangleObject: TileObject
    {
        public float Width;
        public float Height;
        private float _angleDegree;
        public Body Body;
        private PolygonDef _polygonDef;
        private List<Vector2> _points;

        public TileRectangleObject(float x, float y, float width, float height, float angleDegree) : base(x, y)
        {
            Width = width;
            Height = height;
            _angleDegree = angleDegree;

            _points = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(0, 0 + height),
                new Vector2( width, height),
                new Vector2(width, 0),
            }.Select(point => point.Rotate(angleDegree.ToRadius())).ToList();


            var bodyDef = new BodyDef
            {
                Position = new Vec2(x.ToMeter(), y.ToMeter()),
            };

            Body = Map.World.CreateBody(bodyDef);

            _polygonDef = new PolygonDef();

            var hx = (width / 2f).ToMeter();
            var hy = (height / 2f).ToMeter();

            var angleRadian = _angleDegree.ToRadius();

            var rotatedHx = (float)(Math.Cos(angleRadian) * hx - Math.Sin(angleRadian) * hy);
            var rotatedHy = (float)(Math.Sin(angleRadian) * hx + Math.Cos(angleRadian) * hy);

            _polygonDef.SetAsBox(hx, hy, new Vec2(rotatedHx, rotatedHy), angleRadian);

            Body.CreateShape(_polygonDef);

            Body.SetUserData(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            var physicsPolygons = _polygonDef.Vertices.Take(_polygonDef.VertexCount).Select(vec2 => vec2.ToVector2()).ToArray();
            spriteBatch.DrawPolygon(Body.GetPosition().ToVector2(), physicsPolygons, Color.GreenYellow, 2f);

            spriteBatch.DrawPolygon(new Vector2(X, Y), _points.ToArray(), Color.Red);
        }
    }

    class TileImageObject : TileObject
    {
        public readonly int Gid;
        public float Width;
        public float Height;

        public TileImageObject(float x, float y, float width, float height, int gid) : base(x, y)
        {
            Gid = gid;
            Width = width;
            Height = height;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            var texture = TileImage.GetTexture(Gid);
            spriteBatch.Draw(texture, new Vector2(X, Y - Height), Color.White);
        }
    }


    class TilePolygonObject : TileObject
    {
        public TilePolygon Polygon;
        public Body Body;
        private readonly List<PolygonDef> _polygonDefs;
        public TilePolygonObject(float x, float y, TilePolygon polygon): base(x, y) {
            X = x;
            Y = y;
            Polygon = polygon;

            var bodyDef = new BodyDef
            {
                Position = new Vec2(x.ToMeter(), y.ToMeter()),
            };

            Body = Map.World.CreateBody(bodyDef);

            var dividedPolygon = DividePolygon(polygon.Points);

            _polygonDefs = dividedPolygon.Select(points =>
            {
                points = MakePolygonCw(points);
                var polygonDef = new PolygonDef
                {
                    VertexCount = points.Count,
                    Vertices = points.Select(point => point.ToVec2()).ToArray()
                };
                return polygonDef;
            }).ToList();

            _polygonDefs.ForEach(def => { Body.CreateShape(def); });

            Body.SetUserData(this);
        }

        private List<Vector2> MakePolygonCw(List<Vector2> points)
        {
            if (IsPolygonClockWise(points))
            {
                return points;
            }

            points.Reverse();
            return points;
        }

        public bool IsConvexPolygon(List<Vector2> points)
        {
            for (var i = 0; i < points.Count; i += 1)
            {
                var pointA = points[i];
                var pointB = points[(i + 1) % points.Count];
                var pointC = points[(i + 2) % points.Count];

                if (!IsClockWise(pointA, pointB, pointC))
                {
                    return false;
                }
            }
            return true;
        }

        public List<List<Vector2>> DivideConvexPolygon(List<Vector2> points)
        {
            var chunkedPoints = new List<List<Vector2>>();
            var firstPoint = points[0];
            // devide
            {
                var pointChunk = new List<Vector2>();
                for (var i = 1; i < points.Count; i += 1)
                {
                    var point = points[i];
                    pointChunk.Add(point);

                    if (pointChunk.Count >= 6)
                    {
                        chunkedPoints.Add(pointChunk);
                        pointChunk = new List<Vector2>();
                    }
                }

                if (pointChunk.Count > 0)
                {
                    chunkedPoints.Add(pointChunk);
                }
            }

            // insert
            for (var i = 0; i < chunkedPoints.Count; i += 1)
            {
                var pointChunk = chunkedPoints[i];
                pointChunk.Insert(0, firstPoint);
                if (i >= 1)
                {
                    var lastPointChunk = chunkedPoints[i - 1];
                    pointChunk.Insert(1, lastPointChunk.Last());
                }
            }

            return chunkedPoints;
        }

        public bool IsClockWise(Vector2 pointA, Vector2 pointB, Vector2 pointC)
        {
            var v1x = pointB.X - pointC.X;
            var v1y = pointB.Y - pointC.Y;
            var v2x = pointA.X - pointC.X;
            var v2y = pointA.Y - pointC.Y;

            var angle = Math.Atan2(v1x, v1y) - Math.Atan2(v2x, v2y);
            return angle >= Math.PI || angle < 0;
        }

        public bool IsLeftGoingLineContactWithLine(Vector2 startPoint, Vector2 pointA, Vector2 pointB)
        {
            var bigY = Math.Max(pointA.Y, pointB.Y);
            var smallY = Math.Min(pointA.Y, pointB.Y);
            var bigX = Math.Max(pointA.X, pointB.X);
            var smallX = Math.Min(pointA.X, pointB.X);

            if (pointA.X == pointB.X) {
                return smallY <= startPoint.Y && startPoint.Y <= bigY;
            }
            if (pointA.Y == pointB.Y) {
                return pointA.Y == startPoint.Y;
            }

            var a = (pointA.Y - pointB.Y) / (pointA.X - pointB.X);
            var b = pointA.Y - (pointA.Y - pointB.Y) / (pointA.X - pointB.X) * pointA.X;

            var xOnLine = (startPoint.Y - b) / a;

            return xOnLine <= startPoint.X && smallX <= xOnLine && xOnLine <= bigX;
        }

        public bool IsPointInPolygon(Vector2 target, List<Vector2> points)
        {
            var contactCount = 0;
            for (var i = 0; i < points.Count; i += 1)
            {
                var pointA = points[i];
                var pointB = points[(i + 1) % points.Count];
                if (IsLeftGoingLineContactWithLine(target, pointA, pointB)) {
                    contactCount += 1;
                }
            }
            return contactCount % 2 == 1;
        }

        public List<List<Vector2>> DividePolygonByLine(int indexA, int indexB, List<Vector2> points)
        {
            if (indexB < indexA)
            {
                int temp = indexB;
                indexB = indexA;
                indexA = temp;
            }
            var nextPolygonA = points.Skip(indexA).Take(indexB - indexA + 1).ToList();

            var nextPolygonB = new List<Vector2>();
            for (var j = 0; j <= indexA; j += 1)
            {
                nextPolygonB.Add(points[j]);
            }
            for (var j = indexB; j < points.Count; j += 1)
            {
                nextPolygonB.Add(points[j]);
            }

            return DividePolygon(nextPolygonA).Concat(DividePolygon((nextPolygonB))).ToList();
        }

        public bool IsPolygonClockWise(List<Vector2> points)
        {
            var sum = 0f;
            for (var i = 0; i < points.Count; i += 1)
            {
                var indexA = i;
                var indexB = (i + 1) % points.Count;

                var pointA = points[indexA];
                var pointB = points[indexB];

                sum += (pointB.X - pointA.X) * (pointB.Y + pointA.Y);
            }

            return sum < 0;
        }

        public List<List<Vector2>> DividePolygon(List<Vector2> points)
        {
            if (points.Count == 3)
            {
                return new List<List<Vector2>>{ points };
            }
            if (IsConvexPolygon(points))
            {
                return DivideConvexPolygon(points);
            }

            var isPolygonClockWise = IsPolygonClockWise(points);

            if (points.Count == 15)
            {
                Console.WriteLine("sex");
            }

            for (var i = 0; i < points.Count; i += 1)
            {
                var indexA = i;
                var indexB = (i + 1) % points.Count;
                var indexC = (i + 2) % points.Count;

                var pointA = points[indexA];
                var pointB = points[indexB];
                var pointC = points[indexC];

                var isTriangleInDifferentWiseWithPolygon = IsClockWise(pointA, pointB, pointC) != isPolygonClockWise;

                if (isTriangleInDifferentWiseWithPolygon)
                {
                    continue;
                }

                var trianglePoints = new List<Vector2>
                {
                    pointA,
                    pointB,
                    pointC,
                };
                var isAnyPointInsideOfTriangle = points.Except(trianglePoints)
                        .Any(point => IsPointInPolygon(point, trianglePoints));
                if (!isAnyPointInsideOfTriangle)
                {
                    return DividePolygonByLine(indexA, indexC, points);
                }
            }
            throw new Exception("no way");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //var index = 0;
            _polygonDefs.ForEach(polygonDef =>
            {
                var physicsPolygon = polygonDef.Vertices.Take(polygonDef.VertexCount).Select(vec2 => vec2.ToVector2()).ToArray();
                spriteBatch.DrawPolygon(Body.GetPosition().ToVector2(), physicsPolygon, Color.GreenYellow, 2f);

                //foreach (var point in physicsPolygon)
                //{
                //    spriteBatch.DrawString(Game1.DefaultFont, index.ToString(), Body.GetPosition().ToVector2() + point, Color.Black);

                //    index += 1;
                //}
            });

            spriteBatch.DrawPolygon(new Vector2(X, Y), Polygon.Points.ToArray(), Color.Red);
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
                var y = float.Parse(strings[1]);
                return new Vector2(x, y);
            }).ToList();
        }
    }

    public class Map
    {
        private List<TileSet> _tileSetList = new List<TileSet>();
        private List<TileObjectGroup> _tileObjectGroups = new List<TileObjectGroup>();

        public static World World;

        public void Initialize(ContentManager content, SpriteBatch spriteBatch)
        {
            InitializeWorld(spriteBatch);

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

        public void InitializeWorld(SpriteBatch spriteBatch)
        {
            var worldAabb = new AABB();
            worldAabb.LowerBound.Set(float.NegativeInfinity, float.NegativeInfinity);
            worldAabb.UpperBound.Set(float.PositiveInfinity, float.PositiveInfinity);

            var gravity = new Vec2(0f, 10f);

            World = new World(worldAabb, gravity, false);

            var box2DDebugDraw = new Box2DDebugDraw(spriteBatch)
            {
                //Flags = DebugDraw.DrawFlags.Shape,
            };

            World.SetDebugDraw(box2DDebugDraw);
        }

        public void Update(float dt)
        {
            World.Step(dt, 8, 3);
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