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
        OracleVirtualMachine GetMachineByOvmId(string OvmId);
        IEnumerable<OracleVirtualMachine> ReadMachines();
        OracleVirtualMachine GetMachine(string naam);
        OracleVirtualMachine GetMachineById(string id);
        IEnumerable<OracleVirtualMachine> GetKlantMachines(int klantid);
        void UpdateMachine(OracleVirtualMachine ovm);
        OVMLijst AddLijst(OVMLijst lijst);
        void DeleteLijst(OVMLijst lijst);
        OVMLijst GetLijst(int id);
        OVMLijst GetLijst(int klantid, string ovmid);
        IEnumerable<OVMLijst> ReadLijsten();
    }
}
