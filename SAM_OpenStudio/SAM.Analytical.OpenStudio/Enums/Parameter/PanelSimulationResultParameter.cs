using System.ComponentModel;
using SAM.Core.Attributes;

namespace SAM.Analytical.OpenStudio
{
    [AssociatedTypes(typeof(PanelSimulationResult)), Description("PanelSimulationResult Parameter")]
    public enum PanelSimulationResultParameter
    {
        [ParameterProperties("Zone Index", "Zone Index"), ParameterValue(Core.ParameterType.Integer)] ZoneIndex,
        [ParameterProperties("Zone Name", "Zone Name"), ParameterValue(Core.ParameterType.String)] ZoneName,
    }
}