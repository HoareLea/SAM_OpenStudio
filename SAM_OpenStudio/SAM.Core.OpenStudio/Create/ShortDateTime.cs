namespace SAM.Core.OpenStudio
{
    public static partial class Create
    {
        public static ShortDateTime ShortDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string[] values = value.Trim().Split(' ');
            if(values == null || values.Length < 1)
            {
                return null;
            }

            string[] values_Temp;

            values_Temp = values[0].Split('/');
            if(values_Temp == null || values_Temp.Length < 2)
            {
                return null;
            }

            if(!byte.TryParse(values_Temp[0], out byte month))
            {
                return null;
            }

            if (!byte.TryParse(values_Temp[1], out byte day))
            {
                return null;
            }

            byte hour = 0;
            byte minute = 0;
            byte second = 0;
            
            if(values.Length > 1)
            {
                values_Temp = values[1].Split(':');
                if (values_Temp != null)
                {
                    if(values_Temp.Length > 0)
                    {
                        if(!byte.TryParse(values_Temp[0], out hour))
                        {
                            hour = 0;
                        }

                        if (values_Temp.Length > 1)
                        {
                            if (!byte.TryParse(values_Temp[1], out minute))
                            {
                                minute = 0;
                            }

                            if (values_Temp.Length > 2)
                            {
                                if (!byte.TryParse(values_Temp[2], out second))
                                {
                                    second = 0;
                                }
                            }
                        }
                    }
   
                }
            }

            return new ShortDateTime(month, day, hour, minute, second);
        }
    }
}