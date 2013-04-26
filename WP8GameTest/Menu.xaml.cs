using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WP8GameTest
{
  public partial class Menu : PhoneApplicationPage
  {
    public Menu()
    {
      InitializeComponent();
      //GamePage game = new GamePage();
      //Game1 game1 = new Game1();
    }

    private void menuBtn_Click(object sender, RoutedEventArgs e)
    {
      var button = sender as Button;

      switch (button.Name)
      {
        case "btnNewGame":
          NavigationService.Navigate(new Uri("/Views/CharacterSelections.xaml", UriKind.Relative));
          break;
        case "btnLoadGame":
          break;
        case "btnSettings":
          break;
        case "btnExit":
          break;
        default:
          break;
      }

    }
  }
}