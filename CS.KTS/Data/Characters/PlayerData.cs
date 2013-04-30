using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data
{
  public class PlayerData : Character
  {
    public int MaxHp { get; set; }

    public int CurrentHP { get; set; }

    public double CurrentXP { get; set; }

    public Level CurrentLevel { get; set; }

    public int PlayerLevel { get; set; }

    public double Purse { get; set; }

    public double MeleStrenght { get; set; }

    public double RangeStrenght { get; set; }

    public double Toughness { get; set; }

    public List<Weapon> Weapons { get; set; }

    public List<Shield> Shields { get; set; }

    public int MainWeaponId { get; set; }

    public int MainShieldId { get; set; }

    public int SecondaryWeaponId { get; set; }

    public double HpPercent
    {
      get
      {
        return CurrentHP / MaxHp;
      }
    }

    public double XpPercent
    {
      get
      {
        if (CurrentXP <= 0) return 0;
        if (PlayerLevel <= 0) return 0;
        return Math.Round(CurrentXP / PlayerLevel * 100);
      }
    }

  }
}
