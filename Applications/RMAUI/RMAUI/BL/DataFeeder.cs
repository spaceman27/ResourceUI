using System;
using System.Collections.Generic;
using System.Linq;
using RMAUI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using RMADal;
using RMAObjects;

namespace RMAUI.BL
{
    public interface IDataFeeder
    {
        List<ProjectViewModel> GetProjectScheduleViewData(DateTime start, DateTime end);
    }
    public class DataFeeder : IDataFeeder
    {
        //    IMongoClient _client;
        //    IMongoDatabase _database;


        /// <summary>
        /// Init database context
        /// </summary>
        public DataFeeder()
        {
            //_client = new MongoClient();
            //_database = _client.GetDatabase("MRP");
        }
        public List<ProjectViewModel> GetProjectScheduleViewData(DateTime startDayOfMonth, DateTime maxDay)
        {
            List<ProjectViewModel> returnResults = new List<ProjectViewModel>();
            MongoDBContext context = new MongoDBContext();
            var collection = context.Projects;
            var results = collection.AsQueryable()
                .Select(r => new Project
                {
                    ProjectId = r.ProjectId,
                    ProjectType = r.ProjectType,
                    Title = r.Title,
                    Date = r.Date,
                    PlannedHours = r.PlannedHours                    
                })
                .Where(r => r.Date.CompareTo(startDayOfMonth) >= 0 && r.Date.CompareTo(maxDay.AddDays(1)) <= 0)
                .OrderBy(r => r.ProjectType)
                .ThenBy(r => r.Title)
                .ToList();

            var listDateAndHours = new Dictionary<Tuple<string, string>, Dictionary<string, int>>();
            foreach (Project item in results)
            {
                string formatedDate = item.Date.ToString("MM/dd/yyyy");
                Tuple<string, string> keyLookup = Tuple.Create(item.ProjectType, item.Title);
                if (listDateAndHours.ContainsKey(keyLookup))
                {
                    if (listDateAndHours[keyLookup].ContainsKey(formatedDate))
                    {
                        listDateAndHours[keyLookup][formatedDate] += item.PlannedHours;
                    }
                    else
                    {
                        listDateAndHours[keyLookup][formatedDate] = item.PlannedHours;
                    }
                }
                else
                {
                    var initDict = new Dictionary<string, int>();
                    initDict.Add(formatedDate, item.PlannedHours);
                    listDateAndHours.Add(keyLookup, initDict);
                }
            }

            foreach (KeyValuePair<Tuple<string, string>, Dictionary<string, int>> item in listDateAndHours)
            {
                returnResults.Add(new ProjectViewModel
                {
                    ProjectType = item.Key.Item1,
                    Title = item.Key.Item2,
                    HourPerWeeks = item.Value
                });
            }
            return returnResults;
        }

        public DateTime GetStartDayInTheMonth(IMongoCollection<Project> collection)
        {
            var now = DateTime.Now;
            DateTime startDayOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime endDayOfMonth = startDayOfMonth.AddMonths(1).AddDays(-1);

            // get first day in dataset of the month
            var resultsStartDay = collection.AsQueryable()
                .Select(r => new Project
                {
                    ProjectId = r.ProjectId,
                    ProjectType = r.ProjectType,
                    Title = r.Title,
                    Date = r.Date,
                    PlannedHours = r.PlannedHours
                })
                .Where(r => r.Date.CompareTo(startDayOfMonth) >= 0 && r.Date.CompareTo(endDayOfMonth) <= 0)
                .OrderBy(r => r.Date)
                .Take(1)
                .ToList();

            return resultsStartDay[0].Date;
        }
    }
}