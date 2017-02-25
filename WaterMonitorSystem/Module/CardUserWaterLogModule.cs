using DBUtility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class CardUserWaterLogModule
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static long AddCardUserWaterLog(Maticsoft.Model.CardUserWaterLog model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardUserWaterLog(");
            strSql.Append("SerialNumber,WateUserId,UserNo,DeviceId,DeviceNo,StartTime,StartResidualWater,StartResidualElectric,EndTime,EndResidualWater,EndResidualElectric,Duration,WaterUsed,ElectricUsed)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@WateUserId,@UserNo,@DeviceId,@DeviceNo,@StartTime,@StartResidualWater,@StartResidualElectric,@EndTime,@EndResidualWater,@EndResidualElectric,@Duration,@WaterUsed,@ElectricUsed)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@WateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@UserNo", SqlDbType.NVarChar,50),
					new SqlParameter("@DeviceId", SqlDbType.BigInt,8),
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar,50),
					new SqlParameter("@StartTime", SqlDbType.DateTime),
					new SqlParameter("@StartResidualWater", SqlDbType.Decimal,9),
					new SqlParameter("@StartResidualElectric", SqlDbType.Decimal,9),
					new SqlParameter("@EndTime", SqlDbType.DateTime),
					new SqlParameter("@EndResidualWater", SqlDbType.Decimal,9),
					new SqlParameter("@EndResidualElectric", SqlDbType.Decimal,9),
					new SqlParameter("@Duration", SqlDbType.Decimal,9),
					new SqlParameter("@WaterUsed", SqlDbType.Decimal,9),
					new SqlParameter("@ElectricUsed", SqlDbType.Decimal,9)};
            parameters[0].Value = model.SerialNumber;
            parameters[1].Value = model.WateUserId;
            parameters[2].Value = model.UserNo;
            parameters[3].Value = model.DeviceId;
            parameters[4].Value = model.DeviceNo;
            parameters[5].Value = model.StartTime;
            parameters[6].Value = model.StartResidualWater;
            parameters[7].Value = model.StartResidualElectric;
            parameters[8].Value = model.EndTime;
            parameters[9].Value = model.EndResidualWater;
            parameters[10].Value = model.EndResidualElectric;
            parameters[11].Value = model.Duration;
            parameters[12].Value = model.WaterUsed;
            parameters[13].Value = model.ElectricUsed;

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
