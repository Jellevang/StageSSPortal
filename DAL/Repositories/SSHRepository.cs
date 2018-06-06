using DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using Domain;
using System.Data.Entity;

namespace DAL.Repositories
{
    public class SSHRepository : ISSHRepository
    {
        private readonly StageSSPortalDbContext ctx;

        public SSHRepository()
        {
            ctx = new StageSSPortalDbContext();
        }
        //Deze Methode voegt een Oracle Virtueel Machine toe aan de databank.
        public OracleVirtualMachine AddMachine(OracleVirtualMachine ovm)
        {
            ctx.OracleVirtualMachines.Add(ovm);
            ctx.SaveChanges();
            return ovm;
        }
        //Deze Methode verwijdert een Oracle Virtueel Machine uit de databank.
        public void DeleteMachine(OracleVirtualMachine ovm)
        {
            ctx.OracleVirtualMachines.Remove(ovm);
            ctx.SaveChanges();
        }
        //Deze Methode haalt een Oracle Virtueel Machine uit de databank op aan de hand van een id.
        public OracleVirtualMachine GetMachine(int id)
        {
            OracleVirtualMachine ovm = ctx.OracleVirtualMachines.Find(id);
            return ovm;
        }
        //Deze Methode haalt alle Oracle Virtueel Machines uit de databank.
        public IEnumerable<OracleVirtualMachine> ReadMachines()
        {
            return ctx.OracleVirtualMachines.AsEnumerable();
        }
        //Deze Methode haalt een Oracle Virtueel Machine uit de databank op aan de hand van de Oracle Virtueel Machine id.
        public OracleVirtualMachine GetMachineById(string id)
        {
            OracleVirtualMachine ovm = ctx.OracleVirtualMachines.Where(o => o.OvmId.Contains(id)).FirstOrDefault();
            return ovm;
        }
        //Deze methode haalt alle Oracle Virtueel Machines op die gelinkt zijn aan een bepaalde klant.
        public IEnumerable<OracleVirtualMachine> GetKlantMachines(int klantid)
        {
            IEnumerable<OracleVirtualMachine> ovms = ctx.OracleVirtualMachines.Where(o => o.KlantId == klantid);
            return ovms;
        }
        //Deze methode updatet een Oracle Virtueel Machine.
        public void UpdateMachine(OracleVirtualMachine ovm)
        {
            OracleVirtualMachine ovM = ctx.OracleVirtualMachines.Find(ovm.OracleVirtualMachineId);
            ctx.Entry(ovM).CurrentValues.SetValues(ovm);
            ctx.Entry(ovM).State = System.Data.Entity.EntityState.Modified;
            ctx.SaveChanges();
        }
        //Deze methode voegt een OVMLijst toe aan de databank.
        //In OVMlijsten wordt er bijgehouden welke klantaccounts(medewerkers) aan welke Oracle Virtueel Machines hangen.
        public OVMLijst AddLijst(OVMLijst lijst)
        {
            ctx.OVMLijsten.Add(lijst);
            ctx.SaveChanges();
            return lijst;
        }
        //Deze methode verwijdert een OVMLijst uit de databank.
        public void DeleteLijst(OVMLijst lijst)
        {
            ctx.OVMLijsten.Remove(lijst);
            ctx.SaveChanges();
        }
        //Deze methode haalt een OVMLijst op uit de databank aan de hand van een id.
        public OVMLijst GetLijst(int id)
        {
            OVMLijst ovm = ctx.OVMLijsten.Find(id);
            return ovm;
        }
        //Deze methode haalt een OVMLijst op uit de databank aan de hand van een Oracle vitueel machine id en een klantid.
        public OVMLijst GetLijst(int klantid, string ovmid)
        {
            IEnumerable<OVMLijst> ovmsL = ctx.OVMLijsten.Where(o => o.OVMId.Contains(ovmid));
            OVMLijst lijst = ovmsL.Where(l => l.AccountId == klantid).FirstOrDefault();
            return lijst;
        }
        //Deze methode haalt alle OVMLijsten op uit de databank van een bepaalde klant aan de hand van het klantid.
        public IEnumerable<OVMLijst> GetLijstAccount(int klantid)
        {
            IEnumerable<OVMLijst> ovmsL = ctx.OVMLijsten.Where(o => o.AccountId == klantid);
            return ovmsL;
        }
        //Deze methode verwijdert alle OVMLijsten van een bepaalde klant aan de hand van het klantid.
        public void DeleteLijstenAccount(int klantid)
        {
            IEnumerable<OVMLijst> ovmsL = ctx.OVMLijsten.Where(o => o.AccountId == klantid);
            foreach (OVMLijst lijst in ovmsL)
            {
                ctx.OVMLijsten.Remove(lijst);
            }
            ctx.SaveChanges();
        }
        //Deze methode haalt een OVMLijst op uit de databank aan de hand van een Oracle vitueel machine id
        public IEnumerable<OVMLijst> GetLijstOVM(string id)
        {
            IEnumerable<OVMLijst> ovmsL = ctx.OVMLijsten.Where(o => o.OVMId.Contains(id));
            return ovmsL;
        }
        //Deze methode verwijdert alle OVMLijsten aan de hand van een Oracle vitueel machine id.
        public void DeleteLijstenOvm(string id)
        {
            IEnumerable<OVMLijst> ovmsL = ctx.OVMLijsten.Where(o => o.OVMId.Contains(id));
            foreach (OVMLijst lijst in ovmsL)
            {
                ctx.OVMLijsten.Remove(lijst);
            }
            ctx.SaveChanges();
        }
        //Deze methode haalt alle Oracle Vitruele machines op aan de hand van hun server.
        public IEnumerable<OracleVirtualMachine> ReadMachinesByServerId(int id)
        {
            return ctx.OracleVirtualMachines.Where(vm => vm.ServerId == id);
        }
        //Deze methode haalt een server op aan de hand van een id uit de databank.
        public Server GetServer(int id)
        {
            return ctx.Servers.Where(s => s.Id == id).FirstOrDefault();
        }
        //Deze methode haalt een server op aan de hand van het id van de server zelf uit de databank.
        public Server GetServer(string id)
        {
            return ctx.Servers.Where(s => s.ServersId.Contains(id)).FirstOrDefault();
        }
        //Deze methode voegt een server toe aan de databank.
        public Server AddServer(Server server)
        {
            ctx.Servers.Add(server);
            ctx.SaveChanges();
            return server;
        }
        //Deze methode haalt alle servers op uit de databank.
        public List<Server> ReadServers()
        {
            return ctx.Servers.ToList();
        }
        //Deze methode verwijdert een server uit de databank.
        public void DeleteServer(string id)
        {
            ctx.Servers.Remove(GetServer(id));
        }
        //Deze methode maakt een loglijst aan in de databank.
        //Een loglijst is een audit.
        public LogLijst CreateLogLijst(LogLijst log)
        {
            ctx.LogLijsten.Add(log);
            ctx.SaveChanges();
            return log;
        }
        //Deze methode verwijdert alle loglijsten van een bepaalde klant uit de databank.
        public void RemoveLogLijstenKlant(int gebruikersId)
        {
            IEnumerable<LogLijst> logs = ctx.LogLijsten.Where(l => l.GebruikerId ==gebruikersId);
            foreach (LogLijst lijst in logs)
            {
                ctx.LogLijsten.Remove(lijst);
            }
            ctx.SaveChanges();
        }
        //Deze methode verwijdert alle loglijsten van een bepaalde Oracle Virtuele machine uit de databank.
        public void RemoveLogLijstenOVM(string id)
        {
            IEnumerable<LogLijst> logs = ctx.LogLijsten.Where(l => l.OvmId.Contains(id));
            foreach (LogLijst lijst in logs)
            {
                ctx.LogLijsten.Remove(lijst);
            }
            ctx.SaveChanges();
        }
        //Deze methode haalt alle loglijsten op uit de databank van een bepaalde klant.
        public IEnumerable<LogLijst> ReadLogLijstKlant(int gebruikersid)
        {
            IEnumerable<LogLijst> logs = ctx.LogLijsten.Where(l => l.GebruikerId == gebruikersid);
            return logs;
        }
        //Deze methode haalt alle loglijsten op uit de databank van een bepaalde Oracle Virtueel Machine.
        public IEnumerable<LogLijst> ReadLogLijstOVM(string id)
        {
            IEnumerable<LogLijst> logs = ctx.LogLijsten.Where(l => l.OvmId.Contains(id));
            return logs;
        }
        //Deze methode haalt alle loglijsten op uit de databank.
        public IEnumerable<LogLijst> ReadLogLijsten()
        {
            return ctx.LogLijsten.AsEnumerable();
        }
        //Deze methode haalt het Scheduled Downtime op uit de databank van een bepaalde Oracle Virtueel Machine
        //Dit gebeurt aan de hand van het Oracle Virtueel machine Id
        public List<ScheduledDownTime> ReadScheduledDTByOvm(string OvmId)
        {
            return ctx.ScheduleTDLijst.Where(s => s.OvmId.Contains(OvmId)).ToList();
        }
        //Deze methode haalt een Scheduled Downtime op uit de databank.
        public ScheduledDownTime ReadScheduledDTById(int SDTid)
        {
            return ctx.ScheduleTDLijst.Where(s => s.id == SDTid).FirstOrDefault() ;
        }
        //Deze methode haalt alle Scheduled Downtime op uit de databank.
        public List<ScheduledDownTime> ReadScheduledDT()
        {
            return ctx.ScheduleTDLijst.ToList();
        }
        //Deze methode voegt een Scheduled Downtime toe aan de databank.
        public ScheduledDownTime CreateScheduledDT(ScheduledDownTime SDT)
        {
            ctx.ScheduleTDLijst.Add(SDT);
            ctx.SaveChanges();
            return SDT;
        }
        //Deze methode verwijdert een Scheduled Downtime uit de databank.
        public void DeleteScheduledDT(ScheduledDownTime SDT)
        {
            ctx.ScheduleTDLijst.Remove(ReadScheduledDTById(SDT.id));
            ctx.SaveChanges();
        }
    }
}
