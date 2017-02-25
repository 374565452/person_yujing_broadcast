using DBUtility;
using Maticsoft.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class CardNetSetLogModule
    {
        public static long AddCardNetSetLog(CardNetSetLog cardNetSetLog)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardNetSetLog(");
            strSql.Append("SerialNumber,LogUserId,LogUserName,LogAddress,LogTime,LogType,LogContent,IP,Port,IsDomain,APNName,APNUserName,APNPassword)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@LogUserId,@LogUserName,@LogAddress,@LogTime,@LogType,@LogContent,@IP,@Port,@IsDomain,@APNName,@APNUserName,@APNPassword)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@LogUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LogUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LogAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LogTime", SqlDbType.DateTime),
					new SqlParameter("@LogType", SqlDbType.NVarChar,50),
					new SqlParameter("@LogContent", SqlDbType.NVarChar,-1),
					new SqlParameter("@IP", SqlDbType.NVarChar,50),
					new SqlParameter("@Port", SqlDbType.Int,4),
                    new SqlParameter("@IsDomain", SqlDbType.NVarChar,50),
					new SqlParameter("@APNName", SqlDbType.NVarChar,50),
					new SqlParameter("@APNUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@APNPassword", SqlDbType.NVarChar,50)};
            parameters[0].Value = cardNetSetLog.SerialNumber;
            parameters[1].Value = cardNetSetLog.LogUserId;
            parameters[2].Value = cardNetSetLog.LogUserName;
            parameters[3].Value = cardNetSetLog.LogAddress;
            parameters[4].Value = cardNetSetLog.LogTime;
            parameters[5].Value = cardNetSetLog.LogType;
            parameters[6].Value = cardNetSetLog.LogContent;
            parameters[7].Value = cardNetSetLog.IP;
            parameters[8].Value = cardNetSetLog.Port;
            parameters[9].Value = cardNetSetLog.IsDomain;
            parameters[10].Value = cardNetSetLog.APNName;
            parameters[11].Value = cardNetSetLog.APNUserName;
            parameters[12].Value = cardNetSetLog.APNPassword;

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
