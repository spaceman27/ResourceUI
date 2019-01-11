using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using RMAObjects;
using RMADal;
using RMAUI.BL;
using RMAUI.Models;


namespace RMAUI.Controllers
{
    public class ProjectScheduleController : Controller
    {
        private IProjSchedule projschedule;
        private IHostingEnvironment _env;
        public ProjectScheduleController(IHostingEnvironment env)
        {
            _env = env;
            projschedule = new ProjSchedule();
        }
        //public ProjectScheduleController(IProjSchedule ips)
        //{
        //    projschedule = ips;
        //}

        [HttpGet]
        public IActionResult GetListSchedule()
        {
            ScheduleprojectView list = projschedule.getscheduleview();
            return Json(list);
        }

        [HttpGet]
        public IActionResult GetProjectStatus()
        {
            List<GetStatusModel> list = projschedule.GetProjectStatus();
            return Json(list);
        }

        [HttpPost]
        public IActionResult SearchSchedule([FromBody] ScheduleFilterModel values)
        {
            List<ProjectSchedule> list = projschedule.FilterBy(values).Schedule;
            return Json(list);
        }

        [HttpPost]        
        public IActionResult UpdateData([FromBody] List<ProjectSchedule> data)
        {
            foreach (var item in data)
            {
                if (item.RequiredSkills == null)
                    item.RequiredSkills = new List<string>();
                if (item.ReferenceLink == null)
                    item.ReferenceLink = new List<string>();
            }
            ProjectScheduling.UpdateProjectSchedule(data);
            return Json(true);
        }

        [HttpGet]
        public IActionResult ShowSkills()
        {
            XmlDocument doc = new XmlDocument();
            var webRoot = _env.WebRootPath;
            var file = System.IO.Path.Combine(webRoot, "SkillView.xml");
            doc.Load(file);
            var jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None);
            var jsonStrPrep = Regex.Replace(jsonText, "\"?xml\":{\"@version\":\"1.0\",\"@encoding\":\"utf-8\"},", string.Empty, RegexOptions.IgnoreCase).Replace("\"?", "").Replace("\\\"?\\", "").Replace("@", "");
            RootObject deserializedProduct = JsonConvert.DeserializeObject<RootObject> (jsonStrPrep);
            List<SkillSetModel> list = projschedule.ParseSkillSetListFrom(deserializedProduct);
            return Json(list);
        }
    }
}