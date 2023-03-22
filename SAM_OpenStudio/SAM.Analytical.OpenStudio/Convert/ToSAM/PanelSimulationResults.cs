﻿using System.Collections.Generic;
using System.Data;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Convert
    {
        public static List<SurfaceSimulationResult> ToSAM_SurfaceSimulationResult(this DataTable dataTable)
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

            List<SurfaceSimulationResult> result = new List<SurfaceSimulationResult>();
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

                SurfaceSimulationResult surfaceSimulationResult = new SurfaceSimulationResult(surfaceName, source, surfaceIndex.ToString());
                
                if(index_Area != -1 && Core.Query.TryConvert(values[index_Area], out double area))
                {
                    surfaceSimulationResult.SetValue(Analytical.SurfaceSimulationResultParameter.Area, area);
                }

                if (index_ZoneIndex != -1 && Core.Query.TryConvert(values[index_ZoneIndex], out int zoneIndex))
                {
                    surfaceSimulationResult.SetValue(SurfaceSimulationResultParameter.ZoneIndex, zoneIndex);
                }

                result.Add(surfaceSimulationResult);
            }

            return result;

        }
    }
}