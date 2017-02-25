using Common;
using DBUtility;
using Maticsoft.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class CardDeviceModule
    {
        /// <summary>
        /// 是否存在SerialNumber
        /// </summary>
        public static bool ExistsSerialNumber(string SerialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from CardDevice");
            strSql.Append(" where SerialNumber=@SerialNumber");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar)
			};
            parameters[0].Value = SerialNumber;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在SerialNumber
        /// </summary>
        public static bool ExistsSerialNumber(string SerialNumber, long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from CardDevice");
            strSql.Append(" where SerialNumber=@SerialNumber and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = SerialNumber;
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        public static CardDevice GetCardDeviceBySerialNumber(string SerialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from CardDevice ");
            strSql.Append(" where SerialNumber=@SerialNumber");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar)
			};
            parameters[0].Value = SerialNumber;

            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                ModelHandler<CardDevice> modelHandler = new ModelHandler<CardDevice>();
                return modelHandler.FillModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public static CardDevice GetCardDeviceById(long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from CardDevice ");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = Id;

            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                ModelHandler<CardDevice> modelHandler = new ModelHandler<CardDevice>();
                return modelHandler.FillModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public static long AddCardDevice(CardDevice cardDevice)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardDevice(");
            strSql.Append("SerialNumber,AddressCode1,AddressCode2,AddressCode3,YearExploitation,AlertAvailableWater,AlertAvailableElectric,TypeCode,MeterPulse,AlertWaterLevel,OpenUserId,OpenUserName,OpenAddress,OpenTime,LastUpdateUserId,LastUpdateUserName,LastUpdateAddress,LastUpdateTime,StationType,StationCode,Frequency)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@AddressCode1,@AddressCode2,@AddressCode3,@YearExploitation,@AlertAvailableWater,@AlertAvailableElectric,@TypeCode,@MeterPulse,@AlertWaterLevel,@OpenUserId,@OpenUserName,@OpenAddress,@OpenTime,@LastUpdateUserId,@LastUpdateUserName,@LastUpdateAddress,@LastUpdateTime,@StationType,@StationCode,@Frequency)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode1", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode2", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode3", SqlDbType.Int,4),
					new SqlParameter("@YearExploitation", SqlDbType.Decimal,9),
					new SqlParameter("@AlertAvailableWater", SqlDbType.Int,4),
                    new SqlParameter("@AlertAvailableElectric", SqlDbType.Int,4),
					new SqlParameter("@TypeCode", SqlDbType.Int,4),
					new SqlParameter("@MeterPulse", SqlDbType.Int,4),
					new SqlParameter("@AlertWaterLevel", SqlDbType.Decimal,9),
					new SqlParameter("@OpenUserId", SqlDbType.BigInt,8),
					new SqlParameter("@OpenUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@OpenTime", SqlDbType.DateTime),
					new SqlParameter("@LastUpdateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LastUpdateUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LastUpdateAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LastUpdateTime", SqlDbType.DateTime),
					new SqlParameter("@StationType", SqlDbType.Int,4),
					new SqlParameter("@StationCode", SqlDbType.Int,4),
					new SqlParameter("@Frequency", SqlDbType.Int,4)};
            parameters[0].Value = cardDevice.SerialNumber;
            parameters[1].Value = cardDevice.AddressCode1;
            parameters[2].Value = cardDevice.AddressCode2;
            parameters[3].Value = cardDevice.AddressCode3;
            parameters[4].Value = cardDevice.YearExploitation;
            parameters[5].Value = cardDevice.AlertAvailableWater;
            parameters[6].Value = cardDevice.AlertAvailableElectric;
            parameters[7].Value = cardDevice.TypeCode;
            parameters[8].Value = cardDevice.MeterPulse;
            parameters[9].Value = cardDevice.AlertWaterLevel;
            parameters[10].Value = cardDevice.OpenUserId;
            parameters[11].Value = cardDevice.OpenUserName;
            parameters[12].Value = cardDevice.OpenAddress;
            parameters[13].Value = cardDevice.OpenTime;
            parameters[14].Value = cardDevice.LastUpdateUserId;
            parameters[15].Value = cardDevice.LastUpdateUserName;
            parameters[16].Value = cardDevice.LastUpdateAddress;
            parameters[17].Value = cardDevice.LastUpdateTime;
            parameters[18].Value = cardDevice.StationType;
            parameters[19].Value = cardDevice.StationCode;
            parameters[20].Value = cardDevice.Frequency;

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

        public static string ModifyCardDevice(CardDevice cardDevice)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update CardDevice set ");
            strSql.Append("SerialNumber=@SerialNumber,");
            strSql.Append("AddressCode1=@AddressCode1,");
            strSql.Append("AddressCode2=@AddressCode2,");
            strSql.Append("AddressCode3=@AddressCode3,");
            strSql.Append("YearExploitation=@YearExploitation,");
            strSql.Append("AlertAvailableWater=@AlertAvailableWater,");
            strSql.Append("TypeCode=@TypeCode,");
            strSql.Append("MeterPulse=@MeterPulse,");
            strSql.Append("AlertWaterLevel=@AlertWaterLevel,");
            strSql.Append("AlertAvailableElectric=@AlertAvailableElectric,");
            strSql.Append("OpenUserId=@OpenUserId,");
            strSql.Append("OpenUserName=@OpenUserName,");
            strSql.Append("OpenAddress=@OpenAddress,");
            strSql.Append("OpenTime=@OpenTime,");
            strSql.Append("LastUpdateUserId=@LastUpdateUserId,");
            strSql.Append("LastUpdateUserName=@LastUpdateUserName,");
            strSql.Append("LastUpdateAddress=@LastUpdateAddress,");
            strSql.Append("LastUpdateTime=@LastUpdateTime,");
            strSql.Append("StationType=@StationType,");
            strSql.Append("StationCode=@StationCode,");
            strSql.Append("Frequency=@Frequency");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode1", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode2", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode3", SqlDbType.Int,4),
					new SqlParameter("@YearExploitation", SqlDbType.Decimal,9),
					new SqlParameter("@AlertAvailableWater", SqlDbType.Int,4),
                    new SqlParameter("@AlertAvailableElectric", SqlDbType.Int,4),
					new SqlParameter("@TypeCode", SqlDbType.Int,4),
					new SqlParameter("@MeterPulse", SqlDbType.Int,4),
					new SqlParameter("@AlertWaterLevel", SqlDbType.Decimal,9),
					new SqlParameter("@OpenUserId", SqlDbType.BigInt,8),
					new SqlParameter("@OpenUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@OpenTime", SqlDbType.DateTime),
					new SqlParameter("@LastUpdateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LastUpdateUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LastUpdateAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LastUpdateTime", SqlDbType.DateTime),
					new SqlParameter("@StationType", SqlDbType.Int,4),
					new SqlParameter("@StationCode", SqlDbType.Int,4),
					new SqlParameter("@Frequency", SqlDbType.Int,4),
					new SqlParameter("@Id", SqlDbType.BigInt,8)};
            parameters[0].Value = cardDevice.SerialNumber;
            parameters[1].Value = cardDevice.AddressCode1;
            parameters[2].Value = cardDevice.AddressCode2;
            parameters[3].Value = cardDevice.AddressCode3;
            parameters[4].Value = cardDevice.YearExploitation;
            parameters[5].Value = cardDevice.AlertAvailableWater;
            parameters[6].Value = cardDevice.AlertAvailableElectric;
            parameters[7].Value = cardDevice.TypeCode;
            parameters[8].Value = cardDevice.MeterPulse;
            parameters[9].Value = cardDevice.AlertWaterLevel;
            parameters[10].Value = cardDevice.OpenUserId;
            parameters[11].Value = cardDevice.OpenUserName;
            parameters[12].Value = cardDevice.OpenAddress;
            parameters[13].Value = cardDevice.OpenTime;
            parameters[14].Value = cardDevice.LastUpdateUserId;
            parameters[15].Value = cardDevice.LastUpdateUserName;
            parameters[16].Value = cardDevice.LastUpdateAddress;
            parameters[17].Value = cardDevice.LastUpdateTime;
            parameters[18].Value = cardDevice.StationType;
            parameters[19].Value = cardDevice.StationCode;
            parameters[20].Value = cardDevice.Frequency;
            parameters[21].Value = cardDevice.Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return "修改成功";
            }
            else
            {
                return "修改失败，原因：写入数据库失败！";
            }
        }

        public static string DeleteCardDevice(long Id)
        {
            string strSql = "delete CardDevice where Id=@Id";
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = Id;
            try
            {
                int rows = DbHelperSQL.ExecuteSql(strSql, parameters);
                if (rows > 0)
                {
                    return "删除成功";
                }
                else
                {
                    return "删除失败，原因：写入数据库失败！";
                }
            }
            catch
            {
                return "删除用户卡失败，原因：写入数据库失败！";
            }
        }

        public static List<CardDevice> GetCardDevicesByDistrictId(long DistrictId)
        {
            District dist = DistrictModule.ReturnDistrictInfo(DistrictId);
            return GetCardDevicesByDistrict(dist);
        }

        public static List<CardDevice> GetCardDevicesByDistrict(District dist)
        {
            List<CardDevice> list = new List<CardDevice>();
            try
            {
                if (dist != null)
                {
                    District node2 = DistrictModule.ReturnDistrictInfo(dist.ParentId);
                    District node3 = DistrictModule.ReturnDistrictInfo(node2.ParentId);
                    District node4 = DistrictModule.ReturnDistrictInfo(node3.ParentId);
                    District node5 = DistrictModule.ReturnDistrictInfo(node4.ParentId);

                    string sql = "select * from CardDevice where AddressCode1='" + (node5.DistrictCode + node4.DistrictCode + node3.DistrictCode) +
                        "' and AddressCode2='" + (node2.DistrictCode + dist.DistrictCode) + "'";
                    DataTable table = DbHelperSQL.QueryDataTable(sql);
                    if (table.Rows.Count != 0)
                    {
                        ModelHandler<CardDevice> modelHandler = new ModelHandler<CardDevice>();
                        list = modelHandler.FillModel(table);
                    }

                }
            }
            catch { }
            return list;
        }

        public static JavaScriptObject CardDeviceToJson(CardDevice cardDevice)
        {
            JavaScriptObject obj2 = new JavaScriptObject();

            obj2.Add("ID", cardDevice.Id);
            obj2.Add("SerialNumber", cardDevice.SerialNumber);
            obj2.Add("AddressCode1", cardDevice.AddressCode1);
            obj2.Add("AddressCode2", cardDevice.AddressCode2);
            obj2.Add("AddressCode3", cardDevice.AddressCode3);
            obj2.Add("DeviceNo", cardDevice.AddressCode1 + cardDevice.AddressCode2 + cardDevice.AddressCode3.ToString().PadLeft(3, '0'));
            obj2.Add("YearExploitation", cardDevice.YearExploitation);
            obj2.Add("AlertAvailableWater", cardDevice.AlertAvailableWater);
            obj2.Add("AlertAvailableElectric", cardDevice.AlertAvailableElectric);
            obj2.Add("TypeCode", cardDevice.TypeCode);
            obj2.Add("MeterPulse", cardDevice.MeterPulse);
            obj2.Add("AlertWaterLevel", cardDevice.AlertWaterLevel);
            obj2.Add("StationType", cardDevice.StationType);
            obj2.Add("StationCode", cardDevice.StationCode);
            obj2.Add("Frequency", cardDevice.Frequency);
            obj2.Add("OpenUserId", cardDevice.OpenUserId);
            obj2.Add("OpenUserName", cardDevice.OpenUserName);
            obj2.Add("OpenAddress", cardDevice.OpenAddress);
            obj2.Add("OpenTime", cardDevice.OpenTime.ToString("yyyy-MM-dd HH:mm:ss"));
            obj2.Add("LastUpdateUserId", cardDevice.LastUpdateUserId);
            obj2.Add("LastUpdateUserName", cardDevice.LastUpdateUserName);
            obj2.Add("LastUpdateAddress", cardDevice.LastUpdateAddress);
            obj2.Add("LastUpdateTime", cardDevice.LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss"));

            return obj2;
        }
    }
}
