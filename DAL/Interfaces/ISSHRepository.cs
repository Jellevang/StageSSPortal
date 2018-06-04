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
        IEnumerable<OVMLijst> GetLijstAccount(int klantid);
        IEnumerable<OVMLijst> ReadLijsten();
        void DeleteLijstenAccount(int klantid);
        IEnumerable<OVMLijst> GetLijstOVM(string id);
        IEnumerable<OracleVirtualMachine> ReadMachinesByServerId(int id);
        void DeleteLijstenOvm(string id);
        Server GetServer(int id);
        Server GetServer(string id);
        Server AddServer(Server server);
        List<Server> ReadServers();
        void DeleteServer(string id);
        LogLijst CreateLogLijst(LogLijst log);
        void RemoveLogLijstenKlant(int gebruikersId);
        void RemoveLogLijstenOVM(string id);
        IEnumerable<LogLijst> ReadLogLijstKlant(int gebruikersid);
        IEnumerable<LogLijst> ReadLogLijstOVM(string id);
        IEnumerable<LogLijst> ReadLogLijsten();
        List<ScheduledDownTime> ReadScheduledDTByOvm(string OvmId);
        List<ScheduledDownTime> ReadScheduledDT();
        ScheduledDownTime ReadScheduledDTById(int SDTid);
        ScheduledDownTime CreateScheduledDT(ScheduledDownTime SDT);
        void DeleteScheduledDT(ScheduledDownTime SDT);

    }
}
