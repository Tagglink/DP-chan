using System;
using Discord.Commands;

namespace DP_chan.CommandModules.CustomAttributes{
    [AttributeUsage(AttributeTargets.Method)]
    public class UnpausableAttribute : Attribute {

    }
}
