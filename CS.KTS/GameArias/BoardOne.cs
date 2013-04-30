using CS.KTS.Entities;
using CS.KTS.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
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
    public delegate void WriteTextHandler(List<Message> messages);
    public WriteTextHandler GuiMessageWriter;
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
      _foreground.AddBackground("Level1Bg/level1Image3");
      _foreground.AddBackground("Level1Bg/level1Image2");
      _foreground.LoadContent(Content);

      var playerData = CreateNewPlayer();

      _player = new Player("playerChar", "bullit", 1, 4, new Vector2(500, 470), BoardWidth, playerData);
      _player.LoadContent(Content);

      var guiMessages = new List<Message>();
      var playerMaxHp = new Message { Number = _player.Data.MaxHp, MessageType = MessageType.InitPlayerMaxHp };
      var playerLevel = new Message { Number = _player.Data.PlayerLevel, MessageType = MessageType.PlayerLevel };
      GuiMessageWriter(guiMessages);
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

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      if (IsDisposed) return;
      UpdateBackground(gameTime);
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

    private void UpdateBackground(GameTime gameTime)
    {
      if (_player.ScreenPosition == ScreenPosition.Left && _leftIsPressed && _player.CurrentMovement.Type != MovementType.Crouch)
      {
        _foreground.Update(gameTime, 120, ScrollingBackgroundSprite.HorizontalScrollDirection.None);
      }
      else if (_player.ScreenPosition == ScreenPosition.Right && _rightIsPressed && _player.CurrentMovement.Type != MovementType.Crouch)
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
    }

    private void CheckPlayerHits()
    {
      foreach (var walker in _walkers)
      {
        if (_player.IsColliding(walker) && !walker.IsDead)
        {
          _player.Hit(walker.Data.Damage);
          if (_player.IsDead)
          {
            FinishedWriter(new List<Message>());
          }
        }
      }
    }

    private void CheckEnemyHits()
    {
      foreach (var projectile in _player.Projectiles)
      {
        foreach (var walker in _walkers)
        {
          if (walker.IsColliding(projectile) && !walker.IsDead)
          {
            var damage = _player.GetWeaponDamage();
            walker.IsHit((int)damage);

            var guiMessages = new List<Message>();
            var playerDamageDoneMessage = new Message { Text = damage.ToString(), X = (int)walker.Position.X, Y = (int)walker.Position.Y, MessageType = MessageType.PlayerDamageDone };
            var targetHpMessage = new Message { Text = walker.Data.CurrentHp.ToString(), MessageType = MessageType.TargetHp };
            var playerXpMessage = new Message { Text = _player.Data.CurrentXP.ToString(), MessageType = MessageType.PlayerExp };
            var playerXpPercentMessage = new Message { Number = _player.Data.XpPercent, MessageType = MessageType.PlayerXpPercent };
            var playerHpPercentMessage = new Message { Number = _player.Data.HpPercent, MessageType = MessageType.PlayerHpPercent };

            guiMessages.Add(playerDamageDoneMessage);
            guiMessages.Add(targetHpMessage);
            guiMessages.Add(playerXpMessage);
            guiMessages.Add(playerXpPercentMessage);
            guiMessages.Add(playerHpPercentMessage);

            GuiMessageWriter(guiMessages);

            var projectileHitPos = walker.Position.X + (walker.Size.Width / 2) - 45;
            projectile.SetHit();
            projectile.Position = new Vector2(projectileHitPos, projectile.Position.Y);
          }
        }
      }
    }

    public void AddWalkers(int count)
    {
      _waveCount++;
      for (int i = 0; i < count; i++)
      {
        var walker = new EnemyWalker(_graphics, "nisse2", 1, 4, CreateNewEnemy(), GetEnemyStartPosition());
        walker.LoadContent(Content);
        _walkers.Add(walker);
      }
    }

    private Vector2 GetEnemyStartPosition()
    {
      if (rand.NextDouble() >= 0.5)
        return new Vector2(rand.Next(1,100), 500);
      else
        return new Vector2(_graphics.GraphicsDevice.Viewport.Height - rand.Next(80, 150), 500);
    }

    public void CheckAndRemove()
    {
      var ps = _player.Projectiles.ToList();
      foreach (var p in ps)
      {
        if (p.DoRemove)
          _player.Projectiles.Remove(p);
      }

      foreach (var walker in _walkers.ToList())
      {
        if (walker.DoRemove)
        {
          _walkers.Remove(walker);
          _player.UpdateXp(walker.Data.XPValue);
        }
      }
      if (_waveCount >= 10 && _walkers.Count == 0)
      {
        FinishedWriter(new List<Message>());
      }
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

    private Data.PlayerData CreateNewPlayer()
    {
      return new Data.PlayerData
      {
        CurrentLevel = new Data.Level(),
        CurrentHP = 200,
        MaxHp = 200,
        PlayerLevel = 1,
        Id = 1,
        IsGood = true,
        MainShieldId = 1,
        MainWeaponId = 1,
        MeleStrenght = 1,
        Name = "Bob",
        Purse = 0,
        RangeStrenght = 1,
        SecondaryWeaponId = 1,
        Shields = new System.Collections.Generic.List<Data.Shield>(),
        TilesRef = "playerChar",
        Toughness = 1,
        Weapons = new System.Collections.Generic.List<Data.Weapon>(),
        CurrentXP = 0
      };
    }

    private Data.EnemyData CreateNewEnemy()
    {
      return new Data.EnemyData
      {
        CurrentHp = 100,
        Damage = 10,
        GoldValue = 1,
        Id = 1,
        IsGood = false,
        MainWeapon = new Data.Weapon(),
        MaxCityLevel = 1,
        MinCityLevel = 1,
        Name = "Nisse",
        MaxHp = 100,
        TilesRef = "nisse2",
        XPValue = 10
      };
    }
  }
}
