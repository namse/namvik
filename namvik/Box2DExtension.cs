using Box2DX.Common;
using Microsoft.Xna.Framework;

namespace namvik
{
    public static class Box2DExtension
    {
        public static Vector2 ToVector2(this Vec2 vec2)
        {
            return new Vector2(vec2.X, vec2.Y);
        }

        public static Microsoft.Xna.Framework.Color ToXnaColor(this Box2DX.Dynamics.Color color)
        {
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
        }
    }
}
