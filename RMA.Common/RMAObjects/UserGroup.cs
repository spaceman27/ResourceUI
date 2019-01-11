using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace RMAObjects
{
    public class GroupModel
    {
        [BsonId]
        public object _id;
        public string Name { get; set; }
        public string Username { get; set; }
        public List<ModulePermission> ModulePermissions { get; set; }
    }

    public class ModulePermission
    {
        public string Name;
        public UserRight UserRight;
    }

    public class UserRight
    {
        public bool? Read { get; set; } 
        public bool? Write { get; set; }
    }

    public class UserModel
    {
        [BsonId]
        public object _id;
        public string Name { get; set; }
        public string Username { get; set; }
        public List<ModulePermission> ModulePermissions { get; set; }

        public List<Role> Roles { get; set; }
    }

    public class Role
    {
        public string Name { get; set; }
    }

    public class ModuleModel
    {
        public string Name { get; set; }
    }
}
