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

                    dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "ReportDataDictionary", "ReportDataDictionaryIndex", "KeyValue", "Name", "Units");
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

                            int timeIndex = Core.OpenStudio.Query.TimeIndex(dataTable_Time, peakDate, designDayIndex);
                            if(timeIndex == -1)
                            {
                                continue;
                            }

                            //Infiltration
                            string name_Infiltration = loadType == LoadType.Cooling ? "Zone Infiltration Sensible Heat Gain Energy" : "Zone Infiltration Sensible Heat Loss Energy";
                            int reportDataDictionaryIndex_Infiltration = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable, name_Infiltration, spaceSimulationResult.Name);
                            if(reportDataDictionaryIndex_Infiltration != -1)
                            {
                                double value = Core.OpenStudio.Query.ReportData(dataTable_ReportData, reportDataDictionaryIndex_Infiltration, timeIndex);
                                if(double.IsNaN(value))
                                {
                                    continue;
                                }

                                value = Core.OpenStudio.Query.ConvertUnit(dataTable, name_Infiltration, spaceSimulationResult.Name, value);

                                if (loadType == LoadType.Heating)
                                {
                                    value = -value;
                                }

                                spaceSimulationResult.SetValue(Analytical.SpaceSimulationResultParameter.InfiltrationGain, value);
                            }

                            //Equipment Sensible
                            string name_EquipmentRadiant = "Zone Electric Equipment Radiant Heating Rate";
                            string name_EquipmentConvective = "Zone Electric Equipment Convective Heating Rate";
                            int reportDataDictionaryIndex_EquipmentRadiant = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable, name_EquipmentRadiant, spaceSimulationResult.Name);
                            int reportDataDictionaryIndex_EquipmentConvective = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable, name_EquipmentConvective, spaceSimulationResult.Name);
                            if(reportDataDictionaryIndex_EquipmentConvective != -1 || reportDataDictionaryIndex_EquipmentRadiant != -1)
                            {
                                double value_EquipmentRadiant = Core.OpenStudio.Query.ReportData(dataTable_ReportData, reportDataDictionaryIndex_EquipmentRadiant, timeIndex);
                                double value_EquipmentConvective = Core.OpenStudio.Query.ReportData(dataTable_ReportData, reportDataDictionaryIndex_EquipmentConvective, timeIndex);
                                if(!double.IsNaN(value_EquipmentRadiant) || !double.IsNaN(value_EquipmentConvective))
                                {
                                    double value = 0;
                                    if(!double.IsNaN(value_EquipmentRadiant))
                                    {
                                        value_EquipmentRadiant = Core.OpenStudio.Query.ConvertUnit(dataTable, name_EquipmentRadiant, spaceSimulationResult.Name, value_EquipmentRadiant);
                                        value += value_EquipmentRadiant;
                                    }

                                    if (!double.IsNaN(value_EquipmentConvective))
                                    {
                                        value_EquipmentConvective = Core.OpenStudio.Query.ConvertUnit(dataTable, name_EquipmentRadiant, spaceSimulationResult.Name, value_EquipmentConvective);
                                        value += value_EquipmentConvective;
                                    }

                                    spaceSimulationResult.SetValue(Analytical.SpaceSimulationResultParameter.EquipmentSensibleGain, value);
                                }

                            }

                            //People Sensible
                            string name_PeopleSensible = "Zone People Sensible Heating Rate";
                            int reportDataDictionaryIndex_PeopleSensible = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable, name_PeopleSensible, spaceSimulationResult.Name);
                            if(reportDataDictionaryIndex_PeopleSensible != -1)
                            {
                                double value = Core.OpenStudio.Query.ReportData(dataTable_ReportData, reportDataDictionaryIndex_PeopleSensible, timeIndex);
                                if (double.IsNaN(value))
                                {
                                    continue;
                                }

                                value = Core.OpenStudio.Query.ConvertUnit(dataTable, name_PeopleSensible, spaceSimulationResult.Name, value);
                                spaceSimulationResult.SetValue(Analytical.SpaceSimulationResultParameter.OccupancySensibleGain, value);
                            }

                            //People Latent
                            string name_PeopleLatent = "Zone People Latent Gain Rate";
                            int reportDataDictionaryIndex_PeopleLatent = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable, name_PeopleLatent, spaceSimulationResult.Name);
                            if (reportDataDictionaryIndex_PeopleLatent != -1)
                            {
                                double value = Core.OpenStudio.Query.ReportData(dataTable_ReportData, reportDataDictionaryIndex_PeopleLatent, timeIndex);
                                if (double.IsNaN(value))
                                {
                                    continue;
                                }

                                value = Core.OpenStudio.Query.ConvertUnit(dataTable, name_PeopleLatent, spaceSimulationResult.Name, value);
                                spaceSimulationResult.SetValue(Analytical.SpaceSimulationResultParameter.OccupancyLatentGain, value);
                            }

                            //Lighting
                            string name_Lighting = "Zone Lights Total Heating Rate";
                            int reportDataDictionaryIndex_Lighting = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable, name_Lighting, spaceSimulationResult.Name);
                            if (reportDataDictionaryIndex_Lighting != -1)
                            {
                                double value = Core.OpenStudio.Query.ReportData(dataTable_ReportData, reportDataDictionaryIndex_Lighting, timeIndex);
                                if (double.IsNaN(value))
                                {
                                    continue;
                                }

                                value = Core.OpenStudio.Query.ConvertUnit(dataTable, name_Lighting, spaceSimulationResult.Name, value);
                                spaceSimulationResult.SetValue(Analytical.SpaceSimulationResultParameter.LightingGain, value);
                            }
                        }

                        
                    }

                }
            }

            return result;
        }
    }
}