﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSPortalWebApi.Models
{
    public class VmModel
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string id { get; set; }
        public int KlantId { get; set; }
        public int Serverid { get; set; }

    }
}