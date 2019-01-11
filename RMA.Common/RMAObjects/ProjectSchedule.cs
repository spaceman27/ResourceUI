using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;


namespace RMAObjects
{
    // Project Scheduling Document
    public class ProjectSchedule
    {
        [BsonId]
        public string Title { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime ExpectedCompletionDate { get; set; }
        public List<ProjectsPerTitle> projects { get; set; }
        public int TotalPlannedHours { get; set; }

        [BsonDefaultValue("")]
        [BsonIgnoreIfDefault]
        public List<string> RequiredSkills { get; set; }

        [BsonDefaultValue("")]
        [BsonIgnoreIfDefault]
        public List<string> ReferenceLink { get; set; }

        public DateTime ProjectDueDate { get; set; }
        public string ProjectStatus { get; set; }

    }

    //public class PlannedDate
    //{
    //    public DateTime PlannedStartDate { get; set; }
    //    public int Year { get; set; }
    //    public string Quarter { get; set; }
    //    public int Month { get; set; }
    //    public int Week { get; set; }
    //    public int FirstDayOfWeek { get; set; }
    //}

    public class ProjectsPerTitle
    {
        public int ProjectID { get; set; }
        public int Hours { get; set; }
        public DateTime spPlannedDate { get;  set;}
    }    

    public class ProjectScheduleContext
    {
        public string Title { get; set; }
        public string Person { get; set; }
        public int EmployeeHours { get; set; }
    }

}