using DBUtility;
using Maticsoft.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class CardReadLogModule
    {
        public static long AddCardReadLog(CardReadLog cardReadLog)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardReadLog(");
            strSql.Append("SerialNumber,LogUserId,LogUserName,LogAddress,LogTime,LogType,LogContent,AddressCode1,AddressCode2,AddressCode3)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@LogUserId,@LogUserName,@LogAddress,@LogTime,@LogType,@LogContent,@AddressCode1,@AddressCode2,@AddressCode3)");
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
					new SqlParameter("@AddressCode3", SqlDbType.Int,4)};
            parameters[0].Value = cardReadLog.SerialNumber;
            parameters[1].Value = cardReadLog.LogUserId;
            parameters[2].Value = cardReadLog.LogUserName;
            parameters[3].Value = cardReadLog.LogAddress;
            parameters[4].Value = cardReadLog.LogTime;
            parameters[5].Value = cardReadLog.LogType;
            parameters[6].Value = cardReadLog.LogContent;
            parameters[7].Value = cardReadLog.AddressCode1;
            parameters[8].Value = cardReadLog.AddressCode2;
            parameters[9].Value = cardReadLog.AddressCode3;

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
