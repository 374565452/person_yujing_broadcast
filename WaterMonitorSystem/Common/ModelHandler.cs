using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace Common
{
    /// <summary>
    /// DataSet、DataTable、DataRow与实体类互相转换
    /// </summary>
    /// <typeparam name="T">实体类</typeparam>
    public class ModelHandler<T> where T : new()
    {
        #region DataTable转换成实体类

        /// <summary>
        /// 填充对象列表：用DataSet的第一个表填充实体类
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns></returns>
        public List<T> FillModel(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return FillModel(ds.Tables[0]);
            }
        }

        /// <summary>  
        /// 填充对象列表：用DataSet的第index个表填充实体类
        /// </summary>  
        public List<T> FillModel(DataSet ds, int index)
        {
            if (ds == null || ds.Tables.Count <= index || ds.Tables[index].Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return FillModel(ds.Tables[index]);
            }
        }

        /// <summary>  
        /// 填充对象列表：用DataTable填充实体类
        /// </summary>  
        public List<T> FillModel(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            List<T> modelList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                modelList.Add(FillModel(dr));
            }
            return modelList;
        }

        /// <summary>  
        /// 填充对象：用DataRow填充实体类
        /// </summary>  
        public T FillModel(DataRow dr)
        {
            if (dr == null)
            {
                return default(T);
            }

            //T model = (T)Activator.CreateInstance(typeof(T));  
            T model = new T();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                string name = propertyInfo.Name;
                if (dr.Table.Columns.Contains(name))
                {
                    object value = dr[name];
                    try
                    {
                        model.GetType().GetProperty(name).SetValue(model, value, null);
                    }
                    catch
                    {
                        string t = propertyInfo.PropertyType.Name;
                        if (t != "DateTime")
                            model.GetType().GetProperty(name).SetValue(model, value.ToString(), null);
                        else
                            model.GetType().GetProperty(name).SetValue(model, DateTime.MinValue, null);
                    }
                }
            }
            return model;
        }

        /// <summary>  
        /// 填充对象：用DataRow填充实体类
        /// </summary>  
        public T FillModel1(DataRow dr)
        {
            if (dr == null)
            {
                return default(T);
            }

            T model = new T();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                string t = propertyInfo.PropertyType.Name;
                string name = propertyInfo.Name;
                if (dr.Table.Columns.Contains(name))
                {
                    object value = dr[name];

                    if (t == "Guid")
                    {
                        model.GetType().GetProperty(name).SetValue(model, Tools.CastTo<Guid>(value, Guid.Empty), null);
                    }
                    else if (t == "Int64")
                    {
                        model.GetType().GetProperty(name).SetValue(model, Tools.CastTo<Int64>(value, 0), null);
                    }
                    else if (t == "Int32")
                    {
                        model.GetType().GetProperty(name).SetValue(model, Tools.CastTo<Int32>(value, 0), null);
                    }
                    else if (t == "Boolean")
                    {
                        model.GetType().GetProperty(name).SetValue(model, Tools.CastTo<Boolean>(value, false), null);
                    }
                    else if (t == "Decimal")
                    {
                        model.GetType().GetProperty(name).SetValue(model, Tools.CastTo<Decimal>(value, 0), null);
                    }
                    else if (t == "DateTime")
                    {
                        model.GetType().GetProperty(name).SetValue(model, Tools.CastTo<DateTime>(value, DateTime.MinValue), null);
                    }
                    else
                    {
                        try
                        {
                            model.GetType().GetProperty(name).SetValue(model, value, null);
                        }
                        catch
                        {
                            model.GetType().GetProperty(name).SetValue(model, value.ToString(), null);
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>  
        /// 填充对象：用DataRow填充实体类
        /// </summary>  
        public T FillModel2(DataRow dr)
        {
            if (dr == null)
            {
                return default(T);
            }

            T model = new T();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                Type t = propertyInfo.PropertyType;
                string name = propertyInfo.Name;
                if (dr.Table.Columns.Contains(name))
                {
                    try
                    {
                        object value = dr[propertyInfo.Name];
                        model.GetType().GetProperty(name).SetValue(model, Tools.CastTo(value, t), null);
                    }
                    catch { }
                }
            }
            return model;
        }

        #endregion

        #region 实体类转换成DataTable

        /// <summary>
        /// 实体类转换成DataSet
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public DataSet FillDataSet(List<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            else
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(FillDataTable(modelList));
                return ds;
            }
        }

        /// <summary>
        /// 实体类转换成DataTable
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public DataTable FillDataTable(List<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            DataTable dt = CreateData(modelList[0]);

            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue(model, null);
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// 根据实体类得到表结构
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        private DataTable CreateData(T model)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
            }
            return dataTable;
        }

        #endregion

        public static string ToString(T model)
        {
            StringBuilder sBuilder = new StringBuilder();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                sBuilder.Append("【" + propertyInfo.Name + ":" + propertyInfo.GetValue(model, null) + "】");
            }
            return sBuilder.ToString();
        }
    }
}
