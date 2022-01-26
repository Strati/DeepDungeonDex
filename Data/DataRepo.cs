using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dalamud.Logging;
using Dalamud.Plugin;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DeepDungeonDex.Data
{
    public class DataRepo
    {
        private static object _lock = new object();
        private static bool _dataLoaded = false;

        private static Dictionary<uint, MobData> _mobData;
        private static Dictionary<uint, MobData> _mobOverrideData;
        private static Dictionary<uint, JobData> _jobData;

        private static string DataDir => Plugin.PluginInterface.AssemblyLocation.Directory.FullName;
        private static string MobsDbPath => Path.Combine(DataDir, "mob-data.yml");
        private static string MobOverrideDbPath => Path.Combine(Plugin.PluginInterface.GetPluginConfigDirectory(), "mob-overrides-data.yml");
        private static string JobsDbPath => Path.Combine(DataDir, "job-data.yml");

        public static MobData GetMob(uint id)
        {
            if (!_dataLoaded)
                return null;

            if (_mobOverrideData.TryGetValue(id, out var value))
                return new MobData(value);
            if (_mobData.TryGetValue(id, out value))
                return new MobData(value);
            return null;
        }

        public static void SaveOverride(uint id, MobData data)
        {
            if (!_dataLoaded)
                return;
            
            _mobOverrideData[id] = data;
            Save();
        }

        public static void ClearOverride(uint id)
        {
            if (!_dataLoaded)
                return;
            
            _mobOverrideData.Remove(id);
            Save();
        }


        public static JobData GetJob(uint id)
        {
            if (!_dataLoaded)
                return null;

            if (_jobData.TryGetValue(id, out var value))
                return value;
            return null;
        }

        public static void Load()
        {
            Task.Run(() =>
            {
                try
                {
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

                    var yaml = File.ReadAllText(MobsDbPath);
                    _mobData = deserializer.Deserialize<Dictionary<uint, MobData>>(yaml);

                    if (File.Exists(MobOverrideDbPath))
                    {
                        yaml = File.ReadAllText(MobOverrideDbPath);
                        _mobOverrideData = deserializer.Deserialize<Dictionary<uint, MobData>>(yaml);
                    }
                    else
                        _mobOverrideData = new Dictionary<uint, MobData>();

                    yaml = File.ReadAllText(JobsDbPath);
                    _jobData = deserializer.Deserialize<Dictionary<uint, JobData>>(yaml);
                    

                    _dataLoaded = true;
                }
                catch(Exception ex)
                {
                    PluginLog.Error("Error loading database: " + ex);
                }
            });
        }

        private static void Save()
        {
            Task.Run(() =>
            {
                try
                {
                    lock (_lock)
                    {
                        var serializer = new SerializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();

                        var yaml = serializer.Serialize(_mobOverrideData);
                        File.WriteAllText(MobOverrideDbPath, yaml);
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Error("Error saving database: " + ex);
                }
            });
        }
    }
}
