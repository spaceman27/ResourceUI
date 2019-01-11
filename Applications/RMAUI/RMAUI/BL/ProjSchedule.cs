using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RMADal;
using MongoDB.Driver;
using RMAObjects;
using MongoDB.Bson;
using RMAUI.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace RMAUI.BL
{
    public interface IProjSchedule
    {
        ScheduleprojectView getscheduleview();
        List<GetStatusModel> GetProjectStatus();
        ScheduleprojectView FilterBy(ScheduleFilterModel values);
        List<SkillSetModel> ParseSkillSetListFrom(RootObject obj);
    }
    public class ProjSchedule : IProjSchedule
    {
        public ProjSchedule()
        {

        }


        public virtual ScheduleprojectView getscheduleview()
        {
            MongoDBContext context = new MongoDBContext();
            var collection = context.ProjectSchedules;
            var list = collection.AsQueryable()
                .Select(w => new ProjectSchedule
                {
                    Title = w.Title,

                    PlannedStartDate = w.PlannedStartDate,

                    ExpectedCompletionDate = w.ExpectedCompletionDate,

                    projects = w.projects,

                    TotalPlannedHours = w.TotalPlannedHours,

                    RequiredSkills = w.RequiredSkills,

                    ReferenceLink = w.ReferenceLink,

                    ProjectDueDate = w.ProjectDueDate,

                    ProjectStatus = w.ProjectStatus

                }).ToList();
            ScheduleprojectView result = new ScheduleprojectView
            {
                Schedule = list
            };
            return result;

        }

        public List<GetStatusModel> GetProjectStatus() {
            return Enums.GetDisplayNames<Enums.ProjectStatus>().Select(r => new GetStatusModel { Status = r }).ToList();
        }

        

        public virtual ScheduleprojectView FilterBy(ScheduleFilterModel values)
        {
            MongoDBContext context = new MongoDBContext();
            var collection = context.ProjectSchedules;
            Func<ProjectSchedule, bool> filter = (p) =>
            (values.Project == null ? true : p.Title.Contains(values.Project)) &&
            (values.Status == "Select Status" ? true : values.Status == p.ProjectStatus) &&
            (values.PlannedDateFrom.Ticks == 0 ? true : p.PlannedStartDate >= values.PlannedDateFrom) &&
            (values.PlannedDateTo.Ticks == 0 ? true : p.PlannedStartDate <= values.PlannedDateTo) &&
            (values.DueDateFrom.Ticks == 0 ? true : p.ProjectDueDate >= values.DueDateFrom) &&
            (values.DueDateTo.Ticks == 0 ? true : p.ProjectDueDate <= values.DueDateTo) &&
            (values.Skills[0] == "Select Skill" ? true : p.RequiredSkills.Contains(values.Skills[0])) &&
            (values.PlannedHourFrom == 0 ? true : p.TotalPlannedHours >= values.PlannedHourFrom) &&
            (values.PlannedHourTo == 0 ? true : p.TotalPlannedHours <= values.PlannedHourTo);

            var list = collection.AsQueryable()
                .Select(w => new ProjectSchedule
                {
                    Title = w.Title,

                    PlannedStartDate = w.PlannedStartDate,

                    ExpectedCompletionDate = w.ExpectedCompletionDate,

                    projects = w.projects,

                    TotalPlannedHours = w.TotalPlannedHours,

                    RequiredSkills = w.RequiredSkills,

                    ReferenceLink = w.ReferenceLink,

                    ProjectDueDate = w.ProjectDueDate,

                    ProjectStatus = w.ProjectStatus

                })
                .Where(filter).ToList();            
            ScheduleprojectView result = new ScheduleprojectView
            {
                Schedule = list
            };
            return result;
        }

        public List<SkillSetModel> ParseSkillSetListFrom(RootObject obj) {
            var list = new List<SkillSetModel>();
            foreach(var item in obj.SkillsList.category)
            {
                foreach(var skill in item.skill)
                {
                    list.Add(new SkillSetModel
                    {
                        Category = item.name,
                        SkillName = skill.name
                    });
                }
            }
            return list;
        }
        
    }
}