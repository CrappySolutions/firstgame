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
    public int HP { get; set; }
    private MoveDirection _lastDirection;
    private Random rand;

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

    public int GetDamage()
    {
      var fact = rand.Next(50, 200);
      var damage = (_baseDamage * fact / 100);
      return damage;
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

    public Player(string skinAsset, string weaponSkinAsset, int rows, int columns, Vector2 startPoint)
      : base(skinAsset, rows, columns)
    {
      rand = new Random();
      CurrentMovement = new Movement { Direction = MoveDirection.Stop, Type = MovementType.Walking };
      _lastDirection = CurrentMovement.Direction;
      Position = startPoint;
      _projectileAssetName = weaponSkinAsset;
      HP = 100;
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

    public void Update(GameTime theGameTime)
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
      base.Update(theGameTime, mSpeed, mDirection);

      if (SendProjectile)
      {
        SendProjectile = false;
        var projectile = new Projectile(_contentManager.Load<Texture2D>(_projectileAssetName), 1, 2);
        projectile.Fire(_firePosition, new Vector2(500, 0), new Vector2(1, 0), GetProjectileDirection());
        Projectiles.Add(projectile);
      }

      foreach (var projectile in Projectiles)
      {
        projectile.Update(theGameTime, CurrentMovement);
      }
    }

    private MoveDirection GetProjectileDirection()
    {
      if (_currentFrameIndex == 0 || _currentFrameIndex == 2) return MoveDirection.Right;
      return MoveDirection.Left;
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

    public Movement CurrentMovement { get; set; }
  }
}
