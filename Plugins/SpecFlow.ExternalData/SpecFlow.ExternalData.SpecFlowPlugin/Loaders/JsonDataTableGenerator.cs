using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders;

public class JsonDataTableGenerator
{
    public DataTable FlattenDataSetToDataTable(JObject originalJson, string[] objectPath)
    {
        try
        {
            var header = GenerateHeader(originalJson, objectPath);

            var dataTable = new DataTable(header.ToArray());
            var records = GenerateRecordsFromNestedObjects(originalJson, objectPath, new List<DataRecord>());
            foreach (var record in records)
                dataTable.Items.Add(record);

            return dataTable;
        }
        catch (System.NullReferenceException)
        {
            throw new ExternalDataPluginException($"Unable to flatten {objectPath} into DataTable");
        }
    }

    private List<DataRecord> GenerateRecordsFromNestedObjects(JObject currentObject, string[] objectPath,
        List<DataRecord> currentRecords)
    {
        if (objectPath.Length == 0)
            return currentRecords;

        if (currentObject[objectPath.First()] == null) 
            return currentRecords;

        var objectArray = currentObject[objectPath.First()].ToObject<JArray>();
        var remainingObjects = objectPath.Skip(1).ToArray();

        var result = new List<DataRecord>();
        foreach (var jObject in objectArray.Select(i => i.ToObject<JObject>()))
        {
            var intermediateResult = new List<DataRecord>();
            var objectProperties = GetPropertiesExcludingNestedArrayObjects(jObject, remainingObjects);

            var objectRecord = new DataRecord();
            foreach (var property in objectProperties)
                objectRecord.Fields[property.Name] = new DataValue(property.Value);

            if (currentRecords.Any())
            {
                foreach (var record in currentRecords)
                {
                    var updatedRecord = new DataRecord(record.Fields);
                    objectRecord.Fields.ToList().ForEach(x => updatedRecord.Fields.Add(x.Key, x.Value));
                    intermediateResult.Add(updatedRecord);
                }
            }
            else
                intermediateResult.Add(objectRecord);

            result.AddRange(GenerateRecordsFromNestedObjects(jObject, remainingObjects, intermediateResult));
        }

        return result;
    }

    private static List<string> GenerateHeader(JObject originalJson, string[] objectPath)
    {
        var currentObject = originalJson;
      
        List<string> header = new();
        while (objectPath.Length > 0)
        {
            var currentObjectArray = currentObject[objectPath.First()]?.ToObject<JArray>();

            if(currentObjectArray == null)
                throw new ExternalDataPluginException($"Expected {objectPath.First()} to be in json object");

            objectPath = objectPath.Skip(1).ToArray();
        
            if(currentObjectArray.First == null)
                throw new ExternalDataPluginException("Empty object arrays are not supported");

            currentObject = currentObjectArray.First.ToObject<JObject>();
            header.AddRange(GetPropertiesExcludingNestedArrayObjects(currentObject, objectPath).Select(p => p.Name));
        }

        return header;
    }


    private static List<JProperty> GetPropertiesExcludingNestedArrayObjects(JObject childJson,
        string[] remainingDataSets)
    {
        return childJson.Properties().Where(p => p.Name != remainingDataSets.FirstOrDefault()).ToList();
    }
}