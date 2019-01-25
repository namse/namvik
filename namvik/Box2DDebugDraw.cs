using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Color = Box2DX.Dynamics.Color;

namespace namvik
{
    public class Box2DDebugDraw: DebugDraw
    {
        private SpriteBatch _spriteBatch;

        public Box2DDebugDraw(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }
        public override void DrawPolygon(Vec2[] vertices, int vertexCount, Color color)
        {
            var firstVertex = vertices[0].ToVector2();
            var vectors = vertices.Take(vertexCount).Select(vertex => vertex.ToVector2() - firstVertex).ToArray();
            _spriteBatch.DrawPolygon(firstVertex, vectors, color.ToXnaColor());
        }

        public override void DrawSolidPolygon(Vec2[] vertices, int vertexCount, Color color)
        {
            DrawPolygon(vertices, vertexCount, color);
        }

        public override void DrawCircle(Vec2 center, float radius, Color color)
        {
            _spriteBatch.DrawCircle(center.ToVector2(), radius, 32, color.ToXnaColor());
        }

        public override void DrawSolidCircle(Vec2 center, float radius, Vec2 axis, Color color)
        {
            DrawCircle(center, radius, color);
        }

        public override void DrawSegment(Vec2 p1, Vec2 p2, Color color)
        {
            _spriteBatch.DrawLine(p1.ToVector2(), p2.ToVector2(), color.ToXnaColor());
        }

        public override void DrawXForm(XForm xf)
        {
            _spriteBatch.DrawPoint(xf.Position.ToVector2(), Microsoft.Xna.Framework.Color.Red);
        }
    }
}
