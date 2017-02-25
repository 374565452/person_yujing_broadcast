using DBUtility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class DeviceLogModule
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static long Add(Maticsoft.Model.DeviceLog model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into DeviceLog(");
            strSql.Append("DeviceId,LogUserId,LogUserName,LogAddress,LogTime,LogType,LogContent,SimNo,DeviceName,Description,SetupDate,SetupAddress,LON,LAT,IsValid,LastUpdate,DistrictId,DeviceNo,Online,OnlineTime,YearExploitation,AlertAvailableWater,AlertAvailableElectric,DeviceTypeCodeId,MeterPulse,AlertWaterLevel,TerminalState,Remark,CropId,Area,StationType,StationCode,Frequency,MainId,DeviceType)");
            strSql.Append(" values (");
            strSql.Append("@DeviceId,@LogUserId,@LogUserName,@LogAddress,@LogTime,@LogType,@LogContent,@SimNo,@DeviceName,@Description,@SetupDate,@SetupAddress,@LON,@LAT,@IsValid,@LastUpdate,@DistrictId,@DeviceNo,@Online,@OnlineTime,@YearExploitation,@AlertAvailableWater,@AlertAvailableElectric,@DeviceTypeCodeId,@MeterPulse,@AlertWaterLevel,@TerminalState,@Remark,@CropId,@Area,@StationType,@StationCode,@Frequency,@MainId,@DeviceType)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@DeviceId", SqlDbType.BigInt,8),
					new SqlParameter("@LogUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LogUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LogAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LogTime", SqlDbType.DateTime),
					new SqlParameter("@LogType", SqlDbType.NVarChar,50),
					new SqlParameter("@LogContent", SqlDbType.NVarChar,-1),
					new SqlParameter("@SimNo", SqlDbType.NVarChar,20),
					new SqlParameter("@DeviceName", SqlDbType.NVarChar,50),
					new SqlParameter("@Description", SqlDbType.VarChar,200),
					new SqlParameter("@SetupDate", SqlDbType.DateTime),
					new SqlParameter("@SetupAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LON", SqlDbType.BigInt,8),
					new SqlParameter("@LAT", SqlDbType.BigInt,8),
					new SqlParameter("@IsValid", SqlDbType.Int,4),
					new SqlParameter("@LastUpdate", SqlDbType.DateTime),
					new SqlParameter("@DistrictId", SqlDbType.BigInt,8),
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar,50),
					new SqlParameter("@Online", SqlDbType.Int,4),
					new SqlParameter("@OnlineTime", SqlDbType.DateTime),
					new SqlParameter("@YearExploitation", SqlDbType.Decimal,9),
					new SqlParameter("@AlertAvailableWater", SqlDbType.Int,4),
                    new SqlParameter("@AlertAvailableElectric", SqlDbType.Int,4),
					new SqlParameter("@DeviceTypeCodeId", SqlDbType.Int,4),
					new SqlParameter("@MeterPulse", SqlDbType.Int,4),
					new SqlParameter("@AlertWaterLevel", SqlDbType.Decimal,9),
					new SqlParameter("@TerminalState", SqlDbType.NVarChar,50),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@CropId", SqlDbType.BigInt,8),
					new SqlParameter("@Area", SqlDbType.Decimal,9),
					new SqlParameter("@StationType", SqlDbType.Int,4),
					new SqlParameter("@StationCode", SqlDbType.Int,4),
					new SqlParameter("@Frequency", SqlDbType.Int,4),
					new SqlParameter("@MainId", SqlDbType.BigInt,8),
					new SqlParameter("@DeviceType", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.DeviceId;
            parameters[1].Value = model.LogUserId;
            parameters[2].Value = model.LogUserName;
            parameters[3].Value = model.LogAddress;
            parameters[4].Value = model.LogTime;
            parameters[5].Value = model.LogType;
            parameters[6].Value = model.LogContent;
            parameters[7].Value = model.SimNo;
            parameters[8].Value = model.DeviceName;
            parameters[9].Value = model.Description;
            parameters[10].Value = model.SetupDate;
            parameters[11].Value = model.SetupAddress;
            parameters[12].Value = model.LON;
            parameters[13].Value = model.LAT;
            parameters[14].Value = model.IsValid;
            parameters[15].Value = model.LastUpdate;
            parameters[16].Value = model.DistrictId;
            parameters[17].Value = model.DeviceNo;
            parameters[18].Value = model.Online;
            parameters[19].Value = model.OnlineTime;
            parameters[20].Value = model.YearExploitation;
            parameters[21].Value = model.AlertAvailableWater;
            parameters[22].Value = model.AlertAvailableElectric;
            parameters[23].Value = model.DeviceTypeCodeId;
            parameters[24].Value = model.MeterPulse;
            parameters[25].Value = model.AlertWaterLevel;
            parameters[26].Value = model.TerminalState;
            parameters[27].Value = model.Remark;
            parameters[28].Value = model.CropId;
            parameters[29].Value = model.Area;
            parameters[30].Value = model.StationType;
            parameters[31].Value = model.StationCode;
            parameters[32].Value = model.Frequency;
            parameters[33].Value = model.MainId;
            parameters[34].Value = model.DeviceType;

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
