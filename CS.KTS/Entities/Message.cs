using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Entities
{
  public class Message
  {
    public string Text { get; set; }

    public double Number { get; set; }

    public int Number2 { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public MessageType MessageType { get; set; }

    public bool IsCritical { get; set; }
  }

  public enum MessageType
  { 
    PlayerDamageDone,
    EnemyDamageDone,
    PlayerHealing,
    TargetHp,
    PlayerHp,
    PlayerExp,
    PlayerLevel,
    PlayerXpPercent,
    PlayerHpPercent,
    InitPlayerMaxHp,
    InitPlayerMaxXp,
    PlayerHealingCooldown,
    PlayerDps,
    PlayerGold,
    PlayerStun,
    PlayerStunCooldown,
    PlayerBeemCooldown,
    BigMessage
  }
}
