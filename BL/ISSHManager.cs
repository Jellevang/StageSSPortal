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
        OracleVirtualMachine AddOVM(string naam, string ovmId, int klantId, int serverId);
        void RemoveOVM(string id);
        OracleVirtualMachine GetOVM(int id);
        OracleVirtualMachine GetOVM(string naam);
        OracleVirtualMachine GetOVMById(string id);
        IEnumerable<OracleVirtualMachine> GetKlantOVMs(int klantid);
        IEnumerable<OracleVirtualMachine> GetOVMs();
        IEnumerable<OracleVirtualMachine> GetOVMsByServer(int id);

        void ChangeOVM(OracleVirtualMachine ovm);
        OVMLijst AddLijst(string ovmid, int klantid);
        OVMLijst GetLijst(int klantid, string ovmid);
        void RemoveLijst(OVMLijst ovmlijst);
        IEnumerable<OVMLijst> GetLijstAccount(int klantid);
        void RemoveLijstenAccount(int klantid);
        IEnumerable<OVMLijst> GetLijstOvm(string id);
        void RemoveLijstenOvm(string id);
        Server GetServer(int id);
        Server GetServer(string id);
        void RemoveServer(string id);
        Server AddServer(string naam, string id);
        List<Server> GetServers();
        LogLijst AddLogLijst(string naam, int gebruikersid, string ovmid);
        void DeleteLogLijstenKlant(int gebruikerid);
        void DeleteLogLijstenOVM(string id);
        IEnumerable<LogLijst> GetLogLijstsKlant(int id);
        IEnumerable<LogLijst> GetLogLijstsOVM(string id);
        IEnumerable<LogLijst> GetLogLijsten();

        //SCHEDULED DOWNTIME
        //READ
        List<ScheduledDownTime> GetScheduledDTByOvm(string OvmId);
        List<ScheduledDownTime> GetScheduledDT();
        ScheduledDownTime GetScheduledDTById(int SDTid);
        //ADD
        ScheduledDownTime AddScheduledDT(string ovmId, DateTime start, DateTime eind);
        //REMOVE
        void RemoveScheduledDT(ScheduledDownTime SDT);
    }
}
