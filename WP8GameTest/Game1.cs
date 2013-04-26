using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using WP8GameTest.Entities;
using WP8GameTest.Sprites;
using System.Windows;

namespace WP8GameTest
{

  public class Game1 : Microsoft.Xna.Framework.Game
  {
    #region Private

    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;
    HorizontallyScrollingBackground mScrollingBackground1;
    HorizontallyScrollingBackground mScrollingBackground2;
    private RenderTarget2D renderTarget; // what game world is rendered on before being rotated
    private int GameWindowWidth; // the width of the game window we are rendering to 
    private int GameWindowHeight; // the height of the game window we are rendering to
    private bool changeDude = true;
    // private Player player;
    private bool firstUpdate = true;

    private Sprite bigStick;
    private TouchButton smallStick;

    private Vector2 movementControlPosition;

    private TouchButton buttonA;
    private TouchButton buttonB;

    private AnimatedPlayer animatedPlayer;

    #endregion

    public Game1()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Tap | GestureType.Hold | GestureType.Hold;
    }

    protected override void Initialize()
    {
      // TODO: Add your initialization logic here

      GameWindowWidth = graphics.GraphicsDevice.Viewport.Height;
      GameWindowHeight = graphics.GraphicsDevice.Viewport.Width;

      TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Tap | GestureType.Hold;

      base.Initialize();
    }

    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);

      //Initialize and add the background images to the Scrolling background. You can change the
      //scroll area by passing in a different Viewport. The images will then scale and scroll within
      //that given Viewport.
      mScrollingBackground1 = new HorizontallyScrollingBackground(this.GraphicsDevice.Viewport);
      mScrollingBackground1.AddBackground("bg11");
      mScrollingBackground1.AddBackground("bg22");
      mScrollingBackground1.AddBackground("bg33");

      mScrollingBackground2 = new HorizontallyScrollingBackground(this.GraphicsDevice.Viewport);
      mScrollingBackground2.AddBackground("bg1");
      mScrollingBackground2.AddBackground("bg2");
      mScrollingBackground2.AddBackground("bg3");

      //Load the content for the Scrolling background
      mScrollingBackground1.LoadContent(this.Content);
      mScrollingBackground2.LoadContent(this.Content);

      renderTarget = new RenderTarget2D(GraphicsDevice, GameWindowWidth, GameWindowHeight, false, SurfaceFormat.Color, DepthFormat.Depth16);

      bigStick = new Sprite();
      bigStick.LoadContent(Content, "stickBig");
      bigStick.Scale = 2f;
      bigStick.Position = new Vector2(50, GameWindowHeight - bigStick.Size.Height);

      smallStick = new TouchButton();
      smallStick.LoadContent(Content, "stickSmall");
      smallStick.Scale = 2f;
      smallStick.Position = new Vector2(50 + (bigStick.Size.Width / 4), GameWindowHeight - (bigStick.Size.Height / 2) - (smallStick.Size.Height / 2));

      movementControlPosition = smallStick.Position;

      buttonA = new TouchButton();
      buttonA.LoadContent(Content, "stickSmall");
      buttonA.Scale = 2f;
      buttonA.Position = new Vector2(GameWindowWidth - buttonA.Size.Width - 20, GameWindowHeight - (buttonA.Size.Height * 2));

      buttonB = new TouchButton();
      buttonB.LoadContent(Content, "stickSmall");
      buttonB.Scale = 2f;
      buttonB.Position = new Vector2(GameWindowWidth - (buttonA.Size.Width * 2) - 20, GameWindowHeight - (buttonA.Size.Height));

      //player = new Player();
      //player.LoadContent(this.Content);

      animatedPlayer = new AnimatedPlayer();
      animatedPlayer.LoadContent(Content, "SmileyWalk", 4, 4, new Vector2(100, 500));
    }

    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    protected override void Update(GameTime gameTime)
    {
      if (firstUpdate)
      {
        // Temp hack to fix gestures
        typeof(Microsoft.Xna.Framework.Input.Touch.TouchPanel)
            .GetField("_touchScale", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .SetValue(null, Vector2.One);

        firstUpdate = false;
      }

      HandleTouch();

      var playerMoveDirection = GetMovementControllerDirection();
      playerMoveDirection = animatedPlayer.CanMove(GameWindowWidth, GameWindowHeight, playerMoveDirection);
      //player.Update(gameTime, playerMoveDirection);

      if (playerMoveDirection == MoveDirection.Right)
      {
        mScrollingBackground1.Update(gameTime, 10, HorizontallyScrollingBackground.HorizontalScrollDirection.Left);
        mScrollingBackground2.Update(gameTime, 50, HorizontallyScrollingBackground.HorizontalScrollDirection.Left);
      }

      animatedPlayer.Update(gameTime, playerMoveDirection);

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

      // -------------------------------------
      // -- render to the render target buffer
      // -------------------------------------
      GraphicsDevice.SetRenderTarget(renderTarget); // set our target to buffer
      graphics.GraphicsDevice.Clear(Color.White);

      spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

      mScrollingBackground1.Draw(spriteBatch);
      mScrollingBackground2.Draw(spriteBatch);

      //player.Draw(spriteBatch);

      bigStick.Draw(spriteBatch);
      smallStick.Draw(spriteBatch);

      buttonA.Draw(spriteBatch);
      buttonB.Draw(spriteBatch);
      spriteBatch.End();

      animatedPlayer.DrawAnimation(spriteBatch);

      base.Draw(gameTime);

      // -------------------------------------
      // -- draw the render target buffer to the screen with 90 degree rotation
      // -------------------------------------
      GraphicsDevice.SetRenderTarget(null); // set our target to screen
      spriteBatch.Begin();

      //1280 x 720, 385 280, 400 240

      DrawLandscape(385, 200);

      spriteBatch.End();

    }

    #region Private methods

    private void HandleTouch()
    {
      var touchCollection = TouchPanel.GetState();
      foreach (TouchLocation loc in touchCollection)
      {
        SetSmallStickLocation(loc);

        if (buttonA.IsPressed(loc, GameWindowHeight))
        {

        }

        if (buttonB.IsPressed(loc, GameWindowHeight))
        {

        }
      }
    }

    private void SetSmallStickLocation(TouchLocation touchLocation)
    {
      if ((touchLocation.Position.Y > 50 && touchLocation.Position.Y < 150))
      {
        smallStick.Position.X = touchLocation.Position.Y;
      }
    }

    private MoveDirection GetMovementControllerDirection()
    {
      if (smallStick.Position.X > movementControlPosition.X - 20 && smallStick.Position.X < movementControlPosition.X + 20)
      {
        return MoveDirection.Stop;
      }
      else if (smallStick.Position.X < movementControlPosition.X)
      {
        return MoveDirection.Left;
      }
      else if (smallStick.Position.X > movementControlPosition.X)
      {
        return MoveDirection.Right;
      }
      return MoveDirection.Stop;
    }

    private void DrawLandscape(float x, float y)
    {
      spriteBatch.Draw(renderTarget, new Vector2(x, y), null, Color.White, MathHelper.PiOver2, new Vector2(y, x), 1f, SpriteEffects.None, 0);
    }

    #endregion

  }
}