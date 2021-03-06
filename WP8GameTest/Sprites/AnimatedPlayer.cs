﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WP8GameTest.Entities;

namespace WP8GameTest.Sprites
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

    public List<Fireball> Fireballs;
    ContentManager mContentManager;

    enum State
    {
      Walking
    }
    
    State mCurrentState = State.Walking;
    Vector2 mDirection = Vector2.Zero;
    Vector2 mSpeed = Vector2.Zero;

    public void LoadContent(ContentManager theContentManager, string assetname, int rows, int columns, Vector2 startPosition)
    {
      Fireballs = new List<Fireball>();
      mContentManager = theContentManager;
      Rows = rows;
      Columns = columns;
      currentFrame = 0;
      totalFrames = Rows * Columns;
      Position = startPosition;

      foreach (Fireball aFireball in Fireballs)
      {
        aFireball.LoadContent(theContentManager);
      }

      Position = startPosition;
      Source = new Rectangle(0, 0, 200, Size.Height);

      base.LoadContent(theContentManager, assetname);
    }

    Rectangle mSource;
    public Rectangle Source
    {
      get { return mSource; }
      set
      {
        mSource = value;
        Size = new Rectangle(0, 0, (int)(mSource.Width * Scale), (int)(mSource.Height * Scale));
      }
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

      UpdateFireball(theGameTime, moveDirection);
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
      foreach (Fireball aFireball in Fireballs)
      {
        aFireball.Draw(spriteBatch);
      }
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

    public bool Fire;

    private void UpdateFireball(GameTime theGameTime, MoveDirection moveDirection)
    {
      foreach (Fireball aFireball in Fireballs)
      {
        aFireball.Update(theGameTime);
      }

      if (Fire)
      {
        ShootFireball();
        Fire = false;
      }
    }

    private void ShootFireball()
    {
      if (mCurrentState == State.Walking)
      {
        bool aCreateNew = true;
        var firePosition = new Vector2(Position.X + 50, Position.Y + 25);
        foreach (Fireball aFireball in Fireballs)
        {
          if (aFireball.Visible == false)
          {
            aCreateNew = false;
            aFireball.Fire(firePosition, new Vector2(200, 0), new Vector2(1, 0));
            break;
          }
        }
        
        if (Fireballs.Count > 0) return;

        if (aCreateNew == true)
        {
          Fireball aFireball = new Fireball();
          aFireball.LoadContent(mContentManager);
          //aFireball.Fire(new Vector2(Position.X, Position.Y) + new Vector2(Size.Width / 2, Size.Height / 2), new Vector2(200, 200), new Vector2(1, 0));
          aFireball.Fire(firePosition, new Vector2(200, 200), new Vector2(1, 0));
          Fireballs.Add(aFireball);
        }
      }
    }

  }
}
