using System.IO;

namespace DP_chan.Services.JsonService
{
    class FileHelper
    {
        public static void Write(string str, string filepath, bool noCreate = false)
        {
            FileMode mode = FileMode.OpenOrCreate;
            if (noCreate)
                mode = FileMode.Open;

            FileStream stream = File.Open(filepath, mode);

            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);

            stream.Close();
        }

        public static string Read(string filepath, bool noCreate = false)
        {
            string ret = "";
            FileMode mode = FileMode.OpenOrCreate;

            if (noCreate)
                mode = FileMode.Open;

            FileStream stream = File.Open(filepath, mode);

            StreamReader reader = new StreamReader(stream);
            ret = reader.ReadToEnd();

            stream.Close();

            return ret;
        }
    }
}
