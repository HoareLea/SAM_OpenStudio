namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static global::OpenStudio.SqlFile SqlFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
                return null;

            global::OpenStudio.Path openStudioPath = global::OpenStudio.OpenStudioUtilitiesCore.toPath(path);
            if (openStudioPath == null)
                return null;

            global::OpenStudio.SqlFile sqlFile = new global::OpenStudio.SqlFile(openStudioPath);
            return sqlFile;
        }
    }
}