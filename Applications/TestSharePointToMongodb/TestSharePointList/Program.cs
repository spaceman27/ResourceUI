using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using MongoDB.Bson.Serialization.Serializers;
using RMADal;
using RMAObjects;
namespace TestSharePointList
{
    class Program
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        static MongoDBContext ctx = new MongoDBContext();
        
        [STAThread]
        static void Main(string[] args)
        {
            GetADUserAndRole();

            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        static void GetADUserAndRole()
        {
            var allUsers = new List<UserModel>();
            var allGroups = new List<GroupModel>();
            var errorUserList = new List<string>();
            #region Exception List
            var exceptionList = new Dictionary<string, string>()
            {
                { "ctimms", "" },
                { "cfultz", "" },
                { "mmr", "" },
                { "arinc", "" },
                { "hertz", "" },
                { "alight", "" },
                { "asteinke", "" },
                { "cosborn", "" },
                { "dwilliams", "" },
                { "edahl", "" },
                { "smoore", "" },
                { "srilling", "" },
                { "kturner", "" },
                { "ebailey", "" },
                { "mfiely", "" },
                { "personnel", "" },
                { "hmckinney", "" },
                { "ajnasir", "" },
                { "jblanford", "" },
                { "slandsaw", "" },
                { "dsheldon", "" },
                { "dmichaels", "" },
                { "EAdminfbb", "" },
                { "braleigh", "" },
                { "tfsservice", "" },
                { "BADMIN", "" },
                { "cmh", "" },
                { "DIA_FTP", "" },
                { "FPNW Service Account", "" },
                { "goldcontent", "" },
                { "OAG", "" },
                { "rmx", "" },
                { "RNOXML", "" },
                { "rocdata", "" },
                { "sbuild", "" },
                { "tfsreports", "" },
                { "rgoins", "" },
                { "aldick", "" },
                { "aevans", "" },
                { "apetrey", "" },
                { "bkinser", "" },
                { "rroalef", "" },
                { "bwickersham", "" },
                { "bbailey", "" },
                { "bhorton", "" },
                { "bpratt", "" },
                { "cnguyen", "" },
                { "driffle", "" },
                { "dcarroll", ""},
                { "dlammlein", ""},
                { "dheil", ""},
                { "EHDV", ""},
                { "egeva", ""},
                { "hjoseph", ""},
                { "jdavidson", ""},
                { "jcollins", ""},
                { "jlane", ""},
                { "klanger", ""},
                { "kkeller", ""},
                { "lcarr", ""},
                { "mmayfield", ""},
                { "mevans", ""},
                { "OPACC", ""},
                { "rmahan", ""},
                { "srohrig", ""},
                { "thorstmann", ""},
                { "tthomas", ""},
                { "kkessler", ""},
                { "EAdmin0f4b21b8", "" },
                { "Unity_COM-UNIFIED01", "" },
                { "cbrown", ""},
                { "jkautz", ""},
                { "mjenkins", ""},
                { "rtaeschner", ""},
                { "rstokes", ""},
                { "tsheldon", ""},
                { "STISDV", ""},
                { "stiftp", ""},
                { "breed", ""},
                { "ssuttle", ""},
                { "rmuswieck", ""},
                { "jorafferty", ""},
                { "dcook", ""},
                { "jauvil", ""},
                { "cfreeman", ""},
                { "dbeattie", ""},
                { "UnityMsgStoreSvc", ""},
                { "UnityInstall", ""},
                { "UnityDirSvc", ""},
                { "UnityAdmin", ""},
                { "abuse", ""},
                { "Supervisor", ""},
                { "exact-vaa", ""},
                { "sehlinger", ""},
                { "EXACTSQL", "" },
                { "dcolon", "" },
                { "lbrown-sjc", "" },
                { "postmaster", "" }
            };
            #endregion
            UserRight fullPermission = new UserRight()
            {
                Read = true,
                Write = true
            };
            var adminGroup = new List<string>()
            {
                "Administrator", "Directors"
            };
            var superUserGroup = new List<string>()
            {
                "Development", "Com-Net Developers", "Accounting"
            };
            var otherGroup = new List<string>()
            {
                "Com-Net Everyone", "Marketing", "Com-Net Sales", "Com-Net Management"
            };
            // Prepare a list of user to write to mongo
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    foreach (var result in searcher.FindAll())
                    {
                        DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                        string username = de.Properties["samAccountName"].Value.ToString();

                        var userRoles = new List<Role>();
                        UserPrincipal user = result as UserPrincipal;
                        try
                        {
                            var roles = user.GetAuthorizationGroups();
                            if (roles.Any() && !exceptionList.ContainsKey(username))
                            {
                                userRoles.AddRange(roles.Select(role => new Role()
                                {
                                    Name = role.SamAccountName
                                }));
                            }
                            
                        }
                        catch(Exception ex)
                        {
                            errorUserList.Add(username);
                            throw ex;
                        }

                        var defaultUserPermission = new List<ModulePermission>();
                        foreach (var item in Enums.GetDisplayNames<Enums.Modules>())
                        {
                            defaultUserPermission.Add(new ModulePermission()
                            {
                                Name = item.ToString(),
                                UserRight = new UserRight()
                                {
                                    Read = null,
                                    Write = null
                                }
                            });
                        }
                        allUsers.Add(new UserModel()
                        {
                            _id = username,
                            Name = result.Name,
                            Username = username,
                            Roles = userRoles,
                            ModulePermissions = defaultUserPermission
                        });
                    }
                }
                // Set Default Permission for Module base on Their Role

                var defaultPermission = new List<ModulePermission>();
                foreach (var item in Enums.GetDisplayNames<Enums.Modules>())
                {
                    defaultPermission.Add(new ModulePermission()
                    {
                        Name = item.ToString(),
                        UserRight = new UserRight()
                        {
                            Read = true,
                            Write = null
                        }
                    });
                }
                using (var searcher = new PrincipalSearcher(new GroupPrincipal(context)))
                {
                    allGroups.AddRange(
                        from result in searcher.FindAll()
                        select result.GetUnderlyingObject() as DirectoryEntry into de
                        select de.Properties["samAccountName"].Value.ToString() into groupName
                        select new GroupModel() {_id = groupName, Name = groupName, ModulePermissions = defaultPermission});
                }
            }
            // groups we want to move to the top => do later

