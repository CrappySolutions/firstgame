using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Sprites
{
  public class Button : Sprite
  {
    public bool IsPressed(TouchLocation touchLocation, float gameWindowHeight)
    {
      var gY = gameWindowHeight - touchLocation.Position.X;
      var gX = touchLocation.Position.Y;

      if ((gX > Position.X && gX < (Position.X + Size.Width)) && (gY < (Position.Y + Size.Height)) && gY > Position.Y)
      {
        return true;
      }
      return false;
    }
  }
}
