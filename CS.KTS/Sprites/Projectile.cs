using CS.KTS.Data;
using CS.KTS.Data.Objects;
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
  public class Projectile : AnimatedSprite
  {
    private Weapon _weapon;
    private Vector2 StartPosition;
    private MoveDirection _direction;
    private TimeSpan _burnStart;
    public ProjectileData Data { get; set; }

    public Projectile(Texture2D texture, int rows, int columns, Weapon weapon)
      : base(texture, rows, columns)
    {
      Data = new ProjectileData { Effect = weapon.Effect };
      _weapon = weapon;
      Scale = 0.3f;
    }

    public override void Update(GameTime theGameTime, Movement movement)
    {
      if (Data.Effect == ProjectileEffect.Burn)
      {
        if (((theGameTime.TotalGameTime - _burnStart).TotalMilliseconds >= 500))
        {
          DoRemove = true;
        }
      }
      else
      {
        if (!DoRemove) DoRemove = (Vector2.Distance(StartPosition, Position) > _weapon.Distance);

        if (!DoRemove)
        {
          if (_direction == MoveDirection.Right)
            Position += mDirection * _weapon.Speed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
          else
            Position -= mDirection * _weapon.Speed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
        }
      }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      int width = SpriteTexture.Width / Columns;
      int height = SpriteTexture.Height / Rows;
      _sourceHeigth = height;
      _sourceWidth = width;
      int row = (int)((float)_currentFrameIndex / (float)Columns);
      int column = _currentFrameIndex % Columns;

      Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
      Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, height);

      spriteBatch.Draw(SpriteTexture, destinationRectangle, sourceRectangle, Color.White);

    }

    public void Fire(Vector2 theStartPosition, Vector2 theSpeed, Vector2 theDirection, MoveDirection playerDirection, TimeSpan gameTime)
    {
      _direction = playerDirection;
      if (playerDirection == MoveDirection.Right)
        Position = new Vector2(theStartPosition.X - 80, theStartPosition.Y);
      else
        Position = new Vector2(theStartPosition.X - 50, theStartPosition.Y);

      StartPosition = theStartPosition;
      mSpeed = theSpeed;
      mDirection = theDirection;

      if (Data.Effect == ProjectileEffect.Burn)
      {
        mSpeed.X = 0;
        if (_direction == MoveDirection.Left)
        {
          int width = SpriteTexture.Width / Columns;
          if (playerDirection == MoveDirection.Right)
            Position = new Vector2(theStartPosition.X - 80, theStartPosition.Y);
          else
            Position = new Vector2(theStartPosition.X - 50 - width, theStartPosition.Y);
        }
        _burnStart = gameTime;
      }
    }

    public bool IsUsed { get; set; }

    internal void SetHit(EnemyWalker hitWalker)
    {
      switch (Data.Effect)
      {
        case ProjectileEffect.AoeSlow:
          break;
        case ProjectileEffect.Heal:
          break;
        case ProjectileEffect.Burn:
          break;
        case ProjectileEffect.Explode:
          break;
        case ProjectileEffect.Slow:
        case ProjectileEffect.None:
          var projectileHitPos = hitWalker.Position.X + (hitWalker.Size.Width / 2) - 45;
          Position = new Vector2(projectileHitPos, Position.Y);
          DoRemove = true;
          IsUsed = true;
          _currentFrameIndex = 1;
          break;
        default:
          break;
      }
    }
  }
}
