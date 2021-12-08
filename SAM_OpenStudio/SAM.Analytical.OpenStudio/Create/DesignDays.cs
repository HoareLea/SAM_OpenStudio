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


                result = new List<DesignDay>();
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

                    int reportDataDictionaryIndex = -1;

                    List<Tuple<Weather.WeatherDataType, string>> tuples_WeatherDataType = new List<Tuple<Weather.WeatherDataType, string>>();
                    tuples_WeatherDataType.Add(new Tuple<Weather.WeatherDataType, string>(Weather.WeatherDataType.CloudCover, "Site Total Sky Cover"));
                    tuples_WeatherDataType.Add(new Tuple<Weather.WeatherDataType, string>(Weather.WeatherDataType.DirectSolarRadiation, "Site Direct Solar Radiation Rate per Area"));
                    tuples_WeatherDataType.Add(new Tuple<Weather.WeatherDataType, string>(Weather.WeatherDataType.DiffuseSolarRadiation, "Site Diffuse Solar Radiation Rate per Area"));
                    tuples_WeatherDataType.Add(new Tuple<Weather.WeatherDataType, string>(Weather.WeatherDataType.WindDirection, "Site Wind Direction"));
                    tuples_WeatherDataType.Add(new Tuple<Weather.WeatherDataType, string>(Weather.WeatherDataType.WindSpeed, "Site Wind Speed"));
                    tuples_WeatherDataType.Add(new Tuple<Weather.WeatherDataType, string>(Weather.WeatherDataType.RelativeHumidity, "Site Outdoor Air Relative Humidity"));
                    tuples_WeatherDataType.Add(new Tuple<Weather.WeatherDataType, string>(Weather.WeatherDataType.DryBulbTemperature, "Site Outdoor Air Drybulb Temperature"));

                    foreach(Tuple<Weather.WeatherDataType, string> tuple in tuples_WeatherDataType)
                    {
                        double factor = 1;
                        if(tuple.Item1 == Weather.WeatherDataType.CloudCover)
                        {
                            factor = 8;
                        }
                        
                        reportDataDictionaryIndex = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable_ReportDataDictionary, tuple.Item2, "Environment");
                        if (reportDataDictionaryIndex != -1)
                        {
                            SortedDictionary<int, double> reportDataDictionary = Core.OpenStudio.Query.ReportDataDictionary(dataTable_ReportData, reportDataDictionaryIndex);
                            for (int j = 0; j < tuples.Count; j++)
                            {
                                designDay[tuple.Item1, j] = reportDataDictionary[tuples[j].Item1] / factor;
                            }
                        }
                    }

                    result.Add(designDay);
                }


            }

            return result;
        }
    }
}