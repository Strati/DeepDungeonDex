using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DeepDungeonDex.Data
{
    public class DataHandler
    {
        private static bool _dataLoaded = false;

        private static Dictionary<uint, MobData> _mobData;
        private static string DataPath => Path.Combine(Plugin.PluginInterface.AssemblyLocation.Directory.FullName, "data.yml");

        public static MobData Mobs(uint nameID)
        {
            if (!_dataLoaded)
                return null;

            if (_mobData.TryGetValue(nameID, out MobData value)) 
                return value;
            return null;
        }

        public static void Load()
        {
            Task.Run(() =>
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var yaml = File.ReadAllText(DataPath);

                _mobData = deserializer.Deserialize<Dictionary<uint, MobData>>(yaml);

                _dataLoaded = true;
            });
        }
    }
}
