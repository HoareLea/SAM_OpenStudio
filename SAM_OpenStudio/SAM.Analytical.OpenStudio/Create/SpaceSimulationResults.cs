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
                    dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "ZoneSizes", "ZoneName", "LoadType", "CalcDesLoad", "PeakHrMin", "PeakTemp", "PeakHumRat");
                    if (dataTable != null)
                    {
                        int index_ZoneName = dataTable.Columns.IndexOf("ZoneName");
                        int index_LoadType = dataTable.Columns.IndexOf("LoadType");
                        if (index_ZoneName != -1 && index_LoadType != -1)
                        {
                            int index_CalcDesLoad = dataTable.Columns.IndexOf("CalcDesLoad");
                            int index_PeakHrMin = dataTable.Columns.IndexOf("PeakHrMin");

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
                                        spaceSimulationResult_LoadType.SetValue(SpaceSimulationResultParameter.LoadType, dataRow[index_LoadType]);
                                        
                                        if(index_CalcDesLoad != -1)
                                        {
                                            spaceSimulationResult_LoadType.SetValue(SpaceSimulationResultParameter.DesignLoad, dataRow[index_CalcDesLoad]);
                                        }

                                        if (index_PeakHrMin != -1)
                                        {
                                            if (Core.Query.TryConvert(dataRow[index_PeakHrMin], out string dateString))
                                            {
                                                DateTime? dateTime = Convert.ToDateTime(dateString);
                                                if(dateTime != null && dateTime.HasValue)
                                                {
                                                    spaceSimulationResult_LoadType.SetValue(SpaceSimulationResultParameter.LoadIndex, Core.Query.HourOfYear(dateTime.Value));
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

                    dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "NominalLighting", "ObjectName", "DesignLevel");
                    if(dataTable != null)
                    {

                    }

                }
            }

            return result;
        }
    }
}