using MongoDB.Driver;
using RMAObjects;

namespace RMADal
{
    public class ProjectsFilter
    {
        public string Title { get; set; }
        public string ProjectType { get; set; }
        public string Role { get; set; }
        public string Person { get; set; }
        public string Location { get; set; }
        public string PM { get; set; }
        public string Month { get; set; }
        public string Quarter { get; set; }
        public FilterDefinition<Project> ToProjectsFilterDefinition()
        {
            var filterDefinition = Builders<Project>.Filter.Empty; 
          
            if (Title != null) filterDefinition &= Builders<Project>.Filter.Eq(r => r.Title, Title);
            if (ProjectType != null) filterDefinition &= Builders<Project>.Filter.Eq(r => r.ProjectType, ProjectType);
            if (Role != null) filterDefinition &= Builders<Project>.Filter.Eq(r => r.Role, Role);
            if (Person != null) filterDefinition &= Builders<Project>.Filter.Eq(r => r.Person, Person);
            if(Location != null) filterDefinition &= Builders<Project>.Filter.Eq(r => r.Location, Location);
            if(PM != null) filterDefinition &= Builders<Project>.Filter.Eq(r => r.PM, PM);
            if(Month != null) filterDefinition &= Builders<Project>.Filter.Eq(r => r.Month, Month);
            if(Quarter != null) filterDefinition &= Builders<Project>.Filter.Eq(r => r.Quarter, Quarter);
            
            return filterDefinition;
        }
    }

    public class EmployeesFilter
    {
        public string Name { get; set; }
        public string ManagerName { get; set; }

        public FilterDefinition<Employee> ToEmployeesFilterDefinition()
        {
            var filterDefinition = Builders<Employee>.Filter.Empty;

            if (Name != null) filterDefinition &= Builders<Employee>.Filter.Eq(r => r.EmployeeName, Name);

            return filterDefinition;
        }
    }

    public class ProjectSchedulingFilters
    {
        // write project schduling filters
    }
}
