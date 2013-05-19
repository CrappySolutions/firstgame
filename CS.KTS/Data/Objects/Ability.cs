using CS.KTS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data.Objects
{
  public class Ability
  {
    private int _cooldown;
    private int _currentCooldown;
    private int _minPower;
    private int _maxPower;
    private Random _rand;
    private TimeSpan _lastUseTime;
    private TimeSpan _duration;

    public Ability(int cooldown, int minPower, int maxPower, AbilityType abilityType, string textureName)
    {
      _cooldown = cooldown;
      _minPower = minPower;
      _maxPower = maxPower;
      TextureName = textureName;
      AbilityType = abilityType;
      _rand = new Random();
    }

    public AbilityResponse Use(TimeSpan totalGameTime)
    {
      if (_currentCooldown > 0) return new AbilityResponse { CouldUse = false };
      _currentCooldown = _cooldown;
      _lastUseTime = totalGameTime;
      Send = true;
      Power = _rand.Next(_minPower, _maxPower);
      return new AbilityResponse { CouldUse = true, Power = Power };
    }

    public void Update(int minPower = -1, int maxPower = -1, int cooldown = -1)
    {
      if (minPower != -1) _minPower = minPower;
      if (maxPower != -1) maxPower = _maxPower;
      if (cooldown != -1) cooldown = _cooldown;
    }

    public void UpdateCooldown(TimeSpan totalGameTime)
    {
      if (!IsOnCooldown) return;
      if (((totalGameTime - _lastUseTime).Seconds >= 1))
      {
        _currentCooldown--;
        _lastUseTime = totalGameTime;
      }
    }

    public string TextureName { get; private set; }

    public string Speed { get; set; }

    public AbilityType AbilityType { get; set; }

    public bool Send { get; set; }

    public int Power { get; set; }

    public int CurrentCooldown { get { return _currentCooldown; } }

    public bool IsOnCooldown { get { return _currentCooldown >= 0; } }

    public bool IsProjectile { get { return AbilityType == Entities.AbilityType.Beem; } }
  }
}
