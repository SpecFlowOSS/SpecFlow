using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow
{
#if !BODI_LIMITEDRUNTIME
    [Serializable]
#endif
    public class Table
    {
        internal const string ERROR_NO_CELLS_TO_ADD = "No cells to add";
        internal const string ERROR_NO_HEADER_TO_ADD = "No headers to add";
        internal const string ERROR_COLUMN_NAME_NOT_FOUND = "Could not find a column named '{0}' in the table.";
        internal const string ERROR_CELLS_NOT_MATCHING_HEADERS = "The number of cells ({0}) you are trying to add doesn't match the number of columns ({1})";

        private readonly string[] header;
        private readonly TableRows rows = new TableRows();

        public ICollection<string> Header
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

        public bool ContainsColumn(string column)
        {
            return GetHeaderIndex(column, false) >= 0;
        }

        internal int GetHeaderIndex(string column, bool throwIfNotFound = true)
        {
            int index = Array.IndexOf(header, column);
            if (!throwIfNotFound)
                return index;
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

        public void AddRow(IDictionary<string, string> values)
        {
            string[] cells = new string[header.Length];
            foreach (var value in values)
            {
                int headerIndex = GetHeaderIndex(value.Key);
                cells[headerIndex] = value.Value;
            }

            AddRow(cells);
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
                        cells.Length,
                        header.Length,
                        this);
                throw new ArgumentException(mess);
            }
            var row = new TableRow(this, cells);
            rows.Add(row);
        }

        public void RenameColumn(string oldColumn, string newColumn)
        {
            int colIndex = GetHeaderIndex(oldColumn);
            header[colIndex] = newColumn;
        }

        public override string ToString()
        {
            return ToString(false, true);
        }

        public string ToString(bool headersOnly = false, bool withNewline = true)
        {
            int[] columnWidths = new int[header.Length];
            for (int colIndex = 0; colIndex < header.Length; colIndex++)
                columnWidths[colIndex] = header[colIndex].Length;

            if (!headersOnly)
            {
                foreach (TableRow row in rows)
                {
                    for (int colIndex = 0; colIndex < header.Length; colIndex++)
                        columnWidths[colIndex] = Math.Max(columnWidths[colIndex], row[colIndex].Length);
                }
            }

            StringBuilder builder = new StringBuilder();
            AddTableRow(builder, header, columnWidths);

            if (!headersOnly)
            {
                foreach (TableRow row in rows)
                    AddTableRow(builder, row.Select(pair => pair.Value), columnWidths);
            }

            if (!withNewline)
            {
                var newlineLength = Environment.NewLine.Length;
                builder.Remove(builder.Length - newlineLength, newlineLength);
            }

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

#if !BODI_LIMITEDRUNTIME
    [Serializable]
#endif
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

#if !BODI_LIMITEDRUNTIME
    [Serializable]
#endif
    public class TableRow : IDictionary<string, string>
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
            set
            {
                int keyIndex = table.GetHeaderIndex(header, true);
                items[keyIndex] = value;
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

        private SpecFlowException ThrowTableStructureCannotBeModified()
        {
            return new SpecFlowException("The table rows must contain the same number of items as the header count of the table. The structure cannot be modified.");
        }

        #region Implementation of ICollection<KeyValuePair<string,string>>

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            throw ThrowTableStructureCannotBeModified();
        }

        void ICollection<KeyValuePair<string, string>>.Clear()
        {
            throw ThrowTableStructureCannotBeModified();
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            int keyIndex = table.GetHeaderIndex(item.Key, false);
            if (keyIndex < 0)
                return false;
            return items[keyIndex].Equals(item.Value);
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw ThrowTableStructureCannotBeModified();
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            throw ThrowTableStructureCannotBeModified();
        }

        bool ICollection<KeyValuePair<string, string>>.IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Implementation of IDictionary<string,string>

        public bool ContainsKey(string key)
        {
            return table.Header.Contains(key);
        }

        void IDictionary<string, string>.Add(string key, string value)
        {
            throw ThrowTableStructureCannotBeModified();
        }

        bool IDictionary<string, string>.Remove(string key)
        {
            throw ThrowTableStructureCannotBeModified();
        }

        public bool TryGetValue(string key, out string value)
        {
            int keyIndex = table.GetHeaderIndex(key, false);
            if (keyIndex < 0)
            {
                value = null;
                return false;
            }

            value = items[keyIndex];
            return true;
        }

        public ICollection<string> Keys
        {
            get { return table.Header; }
        }

        public ICollection<string> Values
        {
            get { return items; }
        }

        #endregion
    }
}
