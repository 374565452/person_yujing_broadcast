using DBUtility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class SysLogModule
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static long Add(Maticsoft.Model.SysLog model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into SysLog(");
            strSql.Append("LogUserId,LogUserName,LogAddress,LogTime,LogType,LogContent)");
            strSql.Append(" values (");
            strSql.Append("@LogUserId,@LogUserName,@LogAddress,@LogTime,@LogType,@LogContent)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@LogUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LogUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LogAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LogTime", SqlDbType.DateTime),
					new SqlParameter("@LogType", SqlDbType.NVarChar,50),
					new SqlParameter("@LogContent", SqlDbType.NVarChar,-1)};
            parameters[0].Value = model.LogUserId;
            parameters[1].Value = model.LogUserName;
            parameters[2].Value = model.LogAddress;
            parameters[3].Value = model.LogTime;
            parameters[4].Value = model.LogType;
            parameters[5].Value = model.LogContent;

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
