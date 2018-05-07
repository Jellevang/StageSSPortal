using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ISSHRepository
    {
        OracleVirtualMachine AddMachine(OracleVirtualMachine ovm);
        void DeleteMachine(OracleVirtualMachine ovm);
        OracleVirtualMachine GetMachine(int id);
        IEnumerable<OracleVirtualMachine> ReadMachines();
        OracleVirtualMachine GetMachine(string naam);
        IEnumerable<OracleVirtualMachine> GetKlantMachines(int klantid);
        void UpdateMachine(OracleVirtualMachine ovm);
        OVMLijst AddLijst(OVMLijst lijst);
        void DeleteLijst(OVMLijst lijst);
        OVMLijst GetLijst(int id);
        IEnumerable<OVMLijst> ReadLijsten();
    }
}
