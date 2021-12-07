using System.Collections.Generic;
using System.Data;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static List<int> SurfaceIndexes(this DataTable dataTable, int zoneIndex)
        {
            if(dataTable == null || zoneIndex == -1)
            {
                return null;
            }

            DataColumnCollection dataColumnCollection = dataTable.Columns;
            if (dataColumnCollection == null)
            {
                return null;
            }

            int index_SurfaceIndex = dataColumnCollection.IndexOf("SurfaceIndex");
            if (index_SurfaceIndex == -1)
            {
                return null;
            }

            int index_ZoneIndex = dataColumnCollection.IndexOf("ZoneIndex");
            if (index_ZoneIndex == -1)
            {
                return null;
            }

            DataRowCollection dataRowCollection = dataTable.Rows;
            if(dataRowCollection == null)
            {
                return null;
            }

            List<int> result = new List<int>();
            foreach(DataRow dataRow in dataRowCollection)
            {
                if(!Core.Query.TryConvert(dataRow[index_ZoneIndex], out int zoneIndex_Temp))
                {
                    continue;
                }

                if(zoneIndex != zoneIndex_Temp)
                {
                    continue;
                }

                if (!Core.Query.TryConvert(dataRow[index_SurfaceIndex], out int surfaceIndex_Temp))
                {
                    continue;
                }

                result.Add(surfaceIndex_Temp);
            }

            return result;
        }
    }
}