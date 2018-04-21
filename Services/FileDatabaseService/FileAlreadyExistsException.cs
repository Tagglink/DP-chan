using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP_chan.Services.FileDatabaseService
{
    class FileAlreadyExistsException : Exception
    {
        public override string Message {
            get {
                return "That file already exists in that group!";
            }
        }
    }
}
