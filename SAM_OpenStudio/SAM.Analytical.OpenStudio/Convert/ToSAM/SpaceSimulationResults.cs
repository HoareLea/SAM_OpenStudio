using System.Collections.Generic;
using System.Data;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Convert
    {
        public static List<SpaceSimulationResult> ToSAM_SpaceSimulationResult(this DataTable dataTable)
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

            int index_ZoneIndex = dataColumnCollection.IndexOf("ZoneIndex");
            if(index_ZoneIndex == -1)
            {
                return null;
            }

            int index_ZoneName = dataColumnCollection.IndexOf("ZoneName");
            if (index_ZoneName == -1)
            {
                return null;
            }

            int index_FloorArea = dataColumnCollection.IndexOf("FloorArea");
            int index_Volume = dataColumnCollection.IndexOf("Volume");

            string source = Query.Source();

            List<SpaceSimulationResult> result = new List<SpaceSimulationResult>();
            foreach (DataRow dataRow in dataRowCollection)
            {
                if(dataRow == null)
                {
                    continue;
                }

                object[] values = dataRow.ItemArray;

                if(!Core.Query.TryConvert(values[index_ZoneIndex], out int zoneIndex))
                {
                    continue;
                }

                if (!Core.Query.TryConvert(values[index_ZoneName], out string zoneName))
                {
                    continue;
                }

                SpaceSimulationResult spaceSimulationResult = new SpaceSimulationResult(zoneName, source, zoneIndex.ToString());
                
                if(index_FloorArea != -1 && Core.Query.TryConvert(values[index_FloorArea], out double floorArea))
                {
                    spaceSimulationResult.SetValue(SpaceSimulationResultParameter.Area, floorArea);
                }

                if (index_Volume != -1 && Core.Query.TryConvert(values[index_Volume], out double volume))
                {
                    spaceSimulationResult.SetValue(SpaceSimulationResultParameter.Volume, volume);
                }

                result.Add(spaceSimulationResult);
            }

            return result;

        }
    }
}