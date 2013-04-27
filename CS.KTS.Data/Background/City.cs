using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data
{
  public class City
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string TileRef { get; set; }

    public int CountryId { get; set; }

    public List<Character> Characters { get; set; }
  }
}
