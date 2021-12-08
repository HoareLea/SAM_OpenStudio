﻿using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Create
    {
        public static List<PanelSimulationResult> PanelSimulationResults(this string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            {
                return null;
            }

            List<PanelSimulationResult> result = null;
            using (SQLiteConnection sQLiteConnection = Core.SQLite.Create.SQLiteConnection(path))
            {
                result = PanelSimulationResults(sQLiteConnection);
            }

            return result;
        }

        public static List<PanelSimulationResult> PanelSimulationResults(this SQLiteConnection sQLiteConnection)
        {
            if(sQLiteConnection == null)
            {
                return null;
            }

            DataTable dataTable = null;

            List<PanelSimulationResult> result = null;

            dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "Surfaces", "SurfaceIndex", "SurfaceName", "Area", "ZoneIndex");
            if (dataTable != null)
            {
                result = dataTable.ToSAM_PanelSimulationResult();
            }

            dataTable = Core.SQLite.Query.DataTable(sQLiteConnection, "Zones", "ZoneIndex", "ZoneName");
            if (dataTable != null)
            {
                int index_ZoneIndex = dataTable.Columns.IndexOf("ZoneIndex");
                int index_ZoneName = dataTable.Columns.IndexOf("ZoneName");
                if (index_ZoneName != -1 && index_ZoneIndex != -1)
                {
                    DataRowCollection dataRowCollection = dataTable.Rows;
                    if(dataRowCollection != null)
                    {
                        Dictionary<int, List<PanelSimulationResult>> dictionary = new Dictionary<int, List<PanelSimulationResult>>();
                        foreach(PanelSimulationResult panelSimulationResult in result)
                        {
                            if(!panelSimulationResult.TryGetValue(PanelSimulationResultParameter.ZoneIndex, out int zoneIndex))
                            {
                                continue;
                            }

                            if(!dictionary.TryGetValue(zoneIndex, out List<PanelSimulationResult> panelSimulationResults))
                            {
                                panelSimulationResults = new List<PanelSimulationResult>();
                                dictionary[zoneIndex] = panelSimulationResults;
                            }

                            panelSimulationResults.Add(panelSimulationResult);
                        }


                        foreach(DataRow dataRow in dataRowCollection)
                        {
                            if(!Core.Query.TryConvert(dataRow[index_ZoneIndex], out int zoneIndex))
                            {
                                continue;
                            }

                            if(!dictionary.TryGetValue(zoneIndex, out List<PanelSimulationResult> panelSimulationResults))
                            {
                                continue;
                            }

                            if (!Core.Query.TryConvert(dataRow[index_ZoneName], out int zoneName))
                            {
                                continue;
                            }

                            foreach (PanelSimulationResult panelSimulationResult in panelSimulationResults)
                            {
                                panelSimulationResult.SetValue(PanelSimulationResultParameter.ZoneName, zoneName);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }

}