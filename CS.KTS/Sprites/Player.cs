using CS.KTS.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Sprites
{
  public class Player : AnimatedSprite
  {
    public List<Projectile> Projectiles = new List<Projectile>();
    private string _projectileAssetName;
    private Microsoft.Xna.Framework.Content.ContentManager _contentManager;
    public bool SendProjectile { get; set; }
    private MoveDirection _lastDirection;
    private Random rand;
    private int _screenWidth;
    private const int _distanceFromRightEdge = 500;
    private const int _distanceFromLeftEdge = 50;
    private bool _isDead;

    private TimeSpan? _lastProjectileTime;

    public Data.PlayerData Data { get; private set; }

    public Player(string skinAsset, string weaponSkinAsset, int rows, int columns, Vector2 startPoint, int screenWidth, Data.PlayerData data)
      : base(skinAsset, rows, columns)
    {
      Data = data;
      _screenWidth = screenWidth;
      rand = new Random();
      CurrentMovement = new Movement { Direction = MoveDirection.Stop, Type = MovementType.Walking };
      _lastDirection = CurrentMovement.Direction;
      Position = startPoint;
      _projectileAssetName = weaponSkinAsset;
    }

    public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager theContentManager)
    {
      _contentManager = theContentManager;
      base.LoadContent(theContentManager);
      _currentFrameIndex = 0;
      _totalFrameCount = Rows * Columns;
      foreach (var projectile in Projectiles)
      {
        projectile.LoadContent(theContentManager, _projectileAssetName);
      }
    }

    public void Update(GameTime gameTime)
    {
      if (CurrentMovement.Direction == MoveDirection.Left)
      {
        _lastDirection = CurrentMovement.Direction;
        if (CurrentMovement.Type == MovementType.Crouch)
        {
          _currentFrameIndex = 3;
        }
        else
        {
          _currentFrameIndex = 1;
        }
      }
      else if (CurrentMovement.Direction == MoveDirection.Right)
      {
        _lastDirection = CurrentMovement.Direction;
        if (CurrentMovement.Type == MovementType.Crouch)
        {
          _currentFrameIndex = 2;
        }
        else
        {
          _currentFrameIndex = 0;
        }
      }
      else if (CurrentMovement.Direction == MoveDirection.Stop)
      {
        if (CurrentMovement.Type == MovementType.Crouch)
        {
          if (_lastDirection == MoveDirection.Right) _currentFrameIndex = 2;
          if (_lastDirection == MoveDirection.Left) _currentFrameIndex = 3;
        }
        else if (CurrentMovement.Type == MovementType.Walking)
        {
          if (_lastDirection == MoveDirection.Right) _currentFrameIndex = 0;
          if (_lastDirection == MoveDirection.Left) _currentFrameIndex = 1;
        }
      }

      UpdateMovement(CurrentMovement);
      base.Update(gameTime, mSpeed, mDirection);

      if (SendProjectile && CanFire(gameTime))
      {
        SendProjectile = false;
        var projectile = new Projectile(_contentManager.Load<Texture2D>(_projectileAssetName), 1, 2);
        projectile.Fire(_firePosition, new Vector2(500, 0), new Vector2(1, 0), GetProjectileDirection());
        Projectiles.Add(projectile);
        _lastProjectileTime = gameTime.TotalGameTime;
      }

      foreach (var projectile in Projectiles)
      {
        projectile.Update(gameTime, CurrentMovement);
      }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
      spriteBatch.Begin();
      foreach (var proj in Projectiles)
      {
        proj.Draw(spriteBatch);
      }
      spriteBatch.End();
    }

    public void Hit(int damage)
    {
      Data.CurrentHP = -damage;
      if (Data.CurrentHP <= 0) _isDead = true;
    }

    public bool IsDead { get { return _isDead; } }

    public void UpdateXp(int exp)
    {
      Data.CurrentXP += exp;
    }

    public ScreenPosition ScreenPosition
    {
      get
      {
        if (Position.X >= _screenWidth - _distanceFromRightEdge)
        {
          return Entities.ScreenPosition.Right;
        }
        else if (Position.X <= _distanceFromRightEdge)
        {
          return Entities.ScreenPosition.Left;
        }
        else
        {
          return Entities.ScreenPosition.Middle;
        }
      }
    }

    public int GetWeaponDamage()
    {
      var fact = rand.Next(50, 200);
      var damage = (_baseDamage * fact / 100);
      return damage;
    }

    public Movement CurrentMovement { get; set; }

    public void SetPlayerDirection(InputControlSprite.ButtonEventArgs e)
    {
      switch (e.Button)
      {
        case InputControlSprite.ButtonType.Left:
          if (Vector2.Distance(new Vector2(Position.X, 0), new Vector2(0, 0)) > _distanceFromLeftEdge)
            CurrentMovement.Direction = MoveDirection.Left;
          else
            CurrentMovement.Direction = MoveDirection.Stop;
          break;
        case InputControlSprite.ButtonType.Right:
          if (Vector2.Distance(new Vector2(Position.X, 0), new Vector2(_screenWidth, 0)) > _distanceFromRightEdge)
            CurrentMovement.Direction = MoveDirection.Right;
          else
            CurrentMovement.Direction = MoveDirection.Stop;
          break;
        case InputControlSprite.ButtonType.None:
          if (CurrentMovement != null)
            CurrentMovement.Direction = MoveDirection.Stop;
          break;
      }
    }

    private MoveDirection GetProjectileDirection()
    {
      if (_currentFrameIndex == 0 || _currentFrameIndex == 2) return MoveDirection.Right;
      return MoveDirection.Left;
    }

    private Vector2 _firePosition
    {
      get
      {
        if (CurrentMovement.Type == MovementType.Crouch)
        {
          return new Vector2(Position.X + 90, Position.Y + 55);
        }

        return new Vector2(Position.X + 90, Position.Y + 40);

      }
    }

    private int _baseDamage
    {
      get
      {
        if (CurrentMovement.Type == MovementType.Crouch)
          return 15;
        else
          return 8;
      }
    }

    private bool CanFire(GameTime gameTime)
    {
      if (!_lastProjectileTime.HasValue) return true;
      if ((gameTime.TotalGameTime - _lastProjectileTime.Value).Milliseconds > 200) return true;

      return false;

    }
  }
}
