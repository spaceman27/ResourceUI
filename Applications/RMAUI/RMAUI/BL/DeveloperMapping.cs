using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RMADal;
using RMAObjects;
using RMAUI.Models;

namespace RMAUI.BL
{
    public class DeveloperMapping
    {
        private readonly MongoDBContext context = new MongoDBContext();

        public List<DeveloperAssignmentModel> GetDeveloperAssignmentViewDataByUsername(string currentUser)
        {
            
            var userCollection = context.GetCollection<UserModel>(Enums.Collection.Users);
            var devNames = userCollection.AsQueryable().AsEnumerable().Select(r => new
            {
                username = r.Username.ToLowerInvariant(),
                displayname = r.Name
            }).Where(x => x.username == currentUser.ToLowerInvariant()).ToList();

            return GetDeveloperAssignmentViewDataByNames(new List<string> {devNames[0].displayname});
        }

        public List<DeveloperAssignmentModel> GetDeveloperAssignmentViewDataByNames(List<string> devs)
        {
            if (devs == null || devs.Count==0)
            {
                return null;
            }
            var devDict = new Dictionary<string, int>();
            int count = 1;
            foreach (var dev in devs)
            {
                devDict[dev.ToLowerInvariant()] = count++;
            }

            var now = DateTime.Now.AddMonths(-3);
            var collection = context.GetCollection<Assignment>(Enums.Collection.Assignments);
            var results = collection.AsQueryable().AsEnumerable()
                .Select(r => new DeveloperAssignmentModel()
                {
                    title = r.EmployeeName,
                    start = new DateTime(r.Year, DateTime.ParseExact(r.Month, "MMM", CultureInfo.InvariantCulture).Month, r.Day, 9,0,0),
                    end = new DateTime(r.Year, DateTime.ParseExact(r.Month, "MMM", CultureInfo.InvariantCulture).Month, r.Day, 17, 0, 0),
                    assignedBy = r.AssignedBy,
                    isAllDay = true,
                    owner = r.EmployeeName,
                    roomId = devDict.ContainsKey(r.EmployeeName.ToLowerInvariant()) ? devDict[r.EmployeeName.ToLowerInvariant()] : 0
                })
                .Where(r => r.start.CompareTo(now) >= 0 && devDict.ContainsKey(r.title.ToLowerInvariant()))
                .ToList();
            for (int i = 0; i < results.Count; i++)
            {
                results[i].id = i+1;
            }
            return results;
        }

        public List<string> GetEmployeeNameHasAssignment()
        {
            var collection = context.GetCollection<Assignment>(Enums.Collection.Assignments);
            List<string> list = collection.AsQueryable().Select(r => r.EmployeeName).Distinct().ToList();
            return list;
        }
    }
}