using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data
{
  public class EnemyData : Character
  {
    public int Damage { get; set; }

    public int MaxHp { get; set; }

    public int CurrentHp { get; set; }

    public int XPValue { get; set; }

    public double GoldValue { get; set; }

    public int MinCityLevel { get; set; }

    public int? MaxCityLevel { get; set; }

    public Weapon MainWeapon { get; set; }
  }
}
