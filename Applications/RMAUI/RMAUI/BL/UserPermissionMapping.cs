using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using MongoDB.Driver;
using RMADal;
using RMAObjects;
namespace RMAUI.BL
{
    public interface IUserPermissionMapping
    {
        
    }
    public class UserPermissionMapping
    {
        private MongoDBContext context = new MongoDBContext();
        //private BasicCRUD crud = new BasicCRUD();

        public List<GroupModel> GetGroupsCollection()
        {
            return context.GetCollection<GroupModel>(Enums.Collection.Groups).AsQueryable().Select(r => r).ToList();
        }

        public List<UserModel> GetUsersCollection()
        {
            return context.GetCollection<UserModel>(Enums.Collection.Users).AsQueryable().Select(r => r).ToList();
        }

        public UserModel GetUsersCollectionByUserName(string userName)
        {
            return
                context.GetCollection<UserModel>(Enums.Collection.Users)
                    .AsQueryable().SingleOrDefault(r => r.Username.ToLower() == userName.ToLower());
        }

        public GroupModel GetGroupByGroupName(string groupName)
        {
            return
                context.GetCollection<GroupModel>(Enums.Collection.Groups)
                    .AsQueryable().SingleOrDefault(r => r.Name == groupName);
        }

        // get UserRight of a module that bind to user
        public UserRight GetUserRightOfModule(string username, Enums.Modules module)
        {
            UserRight result = null;
            var user = GetUsersCollectionByUserName(username);
            // get group permission
            if (user!=null)
            {
                result = new UserRight();
                foreach (var group in user.Roles)
                {
                    var groupModel = GetGroupByGroupName(group.Name);
                    if (groupModel != null)
                    {
                        foreach (var permission in groupModel.ModulePermissions)
                        {
                            if (permission.Name == Enums.GetDisplayName(module))
                            {
                                result.Read |= permission.UserRight.Read;
                                result.Write |= permission.UserRight.Write;
                            }
                        }
                    }
                }
                // combine to user permission
                foreach (var permission in user.ModulePermissions)
                {
                    if (permission.Name == Enums.GetDisplayName(module))
                    {
                        result.Read = permission.UserRight.Read ?? result.Read;
                        result.Write = permission.UserRight.Write ?? result.Write;
                    }
                }
            }
            
            return result;
        }

        public Task<ReplaceOneResult> UpdateGroupPermission(GroupModel grp)
        {
            return context.GetCollection<GroupModel>(Enums.Collection.Groups).ReplaceOneAsync(e => e.Name == grp.Name, grp);
        }

        public Task<ReplaceOneResult> UpdateUserPermission(UserModel usr)
        {
            return context.GetCollection<UserModel>(Enums.Collection.Users).ReplaceOneAsync(e => e._id == usr._id, usr);
        }
    }
}