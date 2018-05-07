
using Renci.SshNet;
using SSPortalWebApi.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace WebAPI.Controllers
{
    public class SSHController : ApiController
    {
        SshClient ssh;
        public SSHController()
        {
            ssh = new SshClient("10.0.12.240", 10000, "admin", "tst0VMman");

        }

        public string[] GetInfo(string vmId, SshClient ssh, string[] vmInfo2)
        {
            vmInfo2 = new string[7];
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
            return vmInfo2;
        }
        [HttpGet]
        [Route("Home/api/Ssh/Info/{id}")]
        public IHttpActionResult GetInfo(string id)
        {
            //List<String> Info = new List<string>();
            string[] Info = new string[7];
            using (ssh)
            {
                Info = GetInfo(id, ssh, Info);
            }
            return Ok(Info);

        }
        [HttpGet]
        [Route("Home/api/Ssh/Vms")]
        public IHttpActionResult GetVms(List<VmModel> model)
        {
            model = new List<VmModel>();

            //VmModel vmModel = new VmModel();
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
                //2
                ssh.Connect();
                var VmResult = ssh.RunCommand("list vm ");
                VmNames = VmResult.Result.Split('\n');
                ssh.Disconnect();

                string patternVM = @"Vm [0-9]+";
                string idPattern = @"id: [0-9]+";

                //foreach (string line in serverVMs)
                for (int i = 0; i < serverVMs.Length; i++)
                {
                    var res = Regex.Match(serverVMs[i], patternVM);
                    var resId = Regex.Match(serverVMs[i], idPattern);
                    if (res.Length != 0)
                    {
                        VmModel vmModel = new VmModel();
                        string name = serverVMs[i].Substring(serverVMs[i].IndexOf("[") + 1, serverVMs[i].IndexOf("]") - serverVMs[i].IndexOf("[") - 1);
                        string id = serverVMs[i].Substring(serverVMs[i].IndexOf("=") + 1, (serverVMs[i].IndexOf("[")) - serverVMs[i].IndexOf("=") - 1);
                        vmModel.Name = name;
                        vmModel.id = id;
                        //if (name.Contains(" ") || name.Contains("."))
                        //{
                        //    vmModel.id = name.Replace(" ", "-").Replace(".", "-");
                        //}
                        //else
                        //{
                        //    vmModel.id=name;
                        //}
                        vmInfo = GetInfo(name, ssh, getVmInfo);
                        var regex = @"Status = [A-Z]+";
                        //foreach (var line2 in vmInfo)
                        for (int j = 0; j < vmInfo.Length; j++)
                        {
                            var match = Regex.Match(vmInfo[j], regex);
                            if (match.Length != 0)
                            {


                                vmModel.Status = vmInfo[j].Substring(vmInfo[j].IndexOf("=") + 2, vmInfo[j].Length - vmInfo[j].IndexOf("=") - 2);

                                // vmModel.Name = name;


                            }
                            //vmModel.info.Add(line2);
                        }
                        model.Add(vmModel);

                    }
                }


            }
            //List<string> LijstVms = new List<string>();
            //string pattern = @"name:*\w";
            //foreach (string line in VmNames)
            //{
            //    var res = Regex.Match(line, pattern);
            //    if (res.Length != 0)
            //    {
            //        string cut = line.Substring(line.LastIndexOf(":") +2, line.Length - line.LastIndexOf(":") - 2);
            //        LijstVms.Add(cut);
            //    }
            //}
            //// Id's maken voor Vms zonder " " of .
            //List<string> ids = new List<string>();
            //foreach (var id in LijstServerVMs)
            //{
            //    if (id.Contains(" ") || id.Contains("."))
            //    {
            //        string test = id.Replace(" ", "-").Replace(".", "-");
            //        ids.Add(test);
            //    }
            //    else
            //    {
            //        ids.Add(id);
            //    }

            //}
            //ViewBag.Ids = ids;
            //ViewBag.VmInfo = vmInfo;
            //ViewBag.State = vmState;
            //ViewBag.VMs = LijstServerVMs;
            return Ok(model);

        }

        [HttpGet]
        [Route("Home/api/ssh/StopVm/{id}")]
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
        [Route("Home/api/ssh/StartVm/{id}")]
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
    }
}
