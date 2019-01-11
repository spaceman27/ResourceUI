using System;
using System.Collections.Generic;
using System.Linq;
using RMAObjects;
using MongoDB.Driver;
using MongoDB.Driver.Linq;


namespace RMADal
{
    public class SPtoMongoDB
    {
        static MongoDBContext context = new MongoDBContext();

        private static async void UpdatePlannedHoursAndDates()
        {
            foreach (var ps in context.ProjectSchedules.AsQueryable())
            {
                var filter = Builders<ProjectSchedule>.Filter.Eq(p=>p.Title, ps.Title);
                var pSHoursAndDates = Scheduling.PSHoursAndDates(ps.projects);
                var update = Builders<ProjectSchedule>.Update
                                .Set(p => p.TotalPlannedHours, pSHoursAndDates.TotalPlannedHours)
                                .Set(p => p.PlannedStartDate, pSHoursAndDates.PlannedStartDate)
                                .Set(p => p.ExpectedCompletionDate,pSHoursAndDates.ExpectedCompletionDate)
                                .Set(p => p.ProjectDueDate, pSHoursAndDates.ProjectDueDate);
                await context.ProjectSchedules.UpdateOneAsync(filter, update);
            }
        }

        public static async void UpdateProjectSchedulesInMongoDB(List<Project> SpProjectsList)
        {
            var projectSchedulesCountInMongoDB = context.ProjectSchedules.Count(Builders<ProjectSchedule>.Filter.Empty);

            var devProjectsCollection = await context.Projects.AsQueryable().Where(p => p.Role == "Developer").ToListAsync();

            if (projectSchedulesCountInMongoDB == 0)
            {
                var projectSchedulesList = Scheduling.CreateAndPopulateProjectSchedules(devProjectsCollection);
                await context.ProjectSchedules.InsertManyAsync(projectSchedulesList.OrderBy(p=>p.Title));
            }
            else
            {
                foreach (var projectListItem in SpProjectsList)
                {
                    UpdateProjectSchedule(projectListItem);
                }
                UpdatePlannedHoursAndDates();
            }

        }

        private static async void UpdateProjectSchedule(Project projectListItem)
        {
            var ProjSch = await context.ProjectSchedules
                            .AsQueryable().Where(ps => ps.Title == projectListItem.Title).FirstOrDefaultAsync();
            if (ProjSch != null)
            {
                var builder = Builders<ProjectSchedule>.Filter;
                var filter = builder.Eq(p=> p.Title, projectListItem.Title);
                var projectsFilter = filter &
                                builder.ElemMatch(p => p.projects,
                                    Builders<ProjectsPerTitle>.Filter.Eq(p => p.ProjectID, projectListItem.ProjectId));
                if (await context.ProjectSchedules.Find(projectsFilter).AnyAsync())
                {
                    var update = Builders<ProjectSchedule>.Update
                                    .Set("projects.0.Hours", projectListItem.PlannedHours)
                                    .Set("projects.0.spPlannedDate", projectListItem.Date);
                    await context.ProjectSchedules.UpdateOneAsync(projectsFilter, update);
                }
                else
                {                   
                    var update = Builders<ProjectSchedule>.Update
                            .AddToSet(x => x.projects,
                                    new ProjectsPerTitle { Hours = projectListItem.PlannedHours,
                                        ProjectID = projectListItem.ProjectId,
                                        spPlannedDate = projectListItem.Date});
                    await context.ProjectSchedules.UpdateOneAsync(filter, update);
                }
            }
            else
            {
                CreateNewEntryInProjectSchedulingList(projectListItem);
            }
        }

        private static async void CreateNewEntryInProjectSchedulingList(Project projectListItem)
        {
            await context.ProjectSchedules.InsertOneAsync(new ProjectSchedule()
            {
                Title = projectListItem.Title,
                PlannedStartDate = projectListItem.Date,
                ExpectedCompletionDate = projectListItem.Date,
                projects =
                        new List<ProjectsPerTitle>()
                        {
                           new ProjectsPerTitle { ProjectID=projectListItem.ProjectId,
                               Hours = projectListItem.PlannedHours, spPlannedDate=projectListItem.Date }
                        },
                TotalPlannedHours = projectListItem.PlannedHours,
                RequiredSkills = new List<string> { "Skill1", "Skill2" },
                ReferenceLink = new List<string> { "Link1", "Link2" },
                ProjectDueDate = projectListItem.Date,
                ProjectStatus = ""
            });
        }

        public static async void UpdateProjectsInMongoDB(List<Project> SpProjectsList)
        {
            var projectsCountInMongoDB = context.Projects.Count(Builders<Project>.Filter.Empty);

            if (projectsCountInMongoDB == 0)
            {
                await context.Projects.InsertManyAsync(SpProjectsList);
            }
            else
            {
                foreach (var projectListItem in SpProjectsList)
                {
                    var project = context.Projects.AsQueryable().
                        FirstOrDefault(p => p.ProjectId == projectListItem.ProjectId);

                    if (project == null)
                    {
                        await context.Projects.InsertOneAsync(projectListItem);
                    }

                    else if (DateTime.Compare(projectListItem.Modified, project.Modified) > 0)
                    {
                        await context.Projects.ReplaceOneAsync(p => p.ProjectId == projectListItem.ProjectId, projectListItem);
                    }
                }

                UpdateProjectSchedulesInMongoDB(SpProjectsList);
            }

            UpdateProjectSchedulesInMongoDB(SpProjectsList);
        }

        public static async void UpdateEmployeesInMongoDB(List<Employee> LdapEmployeeList)
        {
            var developerList = LdapEmployeeList.Where(e => e.IsDeveloper == true);
            if (developerList.Count() != 0)
            {

                var employeeCountInMongoDB = context.Employees.Count(Builders<Employee>.Filter.Empty);

                if (employeeCountInMongoDB == 0)
                {
                    await context.Employees.InsertManyAsync(developerList);
                }

                foreach (var employeeListItem in developerList)
                {
                    var employee = context.Employees.AsQueryable().
                        FirstOrDefault(e => e.UserPrincipalName == employeeListItem.UserPrincipalName);

                    if (employee == null)
                    {
                        context.Employees.InsertOne(employeeListItem);
                    }
                }
            }
        }

    }
}