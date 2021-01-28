namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static void Test(string path)
        {
            global::OpenStudio.Path openStudioPath = global::OpenStudio.OpenStudioUtilitiesCore.toPath(path);

            global::OpenStudio.SqlFile sqlFile = new global::OpenStudio.SqlFile(openStudioPath);

            global::OpenStudio.OptionalDouble optionalDouble = sqlFile.electricityPumps();
            double @double = optionalDouble.get();
        }
    }
}