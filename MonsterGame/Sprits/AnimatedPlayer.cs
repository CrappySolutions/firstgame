using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
  public class AnimatedPlayer : Sprite
  {
    const int WIZARD_SPEED = 160;
    const int MOVE_UP = -1;
    const int MOVE_DOWN = 1;
    const int MOVE_LEFT = -1;
    const int MOVE_RIGHT = 1;

    public int Rows { get; set; }
    public int Columns { get; set; }
    private int currentFrame;
    private int totalFrames;

    enum State
    {
      Walking
    }
    State mCurrentState = State.Walking;
    Vector2 mDirection = Vector2.Zero;
    Vector2 mSpeed = Vector2.Zero;

    public void LoadContent(ContentManager theContentManager, string assetname, int rows, int columns, Vector2 startPosition)
    {
      Rows = rows;
      Columns = columns;
      currentFrame = 0;
      totalFrames = Rows * Columns;
      Position = startPosition;
      base.LoadContent(theContentManager, assetname);
    }

    public void Update(GameTime theGameTime, MoveDirection moveDirection)
    {
      if (moveDirection == MoveDirection.Stop)
      {
        currentFrame = 0;
      }
      else
      {
        currentFrame++;
        if (currentFrame == totalFrames)
        {
          currentFrame = 0;
        }
      }

      UpdateMovement(moveDirection);

      base.Update(theGameTime, mSpeed, mDirection);
    }

    private void UpdateMovement(MoveDirection moveDirection)
    {
      if (mCurrentState == State.Walking)
      {
        mSpeed = Vector2.Zero;
        mDirection = Vector2.Zero;

        switch (moveDirection)
        {
          case MoveDirection.Left:
            mSpeed.X = WIZARD_SPEED;
            mDirection.X = MOVE_LEFT;
            break;
          case MoveDirection.Right:
            mSpeed.X = WIZARD_SPEED;
            mDirection.X = MOVE_RIGHT;
            break;
          case MoveDirection.Up:
            mSpeed.Y = WIZARD_SPEED;
            mDirection.Y = MOVE_UP;
            break;
          case MoveDirection.Down:
            mSpeed.Y = WIZARD_SPEED;
            mDirection.Y = MOVE_DOWN;
            break;
          case MoveDirection.Stop:
            mSpeed.Y = 0;
            mSpeed.X = 0;
            break;
          default:
            break;
        }

        //if (aCurrentKeyboardState.IsKeyDown(Keys.Left) == true)
        //{
        //  mSpeed.X = WIZARD_SPEED;
        //  mDirection.X = MOVE_LEFT;
        //}
        //else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true)
        //{
        //  mSpeed.X = WIZARD_SPEED;
        //  mDirection.X = MOVE_RIGHT;
        //}

        //if (aCurrentKeyboardState.IsKeyDown(Keys.Up) == true)
        //{
        //  mSpeed.Y = WIZARD_SPEED;
        //  mDirection.Y = MOVE_UP;
        //}
        //else if (aCurrentKeyboardState.IsKeyDown(Keys.Down) == true)
        //{
        //  mSpeed.Y = WIZARD_SPEED;
        //  mDirection.Y = MOVE_DOWN;
        //}
      }
    }

    public void DrawAnimation(SpriteBatch spriteBatch)
    {
      int width = SpriteTexture.Width / Columns;
      int height = SpriteTexture.Height / Rows;
      int row = (int)((float)currentFrame / (float)Columns);
      int column = currentFrame % Columns;

      Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
      Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, height);

      spriteBatch.Begin();
      spriteBatch.Draw(SpriteTexture, destinationRectangle, sourceRectangle, Color.White);
      spriteBatch.End();
    }

    public MoveDirection CanMove(float gameWindowWidth, float gameWindowHeight, MoveDirection playerMoveDirection)
    {
      var _MaxX = gameWindowWidth - (Size.Width / Columns ) - 100;
      var _MinX = 100;
      var _MaxY = gameWindowHeight - Size.Height - 100;
      var _MinY = 0;

      // Check for bounce.
      if (Position.X > _MaxX)
      {
        if (playerMoveDirection == MoveDirection.Right)
          return MoveDirection.Stop;
      }
      else if (Position.X < _MinX)
      {
        if (playerMoveDirection == MoveDirection.Left)
          return MoveDirection.Stop;
      }

      if (Position.Y > _MaxY)
      {
        if (playerMoveDirection == MoveDirection.Up)
          return MoveDirection.Stop;
      }
      else if (Position.Y < _MinY)
      {
        if (playerMoveDirection == MoveDirection.Down)
          return MoveDirection.Stop;
      }
      return playerMoveDirection;
    }

  }
}
