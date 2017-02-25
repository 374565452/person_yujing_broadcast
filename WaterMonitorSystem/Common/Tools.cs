using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace Common
{
    public class Tools
    {
        #region 对象复制，对象转化

        /// <summary>
        /// 对象复制，要复制的实例必须可序列化，包括实例引用的其它实例都必须在类定义时加[Serializable]特性。
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="RealObject">源对象</param>
        /// <returns>新对象</returns>
        public static T Copy<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制     
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回该类型默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败返回类型的默认值 </returns>
        public static T CastTo<T>(object value)
        {
            object result;
            Type type = typeof(T);
            try
            {
                if (type.IsEnum)
                {
                    result = Enum.Parse(type, value.ToString());
                }
                else if (type == typeof(Guid))
                {
                    result = Guid.Parse(value.ToString());
                }
                else
                {
                    result = Convert.ChangeType(value, type);
                }
            }
            catch
            {
                result = default(T);
            }

            return (T)result;
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回该类型默认值
        /// </summary>
        /// <param name="value">要转化的源对象</param>
        /// <param name="t"> 指定类型</param>
        /// <returns></returns>
        public static object CastTo(object value, Type t)
        {
            object result;
            try
            {
                if (t.IsEnum)
                {
                    result = Enum.Parse(t, value.ToString());
                }
                else if (t == typeof(Guid))
                {
                    result = Guid.Parse(value.ToString());
                }
                else
                {
                    result = Convert.ChangeType(value, t);
                }
            }
            catch
            {
                result = t.IsValueType ? Activator.CreateInstance(t) : null;
            }

            return result;
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回指定的默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="defaultValue"> 转化失败返回的指定默认值 </param>
        /// <returns> 转化后的指定类型对象，转化失败时返回指定的默认值 </returns>
        public static T CastTo<T>(object value, T defaultValue)
        {
            object result;
            Type type = typeof(T);
            try
            {
                result = type.IsEnum ? Enum.Parse(type, value.ToString()) : Convert.ChangeType(value, type);
            }
            catch
            {
                result = defaultValue;
            }
            return (T)result;
        }

        #endregion

        public static string RepeatStr(string str, int num)
        {
            string returnStr = "";
            for (int i = 0; i < num; i++)
            {
                returnStr += str;
            }
            return returnStr;
        }

        public static int StringToInt(string v, int defaultV)
        {
            int i = defaultV;
            try
            {
                i = int.Parse(v);
            }
            catch
            {
            }
            return i;
        }

        public static double StringToDouble(string v, double defaultV)
        {
            double d = defaultV;
            try
            {
                d = double.Parse(v);
            }
            catch
            {
            }
            return d;
        }

        public static double StringToDoubleMultiply10(string v, double defaultV)
        {
            return StringToDouble(v, defaultV) * 10;
        }

        public static bool CheckValue(string Value, string type, int minLength, int maxLength, object minValue, object maxValue)
        {
            if (maxLength > 0)
            {
                if (maxLength < minLength) return false;
                if (Value.Length < minLength) return false;
                if (Value.Length > maxLength) return false;
            }

            if (minValue != null && maxValue != null)
            {
                try
                {
                    if (type == "short" && (Convert.ToInt16(Value) < Convert.ToInt16(minValue) || Convert.ToInt16(Value) > Convert.ToInt16(maxValue)))
                    {
                        return false;
                    }
                    if (type == "int" && (Convert.ToInt32(Value) < Convert.ToInt32(minValue) || Convert.ToInt32(Value) > Convert.ToInt32(maxValue)))
                    {
                        return false;
                    }
                    if (type == "long" && (Convert.ToInt64(Value) < Convert.ToInt64(minValue) || Convert.ToInt64(Value) > Convert.ToInt64(maxValue)))
                    {
                        return false;
                    }
                    if (type == "float" && (Convert.ToSingle(Value) < Convert.ToSingle(minValue) || Convert.ToSingle(Value) > Convert.ToSingle(maxValue)))
                    {
                        return false;
                    }
                    if (type == "double" && (Convert.ToDouble(Value) < Convert.ToDouble(minValue) || Convert.ToDouble(Value) > Convert.ToDouble(maxValue)))
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static string GetTest(string s)
        {
            if (Regex.Match(s, "^\\d+$").Success)
            {
                return "数字";
            }
            else if (Regex.Match(s, "^[a-zA-Z]+$").Success)
            {
                return "字符";
            }
            else
            {
                return "其他结果";
            }
        }
    }
}
