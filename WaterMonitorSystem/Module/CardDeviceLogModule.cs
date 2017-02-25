using DBUtility;
using Maticsoft.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class CardDeviceLogModule
    {
        public static long AddCardDeviceLog(CardDeviceLog cardDeviceLog)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardDeviceLog(");
            strSql.Append("SerialNumber,LogUserId,LogUserName,LogAddress,LogTime,LogType,LogContent,AddressCode1,AddressCode2,AddressCode3,YearExploitation,AlertAvailableWater,AlertAvailableElectric,TypeCode,MeterPulse,AlertWaterLevel,StationType,StationCode,Frequency)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@LogUserId,@LogUserName,@LogAddress,@LogTime,@LogType,@LogContent,@AddressCode1,@AddressCode2,@AddressCode3,@YearExploitation,@AlertAvailableWater,@AlertAvailableElectric,@TypeCode,@MeterPulse,@AlertWaterLevel,@StationType,@StationCode,@Frequency)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@LogUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LogUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LogAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LogTime", SqlDbType.DateTime),
					new SqlParameter("@LogType", SqlDbType.NVarChar,50),
					new SqlParameter("@LogContent", SqlDbType.NVarChar,-1),
					new SqlParameter("@AddressCode1", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode2", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode3", SqlDbType.Int,4),
					new SqlParameter("@YearExploitation", SqlDbType.Decimal,9),
					new SqlParameter("@AlertAvailableWater", SqlDbType.Int,4),
                    new SqlParameter("@AlertAvailableElectric", SqlDbType.Int,4),
					new SqlParameter("@TypeCode", SqlDbType.Int,4),
					new SqlParameter("@MeterPulse", SqlDbType.Int,4),
					new SqlParameter("@AlertWaterLevel", SqlDbType.Decimal,9),
					new SqlParameter("@StationType", SqlDbType.Int,4),
					new SqlParameter("@StationCode", SqlDbType.Int,4),
					new SqlParameter("@Frequency", SqlDbType.Int,4)};
            parameters[0].Value = cardDeviceLog.SerialNumber;
            parameters[1].Value = cardDeviceLog.LogUserId;
            parameters[2].Value = cardDeviceLog.LogUserName;
            parameters[3].Value = cardDeviceLog.LogAddress;
            parameters[4].Value = cardDeviceLog.LogTime;
            parameters[5].Value = cardDeviceLog.LogType;
            parameters[6].Value = cardDeviceLog.LogContent;
            parameters[7].Value = cardDeviceLog.AddressCode1;
            parameters[8].Value = cardDeviceLog.AddressCode2;
            parameters[9].Value = cardDeviceLog.AddressCode3;
            parameters[10].Value = cardDeviceLog.YearExploitation;
            parameters[11].Value = cardDeviceLog.AlertAvailableWater;
            parameters[12].Value = cardDeviceLog.AlertAvailableElectric;
            parameters[13].Value = cardDeviceLog.TypeCode;
            parameters[14].Value = cardDeviceLog.MeterPulse;
            parameters[15].Value = cardDeviceLog.AlertWaterLevel;
            parameters[16].Value = cardDeviceLog.StationType;
            parameters[17].Value = cardDeviceLog.StationCode;
            parameters[18].Value = cardDeviceLog.Frequency;

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
    }
}
