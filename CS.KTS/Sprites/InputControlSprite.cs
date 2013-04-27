using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Sprites
{
  public class InputControlSprite
  {
    public enum ButtonType
    {
      Left,
      Right,
      A,
      B,
      C,
      None
    }
    public sealed class ButtonEventArgs : EventArgs
    {
      public ButtonEventArgs(ButtonType button)
      {
        Button = button;
      }
      public ButtonType Button { get; private set; }
    }
    public event EventHandler<ButtonEventArgs> Toucht;
    private Button _leftButton;
    private Button _rightButton;
    private Button _aButton;
    private Button _bButton;
    private Button _cButton;
    private readonly Microsoft.Xna.Framework.Content.ContentManager _contentManager;
    private readonly GraphicsDeviceManager _graphicsDevice;

    public InputControlSprite(Microsoft.Xna.Framework.Content.ContentManager contentManager, GraphicsDeviceManager device)
    {

      _contentManager = contentManager;
      _graphicsDevice = device;
      var viewPort = _graphicsDevice.GraphicsDevice.Viewport;

      _rightButton = new Button();
      _rightButton.LoadContent(contentManager, "ButtonRight");
      _rightButton.Scale = 1f;
      _rightButton.Position = new Vector2(0, viewPort.Width - _rightButton.Size.Height + 10);

      _leftButton = new Button();
      _leftButton.LoadContent(contentManager, "ButtonLeft");
      _leftButton.Scale = 1f;
      _leftButton.Position = new Vector2(0 + _leftButton.Size.Height, viewPort.Width - _leftButton.Size.Height + 10);


      _aButton = new Button();
      _aButton.LoadContent(contentManager, "btnB");
      _aButton.Scale = 1.2f;
      _aButton.Position = new Vector2(_graphicsDevice.GraphicsDevice.Viewport.Height - (_aButton.Size.Height * 2) -100, _graphicsDevice.GraphicsDevice.Viewport.Width - _aButton.Size.Width - 50);

      _bButton = new Button();
      _bButton.LoadContent(contentManager, "btnA");
      _bButton.Scale = 1.2f;
      _bButton.Position = new Vector2(_graphicsDevice.GraphicsDevice.Viewport.Height - (_aButton.Size.Height) - 50, _graphicsDevice.GraphicsDevice.Viewport.Width - _aButton.Size.Width - 50);

      
      _cButton = new Button();
    }

    public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
    {
      _leftButton.Draw(batch);
      _rightButton.Draw(batch);
      _aButton.Draw(batch);
      _bButton.Draw(batch);
    }

    public void OnUpdate(TouchCollection touchLocations)
    {
      if (touchLocations.Count <= 0)
      {
        RaiseToucht(ButtonType.None);
      }
      else
      {
        foreach (TouchLocation loc in touchLocations)
        {
          if (_rightButton.IsPressed(loc, _graphicsDevice.GraphicsDevice.Viewport.Width))
          {
            RaiseToucht(ButtonType.Left);
          }
          else if (_leftButton.IsPressed(loc, _graphicsDevice.GraphicsDevice.Viewport.Width))
          {
            RaiseToucht(ButtonType.Right);
          }
        }
        else if(_aButton.IsPressed(loc,_graphicsDevice.GraphicsDevice.Viewport.Width)
        {
          RaiseToucht(ButtonType.A);
        }
        else if(_bButton.IsPressed(loc,_graphicsDevice.GraphicsDevice.Viewport.Width)
        {
          RaiseToucht(ButtonType.B);
        }
      }
    }

    private void RaiseToucht(ButtonType button)
    {
      if (Toucht != null)
        Toucht(this, new ButtonEventArgs(button));
    }

  }
}
