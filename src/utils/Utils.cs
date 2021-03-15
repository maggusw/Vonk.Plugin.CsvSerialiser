using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json.Linq;
using Serilog;
using Vonk.Core.ElementModel;
using Vonk.Core.Repository;
using Task = System.Threading.Tasks.Task;

namespace Vonk.Plugin.CsvSerializer.utils
{
    public static class Utils
    {
        public static async Task<Bundle> CreateBundleFromSearchResult
        (SearchResult
            searchResult)
        {
            _ = await Task.FromResult(true);
            var bundle = new Bundle()
                {Type = Bundle.BundleType.Searchset};
            // ToDo: add meta data

            if (searchResult.TotalCount == 0)
                throw new NullReferenceException(
                    "Empty search result. Please check query.");
            var watch = new Stopwatch();
            watch.Start();
            foreach (var resource in searchResult)
            {
                bundle.Entry.Add(
                    new Bundle.EntryComponent()
                    {
                        Resource = resource.ToPoco<Resource>()
                    });
            }
            watch.Stop();
            Log.Information("Took: " + watch.ElapsedMilliseconds);
            
            return bundle;
        }

        public static async Task<List<Dictionary<string, string>>>
            CreateDictListFromBundle(
                Bundle bundle)
        {
            _ = await Task.FromResult(true);

            return bundle.Entry
                .Select(t1 => JObject.Parse(t1.ToJson()))
                .Select(o => o.SelectTokens("$..*")
                    .Where(t => !t.HasValues)
                    .ToDictionary(t => t.Path, t => t.ToString()))
                .ToList();
        }

        public static async Task<List<string>>
            CreateDistinctKeyListFromDicts(
                IEnumerable<Dictionary<string, string>> dictList)
        {
            _ = await Task.FromResult(true);

            var listOfStrings = dictList
                .Select(listEntry => listEntry.Keys.ToList())
                .ToList();

            var list2 = listOfStrings.SelectMany(x => x).ToList()
                .Distinct()
                .ToList(); // distinct key list -- to web interfacce?


            return list2;
        }

        public static async Task<string> CreateCsvFromLists
        (IEnumerable<Dictionary<string, string>> dictList,
            List<string> keyList)
        {
            _ = await Task.FromResult(true);
            var sb = new StringBuilder();

            /*
             * currently unused
             * will take a selection of properties
             * and return only the values for those properties
             * will require a bit of different handling
             * 
            
            var selection = new List<string> {"resource.id", "resource.name[0].family", "resource.name[0].given[0]", "resource.gender", "resource.birthDate"};
            
            // this selects the items from the above list.
            
            var firstLine = keyList.Where(item => selection.Contains
                (item));

            var newLine = string.Join(";", firstLine);

            */

            sb.Append(string.Join(";", keyList));
            sb.Append("\n");

            var count = 0;

            foreach (var dict in dictList)
            {
                foreach (var key in keyList)
                {
                    foreach (var kvp in dict)
                    {
                        if (!dict.ContainsKey(key))
                        {
                            if (keyList.IndexOf(key) <
                                keyList.Count - 1) sb.Append(";");
                            break;
                        }

                        if (!key.Equals(kvp.Key)) continue;
                        if (string.IsNullOrEmpty(kvp.Value))
                            sb.Append(";");
                        else if (keyList.IndexOf(key) ==
                                 keyList.Count - 1)
                            sb.Append( "\"" + kvp
                                .Value + "\"");
                        else sb.Append( "\"" + kvp.Value + "\"" + ";");
                    }
                }
                count++;

                if (count < dictList.Count())
                    sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}