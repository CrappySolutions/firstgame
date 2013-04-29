using CS.KTS.Entities;
using CS.KTS.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Linq;
namespace CS.KTS
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class BoardOne : Game
  {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Sprites.InputControlSprite _controls;
    private RenderTarget2D _renderTarget;
    private bool firstUpdate = true;
    private ScrollingBackgroundSprite _background;
    private ScrollingBackgroundSprite _foreground;
    public delegate void WriteTextHandler(Message message);
    public WriteTextHandler HPWriter;
    public WriteTextHandler FinishedWriter;
    private string _pressedButton;
    private Player _player;
    private System.Collections.Generic.List<EnemyWalker> _walkers = new System.Collections.Generic.List<EnemyWalker>();
    private System.Random rand = new System.Random();
    private int _waveCount;
    private TimeSpan? _nextWave;
    private Random rand2 = new Random();
    private bool _leftIsPressed;
    private bool _rightIsPressed;

    private int BoardHeight
    {
      get { return _graphics.GraphicsDevice.Viewport.Width; }
    }

    private int BoardWidth
    {
      get { return _graphics.GraphicsDevice.Viewport.Height; }
    }

    public bool IsDisposed { get; private set; }

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
      TempHackToFixGestures();

      _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
      _controls = new Sprites.InputControlSprite(Content, _graphics);
      _controls.Toucht += OnToucht;
      _renderTarget = new RenderTarget2D(_graphics.GraphicsDevice, _graphics.GraphicsDevice.Viewport.Height, _graphics.GraphicsDevice.Viewport.Width, false, SurfaceFormat.Color, DepthFormat.Depth16);

      _background = new ScrollingBackgroundSprite(this.GraphicsDevice.Viewport);
      _background.AddBackground("level1Test");
      _background.AddBackground("level1Test");
      _background.LoadContent(this.Content);

      _foreground = new ScrollingBackgroundSprite(this.GraphicsDevice.Viewport);
      _foreground.AddBackground("Level1Bg/level1Image1");
      _foreground.AddBackground("Level1Bg/level1Image2");
      _foreground.AddBackground("Level1Bg/level1Image3");
      _foreground.LoadContent(Content);

      _player = new Player("playerChar", "bullit", 1, 4, new Vector2(500, 470), BoardWidth);
      _player.LoadContent(Content);
    }

    private void OnToucht(object sender, InputControlSprite.ButtonEventArgs e)
    {
      _pressedButton = e.Button.ToString();
      _player.SetPlayerDirection(e);

      _leftIsPressed = e.Button == InputControlSprite.ButtonType.Left;
      _rightIsPressed = e.Button == InputControlSprite.ButtonType.Right;
      
      switch (e.Button)
      {
        case InputControlSprite.ButtonType.A:
          _player.SendProjectile = true;
          break;
        case InputControlSprite.ButtonType.B:
          if (_player.CurrentMovement.Type == MovementType.Crouch)
          {
            _player.CurrentMovement.Type = MovementType.Walking;
          }
          else if (_player.CurrentMovement.Type == MovementType.Walking)
          {
            _player.CurrentMovement.Type = MovementType.Crouch;
          }
          break;
        case InputControlSprite.ButtonType.C:
          break;
        case InputControlSprite.ButtonType.None:
          if (_player.CurrentMovement != null)
            _player.CurrentMovement.Direction = MoveDirection.Stop;
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
      base.UnloadContent();
    }

    public void ClearBoard()
    {
      IsDisposed = true;
      foreach (var walker in _walkers)
      {
        walker.Dispose();
      }
      _player.Dispose();
      _background.Dispose();

      UnloadContent();
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      if (IsDisposed) return;
      
      if (_player.ScreenPosition == ScreenPosition.Left && _leftIsPressed)
      {
        _foreground.Update(gameTime, 120, ScrollingBackgroundSprite.HorizontalScrollDirection.None);
      }
      else if (_player.ScreenPosition == ScreenPosition.Right && _rightIsPressed)
      {
        _foreground.Update(gameTime, 120, ScrollingBackgroundSprite.HorizontalScrollDirection.Left);
      }
      else
      {
        _foreground.Update(gameTime, 0, ScrollingBackgroundSprite.HorizontalScrollDirection.None);
      }

      if (_rightIsPressed && _player.CurrentMovement.Type != MovementType.Crouch)
      {
        _background.Update(gameTime, 20, ScrollingBackgroundSprite.HorizontalScrollDirection.Left);
      }
      if (_leftIsPressed && _player.CurrentMovement.Type != MovementType.Crouch)
      {
        _background.Update(gameTime, 20, ScrollingBackgroundSprite.HorizontalScrollDirection.None);
      }

      // TODO: Add your update logic here
      _controls.OnUpdate(TouchPanel.GetState());
      CheckAndRemove();
      CheckEnemyHits();
      _player.Update(gameTime);
      _walkers.ForEach(w => w.Update(gameTime));

      if (!_nextWave.HasValue)
      {
        _nextWave = gameTime.TotalGameTime;
      }
      if ((gameTime.TotalGameTime - _nextWave.Value).Seconds > 5)
      {
        _nextWave = gameTime.TotalGameTime;
        if (_waveCount < 11) AddWalkers(2);
      }

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
            var damage = _player.GetDamage();
            walker.IsHit((int)damage);
            HPWriter(new Message { Text = damage.ToString(), X = (int)walker.Position.X, Y = (int)walker.Position.Y, MessageType = MessageType.PlayerDamageDone });
            HPWriter(new Message { Text = walker.Hp.ToString(), MessageType = MessageType.TargetHp });
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
          //HPWriter(new Message { Text = "30", X = (int)walker.Position.X, Y = (int)walker.Position.Y });
        }
      }
    }

    public void AddWalkers(int count)
    {
      _waveCount++;
      var maxWidth = _graphics.GraphicsDevice.Viewport.Height - 80;
      for (int i = 0; i < count; i++)
      {
        var startX = rand.Next(100, maxWidth);
        var walker = new EnemyWalker(_graphics, "nisse2", 1, 4, new Vector2(startX, 500));
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
      if (_waveCount == 10 && _walkers.Count == 0)
      {
        FinishedWriter(new Message());
      }
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      if (IsDisposed) return;

      GraphicsDevice.SetRenderTarget(_renderTarget);
      _graphics.GraphicsDevice.Clear(Color.Black);

      _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
      _background.Draw(_spriteBatch);
      _foreground.Draw(_spriteBatch);
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


    private void TempHackToFixGestures()
    {
      if (firstUpdate)
      {
        // Temp hack to fix gestures
        typeof(Microsoft.Xna.Framework.Input.Touch.TouchPanel)
            .GetField("_touchScale", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            .SetValue(null, Vector2.One);

        firstUpdate = false;
      }
    }
  }
}
