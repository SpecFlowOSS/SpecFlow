using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow
{
    public class Table
    {
        internal const string ERROR_NO_CELLS_TO_ADD = "No cells to add";
        internal const string ERROR_NO_HEADER_TO_ADD = "No headers to add";
        internal const string ERROR_COLUMN_NAME_NOT_FOUND = "Could not find a column named '{0}' in the table.";
        internal const string ERROR_CELLS_NOT_MATCHING_HEADERS = "The number of cells ({0}) you are trying to add doesn't match the number of columns ({1})";

        private readonly string[] header;
        private readonly TableRows rows = new TableRows();



        public IEnumerable<string> Header
        {
            get { return header; }
        }

        public TableRows Rows
        {
            get { return rows; }
        }

        public int RowCount
        {
            get { return rows.Count; }
        }

        public Table(params string[] header)
        {

            if (header == null || header.Length == 0)
            {
                throw new ArgumentException(ERROR_NO_HEADER_TO_ADD, "header");
            }
            for (int colIndex = 0; colIndex < header.Length; colIndex++)
                header[colIndex] = header[colIndex] ?? string.Empty;
            this.header = header;
        }

        internal int GetHeaderIndex(string column)
        {
            int index = Array.IndexOf(header, column);
            if (index < 0)
            {
                var mess = string.Format(
                            ERROR_COLUMN_NAME_NOT_FOUND + "\nThe table looks like this:\n{1}",
                            column,
                            this);
                throw new IndexOutOfRangeException(mess);
            }
            return index;
        }

        public void AddRow(params string[] cells)
        {
            if (cells == null)
                throw new Exception(ERROR_NO_CELLS_TO_ADD);

            if (cells.Length != header.Length)
            {
                var mess =
                    string.Format(
                        ERROR_CELLS_NOT_MATCHING_HEADERS + ".\nThe table looks like this\n{2}",
                        cells.Count(),
                        header.Length,
                        this);
                throw new ArgumentException(mess);
            }
            var row = new TableRow(this, cells);
            rows.Add(row);
        }

        public override string ToString()
        {
            int[] columnWidths = new int[header.Length];
            for (int colIndex = 0; colIndex < header.Length; colIndex++)
                columnWidths[colIndex] = header[colIndex].Length;

            foreach (TableRow row in rows)
            {
                for (int colIndex = 0; colIndex < header.Length; colIndex++)
                    columnWidths[colIndex] = Math.Max(columnWidths[colIndex], row[colIndex].Length);
            }

            StringBuilder builder = new StringBuilder();
            AddTableRow(builder, header, columnWidths);
            foreach (TableRow row in rows)
                AddTableRow(builder, row.Select(pair => pair.Value), columnWidths);
            return builder.ToString();
        }

        private void AddTableRow(StringBuilder builder, IEnumerable<string> cells, int[] widths)
        {
            const string margin = " ";
            const string separator = "|";
            int colIndex = 0;

            builder.Append(separator);
            foreach (string cell in cells)
            {
                builder.Append(margin);

                builder.Append(cell);
                builder.Append(' ', widths[colIndex] - cell.Length);

                builder.Append(margin);
                builder.Append(separator);

                colIndex++;
            }

            builder.AppendLine();
        }
    }

    public class TableRows : IEnumerable<TableRow>
    {
        private readonly List<TableRow> innerList = new List<TableRow>();

        public int Count { get { return innerList.Count; } }

        public TableRow this[int index]
        {
            get { return innerList[index]; }
        }

        public IEnumerator<TableRow> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void Add(TableRow row)
        {
            innerList.Add(row);
        }
    }

    public class TableRow : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Table table;
        private readonly string[] items;

        internal TableRow(Table table, string[] items)
        {
            for (int colIndex = 0; colIndex < items.Length; colIndex++)
                items[colIndex] = items[colIndex] ?? string.Empty;

            this.table = table;
            this.items = items;
        }

        public string this[string header]
        {
            get
            {
                int itemIndex = table.GetHeaderIndex(header);
                return items[itemIndex];
            }
        }

        public string this[int index]
        {
            get
            {
                return items[index];
            }
        }

        public int Count
        {
            get { return items.Length; }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            Debug.Assert(items.Length == table.Header.Count());
            int itemIndex = 0;
            foreach (string header in table.Header)
            {
                yield return new KeyValuePair<string, string>(header, items[itemIndex]);
                itemIndex++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}