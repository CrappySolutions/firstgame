using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
      D,
      E,
      F,
      GameMenu,
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
    private Button _dButton;
    private Button _eButton;
    private Button _gameMenuButton;
    private readonly ContentManager _contentManager;
    private readonly GraphicsDeviceManager _graphicsDevice;

    public InputControlSprite(ContentManager contentManager, GraphicsDeviceManager device)
    {
      _contentManager = contentManager;
      _graphicsDevice = device;
      var viewPort = _graphicsDevice.GraphicsDevice.Viewport;

      _leftButton = new Button();
      _leftButton.LoadContent(contentManager, "btnLeft");
      _leftButton.Scale = 1.2f;
      _leftButton.Position = new Vector2(0, viewPort.Width - _leftButton.Size.Height - 20);

      _rightButton = new Button();
      _rightButton.LoadContent(contentManager, "btnRight");
      _rightButton.Scale = 1.2f;
      _rightButton.Position = new Vector2(_rightButton.Size.Height + 20, viewPort.Width - _rightButton.Size.Height - 20);

      _aButton = new Button();
      _aButton.LoadContent(contentManager, "btn");
      _aButton.Scale = 1.3f;
      _aButton.Position = new Vector2(_graphicsDevice.GraphicsDevice.Viewport.Height - (_aButton.Size.Height) - 50, _graphicsDevice.GraphicsDevice.Viewport.Width - _aButton.Size.Width - 30);

      _bButton = new Button();
      _bButton.LoadContent(contentManager, "btn");
      _bButton.Scale = 1.3f;
      _bButton.Position = new Vector2(_aButton.Position.X - 110, _aButton.Position.Y);

      _cButton = new Button();
      _cButton.LoadContent(contentManager, "btn");
      _cButton.Scale = 1.3f;
      _cButton.Position = new Vector2(_bButton.Position.X - 110, _bButton.Position.Y);

      _dButton = new Button();
      _dButton.LoadContent(contentManager, "btn");
      _dButton.Scale = 1.3f;
      _dButton.Position = new Vector2(_cButton.Position.X - 110, _cButton.Position.Y);

      _eButton = new Button();
      _eButton.LoadContent(contentManager, "btn");
      _eButton.Scale = 1.3f;
      _eButton.Position = new Vector2(_dButton.Position.X - 110, _dButton.Position.Y);

      _gameMenuButton = new Button();
      _gameMenuButton.LoadContent(contentManager, "btnMenu");
      _gameMenuButton.Scale = .7f;
      _gameMenuButton.Position = new Vector2(350,650);

    }

    public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
    {
      _leftButton.Draw(batch);
      _rightButton.Draw(batch);
      _aButton.Draw(batch);
      _bButton.Draw(batch);
      _cButton.Draw(batch);
      _dButton.Draw(batch);
      _eButton.Draw(batch);
      _gameMenuButton.Draw(batch);

    }

    public void IsEnabled(ButtonType buttonType, bool value)
    {

      switch (buttonType)
      {
        case ButtonType.Left:
          break;
        case ButtonType.Right:
          break;
        case ButtonType.A:
          if (value == true) _aButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
          if (value == false) _aButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
          break;
        case ButtonType.B:
          if (value == true) _bButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
          if (value == false) _bButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
          break;
        case ButtonType.C:
          if (value == true) _cButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
          if (value == false) _cButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
          break;
        case ButtonType.D:
          if (value == true) _dButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
          if (value == false) _dButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
          break;
        case ButtonType.E:
          if (value == true) _eButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
          if (value == false) _eButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
          break;
        case ButtonType.GameMenu:
          //if (value == true) _gameMenuButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
          //if (value == false) _gameMenuButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
          break;
        case ButtonType.None:
          break;
        default:
          break;
      }
    }

    public void OnUpdate(TouchCollection touchLocations, bool isPaused)
    {
      if (touchLocations.Count <= 0)
      {
        RaiseToucht(ButtonType.None, isPaused);
      }
      else
      {
        foreach (TouchLocation loc in touchLocations)
        {
          if (_rightButton.IsPressed(loc, _graphicsDevice.GraphicsDevice.Viewport.Width))
          {
            RaiseToucht(ButtonType.Right, isPaused);
          }
          else if (_leftButton.IsPressed(loc, _graphicsDevice.GraphicsDevice.Viewport.Width))
          {
            RaiseToucht(ButtonType.Left, isPaused);
          }
          else if (_aButton.IsPressed(loc, _graphicsDevice.GraphicsDevice.Viewport.Width))
          {
            _aButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
            if (loc.State == TouchLocationState.Released)
            {
              RaiseToucht(ButtonType.A, isPaused);
              _aButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
            }
          }
          else if (_bButton.IsPressed(loc, _graphicsDevice.GraphicsDevice.Viewport.Width))
          {
            _bButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
            if (loc.State == TouchLocationState.Released)
            {
              RaiseToucht(ButtonType.B, isPaused);
              _bButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
            }
          }
          else if (_cButton.IsPressed(loc, _graphicsDevice.GraphicsDevice.Viewport.Width))
          {
            _cButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
            if (loc.State == TouchLocationState.Released)
            {
              RaiseToucht(ButtonType.C, isPaused);
              _cButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
            }
          }
           else if (_dButton.IsPressed(loc, _graphicsDevice.GraphicsDevice.Viewport.Width))
          {
            _dButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
            if (loc.State == TouchLocationState.Released)
            {
              RaiseToucht(ButtonType.D, isPaused);
              _dButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
            }
          }
          else if (_eButton.IsPressed(loc, _graphicsDevice.GraphicsDevice.Viewport.Width))
          {
            _eButton.SpriteTexture = _contentManager.Load<Texture2D>("btnDisabled");
            if (loc.State == TouchLocationState.Released)
            {
              RaiseToucht(ButtonType.E, isPaused);
              _eButton.SpriteTexture = _contentManager.Load<Texture2D>("btn");
            }
          }
          else if (_gameMenuButton.IsPressed(loc, _graphicsDevice.GraphicsDevice.Viewport.Width))
          {
            if (loc.State == TouchLocationState.Released)
            {
              RaiseToucht(ButtonType.GameMenu, isPaused);
            }
          }
        }
      }
    }

    private void RaiseToucht(ButtonType button, bool isPaused)
    {
      if (Toucht != null)
      {
        if (isPaused)
        {
          if (button == ButtonType.GameMenu)
          {
            Toucht(this, new ButtonEventArgs(button));
          }
        }
        else
        {
          Toucht(this, new ButtonEventArgs(button));
        }
      }
    }

  }
}
