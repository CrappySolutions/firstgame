﻿using CS.KTS.Entities;
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

    public int Hp { get; set; }
    private int _startHp;

    public EnemyWalker(GraphicsDeviceManager aGraphicsManager, string skinAsset, int rows, int columns, Vector2? startLocation = null)
      : base(skinAsset, rows, columns)
    {
      _hpBar = new HpBarSprite(aGraphicsManager,"hpBar",1,3,startLocation);
      Hp = 100;
      _startHp = 100;
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

    public void Update(GameTime gameTime)
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
        UpdateMovement(movement);
        _hpBar.UpdateFrameIndex(Position, _startHp, Hp);
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

    protected override void UpdateMovement(Movement movement)
    {
      if (mCurrentState == CharacterState.Walking)
      {
        mSpeed = Vector2.Zero;
        mDirection = Vector2.Zero;
        mSpeed.X = 80;
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
      _currentFrameIndex = 2;
      mCurrentState = CharacterState.Dead;
    }

    public void IsHit(int damage)
    {
      Hp -= damage;

      if (Hp <= 0) SetDead();
    }
  }
}
