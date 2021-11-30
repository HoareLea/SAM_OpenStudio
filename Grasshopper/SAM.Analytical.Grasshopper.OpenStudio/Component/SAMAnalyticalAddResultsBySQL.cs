﻿using Grasshopper.Kernel;
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
        public override string LatestComponentVersion => "1.0.1";

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
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooAnalyticalObjectParam(), "analytical", "analytical", "SAM Analytical Object such as AdjacencyCluster or AnalyticalModel", GH_ParamAccess.item);
            outputParamManager.AddParameter(new GooResultParam(), "spaceSimulationResults", "spaceSimulationResults", "SAM Analytical SpaceSimulationResults", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            IAnalyticalObject analyticalObject = null;
            if (!dataAccess.GetData(0, ref analyticalObject) || analyticalObject == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string path = null;
            if (!dataAccess.GetData(1, ref path) || path == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            List<Core.Result> results = null;

            if(analyticalObject is AdjacencyCluster)
            {
                AdjacencyCluster adjacencyCluster = new AdjacencyCluster((AdjacencyCluster)analyticalObject);
                results = Analytical.OpenStudio.Modify.AddResults(adjacencyCluster, path);
                analyticalObject = adjacencyCluster;
            }
            else if(analyticalObject is AnalyticalModel)
            {
                AdjacencyCluster adjacencyCluster = ((AnalyticalModel)analyticalObject).AdjacencyCluster;
                results = Analytical.OpenStudio.Modify.AddResults(adjacencyCluster, path);
                analyticalObject = new AnalyticalModel((AnalyticalModel)analyticalObject, adjacencyCluster);

            }
            else if(analyticalObject is BuildingModel)
            {
                BuildingModel buildingModel = new BuildingModel((BuildingModel)analyticalObject);
                results = Analytical.OpenStudio.Modify.AddResults(buildingModel, path);
                analyticalObject = buildingModel;
            }

            dataAccess.SetData(0, analyticalObject);
            dataAccess.SetDataList(1, results?.ConvertAll(x => new GooResult(x)));
        }
    }
}