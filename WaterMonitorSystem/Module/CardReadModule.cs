using Common;
using DBUtility;
using Maticsoft.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class CardReadModule
    {
        /// <summary>
        /// 是否存在SerialNumber
        /// </summary>
        public static bool ExistsSerialNumber(string SerialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from CardRead");
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
            strSql.Append("select count(1) from CardRead");
            strSql.Append(" where SerialNumber=@SerialNumber and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = SerialNumber;
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        public static CardRead GetCardReadBySerialNumber(string SerialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 Id,SerialNumber,AddressCode1,AddressCode2,AddressCode3,OpenUserId,OpenUserName,OpenAddress,OpenTime,LastUpdateUserId,LastUpdateUserName,LastUpdateAddress,LastUpdateTime from CardRead ");
            strSql.Append(" where SerialNumber=@SerialNumber");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar)
			};
            parameters[0].Value = SerialNumber;

            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                ModelHandler<CardRead> modelHandler = new ModelHandler<CardRead>();
                return modelHandler.FillModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public static long AddCardRead(CardRead cardRead)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardRead(");
            strSql.Append("SerialNumber,AddressCode1,AddressCode2,AddressCode3,OpenUserId,OpenUserName,OpenAddress,OpenTime,LastUpdateUserId,LastUpdateUserName,LastUpdateAddress,LastUpdateTime)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@AddressCode1,@AddressCode2,@AddressCode3,@OpenUserId,@OpenUserName,@OpenAddress,@OpenTime,@LastUpdateUserId,@LastUpdateUserName,@LastUpdateAddress,@LastUpdateTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode1", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode2", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode3", SqlDbType.Int,4),
					new SqlParameter("@OpenUserId", SqlDbType.BigInt,8),
					new SqlParameter("@OpenUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@OpenTime", SqlDbType.DateTime),
					new SqlParameter("@LastUpdateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LastUpdateUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LastUpdateAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LastUpdateTime", SqlDbType.DateTime)};
            parameters[0].Value = cardRead.SerialNumber;
            parameters[1].Value = cardRead.AddressCode1;
            parameters[2].Value = cardRead.AddressCode2;
            parameters[3].Value = cardRead.AddressCode3;
            parameters[4].Value = cardRead.OpenUserId;
            parameters[5].Value = cardRead.OpenUserName;
            parameters[6].Value = cardRead.OpenAddress;
            parameters[7].Value = cardRead.OpenTime;
            parameters[8].Value = cardRead.LastUpdateUserId;
            parameters[9].Value = cardRead.LastUpdateUserName;
            parameters[10].Value = cardRead.LastUpdateAddress;
            parameters[11].Value = cardRead.LastUpdateTime;

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

        public static string ModifyCardRead(CardRead cardRead)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update CardRead set ");
            strSql.Append("SerialNumber=@SerialNumber,");
            strSql.Append("AddressCode1=@AddressCode1,");
            strSql.Append("AddressCode2=@AddressCode2,");
            strSql.Append("AddressCode3=@AddressCode3,");
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
					new SqlParameter("@AddressCode1", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode2", SqlDbType.NVarChar,50),
					new SqlParameter("@AddressCode3", SqlDbType.Int,4),
					new SqlParameter("@OpenUserId", SqlDbType.BigInt,8),
					new SqlParameter("@OpenUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@OpenTime", SqlDbType.DateTime),
					new SqlParameter("@LastUpdateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LastUpdateUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LastUpdateAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LastUpdateTime", SqlDbType.DateTime),
					new SqlParameter("@Id", SqlDbType.BigInt,8)};
            parameters[0].Value = cardRead.SerialNumber;
            parameters[1].Value = cardRead.AddressCode1;
            parameters[2].Value = cardRead.AddressCode2;
            parameters[3].Value = cardRead.AddressCode3;
            parameters[4].Value = cardRead.OpenUserId;
            parameters[5].Value = cardRead.OpenUserName;
            parameters[6].Value = cardRead.OpenAddress;
            parameters[7].Value = cardRead.OpenTime;
            parameters[8].Value = cardRead.LastUpdateUserId;
            parameters[9].Value = cardRead.LastUpdateUserName;
            parameters[10].Value = cardRead.LastUpdateAddress;
            parameters[11].Value = cardRead.LastUpdateTime;
            parameters[12].Value = cardRead.Id;

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

        public static string DeleteCardRead(long Id)
        {
            string strSql = "delete CardRead where Id=@Id";
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
