namespace SAM.Core.OpenStudio
{
    public static partial class Create
    {
        /// <summary>
        /// Reads model from give path (*.osm)
        /// </summary>
        /// <param name="path">OSM file path example: C:\MyModels\model.osm</param>
        /// <returns>Model</returns>
        public static global::OpenStudio.Model Model(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
                return null;

            global::OpenStudio.Path openStudioPath = global::OpenStudio.OpenStudioUtilitiesCore.toPath(path);
            if (openStudioPath == null)
                return null;

            global::OpenStudio.OptionalModel optionalModel = global::OpenStudio.Model.load(openStudioPath);
            if(optionalModel == null || optionalModel.isNull())
            {
                return null;
            }

            return optionalModel.get();
        }
    }
}