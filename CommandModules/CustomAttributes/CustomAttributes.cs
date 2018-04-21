using System;
using Discord.Commands;

namespace DP_chan.CommandModules.CustomAttributes{

    [AttributeUsage(AttributeTargets.Method)]
    public class UnpausableAttribute : Attribute {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RequirePermissionAttribute : Attribute {
        private string permission;

        public RequirePermissionAttribute(string permission) {
            this.permission = permission;
        }

        public string Permission { 
            get {
                return permission;
            }
        }
    }
}
