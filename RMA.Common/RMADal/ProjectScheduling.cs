using System.Collections.Generic;
using System.Linq;
using RMAObjects;
using MongoDB.Driver;

namespace RMADal
{
    public class ProjectScheduling
    {
        static MongoDBContext context = new MongoDBContext();
        public static void UpdateProjectSchedule(List<ProjectSchedule> projectsSchedulingInfo)
        {
            if (projectsSchedulingInfo != null)
            {
                foreach (var projectsSchedule in projectsSchedulingInfo)
                {
                    var builder = Builders<ProjectSchedule>.Filter;
                    var filter = builder.Eq(s => s.Title, projectsSchedule.Title);
                    if (context.ProjectSchedules.Find(filter).Any())
                    {
                        var update = Builders<ProjectSchedule>.Update
                            .Set(s => s.RequiredSkills, projectsSchedule.RequiredSkills)
                            .Set(s => s.ReferenceLink, projectsSchedule.ReferenceLink)
                            .Set(s => s.ProjectDueDate, projectsSchedule.ProjectDueDate)
                            .Set(s => s.ProjectStatus, projectsSchedule.ProjectStatus);
                        var updateOptions = new UpdateOptions { IsUpsert = true };
                        context.ProjectSchedules.UpdateOne(filter, update, updateOptions);
                    }
                    else
                    {

                    }
                }
            }

            else
            {
                // do not perform save operations ??
                // UI Check before save method call
            }
        }
    }
}
