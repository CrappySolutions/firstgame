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
using CS.KTS.Entities;

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
            switch (a.MessageType)
            {
              case MessageType.PlayerDamageDone:
                PlayerDamageDone.Text = a.Text;
                var top = GetPoint(a.Y - 50);
                var left = GetPoint(a.X + 50);
                Canvas.SetTop(PlayerDamageDone, top);
                Canvas.SetLeft(PlayerDamageDone, left);
                break;
              case MessageType.TargetHp:
                break;
              case MessageType.PlayerHp:
                PlayerHp.Text = a.Text;
                break;
              case MessageType.PlayerExp:
                break;
              case MessageType.PlayerLevel:
                break;
              default:
                break;
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


    }

    private int GetPoint(int pixel)
    {
      return pixel * 72 / 120;
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
  }
}