using DBUtility;
using Maticsoft.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class CardUserLogModule
    {
        public static long AddCardUserLog(CardUserLog cardUserLog)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardUserLog(");
            strSql.Append("SerialNumber,LogUserId,LogUserName,LogAddress,LogTime,LogType,LogContent,UserNo,WaterUserId,ResidualWater,ResidualElectric,TotalWater,TotalElectric,ResidualMoney,TotallMoney,DeviceList,Remark)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@LogUserId,@LogUserName,@LogAddress,@LogTime,@LogType,@LogContent,@UserNo,@WaterUserId,@ResidualWater,@ResidualElectric,@TotalWater,@TotalElectric,@ResidualMoney,@TotallMoney,@DeviceList,@Remark)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@LogUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LogUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LogAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LogTime", SqlDbType.DateTime),
					new SqlParameter("@LogType", SqlDbType.NVarChar,50),
					new SqlParameter("@LogContent", SqlDbType.NVarChar,-1),
					new SqlParameter("@UserNo", SqlDbType.NVarChar,50),
					new SqlParameter("@WaterUserId", SqlDbType.BigInt,8),
					new SqlParameter("@ResidualWater", SqlDbType.Decimal,9),
					new SqlParameter("@ResidualElectric", SqlDbType.Decimal,9),
					new SqlParameter("@TotalWater", SqlDbType.Decimal,9),
					new SqlParameter("@TotalElectric", SqlDbType.Decimal,9),
					new SqlParameter("@ResidualMoney", SqlDbType.Decimal,9),
					new SqlParameter("@TotallMoney", SqlDbType.Decimal,9),
					new SqlParameter("@DeviceList", SqlDbType.NVarChar,-1),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1)};
            parameters[0].Value = cardUserLog.SerialNumber;
            parameters[1].Value = cardUserLog.LogUserId;
            parameters[2].Value = cardUserLog.LogUserName;
            parameters[3].Value = cardUserLog.LogAddress;
            parameters[4].Value = cardUserLog.LogTime;
            parameters[5].Value = cardUserLog.LogType;
            parameters[6].Value = cardUserLog.LogContent;
            parameters[7].Value = cardUserLog.UserNo;
            parameters[8].Value = cardUserLog.WaterUserId;
            parameters[9].Value = cardUserLog.ResidualWater;
            parameters[10].Value = cardUserLog.ResidualElectric;
            parameters[11].Value = cardUserLog.TotalWater;
            parameters[12].Value = cardUserLog.TotalElectric;
            parameters[13].Value = cardUserLog.ResidualMoney;
            parameters[14].Value = cardUserLog.TotallMoney;
            parameters[15].Value = cardUserLog.DeviceList;
            parameters[16].Value = cardUserLog.Remark;

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
