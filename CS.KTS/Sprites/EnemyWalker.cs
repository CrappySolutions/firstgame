﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    public EnemyWalker(GraphicsDeviceManager aGraphicsManager,string skinAsset, int rows, int columns)
        : base(skinAsset, rows, columns)
    {
      _graphicsManager = aGraphicsManager;
      _minX = 0;
      _maxX = _graphicsManager.GraphicsDevice.Viewport.Height - 80;
      Position = new Vector2(_maxX, 500);
      _currentDirection = MoveDirection.Left;
    }

    public override void LoadContent(ContentManager theContentManager)
    {
      _contentManager = theContentManager;
      _currentFrameIndex = 0;
      _totalFrameCount = Rows * Columns;
      base.LoadContent(theContentManager);
    }

    public void Update(GameTime gameTime)
    {
      var movement = new Movement { Direction = _currentDirection, Type = MovementType.Walking };
      if (_currentDirection == MoveDirection.Left)
      {
        if (Position.X - _minX <= 20)
        {
          _currentDirection = MoveDirection.Right;
        }
      }
      else if (_currentDirection == MoveDirection.Right)
      {
        if (_maxX - Position.X <= 20)
        {
          _currentDirection = MoveDirection.Left;
        }
      }
      UpdateMovement(movement);
      base.Update(gameTime, movement);
    }

    public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
    }
  }
}