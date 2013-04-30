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
using System.Windows.Threading;

namespace CS.KTS
{
  public partial class GamePage : PhoneApplicationPage
  {
    private bool _isDone;

    DispatcherTimer dt = new DispatcherTimer();

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

      dt.Interval = new TimeSpan(0, 0, 0, 0, 500); // 500 Milliseconds
      dt.Tick += new EventHandler(dt_Tick);
      dt.Start();


      App.GameBoard = XamlGame<BoardOne>.Create("", XnaSurface);
      App.GameBoard.GuiMessageWriter = (a) =>
      {
        Dispatcher.BeginInvoke(() =>
          {
            foreach (var message in a)
            {
              switch (message.MessageType)
              {
                case MessageType.PlayerDamageDone:
                  SetFloatingCombatText(message);
                  break;
                //case MessageType.TargetHp:
                //  break;
                //case MessageType.PlayerHp:
                //  tbPlayerHp.Text = message.Text;
                //  break;
                //case MessageType.PlayerExp:
                //  tbPlayerXp.Text = message.Text;
                //  break;
                //case MessageType.PlayerLevel:
                //  tbPlayerLevel.Text = message.Text;
                //  break;
                //case MessageType.PlayerHpPercent:
                //  //var hpPercent = double.Parse(message.Text);
                //  //pbPlayerHp.Value = hpPercent;
                //  break;
                //case MessageType.PlayerXpPercent:
                //  //var xpPercent = double.Parse(message.Text);
                //  //pbPlayerXp.Value = xpPercent;
                //  break;
                //case MessageType.InitPlayerMaxHp:
                //  pbPlayerHp.Maximum = message.Number;
                //  break;
                //default:
                //  break;
              }

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

    private void SetFloatingCombatText(Message message)
    {
      PlayerDamageDone.Visibility = System.Windows.Visibility.Visible;
      PlayerDamageDone.Text = message.Text;
      var top = GetPoint(message.Y - 50);
      var left = GetPoint(message.X + 50);
      Canvas.SetTop(PlayerDamageDone, top);
      Canvas.SetLeft(PlayerDamageDone, left);
    }


    private int GetPoint(int pixel)
    {
      return pixel * 72 / 120;
    }

    void dt_Tick(object sender, EventArgs e)
    {
      PlayerDamageDone.Visibility = System.Windows.Visibility.Collapsed;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (App.GameBoard.IsDisposed) NavigationService.Navigate(new Uri("/Xaml/Menu.xaml", UriKind.Relative));
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      App.GameBoard.ClearBoard();
      base.OnNavigatedFrom(e);
    }
  }
}