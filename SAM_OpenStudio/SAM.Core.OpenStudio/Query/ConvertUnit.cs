using System.Data;

namespace SAM.Core.OpenStudio
{
    public static partial class Query
    {
        public static double ConvertUnit(this DataTable dataTable, string name, string keyValue, double value)
        {
            string units = Units(dataTable, name, keyValue);
            if (!string.IsNullOrWhiteSpace(units))
            {
                switch (units.Trim())
                {
                    case "J":
                        value = value / 3600;
                        break;
                }
            }

            return value;
        }
    }
}