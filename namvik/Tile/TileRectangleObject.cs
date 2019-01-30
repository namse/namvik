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
    class TileRectangleObject: TileObject
    {
        public float Width;
        public float Height;
        private float _angleDegree;
        public Body Body;
        private readonly PolygonDef _polygonDef;
        private readonly List<Vector2> _points;

        public TileRectangleObject(int id, float x, float y, float width, float height, float angleDegree) : base(id, x, y)
        {
            Width = width;
            Height = height;
            _angleDegree = angleDegree;

            _points = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(0, 0 + height),
                new Vector2( width, height),
                new Vector2(width, 0)
            }.Select(point => point.Rotate(angleDegree.ToRadius())).ToList();


            var bodyDef = new BodyDef
            {
                Position = new Vec2(x.ToMeter(), y.ToMeter())
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
}