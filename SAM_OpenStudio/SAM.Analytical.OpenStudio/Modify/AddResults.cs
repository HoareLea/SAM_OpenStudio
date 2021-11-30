using System.Collections.Generic;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Modify
    {
        public static List<Core.Result> AddResults(this AdjacencyCluster adjacencyCluster, string path)
        {
            if(string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path) || adjacencyCluster == null)
            {
                return null;
            }

            List<SpaceSimulationResult> spaceSimulationResults = Create.SpaceSimulationResults(path);
            if(spaceSimulationResults == null)
            {
                return null;
            }

            Dictionary<string, Space> dictionary = null;

            List<Space> spaces = adjacencyCluster.GetSpaces();
            if(spaces != null)
            {
                dictionary = new Dictionary<string, Space>();
                foreach(Space space in spaces)
                {
                    if(space == null)
                    {
                        continue;
                    }

                    string reference = space.Guid.ToString("N").ToUpper();
                    if(string.IsNullOrWhiteSpace(reference))
                    {
                        continue;
                    }

                    dictionary[reference] = space;
                }
            }

            List<Core.Result> result = new List<Core.Result>();
            foreach(SpaceSimulationResult spaceSimulationResult in spaceSimulationResults)
            {
                string reference = spaceSimulationResult.Name;
                Space space = null;
                if(reference != null && dictionary != null && dictionary.Count != 0)
                {
                    if(!dictionary.TryGetValue(reference, out space))
                    {
                        space = null;
                    }
                }

                adjacencyCluster.AddObject(spaceSimulationResult);
                if(space != null)
                {
                    adjacencyCluster.AddRelation(space, spaceSimulationResult);
                }

                result.Add(spaceSimulationResult);
            }

            return result;
        }

        public static List<Core.Result> AddResults(this BuildingModel buildingModel, string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path) || buildingModel == null)
            {
                return null;
            }

            List<SpaceSimulationResult> spaceSimulationResults = Create.SpaceSimulationResults(path);
            if (spaceSimulationResults == null)
            {
                return null;
            }

            Dictionary<string, Space> dictionary = null;

            List<Space> spaces = buildingModel.GetSpaces();
            if (spaces != null)
            {
                dictionary = new Dictionary<string, Space>();
                foreach (Space space in spaces)
                {
                    if (space == null)
                    {
                        continue;
                    }

                    string reference = space.Guid.ToString("N").ToUpper();
                    if (string.IsNullOrWhiteSpace(reference))
                    {
                        continue;
                    }

                    dictionary[reference] = space;
                }
            }

            List<Core.Result> result = new List<Core.Result>();
            foreach (SpaceSimulationResult spaceSimulationResult in spaceSimulationResults)
            {
                string reference = spaceSimulationResult.Name;
                Space space = null;
                if (reference != null && dictionary != null && dictionary.Count != 0)
                {
                    if (!dictionary.TryGetValue(reference, out space))
                    {
                        space = null;
                    }
                }

                buildingModel.Add(spaceSimulationResult, space);

                result.Add(spaceSimulationResult);
            }

            return result;
        }
    }
}
