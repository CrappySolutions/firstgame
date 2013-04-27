using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Sprites
{
  public enum MoveDirection
  {
    Left,
    Right,
    Up,
    Down,
    Stop
  }
  public enum State
  {
    Walking,
    Dead
  }

  public enum MovementType
  {
    Flying,
    Walking,
    Running
  }

  public class Movement
  {
    public MovementType Type { get; set; }

    public MoveDirection Direction { get; set; }
  }



  public abstract class AnimatedSprite : Sprite
  {
    protected static class Constants
    {
      public static readonly Dictionary<MovementType, int> MovementSpeed = new Dictionary<MovementType, int> 
      { 
        { MovementType.Walking, 160 }, 
        { MovementType.Running, 160 }, 
        { MovementType.Flying, 160 } 
      };

      public static readonly Dictionary<MoveDirection, int> DirectionOffsets = new Dictionary<MoveDirection, int> 
      {
        {MoveDirection.Down , 1},
        {MoveDirection.Up , -1},
        {MoveDirection.Left , -1},
        {MoveDirection.Right , 1},
        {MoveDirection.Stop , 0}
      };
    }

    public int Rows { get; set; }
    
    public int Columns { get; set; }
   
    protected State mCurrentState = State.Walking;
    
    protected Vector2 mDirection = Vector2.Zero;
    
    protected Vector2 mSpeed = Vector2.Zero;

    protected int _currentFrameIndex;

    protected int _totalFrameCount;

    protected int _sourceWidth;
    protected int _sourceHeigth;

    public AnimatedSprite(string assetName, int rows, int columns) 
    {
      AssetName = assetName;
      Rows = rows;
      Columns = columns;
    } 

    public AnimatedSprite(Texture2D texture, int rows, int columns)
      : base() 
    {
      this.SpriteTexture = texture;
      this.Rows = rows;
      this.Columns = columns;
      this._currentFrameIndex = 0;
      this._totalFrameCount = Rows * Columns;
    }

    public override void Update()
    {
      _currentFrameIndex++;
      if (_currentFrameIndex == _totalFrameCount)
        _currentFrameIndex = 0;
    }

    public virtual void Update(GameTime theGameTime, Movement movement) 
    {
      if (movement.Direction == MoveDirection.Stop)
      {
        _currentFrameIndex = 0;
      }
      else
      {
        _currentFrameIndex++;
        if (_currentFrameIndex == _totalFrameCount)
        {
          _currentFrameIndex = 0;
        }
      }
      UpdateMovement(movement);
      base.Update(theGameTime, mSpeed, mDirection);
    }

    public override void Draw(SpriteBatch spriteBatch) 
    {
      int width = SpriteTexture.Width / Columns;
      int height = SpriteTexture.Height / Rows;
      int row = (int)((float)_currentFrameIndex / (float)Columns);
      int column = _currentFrameIndex % Columns;

      Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
      Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, height);

      spriteBatch.Begin();
      spriteBatch.Draw(SpriteTexture, destinationRectangle, sourceRectangle, Color.White);
      spriteBatch.End();

      _sourceHeigth = height;
      _sourceWidth = width;
    }

    public override Rectangle BoundingBox
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, _sourceWidth, _sourceHeigth); 
      }
    }

    protected virtual void UpdateMovement(Movement movement)
    {
      if (mCurrentState == State.Walking)
      {
        mSpeed = Vector2.Zero;
        mDirection = Vector2.Zero;
        mSpeed.X = Constants.MovementSpeed[movement.Type];
        if (movement.Direction == MoveDirection.Up || movement.Direction == MoveDirection.Down)
          mDirection.Y = Constants.DirectionOffsets[movement.Direction];
        else
          mDirection.X = Constants.DirectionOffsets[movement.Direction];
      }
    }
  }
}
