using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data
{
  public class Player : Character
  {

    public double HP { get; set; }

    public double XP { get; set; }

    public Level CurrentLevel { get; set; }

    public double Purse { get; set; }

    public double MeleStrenght { get; set; }

    public double RangeStrenght { get; set; }

    public double Toughness { get; set; }

    public List<Weapon> Weapons { get; set; }

    public List<Shield> Shields { get; set; }

    public int MainWeaponId { get; set; }

    public int MainShieldId { get; set; }

    public int SecondaryWeaponId { get; set; }

  }
}
