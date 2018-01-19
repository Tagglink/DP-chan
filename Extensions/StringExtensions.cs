using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DP_chan.Extensions
{
    class StringExtensions
    {
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
