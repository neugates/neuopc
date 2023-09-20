using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Serilog;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuopc
{
    public class Tag
    {
        public string ItemName { get; set; }

        public string DataType { get; set; }
    }

    public class Tags
    {
        public List<Tag> List { get; set; }
    }

    public class TagJson
    {
        public static List<Tag> GetTags(string filename)
        {
            string jsonString;
            try
            {
                jsonString = File.ReadAllText(filename);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "read config file failed, not exist");
                return new List<Tag>();
            }

            Tags tags;
            try
            {
                tags = JsonSerializer.Deserialize<Tags>(jsonString);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "deserialize config failed");
                return new List<Tag>();
            }

            return tags.List;
        }
    }
}
