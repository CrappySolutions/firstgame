using CS.KTS.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Sprites
{
  public class EnemyWalker : AnimatedSprite
  {
    private ContentManager _contentManager;
    private readonly GraphicsDeviceManager _graphicsManager;
    private float _minX;
    private float _maxX;
    private MoveDirection _currentDirection;
    private TimeSpan? _deadTime;
    private HpBarSprite _hpBar;
    private bool _isDebuffed;

    public Data.EnemyData Data { get; private set; }

    public EnemyWalker(GraphicsDeviceManager aGraphicsManager, string skinAsset, int rows, int columns, Data.EnemyData enemyData, Vector2? startLocation = null)
      : base(skinAsset, rows, columns)
    {
      Data = enemyData;
      _hpBar = new HpBarSprite(aGraphicsManager, "hpBar", 1, 3, startLocation);
      _graphicsManager = aGraphicsManager;
      _minX = 0;
      _maxX = _graphicsManager.GraphicsDevice.Viewport.Height - 80;
      Position = startLocation.HasValue ? startLocation.Value : new Vector2(_maxX, 500);
      _currentDirection = MoveDirection.Left;
    }

    public override void LoadContent(ContentManager theContentManager)
    {
      _hpBar.LoadContent(theContentManager);
      _contentManager = theContentManager;
      _currentFrameIndex = 1;
      _totalFrameCount = Rows * Columns;
      base.LoadContent(theContentManager);
    }

    public void Update(GameTime gameTime, ScreenPosition playerScreenPosition, MoveDirection playerMoveDirection)
    {

      if (mCurrentState == CharacterState.Walking)
      {
        var movement = new Movement { Direction = _currentDirection, Type = MovementType.Walking };
        if (_currentDirection == MoveDirection.Left)
        {
          if (Position.X - _minX <= 20)
          {
            _currentFrameIndex = 0;
            _currentDirection = MoveDirection.Right;
          }
        }
        else if (_currentDirection == MoveDirection.Right)
        {

          if (_maxX - Position.X <= 20)
          {
            _currentFrameIndex = 1;
            _currentDirection = MoveDirection.Left;
          }
        }
        UpdateMovement(movement, playerScreenPosition, playerMoveDirection);
        _hpBar.UpdateFrameIndex(Position, Data.MaxHp, Data.CurrentHp);
        _hpBar.Update(gameTime, mSpeed, mDirection);
        base.Update(gameTime, mSpeed, mDirection);
      }
      if (IsDead)
      {
        if (_deadTime.HasValue)
        {
          if ((gameTime.TotalGameTime - _deadTime.Value).Milliseconds > 250)
          {
            DoRemove = true;
          }
        }
        else
        {
          _deadTime = gameTime.TotalGameTime;
        }
      }
    }

    private void UpdateMovement(Movement movement, ScreenPosition playerScreenPosition, MoveDirection playerMoveDirection)
    {
      if (mCurrentState == CharacterState.Walking)
      {
        mSpeed = Vector2.Zero;
        mDirection = Vector2.Zero;
        mSpeed.X = Data.Speed;

        if (movement.Direction == MoveDirection.Left && playerScreenPosition == ScreenPosition.Right)
        {
          mSpeed.X = Data.Speed + (Data.Speed / 2);
        }

        if (movement.Direction == MoveDirection.Left && playerScreenPosition == ScreenPosition.Left)
        {
          mSpeed.X = Data.Speed / 2;
        }

        if (movement.Direction == MoveDirection.Right && playerScreenPosition == ScreenPosition.Left)
        {
          mSpeed.X = Data.Speed + (Data.Speed / 2);
        }

        if (movement.Direction == MoveDirection.Right && playerScreenPosition == ScreenPosition.Right)
        {
          if (playerMoveDirection == MoveDirection.Right) mSpeed.X = Data.Speed / 2;
        }

        if (movement.Direction == MoveDirection.Up || movement.Direction == MoveDirection.Down)
          mDirection.Y = AnimatedSprite.Constants.DirectionOffsets[movement.Direction];
        else
          mDirection.X = AnimatedSprite.Constants.DirectionOffsets[movement.Direction];
      }
    }

    public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
    {
      _hpBar.Draw(spriteBatch);
      base.Draw(spriteBatch);
    }

    public bool IsDead { get; set; }

    internal void SetDead()
    {
      IsDead = true;
      if (_currentDirection == MoveDirection.Right) _currentFrameIndex = 2;
      else if (_currentDirection == MoveDirection.Left) _currentFrameIndex = 3;
      mCurrentState = CharacterState.Dead;
    }

    public void IsHit(int damage, ProjectileEffect projectileEffect)
    {
      ApplyProjectileEffect(projectileEffect);
      Data.CurrentHp -= damage;
      if (Data.CurrentHp <= 0) SetDead();
    }

    private void ApplyProjectileEffect(ProjectileEffect projectileEffect)
    {
      switch (projectileEffect)
      {
        case ProjectileEffect.Slow:
          Data.Speed /= 2;
          _isDebuffed = true;
          break;
        case ProjectileEffect.AoeSlow:
          break;
        case ProjectileEffect.Heal:
          break;
        case ProjectileEffect.Burn:
          break;
        case ProjectileEffect.Explode:
          break;
        case ProjectileEffect.None:
          break;
        default:
          break;
      }
    }
  }
}
