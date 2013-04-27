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
    private List<Projectile> _projectiles = new List<Projectile>();
    private string _projectileAssetName;
    private Microsoft.Xna.Framework.Content.ContentManager _contentManager;
    public bool SendProjectile { get; set; }

    public Player(string skinAsset, string weaponSkinAsset, int rows, int columns, Vector2 startPoint)
        : base(skinAsset, rows, columns)
    {
      Position = startPoint;
      _projectileAssetName = weaponSkinAsset;
    }

    public Player(Texture2D texture, int rows, int columns, Vector2 startPoint, string projectileAssetName)
      : base(texture, rows, columns)
    {
      Position = startPoint;
      _projectileAssetName = projectileAssetName;
    }

    public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager theContentManager)
    {
      _contentManager = theContentManager;
      base.LoadContent(theContentManager);
      _currentFrameIndex = 0;
      _totalFrameCount = Rows * Columns;
      foreach (var projectile in _projectiles)
      {
        projectile.LoadContent(theContentManager, _projectileAssetName);
      }
    }

    public void Update(GameTime theGameTime)
    {
      base.Update(theGameTime, CurrentMovement);
      foreach (var projectile in _projectiles)
      {
        projectile.Update(theGameTime, CurrentMovement);
      }
      if (SendProjectile)
      {
        var firePosition = new Vector2(Position.X + 50, Position.Y + 25);
        var createProjectile = true;
        foreach (var projectile in _projectiles)
        {
          if (!projectile.DoRemove)
          {
            createProjectile = false;
            projectile.Fire(firePosition, new Vector2(200, 0), new Vector2(1, 0));
          }
        }
        if (createProjectile)
        {
          if (string.IsNullOrEmpty(_projectileAssetName))
            return;
          var projectile = new Projectile(_contentManager.Load<Texture2D>(_projectileAssetName), 0, 0);
          //_contentManager
          projectile.Fire(firePosition, new Vector2(200, 0), new Vector2(1, 0));
          _projectiles.Add(projectile);
        }
        SendProjectile = false;
      }
    }

    public override void Draw(SpriteBatch spriteBatch) 
    {
      base.Draw(spriteBatch);
      spriteBatch.Begin();
      foreach (var proj in _projectiles)
      {
        proj.Draw(spriteBatch);
      }
      spriteBatch.End();
    }

    public Movement CurrentMovement { get; set; }
  }
}
