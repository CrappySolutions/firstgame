using CS.KTS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Entities
{
  public class Loot
  {
    public LootType LootType { get; set; }

    public int Gold { get; set; }

    public Weapon Weapon { get; set; }
  }
}
