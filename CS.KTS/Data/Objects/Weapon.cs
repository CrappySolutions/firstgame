using CS.KTS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data
{
  public class Weapon : Artifact
  {
    private Random _rand;

    public Weapon()
    {
      _rand = new Random();
    }

    public int MaxDamage { get; set; }

    public int MinDamage { get; set; }

    public int FireRate { get; set; }

    public int Speed { get; set; }

    public int Distance { get; set; }

    public int CastTime { get; set; }

    public ProjectileEffect Effect { get; set; }

    public int GetWeaponDamage()
    {
      return _rand.Next(MinDamage, MaxDamage);
    }
  }
}
