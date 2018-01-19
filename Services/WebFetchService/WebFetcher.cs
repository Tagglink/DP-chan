using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using HtmlAgilityPack;

namespace DP_chan.Services.WebFetchService
{
    class WebFetcher
    {
        private WebClient mClient;
        private HtmlWeb mWeb;

        public WebFetcher()
        {
            mClient = new WebClient();
            mWeb = new HtmlWeb();
        }

        public string DownloadDocumentString(string url)
        {
            return mClient.DownloadString(url);
        }

        public void DownloadFile(string url, string filepath)
        {
            mClient.DownloadFile(url, filepath);
        }

        public HtmlDocument DownloadDocument(string url)
        {
            return mWeb.Load(url);
        }

        private void MakeFilepath(string filepath)
        {
            
        }
    }
}
