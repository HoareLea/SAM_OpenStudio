using System.Collections.Generic;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static void Max(this SortedDictionary<int , double> sortedDictionary, out int @int, out double @double)
        {
            @int = -1;
            @double = double.NaN;

            if(sortedDictionary == null || sortedDictionary.Count == 0)
            {
                return;
            }

            @double = double.MinValue;
            foreach(KeyValuePair<int, double> keyValuePair in sortedDictionary)
            {
                if(keyValuePair.Value <= @double)
                {
                    continue;
                }

                @double = keyValuePair.Value;
                @int = keyValuePair.Key;
            }
        }
    }
}