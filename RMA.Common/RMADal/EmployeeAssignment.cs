using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RMAObjects;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System.Globalization;

namespace RMADal
{

    public class EmployeeAssignment
    {

        static MongoDBContext context = new MongoDBContext();
        public static List<AssignmentInfoByProjectContext> GetAssignmentInfoByProject()
        {
            return context.Assignments.AsQueryable()
                .GroupBy(a => new
                {
                    a.ProjectTitle,
                    a.EmployeeName
                })
                .Select(g => new AssignmentInfoByProjectContext
                {
                    ProjectTitle = g.Key.ProjectTitle,
                    EmployeeName = g.Key.EmployeeName,
                    EmployeeAssignedHours = g.Sum(a => a.Hours)
                }).ToList();
        }
        public static List<AssignmentStatus> GetProjectsAndAssignmentsInfo(AssignmentShowModel model)
        {
            var noOfHoursInAWorkingDay = 8;
            List<Project> projectsCollection;
            if (model.Year == 0 && model.Month == 0)
            {
                projectsCollection = context.Projects.AsQueryable().Where(p => p.Role == "Developer").ToList();
            }
            else
            {
                DateTime firstDayOfMonth = new DateTime(model.Year, model.Month, 1);
                DateTime lastDayOfTheMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                projectsCollection = context.Projects.AsQueryable().Where(p => p.Role == "Developer" && p.Date >= firstDayOfMonth && p.Date <= lastDayOfTheMonth).ToList();
            }
            List<string> UniqueProjectsNameList = projectsCollection.Select(li => li.Title).Distinct().ToList();
            List<AssignmentStatus> collectionOfAssignmentStatus = new List<AssignmentStatus>();

            foreach (var projectName in UniqueProjectsNameList)
            {
                AssignmentStatus status = new AssignmentStatus()
                {
                    ProjectTitle = projectName,
                    ProjectTotalPlannedHours = GetNearestWholeMultipleOfEight(
                    projectsCollection
                    .FindAll(p => p.Title == projectName)
                    .Select(p => p.PlannedHours).Sum()
                    ),
                    ProjectTotalAssignedHours = context.Assignments.AsQueryable().
                    Where(p => p.ProjectTitle == projectName).Select(p => p.Hours).Sum()
                };

                status.RemainingHours = status.ProjectTotalPlannedHours >= status.ProjectTotalAssignedHours
                    ? status.ProjectTotalPlannedHours - status.ProjectTotalAssignedHours
                    : -999;
                status.IsCompletelyAssigned = status.RemainingHours == 0;

                status.ProjectTotalPlannedDays = status.ProjectTotalPlannedHours / noOfHoursInAWorkingDay;
                status.ProjectTotalAssignedDays = status.ProjectTotalAssignedHours / noOfHoursInAWorkingDay;

                status.RemainingDays = status.ProjectTotalPlannedDays - status.ProjectTotalAssignedDays;

                collectionOfAssignmentStatus.Add(status);
            }

            return collectionOfAssignmentStatus;
        }
        public static int GetNearestWholeMultipleOfEight(int input, int multipleOf = 8)
        {
            var output = (input % multipleOf);

            if (output != 0 && input > 0)
                output = ((input / multipleOf) + 1) * multipleOf;
            else
                output = input;

            return output;
        }
        public static List<Employee> CreateAndPopulateEmployeeInformationCollection()
        {
            List<Employee> ldapEmpyoleeCollection =
                context.Employees.AsQueryable().Where(e => e.IsDeveloper == true).ToList();
            var employeeNames = ldapEmpyoleeCollection.Select(emp => emp.DistinguishedName).Distinct().ToList();
            var EmployeeInfoCollection = new List<Employee>();
            foreach (var name in employeeNames)
            {
                var employeeDataObj = ldapEmpyoleeCollection.FirstOrDefault(e => e.DistinguishedName == name);
                EmployeeInfoCollection.Add(CreateEmployeeInformationDocument(employeeDataObj));
            }

            return EmployeeInfoCollection;

        }
        static Employee CreateEmployeeInformationDocument(Employee ldapInfo)
        {
            // to be added
            return new Employee();
        }
        public static async void InsertManyEmployeesIntoCollection(List<Employee> employees)
        {
            if (context.Employees.Count(new BsonDocument()) == 0)
                await context.Employees.InsertManyAsync(employees);
        }
        public static async void InsertOneEmployeeIntoCollection(Employee employee)
        {
            await context.Employees.InsertOneAsync(employee);
        }
        public async Task<List<Employee>> GetEmployeeInfo()
        {
            return await context.Employees.Find(new BsonDocument()).ToListAsync();

        }
        public async Task<Employee> GetEmployeeInfo(string id)
        {
            var employee =
                await context.Employees.Find(e => e.EmployeeName == id).FirstOrDefaultAsync();
            return employee;
        }
        public async void UpdateEmployeeInfo(Employee emp)
        {

            await context.Employees.ReplaceOneAsync(e => e.EmployeeName == emp.EmployeeName, emp);
        }
        public static List<Assignment> GetAssignmentsInfo(List<string> employeeNames, int year, int month)
        {
            // get assignment from first monday of the date(year/month/1) to next 4 monday
            var firstDay = DateTime.Parse(String.Format("{0}/{1}/{2}", year, month, 1));
            DateTime firstMonday = new DateTime();
            switch (firstDay.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    firstMonday = firstDay;
                    break;
                case DayOfWeek.Sunday:
                    firstMonday = firstDay.AddDays(1);
                    break;
                default:
                    firstMonday = firstDay.AddDays(-((int)firstDay.DayOfWeek - 1));
                    break;
            }
            var lastMonday = firstMonday.AddDays(36);
            var list =
                context.Assignments.AsQueryable().ToList()
              .Where(r => r.AssignmentDate >= firstMonday && r.AssignmentDate <= lastMonday && employeeNames.Contains(r.EmployeeName)).ToList();
            return list;
        }
        public static void SaveAssignmentsInfo(List<Assignment> AssignmentInfo)
        {
            if (AssignmentInfo != null)
            {
                foreach (var assignment in AssignmentInfo)
                {
                    var builder = Builders<Assignment>.Filter;
                    var filter = builder.Eq(a => a.EmployeeName, assignment.EmployeeName)
                                & builder.Eq(a => a.Year, assignment.Year)
                                & builder.Eq(a => a.Month, assignment.Month)
                                & builder.Eq(a => a.WeekNumber, assignment.WeekNumber)
                                & builder.Eq(a => a.Day, assignment.Day)
                                & builder.Eq(a => a.DayOfWeek, assignment.DayOfWeek)
                                & builder.Eq(a => a.Week, assignment.Week);
                    var fullDocumentFilter = filter
                               & builder.Eq(a => a.ProjectTitle, assignment.ProjectTitle)
                               & builder.Eq(a => a.AssignedBy, assignment.AssignedBy)
                               & builder.Eq(a => a.AssignmentOption, assignment.AssignmentOption);
                    if (context.Assignments.Find(fullDocumentFilter).Count() == 0)
                    {
                        var update = Builders<Assignment>.Update
                            .Set(a => a.ProjectTitle, assignment.ProjectTitle)
                            .Set(a => a.Hours, 8)
                            .Set(a => a.Year, assignment.Year)
                            .Set(a => a.Month, assignment.Month)
                            .Set(a => a.WeekNumber, assignment.WeekNumber)
                            .Set(a => a.Day, assignment.Day)
                            .Set(a => a.DayOfWeek, assignment.DayOfWeek)
                            .Set(a => a.AssignedBy, assignment.AssignedBy)
                            .Set(a => a.Week, assignment.Week)
                            .Set(a => a.AssignmentOption, assignment.AssignmentOption)
                            .Set(a => a.AssignmentDate,
                                       new DateTime(assignment.Year,
                                           DateTime.ParseExact(assignment.Month, "MMM", CultureInfo.InvariantCulture).Month,
                                           assignment.Day))
                            .CurrentDate(a => a.LastModified);
                        var updateOptions = new UpdateOptions { IsUpsert = true };
                        context.Assignments.UpdateOne(filter, update, updateOptions);
                    }
                }
            }

            else
            {
                // do not perform save operations ??
                // UI Check before save method call
            }
        }




        public static void DeleteAssignmentsInfo(List<Assignment> AssignmentInfo)
        {
            try
            {
                if (AssignmentInfo != null)
                {
                    foreach (var assignment in AssignmentInfo)
                    {
                        var builder = Builders<Assignment>.Filter;
                        var filter = builder.Eq(a => a.EmployeeName, assignment.EmployeeName)
                                    & builder.Eq(a => a.Year, assignment.Year)
                                    & builder.Eq(a => a.Month, assignment.Month)
                                    & builder.Eq(a => a.WeekNumber, assignment.WeekNumber)
                                    & builder.Eq(a => a.Day, assignment.Day)
                                    & builder.Eq(a => a.DayOfWeek, assignment.DayOfWeek)
                                    & builder.Eq(a => a.Week, assignment.Week);
                        context.Assignments.DeleteOne(filter);
                    }

                }

                else
                {
                    // do not perform save operations ??
                    // UI Check before save method call
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
