using System;
using MongoDB.Bson.Serialization.Attributes;

namespace RMAObjects
{
    public class Project
    {
        public string Title { get; set; }
        public string ProjectType { get; set; }
        public string Role { get; set; }
        public DateTime Date { get; set; }
        public int PlannedHours { get; set; }
        public string Person { get; set; }
        public string Location { get; set; }
        public string PM { get; set; }
        public string Comments { get; set; }
        public string Month { get; set; }
        public string Quarter { get; set; }
        public int Week { get; set; }
        [BsonId]
        public int ProjectId { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
    }
   
    public enum ProjectStatus
    {
        // To Be Added
        //    Intial Assignement
        //    Assignment Completed
        //    % Completed
        //    Testing
        //    Deployed
        //    Finished ..

    }
    public class ProjectsContext
    {
        public string Title { get; set; }
        public string ProjectType { get; set; }
        public string Role { get; set; }
        public int TotalPlannedHours { get; set; }
        public string Person { get; set; }
        public string Location { get; set; }
        public string PM { get; set; }
        public string Month { get; set; }
        public string Quarter { get; set; }
        public int Week { get; set; }
    }
}
