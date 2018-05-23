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

        public OracleVirtualMachine AddOVM(string naam, string ovmId, int klantId, int serverId)
        {
            OracleVirtualMachine ovm = new OracleVirtualMachine()
            {
                Naam = naam,
                OvmId = ovmId,
                KlantId = klantId,
                ServerId=serverId                
            };
            OracleVirtualMachine created = repo.AddMachine(ovm);
            return created;
        }

        public OracleVirtualMachine GetOVMByOvmId(string OvmId)
        {
            return repo.GetMachineByOvmId(OvmId);
        }
        public IEnumerable<OracleVirtualMachine> GetOVMsByServer(int id)
        {
            return repo.ReadMachinesByServerId(id);
        }

        public void RemoveOVM(string id)
        {
            OracleVirtualMachine ovm = repo.GetMachineByOvmId(id);
            repo.DeleteMachine(ovm);
        }

        public OracleVirtualMachine GetOVM(int id)
        {
            return repo.GetMachine(id);
        }
        public OracleVirtualMachine GetOVM(string id)
        {
            return repo.GetMachine(id);
        }
        public OracleVirtualMachine GetOVMById(string id)
        {
            return repo.GetMachineById(id);
        }
        public IEnumerable<OracleVirtualMachine> GetKlantOVMs(int klantid)
        {
            return repo.GetKlantMachines(klantid);
        }
        public IEnumerable<OracleVirtualMachine> GetOVMs()
        {
            return repo.ReadMachines();
        }

        public void ChangeOVM(OracleVirtualMachine ovm)
        {
            repo.UpdateMachine(ovm);
        }

        public OVMLijst AddLijst(string ovmid, int klantid)
        {
            OVMLijst ovm = new OVMLijst()
            {
                AccountId = klantid,
                OVMId = ovmid
            };
            OVMLijst created = repo.AddLijst(ovm);
            return created;
        }

        public OVMLijst GetLijst(int klantid, string ovmid)
        {
            OVMLijst lijst = repo.GetLijst(klantid, ovmid);
            return lijst;
        }
        public IEnumerable<OVMLijst> GetLijstAccount(int klantid)
        {
            IEnumerable<OVMLijst> lijsten = repo.GetLijstAccount(klantid);
            return lijsten;
        }
        public void RemoveLijstenAccount(int klantid)
        {
            repo.DeleteLijstenAccount(klantid);
        }
        public void RemoveLijst(OVMLijst ovmlijst)
        {
            repo.DeleteLijst(ovmlijst);
        }
        public IEnumerable<OVMLijst> GetLijstOvm(string id)
        {
            IEnumerable<OVMLijst> lijsten = repo.GetLijstOVM(id);
            return lijsten;
        }
        public void RemoveLijstenOvm(string id)
        {
            repo.DeleteLijstenOvm(id);
        }
        public Server GetServer(int id)
        {
            return repo.GetServer(id);
        }
        public Server GetServer(string id)
        {
            return repo.GetServer(id);
        }
        public Server AddServer(string naam, string id)
        {
            Server server = new Server()
            {
                ServerNaam = naam,
                ServersId = id
            };
            repo.AddServer(server);
            return server;
        }
        public void RemoveServer(string id)
        {
            repo.DeleteServer(id);
        }
        public List<Server> GetServers()
        {
            return repo.ReadServers();
        }
        public LogLijst AddLogLijst(string naam, int gebruikersid, string ovmid)
        {
            LogLijst lijst = new LogLijst()
            {
                Naam = naam,
                GebruikerId = gebruikersid,
                OvmId = ovmid,
                ActionDate = DateTime.Now
            };
            repo.CreateLogLijst(lijst);
            return lijst;
        }
        public void DeleteLogLijstenKlant(int gebruikerid)
        {
            repo.RemoveLogLijstenKlant(gebruikerid);
        }
        public void DeleteLogLijstenOVM(string id)
        {
            repo.RemoveLogLijstenOVM(id);
        }
    }
}
