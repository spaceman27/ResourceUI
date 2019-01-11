using System;
using System.Collections.Generic;
using System.Linq;
using RMAObjects;

namespace RMAUI.Models
{
    public class ScheduleprojectView
    {       
        public List<ProjectSchedule> Schedule { get; set; }
    }

    public class ScheduleFilterModel
    {
        public string Project { get; set; }
        public string Status { get; set; }
        public DateTime PlannedDateFrom { get; set; }
        public DateTime PlannedDateTo { get; set; }
        public DateTime DueDateFrom { get; set; }
        public DateTime DueDateTo { get; set; }
        public int PlannedHourFrom { get; set; }
        public int PlannedHourTo { get; set; }
        public string[] Skills { get; set; }
    }

    public class GetStatusModel
    {
        public string Status { get; set; }
    }

    public class SkillSetModel
    {
        public string Category { get; set; }
        public string SkillName { get; set; }        
    }


    public class Skill
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
        public List<Skill> skill { get; set; }
    }

    public class SkillsList
    {
        public string name { get; set; }
        public List<Category> category { get; set; }
    }

    public class RootObject
    {
        public SkillsList SkillsList { get; set; }
    }

}