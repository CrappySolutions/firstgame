using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WP8GameTest.Views
{
  public partial class CharacterSelections : PhoneApplicationPage
  {
    public CharacterSelections()
    {
      InitializeComponent();
    }

    private void BtnCharacter_Click(object sender, RoutedEventArgs e)
    {
      var button = sender as Button;

      switch (button.Name)
      {
        case "btnCharacterA":
          NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
          break;
        case "btnCharacterB":
          NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
          break;
        case "btnCharacterC":
          NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
          break;
        case "btnExit":
          break;
        default:
          break;
      }
    }

    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
      NavigationService.Navigate(new Uri("/Menu.xaml", UriKind.Relative));
    }
  }
}