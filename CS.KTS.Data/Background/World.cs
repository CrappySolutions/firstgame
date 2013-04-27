using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data
{
  public class World
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string TilesRef { get; set; }

    public List<Country> Countries { get; set; }
  }
}
