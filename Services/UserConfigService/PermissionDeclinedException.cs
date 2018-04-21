using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP_chan.Services.UserConfigService
{
    public class PermissionDeclinedException : Exception
    {
        private readonly string requiredPermission;

        public PermissionDeclinedException(string requiredPermission) {
            this.requiredPermission = requiredPermission;
        }

        public PermissionDeclinedException() {
            requiredPermission = string.Empty;
        }

        public override string Message {
            get {
                if (requiredPermission == string.Empty) {
                    return "You don't have permission to use that command!";
                } else {
                    return "You don't have permission to use that command!" + 
                        "\nRequired permission: " + requiredPermission;
                }
            }
        }
    }
}
