using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.KTS.Data
{
  public class Boss : EnemyData
  {
    public List<Artifact> Artifacts { get; set; }

    public int MainArtifactId { get; set; }

  }
}
