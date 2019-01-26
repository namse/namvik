using System.Linq;
using System.Runtime.InteropServices;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using Color = Microsoft.Xna.Framework.Color;
using Math = System.Math;

namespace namvik
{
    public static class Box2DExtension
    {
        public static Vector2 ToVector2(this Vec2 vec2)
        {
            return new Vector2(vec2.X.ToPixel(), vec2.Y.ToPixel());
        }

        public static string Print(this Vec2 vec2)
        {
            return $"x: {vec2.X} y: {vec2.Y}";
        }

        public static void SetVelocityY(this Body body, float y)
        {
            var velocity = body.GetLinearVelocity();
            velocity.Y = y;
            body.SetLinearVelocity(velocity);
        }

        public static void SetVelocityX(this Body body, float x)
        {
            var velocity = body.GetLinearVelocity();
            velocity.X = x;
            body.SetLinearVelocity(velocity);
        }

        public static Microsoft.Xna.Framework.Color ToXnaColor(this Box2DX.Dynamics.Color color)
        {
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
        }

        public static void Draw(this PolygonDef polygonDef, Vec2 offset, SpriteBatch spriteBatch)
        {
            var physicsPolygon = polygonDef.Vertices.Take(polygonDef.VertexCount).Select(vec2 => vec2.ToVector2()).ToArray();
            spriteBatch.DrawPolygon(offset.ToVector2(), physicsPolygon, Color.GreenYellow, 2f);
        }

        public static float ToDegree(this float radius)
        {
            return radius * 180f / (float)Math.PI;
        }

        public static float ToRadius(this float degree)
        {
            return degree / 180f * (float)Math.PI;
        }
    }
}
