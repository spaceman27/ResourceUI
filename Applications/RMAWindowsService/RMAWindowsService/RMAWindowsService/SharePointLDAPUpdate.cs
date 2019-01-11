using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using RMADal;
using RMAObjects;
using Topshelf.Logging;

namespace RMAWindowsService
{
    class SharePointLDAPUpdate
    {
        static string spUserName = @"CMSHometest@comnetsoftware.com";
        static string spPassword = @"DEV4life!";
        private static readonly LogWriter _log = HostLogger.Get<SharePointLDAPUpdate>();
        public static void Run()
        {
            _log.InfoFormat("Start Sharepoint Update..");
            // Update both project and ProjectSchedules in InMongoDB
            UpdateProjectCollectionWithData();
            // Update Employees Collection
            LDAPUpdate();
        }

        
        static List<Project> CreateProjectCollectionWithData()
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
        static string objNullCheck(object field)
        {
            return field != null ? field.ToString() : "";
        }
        static Project PopulateProjectCollection(ListItem listItem)
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
    

        static void UpdateProjectCollectionWithData()
        {
            //method call to Data Access Layer
            var projectList = CreateProjectCollectionWithData();
            SPtoMongoDB.UpdateProjectsInMongoDB(projectList);
            SPtoMongoDB.UpdateProjectSchedulesInMongoDB(projectList);
        }


        static void LDAPUpdate()
        {
            List<Employee> ldapDataList = new List<Employee>();

            using (var context = new PrincipalContext(ContextType.Domain, "STI", spUserName, spPassword))
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
                        ldapData.IsDeveloper = string.IsNullOrEmpty(ldapData.DistinguishedName) ?
                            false : ldapData.DistinguishedName.Contains("Development");

                        //ldapData.LdapManagerDistinguishedName = de.Properties["manager"].Value.ToString();

                        //int cnt = de.Properties["memberOf"].Count;
                        //string memberof = "";
                        //for (int idx = 0; idx < cnt; idx++)
                        //{
                        //    memberof += (de.Properties["memberOf"][idx].ToString().Split(',')[0].Replace("CN=", ""));
                        //    if (idx < (cnt - 1))
                        //    {
                        //        memberof += ", ";
                        //    }
                        //}
                        if (ldapData.UserPrincipalName != "")
                            ldapDataList.Add(ldapData);

                    }
                }
            }

            //method call to Data Access Layer
            SPtoMongoDB.UpdateEmployeesInMongoDB(ldapDataList);
        }

        static MsOnlineClaimsHelper helper = null;

        static Microsoft.SharePoint.Client.ListItemCollection GetLatestSharePointMRPData()
        {
            string strUrl = @"https://comnetsoftwareinc.sharepoint.com/Operations/ProjectManagement/";
            string strBaseUrl = @"https://comnetsoftwareinc.sharepoint.com/";

            string spUserName = @"CMSHometest@comnetsoftware.com";
            string spPassword = @"DEV4life!";

            helper = new MsOnlineClaimsHelper(
                        spUserName,
                        spPassword,
                        strBaseUrl);

            using (ClientContext context = new ClientContext(strUrl))
            {
                context.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(ctx_ExecutingWebRequest);

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


        private static void ctx_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            e.WebRequestExecutor.WebRequest.CookieContainer = helper.CookieContainer;
        }


    }
}
