using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using RMAUI.BL;
using RMAUI.Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using RMADal;
using System.Globalization;

namespace RMAUI.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Schedule
        private IDataFeeder dataFeeder;

        public ScheduleController()
        {
            dataFeeder = new DataFeeder();
        }

        //public ScheduleController(IDataFeeder df)
        //{
        //    dataFeeder = df;
        //}

        [HttpPost]
        public ActionResult SearchProject([FromBody] ProjectSearchModel f)
        {
            DateTime start = f.StartDate;
            DateTime end = f.EndDate;
            if (f.StartDate.Ticks == 0)
            {
                start = DateTime.Now;
            }
            if (f.EndDate.Ticks == 0)
            {
                end = DateTime.Now.AddMonths(2);
            }
            List<ProjectViewModel> data = new DataFeeder().GetProjectScheduleViewData(start, end);
            return Json(data);
        }
    }
}