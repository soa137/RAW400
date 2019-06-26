using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RawLib
{
    public class JsonParser
    {
        public static ProcessDTO Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentNullException();

            ProcessDTOTmp tmp = new ProcessDTOTmp();

            //------------ парсинг Json ----------
            tmp = JsonConvert.DeserializeObject<ProcessDTOTmp>(json, new IsoDateTimeConverter { DateTimeFormat = "dd.MM.yyyy HH:mm:ss" });
            tmp.Json = json;
            ProcessDTO dto = tmp.CopyPropertisVal();


            //-----------------------------------
            return dto;

        }

        private class ProcessDTOTmp
        {

            double heightSm;
            double diameterOuter;
            double diameterInner;
            double height;
            double equipmentId;
            double weight;
            int operationId;
            int zagotId;
            int passportId;
            int brigadaId;
            int personUntn;

            public int BrigadaId
            {
                get { return brigadaId; }
                set { brigadaId = value; }
            }

            public int PassportId
            {
                get { return passportId; }
                set { passportId = value; }
            }

            public int ZagotId
            {
                get { return zagotId; }
                set { zagotId = value; }
            }

            public int OperationId
            {
                get { return operationId; }
                set { operationId = value; }
            }

            public double Height
            {
                get { return height; }
                set { height = value; }
            }

            public double DiameterInner
            {
                get { return diameterInner; }
                set { diameterInner = value; }
            }

            public double DiameterOuter
            {
                get { return diameterOuter; }
                set { diameterOuter = value; }
            }

            public double HeightSm
            {
                get { return heightSm; }
                set { heightSm = value; }
            }

            public double EquipmentId
            {
                get { return equipmentId; }
                set { equipmentId = value; }
            }

            public double Weight
            {
                get { return weight; }
                set { weight = value; }
            }

            public int PersonUntn
            {
                get { return personUntn; }
                set { personUntn = value; }
            }


            string json;

            public string Json
            {
                get { return json; }
                set { json = value; }
            }


            string operatorName;
            string userName;
            string npart;
            string nplav;
            string pnum;
            string snum;
            string npp;
            string operationName;
            string tek;
            string brigadeName;
            string reportPath;


            public string ReportPath
            {
                get { return reportPath; }
                set { reportPath = value; }
            }

            public string BrigadeName
            {
                get { return brigadeName; }
                set { brigadeName = value; }
            }

            public string Tek
            {
                get { return tek; }
                set { tek = value; }
            }

            public string OperationName
            {
                get { return operationName; }
                set { operationName = value; }
            }

            public string Npp
            {
                get { return npp; }
                set { npp = value; }
            }

            public string Snum
            {
                get { return snum; }
                set { snum = value; }
            }

            public string Pnum
            {
                get { return pnum; }
                set { pnum = value; }
            }

            public string Nplav
            {
                get { return nplav; }
                set { nplav = value; }
            }

            public string Npart
            {
                get { return npart; }
                set { npart = value; }
            }

            public string UserName
            {
                get { return userName; }
                set { userName = value; }
            }

            public string OperatorName
            {
                get { return operatorName; }
                set { operatorName = value; }
            }

            [JsonProperty("DateTimeStart")]
            object dateTimeStart;
            [JsonProperty("DateTimeFinish")]
            object dateTimeFinish;

            [JsonIgnore]
            public DateTime? DateTimeStart
            {

                get
                {

                    if (dateTimeStart != null)
                    {
                        if (IsDateTime(dateTimeStart.ToString()))
                        {
                            return DateTime.ParseExact(dateTimeStart.ToString(), "dd.MM.yyyy HH:mm:ss",
                                           System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            return (DateTime?)null;
                        }
                    }
                    else
                    {
                        return (DateTime?)null;
                    }
                }
                set
                {
                    dateTimeStart = (DateTime?)value;
                }
            }

            [JsonIgnore]
            public DateTime? DateTimeFinish
            {
                get
                {

                    if (dateTimeFinish != null)
                    {

                        if (IsDateTime(dateTimeFinish.ToString()))
                        {
                            return DateTime.ParseExact(dateTimeFinish.ToString(), "dd.MM.yyyy HH:mm:ss",
                                           System.Globalization.CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            return (DateTime?)null;
                        }
                    }
                    else
                    {
                        return (DateTime?)null;
                    }
                }
                set
                {
                    dateTimeFinish = (DateTime?)value;
                }
            }

           

            public static bool IsDateTime(string txtDate)
            {
                DateTime tempDate;
                return DateTime.TryParse(txtDate, out tempDate);
            }

            public ProcessDTO CopyPropertisVal()
            {
                ProcessDTO obj = new ProcessDTO();
                Type t = this.GetType();
                foreach (PropertyInfo property in t.GetProperties())
                {

                    var val = property.GetValue(this, null);
                    PropertyExtension.SetPropertyValue(obj, property.Name, val);
                }
                return obj;
            }
        }

    }
    static class PropertyExtension
    {
        public static void SetPropertyValue(this object p_object, string p_propertyName, object value)
        {
            PropertyInfo property = p_object.GetType().GetProperty(p_propertyName);
            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            object safeValue = (value == null) ? null : Convert.ChangeType(value, t);

            property.SetValue(p_object, safeValue, null);
        }


    }

}