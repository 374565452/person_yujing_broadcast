using Common;
using DBUtility;
using Maticsoft.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class DeviceAlarmModule
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static long AddDeviceAlarm(Maticsoft.Model.DeviceAlarm model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into DeviceAlarm_" + model.StartTime.Year + "(");
            strSql.Append("DeviceNo,AlarmType,AlarmValue,StartTime,EndTime,Duration,State,SaveTime)");
            strSql.Append(" values (");
            strSql.Append("@DeviceNo,@AlarmType,@AlarmValue,@StartTime,@EndTime,@Duration,@State,@SaveTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar,50),
					new SqlParameter("@AlarmType", SqlDbType.NVarChar,50),
                    new SqlParameter("@AlarmValue", SqlDbType.NVarChar,50),
					new SqlParameter("@StartTime", SqlDbType.DateTime),
					new SqlParameter("@EndTime", SqlDbType.DateTime),
					new SqlParameter("@Duration", SqlDbType.BigInt,8),
					new SqlParameter("@State", SqlDbType.NVarChar,50),
					new SqlParameter("@SaveTime", SqlDbType.DateTime)};
            parameters[0].Value = model.DeviceNo;
            parameters[1].Value = model.AlarmType;
            parameters[2].Value = model.AlarmValue;
            parameters[3].Value = model.StartTime;
            parameters[4].Value = model.EndTime;
            parameters[5].Value = model.Duration;
            parameters[6].Value = model.State;
            parameters[7].Value = model.SaveTime;

            try
            {
                object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
                if (obj == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt64(obj);
                }
            }
            catch
            {
                //如果表不存在，建立表后再插入一次数据
                if (DbHelperSQL.GetSingle("if object_id('DeviceAlarm_" + model.StartTime.Year + "') is not null select 1 else select 0", null).ToString() == "0")
                {
                    DbHelperSQL.ExecuteSql("exec [p_createDeviceAlarmTable] " + model.StartTime.Year);
                    object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
                    if (obj == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return Convert.ToInt64(obj);
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool UpdateDeviceAlarm(Maticsoft.Model.DeviceAlarm model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update DeviceAlarm_" + model.StartTime.Year + " set ");
            strSql.Append("AlarmType=@AlarmType,");
            strSql.Append("AlarmValue=@AlarmValue,");
            strSql.Append("EndTime=@EndTime,");
            strSql.Append("Duration=@Duration,");
            strSql.Append("State=@State,");
            strSql.Append("SaveTime=@SaveTime");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@AlarmType", SqlDbType.NVarChar,50),
                    new SqlParameter("@AlarmValue", SqlDbType.NVarChar,50),
					new SqlParameter("@EndTime", SqlDbType.DateTime),
					new SqlParameter("@Duration", SqlDbType.BigInt,8),
					new SqlParameter("@State", SqlDbType.NVarChar,50),
					new SqlParameter("@SaveTime", SqlDbType.DateTime),
					new SqlParameter("@Id", SqlDbType.BigInt,8),
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar,50),
					new SqlParameter("@StartTime", SqlDbType.DateTime)};
            parameters[0].Value = model.AlarmType;
            parameters[1].Value = model.AlarmValue;
            parameters[2].Value = model.EndTime;
            parameters[3].Value = model.Duration;
            parameters[4].Value = model.State;
            parameters[5].Value = model.SaveTime;
            parameters[6].Value = model.Id;
            parameters[7].Value = model.DeviceNo;
            parameters[8].Value = model.StartTime;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 查询时间段内报警记录
        /// </summary>
        /// <param name="date1">开始时间，大于等于</param>
        /// <param name="date2">结束时间，小于等于</param>
        /// <param name="state">状态，-1只查询Old，1只查询New，0查询所有状态 </param>
        /// <returns></returns>
        public static List<DeviceAlarm> GetAlarm(DateTime date1, DateTime date2, int state)
        {
            ModelHandler<DeviceAlarm> modelHandler = new ModelHandler<DeviceAlarm>();
            List<DeviceAlarm> list = new List<DeviceAlarm>();
            int year = date1.Year;
            while (year <= date2.Year)
            {
                DataTable table = new DataTable();
                string strSql = "select * from DeviceAlarm_" + year + " where 1=1";
                if (year == date1.Year)
                {
                    strSql += " and StartTime>='" + date1.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                }
                if (year == date2.Year)
                {
                    strSql += " and StartTime<='" + date2.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                }
                if (state == 1)
                    strSql += " and State='New'";
                else if (state == -1)
                    strSql += " and State='Old'";

                table = DbHelperSQL.Query(strSql).Tables[0];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DeviceAlarm deviceAlarm = modelHandler.FillModel(table.Rows[i]);
                    list.Add(deviceAlarm);
                }

                year += 1;
            }
            return list;
        }
    }
}
