using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using RMAUI.BL;
using Microsoft.AspNetCore.Mvc;

namespace RMAUI.Controllers
{
    public class DeveloperController : Controller
    {
        DeveloperMapping devMapping = new DeveloperMapping();
        
        [HttpPost]
        public IActionResult GetAssignment([FromBody] List<string> values)
        {
            var username = User.Identity.Name.Substring(User.Identity.Name.IndexOf("\\", StringComparison.Ordinal) + 1);
            var data = values.Count > 0 ? devMapping.GetDeveloperAssignmentViewDataByNames(values) : devMapping.GetDeveloperAssignmentViewDataByUsername(username);
            return Json(data);
        }

        [HttpGet]
        public IActionResult GetUserList()
        {
            // get only developer who have assignments
            var data = devMapping.GetEmployeeNameHasAssignment();
            return Json(data);
        }
    }
}