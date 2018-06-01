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

        public OracleVirtualMachine AddMachine(OracleVirtualMachine ovm)
        {
            ctx.OracleVirtualMachines.Add(ovm);
            ctx.SaveChanges();
            return ovm;
        }

        public void DeleteMachine(OracleVirtualMachine ovm)
        {
            ctx.OracleVirtualMachines.Remove(ovm);
            ctx.SaveChanges();
        }

        public OracleVirtualMachine GetMachine(int id)
        {
            OracleVirtualMachine ovm = ctx.OracleVirtualMachines.Find(id);
            return ovm;
        }

        public OracleVirtualMachine GetMachineByOvmId(string OvmId)
        {
            OracleVirtualMachine ovm = ctx.OracleVirtualMachines.Where(o => o.OvmId.Equals(OvmId)).FirstOrDefault();
            return ovm;

        }
        public IEnumerable<OracleVirtualMachine> ReadMachines()
        {
            return ctx.OracleVirtualMachines.AsEnumerable();
        }
        public OracleVirtualMachine GetMachine(string id)
        {
            //OracleVirtualMachine ovm = ctx.OracleVirtualMachines.Where(o => o.OvmId.Equals(id)).FirstOrDefault();
            List<OracleVirtualMachine> ovms = ReadMachines().ToList();
            OracleVirtualMachine ovm = new OracleVirtualMachine();
            for (int i = 0; i < ovms.Count(); i++)
            {
                string ids = ovms[i].OvmId;
                //ids = ids.Trim();
                if (ids.Equals(id))
                {
                    ovm = ovms[i];
                }
            }
            return ovm;
        }
        public OracleVirtualMachine GetMachineById(string id)
        {

            OracleVirtualMachine ovm = ctx.OracleVirtualMachines.Where(o => o.OvmId.Contains(id)).FirstOrDefault();
            return ovm;
        }
        public IEnumerable<OracleVirtualMachine> GetKlantMachines(int klantid)
        {
            IEnumerable<OracleVirtualMachine> ovms = ctx.OracleVirtualMachines.Where(o => o.KlantId == klantid);
            return ovms;
        }

        public void UpdateMachine(OracleVirtualMachine ovm)
        {
            OracleVirtualMachine ovM = ctx.OracleVirtualMachines.Find(ovm.OracleVirtualMachineId);
            ctx.Entry(ovM).CurrentValues.SetValues(ovm);
            ctx.Entry(ovM).State = System.Data.Entity.EntityState.Modified;

            //ctx.Entry(ovm).State = EntityState.Modified;
            ctx.SaveChanges();
        }

        public OVMLijst AddLijst(OVMLijst lijst)
        {
            ctx.OVMLijsten.Add(lijst);
            ctx.SaveChanges();
            return lijst;
        }
        public void DeleteLijst(OVMLijst lijst)
        {
            ctx.OVMLijsten.Remove(lijst);
            ctx.SaveChanges();
        }

        public OVMLijst GetLijst(int id)
        {
            OVMLijst ovm = ctx.OVMLijsten.Find(id);
            return ovm;
        }
        public OVMLijst GetLijst(int klantid, string ovmid)
        {
            IEnumerable<OVMLijst> ovmsL = ctx.OVMLijsten.Where(o => o.OVMId.Contains(ovmid));
            OVMLijst lijst = ovmsL.Where(l => l.AccountId == klantid).FirstOrDefault();
            return lijst;
        }
        public IEnumerable<OVMLijst> GetLijstAccount(int klantid)
        {
            IEnumerable<OVMLijst> ovmsL = ctx.OVMLijsten.Where(o => o.AccountId == klantid);
            return ovmsL;
        }
        public IEnumerable<OVMLijst> ReadLijsten()
        {
            return ctx.OVMLijsten.AsEnumerable();
        }
        public void DeleteLijstenAccount(int klantid)
        {
            IEnumerable<OVMLijst> ovmsL = ctx.OVMLijsten.Where(o => o.AccountId == klantid);
            foreach (OVMLijst lijst in ovmsL)
            {
                ctx.OVMLijsten.Remove(lijst);
            }
            ctx.SaveChanges();
        }

        public IEnumerable<OVMLijst> GetLijstOVM(string id)
        {
            IEnumerable<OVMLijst> ovmsL = ctx.OVMLijsten.Where(o => o.OVMId.Contains(id));
            return ovmsL;
        }
        public void DeleteLijstenOvm(string id)
        {
            IEnumerable<OVMLijst> ovmsL = ctx.OVMLijsten.Where(o => o.OVMId.Contains(id));
            foreach (OVMLijst lijst in ovmsL)
            {
                ctx.OVMLijsten.Remove(lijst);
            }
            ctx.SaveChanges();
        }
        public IEnumerable<OracleVirtualMachine> ReadMachinesByServerId(int id)
        {
            return ctx.OracleVirtualMachines.Where(vm => vm.ServerId == id);
        }

        public Server GetServer(int id)
        {
            return ctx.Servers.Where(s => s.Id == id).FirstOrDefault();
        }
        public Server GetServer(string id)
        {
            return ctx.Servers.Where(s => s.ServersId.Contains(id)).FirstOrDefault();
        }
        public Server AddServer(Server server)
        {
            ctx.Servers.Add(server);
            ctx.SaveChanges();
            return server;
        }
        public List<Server> ReadServers()
        {
            return ctx.Servers.ToList();
        }
        public void DeleteServer(string id)
        {
            ctx.Servers.Remove(GetServer(id));
        }
        public LogLijst CreateLogLijst(LogLijst log)
        {
            ctx.LogLijsten.Add(log);
            ctx.SaveChanges();
            return log;
        }
        public void RemoveLogLijstenKlant(int gebruikersId)
        {
            IEnumerable<LogLijst> logs = ctx.LogLijsten.Where(l => l.GebruikerId ==gebruikersId);
            foreach (LogLijst lijst in logs)
            {
                ctx.LogLijsten.Remove(lijst);
            }
            ctx.SaveChanges();
        }
        public void RemoveLogLijstenOVM(string id)
        {
            IEnumerable<LogLijst> logs = ctx.LogLijsten.Where(l => l.OvmId.Contains(id));
            foreach (LogLijst lijst in logs)
            {
                ctx.LogLijsten.Remove(lijst);
            }
            ctx.SaveChanges();
        }
        public IEnumerable<LogLijst> ReadLogLijstKlant(int gebruikersid)
        {
            IEnumerable<LogLijst> logs = ctx.LogLijsten.Where(l => l.GebruikerId == gebruikersid);
            return logs;
        }
        public IEnumerable<LogLijst> ReadLogLijstOVM(string id)
        {
            IEnumerable<LogLijst> logs = ctx.LogLijsten.Where(l => l.OvmId.Contains(id));
            return logs;
        }
        public IEnumerable<LogLijst> ReadLogLijsten()
        {
            return ctx.LogLijsten.AsEnumerable();
        }

        public List<ScheduledDownTime> ReadScheduledDTByOvm(string OvmId)
        {
            return ctx.ScheduleTDLijst.Where(s => s.OvmId.Contains(OvmId)).ToList();
        }
        public ScheduledDownTime ReadScheduledDTById(int SDTid)
        {
            return ctx.ScheduleTDLijst.Where(s => s.id == SDTid).FirstOrDefault() ;
        }

        public List<ScheduledDownTime> ReadScheduledDT()
        {
            return ctx.ScheduleTDLijst.ToList();
        }
        public ScheduledDownTime CreateScheduledDT(ScheduledDownTime SDT)
        {
            ctx.ScheduleTDLijst.Add(SDT);
            ctx.SaveChanges();
            return SDT;
        }
        public void DeleteScheduledDT(ScheduledDownTime SDT)
        {
            ctx.ScheduleTDLijst.Remove(ReadScheduledDTById(SDT.id));
            ctx.SaveChanges();
        }

    }
}
