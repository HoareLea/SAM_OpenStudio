using System;
using System.Collections.Generic;
using System.Data;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static int TimeIndex(this DataTable dataTable, ShortDateTime peakDate, int environemntPeriodIndex, bool exactMatch = false)
        {
            if (dataTable == null || peakDate == null || environemntPeriodIndex == -1)
            {
                return -1;
            }

            DataColumnCollection dataColumnCollection = dataTable.Columns;
            if (dataColumnCollection == null)
            {
                return -1;
            }

            int index_EnvironmentPeriodIndex = dataColumnCollection.IndexOf("EnvironmentPeriodIndex");
            if (index_EnvironmentPeriodIndex == -1)
            {
                return -1;
            }

            int index_TimeIndex = dataColumnCollection.IndexOf("TimeIndex");
            if (index_TimeIndex == -1)
            {
                return -1;
            }

            int index_Year = dataColumnCollection.IndexOf("Year");
            if (index_Year == -1)
            {
                return -1;
            }

            int index_Month = dataColumnCollection.IndexOf("Month");
            if (index_Month == -1)
            {
                return -1;
            }

            int index_Day = dataColumnCollection.IndexOf("Day");
            if (index_Day == -1)
            {
                return -1;
            }

            int index_Hour = dataColumnCollection.IndexOf("Hour");
            if (index_Hour == -1)
            {
                return -1;
            }

            int index_Minute = dataColumnCollection.IndexOf("Minute");
            int index_Second = dataColumnCollection.IndexOf("Dst");

            DataRowCollection dataRowCollection = dataTable.Rows;
            if (dataRowCollection == null)
            {
                return -1;
            }

            List<Tuple<int, int>> tuples_Minute = new List<Tuple<int, int>>();
            List<Tuple<int, int>> tuples_Second = new List<Tuple<int, int>>();
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

                if (!Core.Query.TryConvert(dataRow[index_Year], out int year_Temp))
                {
                    continue;
                }

                if (year_Temp != 0)
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_Month], out byte month_Temp) || !peakDate.Month.Equals(month_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_Day], out byte day_Temp) || !peakDate.Day.Equals(day_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_Hour], out byte hour_Temp) || !peakDate.Hour.Equals(hour_Temp))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_TimeIndex], out int result))
                {
                    continue;
                }

                if (index_Minute != -1)
                {
                    if(!Core.Query.TryConvert(dataRow[index_Minute], out byte minute_Temp))
                    {
                        continue;
                    }

                    if(!peakDate.Minute.Equals(minute_Temp))
                    {
                        if (!exactMatch)
                        {
                            tuples_Minute.Add(new Tuple<int, int>(result, minute_Temp));
                        }

                        continue;
                    }
                }

                if (index_Second != -1)
                {
                    if(!Core.Query.TryConvert(dataRow[index_Second], out byte second_Temp))
                    {
                        continue;
                    }

                    if(!peakDate.Second.Equals(second_Temp))
                    {
                        if (!exactMatch)
                        {
                            tuples_Second.Add(new Tuple<int, int>(result, second_Temp));
                        }

                        continue;
                    }
                }

                return result;
            }

            if(tuples_Second != null && tuples_Second.Count != 0)
            {
                tuples_Second.Sort((x, y) => Math.Abs(x.Item2 - peakDate.Second).CompareTo(Math.Abs(x.Item2 - peakDate.Second)));
                return tuples_Second[0].Item1;
            }


            if (tuples_Minute != null && tuples_Minute.Count != 0)
            {
                tuples_Minute.Sort((x, y) => Math.Abs(x.Item2 - peakDate.Minute).CompareTo(Math.Abs(x.Item2 - peakDate.Minute)));
                return tuples_Minute[0].Item1;
            }

            return -1;
        }
    }
}