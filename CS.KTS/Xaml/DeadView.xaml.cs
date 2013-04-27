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
  public partial class DeadView : PhoneApplicationPage
  {
    public DeadView()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      NavigationService.Navigate(new Uri("/Xaml/Menu.xaml", UriKind.Relative));
    }
  }
}