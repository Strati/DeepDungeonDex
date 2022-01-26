using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DeepDungeonDex.Data
{
    public class DataRepo
    {
        private static bool _dataLoaded = false;

        private static Dictionary<uint, MobData> _mobData;
        private static Dictionary<uint, JobData> _jobData;

        private static string DataDir => Plugin.PluginInterface.AssemblyLocation.Directory.FullName;
        private static string MobsDbPath => Path.Combine(DataDir, "mob-data.yml");
        private static string JobsDbPath => Path.Combine(DataDir, "job-data.yml");

        public static MobData GetMob(uint nameID)
        {
            if (!_dataLoaded)
                return null;

            if (_mobData.TryGetValue(nameID, out var value)) 
                return value;
            return null;
        }
        public static JobData GetJob(uint jobId)
        {
            if (!_dataLoaded)
                return null;

            if (_jobData.TryGetValue(jobId, out var value))
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

                var yaml = File.ReadAllText(MobsDbPath);
                _mobData = deserializer.Deserialize<Dictionary<uint, MobData>>(yaml);

                yaml = File.ReadAllText(JobsDbPath);
                _jobData = deserializer.Deserialize<Dictionary<uint, JobData>>(yaml);
                
                _dataLoaded = true;
            });
        }
    }
}
