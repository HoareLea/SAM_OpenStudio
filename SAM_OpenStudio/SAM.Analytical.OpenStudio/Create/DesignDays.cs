using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Create
    {
        public static List<DesignDay> DesignDays(this string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            {
                return null;
            }

            List<DesignDay> result = null;
            using (SQLiteConnection sQLiteConnection = Core.SQLite.Create.SQLiteConnection(path))
            {
                DataTable dataTable_EnvironmentPeriods = Core.SQLite.Query.DataTable(sQLiteConnection, "EnvironmentPeriods", "EnvironmentPeriodIndex", "EnvironmentName");
                DataTable dataTable_ReportDataDictionary = Core.SQLite.Query.DataTable(sQLiteConnection, "ReportDataDictionary", "ReportDataDictionaryIndex", "KeyValue", "Name", "Units");
                DataTable dataTable_Time = Core.SQLite.Query.DataTable(sQLiteConnection, "Time", "TimeIndex", "Year", "Month", "Day", "Hour", "Minute", "Dst", "EnvironmentPeriodIndex");
                DataTable dataTable_ReportData = Core.SQLite.Query.DataTable(sQLiteConnection, "ReportData", "ReportDataDictionaryIndex", "TimeIndex", "Value");

                foreach(DataRow dataRow in dataTable_EnvironmentPeriods.Rows)
                {
                    if(!Core.Query.TryConvert(dataRow["EnvironmentName"], out string environmentName))
                    {
                        continue;
                    }

                    if (!Core.Query.TryConvert(dataRow["EnvironmentPeriodIndex"], out int environmentPeriodIndex))
                    {
                        continue;
                    }

                    int reportDataDictionaryIndex = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable_ReportDataDictionary, "Site Total Sky Cover", "Environment");
                    if(reportDataDictionaryIndex == -1)
                    {
                        continue;
                    }
                    
                    SortedDictionary<int, double> reportDataDictionary = Core.OpenStudio.Query.ReportDataDictionary(dataTable_ReportDataDictionary, reportDataDictionaryIndex);
                    SortedDictionary<int, DateTime> timeIndexDictionary = Core.OpenStudio.Query.TimeIndexDictionary(dataTable_Time, environmentPeriodIndex);

                    List<Tuple<int, DateTime>> tuples = new List<Tuple<int, DateTime>>();
                    foreach(KeyValuePair<int, DateTime> keyValuePair in timeIndexDictionary)
                    {
                        tuples.Add(new Tuple<int, DateTime>(keyValuePair.Key, keyValuePair.Value));
                    }

                    if(tuples.Count == 0)
                    {
                        continue;
                    }

                    tuples.Sort((x, y) => x.Item2.CompareTo(y.Item2));

                    DesignDay designDay = new DesignDay(environmentName, System.Convert.ToInt16(tuples[0].Item2.Year), System.Convert.ToByte(tuples[0].Item2.Month), System.Convert.ToByte(tuples[0].Item2.Day));

                    int index = 0;
                    foreach(Tuple<int, DateTime> tuple in tuples)
                    {
                        designDay[Weather.WeatherDataType.CloudCover, index] = reportDataDictionary[tuple.Item1];
                    }

                    result.Add(designDay);
                }


            }

            return result;
        }
    }
}