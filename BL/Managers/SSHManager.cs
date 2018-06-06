using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Repositories;
using Domain;

namespace BL
{
    public class SSHManager : ISSHManager
    {
        public readonly ISSHRepository repo;
        public SSHManager()
        {
            repo = new SSHRepository();
        }
        //Voegt een Oracle Virtual Machine toe.
        public OracleVirtualMachine AddOVM(string naam, string ovmId, int klantId, int serverId)
        {
            //Hier wordt een nieuwe Oracle Virtueel Machine aangemaakt.
            OracleVirtualMachine ovm = new OracleVirtualMachine()
            {
                Naam = naam,
                OvmId = ovmId,
                KlantId = klantId,
                ServerId=serverId                
            };
            //deze wordt hier aangemaakt in onze SSHRepsoitory.
            OracleVirtualMachine created = repo.AddMachine(ovm);
            return created;
        }
        //Deze methode haalt alle Oracle Virtuele Machines op van een bepaalde server.
        public IEnumerable<OracleVirtualMachine> GetOVMsByServer(int id)
        {
            return repo.ReadMachinesByServerId(id);
        }
        //Deze methode verwijderd een Oracle Virtueel Machine.
        public void RemoveOVM(string id)
        {
            OracleVirtualMachine ovm = repo.GetMachineById(id);
            repo.DeleteMachine(ovm);
        }
        //Deze methode haalt een Oracle Virtueel Machine op.
        public OracleVirtualMachine GetOVM(int id)
        {
            return repo.GetMachine(id);
        }
        //Deze methode haalt een Oracle Virtueel Machine op aan de hand van de OVMID.
        public OracleVirtualMachine GetOVMById(string id)
        {
            return repo.GetMachineById(id);
        }
        //Deze methode haalt alle Oracle Virtueel Machines op van een klant.
        public IEnumerable<OracleVirtualMachine> GetKlantOVMs(int klantid)
        {
            return repo.GetKlantMachines(klantid);
        }
        //Deze methode haalt alle Oracle Virtueel Machines op.
        public IEnumerable<OracleVirtualMachine> GetOVMs()
        {
            return repo.ReadMachines();
        }
        //Deze methode verandert een Oracle Virtueel machine
        public void ChangeOVM(OracleVirtualMachine ovm)
        {
            repo.UpdateMachine(ovm);
        }
        //Deze Methode voegt een nieuwe OVMLijst toe.
        public OVMLijst AddLijst(string ovmid, int klantid)
        {
            //hier wordt een nieuwe OVMLijst gemaakt.
            OVMLijst ovm = new OVMLijst()
            {
                AccountId = klantid,
                OVMId = ovmid
            };
            //deze wordt hier aangemaakt in onze SSHRepsoitory.
            OVMLijst created = repo.AddLijst(ovm);
            return created;
        }
        //Deze methode haalt een OVMLijst op.
        public OVMLijst GetLijst(int klantid, string ovmid)
        {
            OVMLijst lijst = repo.GetLijst(klantid, ovmid);
            return lijst;
        }
        //Deze methode haalt alle OVMLijsten op van een bepaalde KlantAccount.
        public IEnumerable<OVMLijst> GetLijstAccount(int klantid)
        {
            IEnumerable<OVMLijst> lijsten = repo.GetLijstAccount(klantid);
            return lijsten;
        }
        //Deze methode verwijdet alle OVMLijsten van een bepaalde KlantAccount.
        public void RemoveLijstenAccount(int klantid)
        {
            repo.DeleteLijstenAccount(klantid);
        }
        //Deze methode verwijdet een OVMLijst.
        public void RemoveLijst(OVMLijst ovmlijst)
        {
            repo.DeleteLijst(ovmlijst);
        }
        //Deze methode haalt alle OVMLijsten op van een bepaalde Oracle Virtueel Machine.
        public IEnumerable<OVMLijst> GetLijstOvm(string id)
        {
            IEnumerable<OVMLijst> lijsten = repo.GetLijstOVM(id);
            return lijsten;
        }
        //Deze methode verwijdert alle OVMLijsten van een bepaalde Oracle Virtueel Machine.
        public void RemoveLijstenOvm(string id)
        {
            repo.DeleteLijstenOvm(id);
        }
        //Deze methode haalt een server op.
        public Server GetServer(int id)
        {
            return repo.GetServer(id);
        }
        //Deze methode haalt een server op aan de hand van een ServerId.
        public Server GetServer(string id)
        {
            return repo.GetServer(id);
        }
        //Deze methode voegt een server toe.
        public Server AddServer(string naam, string id)
        {
            //Hier wordt een nieuwe server gemaakt.
            Server server = new Server()
            {
                ServerNaam = naam,
                ServersId = id
            };
            //Deze wordt toegevoegd in onze SSHRepository.
            repo.AddServer(server);
            return server;
        }
        //Deze methode verwijdert een server.
        public void RemoveServer(string id)
        {
            repo.DeleteServer(id);
        }
        //Deze methode haalt alle servers op.
        public List<Server> GetServers()
        {
            return repo.ReadServers();
        }
        //Deze methode voegt een LogLijst(Audit) toe.
        public LogLijst AddLogLijst(string naam, int gebruikersid, string ovmid)
        {
            //Hier wordt de nieuwe loglijst gemaakt.
            LogLijst lijst = new LogLijst()
            {
                Naam = naam,
                GebruikerId = gebruikersid,
                OvmId = ovmid,
                ActionDate = DateTime.Now
            };
            //Deze wordt aangemaakt in de SSHRepository.
            repo.CreateLogLijst(lijst);
            return lijst;
        }
        //Deze methode verwijdert alle loglijsten van een bepaalde klant.
        public void DeleteLogLijstenKlant(int gebruikerid)
        {
            repo.RemoveLogLijstenKlant(gebruikerid);
        }
        //Deze methode verwijdert alle loglijsten van een bepaalde Oracle Virtueel Machine.
        public void DeleteLogLijstenOVM(string id)
        {
            repo.RemoveLogLijstenOVM(id);
        }
        //Deze methode haalt alle LogLijsten van een klant op.
        public IEnumerable<LogLijst> GetLogLijstsKlant(int id)
        {
            return repo.ReadLogLijstKlant(id);
        }
        //Deze methode haalt alle LogLijsten van een Oracle Virtueel Machine op.
        public IEnumerable<LogLijst> GetLogLijstsOVM(string id)
        {
            return repo.ReadLogLijstOVM(id);
        }
        //Deze methode haalt alle LogLijsten op.
        public IEnumerable<LogLijst> GetLogLijsten()
        {
            return repo.ReadLogLijsten();
        }
        //Deze methode haalt een Scheduled Downtime van een Oracle Virtueel Machine op.
        public List<ScheduledDownTime> GetScheduledDTByOvm(string OvmId)
        {
            return repo.ReadScheduledDTByOvm(OvmId);
        }
        //Deze methode haalt alle Scheduled Downtime op.
        public List<ScheduledDownTime> GetScheduledDT()
        {
            return repo.ReadScheduledDT();
        }
        //Deze methode haalt een Scheduled Downtime op.
        public ScheduledDownTime GetScheduledDTById(int SDTid)
        {
            return repo.ReadScheduledDTById(SDTid);
        }
        //Deze methode voegt een Scheduled Downtime toe.
        public ScheduledDownTime AddScheduledDT(string ovmId, DateTime start, DateTime eind,string email)
        {
            //Maakt een nieuwe Scheduled Downtime aan.
            ScheduledDownTime SDT = new ScheduledDownTime()
            {
                OvmId = ovmId,
                Eind = eind,
                Start = start,
                Gebruikersnaam=email

            };
            //Deze wordt aangemaakt in de SSHRepository.
            return repo.CreateScheduledDT(SDT);
        }
        //Deze methode verwijdert een Scheduled Downtime.
        public void RemoveScheduledDT(ScheduledDownTime SDT)
        {
            repo.DeleteScheduledDT(SDT);
        }
    }
}
