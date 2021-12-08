using System.Collections.Generic;
using System.Data;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Convert
    {
        public static List<PanelSimulationResult> ToSAM_PanelSimulationResult(this DataTable dataTable)
        {
            DataRowCollection dataRowCollection = dataTable?.Rows;
            if (dataRowCollection == null)
            {
                return null;
            }

            DataColumnCollection dataColumnCollection = dataTable.Columns;
            if (dataColumnCollection == null)
            {
                return null;
            }

            int index_SurfaceIndex = dataColumnCollection.IndexOf("SurfaceIndex");
            if(index_SurfaceIndex == -1)
            {
                return null;
            }

            int index_SurfaceName = dataColumnCollection.IndexOf("SurfaceName");
            if (index_SurfaceName == -1)
            {
                return null;
            }

            int index_Area = dataColumnCollection.IndexOf("Area");
            int index_ZoneIndex = dataColumnCollection.IndexOf("ZoneIndex");

            string source = Query.Source();

            List<PanelSimulationResult> result = new List<PanelSimulationResult>();
            foreach (DataRow dataRow in dataRowCollection)
            {
                if(dataRow == null)
                {
                    continue;
                }

                object[] values = dataRow.ItemArray;

                if(!Core.Query.TryConvert(values[index_SurfaceIndex], out int surfaceIndex))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(values[index_SurfaceName], out string surfaceName))
                {
                    continue;
                }

                PanelSimulationResult spaceSimulationResult = new PanelSimulationResult(surfaceName, source, surfaceIndex.ToString());
                
                if(index_Area != -1 && Core.Query.TryConvert(values[index_Area], out double area))
                {
                    spaceSimulationResult.SetValue(Analytical.PanelSimulationResultParameter.Area, area);
                }

                if (index_ZoneIndex != -1 && Core.Query.TryConvert(values[index_ZoneIndex], out int zoneIndex))
                {
                    spaceSimulationResult.SetValue(PanelSimulationResultParameter.ZoneIndex, zoneIndex);
                }

                result.Add(spaceSimulationResult);
            }

            return result;

        }
    }
}