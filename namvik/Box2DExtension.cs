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

        public static double ToDegree(this double radius)
        {
            return radius * 180 / Math.PI;
        }

        public static double ToRadius(this double degree)
        {
            return degree / 180 * Math.PI;
        }

        public static float ToFloat(this double value)
        {
            return (float) value;
        }

        public static ContactPoint InMyPerspective(this ContactPoint contactPoint, GameObject me)
        {
            if (contactPoint.Shape2.GetBody().GetUserData() == me)
            {
                return contactPoint;
            }

            var reversedContactPoint = new ContactPoint
            {
                Shape1 = contactPoint.Shape2,
                Shape2 = contactPoint.Shape1,
                Normal = -contactPoint.Normal,
                Velocity = -contactPoint.Velocity,
            };

            return reversedContactPoint;
        }

        public static bool IsMyCollision(this ContactPoint contactPoint, GameObject me)
        {
            return contactPoint.Shape1.GetBody().GetUserData() == me ||
                   contactPoint.Shape2.GetBody().GetUserData() == me;
        }

        public static Vec2 Rotate(this Vec2 vec2, float radian)
        {
            return new Vec2(
                (float)Math.Cos(radian) * vec2.X - (float)Math.Sin(radian) * vec2.Y,
                (float)Math.Sin(radian) * vec2.X + (float)Math.Cos(radian) * vec2.Y);
        }
    }
}
