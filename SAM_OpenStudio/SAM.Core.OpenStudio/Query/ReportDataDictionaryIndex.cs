using System.Data;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static int ReportDataDictionaryIndex(this DataTable dataTable, string name, string keyValue, TextComparisonType textComparisonType = TextComparisonType.Equals)
        {
            if(dataTable == null || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(keyValue))
            {
                return -1;
            }

            DataColumnCollection dataColumnCollection = dataTable.Columns;
            if(dataColumnCollection == null)
            {
                return -1;
            }

            int index_ReportDataDictionaryIndex = dataColumnCollection.IndexOf("ReportDataDictionaryIndex");
            if (index_ReportDataDictionaryIndex == -1)
            {
                return -1;
            }

            int index_Name = dataColumnCollection.IndexOf("Name");
            if(index_Name == -1)
            {
                return -1;
            }

            int index_KeyValue = dataColumnCollection.IndexOf("KeyValue");
            if(index_KeyValue == -1)
            {
                return -1;
            }

            DataRowCollection dataRowCollection = dataTable.Rows;
            if(dataRowCollection == null)
            {
                return -1;
            }

            foreach(DataRow dataRow in dataRowCollection)
            {
                if (!Core.Query.TryConvert(dataRow[index_Name], out string name_Temp))
                {
                    continue;
                }

                if (!name.Equals(name_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_KeyValue], out string keyValue_Temp))
                {
                    continue;
                }

                if(!Core.Query.Compare(keyValue_Temp, keyValue, textComparisonType))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_ReportDataDictionaryIndex], out int result))
                {
                    continue;
                }

                return result;
            }

            return -1;
        }
    }
}