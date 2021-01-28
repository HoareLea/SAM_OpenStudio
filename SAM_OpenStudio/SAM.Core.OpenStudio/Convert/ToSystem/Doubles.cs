using System.Collections.Generic;

namespace SAM.Core.OpenStudio
{
    public static partial class Convert
    {
        public static List<double> ToSystem(this global::OpenStudio.Vector vector)
        {
            if (vector == null)
                return null;

            List<double> result = new List<double>();

            uint count = vector.__len__();
            for (uint i = 0; i < count; i++)
                result.Add(vector.__getitem__(i));

            return result;
        }
    }
}