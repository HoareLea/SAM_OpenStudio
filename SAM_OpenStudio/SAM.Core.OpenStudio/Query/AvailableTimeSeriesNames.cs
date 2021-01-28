using System.Collections.Generic;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static List<string> AvailableTimeSeriesNames(this global::OpenStudio.SqlFile sqlFile)
        {
            return Convert.ToSystem(sqlFile?.availableTimeSeries());
        }
    }
}