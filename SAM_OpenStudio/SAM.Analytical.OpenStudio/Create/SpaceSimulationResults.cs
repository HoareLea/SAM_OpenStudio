using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Create
    {
        public static List<SpaceSimulationResult> SpaceSimulationResults(this string path)
        {
            if(string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            {
                return null;
            }

            List<SpaceSimulationResult> result = null;
            using (SQLiteConnection sQLiteConnection = Core.SQLite.Create.SQLiteConnection(path))
            {
                DataTable dataTable = null;

                dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "Zones", "ZoneIndex", "ZoneName", "FloorArea", "Volume");
                if(dataTable != null)
                {
                    result = dataTable.ToSAM_SpaceSimulationResult();
                }

                if(result != null && result.Count != 0)
                {
                    DataTable dataTable_EnvironmentPeriods = Core.SQLite.Query.DataTable(sQLiteConnection, "EnvironmentPeriods", "EnvironmentPeriodIndex", "EnvironmentName");

                    dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "ZoneSizes", "ZoneName", "LoadType", "CalcDesLoad", "DesDayName","PeakHrMin", "PeakTemp", "PeakHumRat");
                    if (dataTable != null)
                    {
                        int index_ZoneName = dataTable.Columns.IndexOf("ZoneName");
                        int index_LoadType = dataTable.Columns.IndexOf("LoadType");
                        if (index_ZoneName != -1 && index_LoadType != -1)
                        {
                            int index_CalcDesLoad = dataTable.Columns.IndexOf("CalcDesLoad");
                            int index_PeakHrMin = dataTable.Columns.IndexOf("PeakHrMin");
                            int index_DesDayName = dataTable.Columns.IndexOf("DesDayName");

                            List<SpaceSimulationResult> spaceSimulationResults = new List<SpaceSimulationResult>();
                            foreach(SpaceSimulationResult spaceSimulationResult in result)
                            {
                                List<int> indexes = Core.Query.FindIndexes(dataTable, "ZoneName", spaceSimulationResult.Name);
                                if(indexes != null)
                                {
                                    foreach(int index in indexes)
                                    {
                                        DataRow dataRow = dataTable.Rows[index];

                                        SpaceSimulationResult spaceSimulationResult_LoadType = new SpaceSimulationResult(Guid.NewGuid(), spaceSimulationResult);
                                        spaceSimulationResult_LoadType.SetValue(Analytical.SpaceSimulationResultParameter.LoadType, dataRow[index_LoadType]);
                                        
                                        if(index_CalcDesLoad != -1)
                                        {
                                            spaceSimulationResult_LoadType.SetValue(Analytical.SpaceSimulationResultParameter.DesignLoad, dataRow[index_CalcDesLoad]);
                                        }

                                        if(index_DesDayName != -1)
                                        {
                                            if(spaceSimulationResult_LoadType.SetValue(SpaceSimulationResultParameter.DesignDayName, dataRow[index_DesDayName]))
                                            {
                                                if (dataTable_EnvironmentPeriods != null)
                                                {
                                                    int environmentPeriodIndex = Core.OpenStudio.Query.EnvironmentPeriodIndex(dataTable_EnvironmentPeriods, spaceSimulationResult_LoadType.GetValue<string>(SpaceSimulationResultParameter.DesignDayName));
                                                    if(environmentPeriodIndex != -1)
                                                    {
                                                        spaceSimulationResult_LoadType.SetValue(SpaceSimulationResultParameter.DesignDayIndex, environmentPeriodIndex);
                                                    }
                                                }
                                            }

                                        }

                                        if (index_PeakHrMin != -1)
                                        {
                                            if (Core.Query.TryConvert(dataRow[index_PeakHrMin], out string peakDate))
                                            {
                                                Core.OpenStudio.ShortDateTime shortDateTime = Core.OpenStudio.Create.ShortDateTime(peakDate);
                                                if(shortDateTime != null)
                                                {
                                                    spaceSimulationResult_LoadType.SetValue(SpaceSimulationResultParameter.PeakDate, shortDateTime);
                                                }
                                            }
                                        }

                                        spaceSimulationResults.Add(spaceSimulationResult_LoadType);
                                    }
                                }
                            }
                            result = spaceSimulationResults;
                        }

                    }

                    dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "NominalLighting", "ZoneIndex", "DesignLevel");
                    if(dataTable != null)
                    {
                        int index_ObjectName = dataTable.Columns.IndexOf("ZoneIndex");
                        int index_DesignLevel = dataTable.Columns.IndexOf("DesignLevel");
                        if (index_ObjectName != -1 && index_DesignLevel != -1)
                        {
                            foreach (SpaceSimulationResult spaceSimulationResult in result)
                            {
                                //if(!spaceSimulationResult.TryGetValue(SpaceSimulationResultParameter.Area, out double area))
                                //{
                                //    continue;
                                //}

                                if(!int.TryParse(spaceSimulationResult.Reference, out int zoneIndex))
                                {
                                    continue;
                                }

                                List<int> indexes = Core.Query.FindIndexes(dataTable, "ZoneIndex", zoneIndex);
                                if(indexes == null || indexes.Count ==0)
                                {
                                    continue;
                                }

                                if(!Core.Query.TryConvert(dataTable.Rows[indexes[0]][index_DesignLevel], out double designLevel))
                                {
                                    continue;
                                }

                                spaceSimulationResult.SetValue(Analytical.SpaceSimulationResultParameter.LightingGain, designLevel);
                            }
                        }
                    }

                    dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "NominalInfiltration", "ZoneIndex", "DesignLevel");
                    if (dataTable != null)
                    {
                        int index_ObjectName = dataTable.Columns.IndexOf("ZoneIndex");
                        int index_DesignLevel = dataTable.Columns.IndexOf("DesignLevel");
                        if (index_ObjectName != -1 && index_DesignLevel != -1)
                        {
                            foreach (SpaceSimulationResult spaceSimulationResult in result)
                            {
                                //if(!spaceSimulationResult.TryGetValue(SpaceSimulationResultParameter.Area, out double area))
                                //{
                                //    continue;
                                //}

                                if (!int.TryParse(spaceSimulationResult.Reference, out int zoneIndex))
                                {
                                    continue;
                                }

                                List<int> indexes = Core.Query.FindIndexes(dataTable, "ZoneIndex", zoneIndex);
                                if (indexes == null || indexes.Count == 0)
                                {
                                    continue;
                                }

                                if (!Core.Query.TryConvert(dataTable.Rows[indexes[0]][index_DesignLevel], out double designLevel))
                                {
                                    continue;
                                }

                                spaceSimulationResult.SetValue(Analytical.SpaceSimulationResultParameter.InfiltrationGain, designLevel);
                            }
                        }
                    }

                    dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "NominalElectricEquipment", "ZoneIndex", "DesignLevel");
                    if (dataTable != null)
                    {
                        int index_ObjectName = dataTable.Columns.IndexOf("ZoneIndex");
                        int index_DesignLevel = dataTable.Columns.IndexOf("DesignLevel");
                        if (index_ObjectName != -1 && index_DesignLevel != -1)
                        {
                            foreach (SpaceSimulationResult spaceSimulationResult in result)
                            {
                                //if(!spaceSimulationResult.TryGetValue(SpaceSimulationResultParameter.Area, out double area))
                                //{
                                //    continue;
                                //}

                                if (!int.TryParse(spaceSimulationResult.Reference, out int zoneIndex))
                                {
                                    continue;
                                }

                                List<int> indexes = Core.Query.FindIndexes(dataTable, "ZoneIndex", zoneIndex);
                                if (indexes == null || indexes.Count == 0)
                                {
                                    continue;
                                }

                                if (!Core.Query.TryConvert(dataTable.Rows[indexes[0]][index_DesignLevel], out double designLevel))
                                {
                                    continue;
                                }

                                spaceSimulationResult.SetValue(Analytical.SpaceSimulationResultParameter.EquipmentSensibleGain, designLevel);
                            }
                        }
                    }

                    dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "ReportDataDictionary", "ReportDataDictionaryIndex", "KeyValue", "Name");
                    if(dataTable != null)
                    {
                        DataTable dataTable_Time = Core.SQLite.Query.DataTable(sQLiteConnection, "Time", "TimeIndex", "Year", "Month", "Day", "Hour", "Minute", "Dst", "EnvironmentPeriodIndex");
                        DataTable dataTable_ReportData = Core.SQLite.Query.DataTable(sQLiteConnection, "ReportData", "ReportDataDictionaryIndex", "TimeIndex", "Value");

                        foreach (SpaceSimulationResult spaceSimulationResult in result)
                        {
                            if(spaceSimulationResult == null)
                            {
                                continue;
                            }

                            if(!spaceSimulationResult.TryGetValue(SpaceSimulationResultParameter.PeakDate, out Core.OpenStudio.ShortDateTime peakDate))
                            {
                                continue;
                            }

                            if (!spaceSimulationResult.TryGetValue(SpaceSimulationResultParameter.DesignDayIndex, out int designDayIndex))
                            {
                                continue;
                            }

                            LoadType loadType = spaceSimulationResult.LoadType();
                            if(loadType == LoadType.Undefined)
                            {
                                continue;
                            }
                            
                            string name = loadType == LoadType.Cooling ? "Zone Infiltration Sensible Heat Gain Energy" : "Zone Infiltration Sensible Heat Loss Energy";

                            int reportDataDictionaryIndex = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable, name, spaceSimulationResult.Name);
                            if(reportDataDictionaryIndex != -1)
                            {
                                SortedDictionary<int, double> reportData = Core.OpenStudio.Query.ReportData(dataTable_ReportData, reportDataDictionaryIndex);
                                int timeIndex = Core.OpenStudio.Query.TimeIndex(dataTable_Time, peakDate, designDayIndex);

                                if(!reportData.TryGetValue(timeIndex, out double value))
                                {
                                    continue;
                                }

                                if(loadType == LoadType.Cooling)
                                {
                                    value = -value;
                                }

                                spaceSimulationResult.SetValue(Analytical.SpaceSimulationResultParameter.InfiltrationGain, value);
                            }

                        }

                        
                    }

                }
            }

            return result;
        }
    }
}