using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Sprites
{
  public class InputControlSprite
  {
    private Button _leftButton;
    private Button _rightButton;
    private Button _aButton;
    private Button _bButton;
    private readonly Microsoft.Xna.Framework.Content.ContentManager _contentManager;
    private readonly GraphicsDeviceManager _graphicsDevice;
    public InputControlSprite(Microsoft.Xna.Framework.Content.ContentManager contentManager, GraphicsDeviceManager device)
    {
      _contentManager = contentManager;
      _graphicsDevice = device;
      var viewPort = _graphicsDevice.GraphicsDevice.Viewport;
      _leftButton = new Button();
      _leftButton.LoadContent(contentManager, "ButtonLeft");
      _leftButton.Scale = 2f;
      _leftButton.Position = new Vector2(200, 200);//viewPort.Width - (_leftButton.Size.Width * 2) - 20, viewPort.Height - (_leftButton.Size.Height));

      
      _rightButton = new Button();
      _rightButton.LoadContent(contentManager, "ButtonRight");
      _rightButton.Scale = 2f;
      _rightButton.Position = new Vector2(400, 200); //(viewPort.Width - (_rightButton.Size.Width * 2) - 10, viewPort.Height - (_rightButton.Size.Height));
      
      _aButton = new Button();
      _bButton = new Button();
    }

    public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
    {
      _leftButton.Draw(batch);
      _rightButton.Draw(batch);
    }
    

  }
}
