using System.ComponentModel;
using SAM.Core.Attributes;

namespace SAM.Analytical.OpenStudio
{
    [AssociatedTypes(typeof(SpaceSimulationResult)), Description("SpaceSimulationResult Parameter")]
    public enum SpaceSimulationResultParameter
    {
        [ParameterProperties("Design Day Name", "Design Day Name"), ParameterValue(Core.ParameterType.String)] DesignDayName,
        [ParameterProperties("Design Day Index", "Design Day Index"), ParameterValue(Core.ParameterType.Integer)] DesignDayIndex,
        [ParameterProperties("Peak Date", "Peak Date"), SAMObjectParameterValue(typeof(Core.OpenStudio.ShortDateTime))] PeakDate,
    }
}