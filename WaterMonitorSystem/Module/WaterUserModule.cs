using Common;
using DBUtility;
using Maticsoft.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module
{
    public class WaterUserModule
    {
        private static Dictionary<long, WaterUser> dicWaterUsersById = new Dictionary<long, WaterUser>();
        private static string waterUserLockFlag = "";

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public static bool Exists(long id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from WaterUser");
            strSql.Append(" where id=@id");
            SqlParameter[] parameters = {
					new SqlParameter("@id", SqlDbType.BigInt)
			};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在相同UserName记录
        /// </summary>
        public static bool ExistsUserName(long DistrictId, string UserName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from WaterUser");
            strSql.Append(" where DistrictId=@DistrictId and UserName=@UserName");
            SqlParameter[] parameters = {
					new SqlParameter("@DistrictId", SqlDbType.BigInt),
                    new SqlParameter("@UserName", SqlDbType.NVarChar)
			};
            parameters[0].Value = DistrictId;
            parameters[1].Value = UserName;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在相同UserName记录
        /// </summary>
        public static bool ExistsUserName(long DistrictId, string UserName, long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from WaterUser");
            strSql.Append(" where DistrictId=@DistrictId and UserName=@UserName and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@DistrictId", SqlDbType.BigInt),
                    new SqlParameter("@UserName", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = DistrictId;
            parameters[1].Value = UserName;
            parameters[2].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在相同IdentityNumber记录
        /// </summary>
        public static bool ExistsIdentityNumber(string IdentityNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from WaterUser");
            strSql.Append(" where IdentityNumber=@IdentityNumber");
            SqlParameter[] parameters = {
                    new SqlParameter("@IdentityNumber", SqlDbType.NVarChar)
			};
            parameters[0].Value = IdentityNumber;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在相同IdentityNumber记录
        /// </summary>
        public static bool ExistsIdentityNumber(string IdentityNumber, long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from WaterUser");
            strSql.Append(" where IdentityNumber=@IdentityNumber and Id<>@Id");
            SqlParameter[] parameters = {
                    new SqlParameter("@IdentityNumber", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = IdentityNumber;
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在相同Telephone记录
        /// </summary>
        public static bool ExistsTelephone(string Telephone)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from WaterUser");
            strSql.Append(" where Telephone=@Telephone");
            SqlParameter[] parameters = {
                    new SqlParameter("@Telephone", SqlDbType.NVarChar)
			};
            parameters[0].Value = Telephone;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在相同Telephone记录
        /// </summary>
        public static bool ExistsTelephone(string Telephone, long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from WaterUser");
            strSql.Append(" where Telephone=@Telephone and Id<>@Id");
            SqlParameter[] parameters = {
                    new SqlParameter("@Telephone", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = Telephone;
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        public static ResMsg AddWaterUser(WaterUser wui)
        {
            if (wui == null)
            {
                return new ResMsg(false, "用水户对象为空");
            }
            if (ExistsIdentityNumber(wui.IdentityNumber))
            {
                return new ResMsg(false, "用水户身份证号重复");
            }
            if (ExistsTelephone(wui.Telephone))
            {
                return new ResMsg(false, "用水户电话号码重复");
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into WaterUser(");
            strSql.Append("UserName,Password,DistrictId,TrueName,IdentityNumber,Telephone,Address,WaterQuota,ElectricQuota,Remark,水价ID,电价ID,State)");
            strSql.Append(" values (");
            strSql.Append("@UserName,@Password,@DistrictId,@TrueName,@IdentityNumber,@Telephone,@Address,@WaterQuota,@ElectricQuota,@Remark,@水价ID,@电价ID,@State)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
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
            parameters[0].Value = wui.UserName;
            parameters[1].Value = wui.Password;
            parameters[2].Value = wui.DistrictId;
            parameters[3].Value = wui.TrueName;
            parameters[4].Value = wui.IdentityNumber;
            parameters[5].Value = wui.Telephone;
            parameters[6].Value = wui.Address;
            parameters[7].Value = wui.WaterQuota;
            parameters[8].Value = wui.ElectricQuota;
            parameters[9].Value = wui.Remark;
            parameters[10].Value = wui.水价ID;
            parameters[11].Value = wui.电价ID;
            parameters[12].Value = wui.State;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return new ResMsg(false, "添加失败");
            }
            else
            {
                wui.id = Convert.ToInt64(obj);
                if (!dicWaterUsersById.ContainsKey(wui.id))
                    dicWaterUsersById.Add(wui.id, wui);
                return new ResMsg(true, Convert.ToInt64(obj).ToString());
            }
        }

        public static ResMsg ModifyWaterUser(WaterUser wui)
        {
            if (wui == null)
            {
                return new ResMsg(false, "用水户对象为空");
            }
            if (!Exists(wui.id))
            {
                return new ResMsg(false, "用水户不存在");
            }
            if (ExistsIdentityNumber(wui.IdentityNumber, wui.id))
            {
                return new ResMsg(false, "用水户身份证号重复");
            }
            if (ExistsTelephone(wui.Telephone, wui.id))
            {
                return new ResMsg(false, "用水户电话号码重复");
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update WaterUser set ");
            strSql.Append("UserName=@UserName,");
            strSql.Append("Password=@Password,");
            strSql.Append("DistrictId=@DistrictId,");
            strSql.Append("TrueName=@TrueName,");
            strSql.Append("IdentityNumber=@IdentityNumber,");
            strSql.Append("Telephone=@Telephone,");
            strSql.Append("Address=@Address,");
            strSql.Append("WaterQuota=@WaterQuota,");
            strSql.Append("ElectricQuota=@ElectricQuota,");
            strSql.Append("Remark=@Remark,");
            strSql.Append("水价ID=@水价ID,");
            strSql.Append("电价ID=@电价ID,");
            strSql.Append("State=@State");
            strSql.Append(" where id=@id");
            SqlParameter[] parameters = {
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
					new SqlParameter("@State", SqlDbType.NVarChar,50),
					new SqlParameter("@id", SqlDbType.BigInt,8)};
            parameters[0].Value = wui.UserName;
            parameters[1].Value = wui.Password;
            parameters[2].Value = wui.DistrictId;
            parameters[3].Value = wui.TrueName;
            parameters[4].Value = wui.IdentityNumber;
            parameters[5].Value = wui.Telephone;
            parameters[6].Value = wui.Address;
            parameters[7].Value = wui.WaterQuota;
            parameters[8].Value = wui.ElectricQuota;
            parameters[9].Value = wui.Remark;
            parameters[10].Value = wui.水价ID;
            parameters[11].Value = wui.电价ID;
            parameters[12].Value = wui.State;
            parameters[13].Value = wui.id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                if (!dicWaterUsersById.ContainsKey(wui.id))
                    dicWaterUsersById.Add(wui.id, wui);
                else
                    dicWaterUsersById[wui.id] = wui;
                return new ResMsg(true, "修改成功");
            }
            else
            {
                return new ResMsg(false, "修改失败");
            }
        }

        public static ResMsg WriteOffWaterUserById(long waterUserId)
        {
            return WriteOffWaterUserById(waterUserId, true);
        }

        private static ResMsg WriteOffWaterUserById(long waterUserId, bool checkWaterUserExist)
        {
            try
            {
                WaterUser wui = GetWaterUserById(waterUserId);
                if (checkWaterUserExist && wui== null)
                {
                    return new ResMsg(false, "用水户不存在");
                }

                wui.State = "已注销";

                return ModifyWaterUser(wui);
            }
            catch {
                return new ResMsg(false, " 注销失败");
            }
        }

        public static List<WaterUser> GetAllWaterUser()
        {
            List<WaterUser> list = new List<WaterUser>();
            lock (dicWaterUsersById)
            {
                foreach (KeyValuePair<long, WaterUser> pair in dicWaterUsersById)
                {
                    list.Add(Tools.Copy<WaterUser>(pair.Value));
                }
            }
            return list;
        }

        public static List<WaterUser> GetWaterUsersByDistrictId(long DistrictId, bool isContainsInvalid)
        {
            List<WaterUser> list = new List<WaterUser>();
            lock (dicWaterUsersById)
            {
                foreach (KeyValuePair<long, WaterUser> pair in dicWaterUsersById)
                {
                    if (pair.Value.DistrictId == DistrictId)
                    {
                        if (!isContainsInvalid && pair.Value.State == "正常" || isContainsInvalid)
                            list.Add(Tools.Copy<WaterUser>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static void LoadWaterUsers()
        {
            string strSql = "select * from WaterUser";
            try
            {
                DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                if (table.Rows.Count != 0)
                {
                    ModelHandler<WaterUser> modelHandler = new ModelHandler<WaterUser>();
                    lock (waterUserLockFlag)
                    {
                        dicWaterUsersById.Clear();
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            DataRow dataRow = table.Rows[i];
                            WaterUser user = modelHandler.FillModel(dataRow);

                            if (!dicWaterUsersById.ContainsKey(user.id))
                            {
                                dicWaterUsersById.Add(user.id, user);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public static WaterUser GetWaterUserById(long waterUserId)
        {
            lock (waterUserLockFlag)
            {
                if (dicWaterUsersById.ContainsKey(waterUserId))
                {
                    return Tools.Copy<WaterUser>(dicWaterUsersById[waterUserId]);
                }
                return null;
            }
        }

        public static List<WaterUser> GetWaterUserByUserName(string UserName)
        {
            List<WaterUser> list = new List<WaterUser>();
            lock (dicWaterUsersById)
            {
                foreach (KeyValuePair<long, WaterUser> pair in dicWaterUsersById)
                {
                    if (pair.Value.UserName == UserName)
                    {
                        list.Add(Tools.Copy<WaterUser>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static List<WaterUser> GetWaterUserByIdentityNumber(string IdentityNumber)
        {
            List<WaterUser> list = new List<WaterUser>();
            lock (dicWaterUsersById)
            {
                foreach (KeyValuePair<long, WaterUser> pair in dicWaterUsersById)
                {
                    if (pair.Value.IdentityNumber == IdentityNumber)
                    {
                        list.Add(Tools.Copy<WaterUser>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static List<WaterUser> GetWaterUserByTelephone(string Telephone)
        {
            List<WaterUser> list = new List<WaterUser>();
            lock (dicWaterUsersById)
            {
                foreach (KeyValuePair<long, WaterUser> pair in dicWaterUsersById)
                {
                    if (pair.Value.Telephone == Telephone)
                    {
                        list.Add(Tools.Copy<WaterUser>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static List<WaterUser> GetWaterUserByUserNameLike(string UserName)
        {
            List<WaterUser> list = new List<WaterUser>();
            lock (dicWaterUsersById)
            {
                foreach (KeyValuePair<long, WaterUser> pair in dicWaterUsersById)
                {
                    if (pair.Value.UserName.Contains(UserName))
                    {
                        list.Add(Tools.Copy<WaterUser>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static List<WaterUser> GetWaterUserByIdentityNumberLike(string IdentityNumber)
        {
            List<WaterUser> list = new List<WaterUser>();
            lock (dicWaterUsersById)
            {
                foreach (KeyValuePair<long, WaterUser> pair in dicWaterUsersById)
                {
                    if (pair.Value.IdentityNumber.Contains(IdentityNumber))
                    {
                        list.Add(Tools.Copy<WaterUser>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static List<WaterUser> GetWaterUserByTelephoneLike(string Telephone)
        {
            List<WaterUser> list = new List<WaterUser>();
            lock (dicWaterUsersById)
            {
                foreach (KeyValuePair<long, WaterUser> pair in dicWaterUsersById)
                {
                    if (pair.Value.Telephone.Contains(Telephone))
                    {
                        list.Add(Tools.Copy<WaterUser>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static JavaScriptObject WaterUserToJson(WaterUser waterUser)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("ID", waterUser.id);
            obj2.Add("名称", waterUser.UserName);
            //obj2.Add("户号", waterUser.UserNum);
            obj2.Add("状态", waterUser.State);
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
            obj2.Add("水价ID", waterUser.水价ID);
            obj2.Add("电价ID", waterUser.电价ID);
            obj2.Add("电话", waterUser.Telephone);
            obj2.Add("身份证号", waterUser.IdentityNumber);
            obj2.Add("地址", waterUser.Address);
            obj2.Add("用水定额", waterUser.WaterQuota);
            obj2.Add("用电定额", waterUser.ElectricQuota);
            /*
            obj2.Add("累计购水金额", waterUser.TotalMoneyOfBuyWater);
            obj2.Add("累计购电金额", waterUser.TotalMoneyOfBuyPower);
            obj2.Add("累计购水量", waterUser.TotalAmountOfBuyWater);
            obj2.Add("累计购电量", waterUser.TotalAmountOfBuyPower);
            obj2.Add("累计用水量", waterUser.TotalAmountOfUseWater);
            obj2.Add("累计用电量", waterUser.TotalAmountOfUsePower);
            obj2.Add("年累计购水量", waterUser.YearAmountOfBuyWater);
            obj2.Add("年累计购电量", waterUser.YearAmountOfBuyPower);
            obj2.Add("最后用水时间", waterUser.LastUseWaterTime);
            obj2.Add("详细信息", waterUser.Details);
            string id = "";
            string str2 = "未分配";
            if (waterUser.WaterUserQuota != null)
            {
                str2 = waterUser.WaterUserQuota.Quota.ToString();
                id = waterUser.WaterUserQuota.Id;
            }
            obj2.Add("用水定额ID", id);
            obj2.Add("用水定额", str2);
            obj2.Add("卡序列号", waterUser.CardSerialNumber);
             * */
            return obj2;
        }

        public static ResMsg GetSaleWaterSummaryReportByWaterUser(string waterUserId, string startTime, string endTime, ref DataTable dtResult)
        {
            string strSql = "select sum(TotalPrice) as 应收金额, sum(WaterPrice) as 售水金额,sum(WaterNum) as 售出水量,sum(ElectricPrice) as 售电金额,sum(ElectricNum) as 售出电量 from CardUserRechargeLog where WateUserId='" + waterUserId + "' and LogTime>='" + startTime + "' and LogTime<'" + endTime + "'";
            try
            {
                //dtResult = DBManager.Query("select sum(应收金额) as 应收金额, sum(售水金额) as 售水金额,sum(售出水量) as 售出水量,sum(售电金额) as 售电金额,sum(售出电量) as 售出电量 from 售水记录 where 用水户ID='" + waterUserId + "' and 售水时间>='" + startTime + "' and 售水时间<'" + endTime + "'", out strMsg);
                dtResult = DbHelperSQL.Query(strSql).Tables[0];
            }
            catch (Exception ex)
            {
                return new ResMsg(false, ex.Message);
            }
            return new ResMsg(true, "");
        }

        public static ResMsg GetSaleWaterTimesSummaryReportByWaterUser(string waterUserId, string baseTime, bool isAsc, uint saleWaterTimes, ref DataTable dtResult)
        {
            string strSql = "select sum(TotalPrice) as 应收金额, sum(WaterPrice) as 售水金额,sum(WaterNum) as 售出水量,sum(ElectricPrice) as 售电金额,sum(ElectricNum) as 售出电量 from (select top " + saleWaterTimes.ToString() + " TotalPrice,WaterPrice,WaterNum,ElectricPrice,ElectricNum from CardUserRechargeLog where WateUserId='" + waterUserId + "' and LogTime" + (isAsc ? ">='" : "<='") + baseTime + "' order by LogTime " + (isAsc ? "ASC" : "DESC") + ") as sumtemp";
            try
            {
                //dtResult = DBManager.Query("select sum(应收金额) as 应收金额, sum(售水金额) as 售水金额,sum(售出水量) as 售出水量,sum(售电金额) as 售电金额,sum(售出电量) as 售出电量 from (select top " + saleWaterTimes.ToString() + " 应收金额,售水金额,售出水量,售电金额,售出电量 from 售水记录 where 用水户ID='" + waterUserId + "' and 售水时间" + (isAsc ? ">='" : "<='") + baseTime + "' order by 售水时间 " + (isAsc ? "ASC" : "DESC") + ") as sumtemp", out strMsg);
                dtResult = DbHelperSQL.Query(strSql).Tables[0];
            }
            catch (Exception ex)
            {
                return new ResMsg(false, ex.Message);
            }
            return new ResMsg(true, "");
        }

        public static ResMsg GetUseWaterSummaryReportByWaterUser(string waterUserId, string startTime, string endTime, ref DataTable dtResult)
        {
            string strSql = "select sum(Duration) as 灌溉时长, sum(WaterUsed) as 用水量,sum(ElectricUsed) as 用电量 from CardUserWaterLog where WateUserId='" + waterUserId + "' and EndTime>='" + startTime + "' and EndTime<'" + endTime + "'";
            try
            {
                //dtResult = DBManager.Query("select sum(灌溉时长) as 灌溉时长, sum(本次用水量) as 用水量,sum(本次用电量) as 用电量 from 用水记录表 where 用水户ID='" + waterUserId + "' and 关泵时间>='" + startTime + "' and 关泵时间<'" + endTime + "'", out strMsg);
                dtResult = DbHelperSQL.Query(strSql).Tables[0];
            }
            catch (Exception ex)
            {
                return new ResMsg(false, ex.Message);
            }
            return new ResMsg(true, "");
        }

        public static ResMsg GetUseWaterTimesSummaryReportByWaterUser(string waterUserId, string baseTime, bool isAsc, uint useWaterTimes, ref DataTable dtResult)
        {
            string strSql = "select sum(Duration) as 灌溉时长, sum(WaterUsed) as 用水量,sum(ElectricUsed) as 用电量 from (select top " + useWaterTimes.ToString() + " Duration,WaterUsed,ElectricUsed from CardUserWaterLog where WateUserId='" + waterUserId + "' and EndTime" + (isAsc ? ">='" : "<='") + baseTime + "' order by EndTime " + (isAsc ? "ASC" : "DESC") + ") as sumtemp";
            try
            {
                //dtResult = DBManager.Query("select sum(灌溉时长) as 灌溉时长, sum(本次用水量) as 用水量,sum(本次用电量) as 用电量 from (select top " + useWaterTimes.ToString() + " 灌溉时长,本次用水量,本次用电量 from 用水记录表 where 用水户ID='" + waterUserId + "' and 关泵时间" + (isAsc ? ">='" : "<='") + baseTime + "' order by 关泵时间 " + (isAsc ? "ASC" : "DESC") + ") as sumtemp", out strMsg);
                dtResult = DbHelperSQL.Query(strSql).Tables[0];
            }
            catch (Exception ex)
            {
                return new ResMsg(false, ex.Message);
            }
            return new ResMsg(true, "");
        }

        public static ResMsg GetSaleWaterSummaryReportByVillage(string villageId, string startTime, string endTime, ref DataTable dtResult)
        {
            List<string> liWaterUserIds = null;
            ResMsg msg = GetWaterUserIdsByManagerId(villageId, true, ref liWaterUserIds);
            if (!msg.Result)
            {
                return msg;
            }
            if (liWaterUserIds.Count == 0)
            {
                if (dtResult == null)
                {
                    dtResult = new DataTable();
                }
                else
                {
                    dtResult.Rows.Clear();
                    dtResult.Columns.Clear();
                }
                dtResult.Columns.Add("应收金额");
                dtResult.Columns.Add("售水金额");
                dtResult.Columns.Add("售出水量");
                dtResult.Columns.Add("售电金额");
                dtResult.Columns.Add("售出电量");
                dtResult.Rows.Add(new object[] { "0", "0", "0", "0", "0" });
                return new ResMsg(true, "");
            }
            bool flag = true;
            StringBuilder builder = new StringBuilder();
            foreach (string str2 in liWaterUserIds)
            {
                if (flag)
                {
                    builder.Append(str2);
                    flag = false;
                }
                else
                {
                    builder.Append("," + str2);
                }
            }
            if (builder.ToString() == "")
            {
                builder.Append("-1");
            }
            string strSql = "select sum(TotalPrice) as 应收金额, sum(WaterPrice) as 售水金额,sum(WaterNum) as 售出水量,sum(ElectricPrice) as 售电金额,sum(ElectricNum) as 售出电量 from CardUserRechargeLog where WateUserId in (" + builder.ToString() + ") and LogTime>='" + startTime + "' and LogTime<'" + endTime + "'";
            try
            {
                //dtResult = DBManager.Query("select sum(应收金额) as 应收金额, sum(售水金额) as 售水金额,sum(售出水量) as 售出水量,sum(售电金额) as 售电金额,sum(售出电量) as 售出电量 from 售水记录 where 用水户ID in (" + builder.ToString() + ") and 售水时间>='" + startTime + "' and 售水时间<'" + endTime + "'", out strMsg);
                dtResult = DbHelperSQL.Query(strSql).Tables[0];
            }
            catch (Exception ex)
            {
                return new ResMsg(false, ex.Message);
            }
            return new ResMsg(true, "");
        }

        public static ResMsg GetWaterUserIdsByManagerId(string mnID, bool isContainsInvalid, ref List<string> liWaterUserIds)
        {
            //DataTable table = DBManager.Query("select ID from 用水户 where 管理ID='" + mnID + "'" + (isContainsInvalid ? "" : " and 状态='正常'"), out strMsg);
            string strSql = "select id from WaterUser where DistrictId='" + mnID + "'" + (isContainsInvalid ? "" : " and State='正常'");
            try
            {
                DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                if (liWaterUserIds == null)
                {
                    liWaterUserIds = new List<string>();
                }
                else
                {
                    liWaterUserIds.Clear();
                }
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    liWaterUserIds.Add(table.Rows[i][0].ToString());
                }
                return new ResMsg(true, "");
            }
            catch (Exception ex)
            {
                return new ResMsg(false, ex.Message);
            }
        }

        public static ResMsg GetUseWaterSummaryReportByVillage(string villageId, string startTime, string endTime, ref DataTable dtResult)
        {
            List<string> liWaterUserIds = null;
            ResMsg msg = GetWaterUserIdsByManagerId(villageId, true, ref liWaterUserIds);
            if (!msg.Result)
            {
                return msg;
            }
            if (liWaterUserIds.Count == 0)
            {
                if (dtResult == null)
                {
                    dtResult = new DataTable();
                }
                else
                {
                    dtResult.Rows.Clear();
                    dtResult.Columns.Clear();
                }
                dtResult.Columns.Add("灌溉时长");
                dtResult.Columns.Add("用水量");
                dtResult.Columns.Add("用电量");
                dtResult.Rows.Add(new object[] { "0", "0", "0" });
                return new ResMsg(true, "");
            }
            bool flag = true;
            StringBuilder builder = new StringBuilder();
            foreach (string str2 in liWaterUserIds)
            {
                if (flag)
                {
                    builder.Append(str2);
                    flag = false;
                }
                else
                {
                    builder.Append("," + str2);
                }
            }
            if (builder.ToString() == "")
            {
                builder.Append("-1");
            }
            string strSql = "select sum(Duration) as 灌溉时长, sum(WaterUsed) as 用水量,sum(ElectricUsed) as 用电量 from CardUserWaterLog where WateUserId in (" + builder.ToString() + ") and EndTime>='" + startTime + "' and EndTime<'" + endTime + "'";
            try
            {
                //dtResult = DBManager.Query("select sum(灌溉时长) as 灌溉时长, sum(本次用水量) as 用水量,sum(本次用电量) as 用电量 from 用水记录表 where 用水户ID in (" + builder.ToString() + ") and 关泵时间>='" + startTime + "' and 关泵时间<'" + endTime + "'", out strMsg);
                dtResult = DbHelperSQL.Query(strSql).Tables[0];
            }
            catch (Exception ex)
            {
                return new ResMsg(false, ex.Message);
            }
            return new ResMsg(true, "");
        }
    }
}
