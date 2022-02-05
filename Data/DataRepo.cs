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
    public class DataRepo<T> where T : class, IRepoData<T>
    {
        private object _lock = new();
        private bool _dataLoaded = false;

        private Dictionary<uint, T> _data;
        private Dictionary<uint, T> _overrideData;

        public string Name { get; init; }
        private string DbPath { get; init; }
        private string OverrideDbPath { get; init; }

        public static DataRepo<T> Create(DalamudPluginInterface plugin, string name)
        {
            var dir = plugin.AssemblyLocation.Directory.FullName;
            var repo = new DataRepo<T>()
            {
                Name = name,
                DbPath = Path.Combine(dir, $"{name}.yml"),
                OverrideDbPath = Path.Combine(plugin.GetPluginConfigDirectory(), $"{name}-overrides.yml")
            };

            return repo;
        }

        private DataRepo() { }

        public T Get(uint id)
        {
            return Get(id, out _);
        }
        public T Get(uint id, out bool isOverride)
        {
            isOverride = false;
            if (!_dataLoaded)
                return null;

            if (_overrideData.TryGetValue(id, out var value))
            {
                isOverride = true;
                return value.Clone();
            }
            if (_data.TryGetValue(id, out value))
                return value.Clone();
            return null;
        }

        public void SaveOverride(uint id, T data)
        {
            if (!_dataLoaded)
                return;
            
            _overrideData[id] = data;
            Save();
        }

        public void ClearOverride(uint id)
        {
            if (!_dataLoaded)
                return;
            
            _overrideData.Remove(id);
            Save();
        }
        
        public DataRepo<T> Load()
        {
            Task.Run(() =>
            {
                try
                {
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

                    var yaml = File.ReadAllText(DbPath);
                    _data = deserializer.Deserialize<Dictionary<uint, T>>(yaml);

                    if (File.Exists(OverrideDbPath))
                    {
                        yaml = File.ReadAllText(OverrideDbPath);
                        _overrideData = deserializer.Deserialize<Dictionary<uint, T>>(yaml);
                    }
                    else
                        _overrideData = new Dictionary<uint, T>();

                    _dataLoaded = true;
                }
                catch(Exception ex)
                {
                    PluginLog.Error($"Error loading database ({Name}): {ex}");
                }
            });

            return this;
        }

        private void Save()
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

                        var yaml = serializer.Serialize(_overrideData);
                        File.WriteAllText(OverrideDbPath, yaml);
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Error($"Error saving database ({Name}): " + ex);
                }
            });
        }
    }
}
