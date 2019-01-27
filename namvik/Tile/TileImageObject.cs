using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace namvik.Tile
{
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
}