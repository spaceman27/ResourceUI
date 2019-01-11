using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RMAUI.BL;
using RMAUI.Models;
using MongoDB.Bson;
using RMADal;
using RMAObjects;

namespace RMAUI.Controllers
{
    public class UserGroupController : Controller
    {
        private UserPermissionMapping mapper = new UserPermissionMapping();
        
        [HttpGet]
        public ActionResult GetUserList()
        {
            List<UserModel> list = mapper.GetUsersCollection();
            return Json(list);
        }

        [HttpGet]
        public ActionResult GetGroupList()
        {
            List<GroupModel> list = mapper.GetGroupsCollection();
            return Json(list);
        }

        [HttpPost]
        public ActionResult UpdateGroupPermission([FromBody] UpdateGroupModel model)
        {
            var grp = mapper.GetGroupByGroupName(model.GroupName);
            foreach (ModulePermission module in grp.ModulePermissions)
            {
                if (module.Name == model.ModuleName)
                {
                    module.UserRight = model.UserRight;
                    break;
                }
            }

            var result = mapper.UpdateGroupPermission(grp);
            if (result.Result.IsAcknowledged)
            {
                return Json(new List<GroupModel>() { mapper.GetGroupByGroupName(model.GroupName) });
            }
            return Json(result.Status);
        }

        [HttpPost]
        public ActionResult UpdateUserPermission([FromBody] UpdateUserModel model)
        {
            var usr = mapper.GetUsersCollectionByUserName(model.UserName);
            foreach (ModulePermission module in usr.ModulePermissions)
            {
                if (module.Name == model.ModuleName)
                {
                    module.UserRight = model.UserRight;
                    break;
                }
            }

            var result = mapper.UpdateUserPermission(usr);
            if (result.Result.IsAcknowledged)
            {
                return Json(new List<UserModel>() { mapper.GetUsersCollectionByUserName(model.UserName) });
            }
            return Json(result.Status);
        }
    }
}