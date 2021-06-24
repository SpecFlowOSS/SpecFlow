using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSource;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders
{
    public class CsvLoader
    {
        public DataValue LoadDataSource(string path, string sourceFilePath, CultureInfo culture)
        {
            culture ??= CultureInfo.CurrentCulture;
            
            var filePath = ResolveFilePath(path, sourceFilePath);
            var fileContent = ReadTextFileContent(filePath);
            var records = LoadCsvDataTableWithErrorHandling(fileContent, culture, filePath);
            return new DataValue(records);
        }

        private DataTable LoadCsvDataTableWithErrorHandling(string fileContent, CultureInfo culture, string filePath)
        {
            try
            {
                return LoadCsvDataTable(fileContent, culture);
            }
            catch (Exception ex)
            {
                throw new ExternalDataPluginException($"Unable to load CSV file from '{filePath}': {ex.Message}", ex);
            }
        }

        private string ReadTextFileContent(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                throw new ExternalDataPluginException($"Unable to load CSV file from '{filePath}': {ex.Message}", ex);
            }
        }

        private string ResolveFilePath(string path, string sourceFilePath)
        {
            if (!string.IsNullOrEmpty(sourceFilePath))
            {
                string sourceDirectoryName = Path.GetDirectoryName(sourceFilePath);
                if (sourceDirectoryName != null)
                    return Path.GetFullPath(Path.Combine(sourceDirectoryName, path));
            }
            return path;
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
