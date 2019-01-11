using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;


namespace RMADal
{
    public class Enums
    {
        public enum Collection
        {
            Projects,
            ProjectSchedules,
            Employee,
            LDAPEmployeeInfo,
            Users,
            Groups,
            Modules,
            Assignments
        }
        public enum ProjectStatus
        {
            [Display(Name = "Not Assigned")]
            NotAssigned,
            [Display(Name = "Pending")]
            Pending,
            [Display(Name = "Completed")]
            Completed,
            [Display(Name = "In-Progress")]
            InProgress,
            [Display(Name = "Cancelled")]
            Cancelled
        }
        public enum Modules
        {
            [Display(Name = "Project Schedule")]
            ProjectSchedule,
            [Display(Name = "Assignment")]
            Assignment,
            [Display(Name = "Schedule")]
            Schedule,
            [Display(Name = "Manage Users and Groups")]
            ManageUserAndGroup,
            [Display(Name = "Developer Assignment")]
            DeveloperAssignment
        }
        public enum AssignmentOptions
        {
            
            [Display(Name = "Good To Go")]
            Option1,
            [Display(Name = "Schedule W/C")]
            Option2,
            [Display(Name = "Waiting Approval")]
            Option3,
            [Display(Name = "Place Holder")]
            Option4,
            [Display(Name = "No Scope")]
            Option5
        }

        public static string GetDisplayName(Enum value)
        {
            Type enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            MemberInfo member = enumType.GetMember(enumValue)[0];

            var attrs = ((DisplayAttribute[])member.GetCustomAttributes(typeof(DisplayAttribute), false))[0];
            var outString = attrs.Name;

            if (attrs.ResourceType != null)
            {
                outString = attrs.GetName();
            }

            return outString;
        }

        public static List<string> GetDisplayNames<K>()
        {
            var list = new List<string>();
            Type genericType = typeof(K);
            dynamic enumType = genericType;
            if (enumType.IsEnum)
            {
                foreach (string item in Enum.GetNames(genericType))
                {
                    Enum iEnum = Enum.Parse(typeof(K), item) as Enum;
                    list.Add(GetDisplayName(iEnum));
                }
            }

            return list;
        }
    }
}

