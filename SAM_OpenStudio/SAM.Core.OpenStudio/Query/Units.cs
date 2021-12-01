using System.Data;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static string Units(this DataTable dataTable, string name, string keyValue)
        {
            if(dataTable == null || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(keyValue))
            {
                return null;
            }

            DataColumnCollection dataColumnCollection = dataTable.Columns;
            if(dataColumnCollection == null)
            {
                return null;
            }

            int index_Units = dataColumnCollection.IndexOf("Units");
            if (index_Units == -1)
            {
                return null;
            }

            int index_Name = dataColumnCollection.IndexOf("Name");
            if(index_Name == -1)
            {
                return null;
            }

            int index_KeyValue = dataColumnCollection.IndexOf("KeyValue");
            if(index_KeyValue == -1)
            {
                return null;
            }

            DataRowCollection dataRowCollection = dataTable.Rows;
            if(dataRowCollection == null)
            {
                return null;
            }

            foreach(DataRow dataRow in dataRowCollection)
            {
                if(!Core.Query.TryConvert(dataRow[index_KeyValue], out string keyValue_Temp))
                {
                    continue;
                }

                if(!keyValue.Equals(keyValue_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_Name], out string name_Temp))
                {
                    continue;
                }

                if (!name.Equals(name_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_Units], out string result))
                {
                    continue;
                }

                return result;
            }

            return null;
        }
    }
}