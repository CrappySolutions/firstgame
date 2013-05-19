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
using CS.KTS.Data;

namespace CS.KTS
{
  public partial class GamePage : PhoneApplicationPage
  {
    private bool _isDone;

    DispatcherTimer _enemyDamageDoneTimer = new DispatcherTimer();
    DispatcherTimer _playerDamageDoneTimer = new DispatcherTimer();
    DispatcherTimer _playerHealingTimer = new DispatcherTimer();
    DispatcherTimer _playerStunTimer = new DispatcherTimer();
    private List<PlayerLevelUpStat> _playerLevelUpStats;

    public GamePage()
    {
      InitializeComponent();
      Init();
    }

    private void Init()
    {

      _playerLevelUpStats = new List<PlayerLevelUpStat>();

      _enemyDamageDoneTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
      _enemyDamageDoneTimer.Tick += new EventHandler(_enemyDamageDoneTimer_Tick);

      _playerDamageDoneTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
      _playerDamageDoneTimer.Tick += new EventHandler(_playerDamageDoneTimer_Tick);

      _playerHealingTimer.Interval = new TimeSpan(0, 0, 0, 1, 500);
      _playerHealingTimer.Tick += new EventHandler(_playerHealingTimer_Tick);

      _playerStunTimer.Interval = new TimeSpan(0, 0, 0, 3, 0);
      _playerStunTimer.Tick += new EventHandler(_playerStunTimer_Tick);

      CharacterScreen.Visibility = System.Windows.Visibility.Collapsed;
      tbPlayerName.Text = App.PlayerData.Name;
      pgCastTime.Value = 0;

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
                  _playerDamageDoneTimer.Start();
                  break;
                case MessageType.EnemyDamageDone:
                  SetFloatingCombatTextPlayerTakeDamage(message);
                  _enemyDamageDoneTimer.Start();
                  break;
                case MessageType.PlayerExp:
                  pbPlayerXp.Maximum = message.Number2;
                  pbPlayerXp.Value = message.Number;
                  tbPlayerXp.Text = message.Number.ToString() + " / " + message.Number2;
                  break;
                case MessageType.PlayerLevel:
                  tbPlayerLevel.Text = message.Text;
                  break;
                case MessageType.PlayerHp:
                  tbPlayerHp.Text = message.Number.ToString() + " / " + message.Number2;
                  pbPlayerHp.Maximum = message.Number2;
                  pbPlayerHp.Value = message.Number;
                  break;
                case MessageType.PlayerHealing:
                  if (message.Number <= 0) return;
                  SetFloatingCombatTextPlayerHealing(message);
                  _playerHealingTimer.Start();
                  break;
                case MessageType.PlayerHealingCooldown:
                  if (message.Number == 0)
                  {
                    tbHealingCooldown.Visibility = System.Windows.Visibility.Collapsed;
                  }
                  else
                  {
                    tbHealingCooldown.Text = message.Number.ToString();
                    tbHealingCooldown.Visibility = System.Windows.Visibility.Visible;
                  }
                  break;
                case MessageType.PlayerStunCooldown:
                  if (message.Number == 0)
                  {
                    tbStunCooldown.Visibility = System.Windows.Visibility.Collapsed;
                  }
                  else
                  {
                    tbStunCooldown.Text = message.Number.ToString();
                    tbStunCooldown.Visibility = System.Windows.Visibility.Visible;
                  }
                  break;
                case MessageType.PlayerBeemCooldown:
                  if (message.Number == 0)
                  {
                    tbBeemCooldown.Visibility = System.Windows.Visibility.Collapsed;
                  }
                  else
                  {

                    tbBeemCooldown.Text = message.Number.ToString();
                    tbBeemCooldown.Visibility = System.Windows.Visibility.Visible;

                  }
                  break;
                case MessageType.PlayerStun:
                  tbBigMessage.Text = message.Text;
                  tbBigMessage.Visibility = System.Windows.Visibility.Visible;
                  _playerStunTimer.Start();
                  break;
                case MessageType.PlayerDps:
                  tbPlayerDps.Text = message.Text;
                  break;
                case MessageType.PlayerGold:
                  tbPlayerGold.Text = message.Number.ToString();
                  break;
                case MessageType.BigMessage:
                  tbBigMessage.Text = "LEVEL " + message.Text;
                  tbBigMessage.Visibility = System.Windows.Visibility.Visible;
                  _playerStunTimer.Start();
                  break;
              }
            }

          });
      };
      App.GameBoard.SendCharacterStats = (stats) =>
        {
          Dispatcher.BeginInvoke(() =>
          {
            tbHealth.Text = stats.MaxHp.ToString();
            tbDefense.Text = stats.Defence.ToString();
            tbAccuracy.Text = stats.Accuracy.ToString();
            tbStrength.Text = stats.Strenght.ToString();
            tbDamage.Text = stats.GetPlayerLowDamage.ToString() + " - " + stats.GetPlayerHighDamage;
            tbCritical.Text = (stats.CriticalChance * 100).ToString() + "%";
            tbWeaponDamage.Text = stats.EquipedWeapon.MinDamage.ToString() + " - " + stats.EquipedWeapon.MaxDamage;
            var fireRate = Math.Round((double)(1000 / stats.EquipedWeapon.FireRate), 0);
            tbWeaponFireRate.Text = fireRate.ToString() + "/s";
            tbMaxDamage.Text = stats.MaxDamage.ToString();
            tbMaxDps.Text = stats.MaxDps.ToString();
            tbEnemiesKilled.Text = stats.Killedenemies.ToString();
            tbWeaponRange.Text = stats.EquipedWeapon.Distance.ToString();
            tbSkillPoints.Text = stats.UnspentSkillPoints.ToString();
            tbHealing.Text = stats.Healing.ToString();

            CharacterScreen.Visibility = System.Windows.Visibility.Visible;
            gridPlayer.Visibility = System.Windows.Visibility.Visible;
            gridWeapon.Visibility = System.Windows.Visibility.Visible;
            gridStats.Visibility = System.Windows.Visibility.Collapsed;


            lbWeapons.Items.Clear();
            stats.Weapons.ForEach(w => lbWeapons.Items.Add(w.Name));

            SetSkillPointBtnVisibility(stats.UnspentSkillPoints > 0);
          });
        };

      App.GameBoard.SendCastTime = (castTime, currentTime) =>
      {
        Dispatcher.BeginInvoke(() =>
        {
          pgCastTime.Visibility = System.Windows.Visibility.Visible;
          pgCastTime.Maximum = castTime;
          pgCastTime.Value = currentTime;
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


    void _enemyDamageDoneTimer_Tick(object sender, EventArgs e)
    {
      EnemyDamageDone.Visibility = System.Windows.Visibility.Collapsed;
      _enemyDamageDoneTimer.Stop();
    }

    void _playerDamageDoneTimer_Tick(object sender, EventArgs e)
    {
      PlayerDamageDone.Visibility = System.Windows.Visibility.Collapsed;
      _playerDamageDoneTimer.Stop();
    }

    void _playerHealingTimer_Tick(object sender, EventArgs e)
    {
      PlayerHealing.Visibility = System.Windows.Visibility.Collapsed;
      _playerHealingTimer.Stop();
    }

    void _playerStunTimer_Tick(object sender, EventArgs e)
    {
      tbBigMessage.Visibility = System.Windows.Visibility.Collapsed;
      _playerStunTimer.Stop();
    }

    private void SetFloatingCombatText(Message message)
    {
      PlayerDamageDone.Visibility = System.Windows.Visibility.Visible;
      PlayerDamageDone.Text = message.Text;
      var top = GetPoint(message.Y - 60);
      var left = GetPoint(message.X + 50);

      if (message.IsCritical)
      {
        PlayerDamageDone.FontSize = 30;
        top -= 30;
      }
      else
      {
        PlayerDamageDone.FontSize = 15;
      }

      Canvas.SetTop(PlayerDamageDone, top);
      Canvas.SetLeft(PlayerDamageDone, left);
    }

    private void SetFloatingCombatTextPlayerTakeDamage(Message message)
    {
      EnemyDamageDone.Visibility = System.Windows.Visibility.Visible;
      EnemyDamageDone.Text = "-" + message.Text;
      var top = GetPoint(message.Y + 30);
      var left = GetPoint(message.X);

      if (message.IsCritical)
      {
        EnemyDamageDone.FontSize = 30;
        top -= 30;
      }
      else
      {
        EnemyDamageDone.FontSize = 15;
      }

      Canvas.SetTop(EnemyDamageDone, top);
      Canvas.SetLeft(EnemyDamageDone, left);
    }

    private void SetFloatingCombatTextPlayerHealing(Message message)
    {
      PlayerHealing.Visibility = System.Windows.Visibility.Visible;
      PlayerHealing.Text = "+" + message.Text;
      var top = GetPoint(message.Y - 50);
      var left = GetPoint(message.X);

      Canvas.SetTop(PlayerHealing, top);
      Canvas.SetLeft(PlayerHealing, left);
    }

    private int GetPoint(int pixel)
    {
      return pixel * 72 / 120;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (App.GameBoard.IsDisposed) NavigationService.Navigate(new Uri("/Xaml/Menu.xaml", UriKind.Relative));
      base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      App.GameBoard.PauseGame();

      App.GameBoard.ClearBoard();
      base.OnNavigatedFrom(e);
    }

    private void btnCloseCharacterScreen_Click(object sender, RoutedEventArgs e)
    {
      CharacterScreen.Visibility = System.Windows.Visibility.Collapsed;
      App.GameBoard.UnPauseGame();
    }

    private void btnPlayer_Checked(object sender, RoutedEventArgs e)
    {
      btnPlayer.IsChecked = true;
      btnStats.IsChecked = false;

      gridPlayer.Visibility = System.Windows.Visibility.Visible;
      gridWeapon.Visibility = System.Windows.Visibility.Visible;
      gridStats.Visibility = System.Windows.Visibility.Collapsed;
    }

    private void btnStats_Checked(object sender, RoutedEventArgs e)
    {
      btnPlayer.IsChecked = false;
      btnStats.IsChecked = true;
      gridPlayer.Visibility = System.Windows.Visibility.Collapsed;
      gridWeapon.Visibility = System.Windows.Visibility.Collapsed;
      gridStats.Visibility = System.Windows.Visibility.Visible;
    }

    private void btnPlayer_Click(object sender, RoutedEventArgs e)
    {
      btnPlayer.IsChecked = true;
    }

    private void btnStats_Click(object sender, RoutedEventArgs e)
    {
      btnStats.IsChecked = true;
    }

    private void lvlUp_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      var statName = (sender as Grid).Name;
      switch (statName)
      {
        case "lvlUpDefence":
          _playerLevelUpStats.Add(new PlayerLevelUpStat { PlayerStat = PlayerStats.Defence, Points = 1 });
          break;
        case "lvlUpAccracy":
          _playerLevelUpStats.Add(new PlayerLevelUpStat { PlayerStat = PlayerStats.Accuracy, Points = 1 });
          break;
        case "lvlUpStrength":
          _playerLevelUpStats.Add(new PlayerLevelUpStat { PlayerStat = PlayerStats.Strength, Points = 1 });
          break;
        case "lvlUpCritical":
          _playerLevelUpStats.Add(new PlayerLevelUpStat { PlayerStat = PlayerStats.CriticalChance, Points = 1 });
          break;
        case "lvlUpHealing":
          _playerLevelUpStats.Add(new PlayerLevelUpStat { PlayerStat = PlayerStats.Healing, Points = 1 });
          break;
        default:
          break;
      }
      App.GameBoard.UpdatePlayerStats(_playerLevelUpStats);
      _playerLevelUpStats = new List<PlayerLevelUpStat>();
    }

    private void SetSkillPointBtnVisibility(bool visible)
    {
      if (visible)
      {
        lvlUpDefence.Visibility = System.Windows.Visibility.Visible;
        lvlUpAccracy.Visibility = System.Windows.Visibility.Visible;
        lvlUpStrength.Visibility = System.Windows.Visibility.Visible;
        lvlUpCritical.Visibility = System.Windows.Visibility.Visible;
        lvlUpHealing.Visibility = System.Windows.Visibility.Visible;
      }
      else
      {
        lvlUpDefence.Visibility = System.Windows.Visibility.Collapsed;
        lvlUpAccracy.Visibility = System.Windows.Visibility.Collapsed;
        lvlUpStrength.Visibility = System.Windows.Visibility.Collapsed;
        lvlUpCritical.Visibility = System.Windows.Visibility.Collapsed;
        lvlUpHealing.Visibility = System.Windows.Visibility.Collapsed;
      }
    }

    private void lbWeapons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var weaponName = lbWeapons.SelectedItem;
      if (weaponName == null) return;
      App.GameBoard.ChangeWeapon(weaponName.ToString());
    }

    private void pgCastTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (pgCastTime == null) return;

      if (pgCastTime.Value >= pgCastTime.Maximum - 0.1)
      {
        pgCastTime.Value = 0;
        pgCastTime.Visibility = System.Windows.Visibility.Collapsed;
      }
    }

    private void Map_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      App.GameBoard.PauseGame();
      mapGrid.Visibility = System.Windows.Visibility.Visible;
    }

    private void MapBtnClose_Click(object sender, RoutedEventArgs e)
    {
      App.GameBoard.UnPauseGame();
      mapGrid.Visibility = System.Windows.Visibility.Collapsed;
    }

    private void MapButton_Click(object sender, RoutedEventArgs e)
    {
      
    }

    

  }
}