using System;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources;
using DataTable = SpecFlow.ExternalData.SpecFlowPlugin.DataSources.DataTable;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Loaders
{
    public class ExcelLoader : FileBasedLoader
    {
        public ExcelLoader() : base("Excel", ".xlsx", ".xlsb", ".xls")
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        protected override DataSource LoadDataSourceFromFilePath(string filePath, string sourceFilePath)
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

            var result = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });

            var worksheetsRecord = new DataRecord();

            foreach (System.Data.DataTable resultTable in result.Tables)
            {
                var dataTable = new DataTable(resultTable.Columns.OfType<DataColumn>().Select(c => c.ColumnName).ToArray());

                foreach (DataRow resultTableRow in resultTable.Rows)
                {
                    var dataRecord = new DataRecord();
                    foreach (DataColumn column in resultTable.Columns)
                    {
                        dataRecord.Fields[column.ColumnName] = new DataValue(resultTableRow[column]);
                    }
                    dataTable.Items.Add(dataRecord);
                }

                worksheetsRecord.Fields[resultTable.TableName] = new DataValue(dataTable);
            }
            
            return new DataSource(worksheetsRecord, result.Tables[0].TableName);
        }
    }
}
