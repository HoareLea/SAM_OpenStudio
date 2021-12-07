using System.Collections.Generic;
using System.Data;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static List<string> SurfaceNames(this DataTable dataTable, int zoneIndex)
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

            int index_SurfaceName = dataColumnCollection.IndexOf("SurfaceName");
            if (index_SurfaceName == -1)
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

            List<string> result = new List<string>();
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

                if (!Core.Query.TryConvert(dataRow[index_SurfaceName], out string surfaceName_Temp))
                {
                    continue;
                }

                result.Add(surfaceName_Temp);
            }

            return result;
        }
    }
}