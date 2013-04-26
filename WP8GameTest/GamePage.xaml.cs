using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WindowsPhone;
using WP8GameTest.Resources;
using WP8GameTest.Entities;

namespace WP8GameTest
{
  public partial class GamePage : PhoneApplicationPage
  {
    private Game1 game;

    // Constructor
    public GamePage()
    {
      InitializeComponent();

      game = XamlGame<Game1>.Create("", XnaSurface);

    }
  }
}