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

    public int X { get; set; }

    public int Y { get; set; }

    public MessageType MessageType { get; set; }
    
  }

  public enum MessageType
  { 
    PlayerDamageDone,
    TargetHp,
    PlayerHp,
    PlayerExp,
    PlayerLevel
  }
}
