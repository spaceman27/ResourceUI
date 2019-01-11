using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace RMAObjects
{
    public class Employee
    {
        public string EmployeeName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SAMAccountName { get; set; }
        public string DistinguishedName { get; set; }
        [BsonId]
        public string UserPrincipalName { get; set; }
        public string Department { get; set; }
        public string Company { get; set; }
        public string Title { get; set; }
        public string ManagerName { get; set; }
        public bool IsDeveloper { get; set; }

        [BsonDefaultValue("")]
        [BsonIgnoreIfDefault]
        public List<string> SkillSet { get; set; }
    }
    public class EmployeeModel
    {
        public string Name { get; set; }
    }
}
