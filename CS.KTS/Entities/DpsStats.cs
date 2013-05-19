using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Entities
{
  public class DpsStats
  {
    public int Damage { get; set; }

    public TimeSpan TimeStamp { get; set; }

    public bool Remove { get; set; }
  }
}
