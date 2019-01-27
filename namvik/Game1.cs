using System;
using System.Collections.Generic;
using System.ServiceModel.Activities.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using namvik.Tile;

namespace namvik
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        private Map _map;
        private Camera2D _camera;
        private Character _character;
        private FpsPrinter _fpsPrinter;

        public static SpriteFont DefaultFont;

        private List<GameObject> _gameObjects = new List<GameObject>();


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            var windowWidth = 1000;
            var windowHeight = 1000;
            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;
            _graphics.ApplyChanges();

            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);


            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, windowWidth, windowHeight);
            _camera = new Camera2D(viewportAdapter);


            _map = new Map();
            _map.Initialize(Content, _spriteBatch);

            _character = new Character();
            _character.Initialize(Content);
            _gameObjects.Add(_character);

            _camera.LookAt(_character.Position);

            _fpsPrinter = new FpsPrinter();
            _fpsPrinter.Initialize(Content);

            DefaultFont = Content.Load<SpriteFont>("font/defaultFont");


            KeyboardManager.OnKeyPress(Keys.F2, (key) =>
            {
                var monkey = Monkey.SpawnMonkey(Content, _character.Position);
                _gameObjects.Add(monkey);
            });
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to Draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);

            KeyboardManager.Update();

            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var physicsUpdateFrequency = 3;
            for (var i = 0; i < physicsUpdateFrequency; i += 1)
            {
                _map.Update(dt / (float)physicsUpdateFrequency);
            }

            _gameObjects.ForEach(gameObject =>
            {
                gameObject.Update(dt);
            });

            FollowCameraTo(_character, dt);

            _fpsPrinter.Update(dt);
        }

        private void FollowCameraTo(Character target, float dt)
        {
            var interpolationFactor = Math.Min(10f * dt, 1);
            var size = _camera.BoundingRectangle.Size;
            var cameraCenter = _camera.Position + new Vector2(size.Width, size.Height) / 2;
            var nextCameraPosition = target.Position * interpolationFactor + cameraCenter * (1 - interpolationFactor);
            _camera.LookAt(nextCameraPosition);
        }

        protected override void Draw(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

            _map.Draw(_camera, _spriteBatch);

            _gameObjects.ForEach(gameObject =>
            {
                gameObject.Draw(dt, _spriteBatch);
            });

            _spriteBatch.End();

            _spriteBatch.Begin();
            _fpsPrinter.Draw(dt, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
