using Grasshopper.Kernel;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;

namespace SAM.Analytical.Grasshopper.OpenStudio
{
    public class SAMAnalyticalAddResultsBySQL : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("c6c53281-ccab-4872-8278-ee8c638be035");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.3";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SAM_Small;

        /// <summary>
        /// Initializes a new instance of the SAMGeometryByGHGeometry class.
        /// </summary>
        public SAMAnalyticalAddResultsBySQL()
          : base("SAMAnalytical.AddResultsBySQL", "SAMAnalytical.AddResultsBySQL",
              "Adds Results From OpenStudio Sql Database",
              "SAM", "OpenStudio")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddParameter(new GooAnalyticalObjectParam(), "_analytical", "_analytical", "SAM Analytical Object such as AdjacencyCluster or AnalyticalModel", GH_ParamAccess.item);
            inputParamManager.AddTextParameter("_sQLPath", "_sQLPath", "SQL File Path", GH_ParamAccess.item);
            inputParamManager.AddBooleanParameter("_run", "_run", "Run", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooAnalyticalObjectParam(), "analytical", "analytical", "SAM Analytical Object such as AdjacencyCluster or AnalyticalModel", GH_ParamAccess.item);
            outputParamManager.AddParameter(new GooResultParam(), "spaceSimulationResults", "spaceSimulationResults", "SAM Analytical SpaceSimulationResults", GH_ParamAccess.list);
            outputParamManager.AddParameter(new GooResultParam(), "panelSimulationResults", "panelSimulationResults", "SAM Analytical PanelSimulationResults", GH_ParamAccess.list);
            outputParamManager.AddBooleanParameter("Successful", "Successful", "Correctly saved?", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            dataAccess.SetData(3, false);

            bool run = false;
            if (!dataAccess.GetData(2, ref run) || !run)
                return;

            IAnalyticalObject analyticalObject = null;
            if (!dataAccess.GetData(0, ref analyticalObject) || analyticalObject == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string path = null;
            dataAccess.GetData(1, ref path);

            List<Core.Result> results = null;

            if(!string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path))
            {
                if (analyticalObject is AdjacencyCluster)
                {
                    AdjacencyCluster adjacencyCluster = new AdjacencyCluster((AdjacencyCluster)analyticalObject);
                    results = Analytical.OpenStudio.Modify.AddResults(adjacencyCluster, path);
                    analyticalObject = adjacencyCluster;
                }
                else if (analyticalObject is AnalyticalModel)
                {
                    AdjacencyCluster adjacencyCluster = ((AnalyticalModel)analyticalObject).AdjacencyCluster;
                    results = Analytical.OpenStudio.Modify.AddResults(adjacencyCluster, path);
                    analyticalObject = new AnalyticalModel((AnalyticalModel)analyticalObject, adjacencyCluster);

                }
                else if (analyticalObject is BuildingModel)
                {
                    BuildingModel buildingModel = new BuildingModel((BuildingModel)analyticalObject);
                    results = Analytical.OpenStudio.Modify.AddResults(buildingModel, path);
                    analyticalObject = buildingModel;
                }
            }

            dataAccess.SetData(0, analyticalObject);
            dataAccess.SetDataList(1, results?.FindAll(x => x is SpaceSimulationResult).ConvertAll(x => new GooResult(x)));
            dataAccess.SetDataList(2, results?.FindAll(x => x is PanelSimulationResult).ConvertAll(x => new GooResult(x)));
            dataAccess.SetData(3, results != null && results.Count != 0);
        }
    }
}