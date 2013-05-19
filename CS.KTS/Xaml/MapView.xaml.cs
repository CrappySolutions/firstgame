using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CS.KTS.Xaml
{
  public partial class MapView : PhoneApplicationPage
  {
    public MapView()
    {
      InitializeComponent();
    }

    private void MapButton_Click(object sender, RoutedEventArgs e)
    {
      var buttonName = ((Button)sender).Name;

      switch (buttonName)
      {
        case "Instance1":
          break;
        case "Instance2":
          break;
        case "Instance3":
          break;
        case "Instance4":
          break;
        case "Instance5":
          break;
        case "Instance6":
          break;
        default:
          break;
      }

      NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
      NavigationService.Navigate(new Uri("/Xaml/CreateNewCharacterView.xaml", UriKind.Relative));
    }
  }
}