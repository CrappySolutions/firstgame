using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CS.KTS.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CS.KTS.Sprites
{
  public class HpBarSprite : AnimatedSprite
  {
    public HpBarSprite(GraphicsDeviceManager aGraphicsManager, string skinAsset, int rows, int columns, Vector2? startLocation = null)
      : base(skinAsset, rows, columns)
    {
    }

    public override void LoadContent(ContentManager theContentManager)
    {
      _currentFrameIndex = 0;
      base.LoadContent(theContentManager);
    }

    public void UpdateFrameIndex(Vector2 walkerPosition, int walkerStartHp, int walkerCurrentHp)
    {
      var hpPercent = ((double)walkerCurrentHp / walkerStartHp);
      if (hpPercent > 0.7)
      {
        _currentFrameIndex = 0;
      }
      else if (hpPercent <= 0.7 && hpPercent > 0.3)
      {
        _currentFrameIndex = 1;
      }
      else
      {
        _currentFrameIndex = 2;
      }

      Position = new Vector2(walkerPosition.X, walkerPosition.Y - 50);

     // base.Update(gameTime, mSpeed, mDirection);
    }
  }
}
