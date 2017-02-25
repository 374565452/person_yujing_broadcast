using Common;
using DBUtility;
using Maticsoft.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class CardNetSetModule
    {
        /// <summary>
        /// 是否存在SerialNumber
        /// </summary>
        public static bool ExistsSerialNumber(string SerialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from CardNetSet");
            strSql.Append(" where SerialNumber=@SerialNumber");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar)
			};
            parameters[0].Value = SerialNumber;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在SerialNumber
        /// </summary>
        public static bool ExistsSerialNumber(string SerialNumber, long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from CardNetSet");
            strSql.Append(" where SerialNumber=@SerialNumber and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = SerialNumber;
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        public static CardNetSet GetCardNetSetBySerialNumber(string SerialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 Id,SerialNumber,IP,Port,IsDomain,APNName,APNUserName,APNPassword,OpenUserId,OpenUserName,OpenAddress,OpenTime,LastUpdateUserId,LastUpdateUserName,LastUpdateAddress,LastUpdateTime from CardNetSet ");
            strSql.Append(" where SerialNumber=@SerialNumber");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar)
			};
            parameters[0].Value = SerialNumber;

            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                ModelHandler<CardNetSet> modelHandler = new ModelHandler<CardNetSet>();
                return modelHandler.FillModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public static long AddCardNetSet(CardNetSet cardNetSet)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardNetSet(");
            strSql.Append("SerialNumber,IP,Port,IsDomain,APNName,APNUserName,APNPassword,OpenUserId,OpenUserName,OpenAddress,OpenTime,LastUpdateUserId,LastUpdateUserName,LastUpdateAddress,LastUpdateTime)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@IP,@Port,@IsDomain,@APNName,@APNUserName,@APNPassword,@OpenUserId,@OpenUserName,@OpenAddress,@OpenTime,@LastUpdateUserId,@LastUpdateUserName,@LastUpdateAddress,@LastUpdateTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@IP", SqlDbType.NVarChar,50),
					new SqlParameter("@Port", SqlDbType.Int,4),
                    new SqlParameter("@IsDomain", SqlDbType.NVarChar,50),
					new SqlParameter("@APNName", SqlDbType.NVarChar,50),
					new SqlParameter("@APNUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@APNPassword", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenUserId", SqlDbType.BigInt,8),
					new SqlParameter("@OpenUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@OpenTime", SqlDbType.DateTime),
					new SqlParameter("@LastUpdateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LastUpdateUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LastUpdateAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LastUpdateTime", SqlDbType.DateTime)};
            parameters[0].Value = cardNetSet.SerialNumber;
            parameters[1].Value = cardNetSet.IP;
            parameters[2].Value = cardNetSet.Port;
            parameters[3].Value = cardNetSet.IsDomain;
            parameters[4].Value = cardNetSet.APNName;
            parameters[5].Value = cardNetSet.APNUserName;
            parameters[6].Value = cardNetSet.APNPassword;
            parameters[7].Value = cardNetSet.OpenUserId;
            parameters[8].Value = cardNetSet.OpenUserName;
            parameters[9].Value = cardNetSet.OpenAddress;
            parameters[10].Value = cardNetSet.OpenTime;
            parameters[11].Value = cardNetSet.LastUpdateUserId;
            parameters[12].Value = cardNetSet.LastUpdateUserName;
            parameters[13].Value = cardNetSet.LastUpdateAddress;
            parameters[14].Value = cardNetSet.LastUpdateTime;

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

        public static string ModifyCardNetSet(CardNetSet cardNetSet)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update CardNetSet set ");
            strSql.Append("SerialNumber=@SerialNumber,");
            strSql.Append("IP=@IP,");
            strSql.Append("Port=@Port,");
            strSql.Append("IsDomain=@IsDomain,");
            strSql.Append("APNName=@APNName,");
            strSql.Append("APNUserName=@APNUserName,");
            strSql.Append("APNPassword=@APNPassword,");
            strSql.Append("OpenUserId=@OpenUserId,");
            strSql.Append("OpenUserName=@OpenUserName,");
            strSql.Append("OpenAddress=@OpenAddress,");
            strSql.Append("OpenTime=@OpenTime,");
            strSql.Append("LastUpdateUserId=@LastUpdateUserId,");
            strSql.Append("LastUpdateUserName=@LastUpdateUserName,");
            strSql.Append("LastUpdateAddress=@LastUpdateAddress,");
            strSql.Append("LastUpdateTime=@LastUpdateTime");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@IP", SqlDbType.NVarChar,50),
					new SqlParameter("@Port", SqlDbType.Int,4),
                    new SqlParameter("@IsDomain", SqlDbType.NVarChar,50),
					new SqlParameter("@APNName", SqlDbType.NVarChar,50),
					new SqlParameter("@APNUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@APNPassword", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenUserId", SqlDbType.BigInt,8),
					new SqlParameter("@OpenUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@OpenTime", SqlDbType.DateTime),
					new SqlParameter("@LastUpdateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LastUpdateUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LastUpdateAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LastUpdateTime", SqlDbType.DateTime),
					new SqlParameter("@Id", SqlDbType.BigInt,8)};
            parameters[0].Value = cardNetSet.SerialNumber;
            parameters[1].Value = cardNetSet.IP;
            parameters[2].Value = cardNetSet.Port;
            parameters[3].Value = cardNetSet.IsDomain;
            parameters[4].Value = cardNetSet.APNName;
            parameters[5].Value = cardNetSet.APNUserName;
            parameters[6].Value = cardNetSet.APNPassword;
            parameters[7].Value = cardNetSet.OpenUserId;
            parameters[8].Value = cardNetSet.OpenUserName;
            parameters[9].Value = cardNetSet.OpenAddress;
            parameters[10].Value = cardNetSet.OpenTime;
            parameters[11].Value = cardNetSet.LastUpdateUserId;
            parameters[12].Value = cardNetSet.LastUpdateUserName;
            parameters[13].Value = cardNetSet.LastUpdateAddress;
            parameters[14].Value = cardNetSet.LastUpdateTime;
            parameters[15].Value = cardNetSet.Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return "修改成功";
            }
            else
            {
                return "修改失败，原因：写入数据库失败！";
            }
        }

        public static string DeleteCardNetSet(long Id)
        {
            string strSql = "delete CardNetSet where Id=@Id";
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = Id;
            try
            {
                int rows = DbHelperSQL.ExecuteSql(strSql, parameters);
                if (rows > 0)
                {
                    return "删除成功";
                }
                else
                {
                    return "删除失败，原因：写入数据库失败！";
                }
            }
            catch
            {
                return "删除用户卡失败，原因：写入数据库失败！";
            }
        }
    }
}
