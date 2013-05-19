using CS.KTS.Entities;
using CS.KTS.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data.Objects
{
  public class LootData
  {
    public Loot GetLoot()
    {
      return DropHelper.GenerateLoot(Level, DropRate, Gold);
    }

    public double DropRate { get; set; }

    public int Gold { get; set; }

    public int Level { get; set; }
  }
}
