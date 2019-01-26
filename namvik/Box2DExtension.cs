using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework;

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

        public static Microsoft.Xna.Framework.Color ToXnaColor(this Box2DX.Dynamics.Color color)
        {
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
        }
    }
}
