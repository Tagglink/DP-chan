using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using HtmlAgilityPack;
using DP_chan.Extensions;

namespace DP_chan.Services.ImageFetchService
{
    class ImageBoardSearcher
    {
        private enum Board { KONACHAN, DANBOORU, YANDERE, _LENGTH };
        private Random randomizer;

        private Dictionary<Board, ImageBoard> imageBoards;

        public ImageBoardSearcher()
        {
            randomizer = new Random();
            imageBoards = CreateImageBoards();
        }

        public string GetImageURI(HtmlDocument doc, string boardName, int index)
        {
            Board board = GetBoard(boardName);
            ImageBoard imageBoard = imageBoards[board];

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(imageBoard.xPathToList);

            if (nodes == null)
            {
                throw new ImageNotFoundException();
            }

            HtmlNode node = GetImageNode(nodes, index);
            string uri = ExtractURI(node, board);

            return uri;
        }

        public string ParseUrl(string boardName, string[] tags, int page, bool safe)
        {
            Board board = GetBoard(boardName);
            ImageBoard imageBoard = imageBoards[board];

            // take only the tags that are within the tag limit
            if (tags.Length > imageBoard.tagLimit)
                tags = StringExtensions.Take(tags, 0, imageBoard.tagLimit - 1);

            string url = CreateTaggedUrl(imageBoard, safe, tags);
            url += "&" + imageBoard.pagePrefix + page;

            return url;
        }

        public string GetSafeTag(string boardName)
        {
            return imageBoards[GetBoard(boardName)].safeTag;
        }

        private HtmlNode GetImageNode(HtmlNodeCollection listNodes, int index)
        {
            if (index < 0) {
                return GetRandomNode(listNodes);
            }
            else if (index >= listNodes.Count)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                return listNodes[index];
            }
        }

        private string ExtractURI(HtmlNode node, Board board)
        {
            string uri = "";
            ImageBoard imageBoard = imageBoards[board];
            if (board == Board.DANBOORU)
            {
                uri = imageBoard.boardUrl + node.Attributes["data-file-url"].Value;
            }
            else if (board == Board.KONACHAN)
            {
                string xPath = "a[1]";
                uri = node.SelectSingleNode(xPath).Attributes["href"].Value;
            }
            else if (board == Board.YANDERE)
            {
                string xPath = "a[1]";
                uri = node.SelectSingleNode(xPath).Attributes["href"].Value;
            }

            return uri;
        }

        private Board GetBoard(string arg)
        {
            switch (arg)
            {
                case "danbooru":
                    return Board.DANBOORU;
                case "konachan":
                    return Board.KONACHAN;
                case "yandere":
                    return Board.YANDERE;
                default:
                     return GetRandomBoard();
            }
        }

        private Board GetRandomBoard()
        {
            int r = randomizer.Next() % (int)Board._LENGTH;
            return (Board)r;
        }

        private HtmlNode GetRandomNode(HtmlNodeCollection nodes)
        {
            int r = randomizer.Next() % nodes.Count;
            return nodes[r];
        }

        private string CreateTaggedUrl(ImageBoard board, bool safe, string[] tags)
        {
            string url = board.boardUrl + board.postsPrefix;

            url += '?' + board.tagPrefix;

            if (safe)
                url += board.safeTag + '+';

            for (int i = 0; i < tags.Length; i++)
            {
                string tag = tags[i];
                tag = Uri.EscapeDataString(tag);
                url += tag;

                if (i != tags.Length - 1)
                    url += '+';
            }

            return url;
        }

        private Dictionary<Board, ImageBoard> CreateImageBoards()
        {
            ImageBoard danbooru;
            danbooru.boardUrl = @"http://danbooru.donmai.us";
            danbooru.safeTag = @"rating%3Asafe";
            danbooru.tagPrefix = @"tags=";
            danbooru.postsPrefix = @"/posts";
            danbooru.pagePrefix = @"page=";
            danbooru.tagLimit = 2;
            danbooru.xPathToList = @"//div[@id='posts']/div[1]/article";

            ImageBoard konachan;
            konachan.boardUrl = @"http://konachan.com";
            konachan.safeTag = @"rating%3Asafe";
            konachan.tagPrefix = @"tags=";
            konachan.pagePrefix = @"page=";
            konachan.postsPrefix = @"/post";
            konachan.tagLimit = 6;
            konachan.xPathToList = @"//div[@class='content']/div[4]/ul/li";

            ImageBoard yandere;
            yandere.boardUrl = @"http://yande.re";
            yandere.safeTag = @"rating%3Asafe";
            yandere.tagPrefix = @"tags=";
            yandere.postsPrefix = @"/post";
            yandere.pagePrefix = @"page=";
            yandere.tagLimit = 6;
            yandere.xPathToList = @"//ul[@id='post-list-posts']/li";

            return new Dictionary<Board, ImageBoard>()
            {
                { Board.DANBOORU, danbooru },
                { Board.KONACHAN, konachan },
                { Board.YANDERE, yandere }
            };
        }
    }
}
