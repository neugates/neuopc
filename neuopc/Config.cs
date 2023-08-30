using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Serilog;

namespace neuopc
{
    public class Config
    {
        public string DAHost { get; set; }
        public string DAServer { get; set; }
        public string UAUrl { get; set; }
        public string UAUser { get; set; }
        public string UAPassword { get; set; }
        public bool AutoConnect { get; set; }
    }

    public static class ConfigUtil
    {
        public static Config LoadConfig(string filename)
        {
            string jsonString = string.Empty;
            try
            {
                jsonString = File.ReadAllText(filename);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "read config file failed");
            }

            var config = new Config();

            try
            {
                config = JsonSerializer.Deserialize<Config>(jsonString);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "deserialize config failed");
            }

            return config;
        }

        public static void SaveConfig(string filename, Config config)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize<Config>(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filename, jsonString);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "write config file error");
            }
        }
    }
}
