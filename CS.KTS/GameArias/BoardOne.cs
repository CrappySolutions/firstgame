using CS.KTS.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace CS.KTS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BoardOne : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        CS.KTS.Sprites.InputControlSprite _controls;
        private RenderTarget2D _renderTarget;
    private bool firstUpdate = true;
    private Sprite background;

        public BoardOne()
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
            TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Tap | GestureType.Hold; 
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
      if (firstUpdate)
      {
        // Temp hack to fix gestures
        typeof(Microsoft.Xna.Framework.Input.Touch.TouchPanel)
            .GetField("_touchScale", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .SetValue(null, Vector2.One);

        firstUpdate = false;
      }

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _controls = new Sprites.InputControlSprite(Content, _graphics);
            _renderTarget = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.GraphicsDevice.Viewport.Height, _graphics.GraphicsDevice.Viewport.Width, false, SurfaceFormat.Color, DepthFormat.Depth16);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

          GraphicsDevice.SetRenderTarget(_renderTarget);   
          GraphicsDevice.Clear(Color.White);
          _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
          _controls.Draw(_spriteBatch);
          _spriteBatch.End();
          base.Draw(gameTime);

          GraphicsDevice.SetRenderTarget(null); 
          _spriteBatch.Begin();

      //1280 x 720, 385 280, 400 240
          DrawLandscape(385, 200);

          _spriteBatch.End();
        }


        private void DrawLandscape(float x, float y)
        {
          _spriteBatch.Draw(_renderTarget, new Vector2(x, y), null, Color.White, MathHelper.PiOver2, new Vector2(y, x), 1f, SpriteEffects.None, 0);
        }
    }
}
