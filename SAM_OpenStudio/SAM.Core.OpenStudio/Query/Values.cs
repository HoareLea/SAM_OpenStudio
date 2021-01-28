using System.Collections.Generic;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static List<double> Values(this global::OpenStudio.SqlFile sqlFile, string enviromentPeriodName, string timeSeriesName, ReportingFrequency reportingFrequency = ReportingFrequency.Hourly)
        {
            if (sqlFile == null || enviromentPeriodName == null || timeSeriesName == null || reportingFrequency == ReportingFrequency.Undefined)
                return null;

            global::OpenStudio.TimeSeriesVector timeSeriesVector = sqlFile.timeSeries(enviromentPeriodName, Core.Query.Description( reportingFrequency), timeSeriesName);

            List<double> result = new List<double>();
            foreach (global::OpenStudio.TimeSeries timeSeries in timeSeriesVector)
            {
                List<double> values = timeSeries.values()?.ToSystem();
                if (values != null)
                    result.AddRange(values);
            }

            return result;
        }
    }
}