            // Remove and add Modules to mongo
            ctx.DropCollection(Enums.Collection.Modules);
            var lstModules = Enums.GetDisplayNames<Enums.Modules>().Select(val => new ModuleModel() {Name = val}).ToList();
            ctx.GetCollection<ModuleModel>(Enums.Collection.Modules).InsertMany(lstModules);

            // Save group
            if (ctx.GetCollection<GroupModel>(Enums.Collection.Groups).Count(r => r.Name != "") == 0)
            {
                ctx.GetCollection<GroupModel>(Enums.Collection.Groups).InsertMany(allGroups);
            }
            else // update group
            {
                var updateGroup = new List<GroupModel>();
                var dic = new Dictionary<string, GroupModel>();
                var list = ctx.GetCollection<GroupModel>(Enums.Collection.Groups).AsQueryable().ToList();
                foreach (var group in list)
                {
                    dic[group.Name] = group;
                }
                foreach (var group in allGroups)
                {
                    updateGroup.Add(dic.ContainsKey(@group.Name) ? dic[@group.Name] : @group);
                }
                ctx.DropCollection(Enums.Collection.Groups);
                ctx.GetCollection<GroupModel>(Enums.Collection.Groups).InsertMany(updateGroup);
            }


            // Save users
            if (ctx.GetCollection<UserModel>(Enums.Collection.Users).Count(r => r.Name != "") == 0)
            {
                ctx.GetCollection<UserModel>(Enums.Collection.Users).InsertMany(allUsers);
            }
            else // update users
            {
                var updateUser = new List<UserModel>();
                var dic = new Dictionary<string, UserModel>();
                var list = ctx.GetCollection<UserModel>(Enums.Collection.Users).AsQueryable().ToList();
                foreach (var user in list)
                {
                    dic[user.Username] = user;
                }
                foreach (var user in allUsers)
                {
                    if (dic.ContainsKey(@user.Username))
                    {
                        UserModel item = dic[@user.Username];
                        item.Roles = user.Roles;
                        // update default user permission
                        updateUser.Add(item);
                    }
                    else
                    {
                        updateUser.Add(@user);
                    }
                }
                ctx.DropCollection(Enums.Collection.Users);
                ctx.GetCollection<UserModel>(Enums.Collection.Users).InsertMany(updateUser);
            }
        }
    }
}
