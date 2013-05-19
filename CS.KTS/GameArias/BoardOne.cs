using CS.KTS.Data;
using CS.KTS.Data.Objects;
using CS.KTS.Entities;
using CS.KTS.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    public delegate void SendCharacterScreenStats(PlayerData playerData);
    public SendCharacterScreenStats SendCharacterStats;

    public delegate void CastTimeHandler(double castTime, double currentTime);
    public CastTimeHandler SendCastTime;

    public delegate void AbilityHandler(List<Ability> ability);
    public AbilityHandler SendAbilityMessage;

    private string _pressedButton;
    private Player _player;
    private System.Collections.Generic.List<EnemyWalker> _walkers = new System.Collections.Generic.List<EnemyWalker>();
    private System.Random rand = new System.Random();
    private int _waveCount;
    private TimeSpan? _nextWaveTime;
    private Random rand2 = new Random();
    private bool _leftIsPressed;
    private bool _rightIsPressed;
    private TimeSpan? _lastMessageTime;
    private TimeSpan? _lastCooldownMessageTime;
    private TimeSpan? _lastGameTime;
    private List<Message> _guiMessages = new List<Message>();
    private bool _isPaused;
    private TimeSpan _pauseTime;
    private TimeSpan _currentGameTime;
    private List<LootSprite> _drops;

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

    protected override void Initialize()
    {
      TouchPanel.EnabledGestures = GestureType.FreeDrag | GestureType.Tap | GestureType.Hold;
      // TODO: Add your initialization logic here
      _pauseTime = new TimeSpan();
      _currentGameTime = new TimeSpan();
      _drops = new List<LootSprite>();

      base.Initialize();
    }

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

      _player = new Player("playerChar", "bullit", 1, 4, new Vector2(500, 470), BoardWidth, App.PlayerData);
      _player.LoadContent(Content);

      var guiMessages = new List<Message>();
      var playerMaxHp = new Message { Number = _player.Data.MaxHp, MessageType = MessageType.InitPlayerMaxHp };
      var playerLevel = new Message { Number = _player.Data.PlayerLevel, MessageType = MessageType.PlayerLevel };
      GuiMessageWriter(guiMessages);

    }

    protected override void UnloadContent()
    {
      base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
      CanExit = false;
      _currentGameTime = gameTime.TotalGameTime;

      if (IsDisposed) return;

      _controls.OnUpdate(TouchPanel.GetState(), _isPaused);
      if (_isPaused) return;

      UpdateBackground(gameTime);
      UpdateLoot(gameTime);
      CheckAndRemove();
      CheckEnemyHits();
      IsPlayerHit();
      CheckLoot();

      if (_player.Data.IsCasting)
      {
        SendCastTime(_player.Data.EquipedWeapon.CastTime, _player.Data.TimeToCast);
      }

      _player.Update(gameTime);
      _walkers.ForEach(w => w.Update(gameTime, _player.ScreenPosition, _player.CurrentMovement.Direction));

      SetNextWave(gameTime);
      SendGuiMessages(gameTime);

      SetAbilityCooldown();

      base.Update(gameTime);
    }

    private void SetAbilityCooldown()
    {
      foreach (var ability in _player.Data.Abilities)
      {
        switch (ability.AbilityType)
        {
          case AbilityType.Healing:
            if (ability.IsOnCooldown)
            {
              var cooldonwMessage = new Message { MessageType = MessageType.PlayerHealingCooldown, Number = ability.CurrentCooldown };
              _guiMessages.Add(cooldonwMessage);
              _controls.IsEnabled(InputControlSprite.ButtonType.C, false);
            }
            else _controls.IsEnabled(InputControlSprite.ButtonType.C, true);
            break;
          case AbilityType.Stun:
            if (ability.IsOnCooldown) _controls.IsEnabled(InputControlSprite.ButtonType.D, false);
            else _controls.IsEnabled(InputControlSprite.ButtonType.D, true);
            break;
          case AbilityType.AoeStun:
            break;
          case AbilityType.DamageBoost:
            break;
          case AbilityType.HealingBoost:
            break;
          case AbilityType.DefenceBoost:
            break;
          case AbilityType.Damage:
            break;
          case AbilityType.AoeDamage:
            break;
          case AbilityType.Beem:
            if (ability.IsOnCooldown)
            {
              var cooldonwMessage = new Message { MessageType = MessageType.PlayerBeemCooldown, Number = ability.CurrentCooldown };
              _guiMessages.Add(cooldonwMessage);
              _controls.IsEnabled(InputControlSprite.ButtonType.E, false);
            }
            else _controls.IsEnabled(InputControlSprite.ButtonType.E, true);
            break;
          default:
            break;
        }
      }
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
      _drops.ForEach(d => d.Draw(_spriteBatch));
      base.Draw(gameTime);

      GraphicsDevice.SetRenderTarget(null);
      _spriteBatch.Begin();

      //1280 x 720, 385 280, 400 240
      DrawLandscape(385, 200);

      _spriteBatch.End();
      CanExit = true;
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
          ButtonAPressed();
          break;
        case InputControlSprite.ButtonType.B:
          ButtonBPressed();
          break;
        case InputControlSprite.ButtonType.C:
          ButtonCPressed();
          break;
        case InputControlSprite.ButtonType.D:
          ButtonDPressed();
          break;
        case InputControlSprite.ButtonType.E:
          ButtonEPressed();
          break;
        case InputControlSprite.ButtonType.None:
          NoButtonPressed();
          break;
        case InputControlSprite.ButtonType.GameMenu:
          ButtonGameMenuPressed();
          break;
      }
    }

    private void ButtonAPressed()
    {
      _player.SendProjectile = true;
    }

    private void ButtonBPressed()
    {
      if (_player.CurrentMovement.Type == MovementType.Crouch)
      {
        _player.CurrentMovement.Type = MovementType.Walking;
      }
      else if (_player.CurrentMovement.Type == MovementType.Walking)
      {
        _player.CurrentMovement.Type = MovementType.Crouch;
      }
    }

    private void ButtonCPressed()
    {
      var abilityResponse = _player.Data.UseAbility(AbilityType.Healing, _currentGameTime);
      if (!abilityResponse.CouldUse) return;
      var healingMessage = new Message { Text = abilityResponse.Power.ToString(), Number = abilityResponse.Power, MessageType = MessageType.PlayerHealing, X = (int)_player.Position.X, Y = (int)_player.Position.Y };
      _guiMessages.Add(healingMessage);
    }

    private void ButtonDPressed()
    {
      _player.Data.UseAbility(AbilityType.AoeStun, _currentGameTime);
    }

    private void ButtonEPressed()
    {
      _player.Data.UseAbility(AbilityType.Beem, _currentGameTime);
    }

    private void ButtonGameMenuPressed()
    {
      _isPaused = true;
      _pauseTime = _currentGameTime - _nextWaveTime.Value;
      SendCharacterStats(_player.Data);
    }

    private void NoButtonPressed()
    {
      if (_player.CurrentMovement != null) _player.CurrentMovement.Direction = MoveDirection.Stop;
    }

    private void SetNextWave(GameTime gameTime)
    {
      if (!_nextWaveTime.HasValue)
      {
        _nextWaveTime = gameTime.TotalGameTime;
      }
      if ((gameTime.TotalGameTime - _nextWaveTime.Value).Seconds > 5)
      {
        _nextWaveTime = gameTime.TotalGameTime;
        AddWalkers(2);
      }
    }

    private void SendGuiMessages(GameTime gameTime)
    {
      if (_lastGameTime == null) _lastGameTime = gameTime.TotalGameTime;

      if (((gameTime.TotalGameTime - _lastGameTime.Value).Seconds >= 1))
      {
        if (_guiMessages != null)
        {
          _player.Data.UpdateCooldowns(gameTime.TotalGameTime);
          _player.Data.UpdateDpsStats(gameTime.TotalGameTime);

          var playerXpMessage = new Message { Text = _player.Data.Exp.Current.ToString(), Number = _player.Data.Exp.Current, Number2 = _player.Data.Exp.Max, MessageType = MessageType.PlayerExp };
          var playerLevelMessage = new Message { Text = _player.Data.PlayerLevel.ToString(), MessageType = MessageType.PlayerLevel };
          var playerHpMessage = new Message { Number = _player.Data.CurrentHP, Number2 = _player.Data.MaxHp, MessageType = MessageType.PlayerHp };
          var playerDpsMessage = new Message { Text = _player.Data.GetCurrentDps().ToString(), MessageType = MessageType.PlayerDps };
          _guiMessages.Add(playerXpMessage);
          _guiMessages.Add(playerLevelMessage);
          _guiMessages.Add(playerHpMessage);
          _guiMessages.Add(playerDpsMessage);

          if (_player.Data.GaindNewLevel())
          {
            _guiMessages.Add(new Message {Text = _player.Data.PlayerLevel.ToString(), MessageType = MessageType.BigMessage });
          }

          GuiMessageWriter(_guiMessages);

          _guiMessages = new List<Message>();
        }
      }
    }

    private void SendAbilityMessages(GameTime gameTime)
    {
      if (_lastCooldownMessageTime == null) _lastCooldownMessageTime = gameTime.TotalGameTime;

      if (((gameTime.TotalGameTime - _lastCooldownMessageTime.Value).Seconds >= 1))
      {
        SendAbilityMessage(_player.Data.Abilities);
      }
    }

    private void UpdateBackground(GameTime gameTime)
    {
      if (_player.ScreenPosition == ScreenPosition.Left && _leftIsPressed && _player.CurrentMovement.Type != MovementType.Crouch)
      {
        _foreground.Update(gameTime, 120, ScrollingBackgroundSprite.HorizontalScrollDirection.Right);
      }
      else if (_player.ScreenPosition == ScreenPosition.Right && _rightIsPressed && _player.CurrentMovement.Type != MovementType.Crouch)
      {
        _foreground.Update(gameTime, 120, ScrollingBackgroundSprite.HorizontalScrollDirection.Left);
      }
      else
      {
        _foreground.Update(gameTime, 0, ScrollingBackgroundSprite.HorizontalScrollDirection.Right);
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

    private void UpdateLoot(GameTime gameTime)
    {
      if (_player.ScreenPosition == ScreenPosition.Left && _leftIsPressed && _player.CurrentMovement.Type != MovementType.Crouch)
      {
        _drops.ForEach(d => d.Update(gameTime, new Movement() { Direction = MoveDirection.Left, Type = MovementType.Walking }));
      }
      if (_player.ScreenPosition == ScreenPosition.Right && _rightIsPressed && _player.CurrentMovement.Type != MovementType.Crouch)
      {
        _drops.ForEach(d => d.Update(gameTime, new Movement() { Direction = MoveDirection.Right, Type = MovementType.Walking }));
      }
      else
      {
        _drops.ForEach(d => d.Update(gameTime, new Movement() { Direction = MoveDirection.Stop, Type = MovementType.Walking }));
      }
    }

    private void IsPlayerHit()
    {
      foreach (var walker in _walkers)
      {
        if (_player.IsColliding(walker) && !walker.IsDead)
        {
          var damage = _player.Data.Hit(walker.Data.Damage);
          var playerDamageMessage = new Message { Text = damage.ToString(), MessageType = MessageType.EnemyDamageDone, X = (int)_player.Position.X, Y = (int)_player.Position.Y, Number = walker.Data.Damage };
          _guiMessages.Add(playerDamageMessage);
          walker.IsDead = true;
          if (_player.Data.IsDead)
          {
            FinishedWriter(new List<Message>());
          }
        }
      }
    }

    private void CheckLoot()
    {
      foreach (var loot in _drops)
      {
        if (_player.IsColliding(loot))
        {
          loot.DoRemove = true;
          var droppedLoot = loot.Data.GetLoot();
          _player.Data.AddLoot(droppedLoot);
          GuiMessageWriter(new List<Message> { new Message { Number = _player.Data.Gold, MessageType = MessageType.PlayerGold } });
        }
      }
    }

    private void CheckEnemyHits()
    {
      foreach (var projectile in _player.Projectiles)
      {
        if (projectile.IsUsed) continue;
        foreach (var walker in _walkers)
        {
          if (walker.IsColliding(projectile) && !walker.IsDead)
          {
            projectile.IsUsed = true;
            var playerDamage = _player.Data.GetWeaponDamage();

            if (projectile.Data.Effect == ProjectileEffect.Burn)
            {
              playerDamage.Damage *= 3;
            }

            walker.IsHit(playerDamage.Damage);

            if (projectile.Data.Effect == ProjectileEffect.Slow)
            {
              walker.Data.ChangeSpeed(walker.Data.Speed / 2);
            }

            _player.Data.AddDpsStats(playerDamage.Damage, _currentGameTime);

            if (walker.IsDead)
            {
              _player.Data.Killedenemies++;
              _player.Data.UpdateExp(walker.Data.XPValue);
              if (walker.Data.DoDrop())
              {
                AddLoot(walker);
              }
            }

            var playerDamageDoneMessage = new Message { Text = playerDamage.Damage.ToString(), X = (int)walker.Position.X, Y = (int)walker.Position.Y, Number = playerDamage.Damage, MessageType = MessageType.PlayerDamageDone, IsCritical = playerDamage.IsCritical };
            var targetHpMessage = new Message { Text = walker.Data.CurrentHp.ToString(), MessageType = MessageType.TargetHp };
            _guiMessages.Add(playerDamageDoneMessage);
            _guiMessages.Add(targetHpMessage);

            projectile.SetHit(walker);
          }
        }
      }
    }

    private void AddWalkers(int count)
    {
      _waveCount++;
      for (int i = 0; i < count; i++)
      {
        var walker = new EnemyWalker(_graphics, "walker", 1, 3, new EnemyData(_player.Data.PlayerLevel), GetEnemyStartPosition());
        walker.LoadContent(Content);
        _walkers.Add(walker);
      }
    }

    private Vector2 GetEnemyStartPosition()
    {
      if (rand.NextDouble() >= 0.5)
        return new Vector2(rand.Next(1, 100), 500);
      else
        return new Vector2(_graphics.GraphicsDevice.Viewport.Height - rand.Next(80, 150), 500);
    }

    private void CheckAndRemove()
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
        }
      }

      foreach (var loot in _drops.ToList())
      {
        if (loot.DoRemove)
        {
          _drops.Remove(loot);
        }
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

    private void AddLoot(EnemyWalker enemyWalker)
    {
      var loot = new LootSprite("loot", 1, 3);
      loot.Position = new Vector2(enemyWalker.Position.X, enemyWalker.Position.Y + 30);
      loot.LoadContent(Content);
      loot.Scale = 0.5f;
      loot.Data.DropRate = enemyWalker.Data.DropRate;
      loot.Data.Gold = enemyWalker.Data.GoldValue;
      loot.Data.Level = enemyWalker.Data.Level;
      _drops.Add(loot);
    }

    public void UnPauseGame()
    {
      _isPaused = false;
      _nextWaveTime = _currentGameTime - _pauseTime;
    }

    public void ChangeWeapon(string weaponName)
    {
      _player.Data.ChangeWeapon(weaponName);
      SendCharacterStats(_player.Data);
    }

    public void UpdatePlayerStats(List<PlayerLevelUpStat> playerLevelUpStats)
    {
      _player.Data.UpdateStats(playerLevelUpStats);
      SendCharacterStats(_player.Data);
    }

    public void PauseGame()
    {
      _isPaused = true;
    }

    public bool CanExit { get; set; }
  }
}
