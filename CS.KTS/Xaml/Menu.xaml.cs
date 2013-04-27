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
  public partial class Menu : PhoneApplicationPage
  {
    public Menu()
    {
      InitializeComponent();
    }

    private void BtnNewGame_Click(object sender, RoutedEventArgs e)
    {
      NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
    }

    private void BtnLoadGame_Click(object sender, RoutedEventArgs e)
    {

    }

    private void BtnSaveGame_Click(object sender, RoutedEventArgs e)
    {

    }

    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {

    }


  }
}