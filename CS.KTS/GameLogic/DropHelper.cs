using CS.KTS.Data;
using CS.KTS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.GameLogic
{
  public static class DropHelper
  {
    private static Random _rand = new Random();

    public static Loot GenerateLoot(int level, double dropRate, int gold)
    {
      var fact = _rand.NextDouble();
      var doDrop = fact <= dropRate;

      if (fact <= (dropRate / 2))
      {
        return new Loot { Weapon = CreateWeapon(level), LootType = LootType.Weapon };
      }
      else
      {
        return new Loot { Gold = gold, LootType = LootType.Gold };
      }
    }

    private static Weapon CreateWeapon(int level)
    {
      return new Weapon
      {
        Desc = "",
        Distance = GetDistance(),
        FireRate = GetFireRate(),
        Id = 1,
        MaxDamage = GetMaxDamage(level),
        MinDamage = GetMinDamage(level),
        Name = GenerateName(),
        Speed = GetSpeed(),
        TilesRef = ""
      };
    }

    private static int GetMinDamage(int level)
    {
      var levelBaseDamage = level * 10;
      var fact = _rand.NextDouble();
      return Convert.ToInt32((double)levelBaseDamage * (1 + fact));
    }

    private static int GetMaxDamage(int level)
    {
      var levelBaseDamage = level * 10 * 2;
      var fact = _rand.NextDouble();
      return Convert.ToInt32((double)levelBaseDamage * (1 + fact));
    }

    private static int GetFireRate()
    {
      return _rand.Next(200, 1000);
    }

    private static int GetDistance()
    {
      return _rand.Next(200, 1000);
    }

    private static int GetSpeed()
    {
      return _rand.Next(200, 1000);
    }

    private static string GenerateName()
    {
      int num = _rand.Next(0, 26); // Zero to 25
      char let1 = (char)('a' + num);
      num = _rand.Next(0, 26); // Zero to 25
      char let2 = (char)('a' + num);

      var number = _rand.Next(100, 1000);

      return let1.ToString().ToUpper() + let2.ToString().ToUpper() + number.ToString();
    }
  }
}
