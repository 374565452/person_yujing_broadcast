using Common;
using DBUtility;
using Maticsoft.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class CardClearModule
    {
        /// <summary>
        /// 是否存在SerialNumber
        /// </summary>
        public static bool ExistsSerialNumber(string SerialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from CardClear");
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
            strSql.Append("select count(1) from CardClear");
            strSql.Append(" where SerialNumber=@SerialNumber and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = SerialNumber;
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        public static CardClear GetCardClearBySerialNumber(string SerialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 Id,SerialNumber,OpenUserId,OpenUserName,OpenAddress,OpenTime from CardClear ");
            strSql.Append(" where SerialNumber=@SerialNumber");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar)
			};
            parameters[0].Value = SerialNumber;

            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                ModelHandler<CardClear> modelHandler = new ModelHandler<CardClear>();
                return modelHandler.FillModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public static long AddCardClear(CardClear cardClear)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardClear(");
            strSql.Append("SerialNumber,OpenUserId,OpenUserName,OpenAddress,OpenTime)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@OpenUserId,@OpenUserName,@OpenAddress,@OpenTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenUserId", SqlDbType.BigInt,8),
					new SqlParameter("@OpenUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@OpenTime", SqlDbType.DateTime)};
            parameters[0].Value = cardClear.SerialNumber;
            parameters[1].Value = cardClear.OpenUserId;
            parameters[2].Value = cardClear.OpenUserName;
            parameters[3].Value = cardClear.OpenAddress;
            parameters[4].Value = cardClear.OpenTime;

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

        public static string DeleteCardClear(long Id)
        {
            string strSql = "delete CardClear where Id=@Id";
            SqlParameter[] cmdParms = new SqlParameter[]{ 
                new SqlParameter("@Id", SqlDbType.BigInt)
            };
            cmdParms[0].Value = Id;
            try
            {
                int rows = DbHelperSQL.ExecuteSql(strSql, cmdParms);
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
