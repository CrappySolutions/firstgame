using CS.KTS.Data.Objects;
using CS.KTS.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Sprites
{
  public class LootSprite : AnimatedSprite
  {
    private TimeSpan? _lastGameTime;

    public LootData Data { get; set; }

    public LootSprite(string assetName, int rows, int columns)
      : base(assetName, rows, columns)
    {
      Data = new LootData();
    }

    public override void Update(GameTime gameTime, Movement movement)
    {
      if (_lastGameTime == null) _lastGameTime = gameTime.TotalGameTime;

      if (((gameTime.TotalGameTime - _lastGameTime.Value).Milliseconds >= 200))
      {
        _lastGameTime = gameTime.TotalGameTime;
        _currentFrameIndex++;
        if (_currentFrameIndex == _totalFrameCount)
          _currentFrameIndex = 0;
      }

      UpdateLootMovement(movement);
      base.Update(gameTime, mSpeed, mDirection);
    }
    
    private void UpdateLootMovement(Movement movement)
    {
      var direction = MoveDirection.Stop;

      if (movement.Direction == MoveDirection.Left) direction = MoveDirection.Right;
      if (movement.Direction == MoveDirection.Right) direction = MoveDirection.Left;

      mSpeed = Vector2.Zero;
      mDirection = Vector2.Zero;
      mSpeed.X = 120;
      mDirection.X = Constants.DirectionOffsets[direction];
    }

    
  }
}
