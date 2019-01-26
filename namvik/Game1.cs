using System;
using System.ServiceModel.Activities.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace namvik
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        private Map _map;
        private Camera2D _camera;
        private Character _character;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
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

            _camera.LookAt(_character.Position);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to Draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);

            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var physicsUpdateFrequency = 3;
            for (var i = 0; i < physicsUpdateFrequency; i += 1)
            {
                _map.Update(dt / (float)physicsUpdateFrequency);
            }
            
            _character.Update(dt);

            FollowCameraTo(_character, dt);
        }

        private void FollowCameraTo(Character target, float dt)
        {
            var interpolationFactor = Math.Min(10f * dt, 1);
            var size = _camera.BoundingRectangle.Size;
            var cameraCenter = _camera.Position + new Vector2(size.Width, size.Height) / 2;
            var nextCameraPosition = target.Position * interpolationFactor + cameraCenter * (1 - interpolationFactor);
            _camera.LookAt(nextCameraPosition);
        }

        /// <summary>
        /// This is called when the game should Draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

            _map.Draw(_camera, _spriteBatch);

            _character.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
