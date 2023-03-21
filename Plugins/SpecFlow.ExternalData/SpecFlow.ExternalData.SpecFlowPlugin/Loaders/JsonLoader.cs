using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources;
using Newtonsoft.Json;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders;

public class JsonLoader : FileBasedLoader
{
    public JsonLoader() : base("Json", ".json")
    {
                
    }

    protected override DataSource LoadDataSourceFromFilePath(string filePath, string sourceFilePath)
    {
        var fileContent = ReadTextFileContent(filePath);

        return LoadJsonDataSource(fileContent, sourceFilePath);
    }

    private List<string> DetermineDataSets(JObject jsonObject, string dataSetPrepend)
    {
        var dataSets = new HashSet<string>();
        foreach (var array in GetArraysFromObject(jsonObject))
        { 
            var firstArrayObject = array.Children().First().ToObject<JObject>();
            if (firstArrayObject == null)
                continue;

            var dataSetPath = string.IsNullOrWhiteSpace(dataSetPrepend) ? array.Path : $"{dataSetPrepend}.{array.Path}";
                
            dataSets.Add(dataSetPath);

            foreach (var nestedDataSetPath in DetermineDataSets(firstArrayObject, dataSetPath)) 
                dataSets.Add(nestedDataSetPath); 
        }

        return dataSets.ToList();
    }

    private static IEnumerable<JToken> GetArraysFromObject(JObject jsonObject)
    {
        return jsonObject.Properties().Where(p => p.Value.Type == JTokenType.Array).Select(a => a.Value);
    }

    private DataSource LoadJsonDataSource(string fileContent, string sourceFilePath)
    {
        JObject fileJson = ParseJson(fileContent);
        var dataSets = DetermineDataSets(fileJson, "");
        var dataSetsRecord = new DataRecord();
        var nestedObjectsHelper = new JsonDataTableGenerator();
        foreach (var dataSet in dataSets)
        {
            var dataTable = nestedObjectsHelper.FlattenDataSetToDataTable(fileJson, dataSet.Split('.'));
            dataSetsRecord.Fields[dataSet] = new DataValue(dataTable);
        }
            
        return new DataSource(dataSetsRecord, dataSets.First());
    }

    private static JObject ParseJson(string fileContent)
    {
        JObject fileJson;
        try
        {
            fileJson = JObject.Parse(fileContent);
        }
        catch (JsonReaderException jsonReaderException)
        {
            throw new ExternalDataPluginException($"Failed to parse json file: {jsonReaderException}");
        }

        return fileJson;
    }
}