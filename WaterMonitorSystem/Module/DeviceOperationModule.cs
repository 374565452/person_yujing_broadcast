using DBUtility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class DeviceOperationModule
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static long AddDeviceOperation(Maticsoft.Model.DeviceOperation model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into DeviceOperation(");
            strSql.Append("DeviceNo,DeviceName,OperationType,OperationTime,UserId,UserName,RawData,Remark,State)");
            strSql.Append(" values (");
            strSql.Append("@DeviceNo,@DeviceName,@OperationType,@OperationTime,@UserId,@UserName,@RawData,@Remark,@State)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar,50),
                    new SqlParameter("@DeviceName", SqlDbType.NVarChar,50),
					new SqlParameter("@OperationType", SqlDbType.NVarChar,50),
					new SqlParameter("@OperationTime", SqlDbType.DateTime),
					new SqlParameter("@UserId", SqlDbType.BigInt,8),
					new SqlParameter("@UserName", SqlDbType.NVarChar,50),
					new SqlParameter("@RawData", SqlDbType.NVarChar,-1),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@State", SqlDbType.NVarChar,50)};
            parameters[0].Value = model.DeviceNo;
            parameters[1].Value = model.DeviceName;
            parameters[2].Value = model.OperationType;
            parameters[3].Value = model.OperationTime;
            parameters[4].Value = model.UserId;
            parameters[5].Value = model.UserName;
            parameters[6].Value = model.RawData;
            parameters[7].Value = model.Remark;
            parameters[8].Value = model.State;

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
