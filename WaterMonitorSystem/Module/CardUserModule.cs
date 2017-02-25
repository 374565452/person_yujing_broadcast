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
    public class CardUserModule
    {
        static log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<long, CardUser> dicCardUsersById = new Dictionary<long, CardUser>();
        private static string cardUserLockFlag = "";

        /// <summary>
        /// 是否存在SerialNumber
        /// </summary>
        public static bool ExistsSerialNumber(string SerialNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from CardUser");
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
            strSql.Append("select count(1) from CardUser");
            strSql.Append(" where SerialNumber=@SerialNumber and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = SerialNumber;
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在UserNo
        /// </summary>
        public static bool ExistsUserNo(string UserNo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from CardUser");
            strSql.Append(" where UserNo=@UserNo");
            SqlParameter[] parameters = {
					new SqlParameter("@UserNo", SqlDbType.NVarChar)
			};
            parameters[0].Value = UserNo.Trim().TrimStart('0');

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在UserNo
        /// </summary>
        public static bool ExistsUserNo(string UserNo, long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from CardUser");
            strSql.Append(" where UserNo=@UserNo and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@UserNo", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = UserNo.Trim().TrimStart('0');
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        public static List<CardUser> GetAllCardUser(bool IsCountermand)
        {
            List<CardUser> list = new List<CardUser>();
            lock (dicCardUsersById)
            {
                foreach (KeyValuePair<long, CardUser> pair in dicCardUsersById)
                {
                    if (!IsCountermand && pair.Value.IsCountermand == 0 || IsCountermand)
                        list.Add(Tools.Copy<CardUser>(pair.Value));
                }
            }
            return list;
        }

        public static long AddCardUser(CardUser cardUser)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into CardUser(");
            strSql.Append("SerialNumber,UserNo,WaterUserId,ResidualWater,ResidualElectric,TotalWater,TotalElectric,ResidualMoney,TotallMoney,DeviceList,Remark,OpenUserId,OpenUserName,OpenAddress,OpenTime,LastConsumptionDeviceId,LastConsumptionDeviceNo,LastConsumptionTime,LastChargeUserId,LastChargeUserName,LastChargeAddress,LastChargeTime,LastUpdateUserId,LastUpdateUserName,LastUpdateAddress,LastUpdateTime,IsCountermand,CountermandContent,CountermandUserId,CountermandUserName,CountermandAddress,CountermandTime,CountermandCancelContent,CountermandCancelUserId,CountermandCancelUserName,CountermandCancelAddress,CountermandCancelTime)");
            strSql.Append(" values (");
            strSql.Append("@SerialNumber,@UserNo,@WaterUserId,@ResidualWater,@ResidualElectric,@TotalWater,@TotalElectric,@ResidualMoney,@TotallMoney,@DeviceList,@Remark,@OpenUserId,@OpenUserName,@OpenAddress,@OpenTime,@LastConsumptionDeviceId,@LastConsumptionDeviceNo,@LastConsumptionTime,@LastChargeUserId,@LastChargeUserName,@LastChargeAddress,@LastChargeTime,@LastUpdateUserId,@LastUpdateUserName,@LastUpdateAddress,@LastUpdateTime,@IsCountermand,@CountermandContent,@CountermandUserId,@CountermandUserName,@CountermandAddress,@CountermandTime,@CountermandCancelContent,@CountermandCancelUserId,@CountermandCancelUserName,@CountermandCancelAddress,@CountermandCancelTime)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@UserNo", SqlDbType.NVarChar,50),
					new SqlParameter("@WaterUserId", SqlDbType.BigInt,8),
					new SqlParameter("@ResidualWater", SqlDbType.Decimal,9),
					new SqlParameter("@ResidualElectric", SqlDbType.Decimal,9),
					new SqlParameter("@TotalWater", SqlDbType.Decimal,9),
					new SqlParameter("@TotalElectric", SqlDbType.Decimal,9),
					new SqlParameter("@ResidualMoney", SqlDbType.Decimal,9),
					new SqlParameter("@TotallMoney", SqlDbType.Decimal,9),
					new SqlParameter("@DeviceList", SqlDbType.NVarChar,-1),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@OpenUserId", SqlDbType.BigInt,8),
					new SqlParameter("@OpenUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@OpenTime", SqlDbType.DateTime),
					new SqlParameter("@LastConsumptionDeviceId", SqlDbType.BigInt,8),
					new SqlParameter("@LastConsumptionDeviceNo", SqlDbType.NVarChar,50),
					new SqlParameter("@LastConsumptionTime", SqlDbType.DateTime),
					new SqlParameter("@LastChargeUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LastChargeUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LastChargeAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LastChargeTime", SqlDbType.DateTime),
					new SqlParameter("@LastUpdateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LastUpdateUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LastUpdateAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LastUpdateTime", SqlDbType.DateTime),
					new SqlParameter("@IsCountermand", SqlDbType.Int,4),
					new SqlParameter("@CountermandContent", SqlDbType.NVarChar,-1),
					new SqlParameter("@CountermandUserId", SqlDbType.BigInt,8),
					new SqlParameter("@CountermandUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@CountermandAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@CountermandTime", SqlDbType.DateTime),
					new SqlParameter("@CountermandCancelContent", SqlDbType.NVarChar,-1),
					new SqlParameter("@CountermandCancelUserId", SqlDbType.BigInt,8),
					new SqlParameter("@CountermandCancelUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@CountermandCancelAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@CountermandCancelTime", SqlDbType.DateTime)};
            parameters[0].Value = cardUser.SerialNumber;
            parameters[1].Value = cardUser.UserNo.Trim().TrimStart('0');
            parameters[2].Value = cardUser.WaterUserId;
            parameters[3].Value = cardUser.ResidualWater;
            parameters[4].Value = cardUser.ResidualElectric;
            parameters[5].Value = cardUser.TotalWater;
            parameters[6].Value = cardUser.TotalElectric;
            parameters[7].Value = cardUser.ResidualMoney;
            parameters[8].Value = cardUser.TotallMoney;
            parameters[9].Value = cardUser.DeviceList;
            parameters[10].Value = cardUser.Remark;
            parameters[11].Value = cardUser.OpenUserId;
            parameters[12].Value = cardUser.OpenUserName;
            parameters[13].Value = cardUser.OpenAddress;
            parameters[14].Value = cardUser.OpenTime;
            parameters[15].Value = cardUser.LastConsumptionDeviceId;
            parameters[16].Value = cardUser.LastConsumptionDeviceNo;
            parameters[17].Value = cardUser.LastConsumptionTime;
            parameters[18].Value = cardUser.LastChargeUserId;
            parameters[19].Value = cardUser.LastChargeUserName;
            parameters[20].Value = cardUser.LastChargeAddress;
            parameters[21].Value = cardUser.LastChargeTime;
            parameters[22].Value = cardUser.LastUpdateUserId;
            parameters[23].Value = cardUser.LastUpdateUserName;
            parameters[24].Value = cardUser.LastUpdateAddress;
            parameters[25].Value = cardUser.LastUpdateTime;
            parameters[26].Value = cardUser.IsCountermand;
            parameters[27].Value = cardUser.CountermandContent;
            parameters[28].Value = cardUser.CountermandUserId;
            parameters[29].Value = cardUser.CountermandUserName;
            parameters[30].Value = cardUser.CountermandAddress;
            parameters[31].Value = cardUser.CountermandTime;
            parameters[32].Value = cardUser.CountermandCancelContent;
            parameters[33].Value = cardUser.CountermandCancelUserId;
            parameters[34].Value = cardUser.CountermandCancelUserName;
            parameters[35].Value = cardUser.CountermandCancelAddress;
            parameters[36].Value = cardUser.CountermandCancelTime;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                cardUser.Id = Convert.ToInt64(obj);
                UpdateCardUserInfo(cardUser);
                return Convert.ToInt64(obj);
            }
        }

        public static string ModifyCardUser(CardUser cardUser)
        {
            lock (dicCardUsersById)
            {
                if (!dicCardUsersById.ContainsKey(cardUser.Id))
                {
                    return "修改失败，原因：不存在此用户卡！";
                }
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update CardUser set ");
            strSql.Append("SerialNumber=@SerialNumber,");
            strSql.Append("UserNo=@UserNo,");
            strSql.Append("WaterUserId=@WaterUserId,");
            strSql.Append("ResidualWater=@ResidualWater,");
            strSql.Append("ResidualElectric=@ResidualElectric,");
            strSql.Append("TotalWater=@TotalWater,");
            strSql.Append("TotalElectric=@TotalElectric,");
            strSql.Append("ResidualMoney=@ResidualMoney,");
            strSql.Append("TotallMoney=@TotallMoney,");
            strSql.Append("DeviceList=@DeviceList,");
            strSql.Append("Remark=@Remark,");
            strSql.Append("OpenUserId=@OpenUserId,");
            strSql.Append("OpenUserName=@OpenUserName,");
            strSql.Append("OpenAddress=@OpenAddress,");
            strSql.Append("OpenTime=@OpenTime,");
            strSql.Append("LastConsumptionDeviceId=@LastConsumptionDeviceId,");
            strSql.Append("LastConsumptionDeviceNo=@LastConsumptionDeviceNo,");
            strSql.Append("LastConsumptionTime=@LastConsumptionTime,");
            strSql.Append("LastChargeUserId=@LastChargeUserId,");
            strSql.Append("LastChargeUserName=@LastChargeUserName,");
            strSql.Append("LastChargeAddress=@LastChargeAddress,");
            strSql.Append("LastChargeTime=@LastChargeTime,");
            strSql.Append("LastUpdateUserId=@LastUpdateUserId,");
            strSql.Append("LastUpdateUserName=@LastUpdateUserName,");
            strSql.Append("LastUpdateAddress=@LastUpdateAddress,");
            strSql.Append("LastUpdateTime=@LastUpdateTime,");
            strSql.Append("IsCountermand=@IsCountermand,");
            strSql.Append("CountermandContent=@CountermandContent,");
            strSql.Append("CountermandUserId=@CountermandUserId,");
            strSql.Append("CountermandUserName=@CountermandUserName,");
            strSql.Append("CountermandAddress=@CountermandAddress,");
            strSql.Append("CountermandTime=@CountermandTime,");
            strSql.Append("CountermandCancelContent=@CountermandCancelContent,");
            strSql.Append("CountermandCancelUserId=@CountermandCancelUserId,");
            strSql.Append("CountermandCancelUserName=@CountermandCancelUserName,");
            strSql.Append("CountermandCancelAddress=@CountermandCancelAddress,");
            strSql.Append("CountermandCancelTime=@CountermandCancelTime");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@SerialNumber", SqlDbType.NVarChar,50),
					new SqlParameter("@UserNo", SqlDbType.NVarChar,50),
					new SqlParameter("@WaterUserId", SqlDbType.BigInt,8),
					new SqlParameter("@ResidualWater", SqlDbType.Decimal,9),
					new SqlParameter("@ResidualElectric", SqlDbType.Decimal,9),
					new SqlParameter("@TotalWater", SqlDbType.Decimal,9),
					new SqlParameter("@TotalElectric", SqlDbType.Decimal,9),
					new SqlParameter("@ResidualMoney", SqlDbType.Decimal,9),
					new SqlParameter("@TotallMoney", SqlDbType.Decimal,9),
					new SqlParameter("@DeviceList", SqlDbType.NVarChar,-1),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@OpenUserId", SqlDbType.BigInt,8),
					new SqlParameter("@OpenUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@OpenAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@OpenTime", SqlDbType.DateTime),
					new SqlParameter("@LastConsumptionDeviceId", SqlDbType.BigInt,8),
					new SqlParameter("@LastConsumptionDeviceNo", SqlDbType.NVarChar,50),
					new SqlParameter("@LastConsumptionTime", SqlDbType.DateTime),
					new SqlParameter("@LastChargeUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LastChargeUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LastChargeAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LastChargeTime", SqlDbType.DateTime),
					new SqlParameter("@LastUpdateUserId", SqlDbType.BigInt,8),
					new SqlParameter("@LastUpdateUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@LastUpdateAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LastUpdateTime", SqlDbType.DateTime),
					new SqlParameter("@IsCountermand", SqlDbType.Int,4),
					new SqlParameter("@CountermandContent", SqlDbType.NVarChar,-1),
					new SqlParameter("@CountermandUserId", SqlDbType.BigInt,8),
					new SqlParameter("@CountermandUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@CountermandAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@CountermandTime", SqlDbType.DateTime),
					new SqlParameter("@CountermandCancelContent", SqlDbType.NVarChar,-1),
					new SqlParameter("@CountermandCancelUserId", SqlDbType.BigInt,8),
					new SqlParameter("@CountermandCancelUserName", SqlDbType.NVarChar,50),
					new SqlParameter("@CountermandCancelAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@CountermandCancelTime", SqlDbType.DateTime),
					new SqlParameter("@Id", SqlDbType.BigInt,8)};
            parameters[0].Value = cardUser.SerialNumber;
            parameters[1].Value = cardUser.UserNo.Trim().TrimStart('0');
            parameters[2].Value = cardUser.WaterUserId;
            parameters[3].Value = cardUser.ResidualWater;
            parameters[4].Value = cardUser.ResidualElectric;
            parameters[5].Value = cardUser.TotalWater;
            parameters[6].Value = cardUser.TotalElectric;
            parameters[7].Value = cardUser.ResidualMoney;
            parameters[8].Value = cardUser.TotallMoney;
            parameters[9].Value = cardUser.DeviceList;
            parameters[10].Value = cardUser.Remark;
            parameters[11].Value = cardUser.OpenUserId;
            parameters[12].Value = cardUser.OpenUserName;
            parameters[13].Value = cardUser.OpenAddress;
            parameters[14].Value = cardUser.OpenTime;
            parameters[15].Value = cardUser.LastConsumptionDeviceId;
            parameters[16].Value = cardUser.LastConsumptionDeviceNo;
            parameters[17].Value = cardUser.LastConsumptionTime;
            parameters[18].Value = cardUser.LastChargeUserId;
            parameters[19].Value = cardUser.LastChargeUserName;
            parameters[20].Value = cardUser.LastChargeAddress;
            parameters[21].Value = cardUser.LastChargeTime;
            parameters[22].Value = cardUser.LastUpdateUserId;
            parameters[23].Value = cardUser.LastUpdateUserName;
            parameters[24].Value = cardUser.LastUpdateAddress;
            parameters[25].Value = cardUser.LastUpdateTime;
            parameters[26].Value = cardUser.IsCountermand;
            parameters[27].Value = cardUser.CountermandContent;
            parameters[28].Value = cardUser.CountermandUserId;
            parameters[29].Value = cardUser.CountermandUserName;
            parameters[30].Value = cardUser.CountermandAddress;
            parameters[31].Value = cardUser.CountermandTime;
            parameters[32].Value = cardUser.CountermandCancelContent;
            parameters[33].Value = cardUser.CountermandCancelUserId;
            parameters[34].Value = cardUser.CountermandCancelUserName;
            parameters[35].Value = cardUser.CountermandCancelAddress;
            parameters[36].Value = cardUser.CountermandCancelTime;
            parameters[37].Value = cardUser.Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                UpdateCardUserInfo(cardUser);
                return "修改成功";
            }
            else
            {
                return "修改失败，原因：写入数据库失败！";
            }
        }

        public static string DeleteCardUser(long Id)
        {
            lock (dicCardUsersById)
            {
                if (!dicCardUsersById.ContainsKey(Id))
                {
                    return "删除失败：原因：该用户卡不存在！";
                }
            }

            string strSql = "delete CardUser where Id=@Id";
            SqlParameter[] cmdParms = new SqlParameter[]{ 
                new SqlParameter("@Id", SqlDbType.BigInt)
            };
            cmdParms[0].Value = Id;
            try
            {
                int rows = DbHelperSQL.ExecuteSql(strSql, cmdParms);
                if (rows > 0)
                {
                    RemoveCardUserInfo(Id);
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

        public static List<CardUser> GetCardUsersByDistrictId(long DistrictId, bool IsCountermand)
        {
            List<CardUser> list = new List<CardUser>();
            lock (dicCardUsersById)
            {
                foreach (KeyValuePair<long, CardUser> pair in dicCardUsersById)
                {
                    WaterUser wu = WaterUserModule.GetWaterUserById(pair.Value.WaterUserId);
                    if (wu.DistrictId == DistrictId)
                    {
                        if (!IsCountermand && pair.Value.IsCountermand == 0 || IsCountermand)
                            list.Add(Tools.Copy<CardUser>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static List<CardUser> GetCardUsersByWaterUserId(long waterUserId, bool IsCountermand)
        {
            List<CardUser> list = new List<CardUser>();
            lock (dicCardUsersById)
            {
                foreach (KeyValuePair<long, CardUser> pair in dicCardUsersById)
                {
                    if (pair.Value.WaterUserId == waterUserId)
                        if (!IsCountermand && pair.Value.IsCountermand == 0 || IsCountermand)
                            list.Add(Tools.Copy<CardUser>(pair.Value));
                }
            }
            return list;
        }

        public static void LoadCardUsers()
        {
            string strSql = "select * from CardUser";
            try
            {
                DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                if (table.Rows.Count != 0)
                {
                    ModelHandler<CardUser> modelHandler = new ModelHandler<CardUser>();
                    lock (cardUserLockFlag)
                    {
                        dicCardUsersById.Clear();
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            DataRow dataRow = table.Rows[i];
                            CardUser cu = modelHandler.FillModel(dataRow);

                            if (!dicCardUsersById.ContainsKey(cu.Id))
                            {
                                dicCardUsersById.Add(cu.Id, cu);
                            }
                            else
                            {
                                dicCardUsersById[cu.Id] = cu;
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public static CardUser GetCardUserBySerialNumber(string SerialNumber)
        {
            lock (cardUserLockFlag)
            {
                foreach (KeyValuePair<long, CardUser> pair in dicCardUsersById)
                {
                    if (pair.Value.SerialNumber.ToLower() == SerialNumber.ToLower())
                        return Tools.Copy<CardUser>(pair.Value);
                }
                return null;
            }
        }

        public static CardUser GetCardUserBySerialNumber_DB(string SerialNumber)
        {
            string strSql = "select * from CardUser where SerialNumber='" + SerialNumber + "'";
            try
            {
                DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                if (table.Rows.Count != 0)
                {
                    ModelHandler<CardUser> modelHandler = new ModelHandler<CardUser>();
                    lock (cardUserLockFlag)
                    {
                        DataRow dataRow = table.Rows[0];
                        CardUser cu = modelHandler.FillModel(dataRow);

                        if (!dicCardUsersById.ContainsKey(cu.Id))
                        {
                            dicCardUsersById.Add(cu.Id, cu);
                        }
                        else
                        {
                            dicCardUsersById[cu.Id] = cu;
                        }

                        return cu;
                    }
                }
            }
            catch { }
            return null;
        }

        public static CardUser GetCardUserByUserNo(string UserNo)
        {
            lock (cardUserLockFlag)
            {
                foreach (KeyValuePair<long, CardUser> pair in dicCardUsersById)
                {
                    if (pair.Value.UserNo == UserNo.Trim().TrimStart('0'))
                        return Tools.Copy<CardUser>(pair.Value);
                }
                return null;
            }
        }

        public static CardUser GetCardUserByUserNo_DB(string UserNo)
        {
            string strSql = "select * from CardUser where UserNo='" + UserNo.Trim().TrimStart('0') + "'";
            try
            {
                DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                if (table.Rows.Count != 0)
                {
                    ModelHandler<CardUser> modelHandler = new ModelHandler<CardUser>();
                    lock (cardUserLockFlag)
                    {
                        DataRow dataRow = table.Rows[0];
                        CardUser cu = modelHandler.FillModel(dataRow);

                        if (!dicCardUsersById.ContainsKey(cu.Id))
                        {
                            dicCardUsersById.Add(cu.Id, cu);
                        }
                        else
                        {
                            dicCardUsersById[cu.Id] = cu;
                        }

                        return cu;
                    }
                }
            }
            catch { }
            return null;
        }

        public static CardUser GetCardUserById(long cardUserId)
        {
            lock (cardUserLockFlag)
            {
                if (dicCardUsersById.ContainsKey(cardUserId))
                {
                    return Tools.Copy<CardUser>(dicCardUsersById[cardUserId]);
                }
                return null;
            }
        }

        public static CardUser GetCardUserById_DB(long cardUserId)
        {
            string strSql = "select * from CardUser where cardUserId=" + cardUserId + "";
            try
            {
                DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                if (table.Rows.Count != 0)
                {
                    ModelHandler<CardUser> modelHandler = new ModelHandler<CardUser>();
                    lock (cardUserLockFlag)
                    {
                        DataRow dataRow = table.Rows[0];
                        CardUser cu = modelHandler.FillModel(dataRow);

                        if (!dicCardUsersById.ContainsKey(cu.Id))
                        {
                            dicCardUsersById.Add(cu.Id, cu);
                        }
                        else
                        {
                            dicCardUsersById[cu.Id] = cu;
                        }

                        return cu;
                    }
                }
            }
            catch { }
            return null;
        }

        public static void UpdateCardUserInfo(CardUser cardUser)
        {
            lock (cardUserLockFlag)
            {
                if (!dicCardUsersById.ContainsKey(cardUser.Id))
                {
                    dicCardUsersById.Add(cardUser.Id, cardUser);
                }
                else
                {
                    dicCardUsersById[cardUser.Id] = cardUser;
                }
            }
        }

        public static void RemoveCardUserInfo(long Id)
        {
            lock (dicCardUsersById)
            {
                if (dicCardUsersById.ContainsKey(Id))
                {
                    dicCardUsersById.Remove(Id);
                }
            }
        }

        public static void RemoveCardUserInfo(CardUser cardUser)
        {
            RemoveCardUserInfo(cardUser.Id);
        }

        public static void RemoveCardUserInfo(string SerialNumber)
        {
            CardUser cardUser = GetCardUserBySerialNumber(SerialNumber);
            if (cardUser != null)
            {
                RemoveCardUserInfo(cardUser.Id);
            }
        }

        public static JavaScriptObject CardUserToJson(CardUser cardUser)
        {
            JavaScriptObject obj2 = new JavaScriptObject();

            obj2.Add("ID", cardUser.Id);
            obj2.Add("WaterUserId", cardUser.WaterUserId);
            obj2.Add("SerialNumber", cardUser.SerialNumber);
            obj2.Add("UserNo", cardUser.UserNo.Trim().TrimStart('0'));
            obj2.Add("ResidualWater", cardUser.ResidualWater);
            obj2.Add("ResidualElectric", cardUser.ResidualElectric);
            obj2.Add("TotalWater", cardUser.TotalWater);
            obj2.Add("TotalElectric", cardUser.TotalElectric);
            obj2.Add("TotallMoney", cardUser.TotallMoney);
            obj2.Add("DeviceList", cardUser.DeviceList);
            obj2.Add("IsCountermand", cardUser.IsCountermand == 0 ? "否" : "是");
            obj2.Add("OpenTime", cardUser.OpenTime.ToString("yyyy-MM-dd HH:mm:ss"));
            obj2.Add("LastChargeTime", cardUser.LastChargeTime > DateTime.Parse("2010-1-1") ? cardUser.LastChargeTime.ToString("yyyy-MM-dd HH:mm:ss") : "");

            WaterUser waterUser = WaterUserModule.GetWaterUserById(cardUser.WaterUserId);
            if (waterUser != null)
            {
                obj2.Add("UserName", waterUser.UserName);
                obj2.Add("IdentityNumber", waterUser.IdentityNumber);
                obj2.Add("Telephone", waterUser.Telephone);
                District node = DistrictModule.ReturnDistrictInfo(waterUser.DistrictId);
                District node2 = DistrictModule.ReturnDistrictInfo(node.ParentId);
                District node3 = DistrictModule.ReturnDistrictInfo(node2.ParentId);
                District node4 = DistrictModule.ReturnDistrictInfo(node3.ParentId);
                District node5 = DistrictModule.ReturnDistrictInfo(node4.ParentId);
                obj2.Add("省ID", node5.Id);
                obj2.Add("市ID", node4.Id);
                obj2.Add("区县ID", node3.Id);
                obj2.Add("乡镇ID", node2.Id);
                obj2.Add("村庄ID", node.Id);
            }
            else
            {
                obj2.Add("UserName", "未知");
                obj2.Add("IdentityNumber", "未知");
                obj2.Add("Telephone", "未知");
                obj2.Add("省ID", "未知");
                obj2.Add("市ID", "未知");
                obj2.Add("区县ID", "未知");
                obj2.Add("乡镇ID", "未知");
                obj2.Add("村庄ID", "未知");
            }

            return obj2;
        }
    }
}
