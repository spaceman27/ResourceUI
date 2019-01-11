using System;
using System.Collections.Generic;
using System.Linq;
using RMAObjects;

namespace RMAUI.Models
{
    public class UpdateGroupModel
    {
        public string GroupName { get; set; }
        
        public string ModuleName { get; set; }
        public UserRight UserRight { get; set; }
    }

    public class UpdateUserModel
    {
        public string UserName { get; set; }
        public string ModuleName { get; set; }
        public UserRight UserRight { get; set; }
    }
}