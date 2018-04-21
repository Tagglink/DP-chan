using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DP_chan.Services.WebFetchService;
using DP_chan.Services.JsonService;
using DP_chan.Extensions;

namespace DP_chan.Services.ImageFetchService
{
    class ImageBoardFetcher : WebFetcher
    {
        private ImageBoardSearcher searcher;
        private Json json;

        private readonly string imagePath;
        private readonly string dataPath;
        private readonly string dataFilename;
        private readonly int cacheLimit;

        private List<BoardImage> images;

        public ImageBoardFetcher(Json json, string imagePath, string dataPath)
        {
            searcher = new ImageBoardSearcher();
            this.json = json;
            this.imagePath = imagePath;
            this.dataPath = dataPath;
            dataFilename = "board_images.json";
            cacheLimit = 50;

            images = Load();
            if (images == null)
            {
                images = new List<BoardImage>();
            }
 
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
        }

        public string GetImage(string board, string[] tags, int page, int index, bool safe)
        {
            string url = searcher.ParseUrl(board, tags, page, safe);

            HtmlDocument doc = DownloadDocument(url);
            string imageURI = searcher.GetImageURI(doc, board, index);

            string filepath = StringExtensions.GetFilepath(imageURI, imagePath);
            
            if (!File.Exists(filepath))
            {
                BoardImage img = DownloadImage(imageURI, filepath);
                AddImage(img);
            }

            return filepath;
        }

        private BoardImage DownloadImage(string imageURI, string filepath)
        {
            BoardImage img = new BoardImage();
            DownloadFile(imageURI, filepath);

            img.timeDownloaded = DateTime.Now;
            img.filename = Path.GetFileName(filepath);
            img.filepath = filepath;
            img.uri = imageURI;

            return img;
        }

        private void RemoveOldestImage()
        {
            DateTime oldestTime = DateTime.Now;
            int oldestIndex = 0;

            for (int i = 0; i < images.Count; i++)
            {
                BoardImage img = images[i];
                if (img.timeDownloaded.CompareTo(oldestTime) < 0)
                {
                    oldestTime = img.timeDownloaded;
                    oldestIndex = i;
                }
            }

            RemoveImage(oldestIndex);
        }

        private void AddImage(BoardImage img)
        {
            if (images.Count >= cacheLimit)
            {
                RemoveOldestImage();
            }
            images.Add(img);
            Save();
        }

        private void RemoveImage(int index)
        {
            BoardImage img = images[index];
            File.Delete(img.filepath);
            images.RemoveAt(index);
            Save();
        }

        private void Save()
        {
            json.SaveProperly(images, dataPath + dataFilename);
        }

        private List<BoardImage> Load()
        {
            return Json.Open<List<BoardImage>>(dataPath + dataFilename);
        }
    }
}
