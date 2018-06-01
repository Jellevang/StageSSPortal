using BL;
using Domain;
using Quartz;
using StageSSPortal.Controllers.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageSSPortal.Job
{
    public class DownTimeJob : IJob
    {
        SSHController ctr = new SSHController();
        SSHManager mgr = new SSHManager();
        public void Execute(IJobExecutionContext ctx)
        {
            List<ScheduledDownTime> SDTList= mgr.GetScheduledDT();
            foreach (var f in SDTList)
            {
                if (f.Eind < DateTime.Now)
                {
                    mgr.RemoveScheduledDT(f);
                }
                var start = f.Start - DateTime.Now;
                if (start.TotalSeconds < 0 && start.TotalSeconds > -5)
                {
                    ctr.StopVm(f.OvmId);
                }
                else
                {
                    var stop = f.Eind - DateTime.Now;
                    if (stop.TotalSeconds < 40 && stop.TotalSeconds >30)
                    {
                        ctr.StartVm(f.OvmId);
                    }
                }
            }
        }
    }
}