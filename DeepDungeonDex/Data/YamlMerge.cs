using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dalamud.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DeepDungeonDex.Data
{
    public class YamlMerge
    {
        private static bool YamlObjectStart(string line)
        {
            return line.Length > 0 && Char.IsNumber(line[0]);
        }

        private static bool MergeYamlChanges(List<string> data, out List<string> newData)
        {
            newData = new List<string>();

            var m = Regex.Match(data[0], @"\d+");
            if (!m.Success)
            {
                PluginLog.Error("Invalid object pattern: " + data[0]);
                return false;
            }

            var id = uint.Parse(m.Value);
            var mob = Plugin.MobRepo.Get(id, out var isOverride);

            if (!isOverride)
                return false;

            PluginLog.Information("Merging override for: " + id);

            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var mobDict = new Dictionary<uint, MobData>() { { id, mob } };

            var yaml = serializer.Serialize(mobDict);
            newData = yaml.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return true;
        }

        //sketchy yaml merge
        public static void MergeCustomChanges()
        {
            PluginLog.Information("Merging mob overrides");

            var dbData = File.ReadAllLines(Plugin.MobRepo.DbPath).ToList();

            int start = -1;
            for (int i = 0; i < dbData.Count + 1; i++)
            {
                if (i == dbData.Count || YamlObjectStart(dbData[i]))
                {
                    if (start == -1)
                        start = i;
                    else
                    {
                        var len = i - start;
                        if (len > 1)
                        {
                            var data = dbData.GetRange(start, len);
                            if (MergeYamlChanges(data, out var newData))
                            {
                                dbData.RemoveRange(start, len);
                                dbData.InsertRange(start, newData);

                                i += newData.Count - len;
                            }
                        }

                        start = -1;
                    }
                }
            }

            File.WriteAllLines(Plugin.MobRepo.DbPath+"2", dbData);
        }
    }
}
