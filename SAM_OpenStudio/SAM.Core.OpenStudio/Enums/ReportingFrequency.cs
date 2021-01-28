using System.ComponentModel;

namespace SAM.Core.OpenStudio
{
    [Description("Reporting Frequency")]
    public enum ReportingFrequency
    {
        [Description("Undefined")] Undefined,
        [Description("HVAC System Timestep")] Detailed,
        [Description("Zone Timestep")] Timestep,
        [Description("Hourly")] Hourly,
        [Description("Daily")] Daily,
        [Description("Monthly")] Monthly,
        [Description("RunPeriod")] RunPeriod
    }
}