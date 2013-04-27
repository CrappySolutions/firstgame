using CS.KTS.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Linq;
namespace CS.KTS
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class BoardOne : Game
  {
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;
    private Sprites.InputControlSprite _controls;
    private RenderTarget2D _renderTarget;
    private bool firstUpdate = true;
    private ScrollingBackgroundSprite _background;
    public delegate void WriteTextHandler(string value);
    public WriteTextHandler ConsoleWrite;
    public WriteTextHandler HPWriter;
    public WriteTextHandler FinishedWriter;
    private string _pressedButton;
    private Player _player;
    private System.Collections.Generic.List<EnemyWalker> _walkers = new System.Collections.Generic.List<EnemyWalker>();
    private System.Random rand = new System.Random();
    private int BoardHeight
    {
      get { return _graphics.GraphicsDevice.Viewport.Width; }
    }

    private int BoardWidth
    {
      get { return _graphics.GraphicsDevice.Viewport.Height; }
    }

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
      TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Tap | GestureType.Hold;
      // TODO: Add your initialization logic here
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
      _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
      _controls = new Sprites.InputControlSprite(Content, _graphics);
      _controls.Toucht += OnToucht;
      _renderTarget = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.GraphicsDevice.Viewport.Height, _graphics.GraphicsDevice.Viewport.Width, false, SurfaceFormat.Color, DepthFormat.Depth16);
      // TODO: use this.Content to load your game content here

      _background = new ScrollingBackgroundSprite(this.GraphicsDevice.Viewport);
      _background.AddBackground("level1Test");
      _background.AddBackground("level1Test");
      _background.LoadContent(this.Content);

      _player = new Player("player", "bullit", 1, 2, new Vector2(500, 500));
      _player.LoadContent(Content);
      HPWriter(_player.HP.ToString());
      AddWalkers(2);
    }

    private void OnToucht(object sender, InputControlSprite.ButtonEventArgs e)
    {
      _pressedButton = e.Button.ToString();
      ConsoleWrite(_pressedButton);
      switch (e.Button)
      {
        case InputControlSprite.ButtonType.Left:
          if(Vector2.Distance(new Vector2(_player.Position.X, 0), new Vector2(0, 0)) > 20)
            _player.CurrentMovement = new Movement { Direction = MoveDirection.Left, Type = MovementType.Walking };
          else
            _player.CurrentMovement = new Movement { Direction = MoveDirection.Stop, Type = MovementType.Walking };
          break;
        case InputControlSprite.ButtonType.Right:
          if (Vector2.Distance(new Vector2(_player.Position.X, 0), new Vector2(BoardWidth, 0)) > 80)
           _player.CurrentMovement = new Movement { Direction = MoveDirection.Right, Type = MovementType.Walking };
          else
            _player.CurrentMovement = new Movement { Direction = MoveDirection.Stop, Type = MovementType.Walking };
          break;
        case InputControlSprite.ButtonType.A:
          _player.SendProjectile = true;
          break;
        case InputControlSprite.ButtonType.B:
          break;
        case InputControlSprite.ButtonType.C:
          break;
        case InputControlSprite.ButtonType.None:
          _player.CurrentMovement = new Movement { Direction = MoveDirection.Stop, Type = MovementType.Walking };
          break;
      }
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
      //// TODO: Unload any non ContentManager content here
      //Dispose();
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      // TODO: Add your update logic here
      _controls.OnUpdate(TouchPanel.GetState());
      _background.Update(gameTime, 10, ScrollingBackgroundSprite.HorizontalScrollDirection.Left);
      CheckAndRemove();
      CheckEnemyHits();
      _player.Update(gameTime);
      _walkers.ForEach(w => w.Update(gameTime));
      base.Update(gameTime);
    }

    private void CheckEnemyHits()
    {
      foreach (var projectile in _player.Projectiles)
      {
        foreach (var walker in _walkers)
        {
          if (walker.IsColliding(projectile) && !walker.IsDead)
          {
            
            walker.SetDead();
            var xPos = walker.Position.X + (walker.Size.Width / 2) - 45;
            projectile.SetHit();
            projectile.Position = new Vector2(xPos, projectile.Position.Y);
          }
        }
      }

      foreach (var walker in _walkers)
      {
        if (_player.IsColliding(walker) && !walker.IsDead)
        {
          _player.HP -= 10;
          HPWriter(_player.HP.ToString());
        }
      }
    }

    public void AddWalkers(int count)
    {
      var maxWidth = _graphics.GraphicsDevice.Viewport.Height - 80;
      for (int i = 0; i < count; i++)
      {
        var startX = rand.Next(100, maxWidth);
        var  walker = new EnemyWalker(_graphics, "walker", 1, 3, new Vector2(startX, 500));
        walker.LoadContent(Content);
        _walkers.Add(walker);
      }
    }

    public void CheckAndRemove()
    { 
      var ps = _player.Projectiles.ToList();
      foreach (var p in ps)
      {
        if (p.DoRemove)
          _player.Projectiles.Remove(p);
      }

      var ws = _walkers.ToList();
      bool removed = false;
      foreach (var walker in ws)
      {
        if (walker.DoRemove)
        {
          removed = true;
          _walkers.Remove(walker);
        }
      }
      if (removed && _walkers.Count == 0)
      {
        AddWalkers(2);
       // FinishedWriter("Completed");
      }
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.SetRenderTarget(_renderTarget);
      _graphics.GraphicsDevice.Clear(Color.Black);

      _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
      _background.Draw(_spriteBatch);
      _controls.Draw(_spriteBatch);
      _spriteBatch.End();

      _player.Draw(_spriteBatch);
      _walkers.ForEach(w => w.Draw(_spriteBatch));
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
