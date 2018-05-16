using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Renci.SshNet;
using StageSSPortal.Models;
using Domain;
using BL;
using DAL.EF;
using Microsoft.AspNet.Identity;
using System.Web.Http;
using SSPortalWebApi.Models;

namespace StageSSPortal.Controllers.api
{
    public class SSHController : ApiController
    {
        private readonly ISSHManager mgr = new SSHManager();
        private readonly IKlantManager klantmgr = new KlantManager();
        SshClient ssh;
        public SSHController()
        {

            ssh = new SshClient("10.0.12.240", 10000, "admin", "tst0VMman");

        }
        public string[] GetInfo(string vmId, SshClient ssh, string[] vmInfo2)
        {
            vmInfo2 = new string[8];
            string[] vmLines;
            ssh.Connect();
            var commandResult = ssh.RunCommand("show vm id=" + "'" + vmId + "'");
            ssh.Disconnect();
            vmLines = commandResult.Result.Split('\n');
            int j = 1;
            vmInfo2[0] = vmId;
            for (int i = 0; i < vmLines.Length; i++)
            {
                if (vmLines[i].Contains("Data:") || vmLines[i].Contains("Status =") || vmLines[i].Contains("Memory") || vmLines[i].Contains("Processors"))
                {
                    vmInfo2[j] = vmLines[i];
                    j++;
                }
            }
            OracleVirtualMachine vm = mgr.GetOVMById(vmId);
            if (vm.KlantId==0)
            {
                vmInfo2[7] = "NULL";
            }
            else
            {
                vmInfo2[7] = Convert.ToString(klantmgr.GetKlant(vm.KlantId).Naam);
            }
            return vmInfo2;
        }

