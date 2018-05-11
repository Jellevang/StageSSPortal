using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace BL
{
    public interface ISSHManager
    {
        OracleVirtualMachine AddOVM(string naam, string ovmId, int klantId);
        void RemoveOVM(int id);
        OracleVirtualMachine GetOVM(int id);
        OracleVirtualMachine GetOVM(string naam);
        OracleVirtualMachine GetOVMById(string id);
        IEnumerable<OracleVirtualMachine> GetKlantOVMs(int klantid);
        IEnumerable<OracleVirtualMachine> GetOVMs();
        void ChangeOVM(OracleVirtualMachine ovm);
        OVMLijst AddLijst(int ovmid, int klantid);

    }
}
