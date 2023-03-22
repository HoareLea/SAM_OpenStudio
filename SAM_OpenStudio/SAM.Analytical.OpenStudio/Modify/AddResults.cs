using System.Collections.Generic;
using System.Data.SQLite;

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

            List<SpaceSimulationResult> spaceSimulationResults = null;
            List<SurfaceSimulationResult> surfaceSimulationResults = null;
            using (SQLiteConnection sQLiteConnection = Core.SQLite.Create.SQLiteConnection(path))
            {
                spaceSimulationResults = Create.SpaceSimulationResults(sQLiteConnection);
                surfaceSimulationResults = Create.SurfaceSimulationResults(sQLiteConnection, spaceSimulationResults);
            }

            List<Core.Result> result = new List<Core.Result>();

            List<Space> spaces = adjacencyCluster.GetSpaces();
            Dictionary<string, Space> dictionary_Space = null;

            if (spaces != null)
            {
                dictionary_Space = new Dictionary<string, Space>();
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

                    dictionary_Space[reference] = space;
                }
            }

            if (spaceSimulationResults != null && dictionary_Space != null)
            {
                foreach (SpaceSimulationResult spaceSimulationResult in spaceSimulationResults)
                {
                    string reference = spaceSimulationResult.Name;
                    Space space = null;
                    if (reference != null && dictionary_Space != null && dictionary_Space.Count != 0)
                    {
                        if (!dictionary_Space.TryGetValue(reference, out space))
                        {
                            space = null;
                        }
                    }

                    adjacencyCluster.AddObject(spaceSimulationResult);
                    if (space != null)
                    {
                        adjacencyCluster.AddRelation(space, spaceSimulationResult);
                    }

                    result.Add(spaceSimulationResult);
                }
            }

            List<Panel> panels = adjacencyCluster.GetPanels();
            Dictionary<string, Panel> dictionary_Panel = null;

            if(panels != null)
            {
                dictionary_Panel = new Dictionary<string, Panel>();
                foreach (Panel panel in panels)
                {
                    if (panel == null)
                    {
                        continue;
                    }

                    string reference = panel.Guid.ToString("N").ToUpper();
                    if (string.IsNullOrWhiteSpace(reference))
                    {
                        continue;
                    }

                    dictionary_Panel[reference] = panel;
                }
            }

            if (surfaceSimulationResults != null && dictionary_Panel != null)
            {
                foreach (SurfaceSimulationResult surfaceSimulationResult in surfaceSimulationResults)
                {
                    string reference = surfaceSimulationResult.Name;
                    string[] values = reference.Split(new string[] { "__" }, System.StringSplitOptions.None);
                    if(values.Length > 1)
                    {
                        reference = values[1];
                    }

                    Panel panel = null;
                    if (reference != null && dictionary_Panel != null && dictionary_Panel.Count != 0)
                    {
                        if (!dictionary_Panel.TryGetValue(reference, out panel))
                        {
                            panel = null;
                        }
                    }

                    adjacencyCluster.AddObject(surfaceSimulationResult);
                    if (panel != null)
                    {
                        adjacencyCluster.AddRelation(panel, surfaceSimulationResult);
                    }

                    result.Add(surfaceSimulationResult);

                    if(surfaceSimulationResult.TryGetValue(SurfaceSimulationResultParameter.ZoneName, out string reference_Space))
                    {
                        if (reference != null && dictionary_Space != null && dictionary_Space.Count != 0)
                        {
                            if (dictionary_Space.TryGetValue(reference_Space, out Space space) && space != null)
                            {
                                adjacencyCluster.AddRelation(space, surfaceSimulationResult);
                            }
                        }
                    }
                }
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
