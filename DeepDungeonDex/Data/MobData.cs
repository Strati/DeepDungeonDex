using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepDungeonDex.Data
{
    public class MobData : IRepoData<MobData>
    {
        public class Vulnerabilities
        {
            public bool? CanStun;
            public bool? CanHeavy;
            public bool? CanSlow;
            public bool? CanSleep;
            public bool? CanBind;
            public bool? IsUndead;

            public Vulnerabilities() { }
            public Vulnerabilities(Vulnerabilities vuln)
            {
                CanStun = vuln.CanStun;
                CanHeavy = vuln.CanHeavy;
                CanSlow = vuln.CanSlow;
                CanSleep = vuln.CanSleep;
                CanBind = vuln.CanBind;
                IsUndead = vuln.IsUndead;
            }
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

        public MobData() { }

        public MobData Clone()
        {
            return new MobData()
            {
                Vuln = new Vulnerabilities(Vuln),
                MobNotes = MobNotes,
                Threat = Threat,
                Aggro = Aggro
            };
        }
    }
}
