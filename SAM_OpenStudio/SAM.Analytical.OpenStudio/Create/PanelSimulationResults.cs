using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Create
    {
        public static List<PanelSimulationResult> PanelSimulationResults(this string path, IEnumerable<SpaceSimulationResult> spaceSimulationResults = null)
        {
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            {
                return null;
            }

            List<PanelSimulationResult> result = null;
            using (SQLiteConnection sQLiteConnection = Core.SQLite.Create.SQLiteConnection(path))
            {
                result = PanelSimulationResults(sQLiteConnection, spaceSimulationResults);
            }

            return result;
        }

        public static List<PanelSimulationResult> PanelSimulationResults(this SQLiteConnection sQLiteConnection, IEnumerable<SpaceSimulationResult> spaceSimulationResults = null)
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

            if(spaceSimulationResults != null)
            {
                DataTable dataTable_ReportDataDictionary = Core.SQLite.Query.DataTable(sQLiteConnection, "ReportDataDictionary", "ReportDataDictionaryIndex", "KeyValue", "Name", "Units");
                DataTable dataTable_ReportData = Core.SQLite.Query.DataTable(sQLiteConnection, "ReportData", "ReportDataDictionaryIndex", "TimeIndex", "Value");

                string name_InsideConductionHeatTransfer = "Surface Inside Face Conduction Heat Transfer Rate";
                string name_OutsideConductionHeatTransfer = "Surface Outside Face Conduction Heat Transfer Rate";

                foreach (SpaceSimulationResult spaceSimulationResult in spaceSimulationResults)
                {
                    if(!spaceSimulationResult.TryGetValue(SpaceSimulationResultParameter.LoadTimeIndex, out int loadTimeIndex))
                    {
                        continue;
                    }

                    List<PanelSimulationResult> panelSimulationResults_Space = result.PanelSimulationResults(spaceSimulationResult);
                    if(panelSimulationResults_Space == null || panelSimulationResults_Space.Count == 0)
                    {
                        continue;
                    }

                    foreach (PanelSimulationResult panelSimulationResult in panelSimulationResults_Space)
                    {
                        int reportDataDictionaryIndex_InsideConductionHeatTransfer = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable_ReportDataDictionary, name_InsideConductionHeatTransfer, panelSimulationResult.Name);
                        if (reportDataDictionaryIndex_InsideConductionHeatTransfer != -1)
                        {
                            double value = Core.OpenStudio.Query.ReportData(dataTable_ReportData, reportDataDictionaryIndex_InsideConductionHeatTransfer, loadTimeIndex);
                            if (double.IsNaN(value))
                            {
                                continue;
                            }

                            value = Core.OpenStudio.Query.ConvertUnit(dataTable, name_InsideConductionHeatTransfer, spaceSimulationResult.Name, value);

                            panelSimulationResult.SetValue(Analytical.PanelSimulationResultParameter.InsideConductionHeatTransfer, value);
                        }

                        int reportDataDictionaryIndex_OutsideConductionHeatTransfer = Core.OpenStudio.Query.ReportDataDictionaryIndex(dataTable_ReportDataDictionary, name_OutsideConductionHeatTransfer, panelSimulationResult.Name);
                        if (reportDataDictionaryIndex_OutsideConductionHeatTransfer != -1)
                        {
                            double value = Core.OpenStudio.Query.ReportData(dataTable_ReportData, reportDataDictionaryIndex_OutsideConductionHeatTransfer, loadTimeIndex);
                            if (double.IsNaN(value))
                            {
                                continue;
                            }

                            value = Core.OpenStudio.Query.ConvertUnit(dataTable, name_OutsideConductionHeatTransfer, spaceSimulationResult.Name, value);

                            panelSimulationResult.SetValue(Analytical.PanelSimulationResultParameter.OutsideConductionHeatTransfer, value);
                        }
                    }
                }
            }

            return result;
        }
    }

}
