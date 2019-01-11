using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace RMAUI.Models
{
    public class ProjectViewModel
    {
        //uncomment when need
        [BsonId]
        public int ID;
        public string ProjectType;
        public string Title;
        public Dictionary<string, int> HourPerWeeks;
    }
}
