using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
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
            var records = LoadCsvDataList(fileContent, culture);
            return new DataValue(records);
        }

        private string ReadTextFileContent(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        private string ResolveFilePath(string path, string sourceFilePath)
        {
            return path;//TODO
        }

        private DataList LoadCsvDataList(string fileContent, CultureInfo culture)
        {
            var dataList = new DataList();
            using var reader = new StringReader(fileContent);
            using var csv = new CsvReader(reader, culture);
            
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var dataRecord = new DataRecord();
                foreach (string header in csv.HeaderRecord)
                {
                    dataRecord.Fields[header] = new DataValue(csv.GetField(header));
                }
                dataList.Items.Add(new DataValue(dataRecord));
            }

            return dataList;
        }
    }
}
