using System;
using MongoDB.Driver;
using RMAObjects;

namespace RMADal
{
    using Properties;
    public class MongoDBContext
    {
        IMongoDatabase Database;        
        public MongoDBContext()
        {
           //var connectionString = Settings.Default.MongoDBConnectionString;
           var connectionString = Settings.Default.ServerMongoDBConnectionString;
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(Settings.Default.DbName);
        }

        public IMongoCollection<Project> Projects
             => Database.GetCollection<Project>("Projects");
        public IMongoCollection<ProjectSchedule> ProjectSchedules
             => Database.GetCollection<ProjectSchedule>("ProjectSchedules");
        public IMongoCollection<Employee> Employees
             => Database.GetCollection<Employee>("Employees");
        public IMongoCollection<Assignment> Assignments
             => Database.GetCollection<Assignment>("Assignments");
        public IMongoCollection<AssignmentStatus> AssignmentStatus 
             => Database.GetCollection<AssignmentStatus>("AssignmentStatus");

        public IMongoCollection<T> GetCollection<T>(Enums.Collection collectionName)
        {
            return Database.GetCollection<T> (Enum.GetName(typeof(Enums.Collection), collectionName));
        }

        public void DropCollection(Enums.Collection collectionName)
        {
            Database.DropCollection(Enum.GetName(typeof(Enums.Collection), collectionName));
        }
    }

}
