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

    public Player(string skinAsset, string weaponSkinAsset, int rows, int columns, Vector2 startPoint)
      : base(skinAsset, rows, columns)
    {
      Position = startPoint;
      _projectileAssetName = weaponSkinAsset;
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
        _currentFrameIndex = 1;
      else if (CurrentMovement.Direction == MoveDirection.Right)
        _currentFrameIndex = 0;

      UpdateMovement(CurrentMovement);
      base.Update(theGameTime, mSpeed, mDirection);
      //base.Update(theGameTime, CurrentMovement);

      if (SendProjectile)
      {
        SendProjectile = false;
        var firePosition = new Vector2(Position.X + 90, Position.Y + 25);
        var projectile = new Projectile(_contentManager.Load<Texture2D>(_projectileAssetName), 1, 2);
        projectile.Fire(firePosition, new Vector2(500, 0), new Vector2(1, 0));
        Projectiles.Add(projectile);
      }
      
      foreach (var projectile in Projectiles)
      {
        projectile.Update(theGameTime, CurrentMovement);
      }
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
