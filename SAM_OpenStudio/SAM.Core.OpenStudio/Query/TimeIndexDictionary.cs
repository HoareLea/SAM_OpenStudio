using System;
using System.Collections.Generic;
using System.Data;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static SortedDictionary<int, DateTime> TimeIndexDictionary(this DataTable dataTable, int environemntPeriodIndex, short defaultYear = 2017)
        {
            if (dataTable == null || environemntPeriodIndex == -1)
            {
                return null;
            }

            DataColumnCollection dataColumnCollection = dataTable.Columns;
            if (dataColumnCollection == null)
            {
                return null;
            }

            int index_EnvironmentPeriodIndex = dataColumnCollection.IndexOf("EnvironmentPeriodIndex");
            if (index_EnvironmentPeriodIndex == -1)
            {
                return null;
            }

            int index_TimeIndex = dataColumnCollection.IndexOf("TimeIndex");
            if (index_TimeIndex == -1)
            {
                return null;
            }

            int index_Year = dataColumnCollection.IndexOf("Year");
            if (index_Year == -1)
            {
                return null;
            }

            int index_Month = dataColumnCollection.IndexOf("Month");
            if (index_Month == -1)
            {
                return null;
            }

            int index_Day = dataColumnCollection.IndexOf("Day");
            if (index_Day == -1)
            {
                return null;
            }

            int index_Hour = dataColumnCollection.IndexOf("Hour");
            if (index_Hour == -1)
            {
                return null;
            }

            int index_Minute = dataColumnCollection.IndexOf("Minute");
            int index_Second = dataColumnCollection.IndexOf("Dst");

            DataRowCollection dataRowCollection = dataTable.Rows;
            if (dataRowCollection == null)
            {
                return null;
            }

            SortedDictionary<int, DateTime> result = new SortedDictionary<int, DateTime>();
            foreach (DataRow dataRow in dataRowCollection)
            {
                if (!Core.Query.TryConvert(dataRow[index_EnvironmentPeriodIndex], out int environemntPeriodIndex_Temp))
                {
                    continue;
                }

                if (!environemntPeriodIndex.Equals(environemntPeriodIndex_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_Year], out int year))
                {
                    continue;
                }

                if (year == 0)
                {
                    year = defaultYear;
                }

                if (!Core.Query.TryConvert(dataRow[index_Month], out byte month))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_Day], out byte day))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_Hour], out byte hour))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_TimeIndex], out int timeIndex))
                {
                    continue;
                }

                byte minute = 0;
                if (index_Minute != -1)
                {
                    if (!Core.Query.TryConvert(dataRow[index_Minute], out minute))
                    {
                        continue;
                    }
                }

                byte second = 0;
                if (index_Second != -1)
                {
                    if (!Core.Query.TryConvert(dataRow[index_Second], out second))
                    {
                        continue;
                    }
                }

                result[timeIndex] = new DateTime(year, month, day, hour - 1, minute, second);
            }

            return result;
        }
    }
}