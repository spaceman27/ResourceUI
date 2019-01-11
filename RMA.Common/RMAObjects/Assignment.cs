using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;




namespace RMAObjects
{

    public class Assignment
    {
        [BsonId]
        public MongoDB.Bson.ObjectId _id { get; set; }
        public string EmployeeName { get; set; }
        public string ProjectTitle { get; set; }
        public int Hours { get; set; }
        public int Year { get; set; }
        public string Month { get; set; }
        public int WeekNumber { get; set; }
        public int Day { get; set; }
        public string DayOfWeek { get; set; }
        public string AssignedBy { get; set; }
        public string Week { get; set; }
        public string AssignmentOption { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime AssignmentDate { get; set;}

    }

  
    public class AssignmentStatus
    {
        public int ProjectTotalPlannedHours { get; set; }
        public int ProjectTotalPlannedDays { get; set; }
        public int ProjectTotalAssignedHours { get; set; }
        public int ProjectTotalAssignedDays { get; set; }
        public string ProjectTitle { get; set; }
        public int RemainingHours { get; set; }
        public int RemainingDays { get; set; }
        public bool IsCompletelyAssigned { get; set; }
    }

    public class AssignmentInfoByProjectContext
    {
        public string ProjectTitle { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeAssignedHours { get; set; }
    }

    public class AssignmentByEmployee
    {
        public string EmployeeName { get; set; }
        public Dictionary<DateTime, Assignment> Assignments { get; set; }
    }

    public class AssignmentSaveModel
    {
        public AssignmentShowModel ShowData { get; set; }
        public List<Assignment> Assignments { get; set; }

    }

    public class AssignmentShowModel
    {
        public List<string> EmployeeNames { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
    
}
