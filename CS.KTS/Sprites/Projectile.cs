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
    private int MAX_DISTANCE = 700;

    private Vector2 StartPosition;
    public Projectile(Texture2D texture, int rows, int columns)
      : base(texture, rows, columns)
    {
      Scale = 0.3f;
    }

    public override void Update(Microsoft.Xna.Framework.GameTime theGameTime, Movement movement)
    {
      if(!DoRemove)
        DoRemove = (Vector2.Distance(StartPosition, Position) > MAX_DISTANCE);
      if (!DoRemove)
      {
        if (_direction == MoveDirection.Right)
          Position += mDirection * mSpeed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
        else
          Position -= mDirection * mSpeed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
      }
    }

    public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
    {
      int width = SpriteTexture.Width / Columns;
      int height = SpriteTexture.Height / Rows;
      _sourceHeigth = height;
      _sourceWidth = width;
      int row = (int)((float)_currentFrameIndex / (float)Columns);
      int column = _currentFrameIndex % Columns;

      Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
      Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, height);
      //spriteBatch.Begin();
      spriteBatch.Draw(SpriteTexture, destinationRectangle, sourceRectangle, Color.White);
      //spriteBatch.End();
    }
    private MoveDirection _direction;
    public void Fire(Vector2 theStartPosition, Vector2 theSpeed, Vector2 theDirection, MoveDirection playerDirection)
    {
      _direction = playerDirection;
      if (playerDirection == MoveDirection.Right)
        Position = new Vector2(theStartPosition.X - 80, theStartPosition.Y - 25);
      else
        Position = new Vector2(theStartPosition.X - 50, theStartPosition.Y - 25);
      StartPosition = theStartPosition;
      mSpeed = theSpeed;
      mDirection = theDirection;
    }


    internal void SetHit()
    {
      DoRemove = true;
      _currentFrameIndex = 1;
    }
  }
}
