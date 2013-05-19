using CS.KTS.Data.Objects;
using CS.KTS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CS.KTS.Data
{
  public class PlayerData : Character
  {
    private Random _rand;
    private TimeSpan _lastGameTime;
    private bool _isDead;
    private List<DpsStats> _dpsStats;
    private bool _gaindNewLevel;

    #region Public props

    public PlayerData(CharacterClass characterClass, string name)
    {
      _rand = new Random();
      _dpsStats = new List<DpsStats>();
      _lastGameTime = new TimeSpan();
      CharacterClass = characterClass;
      Name = name;

      InitNewPlayer();
    }

    public int MaxHp { get; set; }

    public int CurrentHP { get; set; }

    public Exp Exp { get; set; }

    public Level CurrentLevel { get; set; }

    public int PlayerLevel { get; set; }

    public int Gold { get; set; }

    public int Strenght { get; set; }

    public int Accuracy { get; set; }

    public int Defence { get; set; }

    public int Healing { get; set; }

    public List<Weapon> Weapons { get; set; }

    public Weapon EquipedWeapon { get; set; }

    public List<Shield> Shields { get; set; }

    public int MainWeaponId { get; set; }

    public int MainShieldId { get; set; }

    public int SecondaryWeaponId { get; set; }

    public double CriticalChance { get; set; }

    public int HealingCooldown { get; set; }

    public int StunCooldown { get; set; }

    public bool IsDead { get { return _isDead; } }

    public int GetPlayerLowDamage
    {
      get
      {
        return Convert.ToInt32((1 + (double)Accuracy / 10) * EquipedWeapon.MinDamage);
      }
    }

    public int GetPlayerHighDamage
    {
      get
      {
        return Convert.ToInt32((1 + (double)Accuracy / 10) * EquipedWeapon.MaxDamage);
      }
    }

    public int MaxDamage { get; set; }

    public int MaxDps { get; set; }

    public int Killedenemies { get; set; }

    public int UnspentSkillPoints { get; set; }

    public bool IsCasting { get; set; }

    public double TimeToCast { get; set; }

    public List<Ability> Abilities { get; set; }

    public CharacterClass CharacterClass { get; set; }

    #endregion

    #region Public Methods

    public WeaponDamage GetWeaponDamage()
    {
      var weaponDamage = _rand.Next(EquipedWeapon.MinDamage, EquipedWeapon.MaxDamage);
      var damage = Convert.ToInt32((1 + (double)Accuracy / 10) * weaponDamage);
      var isCritical = IsCritical();
      if (isCritical)
      {
        damage += damage;
      }
      if (MaxDamage < damage) MaxDamage = damage;
      return new WeaponDamage { Damage = damage, IsCritical = isCritical };
    }

    public void UpdateStats(List<PlayerLevelUpStat> stats)
    {
      foreach (var stat in stats)
      {
        switch (stat.PlayerStat)
        {
          case PlayerStats.Health:
            MaxHp += stat.Points;
            UnspentSkillPoints--;
            break;
          case PlayerStats.Defence:
            Defence += stat.Points;
            UnspentSkillPoints--;
            break;
          case PlayerStats.Strength:
            Strenght += stat.Points;
            UnspentSkillPoints--;
            break;
          case PlayerStats.Accuracy:
            Accuracy += stat.Points;
            UnspentSkillPoints--;
            break;
          case PlayerStats.CriticalChance:
            CriticalChance += ((double)stat.Points / 100) / 2;
            UnspentSkillPoints--;
            break;
          case PlayerStats.Healing:
            Healing += stat.Points;
            UnspentSkillPoints--;
            break;
          default:
            break;
        }
      }
    }

    public void UpdateExp(int value)
    {
      Exp.Current += value;
      if (Exp.Current >= Exp.Max)
      {
        _gaindNewLevel = true;
        LevelUp();
      }
    }

    public bool GaindNewLevel()
    {
      if (_gaindNewLevel)
      {
        _gaindNewLevel = false;
        return true;
      }
      return false;
    }

    public int Hit(int damage)
    {
      var reducedDamage = DamageReduction(damage);
      CurrentHP -= reducedDamage;
      if (CurrentHP <= 0) _isDead = true;
      return reducedDamage;
    }

    public AbilityResponse UseAbility(AbilityType abilityType, TimeSpan gameTime)
    {
      var abilityToUse = Abilities.FirstOrDefault(a => a.AbilityType == abilityType);
      if (abilityToUse == null) return new AbilityResponse { CouldUse = false };
      var abilityRespnse = abilityToUse.Use(gameTime);

      ApplyAbility(abilityType, abilityRespnse);

      return abilityRespnse;
    }

    public void UpdateCooldowns(TimeSpan totalGameTime)
    {
      if (totalGameTime == null) return;
      if (((totalGameTime - _lastGameTime).Seconds >= 1))
      {
        Abilities.ForEach(a => a.UpdateCooldown(totalGameTime));
      }
    }

    public void UpdateDpsStats(TimeSpan timestamp)
    {
      for (var i = _dpsStats.Count - 1; i >= 0; i--)
      {
        if (_dpsStats[i].TimeStamp.TotalSeconds < (timestamp.TotalSeconds - 10))
        {
          _dpsStats.RemoveAt(i);
        }
      }
    }

    public void AddDpsStats(int damage, TimeSpan timestamp)
    {
      _dpsStats.Add(new DpsStats { Damage = damage, TimeStamp = timestamp });
    }

    public int GetCurrentDps()
    {
      var totalDamage = 0;
      foreach (var dpsStat in _dpsStats)
      {
        totalDamage += dpsStat.Damage;
      }
      if (totalDamage == 0) return 0;
      var dps = (int)totalDamage / 10;
      if (MaxDps < dps) MaxDps = dps;
      return dps;
    }

    public void ChangeWeapon(string weaponName)
    {
      EquipedWeapon = Weapons.First(w => w.Name == weaponName);
    }

    public void AddLoot(Loot loot)
    {
      switch (loot.LootType)
      {
        case LootType.Gold:
          Gold += loot.Gold;
          break;
        case LootType.Weapon:
          Weapons.Add(loot.Weapon);
          break;
        case LootType.Shiled:
          break;
        case LootType.None:
          break;
        default:
          break;
      }

    }

    #endregion

    #region Private Methods

    private void ApplyAbility(AbilityType abilityType, AbilityResponse abilityResponse)
    {
      if (!abilityResponse.CouldUse) return;
      switch (abilityType)
      {
        case AbilityType.Healing:
          var newHp = CurrentHP + abilityResponse.Power;
          if (newHp > MaxHp) CurrentHP = MaxHp;
          else CurrentHP = newHp;
          break;
        case AbilityType.Stun:
          break;
        case AbilityType.AoeStun:
          break;
        case AbilityType.DamageBoost:
          break;
        case AbilityType.HealingBoost:
          break;
        case AbilityType.DefenceBoost:
          break;
        case AbilityType.Damage:
          break;
        case AbilityType.AoeDamage:
          break;
        case AbilityType.Beem:
          break;
        default:
          break;
      }
    }

    private int DamageReduction(int damage)
    {
      double defenceFact = 1 - ((double)Defence / 100);
      return Convert.ToInt32(defenceFact * damage);
    }

    private bool IsCritical()
    {
      return _rand.NextDouble() <= CriticalChance;
    }

    private void LevelUp()
    {
      PlayerLevel++;
      Exp.Current = 0;
      Exp.Max = Exp.Max * 2;
      MaxHp = Convert.ToInt32(MaxHp * 1.2);
      CurrentHP = MaxHp;
      UnspentSkillPoints += 5;
    }

    private void InitNewPlayer()
    {
      CriticalChance = 0.1;
      CurrentLevel = new Data.Level();
      PlayerLevel = 1;
      Id = 1;
      IsGood = true;
      MainShieldId = 1;
      MainWeaponId = 1;
      Strenght = 5;
      Gold = 0;
      Accuracy = 5;
      SecondaryWeaponId = 1;
      TilesRef = "playerChar";
      Defence = 5;
      HealingCooldown = 0;
      UnspentSkillPoints = 0;
      Exp = new Exp { Current = 0, Max = 100 };
      Abilities = CreateAbilities();
      Shields = new List<Shield>();
      Weapons = CreateBasicWeapons();
      EquipedWeapon = Weapons.First();

      switch (CharacterClass)
      {
        case CharacterClass.Wizard:
          CurrentHP = 120;
          MaxHp = 120;
          Healing = 10;
          break;
        case CharacterClass.Hunter:
          CurrentHP = 180;
          MaxHp = 180;
          Healing = 5;
          Accuracy = 10;
          break;
        case CharacterClass.Warrior:
          break;
        default:
          break;
      }


    }

    private List<Weapon> CreateBasicWeapons()
    {
      var weapons = new List<Weapon>();
      switch (CharacterClass)
      {
        case CharacterClass.Wizard:
          weapons.Add(new Weapon()
          {
            MinDamage = 30,
            MaxDamage = 45,
            Desc = "Magic Staff",
            Id = 1,
            Name = "Basic Staff",
            TilesRef = "",
            FireRate = 1000,
            Distance = 900,
            Speed = 600,
            CastTime = 1500,
            Effect = ProjectileEffect.Slow
          });
          break;
        case CharacterClass.Hunter:
          weapons.Add(new Weapon()
          {
            MinDamage = 10,
            MaxDamage = 20,
            Desc = "Ranged weapon",
            Id = 1,
            Name = "Basic rifle",
            TilesRef = "",
            FireRate = 333,
            Distance = 600,
            Speed = 400,
            Effect = ProjectileEffect.None
          });
         
          break;
        case CharacterClass.Warrior:

          break;
        default:
          break;
      }

      return weapons;
    }

    private List<Ability> CreateAbilities()
    {
      var abilities = new List<Ability>();
      abilities.Add(new Ability(10, 10, 20, AbilityType.Healing, ""));
      abilities.Add(new Ability(10, 50, 100, AbilityType.Beem, "fire2"));
      return abilities;

    }
    #endregion

  }
}
