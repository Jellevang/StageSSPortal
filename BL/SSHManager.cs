﻿using System;
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

        public OracleVirtualMachine AddOVM(string naam, int klantId)
        {
            OracleVirtualMachine ovm = new OracleVirtualMachine()
            {
                Naam = naam,
                KlantId = klantId
            };
            OracleVirtualMachine created = repo.AddMachine(ovm);
            return created;
        }
        public void RemoveOVM(int id)
        {
            OracleVirtualMachine ovm = repo.GetMachine(id);
            repo.DeleteMachine(ovm);
        }

        public OracleVirtualMachine GetOVM(int id)
        {
            return repo.GetMachine(id);
        }
        public OracleVirtualMachine GetOVM(string naam)
        {
            return repo.GetMachine(naam);
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

        public OVMLijst AddLijst(int ovmid, int klantid)
        {
            OVMLijst ovm = new OVMLijst()
            {
                AccountId = klantid,
                OVMId = ovmid
            };
            OVMLijst created = repo.AddLijst(ovm);
            return created;
        }
    }
}
