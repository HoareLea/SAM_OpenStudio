using System.Collections.Generic;
using System.Reflection;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Query
    {
        public static List<PanelSimulationResult> PanelSimulationResults(this IEnumerable<PanelSimulationResult> panelSimulationResults, SpaceSimulationResult spaceSimulationResult)
        {
            if(panelSimulationResults == null || spaceSimulationResult == null)
            {
                return null;
            }

            if(!Core.Query.TryConvert(spaceSimulationResult.Reference, out int zoneIndex))
            {
                zoneIndex = -1;
            }

            List<PanelSimulationResult> result = new List<PanelSimulationResult>();
            foreach(PanelSimulationResult panelSimulationResult in panelSimulationResults)
            {
                if(zoneIndex != -1 && panelSimulationResult.TryGetValue(PanelSimulationResultParameter.ZoneIndex, out int zoneIndex_Temp) && zoneIndex_Temp != -1)
                {
                    if (zoneIndex_Temp == zoneIndex)
                    {
                        result.Add(panelSimulationResult);
                        continue;
                    }
                }

                if(panelSimulationResult.TryGetValue(PanelSimulationResultParameter.ZoneName, out string zoneName) && !string.IsNullOrEmpty(zoneName))
                {
                    if(zoneName.Equals(zoneName))
                    {
                        result.Add(panelSimulationResult);
                        continue;
                    }
                }
            }

            return result;
        }
    }
}