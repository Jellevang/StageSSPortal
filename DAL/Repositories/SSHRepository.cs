﻿using DAL.EF;
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
            OracleVirtualMachine ovm = ctx.OracleVirtualMachines.Where(o=>o.OvmId.Equals(OvmId)).FirstOrDefault();
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
            for (int i=0;i<ovms.Count();i++)
            {
                string ids = ovms[i].OvmId;
                //ids = ids.Trim();
                if(ids.Equals(id))
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
        public IEnumerable<OVMLijst> ReadLijsten()
        {
            return ctx.OVMLijsten.AsEnumerable();
        }
    }
}
