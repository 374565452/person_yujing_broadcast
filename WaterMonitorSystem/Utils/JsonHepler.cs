using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Utils
{
    public class JsonHepler
    {
        private static string ConvertDateStringToJsonDate(Match m)
        {
            TimeSpan span = (TimeSpan)(DateTime.Parse(m.Groups[0].Value).ToUniversalTime() - DateTime.Parse("1970-01-01"));
            return string.Format(@"\/Date({0}+0800)\/", span.TotalMilliseconds);
        }

        private static string ConvertJsonDateToDateString(Match m)
        {
            DateTime time = new DateTime(0x7b2, 1, 1);
            return time.AddMilliseconds((double)long.Parse(m.Groups[1].Value)).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static T[] JsonDeserializeByArrayData<T>(string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T[]));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return (T[])serializer.ReadObject(stream);
        }

        public static T JsonDeserializeBySingleData<T>(string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return (T)serializer.ReadObject(stream);
        }

        public static string JsonSerializerByArrayData<T>(T[] tArray)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T[]));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, tArray);
            string str = Encoding.UTF8.GetString(stream.ToArray());
            stream.Close();
            return str;
        }

        public static string JsonSerializerBySingleData<T>(T t)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, t);
            string str = Encoding.UTF8.GetString(stream.ToArray());
            stream.Close();
            return str;
        }

        public static string ListToJson<T>(IList<T> list)
        {
            object obj2 = list[0];
            return ListToJson<T>(list, obj2.GetType().Name);
        }

        public static string ListToJson<T>(IList<T> list, string jsonName)
        {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
            {
                T local2 = list[0];
                jsonName = local2.GetType().Name;
            }
            builder.Append("{\"" + jsonName + "\":[");
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    PropertyInfo[] properties = Activator.CreateInstance<T>().GetType().GetProperties();
                    builder.Append("{");
                    for (int j = 0; j < properties.Length; j++)
                    {
                        Type type;
                        object obj2 = properties[j].GetValue(list[i], null);
                        string str = string.Empty;
                        if (obj2 != null)
                        {
                            type = obj2.GetType();
                            str = obj2.ToString();
                        }
                        else
                        {
                            type = typeof(string);
                        }
                        builder.Append("\"" + properties[j].Name.ToString() + "\":" + StringFormat(str, type));
                        if (j < (properties.Length - 1))
                        {
                            builder.Append(",");
                        }
                    }
                    builder.Append("}");
                    if (i < (list.Count - 1))
                    {
                        builder.Append(",");
                    }
                }
            }
            builder.Append("]}");
            return builder.ToString();
        }

        public static string String2Json(string s)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s.ToCharArray()[i];
                switch (ch)
                {
                    case '"':
                        builder.Append("\\\"");
                        break;

                    case '/':
                        builder.Append(@"\/");
                        break;

                    case '\\':
                        builder.Append(@"\\");
                        break;

                    case '\b':
                        builder.Append(@"\b");
                        break;

                    case '\t':
                        builder.Append(@"\t");
                        break;

                    case '\n':
                        builder.Append(@"\n");
                        break;

                    case '\v':
                        builder.Append(@"\v");
                        break;

                    case '\f':
                        builder.Append(@"\f");
                        break;

                    case '\r':
                        builder.Append(@"\r");
                        break;

                    case '\0':
                        builder.Append(@"\0");
                        break;

                    default:
                        builder.Append(ch);
                        break;
                }
            }
            return builder.ToString();
        }

        private static string StringFormat(string str, Type type)
        {
            if ((type != typeof(string)) && string.IsNullOrEmpty(str))
            {
                str = "\"" + str + "\"";
                return str;
            }
            if (type == typeof(string))
            {
                str = String2Json(str);
                str = "\"" + str + "\"";
                return str;
            }
            if (type == typeof(DateTime))
            {
                str = "\"" + str + "\"";
                return str;
            }
            if (type == typeof(bool))
            {
                str = str.ToLower();
                return str;
            }
            if (type == typeof(byte[]))
            {
                str = "\"" + str + "\"";
                return str;
            }
            if (type == typeof(Guid))
            {
                str = "\"" + str + "\"";
            }
            return str;
        }

        public static string ToArrayString(IEnumerable array)
        {
            string str = "[";
            foreach (object obj2 in array)
            {
                str = ToJson((IEnumerable)obj2.ToString()) + ",";
            }
            str.Remove(str.Length - 1, str.Length);
            return (str + "]");
        }

        public static string ToJson(IEnumerable array)
        {
            string str = "[";
            foreach (object obj2 in array)
            {
                str = str + ToJson(obj2) + ",";
            }
            if (str.Length > 1)
            {
                str.Remove(str.Length - 1, str.Length);
            }
            else
            {
                str = "[]";
            }
            return (str + "]");
        }

        public static string ToJson(DataSet dataSet)
        {
            string str = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                string str3 = str;
                str = str3 + "\"" + table.TableName + "\":" + ToJson(table) + ",";
            }
            return (str.TrimEnd(new char[] { ',' }) + "}");
        }

        public static string ToJson(DataTable dt)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            DataRowCollection rows = dt.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                builder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string columnName = dt.Columns[j].ColumnName;
                    string str = rows[i][j].ToString();
                    Type dataType = dt.Columns[j].DataType;
                    builder.Append("\"" + columnName + "\":");
                    str = StringFormat(str, dataType);
                    if (j < (dt.Columns.Count - 1))
                    {
                        builder.Append(str + ",");
                    }
                    else
                    {
                        builder.Append(str);
                    }
                }
                builder.Append("},");
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append("]");
            if (builder.Length == 1)
            {
                return "[]";
            }
            return builder.ToString();
        }

        public static string ToJson(IDataReader dataReader)
        {
            string str3;
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("[");
                while (dataReader.Read())
                {
                    builder.Append("{");
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        Type fieldType = dataReader.GetFieldType(i);
                        string name = dataReader.GetName(i);
                        string str = dataReader[i].ToString();
                        builder.Append("\"" + name + "\":");
                        str = StringFormat(str, fieldType);
                        if (i < (dataReader.FieldCount - 1))
                        {
                            builder.Append(str + ",");
                        }
                        else
                        {
                            builder.Append(str);
                        }
                    }
                    builder.Append("},");
                }
                if (!dataReader.IsClosed)
                {
                    dataReader.Close();
                }
                builder.Remove(builder.Length - 1, 1);
                builder.Append("]");
                if (builder.Length == 1)
                {
                    return "[]";
                }
                str3 = builder.ToString();
            }
            catch
            {
                throw;
            }
            return str3;
        }

        public static string ToJson(object jsonObject)
        {
            string str;
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("{");
                PropertyInfo[] properties = jsonObject.GetType().GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    object obj2 = properties[i].GetGetMethod().Invoke(jsonObject, null);
                    if (obj2 != null)
                    {
                        StringBuilder builder2 = new StringBuilder();
                        if (((obj2 is DateTime) || (obj2 is Guid)) || (obj2 is TimeSpan))
                        {
                            builder2.Append("\"" + obj2.ToString() + "\"");
                        }
                        else if (obj2 is string)
                        {
                            builder2.Append("\"" + obj2.ToString() + "\"");
                        }
                        else if (obj2 is IEnumerable)
                        {
                            builder2.Append(ToJson((IEnumerable)obj2));
                        }
                        else
                        {
                            builder2.Append("\"" + obj2.ToString() + "\"");
                        }
                        builder.Append(string.Concat(new object[] { "\"", properties[i].Name, "\":", builder2, "," }));
                    }
                }
                str = builder.ToString().TrimEnd(new char[] { ',' }) + "}";
            }
            catch
            {
                throw;
            }
            return str;
        }

        public static string ToJson(DataTable dt, string jsonName)
        {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
            {
                jsonName = dt.TableName;
            }
            builder.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    builder.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        builder.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + StringFormat((dt.Rows[i][j] is DBNull) ? string.Empty : dt.Rows[i][j].ToString(), type));
                        if (j < (dt.Columns.Count - 1))
                        {
                            builder.Append(",");
                        }
                    }
                    builder.Append("}");
                    if (i < (dt.Rows.Count - 1))
                    {
                        builder.Append(",");
                    }
                }
            }
            builder.Append("]}");
            return builder.ToString();
        }
    }
}
