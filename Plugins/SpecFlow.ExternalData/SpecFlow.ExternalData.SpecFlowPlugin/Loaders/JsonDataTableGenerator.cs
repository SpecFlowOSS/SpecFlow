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
            header.AddRange(GetPropertiesExcludingToBeFlattenedArrayObject(currentObject, objectPath).Select(p => p.Name));
        }

        return header;
    }

    private List<DataRecord> GenerateRecordsFromNestedObjects(JObject currentObject, string[] dataSetObjectPath,
        List<DataRecord> currentRecords)
    {
        if (dataSetObjectPath.Length == 0)
            return currentRecords;

        var objectArrayPropertyName = dataSetObjectPath.First();
        var remainingObjectArrays = dataSetObjectPath.Skip(1).ToArray();

        var objectArray = currentObject[objectArrayPropertyName]?.ToObject<JArray>();
        if (objectArray == null) 
            throw new ExternalDataPluginException($"Expected object array property {objectArrayPropertyName} inside {currentObject}");
        
        var result = new List<DataRecord>();
        foreach (var jObject in objectArray.Select(i => i.ToObject<JObject>()))
        {
            
            var objectProperties = GetPropertiesExcludingToBeFlattenedArrayObject(jObject, remainingObjectArrays);

            var objectRecord = new DataRecord();
            foreach (var property in objectProperties)
                objectRecord.Fields[property.Name] = new DataValue(property.Value);

            List<DataRecord> intermediateResult;
            if (currentRecords.Any())
                intermediateResult = AppendFieldsToCurrentDataRecords(currentRecords, objectRecord);
            else
                intermediateResult = new List<DataRecord> { objectRecord };

            result.AddRange(GenerateRecordsFromNestedObjects(jObject, remainingObjectArrays, intermediateResult));
        }

        return result;
    }

    private static List<DataRecord> AppendFieldsToCurrentDataRecords(List<DataRecord> currentRecords, DataRecord objectRecord)
    {
        var result = new List<DataRecord>();
        foreach (var record in currentRecords)
        {
            var updatedRecord = new DataRecord(record.Fields);
            objectRecord.Fields.ToList().ForEach(x => updatedRecord.Fields.Add(x.Key, x.Value));
            result.Add(updatedRecord);
        }

        return result;
    }
    
    private static List<JProperty> GetPropertiesExcludingToBeFlattenedArrayObject(JObject childJson,
        string[] remainingDataSets)
    {
        return childJson.Properties().Where(p => p.Name != remainingDataSets.FirstOrDefault()).ToList();
    }
}