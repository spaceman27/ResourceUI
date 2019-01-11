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
    public class ResourceController : Controller
    {
        public JsonResult getEnums()
        {
            var v1 = Scheduling.GetAssignmentOptions();
            return Json(v1);
        }

        [HttpPost]
        public JsonResult showAssignmentstatus([FromBody] AssignmentShowModel model)
        {
            model.Month++;
            List<AssignmentStatus> data = EmployeeAssignment.GetProjectsAndAssignmentsInfo(model);
            return Json(data);
        }
        [HttpPost]
        public ActionResult postdata([FromBody] AssignmentSaveModel model)
        {
            EmployeeAssignment.SaveAssignmentsInfo(model.Assignments);
            return showdata(model.ShowData);
        }

        [HttpPost]
        public ActionResult showdata([FromBody] AssignmentShowModel model)
        {
            List<Assignment> assignmentInfo = EmployeeAssignment.GetAssignmentsInfo(model.EmployeeNames, model.Year, model.Month + 1);
            return Json(assignmentInfo);
        }
        [HttpPost]
        public ActionResult deletedata([FromBody] AssignmentSaveModel model)
        {
            EmployeeAssignment.DeleteAssignmentsInfo(model.Assignments);
            return showdata(model.ShowData);
        }

        [HttpGet]
        public ActionResult GetProjectList()
        {
            return Json(Scheduling.GetProjectList());
        }

        [HttpGet]
        public ActionResult GetEmployeeList()
        {
            return Json(Scheduling.GetEmployeeNames().OrderBy(r => r.Name));
        }
    }
}