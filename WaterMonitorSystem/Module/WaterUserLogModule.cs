using DBUtility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class WaterUserLogModule
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static long Add(Maticsoft.Model.WaterUserLog model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into WaterUserLog(");
            strSql.Append("WaterUserId,LogUserId,LogUserName,LogAddress,LogTime,LogType,LogContent,UserName,Password,DistrictId,TrueName,IdentityNumber,Telephone,Address,WaterQuota,ElectricQuota,Remark,水价ID,电价ID,State)");
            strSql.Append(" values (");
            strSql.Append("@WaterUserId,@LogUserId,@LogUserName,@LogAddress,@LogTime,@LogType,@LogContent,@UserName,@Password,@DistrictId,@TrueName,@IdentityNumber,@Telephone,@Address,@WaterQuota,@ElectricQuota,@Remark,@水价ID,@电价ID,@State)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@WaterUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LogUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LogUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LogAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LogTime", SqlDbType.DateTime),
					new SqlParameter("@LogType", SqlDbType.NVarChar,50),
					new SqlParameter("@LogContent", SqlDbType.NVarChar,-1),
					new SqlParameter("@UserName", SqlDbType.NVarChar,50),
					new SqlParameter("@Password", SqlDbType.NVarChar,50),
					new SqlParameter("@DistrictId", SqlDbType.BigInt,8),
					new SqlParameter("@TrueName", SqlDbType.NVarChar,50),
					new SqlParameter("@IdentityNumber", SqlDbType.NVarChar,20),
					new SqlParameter("@Telephone", SqlDbType.NVarChar,50),
					new SqlParameter("@Address", SqlDbType.NVarChar,200),
					new SqlParameter("@WaterQuota", SqlDbType.Decimal,9),
					new SqlParameter("@ElectricQuota", SqlDbType.Decimal,9),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@水价ID", SqlDbType.Int,4),
					new SqlParameter("@电价ID", SqlDbType.Int,4),
					new SqlParameter("@State", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.WaterUserId;
            parameters[1].Value = model.LogUserId;
            parameters[2].Value = model.LogUserName;
            parameters[3].Value = model.LogAddress;
            parameters[4].Value = model.LogTime;
            parameters[5].Value = model.LogType;
            parameters[6].Value = model.LogContent;
            parameters[7].Value = model.UserName;
            parameters[8].Value = model.Password;
            parameters[9].Value = model.DistrictId;
            parameters[10].Value = model.TrueName;
            parameters[11].Value = model.IdentityNumber;
            parameters[12].Value = model.Telephone;
            parameters[13].Value = model.Address;
            parameters[14].Value = model.WaterQuota;
            parameters[15].Value = model.ElectricQuota;
            parameters[16].Value = model.Remark;
            parameters[17].Value = model.水价ID;
            parameters[18].Value = model.电价ID;
            parameters[19].Value = model.State;

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
