using System;
using System.Collections.Generic;
using System.Linq;
using RMAObjects;
using MongoDB.Driver;
using MongoDB.Driver.Linq;


namespace RMADal
{
    public class Scheduling
    {
        Project splist = new Project();

        static MongoDBContext context = new MongoDBContext();

        static List<Project> projectsCollection = context.Projects.AsQueryable().Where(p => p.Role == "Developer").ToList();

        public static List<string> GetAssignmentOptions()
        {
            return Enums.GetDisplayNames<Enums.AssignmentOptions>();
        }

        public static List<EmployeeModel> GetEmployeeNames()
        {
            return context.Employees.AsQueryable().Select(e => new EmployeeModel { Name = e.EmployeeName}).Distinct().ToList();
        }

        public static List<string> GetProjectList()
        {
            return projectsCollection.OrderByDescending(p => p.Title)
                .Select(li => li.Title).Distinct().ToList();
        }

        public static List<ProjectScheduleContext> GetProjectsByEmployeeAndPlannedHours()
        {
            return projectsCollection.GroupBy(p => new { p.Title, p.Person }).Select(pc =>
                new ProjectScheduleContext
                {
                    Title = pc.Key.Title,
                    Person = pc.Key.Person,
                    EmployeeHours = pc.Sum(h => h.PlannedHours)
                }).ToList();
        }

        public static List<ProjectsContext> GetProjectsByCriteria()
        {
            return projectsCollection.GroupBy(p =>
            new
            {
                p.ProjectType,
                p.Title,
                p.Month,
                p.Week
            }
            )
            .Select(pc =>
                new ProjectsContext
                {
                    ProjectType = pc.Key.ProjectType,
                    Title = pc.Key.Title,
                    Month = pc.Key.Month,
                    Week = pc.Key.Week,
                    TotalPlannedHours = pc.Sum(h => h.PlannedHours)
                }).OrderBy(p => p.Week).ToList();
        }

        public static List<ProjectSchedule> CreateAndPopulateProjectSchedules(List<Project> collection)
        {
            List<string> UniqueProjectsNameList = GetProjectList();
            ProjectSchedule projectSchedule = new ProjectSchedule();
            List<ProjectSchedule> collectionOfProjectSchedules = new List<ProjectSchedule>();
            foreach (var projectName in UniqueProjectsNameList)
            {
                List<Project> matchedCollection = collection.FindAll(p => p.Title == projectName).ToList();
                projectSchedule.projects = matchedCollection.
                                Select(p => new ProjectsPerTitle
                                {
                                    ProjectID = p.ProjectId,
                                    Hours = p.PlannedHours,
                                    spPlannedDate = p.Date.Date
                                }).ToList();
                collectionOfProjectSchedules.Add(CreateProjectScheduleDocument(collection
                    .FirstOrDefault(p => p.Title == projectName), projectSchedule.projects));
            }

            return collectionOfProjectSchedules;
        }

        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static DateTime AddWorkDays(DateTime date, int workingDays)
        {
            int direction = workingDays < 0 ? -1 : 1;
            DateTime newDate = date;
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.DayOfWeek != DayOfWeek.Saturday &&
                    newDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays -= direction;
                }
            }
            return newDate;
        }

        static ProjectSchedule CreateProjectScheduleDocument(Project project,
            List<ProjectsPerTitle> collection)
        {
            var psHoursAndDates = PSHoursAndDates(collection);

            try
            {
                return new ProjectSchedule
                {
                    Title = project.Title,
                    PlannedStartDate = psHoursAndDates.PlannedStartDate,
                    ExpectedCompletionDate = psHoursAndDates.ExpectedCompletionDate,
                    projects = collection,
                    TotalPlannedHours = psHoursAndDates.TotalPlannedHours,
                    RequiredSkills = new List<string> { "Skill 1", "Skill 2" },
                    ReferenceLink = new List<string> { "Link 1", "Link 2" },
                    ProjectDueDate = psHoursAndDates.ProjectDueDate,
                    ProjectStatus = ""
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error: " + ex.Message);
                return null;
            }
        }

        public static SchedulingHoursAndDates PSHoursAndDates(List<ProjectsPerTitle> collection)
        {
            //int hours = EmployeeAssignment
            //                .GetNearestWholeMultipleOfEight(collection.OrderByDescending(p => p.spPlannedDate).FirstOrDefault().Hours);
            //var date = AddWorkDays(collection.Max(p => p.spPlannedDate), hours / 8);
            var date = GetNextWeekday(collection.Max(p => p.spPlannedDate), DayOfWeek.Friday);
            return new SchedulingHoursAndDates
            {
                PlannedStartDate = collection.Min(p => p.spPlannedDate),
                ExpectedCompletionDate = date,
                TotalPlannedHours = collection.Sum(x => x.Hours),
                ProjectDueDate = date
            };
        }       

        public async void PostProjectScheduleDocument(ProjectSchedule ps)
        {
            await context.ProjectSchedules.InsertOneAsync(ps);
        }

    }

    public class SchedulingHoursAndDates
    {
        public DateTime PlannedStartDate { get; set; }
        public DateTime ExpectedCompletionDate { get; set; }
        public int TotalPlannedHours { get; set; }
        public DateTime ProjectDueDate { get; set; }
    }

}
