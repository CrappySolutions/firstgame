using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data
{
  public class EnemyData : Character
  {
    private Random _rand;

    public EnemyData(int level)
    {
      _rand = new Random();
      SetBasicEnemyValues(level);
    }
    #region Public props
    public int Damage { get; set; }

    public int MaxHp { get; set; }

    public int CurrentHp { get; set; }

    public int XPValue { get; set; }

    public int GoldValue { get; set; }

    public int MinCityLevel { get; set; }

    public int? MaxCityLevel { get; set; }

    public Weapon MainWeapon { get; set; }

    public int Level { get; set; }

    public int Speed { get; set; }

    private int _previousSpeed { get; set; }

    public double DropRate { get; set; }

    #endregion

    #region Public Methods

    public bool DoDrop()
    {
      var fact = _rand.NextDouble();
      return fact <= DropRate;
      
    }

    public void ChangeSpeed(int newSpeed)
    {
      _previousSpeed = Speed;
      Speed = newSpeed;
    }

    public void ResetSpeed()
    {
      if (_previousSpeed == 0) Speed = 50;
      else Speed = _previousSpeed;
    }

    #endregion

    private void SetBasicEnemyValues(int level)
    { 
      Level = level;
        CurrentHp = 100 * level;
        Damage = 10 * level;
        GoldValue = 1;
        Id = 1;
        IsGood = false;
        MainWeapon = new Data.Weapon();
        MaxCityLevel = 1;
        MinCityLevel = 1;
        Name = "Nisse";
        MaxHp = 100 * level;
        TilesRef = "nisse2";
        XPValue = 10 * level;
        Speed = 50;
        DropRate = 0.2;
    }
  }
}
