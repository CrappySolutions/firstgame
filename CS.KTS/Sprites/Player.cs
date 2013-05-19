using CS.KTS.Data;
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
    private int _screenWidth;
    private const int _distanceFromRightEdge = 300;
    private const int _distanceFromLeftEdge = 300;
    private bool _isCasting;
    private TimeSpan? _castStart;
    private TimeSpan? _lastProjectileTime;

    public Data.PlayerData Data { get; private set; }

    public Player(string skinAsset, string weaponSkinAsset, int rows, int columns, Vector2 startPoint, int screenWidth, PlayerData playerData)
      : base(skinAsset, rows, columns)
    {
      Data = playerData;
      _screenWidth = screenWidth;
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

      if (SendProjectile && Data.EquipedWeapon.CastTime > 0)
      {
        SendProjectileWithCastTime(gameTime);
      }
      else if (SendProjectile && CanFire(gameTime) && Data.EquipedWeapon.CastTime == 0)
      {
        SendProjectileWithNoCastTime(gameTime);
      }


      foreach (var ability in Data.Abilities)
      {
        if (!ability.Send) continue;
        if (!ability.IsProjectile) continue;
        var projectile = new Projectile(_contentManager.Load<Texture2D>(ability.TextureName), 1, 2, Data.EquipedWeapon);
        projectile.Data.Effect = ProjectileEffect.Burn;
        projectile.Fire(_firePosition, new Vector2(500, 0), new Vector2(1, 0), GetProjectileDirection(), gameTime.TotalGameTime);
        Projectiles.Add(projectile);
        ability.Send = false;
      }

      foreach (var projectile in Projectiles)
      {
        projectile.Update(gameTime, CurrentMovement);
      }
    }

    private void SendProjectileWithNoCastTime(GameTime gameTime)
    {
      _isCasting = true;
      SendProjectile = false;
      var projectile = new Projectile(_contentManager.Load<Texture2D>(_projectileAssetName), 1, 2, Data.EquipedWeapon);
      projectile.Fire(_firePosition, new Vector2(500, 0), new Vector2(1, 0), GetProjectileDirection(), gameTime.TotalGameTime);
      Projectiles.Add(projectile);
      _lastProjectileTime = gameTime.TotalGameTime;
    }

    private void SendProjectileWithCastTime(GameTime gameTime)
    {
      if (!Data.IsCasting)
      {
        Data.IsCasting = true;
        _castStart = gameTime.TotalGameTime;
      }

      Data.TimeToCast = (gameTime.TotalGameTime - _castStart.Value).TotalMilliseconds;

      if ((gameTime.TotalGameTime - _castStart.Value).TotalMilliseconds > Data.EquipedWeapon.CastTime)
      {
        SendProjectile = false;
        var projectile = new Projectile(_contentManager.Load<Texture2D>(_projectileAssetName), 1, 2, Data.EquipedWeapon);
        projectile.Fire(_firePosition, new Vector2(500, 0), new Vector2(1, 0), GetProjectileDirection(), gameTime.TotalGameTime);
        Projectiles.Add(projectile);
        _lastProjectileTime = gameTime.TotalGameTime;
        Data.IsCasting = false;
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

    protected override void UpdateMovement(Movement movement)
    {
      if (mCurrentState == CharacterState.Walking)
      {
        mSpeed = Vector2.Zero;
        mDirection = Vector2.Zero;
        mSpeed.X = Constants.MovementSpeed[movement.Type];

        if (ScreenPosition == ScreenPosition.Left && movement.Direction == MoveDirection.Left)
        {
          mSpeed.X = 0;
        }

        if (ScreenPosition == ScreenPosition.Right && movement.Direction == MoveDirection.Right)
        {
          mSpeed.X = 0;
        }

        if (movement.Direction == MoveDirection.Up || movement.Direction == MoveDirection.Down)
          mDirection.Y = Constants.DirectionOffsets[movement.Direction];
        else
          mDirection.X = Constants.DirectionOffsets[movement.Direction];
      }
    }

    public ScreenPosition ScreenPosition
    {
      get
      {
        if (Position.X >= _screenWidth - _distanceFromRightEdge)
        {
          return Entities.ScreenPosition.Right;
        }
        else if (Position.X <= _distanceFromRightEdge && Position.X <= _screenWidth - _distanceFromRightEdge)
        {
          return Entities.ScreenPosition.Left;
        }
        else
        {
          return Entities.ScreenPosition.Middle;
        }
      }
    }

    public Movement CurrentMovement { get; set; }

    public void SetPlayerDirection(InputControlSprite.ButtonEventArgs e)
    {
      switch (e.Button)
      {
        case InputControlSprite.ButtonType.Left:
          CurrentMovement.Direction = MoveDirection.Left;
          break;
        case InputControlSprite.ButtonType.Right:
          CurrentMovement.Direction = MoveDirection.Right;
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
      if ((gameTime.TotalGameTime - _lastProjectileTime.Value).Milliseconds > Data.EquipedWeapon.FireRate) return true;

      return false;

    }

  }
}
