using System.Collections.Generic;
using System.Reflection;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Query
    {
        public static List<SurfaceSimulationResult> SurfaceSimulationResults(this IEnumerable<SurfaceSimulationResult> surfaceSimulationResults, SpaceSimulationResult spaceSimulationResult)
        {
            if(surfaceSimulationResults == null || spaceSimulationResult == null)
            {
                return null;
            }

            if(!Core.Query.TryConvert(spaceSimulationResult.Reference, out int zoneIndex))
            {
                zoneIndex = -1;
            }

            List<SurfaceSimulationResult> result = new List<SurfaceSimulationResult>();
            foreach(SurfaceSimulationResult surfaceSimulationResult in surfaceSimulationResults)
            {
                if(zoneIndex != -1 && surfaceSimulationResult.TryGetValue(SurfaceSimulationResultParameter.ZoneIndex, out int zoneIndex_Temp) && zoneIndex_Temp != -1)
                {
                    if (zoneIndex_Temp == zoneIndex)
                    {
                        result.Add(surfaceSimulationResult);
                        continue;
                    }
                }
                else
                {
                    if (surfaceSimulationResult.TryGetValue(SurfaceSimulationResultParameter.ZoneName, out string zoneName) && !string.IsNullOrEmpty(zoneName))
                    {
                        if (zoneName.Equals(spaceSimulationResult.Name))
                        {
                            result.Add(surfaceSimulationResult);
                            continue;
                        }
                    }
                }
            }

            return result;
        }
    }
}