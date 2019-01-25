using Box2DX.Common;
using Microsoft.Xna.Framework;

namespace namvik
{
    public static class Vector2Extension
    {
        public static Vec2 ToVec2(this Vector2 vector2)
        {
            return new Vec2(vector2.X.ToMeter(), vector2.Y.ToMeter());
        }
    }
}
