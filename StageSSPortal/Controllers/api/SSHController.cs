using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Renci.SshNet;
using Microsoft.Owin.Security.DataProtection;
using Domain;
using BL;
using DAL.EF;
using Microsoft.AspNet.Identity;
using System.Web.Http;
using System.Web;
using SSPortalWebApi.Models;
using Domain.Gebruikers;
using Microsoft.AspNet.Identity.Owin;
using StageSSPortal.Helpers;
using StageSSPortal.Models;
using RestSharp;

namespace StageSSPortal.Controllers.api
{
    public class SSHController : ApiController
    {
        private readonly IAdminManager Admgr = new AdminManager();
        private readonly ISSHManager mgr = new SSHManager();
        private readonly IKlantManager klantmgr = new KlantManager();
        private GebruikerManager userManager;
        SshClient ssh;
        AdminManager admgr = new AdminManager();

        SSHManager sshmgr=new SSHManager();
        public SSHController()
        {
            if (User.Identity.Name.Trim() != null && User.Identity.Name.Trim() !="")
            {
                userManager = GebruikerManager.Create(System.Web.HttpContext.Current.GetOwinContext()
                    .Get<AppBuilderProvider>().Get().GetDataProtectionProvider());                          // AppbuilderProvider is een custom klasse die geregistreerd wordt in de startup.auth.cs
            }
            Admin admin = admgr.GetAdmin();
            string passwd = admgr.GetPasswd(admin);
            string trimpasswd=passwd.Replace("'", "");

            ssh = new SshClient("10.0.12.240", 10000, "admin", trimpasswd);
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
                if (vmLines[i].Contains("Data:") || 
                    vmLines[i].Contains("Status =") || 
                    vmLines[i].Contains("Memory") || vmLines[i].Contains("Processors"))
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
        [Route("api/ssh/getServersDB")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult getServersDB()
        {
            List<Server> servers = new List<Server>();
            servers=sshmgr.GetServers();
            return Ok(servers);
        }
        [HttpGet]
        [Route("api/ssh/getServers")]
        [Authorize(Roles="Admin")]
        public IHttpActionResult getServers()
        {
            string[] serverVMs;
            List<Server> servers = new List<Server>();
            List<Server> OrderServers = new List<Server>();
            List<Server> DbServers = new List<Server>();
            List<Server> OrderDbServers = new List<Server>();
            List<string> databaseId = new List<string>();
            List<string> ListServerId = new List<string>();
            string serverId;
            string serverName;
            using (ssh)
            {
                ssh.Connect();
                var ServerResult = ssh.RunCommand("list server");
                serverVMs = ServerResult.Result.Split('\n');
                ssh.Disconnect();
                string patternId = @"id:";
                
                for (int i = 0; i < serverVMs.Length; i++)
                {
                    var resId = Regex.Match(serverVMs[i], patternId);
                    if (resId.Length != 0)
                    {
                        string[] serverAttributes=serverVMs[i].Split(' ');
                        serverId = serverAttributes[2].Substring(serverAttributes[2].IndexOf(":") + 1
                            , serverAttributes[2].Length - (serverAttributes[4].IndexOf(":") -1));
                        serverName = serverAttributes[4].Substring(serverAttributes[4].IndexOf(":") + 1
                            , serverAttributes[4].Length - serverAttributes[4].IndexOf(":") -1);
                        Server s = new Server();
                        s.ServerNaam = serverName;
                        s.ServersId = serverId;
                        servers.Add(s);                    
                    }
                }
                OrderServers = servers.OrderBy(Os => Os.ServersId).ToList();
                DbServers = sshmgr.GetServers();
                OrderDbServers = DbServers.OrderBy(ODs => ODs.ServersId).ToList();
                for (int i = 0; i < OrderDbServers.Count(); i++)
                {
                    databaseId.Add(OrderDbServers.ElementAt(i).ServersId);
                }
                for (int i = 0; i < OrderServers.Count(); i++)
                {
                    ListServerId.Add(OrderServers[i].ServersId);
                }

                for (int i = 0; i < OrderServers.Count(); i++)
                {

                    if (!databaseId.Contains(OrderServers[i].ServersId))
                    {
                        sshmgr.AddServer(OrderServers[i].ServerNaam, OrderServers[i].ServersId);
                        OrderDbServers.Add(OrderServers[i]);
                    }
                }
                for (int i = 0; i < OrderDbServers.Count(); i++)
                {
                    if (!ListServerId.Contains(OrderDbServers.ElementAt(i).ServersId))
                    {
                        sshmgr.RemoveServer(OrderDbServers.ElementAt(i).ServersId);
                        OrderDbServers.Remove(sshmgr.GetServer(OrderDbServers.ElementAt(i).ServersId));
                    }
                }
                return Ok(true);
            }
            
        }

        [HttpGet]
        [Route("api/SSH/CheckVms")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult checkForVms()
        {
            List<OracleVirtualMachine> serverVms = new List<OracleVirtualMachine>();
            string[] serverVMs;
            IEnumerable<Server> serverlist = sshmgr.GetServers();
            List<OracleVirtualMachine> OrderServerVms = new List<OracleVirtualMachine>();
            
            using (ssh)
            {
                for(int i = 0; i < serverlist.Count(); i++)
                {
                    ssh.Connect();
                    var ServerResult = ssh.RunCommand("show Server name="+serverlist.ElementAt(i).ServerNaam);
                    serverVMs = ServerResult.Result.Split('\n');
                    ssh.Disconnect();
                
                
                string patternVM = @"Vm [0-9]+";
               
                for (int j = 0; j < serverVMs.Length; j++)
                {
                    OracleVirtualMachine vm = new OracleVirtualMachine();
                    var resId = Regex.Match(serverVMs[j], patternVM);
                    if (resId.Length != 0)
                    {
                        string name = serverVMs[j].Substring(serverVMs[j].IndexOf("[") + 1, serverVMs[j].IndexOf("]") - serverVMs[j].IndexOf("[") - 1);
                        string id = serverVMs[j].Substring(serverVMs[j].IndexOf("=") + 1, (serverVMs[j].IndexOf("[")) - serverVMs[j].IndexOf("=") - 1).Trim();
                        vm.OvmId = id;
                        vm.Naam=name;
                        vm.KlantId = 0;
                        vm.ServerId = serverlist.ElementAt(i).Id;
                        serverVms.Add(vm);
                    }
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
                    mgr.AddOVM(OrderServerVms[i].Naam, OrderServerVms[i].OvmId, 0,OrderServerVms[i].ServerId);
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
        [Route("api/SSH/getKlanten")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetAllKlanten()
        {
            IEnumerable<Klant> temp = klantmgr.GetKlanten();
            return Ok(temp);
        }
        [HttpGet]
        [Route("api/SSH/getAdmin")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetAdmin()
        {
            Admin a = admgr.GetAdmin();
            var g = userManager.GetGebruiker(a.Email);
            return Ok(g);
        }

        [HttpGet]
        [Route("api/SSH/GetAllVmsDB")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetAllVmsDB(List<VmModel> model)
        {
            model = new List<VmModel>();
            IEnumerable<OracleVirtualMachine> ovms;//= new List<OracleVirtualMachine>();
            ovms = mgr.GetOVMs() ;

            foreach (OracleVirtualMachine vm in ovms)
            {
                VmModel vmModel = new VmModel();
                vmModel.Name = vm.Naam;
                vmModel.id = vm.OvmId.Trim();
                vmModel.KlantId = vm.KlantId;
                vmModel.Serverid = vm.ServerId;
                model.Add(vmModel);
            }

            var orderModel = model.OrderBy(c => c.Name);
            return Ok(orderModel);
        }
        [HttpGet]
        [Route("api/SSH/VmsDB/{id}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetVmsDB(List<VmModel> model, int id)
        {
            model = new List<VmModel>();
            List<string> vmState = new List<string>();
            string[] vmInfo = new string[8];
            string[] getVmInfo = new string[8];
            List<string> LijstServerVMs = new List<string>();
            List<OracleVirtualMachine> ovms = new List<OracleVirtualMachine>();
            ovms = mgr.GetOVMsByServer(id).ToList();

            foreach (OracleVirtualMachine vm in ovms)
            {
                VmModel vmModel = new VmModel();
                vmModel.Name = vm.Naam;
                vmModel.id = vm.OvmId.Trim();
                vmModel.KlantId = vm.KlantId;
                vmModel.Serverid = vm.ServerId;
                model.Add(vmModel);
            }

            var orderModel = model.OrderBy(c => c.Name);
            return Ok(orderModel);
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
        [Route("api/SSH/GetKlantenUser")]
        [Authorize(Roles = "Klant")]
        public IHttpActionResult GetKlantenBedrijf()
        {
            Klant k = klantmgr.GetKlant(User.Identity.Name);
            List<Klant> klanten = new List<Klant>();
            klanten.Add(k);
            klanten.AddRange(klantmgr.GetKlantenAccounts(k));
            return Ok(klanten);
        }

        [HttpGet]
        [Route("api/SSH/KlantOVM/{id}/{k}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult SetKlantOVM(string id,string k)
        {
            List<OVMLijst> lijsten = mgr.GetLijstOvm(id).ToList();
            if(lijsten != null)
            {
                mgr.RemoveLijstenOvm(id);
            }
            OracleVirtualMachine ovm = mgr.GetOVMById(id);
            Klant klant = klantmgr.GetKlantByName(k);
            if (klant == null)
            {
                klant = klantmgr.GetKlant(k);
            }
            ovm.KlantId = klant.KlantId;
            mgr.ChangeOVM(ovm);
            return Ok(ovm.OvmId);
        }
        [HttpGet]
        [Route("api/SSH/CreateNaam/{id}/{naam}")]
        [Authorize(Roles = "Admin , Klant , KlantAccount")]
        public IHttpActionResult CreateNaam(string id, string naam)
        {
            Gebruiker user = userManager.GetGebruiker(User.Identity.GetUserName());
            LogLijst lijst = mgr.AddLogLijst(naam, user.GebruikerId, id);
            return Ok(true);
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
            return Ok(true);
            
            
        }

        [HttpGet]
        [Route("api/SSH/RestartVm/{id}")]
        [Authorize(Roles = "Admin , Klant , KlantAccount")]
        public IHttpActionResult RestartVm(string id)
        {
            using (ssh)
            {
                ssh.Connect();
                ssh.RunCommand("restart Vm name=" + id);
                ssh.Disconnect();
            }           
            return Ok(true);

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
            return Ok(true);
        }

        [HttpGet]
        [Route("api/Klant/SSH/KlantOVMs")]
        [Authorize(Roles = "Klant")]
        public IHttpActionResult KlantOVMs(List<VmModel> model)
        {
            model = new List<VmModel>();
            string[] vmInfo = new string[7];
            string[] getVmInfo = new string[7];
            List<OracleVirtualMachine> ovms = new List<OracleVirtualMachine>();
            Klant k = klantmgr.GetKlant(User.Identity.GetUserName());
            ovms = mgr.GetKlantOVMs(k.KlantId).ToList();
            foreach (OracleVirtualMachine vm in ovms)
            {
                VmModel vmModel = new VmModel();
                vmModel.Name = vm.Naam;
                vmModel.id = vm.OvmId;
                model.Add(vmModel);
            }
            if (!model.Any())
            {
                return Ok(false);
            }
            else
            {
                var orderModel = model.OrderBy(c => c.Name);
                return Ok(orderModel);
            } 
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
            return Ok(true);
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
            string[] vmInfo = new string[7];
            string[] getVmInfo = new string[7];
            List<OracleVirtualMachine> ovms = new List<OracleVirtualMachine>();
            Klant k = klantmgr.GetKlant(User.Identity.GetUserName());
            List<OVMLijst> lijsten = mgr.GetLijstAccount(k.KlantId).ToList();
            for(int i= 0; i< lijsten.Count();i++)
            {
                OracleVirtualMachine ovm = mgr.GetOVMById(lijsten[i].OVMId);
                ovms.Add(ovm);
            }
            foreach (OracleVirtualMachine vm in ovms)
            {
                VmModel vmModel = new VmModel();
                vmModel.Name = vm.Naam;
                vmModel.id = vm.OvmId;
                model.Add(vmModel);
            }
            if (!model.Any())
            {
                return Ok(false);
            }
            else
            {
                return Ok(model);
            }
        }
        [HttpGet]
        [Route("api/Klant/SSH/LogLijstOvm/{id}")]
        [Authorize(Roles = "Admin , Klant")]
        public IHttpActionResult LogLijstOvm(string id)
        {
            List<LogModel> model = new List<LogModel>();
            List<LogLijst> logs = mgr.GetLogLijstsOVM(id).ToList();
            var orderLogs = logs.OrderByDescending(l => l.ActionDate);
            OracleVirtualMachine ovm = mgr.GetOVMById(id);
            Admin admin = admgr.GetAdmin();
            if (logs.Count() != 0)
            {
                if (logs.Count() >= 10)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Gebruiker user = userManager.GetGebruiker(orderLogs.ElementAt(i).GebruikerId);
                        LogModel logmodel = new LogModel();
                        logmodel.Naam = orderLogs.ElementAt(i).Naam;
                        logmodel.ActionDate = orderLogs.ElementAt(i).ActionDate;
                        logmodel.Gebruiker = user.Email;
                        if (admin.Email == User.Identity.Name)
                        {
                            model.Add(logmodel);
                        }
                        else
                        {
                            if (!logmodel.Gebruiker.Equals(admin.Email))
                            {
                                model.Add(logmodel);
                            }
                        }
                    }
                }
                if (logs.Count() < 10)
                {
                    for (int i = 0; i < logs.Count(); i++)
                    {
                        Gebruiker user = userManager.GetGebruiker(orderLogs.ElementAt(i).GebruikerId);
                        LogModel logmodel = new LogModel();
                        logmodel.Naam = orderLogs.ElementAt(i).Naam;
                        logmodel.ActionDate = orderLogs.ElementAt(i).ActionDate;
                        logmodel.Gebruiker = user.Email;
                        if (admin.Email == User.Identity.Name)
                        {
                            model.Add(logmodel);
                        }
                        else
                        {
                            if (!logmodel.Gebruiker.Equals(admin.Email))
                            {
                                model.Add(logmodel);
                            }
                        }
                    }
                }
            }
            if (!model.Any())
            {
                return Ok(false);
            }
            else
            {
                return Ok(model);
            }
        }
        [HttpGet]
        [Route("api/Klant/SSH/LogLijstKlant/{id}")]
        [Authorize(Roles = "Admin , Klant")]
        public IHttpActionResult LogLijstKlant(List<LogModel> model, int id)
        {
            List<LogModel> OrderModel = new List<LogModel>();
            model = new List<LogModel>();
            Klant k = klantmgr.GetKlant(id);
            if(k.IsKlantAccount==false)
            {
                List<LogLijst> logs = mgr.GetLogLijstsKlant(id).ToList();
                var orderLogs = logs.OrderByDescending(l => l.ActionDate);
                List<Klant> medewerkers = klantmgr.GetKlantenAccounts(k).ToList();
                if (logs.Count() != 0)
                {
                    if (logs.Count() >= 10)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            OracleVirtualMachine ovm = mgr.GetOVMById(logs[i].OvmId);
                            LogModel logmodel = new LogModel();
                            logmodel.Naam = orderLogs.ElementAt(i).Naam;
                            logmodel.ActionDate = orderLogs.ElementAt(i).ActionDate;
                            logmodel.Ovm = ovm.Naam;
                            logmodel.Gebruiker = k.Email;
                            model.Add(logmodel);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < logs.Count(); i++)
                        {
                            OracleVirtualMachine ovm = mgr.GetOVMById(logs[i].OvmId);
                            LogModel logmodel = new LogModel();
                            logmodel.Naam = orderLogs.ElementAt(i).Naam;
                            logmodel.ActionDate = orderLogs.ElementAt(i).ActionDate;
                            logmodel.Ovm = ovm.Naam;
                            logmodel.Gebruiker = k.Email;
                            model.Add(logmodel);
                        }
                    }
                    for (int i = 0; i < medewerkers.Count(); i++)
                    {
                        List<LogLijst> logsM = mgr.GetLogLijstsKlant(medewerkers[i].KlantId).ToList();
                        var orderLogsM = logsM.OrderByDescending(l => l.ActionDate);
                        for (int j = 0; j < logsM.Count(); j++)
                        {
                            OracleVirtualMachine ovm = mgr.GetOVMById(logsM[j].OvmId);
                            LogModel logmodel = new LogModel();
                            logmodel.Naam = orderLogsM.ElementAt(i).Naam;
                            logmodel.ActionDate = orderLogsM.ElementAt(i).ActionDate;
                            logmodel.Ovm = ovm.Naam;
                            logmodel.Gebruiker = medewerkers[i].Email;
                            model.Add(logmodel);
                        }
                    }
                }
            }
            else
            {
                List<LogLijst> logs = mgr.GetLogLijstsKlant(id).ToList();
                var orderLogs = logs.OrderByDescending(l => l.ActionDate);
                if (logs.Count() != 0)
                {
                    if (logs.Count() >= 10)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            OracleVirtualMachine ovm = mgr.GetOVMById(logs[i].OvmId);
                            LogModel logmodel = new LogModel();
                            logmodel.Naam = orderLogs.ElementAt(i).Naam;
                            logmodel.ActionDate = orderLogs.ElementAt(i).ActionDate;
                            logmodel.Ovm = ovm.Naam;
                            logmodel.Gebruiker = k.Email;
                            model.Add(logmodel);
                        }
                    }
                    if (logs.Count() < 10 )
                    {
                        for (int i = 0; i <logs.Count(); i++)
                        {
                            OracleVirtualMachine ovm = mgr.GetOVMById(logs[i].OvmId);
                            LogModel logmodel = new LogModel();
                            logmodel.Naam = orderLogs.ElementAt(i).Naam;
                            logmodel.ActionDate = orderLogs.ElementAt(i).ActionDate;
                            logmodel.Ovm = ovm.Naam;
                            logmodel.Gebruiker = k.Email;
                            model.Add(logmodel);
                        }
                    }
                }
            }
            OrderModel = model.OrderByDescending(m => m.ActionDate).ToList();
            //return Ok(OrderModel);
            if (!OrderModel.Any())
            {
                return Ok(false);
            }
            else
            {
                 OrderModel = model.OrderByDescending(m => m.ActionDate).ToList();
                try
                {
                    OrderModel.RemoveRange(9, OrderModel.Count() - 10);
                    return Ok(OrderModel);
                }
                catch
                {
                    return Ok(OrderModel);

                }
            }
            
        }
        [HttpGet]
        [Route("api/Klant/SSH/LogLijstUser")]
        [Authorize(Roles = "Admin , Klant")]
        public IHttpActionResult LogLijstUser(List<LogModel> model)
        {
            List<LogModel> OrderModel = new List<LogModel>();
            model = new List<LogModel>();
            Gebruiker user = userManager.GetGebruiker(User.Identity.GetUserName());
            List<LogLijst> logs = mgr.GetLogLijstsKlant(user.GebruikerId).ToList();
            if (logs.Count() != 0)
            {
                if (logs.Count() >= 10)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        OracleVirtualMachine ovm = mgr.GetOVMById(logs[i].OvmId);
                        LogModel logmodel = new LogModel();
                        logmodel.Gebruiker = user.Email;
                        logmodel.Naam = logs[i].Naam;
                        logmodel.ActionDate = logs[i].ActionDate;
                        logmodel.Ovm = ovm.Naam;
                        model.Add(logmodel);
                    }
                }
            }
            else
            {
                if (logs.Count() < 10)
                {
                    for (int i = 0; i < logs.Count(); i++)
                    {
                        OracleVirtualMachine ovm = mgr.GetOVMById(logs[i].OvmId);
                        LogModel logmodel = new LogModel();
                        logmodel.Gebruiker = user.Email;
                        logmodel.Naam = logs[i].Naam;
                        logmodel.ActionDate = logs[i].ActionDate;
                        logmodel.Ovm = ovm.Naam;
                        model.Add(logmodel);
                    }
                }
            }        
            if (!model.Any())
            {
                return Ok(false);
            }
            else
            {
                OrderModel = model.OrderByDescending(o => o.ActionDate).ToList(); ;
                return Ok(OrderModel);
            }
        }
        [HttpGet]
        [Route("api/Klant/SSH/LogLijstUser/{id}")]
        [Authorize(Roles = "Klant")]
        public IHttpActionResult LogLijstUser(int id, List<LogModel> model)
        {
            List<LogModel> OrderModel = new List<LogModel>();
            model = new List<LogModel>();
            Gebruiker user = userManager.GetGebruiker(id);
            List<LogLijst> logs = mgr.GetLogLijstsKlant(user.GebruikerId).ToList();
            if (logs.Count() != 0)
            {
                if (logs.Count() >= 10)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        OracleVirtualMachine ovm = mgr.GetOVMById(logs[i].OvmId);
                        LogModel logmodel = new LogModel();
                        logmodel.Naam = logs[i].Naam;
                        logmodel.ActionDate = logs[i].ActionDate;
                        logmodel.Ovm = ovm.Naam;
                        model.Add(logmodel);
                    }
                }
            }
            else
            {
                if (logs.Count() < 10)
                {
                    for (int i = 0; i < logs.Count(); i++)
                    {
                        OracleVirtualMachine ovm = mgr.GetOVMById(logs[i].OvmId);
                        LogModel logmodel = new LogModel();
                        logmodel.Naam = logs[i].Naam;
                        logmodel.ActionDate = logs[i].ActionDate;
                        logmodel.Ovm = ovm.Naam;
                        model.Add(logmodel);
                    }
                }
            }
            if (!model.Any())
            {
                return Ok(false);
            }
            else
            {
                OrderModel = model.OrderByDescending(o => o.ActionDate).ToList(); ;
                return Ok(OrderModel);
            }
        }
        [HttpGet]
        [Route("api/Klant/SSH/LogLijstAll")]
        [Authorize(Roles = "Admin , Klant")]
        public IHttpActionResult LogLijstAll(List<LogModel> model)
        {
            Gebruiker user = userManager.GetGebruiker(User.Identity.GetUserName());
            if(user.Rol.Equals(RolType.Admin))
            {
                List<LogLijst> logs = mgr.GetLogLijsten().ToList();
                for (int i = 0; i < logs.Count(); i++)
                {
                    OracleVirtualMachine ovm = mgr.GetOVMById(logs[i].OvmId);
                    Gebruiker userlog = userManager.GetGebruiker(logs[i].GebruikerId);
                    LogModel logmodel = new LogModel();
                    logmodel.Naam = logs[i].Naam;
                    logmodel.ActionDate = logs[i].ActionDate;
                    logmodel.Ovm = ovm.Naam;
                    logmodel.Naam = userlog.Naam;
                    model.Add(logmodel);
                }
            }
            else
            {
                Klant k = klantmgr.GetKlant(user.GebruikerId);
                List<LogLijst> logs = mgr.GetLogLijstsKlant(k.KlantId).ToList();
                List<Klant> medewerkers = klantmgr.GetKlantenAccounts(k).ToList();
                for (int i = 0; i < logs.Count(); i++)
                {
                    OracleVirtualMachine ovm = mgr.GetOVMById(logs[i].OvmId);
                    LogModel logmodel = new LogModel();
                    logmodel.Naam = logs[i].Naam;
                    logmodel.ActionDate = logs[i].ActionDate;
                    logmodel.Ovm = ovm.Naam;
                    model.Add(logmodel);
                }
                for (int i = 0; i < medewerkers.Count(); i++)
                {
                    List<LogLijst> logsM = mgr.GetLogLijstsKlant(medewerkers[i].KlantId).ToList();
                    for (int j = 0; j < logsM.Count(); j++)
                    {
                        OracleVirtualMachine ovm = mgr.GetOVMById(logsM[j].OvmId);
                        LogModel logmodel = new LogModel();
                        logmodel.Naam = logsM[j].Naam;
                        logmodel.ActionDate = logsM[j].ActionDate;
                        logmodel.Ovm = ovm.Naam;
                        model.Add(logmodel);
                    }
                }
            }
            if (!model.Any())
            {
                return Ok(false);
            }
            else
            {
                return Ok(model);
            }
        }

        [HttpGet]
        [Route("api/Klant/SSH/PushDowntime/{id}/{duur}")]
        [Authorize(Roles = "Admin , Klant, KlantAccount")]
        public IHttpActionResult PushDowntime(string id,int duur)
        {
            DateTime start_time = DateTime.Now;
            DateTime end_time = DateTime.Now.AddMinutes(duur);
            Gebruiker user = userManager.GetGebruiker(User.Identity.GetUserName());
            OracleVirtualMachine ovm = mgr.GetOVM(id);
            var client = new RestClient("https://api.monitoring.be/command/prod/op5command");
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-api-key", "ZCeD4fSfqR8GeEJU4jGv43muowCGTybIabBVTpcK");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"$\":[{\n \"method\": \"POST\",\n \"endpoint\": \"command/SCHEDULE_HOST_DOWNTIME\",\n \"data\": {\n  \"host_name\": \"TEST-"+ovm.Naam+"\",\n  \"start_time\":"+start_time+",\n  \"end_time\":"+end_time+",\n\t\"fixed\": true,\n  \"comment\": \"MONIN-PORTAL: automatic downtime for "+ovm.Naam+" by "+user.Naam+"\",\n\t\"trigger_id\": 0,\n\t\"duration\": \"none\"\n\t}\n}]\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return Ok();
        }

        [HttpGet]
        [Route("api/Klant/SSH/ScheduleDowntime/{id}/{start}/{end}")]
        [Authorize(Roles = "Admin , Klant, KlantAccount")]
        public IHttpActionResult ScheduleDowntime(string id, string start, string end)
        {
            DateTime start_time=makeDateTime(start);
            DateTime end_time = makeDateTime(end);
            mgr.AddScheduledDT(id, start_time, end_time,User.Identity.Name); 
            // DateTime start_time = Convert.ToDateTime(start);
            //DateTime end_time = Convert.ToDateTime(eind);
            Gebruiker user = userManager.GetGebruiker(User.Identity.GetUserName());
            OracleVirtualMachine ovm = mgr.GetOVM(id);
            var client = new RestClient("https://api.monitoring.be/command/prod/op5command");
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-api-key", "ZCeD4fSfqR8GeEJU4jGv43muowCGTybIabBVTpcK");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"$\":[{\n \"method\": \"POST\",\n \"endpoint\": \"command/SCHEDULE_HOST_DOWNTIME\",\n \"data\": {\n  \"host_name\": \"TEST-" + ovm.Naam + "\",\n  \"start_time\":" + start_time + ",\n  \"end_time\":" + end_time + ",\n\t\"fixed\": true,\n  \"comment\": \"MONIN-PORTAL: automatic downtime for " + ovm.Naam + " by " + user.Naam + "\",\n\t\"trigger_id\": 0,\n\t\"duration\": \"none\"\n\t}\n}]\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return Ok();
        }
        [HttpGet]
        [Route("api/Klant/SSH/GetDowntime/{id}")]
        [Authorize(Roles = "Admin , Klant, KlantAccount")]
        public IHttpActionResult GetDowntime(string id)
        {
            List<ScheduledDownTime> LijstDT = mgr.GetScheduledDTByOvm(id);
            List<ScheduledDownTime> Order = (LijstDT.OrderBy(x => x.Start)).OrderBy(x=>x.Eind).ToList();
            return Ok(Order);
        }
        
        public DateTime makeDateTime(string ToFormat)
        {
            string date = ToFormat.Substring(0, 8);
            string time = ToFormat.Substring(8, 4);
            string year = date.Substring(0, 4);
            string month= date.Substring(4, 2);
            string day= date.Substring(6, 2);
            string hour = time.Substring(0, 2);
            string minutes = time.Substring(2, 2);
            string datetime = day + "/" + month + "/" + year + " " + hour + ":" + minutes + ":00";
            DateTime FullDate = Convert.ToDateTime(datetime);
            return FullDate;
        }
    }
}