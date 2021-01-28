using System.Collections.Generic;

namespace SAM.Core.OpenStudio
{
    public static partial class Convert
    {
        public static List<string> ToSystem(this global::OpenStudio.StringVector stringVector)
        {
            if (stringVector == null)
                return null;

            List<string> result = new List<string>();
            foreach (string value in stringVector)
                result.Add(value);

            return result;
        }
    }
}