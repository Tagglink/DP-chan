using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP_chan.Extensions
{
    class StringExtensions
    {
        public const int MAX_FILEPATH_LENGTH = 259;

        public static string GetFilepath(string uri, string localDir)
        {
            string filename = Path.GetFileName(uri);
            string filepath = localDir + filename;

            int diff = MAX_FILEPATH_LENGTH - filepath.Length;

            if (diff < 0)
            {
                filepath = localDir + filename.Remove(0, -diff);
            }

            return filepath;
        }

        public static string[] Take(string[] arr, int startIndex)
        {
            return Take(arr, startIndex, arr.Length - 1);
        }

        public static string[] Take(string[] arr, int startIndex, int endIndex)
        {
            string[] retArray = new string[endIndex - startIndex + 1];

            for (int i = 0; i < retArray.Length; i++)
            {
                retArray[i] = arr[startIndex + i];
            }

            return retArray;
        }

        public static string[] DecoupleDisordTag(string tag)
        {
            string[] ret = new string[2];

            int pos = 0;

            while (pos < tag.Length)
            {
                if (tag[pos] == '#')
                    break;
                pos++;
            }
            string username = tag.Remove(pos);
            string discriminator = tag.Remove(0, pos + 2);

            ret[0] = username;
            ret[1] = discriminator;

            return ret;
        }

        public static ulong GetDiscordUserIdFromMention(string mention)
        {
            string idString = mention.Remove(mention.Length - 1, 1).Remove(0, 2);

            return ulong.Parse(idString);
        }
    }
}
