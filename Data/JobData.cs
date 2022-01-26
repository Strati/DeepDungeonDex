using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepDungeonDex.Data
{
    public class JobData
    {
        public string Name { get; set; }

        public bool CanStun { get; set; }
        public bool CanSleep { get; set; }
        public bool CanBind { get; set; }
        public bool CanHeavy { get; set; }
        public bool CanSlow { get; set; }
    }
}
