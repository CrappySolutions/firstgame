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
using CS.KTS.Resources;

namespace CS.KTS
{
  public partial class GamePage : PhoneApplicationPage
  {
    private bool _isDone;
    // Constructor
    public GamePage()
    {
      InitializeComponent();
      Init();
      // Sample code to localize the ApplicationBar
      //BuildLocalizedApplicationBar();
    }

    private void Init()
    {

      App.GameBoard = XamlGame<BoardOne>.Create("", XnaSurface);
      App.GameBoard.HPWriter = (a) =>
      {
        Dispatcher.BeginInvoke(() =>
          {
            if (a.MessageType == Entities.MessageType.Damage)
            {
              DamageDone.Text = a.Text;
            }
            else if (a.MessageType == Entities.MessageType.HpLeft)
            {
              HpLeft.Text = a.Text;
            }

          });
      };

      App.GameBoard.FinishedWriter = (a) =>
      {
        if (_isDone) return;
        else _isDone = true;
        Dispatcher.BeginInvoke(() =>
        {
          NavigationService.Navigate(new Uri("/Xaml/CompletedVIew.xaml", UriKind.Relative));
        });
      };
      //// Sample code to localize the ApplicationBar
      ////BuildLocalizedApplicationBar();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (App.GameBoard.IsDisposed) return;
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      App.GameBoard.ClearBoard();
      base.OnNavigatedFrom(e);
    }

    // Sample code for building a localized ApplicationBar
    //private void BuildLocalizedApplicationBar()
    //{
    //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
    //    ApplicationBar = new ApplicationBar();

    //    // Create a new button and set the text value to the localized string from AppResources.
    //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
    //    appBarButton.Text = AppResources.AppBarButtonText;
    //    ApplicationBar.Buttons.Add(appBarButton);

    //    // Create a new menu item with the localized string from AppResources.
    //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
    //    ApplicationBar.MenuItems.Add(appBarMenuItem);
    //}
  }
}