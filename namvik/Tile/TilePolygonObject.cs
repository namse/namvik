using System;
using System.Collections.Generic;
using System.Linq;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Color = Microsoft.Xna.Framework.Color;
using Math = System.Math;

namespace namvik.Tile
{
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
                    Vertices = points.Select(point => Vector2Extension.ToVec2(point)).ToArray()
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
}