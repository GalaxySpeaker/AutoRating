using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRating.AdofaiRater.Objects
{
    public class AdofaiLevel
    {

        public List<AdofaiTile> Tiles { get; set; } = new List<AdofaiTile>();
        public long TotalLength { get; set; }
        public double StartBpm { get; set; }
        public JObject LinearLevelJson { get; set; }

    }
}
