using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP_chan.Services.ImageFetchService
{
    struct ImageBoard
    {
        public string safeTag;
        public string boardUrl;
        public string tagPrefix;
        public string postsPrefix;
        public string pagePrefix;
        public int tagLimit;
        public string xPathToList;
    }
}
