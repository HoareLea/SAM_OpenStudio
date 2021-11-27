using Grasshopper.Kernel;
using SAM.Core.Grasshopper;
using System;

namespace SAM.Analytical.Grasshopper.OpenStudio
{
    public class SAMAnalyticalSpaceSimulationResultsBySQL : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("18c47b95-93cf-4737-829c-11e6bc95c9d8");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.0";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SAM_Small;

        /// <summary>
        /// Initializes a new instance of the SAMGeometryByGHGeometry class.
        /// </summary>
        public SAMAnalyticalSpaceSimulationResultsBySQL()
          : base("SAMAnalytical.SpaceSimulationResultsBySQL", "SAMAnalytical.SpaceSimulationResultsBySQL",
              "Converts OpenStudio Sql Database to SpaceSymulationResults",
              "SAM", "OpenStudio")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddTextParameter("_sQLPath", "_sQLPath", "SQL File Path", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooResultParam(), "spaceSymulationResults", "spaceSymulationResults", "SAM Analytical SpaceSimulationResults", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            string path = null;

            if (!dataAccess.GetData(0, ref path) || path == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            dataAccess.SetDataList(0, Analytical.OpenStudio.Create.SpaceSimulationResults(path)?.ConvertAll(x => new GooResult(x)));
        }
    }
}