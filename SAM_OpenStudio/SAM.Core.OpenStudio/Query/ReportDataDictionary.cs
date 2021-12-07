using System.Collections.Generic;
using System.Data;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static SortedDictionary<int, double> ReportDataDictionary(this DataTable dataTable, int reportDataDictionaryIndex, int minTimeIndex = -1, int maxTimeIndex = -1)
        {
            if(dataTable == null || reportDataDictionaryIndex == -1)
            {
                return null;
            }

            DataColumnCollection dataColumnCollection = dataTable.Columns;
            if (dataColumnCollection == null)
            {
                return null;
            }

            int index_ReportDataDictionaryIndex = dataColumnCollection.IndexOf("ReportDataDictionaryIndex");
            if (index_ReportDataDictionaryIndex == -1)
            {
                return null;
            }

            int index_TimeIndex = dataColumnCollection.IndexOf("TimeIndex");
            if (index_TimeIndex == -1)
            {
                return null;
            }

            int index_Value = dataColumnCollection.IndexOf("Value");
            if (index_Value == -1)
            {
                return null;
            }

            DataRowCollection dataRowCollection = dataTable.Rows;
            if (dataRowCollection == null)
            {
                return null;
            }

            SortedDictionary<int, double> result = new SortedDictionary<int, double>();
            foreach (DataRow dataRow in dataRowCollection)
            {
                if (!Core.Query.TryConvert(dataRow[index_ReportDataDictionaryIndex], out int reportDataDictionaryIndex_Temp))
                {
                    continue;
                }

                if(!reportDataDictionaryIndex.Equals(reportDataDictionaryIndex_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_Value], out double value_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_TimeIndex], out int timeIndex_Temp))
                {
                    continue;
                }

                if(minTimeIndex != -1 && timeIndex_Temp < minTimeIndex)
                {
                    continue;
                }

                if (maxTimeIndex != -1 && timeIndex_Temp > maxTimeIndex)
                {
                    continue;
                }

                result[timeIndex_Temp] = value_Temp;
            }

            return result;
        }
    }
}