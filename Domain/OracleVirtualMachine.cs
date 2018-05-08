using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class OracleVirtualMachine
    {
        public int OracleVirtualMachineId { get; set; }
        public string Naam { get; set; }
        public string OvmId { get; set; }
        public int KlantId { get; set; }
    }
}
