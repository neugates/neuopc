using System;
using System.IO;
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
            string jsonString;
            try
            {
                jsonString = File.ReadAllText(filename);
            }
            catch (Exception ex)
            {
                Log.Information(ex, "read config file failed, not exist");
                return new Config();
            }

            Config config;
            try
            {
                config = JsonSerializer.Deserialize<Config>(jsonString);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "deserialize config failed");
                return new Config();
            }

            return config;
        }

        public static void SaveConfig(string filename, Config config)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize<Config>(
                    config,
                    new JsonSerializerOptions { WriteIndented = true }
                );
                File.WriteAllText(filename, jsonString);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "write config file error");
            }
        }
    }
}
