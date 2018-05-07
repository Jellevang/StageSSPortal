using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageSSPortal.Models
{
    public class SSHModel
    {
        public List<string> Vms { get; set; }

        public SSHModel()
        {
            Vms = new List<string>();
        }
    }
}