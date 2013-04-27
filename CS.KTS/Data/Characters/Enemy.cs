using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data
{
  public class Enemy : Character
  {
    public double Damage { get; set; }

    public double HP { get; set; }

    public double XPValue { get; set; }

    public double GoldValue { get; set; }

    public int MinCityLevel { get; set; }

    public int? MaxCityLevel { get; set; }

    public Weapon MainWeapon { get; set; }
  }
}
