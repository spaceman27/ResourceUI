using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using RMADal;
using RMAObjects;
using MongoDB.Driver;

namespace LocalMongoWriting
{

    class Program
    {       

        [STAThread]
        static void Main(string[] args)
        {            

            //// Writing to Employees Collection
            //Console.WriteLine("Writing into Employees Collection");
            //LDAPUpdate();
            //Console.WriteLine("Finished Writing.....");
            //Console.ReadLine();

            //// Writing to Projects Collection
            //Console.WriteLine("Writing into Projects Collection");
            //UpdateProjectCollectionWithData();
            //Console.WriteLine("----Finished Writing.....");
            //Console.ReadLine();

            //// Writing to Projects Scheduling Collection
            //Console.WriteLine("Writing into Projects Scheduling  Collection");
            //MongoDBContext context = new MongoDBContext();
            //SPtoMongoDB.UpdateProjectSchedulesInMongoDB(context.Projects.AsQueryable().Where(p => p.Role == "Developer").ToList());
            //Console.WriteLine("Finished Writing.....");
            //Console.ReadLine();


            //// Writing to Assignment Status Collection
            //Console.WriteLine("Writing into Assignment Status Collection  Collection");
            //MongoDBContext context = new MongoDBContext();
            //context.AssignmentStatus.InsertMany(EmployeeAssignment.GetProjectsAndAssignmentsInfo());
            //Console.WriteLine("Finished Writing.....");
            //Console.ReadLine();
        }

        public static List<Project> CreateProjectCollectionWithData()
        {
            ListItemCollection spItems = GetLatestSharePointMRPData();
            List<Project> ProjectDocuments = new List<Project>();
            if (spItems.Count != 0)
            {
                foreach (ListItem item in spItems)
                {

                    ProjectDocuments.Add(PopulateProjectCollection(item));

                }
                return ProjectDocuments;
            }
            else return new List<Project>();
        }

        public static string objNullCheck(object field)
        {
            return field != null ? field.ToString() : "";
        }

        public static Project PopulateProjectCollection(ListItem listItem)
        {
            try
            {
                return new Project()
                {
                    Title = objNullCheck(listItem["Title"]),
                    ProjectType = listItem["Project_x0020_Type"] != null ?
                                 ((string[])listItem["Project_x0020_Type"])[0] : "",
                    Role = objNullCheck(listItem["Role"]),
                    Date = (DateTime)listItem["Date"],
                    PlannedHours = listItem["Planned_x0020_Hours"] != null ?
                                  Convert.ToInt32(listItem["Planned_x0020_Hours"]) : -1,
                    Person = listItem["Person"] != null ?
                                 (((Microsoft.SharePoint.Client.FieldUserValue)listItem["Person"]).LookupValue) : "",
                    Location = objNullCheck(listItem["Location"]),
                    PM = objNullCheck(listItem["PM"]),
                    Comments = objNullCheck(listItem["Comments"]),
                    Month = objNullCheck(listItem["Month"]),
                    Quarter = objNullCheck(listItem["Quarter"]),
                    Week = listItem["Week"] != null ?
                                 Convert.ToInt32(listItem["Week"]) : -1,
                    ProjectId = listItem["ID"] != null ? Convert.ToInt32(listItem["ID"]) : -1,
                    Modified = (DateTime)listItem["Modified"],
                    Created = (DateTime)listItem["Created"]
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(listItem["Title"].ToString() + " | Error" + ex.Message);
                return null;
            }

        }

        public static void UpdateProjectCollectionWithData()
        {
            var projectList = CreateProjectCollectionWithData();
            SPtoMongoDB.UpdateProjectsInMongoDB(projectList);
        }

        public static void LDAPUpdate()
        {
            List<Employee> ldapDataList = new List<Employee>();

            using (var context = new PrincipalContext(ContextType.Domain, "STI"))
            {
                using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    DirectorySearcher ds = searcher.GetUnderlyingSearcher() as DirectorySearcher;
                    if (ds != null)
                        ds.PageSize = 10000;
                    else
                    {
                        Console.WriteLine("ds not null");
                        Console.ReadLine();
                    }

                    foreach (var result in searcher.FindAll())
                    {
                        DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                        Employee ldapData = new Employee();
                        var mngr = objNullCheck(de.Properties["manager"].Value);
                        mngr = mngr != null ? mngr.Split(',')[0].Replace("CN=", "") : "";

                        ldapData.EmployeeName = objNullCheck(de.Properties["name"].Value);
                        ldapData.FirstName = objNullCheck(de.Properties["givenName"].Value);
                        ldapData.LastName = objNullCheck(de.Properties["sn"].Value);
                        ldapData.SAMAccountName = objNullCheck(de.Properties["samAccountName"].Value);
                        ldapData.DistinguishedName = objNullCheck(de.Properties["distinguishedName"].Value);
                        ldapData.UserPrincipalName = objNullCheck(de.Properties["userPrincipalName"].Value);
                        ldapData.Department = objNullCheck(de.Properties["department"].Value);
                        ldapData.Company = objNullCheck(de.Properties["company"].Value);
                        ldapData.Title = objNullCheck(de.Properties["title"].Value);
                        ldapData.ManagerName = mngr;
                        ldapData.IsDeveloper = ldapData.DistinguishedName.ToUpper().Contains("OU=DEVELOPMENT");

                        if (ldapData.UserPrincipalName != "")
                        {
                            ldapDataList.Add(ldapData);
                        }

                    }
                }
            }

            //method call to Data Access Layer
            SPtoMongoDB.UpdateEmployeesInMongoDB(ldapDataList);
        }

        public static Microsoft.SharePoint.Client.ListItemCollection GetLatestSharePointMRPData()
        {
            string strUrl = @"https://comnetsoftwareinc.sharepoint.com/Operations/ProjectManagement/";

            using (ClientContext context = ClaimClientContext.GetAuthenticatedContext(strUrl))
            {

                if (context != null)
                {
                    // Starting with ClientContext, the constructor requires a URL to the
                    // server running SharePoint.

                    // The SharePoint web at the URL.
                    Web web = context.Web;

                    // Retrieve all lists from the server.
                    // For each list, retrieve Title and Id.
                    context.Load(web.Lists, lists => lists.Include(list => list.Title, list => list.Id));

                    // Execute query.
                    context.ExecuteQuery();

                    Microsoft.SharePoint.Client.List announcementsList = context.Web.Lists.GetByTitle("MRP");

                    // This creates a CamlQuery that specifies Scope="RecursiveAll"
                    // so that it grabs all list items, regardless of the folder they are in.
                    CamlQuery query = CamlQuery.CreateAllItemsQuery();
                    Microsoft.SharePoint.Client.ListItemCollection items = announcementsList.GetItems(query);

                    // Retrieve all items in the ListItemCollection from List.GetItems(Query).
                    context.Load(items);
                    context.ExecuteQuery();

                    return items;
                }
                return null;
            }
        }

    
        }
    }
