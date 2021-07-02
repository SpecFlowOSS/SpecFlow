using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders
{
    public class CsvLoader : FileBasedLoader
    {
        public CsvLoader() : base("CSV", ".csv")
        {
        }
        
        protected override DataSource LoadDataSourceFromFilePath(string filePath, string sourceFilePath)
        {
            var culture = CultureInfo.InvariantCulture;
            var fileContent = ReadTextFileContent(filePath);
            var records = LoadCsvDataTable(fileContent, culture);
            return new DataSource(records);
        }

        internal DataTable LoadCsvDataTable(string fileContent, CultureInfo culture)
        {
            using var reader = new StringReader(fileContent);
            using var csv = new CsvReader(reader, new CsvConfiguration(culture)
            {
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                BadDataFound = args => throw new ExternalDataPluginException($"Invalid data found in CSV, in row {args.Context.Parser.RawRow}, near '{args.RawRecord}'")
            });
            
            csv.Read();
            csv.ReadHeader();
            var dataTable = new DataTable(csv.HeaderRecord);
            while (csv.Read())
            {
                var dataRecord = new DataRecord();
                foreach (string header in csv.HeaderRecord)
                {
                    dataRecord.Fields[header] = new DataValue(csv.GetField(header));
                }
                dataTable.Items.Add(dataRecord);
            }

            return dataTable;
        }
    }
}
