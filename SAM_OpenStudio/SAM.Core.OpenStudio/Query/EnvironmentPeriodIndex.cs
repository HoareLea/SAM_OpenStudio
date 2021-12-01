using System.Data;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static int EnvironmentPeriodIndex(this DataTable dataTable, string environmentName)
        {
            if(dataTable == null || string.IsNullOrWhiteSpace(environmentName))
            {
                return -1;
            }

            DataColumnCollection dataColumnCollection = dataTable.Columns;
            if(dataColumnCollection == null)
            {
                return -1;
            }

            int index_EnvironmentPeriodIndex = dataColumnCollection.IndexOf("EnvironmentPeriodIndex");
            if (index_EnvironmentPeriodIndex == -1)
            {
                return -1;
            }

            int index_EnvironmentName = dataColumnCollection.IndexOf("EnvironmentName");
            if(index_EnvironmentName == -1)
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
                if (!Core.Query.TryConvert(dataRow[index_EnvironmentName], out string environmentName_Temp))
                {
                    continue;
                }

                if (!environmentName.Equals(environmentName_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_EnvironmentPeriodIndex], out int result))
                {
                    continue;
                }

                return result;
            }

            return -1;
        }
    }
}