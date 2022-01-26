using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepDungeonDex.Data
{
    public class MobData
    {
        public class Vulnerabilities
        {
            public bool? CanStun { get; set; }
            public bool? CanHeavy { get; set; }
            public bool? CanSlow { get; set; }
            public bool? CanSleep { get; set; }
            public bool? CanBind { get; set; }
            public bool? IsUndead { get; set; }
        }
        public Vulnerabilities Vuln { get; set; }

        public string MobNotes { get; set; }

        public enum ThreatLevel
        {
            Unspecified,
            Easy,
            Caution,
            Dangerous,
            Vicious
        }
        public ThreatLevel Threat { get; set; }

        public enum AggroType
        {
            Unspecified,
            Sight,
            Sound,
            Proximity,
            Boss
        }
        public AggroType Aggro { get; set; }

    }
}
