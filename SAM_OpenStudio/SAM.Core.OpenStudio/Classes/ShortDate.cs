using Newtonsoft.Json.Linq;
using System;

namespace SAM.Core.OpenStudio
{
    public class ShortDateTime : IJSAMObject
    {
        private byte month;
        private byte day;
        private byte hour;
        private byte minute;
        private byte second;

        public ShortDateTime(byte month, byte day, byte hour, byte minute, byte second)
        {
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }
        
        public ShortDateTime(JObject jObject)
        {
            FromJObject(jObject);
        }

        public ShortDateTime(ShortDateTime shortDateTime)
        {
            if(shortDateTime != null)
            {
                month = shortDateTime.month;
                day = shortDateTime.day;
                hour = shortDateTime.hour;
                minute = shortDateTime.minute;
                second = shortDateTime.second;
            }
        }

        public byte Month
        {
            get
            {
                return month;
            }
        }

        public byte Day
        {
            get
            {
                return day;
            }
        }

        public byte Hour
        {
            get
            {
                return hour;
            }
        }

        public byte Minute
        {
            get
            {
                return minute;
            }
        }

        public byte Second
        {
            get
            {
                return second;
            }
        }

        public override string ToString()
        {
            string hourString = hour.ToString();
            if (hourString.Length < 2)
                hourString = "0" + hourString;

            string minuteString = minute.ToString();
            if (minuteString.Length < 2)
                minuteString = "0" + minuteString;

            string secondString = second.ToString();
            if (secondString.Length < 2)
                secondString = "0" + secondString;

            return string.Format("{0}/{1} {2}:{3}:{4}", month, day, hourString, minuteString, secondString);
        }

        public bool FromJObject(JObject jObject)
        {
            if(jObject == null)
            {
                return false;
            }

            if(!jObject.ContainsKey("Month"))
            {
                return false;
            }

            month = System.Convert.ToByte(jObject.Value<int>("Month"));

            if (!jObject.ContainsKey("Day"))
            {
                return false;
            }

            day = System.Convert.ToByte(jObject.Value<int>("Day"));

            if (!jObject.ContainsKey("Hour"))
            {
                return false;
            }

            hour = System.Convert.ToByte(jObject.Value<int>("Hour"));

            if (!jObject.ContainsKey("Minute"))
            {
                return false;
            }

            minute = System.Convert.ToByte(jObject.Value<int>("Minute"));

            if (!jObject.ContainsKey("Second"))
            {
                return false;
            }

            second = System.Convert.ToByte(jObject.Value<int>("Second"));
            return true;
        }

        public JObject ToJObject()
        {
            JObject result = new JObject();
            result.Add("_type", SAM.Core.Query.FullTypeName(this));

            result.Add("Month", System.Convert.ToInt32(month));
            result.Add("Day", System.Convert.ToInt32(day));
            result.Add("Hour", System.Convert.ToInt32(hour));
            result.Add("Minute", System.Convert.ToInt32(minute));
            result.Add("Second", System.Convert.ToInt32(second));

            return result;
        }
    }
}
