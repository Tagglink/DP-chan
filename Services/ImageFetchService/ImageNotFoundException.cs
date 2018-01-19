using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP_chan.Services.ImageFetchService
{
    class ImageNotFoundException : Exception
    {
        public override string Message {
            get {
                return "Image could not be found!";
            }
        }
    }
}
