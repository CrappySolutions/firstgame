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
        private BoardOne _game;

        // Constructor
        public GamePage()
        {
            InitializeComponent();

            _game = XamlGame<BoardOne>.Create("", XnaSurface);
            _game.ConsoleWrite = (a) => { Dispatcher.BeginInvoke(() => Test.Text = a); };
            _game.HPWriter = (a) => { 
              Dispatcher.BeginInvoke(() => 
                {
                    HP.Text = int.Parse(a) <= 0 ? "You are Dead" : string.Format("HP: {0}", a);
                    if (int.Parse(a) <= 0)
                    {
                      NavigationService.Navigate(new Uri("/Xaml/DeadView.xaml", UriKind.Relative));
                    }
                }); 
            };

            _game.FinishedWriter = (a) => 
            {
              Dispatcher.BeginInvoke(() =>
              {
                Test.Text = a;
                NavigationService.Navigate(new Uri("/Xaml/CompletedVIew.xaml", UriKind.Relative));
              });
            };
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
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