        [HttpGet]
        [Route("api/SSH/CheckVms")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult checkForVms()
        {
            List<OracleVirtualMachine> serverVms = new List<OracleVirtualMachine>();
            string[] serverVMs;
            List<OracleVirtualMachine> OrderServerVms = new List<OracleVirtualMachine>();
            using (ssh)
            {
                ssh.Connect();
                var ServerResult = ssh.RunCommand("show Server name=hes-ora-vmtst01");
                serverVMs = ServerResult.Result.Split('\n');
                ssh.Disconnect();
                string patternVM = @"Vm [0-9]+";
                for (int i = 0; i < serverVMs.Length; i++)
                {
                    OracleVirtualMachine vm = new OracleVirtualMachine();
                    var resId = Regex.Match(serverVMs[i], patternVM);
                    if (resId.Length != 0)
                    {
                        string name = serverVMs[i].Substring(serverVMs[i].IndexOf("[") + 1, serverVMs[i].IndexOf("]") - serverVMs[i].IndexOf("[") - 1);
                        string id = serverVMs[i].Substring(serverVMs[i].IndexOf("=") + 1, (serverVMs[i].IndexOf("[")) - serverVMs[i].IndexOf("=") - 1).Trim();
                        vm.OvmId = id;
                        vm.Naam=name;
                        vm.KlantId = 0;
                        serverVms.Add(vm);
                    }
                }
            }
            List<OracleVirtualMachine> ovms = new List<OracleVirtualMachine>();
            ovms = mgr.GetOVMs().ToList();
            List<OracleVirtualMachine> orderDbVms = new List<OracleVirtualMachine>();
            List<string> databaseId = new List<string>();
            List<string> serverId = new List<string>();

            orderDbVms = ovms.OrderBy(o => o.OvmId).ToList();
            OrderServerVms = serverVms.OrderBy(o => o.OvmId).ToList();

            for (int i = 0; i < orderDbVms.Count(); i++)
            {
                databaseId.Add(orderDbVms[i].OvmId);
            }
            for (int i = 0; i < OrderServerVms.Count(); i++)
            {
                serverId.Add(OrderServerVms[i].OvmId);
            }

            for (int i = 0; i < OrderServerVms.Count(); i++)
            {

                if (!databaseId.Contains(OrderServerVms[i].OvmId))
                {
                    mgr.AddOVM(OrderServerVms[i].Naam, OrderServerVms[i].OvmId, 0);
                    orderDbVms.Add(OrderServerVms[i]);
                }
            }
            for (int i = 0; i < orderDbVms.Count(); i++)
            {
                if (!serverId.Contains(orderDbVms[i].OvmId))
                {
                    mgr.RemoveOVM(orderDbVms[i].OvmId);
                    orderDbVms.Remove(orderDbVms[i]);
                }
            }
            return Ok(orderDbVms);
        }


        [HttpGet]
        [Route("api/SSH/getHoofdAcc/{id}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetHoofdAcc(int id)
        {
                Klant temp = klantmgr.GetKlant(id);
                return Ok(temp);
            

        }

        [HttpGet]
        [Route("api/SSH/getKlantAcc/{id}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetKlantAcc(int id)
        {
            Klant temp = klantmgr.GetKlant(id);
            IEnumerable<Klant> klantAccs = new List<Klant>();
            klantAccs = klantmgr.GetKlantenAccounts(temp);
            return Ok(klantAccs);


        }

        [HttpGet]
        [Route("api/SSH/Vms")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetVms(List<VmModel> model)
        {
            model = new List<VmModel>();
            List<string> vmState = new List<string>();
            string[] vmInfo = new string[7];
            string[] getVmInfo = new string[7];
            List<string> LijstServerVMs = new List<string>();

            string[] serverVMs;
            string[] VmNames;
            using (ssh)
            {
                ssh.Connect();
                var ServerResult = ssh.RunCommand("show Server name=hes-ora-vmtst01");
                serverVMs = ServerResult.Result.Split('\n');
                ssh.Disconnect();
                ssh.Connect();
                var VmResult = ssh.RunCommand("list vm ");
                VmNames = VmResult.Result.Split('\n');
                ssh.Disconnect();
                string patternVM = @"Vm [0-9]+";
                string idPattern = @"id: [0-9]+";
                List<OracleVirtualMachine> ovms = new List<OracleVirtualMachine>();
                ovms = mgr.GetOVMs().ToList();
                bool isEmpty = !ovms.Any();
                for (int i = 0; i < serverVMs.Length; i++)
                {
                    var res = Regex.Match(serverVMs[i], patternVM);
                    var resId = Regex.Match(serverVMs[i], idPattern);
                    if (res.Length != 0)
                    {
                        VmModel vmModel = new VmModel();
                        string name = serverVMs[i].Substring(serverVMs[i].IndexOf("[") + 1, serverVMs[i].IndexOf("]") - serverVMs[i].IndexOf("[") - 1);
                        string id = serverVMs[i].Substring(serverVMs[i].IndexOf("=") + 1, (serverVMs[i].IndexOf("[")) - serverVMs[i].IndexOf("=") - 1);
                        id=id.Trim();
                        vmModel.Name = name;
                        vmModel.id = id;

                        if (isEmpty)
                        {
                            OracleVirtualMachine cutted = mgr.AddOVM(name,id, 1);
                        }
                        else
                        {
                            bool bezit = false;
                            foreach (OracleVirtualMachine ovm in ovms)
                            {
                                if (ovm.Naam.Equals(name))
                                {
                                    bezit = true;
                                }

                            }
                            if (bezit == true)
                            {

                            }
                            else
                            {
                                OracleVirtualMachine cutted = mgr.AddOVM(name,id, 1);
                            }
                        }

                        vmInfo = GetInfo(id, ssh, getVmInfo);
                        var regex = @"Status = [A-Z]+";
                        for (int j = 0; j < vmInfo.Length; j++)
                        {
                            var match = Regex.Match(vmInfo[j], regex);
                            if (match.Length != 0)
                            {
                                vmModel.Status = vmInfo[j].Substring(vmInfo[j].IndexOf("=") + 2, vmInfo[j].Length - vmInfo[j].IndexOf("=") - 2);
                            }
                        }
                        model.Add(vmModel);
                    }
                }
            }
            return Ok(model);
        }

        [HttpGet]
        [Route("api/SSH/VmsDB")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetVmsDB(List<VmModel> model)
        {
            model = new List<VmModel>();
            List<string> vmState = new List<string>();
            string[] vmInfo = new string[8];
            string[] getVmInfo = new string[8];
            List<string> LijstServerVMs = new List<string>();
            List<OracleVirtualMachine> ovms = new List<OracleVirtualMachine>();
            ovms = mgr.GetOVMs().ToList();

            foreach (OracleVirtualMachine vm in ovms)
            {
                VmModel vmModel = new VmModel();
                vmModel.Name = vm.Naam;
                vmModel.id = vm.OvmId.Trim();
                vmModel.KlantId = vm.KlantId;
                model.Add(vmModel);
            }

            return Ok(model);
        }

        public List<String> GetVmState(List<string> LijstServerVMs, SshClient ssh)
        {
            List<string> vmState = new List<string>();
                foreach (var vm in LijstServerVMs)
                {
                string[] tempState = new string[7];
                    GetInfo(vm, ssh, tempState);
                    var regex = @"Status = [A-Z]+";
                    foreach (var line in tempState)
                    {
                        var match = Regex.Match(line, regex);
                        if (match.Length != 0)
                        {
                            string cut = line.Substring(line.IndexOf("=") + 2, line.Length - line.IndexOf("=") - 2);
                            vmState.Add(cut);
                        }
                    }
                }
            return vmState;
        }

        [HttpGet]
        [Route("api/SSH/Info/{id}")]
        [Authorize(Roles = "Admin , Klant , KlantAccount")]
        public IHttpActionResult GetInfo(string id)
        {
            string[] Info = new string[8];
            using (ssh)
            {
                Info = GetInfo(id, ssh, Info);
            }

            return Ok(Info);
        }
        [HttpGet]
        [Route("api/SSH/Klanten")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetKlanten()
        {
            List<Klant> klanten = new List<Klant>();
            klanten = klantmgr.GetHoofdKlanten().ToList();
            return Ok(klanten);
        }

        [HttpGet]
        [Route("api/SSH/KlantOVM/{id}/{k}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult SetKlantOVM(string id,string k)
        {
            OracleVirtualMachine ovm = mgr.GetOVMById(id);
            Klant klant = klantmgr.GetKlantByName(k);
            ovm.KlantId = klant.KlantId;
            mgr.ChangeOVM(ovm);
            return Ok(ovm.OvmId);
        }

        [HttpGet]
        [Route("api/SSH/StopVm/{id}")]
        [Authorize(Roles = "Admin , Klant , KlantAccount")]
        public IHttpActionResult StopVm(string id)
        {
            using (ssh)
            {
                ssh.Connect();
                ssh.RunCommand("stop Vm name=" + id);
                ssh.Disconnect();
            }
            return Ok();
            
        }

        [HttpGet]
        [Route("api/SSH/StartVm/{id}")]
        [Authorize(Roles = "Admin , Klant, KlantAccount")]
        public IHttpActionResult StartVm(string id)
        {
            using (ssh)
            {
                ssh.Connect();
                ssh.RunCommand("start Vm name=" + id);
                ssh.Disconnect();
            }
            return Ok();
        }

        [HttpGet]
        [Route("api/Klant/SSH/KlantOVMs")]
        [Authorize(Roles = "Klant")]
        public IHttpActionResult KlantOVMs(List<VmModel> model)
        {
            model = new List<VmModel>();
            //List<string> vmState = new List<string>();
            string[] vmInfo = new string[7];
            string[] getVmInfo = new string[7];
            //List<string> LijstServerVMs = new List<string>();
            List<OracleVirtualMachine> ovms = new List<OracleVirtualMachine>();
            Klant k = klantmgr.GetKlant(User.Identity.GetUserName());
            ovms = mgr.GetKlantOVMs(k.KlantId).ToList();
            //List<string> klantovms = new List<string>();
            //using (ssh)
            //{
            foreach (OracleVirtualMachine vm in ovms)
            {
                VmModel vmModel = new VmModel();
                vmModel.Name = vm.Naam;
                vmModel.id = vm.OvmId;
                model.Add(vmModel);
            }
            // }

            return Ok(model);
        }
        [HttpGet]
        [Route("api/Klant/SSH/lijstcreate/{id}/{k}")]
        [Authorize(Roles = "Klant")]
        public IHttpActionResult CreateLijst(string id, string k)
        {
            Klant klant = klantmgr.GetKlant(k);
            OVMLijst check = mgr.GetLijst(klant.KlantId, id);
            if (check == null)
            {
                OVMLijst lijst = mgr.AddLijst(id, klant.KlantId);
            } 
            return Ok();
        }
        [HttpGet]
        [Route("api/Klant/SSH/lijstDelete/{id}/{k}")]
        [Authorize(Roles = "Klant")]
        public IHttpActionResult DeleteLijst(string id, string k)
        {
            Klant klant = klantmgr.GetKlant(k);
            OVMLijst lijst = mgr.GetLijst(klant.KlantId, id);
            if(lijst != null)
            {
                mgr.RemoveLijst(lijst);
            }           
            return Ok();
        }

        [HttpGet]
        [Route("api/Klant/SSH/AccountOVMs")]
        [Authorize(Roles = "KlantAccount")]
        public IHttpActionResult AccountOVMs(List<VmModel> model)
        {
            model = new List<VmModel>();
            //List<string> vmState = new List<string>();
            string[] vmInfo = new string[7];
            string[] getVmInfo = new string[7];
            //List<string> LijstServerVMs = new List<string>();
            List<OracleVirtualMachine> ovms = new List<OracleVirtualMachine>();
            Klant k = klantmgr.GetKlant(User.Identity.GetUserName());
            List<OVMLijst> lijsten = mgr.GetLijstAccount(k.KlantId).ToList();
            for(int i= 0; i< lijsten.Count();i++)
            {
                OracleVirtualMachine ovm = mgr.GetOVMById(lijsten[i].OVMId);
                ovms.Add(ovm);
            }
            //List<string> klantovms = new List<string>();
            //using (ssh)
            //{
            foreach (OracleVirtualMachine vm in ovms)
            {
                VmModel vmModel = new VmModel();
                vmModel.Name = vm.Naam;
                vmModel.id = vm.OvmId;
                model.Add(vmModel);
            }
            // }

            return Ok(model);
        }

    }
}