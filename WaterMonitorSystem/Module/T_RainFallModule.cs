using DBUtility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class T_RainFallModule
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static long Add(Maticsoft.Model.T_RainFall model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into T_RainFall(");
            strSql.Append("StationID,Rainfall,Rainfall_Hour,Rainfall_Day,Rainfall_Total,WaterLevel,BV,Acq_Time,CREATE_TIME)");
            strSql.Append(" values (");
            strSql.Append("@StationID,@Rainfall,@Rainfall_Hour,@Rainfall_Day,@Rainfall_Total,@WaterLevel,@BV,@Acq_Time,@CREATE_TIME)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@StationID", SqlDbType.VarChar,20),
					new SqlParameter("@Rainfall", SqlDbType.Decimal,5),
					new SqlParameter("@Rainfall_Hour", SqlDbType.Decimal,5),
					new SqlParameter("@Rainfall_Day", SqlDbType.Decimal,5),
					new SqlParameter("@Rainfall_Total", SqlDbType.Decimal,5),
					new SqlParameter("@WaterLevel", SqlDbType.Decimal,5),
					new SqlParameter("@BV", SqlDbType.Decimal,5),
					new SqlParameter("@Acq_Time", SqlDbType.DateTime),
					new SqlParameter("@CREATE_TIME", SqlDbType.DateTime)};
            parameters[0].Value = model.StationID;
            parameters[1].Value = model.Rainfall;
            parameters[2].Value = model.Rainfall_Hour;
            parameters[3].Value = model.Rainfall_Day;
            parameters[4].Value = model.Rainfall_Total;
            parameters[5].Value = model.WaterLevel;
            parameters[6].Value = model.BV;
            parameters[7].Value = model.Acq_Time;
            parameters[8].Value = model.CREATE_TIME;

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

