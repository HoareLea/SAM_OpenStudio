using System;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Convert
    {

        public static DateTime? ToDateTime(string @string, int year = 1)
        {
            if(string.IsNullOrWhiteSpace(@string))
            {
                return null;
            }

            string[] values = @string.Split(' ');
            if(values == null || values.Length < 2)
            {
                return null;
            }

            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;

            string[] values_Temp = null;

            values_Temp = values[0].Split('/');
            if(values_Temp == null || values_Temp.Length < 2)
            {
                return null;
            }

            if (!int.TryParse(values_Temp[0], out month))
            {
                return null;
            }

            if (!int.TryParse(values_Temp[1], out day))
            {
                return null;
            }

            values_Temp = values[1].Split(':');
            if (values_Temp == null || values_Temp.Length < 3)
            {
                return null;
            }

            if (!int.TryParse(values_Temp[0], out hour))
            {
                return null;
            }

            if (!int.TryParse(values_Temp[1], out minute))
            {
                return null;
            }

            if (!int.TryParse(values_Temp[2], out second))
            {
                return null;
            }

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
