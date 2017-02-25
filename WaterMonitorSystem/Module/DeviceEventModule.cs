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
    public class DeviceEventModule
    {
        public static bool InitDeviceEvent(Maticsoft.Model.DeviceEvent model)
        {
            try
            {
                model.DeviceNo = "0".PadLeft(8, '0');
                model.EventType = "";
                model.EventTime = DateTime.Now;
                model.DeviceTime = DateTime.Parse("2000-1-1");
                model.DeviceState = "0".PadLeft(32, '0');
                model.SerialNumber = "0".PadLeft(8, '0');
                model.UserNo = "0".PadLeft(8, '0');
                model.StartTime = DateTime.Parse("2000-1-1");
                model.StartResidualWater = 0;
                model.StartResidualElectric = 0;
                model.EndTime = DateTime.Parse("2000-1-1");
                model.EndResidualWater = 0;
                model.EndResidualElectric = 0;
                model.WaterUsed = 0;
                model.ElectricUsed = 0;
                model.YearWaterUsed = 0;
                model.YearElectricUsed = 0;
                model.YearSurplus = 0;
                model.YearExploitation = 0;
                model.RecordType = 0;
                model.REV1 = 1;
                model.REV2 = 1;
                model.Remark = "";
                model.UserId = 0;
                model.UserName = "";
                model.RawData = "";
                model.SendSate = "";

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static long AddDeviceEvent(Maticsoft.Model.DeviceEvent model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into DeviceEvent_" + model.DeviceTime.Year + "(");
            strSql.Append("DeviceNo,EventType,EventTime,DeviceTime,DeviceState,SerialNumber,UserNo,StartTime,StartResidualWater,StartResidualElectric,EndTime,EndResidualWater,EndResidualElectric,WaterUsed,ElectricUsed,YearWaterUsed,YearElectricUsed,YearSurplus,YearExploitation,RecordType,REV1,REV2,Remark,UserId,UserName,RawData,SendSate)");
            strSql.Append(" values (");
            strSql.Append("@DeviceNo,@EventType,@EventTime,@DeviceTime,@DeviceState,@SerialNumber,@UserNo,@StartTime,@StartResidualWater,@StartResidualElectric,@EndTime,@EndResidualWater,@EndResidualElectric,@WaterUsed,@ElectricUsed,@YearWaterUsed,@YearElectricUsed,@YearSurplus,@YearExploitation,@RecordType,@REV1,@REV2,@Remark,@UserId,@UserName,@RawData,@SendSate)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar,50),
					new SqlParameter("@EventType", SqlDbType.NVarChar,50),
					new SqlParameter("@EventTime", SqlDbType.DateTime),
					new SqlParameter("@DeviceTime", SqlDbType.DateTime),
					new SqlParameter("@DeviceState", SqlDbType.NVarChar,50),
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@UserNo", SqlDbType.NVarChar,50),
					new SqlParameter("@StartTime", SqlDbType.DateTime),
					new SqlParameter("@StartResidualWater", SqlDbType.Decimal,9),
					new SqlParameter("@StartResidualElectric", SqlDbType.Decimal,9),
					new SqlParameter("@EndTime", SqlDbType.DateTime),
					new SqlParameter("@EndResidualWater", SqlDbType.Decimal,9),
					new SqlParameter("@EndResidualElectric", SqlDbType.Decimal,9),
					new SqlParameter("@WaterUsed", SqlDbType.Decimal,9),
					new SqlParameter("@ElectricUsed", SqlDbType.Decimal,9),
					new SqlParameter("@YearWaterUsed", SqlDbType.Decimal,9),
					new SqlParameter("@YearElectricUsed", SqlDbType.Decimal,9),
					new SqlParameter("@YearSurplus", SqlDbType.Decimal,9),
					new SqlParameter("@YearExploitation", SqlDbType.Decimal,9),
					new SqlParameter("@RecordType", SqlDbType.TinyInt,1),
					new SqlParameter("@REV1", SqlDbType.TinyInt,1),
					new SqlParameter("@REV2", SqlDbType.TinyInt,1),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@UserId", SqlDbType.BigInt,8),
					new SqlParameter("@UserName", SqlDbType.NVarChar,50),
					new SqlParameter("@RawData", SqlDbType.NVarChar,-1),
					new SqlParameter("@SendSate", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.DeviceNo;
            parameters[1].Value = model.EventType;
            parameters[2].Value = model.EventTime;
            parameters[3].Value = model.DeviceTime;
            parameters[4].Value = model.DeviceState;
            parameters[5].Value = model.SerialNumber;
            parameters[6].Value = model.UserNo;
            parameters[7].Value = model.StartTime;
            parameters[8].Value = model.StartResidualWater;
            parameters[9].Value = model.StartResidualElectric;
            parameters[10].Value = model.EndTime;
            parameters[11].Value = model.EndResidualWater;
            parameters[12].Value = model.EndResidualElectric;
            parameters[13].Value = model.WaterUsed;
            parameters[14].Value = model.ElectricUsed;
            parameters[15].Value = model.YearWaterUsed;
            parameters[16].Value = model.YearElectricUsed;
            parameters[17].Value = model.YearSurplus;
            parameters[18].Value = model.YearExploitation;
            parameters[19].Value = model.RecordType;
            parameters[20].Value = model.REV1;
            parameters[21].Value = model.REV2;
            parameters[22].Value = model.Remark;
            parameters[23].Value = model.UserId;
            parameters[24].Value = model.UserName;
            parameters[25].Value = model.RawData;
            parameters[26].Value = model.SendSate;

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
                if (DbHelperSQL.GetSingle("if object_id('DeviceEvent_" + model.DeviceTime.Year + "') is not null select 1 else select 0", null).ToString() == "0")
                {
                    DbHelperSQL.ExecuteSql("exec [p_createDeviceEventTable] " + model.DeviceTime.Year);
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
        public static bool UpdateDeviceEvent(Maticsoft.Model.DeviceEvent model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update DeviceEvent_" + model.DeviceTime.Year + " set ");
            strSql.Append("EventType=@EventType,");
            strSql.Append("EventTime=@EventTime,");
            strSql.Append("DeviceState=@DeviceState,");
            strSql.Append("SerialNumber=@SerialNumber,");
            strSql.Append("UserNo=@UserNo,");
            strSql.Append("StartTime=@StartTime,");
            strSql.Append("StartResidualWater=@StartResidualWater,");
            strSql.Append("StartResidualElectric=@StartResidualElectric,");
            strSql.Append("EndTime=@EndTime,");
            strSql.Append("EndResidualWater=@EndResidualWater,");
            strSql.Append("EndResidualElectric=@EndResidualElectric,");
            strSql.Append("WaterUsed=@WaterUsed,");
            strSql.Append("ElectricUsed=@ElectricUsed,");
            strSql.Append("YearWaterUsed=@YearWaterUsed,");
            strSql.Append("YearElectricUsed=@YearElectricUsed,");
            strSql.Append("YearSurplus=@YearSurplus,");
            strSql.Append("YearExploitation=@YearExploitation,");
            strSql.Append("RecordType=@RecordType,");
            strSql.Append("REV1=@REV1,");
            strSql.Append("REV2=@REV2,");
            strSql.Append("Remark=@Remark,");
            strSql.Append("UserId=@UserId,");
            strSql.Append("UserName=@UserName,");
            strSql.Append("RawData=@RawData,");
            strSql.Append("SendSate=@SendSate");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@EventType", SqlDbType.NVarChar,50),
					new SqlParameter("@EventTime", SqlDbType.DateTime),
					new SqlParameter("@DeviceState", SqlDbType.NVarChar,50),
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@UserNo", SqlDbType.NVarChar,50),
					new SqlParameter("@StartTime", SqlDbType.DateTime),
					new SqlParameter("@StartResidualWater", SqlDbType.Decimal,9),
					new SqlParameter("@StartResidualElectric", SqlDbType.Decimal,9),
					new SqlParameter("@EndTime", SqlDbType.DateTime),
					new SqlParameter("@EndResidualWater", SqlDbType.Decimal,9),
					new SqlParameter("@EndResidualElectric", SqlDbType.Decimal,9),
					new SqlParameter("@WaterUsed", SqlDbType.Decimal,9),
					new SqlParameter("@ElectricUsed", SqlDbType.Decimal,9),
					new SqlParameter("@YearWaterUsed", SqlDbType.Decimal,9),
					new SqlParameter("@YearElectricUsed", SqlDbType.Decimal,9),
					new SqlParameter("@YearSurplus", SqlDbType.Decimal,9),
					new SqlParameter("@YearExploitation", SqlDbType.Decimal,9),
					new SqlParameter("@RecordType", SqlDbType.TinyInt,1),
					new SqlParameter("@REV1", SqlDbType.TinyInt,1),
					new SqlParameter("@REV2", SqlDbType.TinyInt,1),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@UserId", SqlDbType.BigInt,8),
					new SqlParameter("@UserName", SqlDbType.NVarChar,50),
					new SqlParameter("@RawData", SqlDbType.NVarChar,-1),
					new SqlParameter("@SendSate", SqlDbType.NVarChar,50),
					new SqlParameter("@Id", SqlDbType.BigInt,8),
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar,50),
					new SqlParameter("@DeviceTime", SqlDbType.DateTime)};
            parameters[0].Value = model.EventType;
            parameters[1].Value = model.EventTime;
            parameters[2].Value = model.DeviceState;
            parameters[3].Value = model.SerialNumber;
            parameters[4].Value = model.UserNo;
            parameters[5].Value = model.StartTime;
            parameters[6].Value = model.StartResidualWater;
            parameters[7].Value = model.StartResidualElectric;
            parameters[8].Value = model.EndTime;
            parameters[9].Value = model.EndResidualWater;
            parameters[10].Value = model.EndResidualElectric;
            parameters[11].Value = model.WaterUsed;
            parameters[12].Value = model.ElectricUsed;
            parameters[13].Value = model.YearWaterUsed;
            parameters[14].Value = model.YearElectricUsed;
            parameters[15].Value = model.YearSurplus;
            parameters[16].Value = model.YearExploitation;
            parameters[17].Value = model.RecordType;
            parameters[18].Value = model.REV1;
            parameters[19].Value = model.REV2;
            parameters[20].Value = model.Remark;
            parameters[21].Value = model.UserId;
            parameters[22].Value = model.UserName;
            parameters[23].Value = model.RawData;
            parameters[24].Value = model.SendSate;
            parameters[25].Value = model.Id;
            parameters[26].Value = model.DeviceNo;
            parameters[27].Value = model.DeviceTime;

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
        /// 查询指定终端最新开关泵和状态自报数据
        /// </summary>
        /// <param name="date1">开始时间，大于等于</param>
        /// <param name="date2">结束时间，小于等于</param>
        /// <returns></returns>
        public static DeviceEvent[] GetEventNewByDeviceNo(string DeviceNo, DateTime date)
        {
            ModelHandler<DeviceEvent> modelHandler = new ModelHandler<DeviceEvent>();
            DeviceEvent[] array = new DeviceEvent[3];
            array[0] = null;
            array[1] = null;
            array[2] = null;

            int year = date.Year;

            try
            {
                DataTable table = new DataTable();
                string strSql = "select * from DeviceEvent_" + year + " where DeviceNo='" + DeviceNo + "' and DeviceTime in (select MAX(DeviceTime) from DeviceEvent_" + year + " where DeviceNo='" + DeviceNo + "' and EventType in ('状态自报','开泵上报','关泵上报') group by EventType)";

                table = DbHelperSQL.Query(strSql).Tables[0];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DeviceEvent deviceEvent = modelHandler.FillModel(table.Rows[i]);
                    if (deviceEvent.EventType == "状态自报")
                    {
                        array[0] = deviceEvent;
                    }
                    else if (deviceEvent.EventType == "开泵上报")
                    {
                        if (array[2] == null)
                        {
                            array[1] = deviceEvent;
                        }
                        else
                        {
                            if (deviceEvent.EventTime > array[2].StartTime)
                            {
                                array[1] = deviceEvent;
                                array[2] = null;
                            }
                            else
                            {
                                array[1] = null;
                            }
                        }
                    }
                    else if (deviceEvent.EventType == "关泵上报")
                    {
                        if (array[1] == null)
                        {
                            array[2] = deviceEvent;
                        }
                        else
                        {
                            if (array[1].StartTime > deviceEvent.EventTime)
                            {
                                array[2] = null;
                            }
                            else
                            {
                                array[1] = null;
                                array[2] = deviceEvent;
                            }
                        }
                    }
                }
            }
            catch { }

            return array;
        }
    }
}
