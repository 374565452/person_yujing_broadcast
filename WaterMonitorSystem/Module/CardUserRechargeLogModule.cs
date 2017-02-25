using DBUtility;
using Maticsoft.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module
{
    public class CardUserRechargeLogModule
    {
        public static long AddCardUserRechargeLog(CardUserRechargeLog cardUserRechargeLog)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardUserRechargeLog(");
            strSql.Append("SerialNumber,UserNo,WateUserId,LogUserId,LogUserName,LogAddress,LogTime,LogType,RechargeType,WaterPrice,WaterNum,ElectricPrice,ElectricNum,TotalPrice,Remark,WaterQuota,ElectricQuota,WaterPriceId,ElectricPriceId,WaterUsed,ElectricUsed)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@UserNo,@WateUserId,@LogUserId,@LogUserName,@LogAddress,@LogTime,@LogType,@RechargeType,@WaterPrice,@WaterNum,@ElectricPrice,@ElectricNum,@TotalPrice,@Remark,@WaterQuota,@ElectricQuota,@WaterPriceId,@ElectricPriceId,@WaterUsed,@ElectricUsed)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@UserNo", SqlDbType.NVarChar,50),
					new SqlParameter("@WateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LogUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LogUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LogAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LogTime", SqlDbType.DateTime),
					new SqlParameter("@LogType", SqlDbType.NVarChar,50),
					new SqlParameter("@RechargeType", SqlDbType.NVarChar,50),
					new SqlParameter("@WaterPrice", SqlDbType.Decimal,9),
					new SqlParameter("@WaterNum", SqlDbType.Decimal,9),
					new SqlParameter("@ElectricPrice", SqlDbType.Decimal,9),
					new SqlParameter("@ElectricNum", SqlDbType.Decimal,9),
					new SqlParameter("@TotalPrice", SqlDbType.Decimal,9),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@WaterQuota", SqlDbType.Decimal,9),
					new SqlParameter("@ElectricQuota", SqlDbType.Decimal,9),
					new SqlParameter("@WaterPriceId", SqlDbType.BigInt,8),
					new SqlParameter("@ElectricPriceId", SqlDbType.BigInt,8),
					new SqlParameter("@WaterUsed", SqlDbType.Decimal,9),
					new SqlParameter("@ElectricUsed", SqlDbType.Decimal,9)};
            parameters[0].Value = cardUserRechargeLog.SerialNumber;
            parameters[1].Value = cardUserRechargeLog.UserNo;
            parameters[2].Value = cardUserRechargeLog.WateUserId;
            parameters[3].Value = cardUserRechargeLog.LogUserId;
            parameters[4].Value = cardUserRechargeLog.LogUserName;
            parameters[5].Value = cardUserRechargeLog.LogAddress;
            parameters[6].Value = cardUserRechargeLog.LogTime;
            parameters[7].Value = cardUserRechargeLog.LogType;
            parameters[8].Value = cardUserRechargeLog.RechargeType;
            parameters[9].Value = cardUserRechargeLog.WaterPrice;
            parameters[10].Value = cardUserRechargeLog.WaterNum;
            parameters[11].Value = cardUserRechargeLog.ElectricPrice;
            parameters[12].Value = cardUserRechargeLog.ElectricNum;
            parameters[13].Value = cardUserRechargeLog.TotalPrice;
            parameters[14].Value = cardUserRechargeLog.Remark;
            parameters[15].Value = cardUserRechargeLog.WaterQuota;
            parameters[16].Value = cardUserRechargeLog.ElectricQuota;
            parameters[17].Value = cardUserRechargeLog.WaterPriceId;
            parameters[18].Value = cardUserRechargeLog.ElectricPriceId;
            parameters[19].Value = cardUserRechargeLog.WaterUsed;
            parameters[20].Value = cardUserRechargeLog.ElectricUsed;

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

        public static DataTable GetCardUserRechargeYear(long WateUserId, int Year)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select isnull(SUM(WaterNum),0),isnull(SUM(ElectricNum),0)");
            strSql.Append(" from CardUserRechargeLog");
            strSql.Append(" where WateUserId=@WateUserId and YEAR(LogTime)=@Year");

            SqlParameter[] parameters = {
					new SqlParameter("@WateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@Year", SqlDbType.Int)};
            parameters[0].Value = WateUserId;
            parameters[1].Value = Year;

            return DbHelperSQL.Query(strSql.ToString(), parameters).Tables[0];
        }
    }
}
