using System;
using System.Collections.Generic;
using System.Text;

namespace OMItemRestrictions.Models
{
    [Serializable]
    public class BlacklistData
    {
        public Dictionary<string, List<int>> Blacklist { get; set; }
    }
}
