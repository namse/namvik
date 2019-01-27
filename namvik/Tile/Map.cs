using System;
using System.Collections.Generic;
using System.Xml;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace namvik.Tile
{
    public class Map
    {
        private readonly List<TileSet> _tileSetList = new List<TileSet>();
        private readonly List<TileObjectGroup> _tileObjectGroups = new List<TileObjectGroup>();

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
            worldAabb.LowerBound.Set(-100f, -100f);
            worldAabb.UpperBound.Set(100f, 100f);

            var gravity = new Vec2(0f, 0f);

            World = new World(worldAabb, gravity, false);
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