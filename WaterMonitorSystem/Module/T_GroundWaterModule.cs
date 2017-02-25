using DBUtility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class T_GroundWaterModule
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static int Add(Maticsoft.Model.T_GroundWater model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into T_GroundWater(");
            strSql.Append("StationID,GroundWaterLevel,LineLength,GroundWaterTempture,BV,Acq_Time,CREATE_TIME)");
            strSql.Append(" values (");
            strSql.Append("@StationID,@GroundWaterLevel,@LineLength,@GroundWaterTempture,@BV,@Acq_Time,@CREATE_TIME)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@StationID", SqlDbType.VarChar,20),
					new SqlParameter("@GroundWaterLevel", SqlDbType.Decimal,5),
                    new SqlParameter("@LineLength", SqlDbType.Decimal,5),
					new SqlParameter("@GroundWaterTempture", SqlDbType.Decimal,5),
					new SqlParameter("@BV", SqlDbType.Decimal,5),
					new SqlParameter("@Acq_Time", SqlDbType.DateTime),
					new SqlParameter("@CREATE_TIME", SqlDbType.DateTime)};
            parameters[0].Value = model.StationID;
            parameters[1].Value = model.GroundWaterLevel;
            parameters[2].Value = model.LineLength;
            parameters[3].Value = model.GroundWaterTempture;
            parameters[4].Value = model.BV;
            parameters[5].Value = model.Acq_Time;
            parameters[6].Value = model.CREATE_TIME;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }
    }
}
