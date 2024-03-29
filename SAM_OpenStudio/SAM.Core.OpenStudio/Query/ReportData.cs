﻿using System.Data;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static double ReportData(this DataTable dataTable, int reportDataDictionaryIndex, int timeIndex)
        {
            if(dataTable == null || reportDataDictionaryIndex == -1)
            {
                return double.NaN;
            }

            DataColumnCollection dataColumnCollection = dataTable.Columns;
            if (dataColumnCollection == null)
            {
                return double.NaN;
            }

            int index_ReportDataDictionaryIndex = dataColumnCollection.IndexOf("ReportDataDictionaryIndex");
            if (index_ReportDataDictionaryIndex == -1)
            {
                return double.NaN;
            }

            int index_TimeIndex = dataColumnCollection.IndexOf("TimeIndex");
            if (index_TimeIndex == -1)
            {
                return double.NaN;
            }

            int index_Value = dataColumnCollection.IndexOf("Value");
            if (index_Value == -1)
            {
                return double.NaN;
            }

            DataRowCollection dataRowCollection = dataTable.Rows;
            if (dataRowCollection == null)
            {
                return double.NaN;
            }

            foreach (DataRow dataRow in dataRowCollection)
            {
                if (!Core.Query.TryConvert(dataRow[index_ReportDataDictionaryIndex], out int reportDataDictionaryIndex_Temp) || !reportDataDictionaryIndex.Equals(reportDataDictionaryIndex_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_TimeIndex], out int timeIndex_Temp) || !timeIndex.Equals(timeIndex_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_Value], out double value_Temp))
                {
                    continue;
                }

                return value_Temp;
            }

            return double.NaN;
        }
    }
}