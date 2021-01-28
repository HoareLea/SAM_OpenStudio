using System.Collections.Generic;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static List<string> AvailableEnvironmentPeriodsNames(this global::OpenStudio.SqlFile sqlFile)
        {
            return Convert.ToSystem(sqlFile?.availableEnvPeriods());
        }
    }
}