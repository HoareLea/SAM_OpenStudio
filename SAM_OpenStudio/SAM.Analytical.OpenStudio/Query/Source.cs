using System.Reflection;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Query
    {
        public static string Source()
        {
            return Assembly.GetExecutingAssembly().GetName()?.Name;
        }
    }
}