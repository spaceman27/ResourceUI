using System;
using System.Collections.Generic;
using System.Linq;
using RMAObjects;

namespace RMAUI.Models
{
    public class DeveloperAssignmentModel
    {
        public int id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string title { get; set; }
        public bool isAllDay { get; set; }
        public int roomId { get; set; }
        public string assignedBy { get; set; }
        public string owner { get; set; }
    }
}