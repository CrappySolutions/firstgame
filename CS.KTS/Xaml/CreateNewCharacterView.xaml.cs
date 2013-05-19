using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CS.KTS.Entities;

namespace CS.KTS.Xaml
{
  public partial class CreateNewCharacterView : PhoneApplicationPage
  {
    public CreateNewCharacterView()
    {
      InitializeComponent();
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
      NavigationService.Navigate(new Uri("/Xaml/Menu.xaml", UriKind.Relative));
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e)
    {
      if (panorama.SelectedIndex == 0)
      {
        App.PlayerData = new Data.PlayerData { Name = PlayerName.Text, Class = CharacterClass.Warrior };
      }
      else if (panorama.SelectedIndex == 1)
      {
        App.PlayerData = new Data.PlayerData { Name = PlayerName.Text, Class = CharacterClass.Hunter };
      }
      
      NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
      
    }
  }
}