using CarlosAg.ExcelXmlWriter;
using Common;
using DBUtility;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// DataQueryService 的摘要说明
    /// </summary>
    [Serializable, ToolboxItem(false), WebService(Description = "历史记录查询、报警记录查询、操作记录查询、事件记录查询、拍照查询、登录记录查询、售水记录查询和用水记录查询", Name = "数据查询服务", Namespace = "http://www.data86.net/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class DataQueryService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public DataQueryService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定时间向前或向后多少次的售水记录数量</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserIds=用水户ID,多个用','隔开，baseTime=基准时间，isAsc=true:向后,false:向前，saleWaterTimes=次数<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetSaleWaterRecordsCountByTimes(string loginIdentifer, string waterUserIds, string baseTime, bool isAsc, uint saleWaterTimes)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Count", "");
            obj2.Add("Guid", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, false);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserIds == null) || (waterUserIds.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((baseTime == null) || (baseTime.Trim() == ""))
                {
                    obj2["Message"] = "基准时间不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                try
                {
                    Convert.ToDateTime(baseTime);
                }
                catch (Exception)
                {
                    obj2["Message"] = "基准时间格式不正确";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                string[] strArray = waterUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                PageInfo pageInfo = new PageInfo
                {
                    OperateIdentifer = Guid.NewGuid().ToString(""),
                    RecordType = isAsc ? "ASC" : "DESC",
                    QueryStartTime = baseTime,
                    QueryEndTime = saleWaterTimes.ToString()
                };
                for (int i = 0; i < strArray.Length; i++)
                {
                    /*
                    string strMsg = "";
                    DataTable table = DBManager.Query("select count(售水时间) from (select top " + saleWaterTimes.ToString() + " 售水时间 from 售水记录 where 用水户ID='" + strArray[i] + "' and 售水时间" + (isAsc ? ">='" : "<='") + baseTime + "' order by 售水时间 " + (isAsc ? "ASC" : "DESC") + ") as counttemp", out strMsg);
                    if (table == null)
                    {
                        obj2["Message"] = strMsg;
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                    if (!(table.Rows[0][0].ToString() == "0"))
                    {
                        pageInfo.DicDeviceRecordsCount.Add(strArray[i], Convert.ToInt32(table.Rows[0][0].ToString()));
                    }
                     * */
                    string sql = "select count(Id) from (select top " + saleWaterTimes.ToString() + " Id from CardUserRechargeLog where WateUserId='" + strArray[i] + "' and LogTime" + (isAsc ? ">='" : "<='") + baseTime + "' order by LogTime " + (isAsc ? "ASC" : "DESC") + ") as counttemp";
                    try
                    {
                        DataTable table = DbHelperSQL.Query(sql).Tables[0];
                        if (!(table.Rows[0][0].ToString() == "0"))
                        {
                            pageInfo.DicDeviceRecordsCount.Add(strArray[i], Convert.ToInt32(table.Rows[0][0].ToString()));
                        }
                    }
                    catch (Exception ex)
                    {
                        obj2["Message"] = ex.Message;
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                }
                GlobalAppModule.AddPageInfo(pageInfo);
                obj2["Result"] = true;
                obj2["Count"] = pageInfo.RecordsCount.ToString();
                obj2["Guid"] = pageInfo.OperateIdentifer;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定时间段内的售水记录数量</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserId=用水户ID，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetSaleWaterRecordsCountByWaterUser(string loginIdentifer, string waterUserId, string startTime, string endTime)
        {
            return this.GetSaleWaterRecordsCountByWaterUsers(loginIdentifer, waterUserId, startTime, endTime);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定时间段内的售水记录数量</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserIds=用水户ID,多个用','隔开，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetSaleWaterRecordsCountByWaterUsers(string loginIdentifer, string waterUserIds, string startTime, string endTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Count", "");
            obj2.Add("Guid", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, false);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserIds == null) || (waterUserIds.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                string[] strArray = waterUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                PageInfo pageInfo = new PageInfo
                {
                    OperateIdentifer = Guid.NewGuid().ToString(""),
                    QueryStartTime = startTime,
                    QueryEndTime = endTime
                };
                for (int i = 0; i < strArray.Length; i++)
                {
                    /*
                    string strMsg = "";
                    DataTable table = DBManager.Query("select count(售水时间) from 售水记录 where 用水户ID='" + strArray[i] + "' and 售水时间>='" + startTime + "' and 售水时间<='" + endTime + "'", out strMsg);
                    if (table == null)
                    {
                        obj2["Message"] = strMsg;
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                    if (!(table.Rows[0][0].ToString() == "0"))
                    {
                        pageInfo.DicDeviceRecordsCount.Add(strArray[i], Convert.ToInt32(table.Rows[0][0].ToString()));
                    }
                     * */
                    string sql = "select count(Id) from CardUserRechargeLog where WateUserId='" + strArray[i] + "' and LogTime>='" + startTime + "' and LogTime<='" + endTime + "'";
                    try
                    {
                        DataTable table = DbHelperSQL.Query(sql).Tables[0];
                        if (!(table.Rows[0][0].ToString() == "0"))
                        {
                            pageInfo.DicDeviceRecordsCount.Add(strArray[i], Convert.ToInt32(table.Rows[0][0].ToString()));
                        }
                    }
                    catch (Exception ex)
                    {
                        obj2["Message"] = ex.Message;
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                }
                GlobalAppModule.AddPageInfo(pageInfo);
                obj2["Result"] = true;
                obj2["Count"] = pageInfo.RecordsCount.ToString();
                obj2["Guid"] = pageInfo.OperateIdentifer;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定页码指定数量的售水记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Records':[object1,...,objectn]}</p>")]
        public string GetSaleWaterRecordsByWaterUser(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            return this.GetSaleWaterRecordsByWaterUsers(loginIdentifer, operateIdentifer, startIndex, count);
        }

        private ResMsg GetSaleWaterRecordsByWaterUsers(string operateIdentifer, string startIndex, string count, ref JavaScriptArray jsaRecords)
        {
            return this.GetSaleWaterRecords(operateIdentifer, "", startIndex, count, ref jsaRecords);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定页码指定数量的售水记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Records':[object1,...,objectn]}</p>")]
        public string GetSaleWaterRecordsByWaterUsers(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Records", new JavaScriptArray());
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                JavaScriptArray jsaRecords = new JavaScriptArray();
                msg = this.GetSaleWaterRecordsByWaterUsers(operateIdentifer, startIndex, count, ref jsaRecords);
                if (msg.Result)
                {
                    obj2["Result"] = true;
                    obj2["Records"] = jsaRecords;
                }
                else
                {
                    obj2["Message"] = msg.Message;
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg GetSaleWaterRecords(string operateIdentifer, string queryType, string startIndex, string count, ref JavaScriptArray jsaRecords)
        {
            PageInfo pageInfo = GlobalAppModule.GetPageInfo(operateIdentifer);
            if (pageInfo == null)
            {
                return new ResMsg(false, "查询操作超时");
            }
            pageInfo.LastOperateTime = DateTime.Now;
            int num = 0;
            try
            {
                num = Convert.ToInt32(startIndex);
            }
            catch
            {
                return new ResMsg(false, "参数startIndex格式不正确");
            }
            int recordsCount = 0;
            try
            {
                recordsCount = Convert.ToInt32(count);
            }
            catch
            {
                return new ResMsg(false, "参数count格式不正确");
            }
            if (num > pageInfo.RecordsCount)
            {
                return new ResMsg(false, "参数startIndex超范围");
            }
            if (((num + recordsCount) - 1) > pageInfo.RecordsCount)
            {
                recordsCount = (pageInfo.RecordsCount - num) + 1;
            }
            if (jsaRecords == null)
            {
                jsaRecords = new JavaScriptArray();
            }
            PageQueryCondition pageQueryCondition = pageInfo.GetPageQueryCondition(num, recordsCount);
            string sql = "";
            for (int i = 0; i < pageQueryCondition.Ids.Count; i++)
            {
                if (queryType == "times")
                {
                    sql = "select top " + pageInfo.QueryEndTime + " * from CardUserRechargeLog where WateUserId='" + pageQueryCondition.Ids[i] + "' and LogTime" + ((pageInfo.RecordType == "ASC") ? ">='" : "<='") + pageInfo.QueryStartTime + "' order by LogTime " + pageInfo.RecordType;
                }
                else
                {
                    sql = "select * from CardUserRechargeLog where WateUserId='" + pageQueryCondition.Ids[i] + "' and LogTime>='" + pageInfo.QueryStartTime + "' and LogTime<'" + pageInfo.QueryEndTime + "' order by LogTime";
                }
                try
                {
                    //DataTable table = DBManager.Query(sql, out strMsg);
                    DataTable table = DbHelperSQL.Query(sql).Tables[0];
                    for (int j = 0; j < table.Rows.Count; j++)
                    {
                        if ((i != 0) || (j >= pageQueryCondition.HeadRemoveCount))
                        {
                            if ((i == (pageQueryCondition.Ids.Count - 1)) && (j >= pageQueryCondition.EndRemoveCount))
                            {
                                break;
                            }
                            DataRow row = table.Rows[j];
                            JavaScriptObject item = new JavaScriptObject();
                            string UserName = WaterUserModule.GetWaterUserById(long.Parse(row["WateUserId"].ToString())).UserName;
                            item.Add("用水户ID", row["WateUserId"].ToString());
                            item.Add("用水户名称", UserName);
                            item.Add("用水户卡号", row["UserNo"].ToString());
                            item.Add("售水时间", row["LogTime"].ToString());
                            item.Add("售水方式", "IC卡");
                            item.Add("售水金额", row["WaterPrice"].ToString());
                            item.Add("售出水量", row["WaterNum"].ToString());
                            item.Add("售电金额", row["ElectricPrice"].ToString());
                            item.Add("售出电量", row["ElectricNum"].ToString());
                            item.Add("应收金额", row["TotalPrice"].ToString());
                            item.Add("实收金额", row["TotalPrice"].ToString());
                            item.Add("购水人", UserName);
                            item.Add("操作员ID", row["LogUserId"].ToString());
                            item.Add("操作员名称", row["LogUserName"].ToString());
                            item.Add("备注", row["Remark"].ToString());
                            item.Add("详细信息", "");
                            jsaRecords.Add(item);
                        }
                    }
                }
                catch(Exception ex)
                {
                    return new ResMsg(false, ex.Message);
                }
            }
            return new ResMsg(true, "");
        }

        private ResMsg GetSaleWaterRecordsByTimes(string operateIdentifer, string startIndex, string count, ref JavaScriptArray jsaRecords)
        {
            return this.GetSaleWaterRecords(operateIdentifer, "times", startIndex, count, ref jsaRecords);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定页码指定数量的售水记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Records':[object1,...,objectn]}</p>")]
        public string GetSaleWaterRecordsByTimes(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Records", new JavaScriptArray());
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                JavaScriptArray jsaRecords = new JavaScriptArray();
                msg = this.GetSaleWaterRecordsByTimes(operateIdentifer, startIndex, count, ref jsaRecords);
                if (msg.Result)
                {
                    obj2["Result"] = true;
                    obj2["Records"] = jsaRecords;
                }
                else
                {
                    obj2["Message"] = msg.Message;
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定时间前或后指定数量的用水记录数量</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserIds=用水户ID,多个用','隔开，baseTime=起始时间，isAsc=true:向后,false:向前，useWaterTimes=次数<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetUseWaterRecordsCountByTimes(string loginIdentifer, string waterUserIds, string baseTime, bool isAsc, uint useWaterTimes)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Count", "");
            obj2.Add("Guid", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, false);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserIds == null) || (waterUserIds.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((baseTime == null) || (baseTime.Trim() == ""))
                {
                    obj2["Message"] = "基准时间不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                try
                {
                    Convert.ToDateTime(baseTime);
                }
                catch (Exception)
                {
                    obj2["Message"] = "基准时间格式不正确";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                string[] strArray = waterUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                PageInfo pageInfo = new PageInfo
                {
                    OperateIdentifer = Guid.NewGuid().ToString(""),
                    RecordType = isAsc ? "ASC" : "DESC",
                    QueryStartTime = baseTime,
                    QueryEndTime = useWaterTimes.ToString()
                };
                for (int i = 0; i < strArray.Length; i++)
                {
                    /*
                    string strMsg = "";
                    DataTable table = DBManager.Query(string.Concat(new object[] { "select count(关泵时间) from (select top ", useWaterTimes, " 关泵时间 from 用水记录表 where 用水户ID='", strArray[i], "' and 关泵时间", isAsc ? ">='" : "<='", baseTime, "' order by 关泵时间 ", isAsc ? "ASC" : "DESC", ") as counttemp" }), out strMsg);
                    if (table == null)
                    {
                        obj2["Message"] = strMsg;
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                    if (!(table.Rows[0][0].ToString() == "0"))
                    {
                        pageInfo.DicDeviceRecordsCount.Add(strArray[i], Convert.ToInt32(table.Rows[0][0].ToString()));
                    }
                     * */
                    string sql = string.Concat(new object[] { "select count(Id) from (select top ", useWaterTimes, " Id from CardUserWaterLog where WateUserId='", strArray[i], "' and EndTime", isAsc ? ">='" : "<='", baseTime, "' order by EndTime ", isAsc ? "ASC" : "DESC", ") as counttemp" });
                    try
                    {
                        DataTable table = DbHelperSQL.Query(sql).Tables[0];
                        if (!(table.Rows[0][0].ToString() == "0"))
                        {
                            pageInfo.DicDeviceRecordsCount.Add(strArray[i], Convert.ToInt32(table.Rows[0][0].ToString()));
                        }
                    }
                    catch (Exception ex)
                    {
                        obj2["Message"] = ex.Message;
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                }
                GlobalAppModule.AddPageInfo(pageInfo);
                obj2["Result"] = true;
                obj2["Count"] = pageInfo.RecordsCount.ToString();
                obj2["Guid"] = pageInfo.OperateIdentifer;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定数量的用水记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Records':[object1,...,objectn]}</p>")]
        public string GetPeriodUseWaterRecordsByWaterUser(string loginIdentifer, string waterUserId, string periodType, string periodTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Records", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserId == null) || (waterUserId.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((periodType == null) || (periodType.Trim() == ""))
                {
                    obj2["Message"] = "时段类型不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((periodTime == null) || (periodTime.Trim() == ""))
                {
                    obj2["Message"] = "时段起始时间不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                DateTime now = DateTime.Now;
                DateTime time2 = DateTime.Now;
                try
                {
                    now = Convert.ToDateTime(periodTime);
                    string str3 = periodType;
                    if (str3 == null)
                    {
                        goto Label_017A;
                    }
                    if (!(str3 == "时"))
                    {
                        if (str3 == "日")
                        {
                            goto Label_015A;
                        }
                        if (str3 == "月")
                        {
                            goto Label_016E;
                        }
                        goto Label_017A;
                    }
                    time2 = now.AddHours(1.0);
                    goto Label_01BD;
                Label_015A:
                    time2 = now.AddDays(1.0);
                    goto Label_01BD;
                Label_016E:
                    time2 = now.AddMonths(1);
                    goto Label_01BD;
                Label_017A:
                    obj2["Message"] = "不支持时段" + periodType;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                catch
                {
                    obj2["Message"] = "时段起始时间格式不正确";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            Label_01BD:
                try
                {
                    string sql = "select * from CardUserWaterLog where WateUserId='" + waterUserId + "' and EndTime>='" + now.ToString("yyyy-MM-dd HH:mm:ss") + "' and EndTime<'" + time2.ToString("yyyy-MM-dd HH:mm:ss") + "' order by EndTime";
                    //DataTable table = DBManager.Query("select * from 用水记录表 where 用水户ID='" + waterUserId + "' and 关泵时间>='" + now.ToString("yyyy-MM-dd HH:mm:ss") + "' and 关泵时间<'" + time2.ToString("yyyy-MM-dd HH:mm:ss") + "' order by 关泵时间", out str);
                    DataTable table = DbHelperSQL.Query(sql).Tables[0];
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow row = table.Rows[i];
                        JavaScriptObject item = new JavaScriptObject();
                        item.Add("用水户ID", row["WateUserId"].ToString());
                        item.Add("用水户名称", WaterUserModule.GetWaterUserById(long.Parse(row["WateUserId"].ToString())).UserName);
                        item.Add("设备ID", row["DeviceId"].ToString());
                        item.Add("设备名称", DeviceModule.GetDeviceByID(long.Parse(row["DeviceId"].ToString())).DeviceName);
                        item.Add("用户卡号", row["UserNo"].ToString());
                        item.Add("卡序列号", row["SerialNumber"].ToString());
                        item.Add("开泵时间", row["StartTime"].ToString());
                        item.Add("开泵卡剩余水量", row["StartResidualWater"].ToString());
                        item.Add("开泵卡剩余电量", row["StartResidualElectric"].ToString());
                        item.Add("关泵时间", row["EndTime"].ToString());
                        item.Add("关泵卡剩余水量", row["EndResidualWater"].ToString());
                        item.Add("关泵卡剩余电量", row["EndResidualElectric"].ToString());
                        item.Add("灌溉时长", CommonUtil.GetTimeFromSecond(row["Duration"].ToString()));
                        item.Add("本次用水量", row["WaterUsed"].ToString());
                        item.Add("本次用电量", row["ElectricUsed"].ToString());
                        array.Add(item);
                    }
                    obj2["Result"] = true;
                }
                catch (Exception ex)
                {
                    obj2["Message"] = ex.Message;
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定时间段内的用水记录数量</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserId=用水户ID，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetUseWaterRecordsCountByWaterUser(string loginIdentifer, string waterUserId, string startTime, string endTime)
        {
            return this.GetUseWaterRecordsCountByWaterUsers(loginIdentifer, waterUserId, startTime, endTime);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定时间段内的用水记录数量</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserIds=用水户ID,多个用','隔开，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetUseWaterRecordsCountByWaterUsers(string loginIdentifer, string waterUserIds, string startTime, string endTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Count", "");
            obj2.Add("Guid", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            if ((waterUserIds == null) || (waterUserIds.Trim() == ""))
            {
                obj2["Message"] = "用水户ID不能为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string[] strArray = waterUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            PageInfo pageInfo = new PageInfo
            {
                OperateIdentifer = Guid.NewGuid().ToString(""),
                QueryStartTime = startTime,
                QueryEndTime = endTime
            };
            for (int i = 0; i < strArray.Length; i++)
            {
                /*
                string strMsg = "";
                DataTable table = DBManager.Query("select count(关泵时间) from 用水记录表 where 用水户ID='" + strArray[i] + "' and 关泵时间>='" + startTime + "' and 关泵时间<'" + endTime + "'", out strMsg);
                if (table == null)
                {
                    obj2["Message"] = strMsg;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (!(table.Rows[0][0].ToString() == "0"))
                {
                    pageInfo.DicDeviceRecordsCount.Add(strArray[i], Convert.ToInt32(table.Rows[0][0].ToString()));
                }
                 * */
                string sql = "select count(Id) from CardUserWaterLog where WateUserId='" + strArray[i] + "' and EndTime>='" + startTime + "' and EndTime<'" + endTime + "'";
                try
                {
                    DataTable table = DbHelperSQL.Query(sql).Tables[0];
                    if (!(table.Rows[0][0].ToString() == "0"))
                    {
                        pageInfo.DicDeviceRecordsCount.Add(strArray[i], Convert.ToInt32(table.Rows[0][0].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    obj2["Message"] = ex.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            GlobalAppModule.AddPageInfo(pageInfo);
            obj2["Result"] = true;
            obj2["Count"] = pageInfo.RecordsCount.ToString();
            obj2["Guid"] = pageInfo.OperateIdentifer;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定数量的用水记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Records':[object1,...,objectn]}</p>")]
        public string GetUseWaterRecordsByWaterUser(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            return this.GetUseWaterRecordsByWaterUsers(loginIdentifer, operateIdentifer, startIndex, count);
        }

        private ResMsg GetUseWaterRecordsByWaterUsers(string operateIdentifer, string startIndex, string count, ref JavaScriptArray jsaRecords)
        {
            return this.GetUseWaterRecords(operateIdentifer, "", startIndex, count, ref jsaRecords);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定数量的用水记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Records':[object1,...,objectn]}</p>")]
        public string GetUseWaterRecordsByWaterUsers(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            JavaScriptArray jsaRecords = new JavaScriptArray();
            ResMsg msg = this.GetUseWaterRecordsByWaterUsers(operateIdentifer, startIndex, count, ref jsaRecords);
            if (msg.Result)
            {
                obj2["Result"] = true;
                obj2["Records"] = jsaRecords;
            }
            else
            {
                obj2["Message"] = msg.Message;
                obj2["Records"] = new JavaScriptArray();
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg GetUseWaterRecords(string operateIdentifer, string queryType, string startIndex, string count, ref JavaScriptArray jsaRecords)
        {
            PageInfo pageInfo = GlobalAppModule.GetPageInfo(operateIdentifer);
            if (pageInfo == null)
            {
                return new ResMsg(false, "查询操作超时");
            }
            pageInfo.LastOperateTime = DateTime.Now;
            int num = 0;
            try
            {
                num = Convert.ToInt32(startIndex);
            }
            catch
            {
                return new ResMsg(false, "参数startIndex格式不正确");
            }
            int recordsCount = 0;
            try
            {
                recordsCount = Convert.ToInt32(count);
            }
            catch
            {
                return new ResMsg(false, "参数count格式不正确");
            }
            if (num > pageInfo.RecordsCount)
            {
                return new ResMsg(false, "参数startIndex超范围");
            }
            if (((num + recordsCount) - 1) > pageInfo.RecordsCount)
            {
                recordsCount = (pageInfo.RecordsCount - num) + 1;
            }
            if (jsaRecords == null)
            {
                jsaRecords = new JavaScriptArray();
            }
            PageQueryCondition pageQueryCondition = pageInfo.GetPageQueryCondition(num, recordsCount);
            string sql = "";
            for (int i = 0; i < pageQueryCondition.Ids.Count; i++)
            {
                if (queryType == "times")
                {
                    sql = "select top " + pageInfo.QueryEndTime + " * from CardUserWaterLog where WateUserId='" + pageQueryCondition.Ids[i] + "' and EndTime" + ((pageInfo.RecordType == "ASC") ? ">='" : "<='") + pageInfo.QueryStartTime + "' order by EndTime " + pageInfo.RecordType;
                }
                else
                {
                    sql = "select * from CardUserWaterLog where WateUserId='" + pageQueryCondition.Ids[i] + "' and EndTime>='" + pageInfo.QueryStartTime + "' and EndTime<'" + pageInfo.QueryEndTime + "' order by EndTime";
                }
                try
                {
                    DataTable table = DbHelperSQL.Query(sql).Tables[0];
                    for (int j = 0; j < table.Rows.Count; j++)
                    {
                        if ((i != 0) || (j >= pageQueryCondition.HeadRemoveCount))
                        {
                            if ((i == (pageQueryCondition.Ids.Count - 1)) && (j >= pageQueryCondition.EndRemoveCount))
                            {
                                break;
                            }
                            DataRow row = table.Rows[j];
                            JavaScriptObject item = new JavaScriptObject();
                            item.Add("用水户ID", row["WateUserId"].ToString());
                            item.Add("用水户名称", WaterUserModule.GetWaterUserById(long.Parse(row["WateUserId"].ToString())).UserName);
                            item.Add("设备ID", row["DeviceId"].ToString());
                            item.Add("设备名称", DeviceModule.GetDeviceByID(long.Parse(row["DeviceId"].ToString())).DeviceName);
                            item.Add("用户卡号", row["UserNo"].ToString());
                            item.Add("卡序列号", row["SerialNumber"].ToString());
                            item.Add("开泵时间", row["StartTime"].ToString());
                            item.Add("开泵卡剩余水量", row["StartResidualWater"].ToString());
                            item.Add("开泵卡剩余电量", row["StartResidualElectric"].ToString());
                            item.Add("关泵时间", row["EndTime"].ToString());
                            item.Add("关泵卡剩余水量", row["EndResidualWater"].ToString());
                            item.Add("关泵卡剩余电量", row["EndResidualElectric"].ToString());
                            item.Add("灌溉时长", CommonUtil.GetTimeFromSecond(row["Duration"].ToString()));
                            item.Add("本次用水量", row["WaterUsed"].ToString());
                            item.Add("本次用电量", row["ElectricUsed"].ToString());
                            jsaRecords.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new ResMsg(false, ex.Message);
                }
            }
            return new ResMsg(true, "");
        }

        private ResMsg GetUseWaterRecordsByTimes(string operateIdentifer, string startIndex, string count, ref JavaScriptArray jsaRecords)
        {
            return this.GetUseWaterRecords(operateIdentifer, "times", startIndex, count, ref jsaRecords);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户查询指定数量的用水记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Records':[object1,...,objectn]}</p>")]
        public string GetUseWaterRecordsByTimes(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                JavaScriptArray jsaRecords = new JavaScriptArray();
                msg = this.GetUseWaterRecordsByTimes(operateIdentifer, startIndex, count, ref jsaRecords);
                if (msg.Result)
                {
                    obj2["Result"] = true;
                    obj2["Records"] = jsaRecords;
                }
                else
                {
                    obj2["Message"] = msg.Message;
                    obj2["Records"] = new JavaScriptArray();
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户导出指定数量的售水记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Records':[object1,...,objectn]}</p>")]
        public string ExportSaleWaterRecordsByWaterUsers(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("ExcelURL", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            JavaScriptArray jsaRecords = new JavaScriptArray();
            ResMsg msg = this.GetSaleWaterRecordsByWaterUsers(operateIdentifer, startIndex, count, ref jsaRecords);
            if (!msg.Result)
            {
                obj2["Message"] = msg.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "Sale Water Records", Created = DateTime.Now }
            };
            WorksheetStyle style = workbook.Styles.Add("HeaderStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Font.Color = "Black";
            style = workbook.Styles.Add("ColumnCaptionStyle");
            style.Font.FontName = "宋体";
            style.Font.Bold = true;
            style.Font.Size = 11;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style = workbook.Styles.Add("DefaultStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            Worksheet worksheet = workbook.Worksheets.Add("售水记录");
            for (int i = 0; i < 9; i++)
            {
                if (i == 1)
                {
                    worksheet.Table.Columns.Add(new WorksheetColumn(110));
                }
                else
                {
                    worksheet.Table.Columns.Add(new WorksheetColumn(80));
                }
            }
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("售水记录");
            cell.MergeAcross = 8;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("用水户").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("充值时间").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("充值金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购水金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购水量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购电金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购电量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("代办人").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("操作员").StyleID = "ColumnCaptionStyle";
            JavaScriptObject obj3 = null;
            for (int j = 0; j < jsaRecords.Count; j++)
            {
                obj3 = (JavaScriptObject)jsaRecords[j];
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(obj3["用水户名称"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["售水时间"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["应收金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["售水金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["售出水量"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["售电金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["售出电量"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["购水人"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["操作员名称"].ToString()).StyleID = "DefaultStyle";
            }
            try
            {
                string str = DateTime.Now.Ticks.ToString() + ".xls";
                string strPath = this.context.Server.MapPath("~/DataQuery");
                workbook.Save(strPath + @"\" + str);
                obj2["ExcelURL"] = str;
                obj2["Result"] = true;
                CommonUtil.RemoveFiles(strPath, ".xls");
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户导出指定数量的用水记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'ExcelURL':string}</p>")]
        public string ExportUseWaterRecordsByWaterUsers(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("ExcelURL", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            JavaScriptArray jsaRecords = new JavaScriptArray();
            ResMsg msg = this.GetUseWaterRecordsByWaterUsers(operateIdentifer, startIndex, count, ref jsaRecords);
            if (!msg.Result)
            {
                obj2["Message"] = msg.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "Use Water Records", Created = DateTime.Now }
            };
            WorksheetStyle style = workbook.Styles.Add("HeaderStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Font.Color = "Black";
            style = workbook.Styles.Add("ColumnCaptionStyle");
            style.Font.FontName = "宋体";
            style.Font.Bold = true;
            style.Font.Size = 11;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style = workbook.Styles.Add("ColumnCaptionWrapTextStyle");
            style.Font.FontName = "宋体";
            style.Font.Bold = true;
            style.Font.Size = 11;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Alignment.WrapText = true;
            style = workbook.Styles.Add("DefaultStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style = workbook.Styles.Add("DefaultWrapTextStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Alignment.WrapText = true;
            Worksheet worksheet = workbook.Worksheets.Add("用水记录");
            worksheet.Table.Columns.Add(new WorksheetColumn(130));
            worksheet.Table.Columns.Add(new WorksheetColumn(130));
            worksheet.Table.Columns.Add(new WorksheetColumn(200));
            worksheet.Table.Columns.Add(new WorksheetColumn(150));
            worksheet.Table.Columns.Add(new WorksheetColumn(150));
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("用水记录");
            cell.MergeAcross = 4;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("用水户").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("设备名称").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("灌溉时长\n开泵时间/关泵时间").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("本次用水量\n开泵时剩余/关泵时剩余").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("本次用电量\n开泵时剩余/关泵时剩余").StyleID = "ColumnCaptionWrapTextStyle";
            JavaScriptObject obj3 = null;
            for (int i = 0; i < jsaRecords.Count; i++)
            {
                obj3 = (JavaScriptObject)jsaRecords[i];
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(obj3["用水户名称"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["设备名称"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["灌溉时长"].ToString() + "\n" + obj3["开泵时间"].ToString() + "/" + obj3["关泵时间"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["本次用水量"].ToString() + "\n" + obj3["开泵卡剩余水量"].ToString() + "/" + obj3["关泵卡剩余水量"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["本次用电量"].ToString() + "\n" + obj3["开泵卡剩余电量"].ToString() + "/" + obj3["关泵卡剩余电量"].ToString()).StyleID = "DefaultWrapTextStyle";
            }
            try
            {
                string str = DateTime.Now.Ticks.ToString() + ".xls";
                string strPath = this.context.Server.MapPath("~/DataQuery");
                workbook.Save(strPath + @"\" + str);
                obj2["ExcelURL"] = str;
                obj2["Result"] = true;
                CommonUtil.RemoveFiles(strPath, ".xls");
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>操作记录查询</span><br/><p style='text-indent:15px'>输入参数：loginIdentifer=登录标识;operatorIds=操作员ID,多个用','隔开;startTime=起始时间;endTime=结束时间;isExprot=是否导出Excel<br/>\r\n                返回数据：{'Results':bool,'Message':string,'Records':[{记录1}、{记录2}、……],ExcelURL:string}</p>")]
        public string GetOperateRecordsByUser(string loginIdentifer, string operatorIds, string startTime, string endTime, bool isExprot)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Records", array);
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            if ((operatorIds == null) || (operatorIds.Trim() == ""))
            {
                obj2["Message"] = "操作员ID不能为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            try
            {
                List<string> userIDs = new List<string>(operatorIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                DateTime time = Convert.ToDateTime(startTime);
                DateTime time2 = Convert.ToDateTime(endTime);
                DataSet set = DBManageModule.OperateDataQueryByUser(userIDs, time, time2);
                if (set.Tables.Count == 0)
                {
                    obj2["Message"] = "查询结果为空！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                DataTable dtExcel = null;
                if (isExprot)
                {
                    dtExcel = new DataTable();
                }
                ResMsg msg = this.OperateRecordsToJson(set.Tables[0], ref array, ref dtExcel);
                if (!msg.Result)
                {
                    obj2["Message"] = "查询失败，原因：" + msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((dtExcel != null) && (dtExcel.Rows.Count > 0))
                {
                    msg = this.ExportOperateRecords(dtExcel, time, time2);
                    if (!msg.Result)
                    {
                        obj2["Message"] = "查询失败，原因：" + msg.Message;
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                    obj2["Result"] = true;
                    obj2["ExcelURL"] = msg.Message;
                }
                else
                {
                    obj2["Result"] = true;
                    obj2["ExcelURL"] = "";
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg OperateRecordsToJson(DataTable dtOperateRecords, ref JavaScriptArray jsaRecords, ref DataTable dtExcel)
        {
            try
            {
                if (dtExcel != null)
                {
                    dtExcel.Columns.Add("操作员");
                    dtExcel.Columns.Add("操作时间");
                    dtExcel.Columns.Add("操作名称");
                    dtExcel.Columns.Add("操作描述");
                    dtExcel.Columns.Add("发送数据");
                    dtExcel.Columns.Add("发送状态");
                }
                if (jsaRecords == null)
                {
                    jsaRecords = new JavaScriptArray();
                }
                for (int i = 0; i < dtOperateRecords.Rows.Count; i++)
                {
                    string str = dtOperateRecords.Rows[i]["用户名"].ToString();
                    string str2 = dtOperateRecords.Rows[i]["记录时间"].ToString();
                    string str3 = dtOperateRecords.Rows[i]["操作名称"].ToString();
                    string str4 = ((dtOperateRecords.Rows[i]["设备名称"].ToString() == "") ? "" : (dtOperateRecords.Rows[i]["设备名称"].ToString() + ":")) + dtOperateRecords.Rows[i]["操作描述"].ToString();
                    string str5 = dtOperateRecords.Rows[i]["发送数据"].ToString();
                    string str6 = dtOperateRecords.Rows[i]["发送状态"].ToString();
                    if (dtExcel != null)
                    {
                        dtExcel.Rows.Add(dtExcel.NewRow());
                        dtExcel.Rows[dtExcel.Rows.Count - 1]["操作员"] = str;
                        dtExcel.Rows[dtExcel.Rows.Count - 1]["操作时间"] = str2;
                        dtExcel.Rows[dtExcel.Rows.Count - 1]["操作名称"] = str3;
                        dtExcel.Rows[dtExcel.Rows.Count - 1]["操作描述"] = str4;
                        dtExcel.Rows[dtExcel.Rows.Count - 1]["发送数据"] = str5;
                        dtExcel.Rows[dtExcel.Rows.Count - 1]["发送状态"] = str6;
                    }
                    JavaScriptObject item = new JavaScriptObject();
                    item.Add("操作员", new JavaScriptArray(new string[] { "userName", str, "操作员" }));
                    item.Add("操作时间", new JavaScriptArray(new string[] { "recordTime", str2, "操作时间" }));
                    item.Add("操作名称", new JavaScriptArray(new string[] { "operateName", str3, "操作名称" }));
                    item.Add("操作描述", new JavaScriptArray(new string[] { "operateDecription", str4, "操作描述" }));
                    item.Add("发送数据", new JavaScriptArray(new string[] { "RawData", str5, "发送数据" }));
                    item.Add("发送状态", new JavaScriptArray(new string[] { "State", str6, "发送状态" }));
                    jsaRecords.Add(item);
                }
                return new ResMsg(true, "");
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
        }

        private ResMsg ExportOperateRecords(DataTable dtExcel, DateTime dtStartTime, DateTime dtEndTime)
        {
            if ((dtExcel == null) || (dtExcel.Rows.Count == 0))
            {
                return new ResMsg(true, "");
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { ActiveSheetIndex = 0, WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "操作记录", Created = DateTime.Now }
            };
            WorksheetStyle style = workbook.Styles.Add("HeaderStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Font.Color = "Black";
            style = workbook.Styles.Add("ColumnCaptionStyle");
            style.Font.FontName = "宋体";
            style.Font.Bold = true;
            style.Font.Size = 11;
            style = workbook.Styles.Add("Default");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            Worksheet worksheet = workbook.Worksheets.Add("操作记录");
            for (int i = 0; i < dtExcel.Columns.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        {
                            worksheet.Table.Columns.Add(new WorksheetColumn(110));
                            continue;
                        }
                    case 3:
                        {
                            worksheet.Table.Columns.Add(new WorksheetColumn(200));
                            continue;
                        }
                }
                worksheet.Table.Columns.Add(new WorksheetColumn(80));
            }
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("操作记录(" + dtStartTime.ToString("yyyy-MM-dd HH:mm") + "—" + dtEndTime.ToString("yyyy-MM-dd HH:mm") + ")");
            cell.MergeAcross = 3;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            for (int j = 0; j < dtExcel.Columns.Count; j++)
            {
                row.Cells.Add(dtExcel.Columns[j].Caption).StyleID = "ColumnCaptionStyle";
            }
            for (int k = 0; k < dtExcel.Rows.Count; k++)
            {
                row = worksheet.Table.Rows.Add();
                for (int m = 0; m < dtExcel.Columns.Count; m++)
                {
                    row.Cells.Add(dtExcel.Rows[k][m].ToString()).StyleID = "Default";
                }
            }
            try
            {
                string message = DateTime.Now.Ticks.ToString() + ".xls";
                string strPath = this.context.Server.MapPath("~/DataQuery");
                workbook.Save(strPath + @"\" + message);
                CommonUtil.RemoveFiles(strPath, ".xls");
                return new ResMsg(true, message);
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>操作记录查询</span><br/><p style='text-indent:15px'>输入参数：loginIdentifer=登录标识;operatorIds=操作员ID,多个用','隔开;startTime=起始时间;endTime=结束时间;isExprot=是否导出Excel<br/>\r\n                返回数据：{'Results':bool,'Message':string,'Records':[{记录1}、{记录2}、……],ExcelURL:string}</p>")]
        public string ExportOperateRecordsByUser(string loginIdentifer, string operatorIds, string startTime, string endTime)
        {
            JavaScriptObject obj2 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(this.GetOperateRecordsByUser(loginIdentifer, operatorIds, startTime, endTime, true));
            obj2.Remove("Records");
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取全部事件类型</span><br/><p>输入参数：loginIdentifer=登录用户标识，templateIds=模板ID,多个用','隔开<br/>返回数据格式：{'Result':bool,'Message':string,'EventTypes':[type1,...,typen]}</p>")]
        public string GetEventTypes(string loginIdentifer, string deviceIds)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("EventTypes", array);
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            try
            {

                List<string> eventTypes = new List<string>();
                eventTypes.Add("设备登录");
                eventTypes.Add("状态自报");
                eventTypes.Add("开泵上报");
                eventTypes.Add("关泵上报");
                eventTypes.Add("查询时间响应");
                eventTypes.Add("查询开采量响应");
                eventTypes.Add("设置时间响应");
                eventTypes.Add("设置开采量响应");
                eventTypes.Add("远程开泵响应");
                eventTypes.Add("远程关泵响应");
                if (eventTypes.Count > 1)
                {
                    eventTypes.Insert(0, "全部");
                }
                array.AddRange(eventTypes.ToArray());
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定时间段内的事件记录条数</span><br/><p>输入参数：loginIdentifer=登录用户标识，deviceIDs=设备ID-多个用','隔开，startTime=起始时间，endTime=结束时间，eventType=事件类型<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetEventRecordsCount(string loginIdentifer, string deviceIDs, string startTime, string endTime, string eventType)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Count", "");
            obj2.Add("Guid", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            try
            {
                string[] collection = deviceIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> list = new List<string>(collection);
                DateTime time = Convert.ToDateTime(startTime);
                DateTime time2 = Convert.ToDateTime(endTime);
                DataSet set = DBManageModule.EventRecordsCount(list, time, time2, eventType);
                PageInfo pageInfo = new PageInfo
                {
                    OperateIdentifer = Guid.NewGuid().ToString(""),
                    OperatorId = loginUser.UserId.ToString(),
                    RecordType = eventType,
                    QueryStartTime = startTime,
                    QueryEndTime = endTime
                };
                if (set.Tables.Count > 0)
                {
                    for (int i = 0; i < set.Tables.Count; i++)
                    {
                        int num2 = Convert.ToInt32(set.Tables[i].Rows[0][0].ToString());
                        if (num2 != 0)
                        {
                            pageInfo.DicDeviceRecordsCount.Add(collection[i], num2);
                        }
                    }
                }
                GlobalAppModule.AddPageInfo(pageInfo);
                obj2["Result"] = true;
                obj2["Count"] = pageInfo.RecordsCount.ToString();
                obj2["Guid"] = pageInfo.OperateIdentifer;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的事件记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，记录数量=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetEventRecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptObject obj3 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Columns", obj3);
            obj2.Add("Records", array);
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            ResMsg msg = this.GetEventRecords(operateIdentifer, startIndex, count, ref obj3, ref array);
            obj2["Result"] = msg.Result;
            obj2["Message"] = msg.Message;
            string s = JavaScriptConvert.SerializeObject(obj2);
            return s;
        }

        private ResMsg GetEventRecords(string operateIdentifer, string startIndex, string count, ref JavaScriptObject jsoColumns, ref JavaScriptArray jsaRecords)
        {
            PageInfo pageInfo = GlobalAppModule.GetPageInfo(operateIdentifer);
            if (pageInfo == null)
            {
                return new ResMsg(false, "查询操作超时");
            }
            pageInfo.LastOperateTime = DateTime.Now;
            int num = 0;
            try
            {
                num = Convert.ToInt32(startIndex);
            }
            catch
            {
                return new ResMsg(false, "参数startIndex格式不正确");
            }
            int recordsCount = 0;
            try
            {
                recordsCount = Convert.ToInt32(count);
            }
            catch
            {
                return new ResMsg(false, "参数count格式不正确");
            }
            if (num > pageInfo.RecordsCount)
            {
                return new ResMsg(false, "参数startIndex超范围");
            }
            if (((num + recordsCount) - 1) > pageInfo.RecordsCount)
            {
                recordsCount = (pageInfo.RecordsCount - num) + 1;
            }
            if (jsoColumns == null)
            {
                jsoColumns = new JavaScriptObject();
            }
            if (jsaRecords == null)
            {
                jsaRecords = new JavaScriptArray();
            }
            try
            {
                PageQueryCondition pageQueryCondition = pageInfo.GetPageQueryCondition(num, recordsCount);
                List<string> eventRecordColumns = new List<string>(new string[] { "村庄", "设备", "设备编号", "事件时间", "事件类型", "事件描述", "事件数据" });
                for (int k = 0; k < eventRecordColumns.Count; k++)
                {
                    JavaScriptObject obj2 = new JavaScriptObject();
                    obj2.Add("Field", ChinesePY.GetPinYinIndex(eventRecordColumns[k]));
                    obj2.Add("HeadText", eventRecordColumns[k]);
                    jsoColumns.Add(eventRecordColumns[k], obj2);
                }
                DateTime startTime = Convert.ToDateTime(pageInfo.QueryStartTime);
                DateTime endTime = Convert.ToDateTime(pageInfo.QueryEndTime);
                DataSet dsQuery = DBManageModule.EventDataQuery(pageQueryCondition.Ids, startTime, endTime, pageInfo.RecordType);
                return this.AlarmOrEventRecordsToJson(dsQuery, pageQueryCondition.HeadRemoveCount, pageQueryCondition.EndRemoveCount, eventRecordColumns, ref jsaRecords);
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的事件记录2</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，记录数量=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetEventRecords2(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptObject obj3 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Columns", obj3);
            obj2.Add("Records", array);
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            ResMsg msg = this.GetEventRecords2(operateIdentifer, startIndex, count, ref obj3, ref array);
            obj2["Result"] = msg.Result;
            obj2["Message"] = msg.Message;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg GetEventRecords2(string operateIdentifer, string startIndex, string count, ref JavaScriptObject jsoColumns, ref JavaScriptArray jsaRecords)
        {
            PageInfo pageInfo = GlobalAppModule.GetPageInfo(operateIdentifer);
            if (pageInfo == null)
            {
                return new ResMsg(false, "查询操作超时");
            }
            pageInfo.LastOperateTime = DateTime.Now;
            int num = 0;
            try
            {
                num = Convert.ToInt32(startIndex);
            }
            catch
            {
                return new ResMsg(false, "参数startIndex格式不正确");
            }
            int recordsCount = 0;
            try
            {
                recordsCount = Convert.ToInt32(count);
            }
            catch
            {
                return new ResMsg(false, "参数count格式不正确");
            }
            if (num > pageInfo.RecordsCount)
            {
                return new ResMsg(false, "参数startIndex超范围");
            }
            if (((num + recordsCount) - 1) > pageInfo.RecordsCount)
            {
                recordsCount = (pageInfo.RecordsCount - num) + 1;
            }
            if (jsoColumns == null)
            {
                jsoColumns = new JavaScriptObject();
            }
            if (jsaRecords == null)
            {
                jsaRecords = new JavaScriptArray();
            }
            try
            {
                PageQueryCondition pageQueryCondition = pageInfo.GetPageQueryCondition(num, recordsCount);
                List<string> eventRecordColumns = new List<string>(new string[] { "村庄", "设备", "设备编号", "开泵时间", "关泵时间", "灌溉时长", "用水量" });
                for (int k = 0; k < eventRecordColumns.Count; k++)
                {
                    JavaScriptObject obj2 = new JavaScriptObject();
                    obj2.Add("Field", ChinesePY.GetPinYinIndex(eventRecordColumns[k]));
                    obj2.Add("HeadText", eventRecordColumns[k]);
                    jsoColumns.Add(eventRecordColumns[k], obj2);
                }
                DateTime startTime = Convert.ToDateTime(pageInfo.QueryStartTime);
                DateTime endTime = Convert.ToDateTime(pageInfo.QueryEndTime);
                DataSet dsQuery = DBManageModule.EventDataQuery2(pageQueryCondition.Ids, startTime, endTime, pageInfo.RecordType);
                return this.AlarmOrEventRecordsToJson(dsQuery, pageQueryCondition.HeadRemoveCount, pageQueryCondition.EndRemoveCount, eventRecordColumns, ref jsaRecords);
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
        }

        private ResMsg AlarmOrEventRecordsToJson(DataSet dsQuery, int firstStartIndex, int lastEndIndex, List<string> displayColumns, ref JavaScriptArray jsaRecords)
        {
            try
            {
                if (jsaRecords == null)
                {
                    jsaRecords = new JavaScriptArray();
                }
                Dictionary<string, string> aliasColumnValues = new Dictionary<string, string>();
                for (int i = 0; i < dsQuery.Tables.Count; i++)
                {
                    for (int j = 0; j < dsQuery.Tables[i].Rows.Count; j++)
                    {
                        if ((i != 0) || (j >= firstStartIndex))
                        {
                            if ((i == (dsQuery.Tables.Count - 1)) && (j >= lastEndIndex))
                            {
                                break;
                            }
                            JavaScriptObject item = new JavaScriptObject();
                            for (int k = 0; k < displayColumns.Count; k++)
                            {
                                string str = displayColumns[k];
                                string str3 = ChinesePY.GetPinYinIndex(str) + ((k + 1)).ToString();
                                JavaScriptObject obj3 = new JavaScriptObject();
                                obj3.Add("Field", str3);
                                if (dsQuery.Tables[i].Columns.Contains(str))
                                {
                                    if (str == "灌溉时长")
                                    {
                                        obj3.Add("Value", CommonUtil.GetTimeFromSecond(dsQuery.Tables[i].Rows[j][str].ToString()));
                                    }
                                    else
                                    {
                                        obj3.Add("Value", dsQuery.Tables[i].Rows[j][str].ToString());
                                    }
                                }
                                else
                                {
                                    obj3.Add("Value", "--");
                                }
                                item.Add(str, obj3);
                            }
                            jsaRecords.Add(item);
                        }
                    }
                }
                return new ResMsg(true, "");
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>导出指定数量的事件记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，记录数量=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'ExcelURL':string}</p>")]
        public string ExportEventRecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("ExcelUrl", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                msg = this.ExportEventRecords(operateIdentifer, startIndex, count);
                if (msg.Result)
                {
                    obj2["Result"] = true;
                    obj2["ExcelUrl"] = msg.Message;
                }
                else
                {
                    obj2["Message"] = msg.Message;
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg ExportEventRecords(string operateIdentifer, string startIndex, string count)
        {
            PageInfo pageInfo = GlobalAppModule.GetPageInfo(operateIdentifer);
            if (pageInfo == null)
            {
                return new ResMsg(false, "查询操作超时");
            }
            pageInfo.LastOperateTime = DateTime.Now;
            int num = 0;
            try
            {
                num = Convert.ToInt32(startIndex);
            }
            catch
            {
                return new ResMsg(false, "参数startIndex格式不正确");
            }
            int recordsCount = 0;
            try
            {
                recordsCount = Convert.ToInt32(count);
            }
            catch
            {
                return new ResMsg(false, "参数count格式不正确");
            }
            if (num > pageInfo.RecordsCount)
            {
                return new ResMsg(false, "参数startIndex超范围");
            }
            if (((num + recordsCount) - 1) > pageInfo.RecordsCount)
            {
                recordsCount = (pageInfo.RecordsCount - num) + 1;
            }
            try
            {
                PageQueryCondition pageQueryCondition = pageInfo.GetPageQueryCondition(num, recordsCount);
                List<string> eventRecordColumns = new List<string>(new string[] { "村庄", "设备", "设备编号", "事件时间", "事件类型", "事件描述" });
                DateTime startTime = Convert.ToDateTime(pageInfo.QueryStartTime);
                DateTime endTime = Convert.ToDateTime(pageInfo.QueryEndTime);
                DataSet dsQuery = DBManageModule.EventDataQuery(pageQueryCondition.Ids, startTime, endTime, pageInfo.RecordType);
                string flag = "event";
                return new ResMsg(true, this.AlarmOrEventRecordsToExcel(dsQuery, pageQueryCondition.HeadRemoveCount, pageQueryCondition.EndRemoveCount, eventRecordColumns, flag));
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>导出指定数量的事件记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，记录数量=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'ExcelURL':string}</p>")]
        public string ExportEventRecords2(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("ExcelUrl", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                msg = this.ExportEventRecords2(operateIdentifer, startIndex, count);
                if (msg.Result)
                {
                    obj2["Result"] = true;
                    obj2["ExcelUrl"] = msg.Message;
                }
                else
                {
                    obj2["Message"] = msg.Message;
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg ExportEventRecords2(string operateIdentifer, string startIndex, string count)
        {
            PageInfo pageInfo = GlobalAppModule.GetPageInfo(operateIdentifer);
            if (pageInfo == null)
            {
                return new ResMsg(false, "查询操作超时");
            }
            pageInfo.LastOperateTime = DateTime.Now;
            int num = 0;
            try
            {
                num = Convert.ToInt32(startIndex);
            }
            catch
            {
                return new ResMsg(false, "参数startIndex格式不正确");
            }
            int recordsCount = 0;
            try
            {
                recordsCount = Convert.ToInt32(count);
            }
            catch
            {
                return new ResMsg(false, "参数count格式不正确");
            }
            if (num > pageInfo.RecordsCount)
            {
                return new ResMsg(false, "参数startIndex超范围");
            }
            if (((num + recordsCount) - 1) > pageInfo.RecordsCount)
            {
                recordsCount = (pageInfo.RecordsCount - num) + 1;
            }
            try
            {
                PageQueryCondition pageQueryCondition = pageInfo.GetPageQueryCondition(num, recordsCount);
                List<string> eventRecordColumns = new List<string>(new string[] { "村庄", "设备", "设备编号", "开泵时间", "关泵时间", "灌溉时长", "用水量" });
                DateTime startTime = Convert.ToDateTime(pageInfo.QueryStartTime);
                DateTime endTime = Convert.ToDateTime(pageInfo.QueryEndTime);
                DataSet dsQuery = DBManageModule.EventDataQuery2(pageQueryCondition.Ids, startTime, endTime, pageInfo.RecordType);
                string flag = "event";
                return new ResMsg(true, this.AlarmOrEventRecordsToExcel(dsQuery, pageQueryCondition.HeadRemoveCount, pageQueryCondition.EndRemoveCount, eventRecordColumns, flag));
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
        }

        private string AlarmOrEventRecordsToExcel(DataSet dsQuery, int firstStartIndex, int lastEndIndex, List<string> displayColumns, string Flag)
        {
            try
            {
                string id = "";
                Dictionary<string, string> aliasColumnValues = new Dictionary<string, string>();
                JavaScriptArray jsaAlarmData = new JavaScriptArray();
                for (int i = 0; i < dsQuery.Tables.Count; i++)
                {
                    for (int j = 0; j < dsQuery.Tables[i].Rows.Count; j++)
                    {
                        if ((i != 0) || (j >= firstStartIndex))
                        {
                            if ((i == (dsQuery.Tables.Count - 1)) && (j >= lastEndIndex))
                            {
                                break;
                            }
                            JavaScriptObject item = new JavaScriptObject();
                            for (int k = 0; k < displayColumns.Count; k++)
                            {
                                string key = displayColumns[k];
                                if (dsQuery.Tables[i].Columns.Contains(key))
                                {
                                    item.Add(key, dsQuery.Tables[i].Rows[j][key].ToString());
                                }
                                else
                                {
                                    item.Add(key, "--");
                                }
                            }
                            jsaAlarmData.Add(item);
                        }
                    }
                }
                string str3 = "";
                str3 = (Flag == "alarm") ? "报警记录" : "事件记录";
                string bookTitle = str3;
                string tableTitle = str3;
                string bodyTitle = str3;
                return this.createExcel(bookTitle, tableTitle, bodyTitle, displayColumns, jsaAlarmData);
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        private string createExcel(string bookTitle, string tableTitle, string bodyTitle, List<string> displayColumns, JavaScriptArray jsaAlarmData)
        {
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { ActiveSheetIndex = 0, WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = bookTitle, Created = DateTime.Now }
            };
            WorksheetStyle style = workbook.Styles.Add("HeaderStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Font.Color = "Black";
            style = workbook.Styles.Add("ColumnCaptionStyle");
            style.Font.FontName = "宋体";
            style.Font.Bold = true;
            style.Font.Size = 10;
            style = workbook.Styles.Add("Default");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            Worksheet worksheet = workbook.Worksheets.Add(tableTitle);
            for (int i = 0; i < displayColumns.Count; i++)
            {
                string str4 = displayColumns[i];
                if (str4 == null)
                {
                    goto Label_01C6;
                }
                if (!(str4 == "事件描述"))
                {
                    if (((str4 == "事件类型") || (str4 == "事件时间")) || (str4 == "设备编号"))
                    {
                        goto Label_01AC;
                    }
                    goto Label_01C6;
                }
                worksheet.Table.Columns.Add(new WorksheetColumn(500));
                continue;
            Label_01AC:
                worksheet.Table.Columns.Add(new WorksheetColumn(120));
                continue;
            Label_01C6:
                worksheet.Table.Columns.Add(new WorksheetColumn(60));
            }
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add(bodyTitle);
            cell.MergeAcross = displayColumns.Count;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            for (int j = 0; j < displayColumns.Count; j++)
            {
                row.Cells.Add(displayColumns[j]).StyleID = "ColumnCaptionStyle";
            }
            string str = "";
            foreach (JavaScriptObject obj2 in jsaAlarmData)
            {
                row = worksheet.Table.Rows.Add();
                for (int k = 0; k < displayColumns.Count; k++)
                {
                    row.Cells.Add((string)obj2[displayColumns[k]]).StyleID = "Default";
                }
            }
            try
            {
                string str2 = DateTime.Now.Ticks.ToString() + ".xls";
                string strPath = this.context.Server.MapPath("~/DataQuery");
                workbook.Save(strPath + @"\" + str2);
                str = str2;
                CommonUtil.RemoveFiles(strPath, ".xls");
            }
            catch
            {
            }
            return str;
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取全部报警类型</span><br/><p>输入参数：loginIdentifer=登录用户标识<br/>返回数据格式：{'Result':bool,'Message':string,'AlarmTypes':[type1,...,typen]}</p>")]
        public string GetAlarmTypes(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("AlarmTypes", array);
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            try
            {
                List<string> list = BaseModule.GetAllAlarmTypeDesc();
                if (list.Count > 1)
                {
                    list.Insert(0, "全部");
                }
                array.AddRange(list.ToArray());
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定时间段内的报警记录条数</span><br/><p>输入参数：loginIdentifer=登录用户标识，deviceIDs=设备ID-多个用','隔开，startTime=起始时间，endTime=结束时间，alarmType=报警类型<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetAlarmRecordsCount(string loginIdentifer, string deviceIDs, string startTime, string endTime, string alarmType)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Count", "");
            obj2.Add("Guid", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            try
            {
                string[] collection = deviceIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> list = new List<string>(collection);
                DateTime time = Convert.ToDateTime(startTime);
                DateTime time2 = Convert.ToDateTime(endTime);
                DataSet set = DBManageModule.AlarmRecordsCount(list, time, time2, alarmType, false);
                PageInfo pageInfo = new PageInfo
                {
                    OperateIdentifer = Guid.NewGuid().ToString(""),
                    OperatorId = loginUser.UserId.ToString(),
                    RecordType = alarmType,
                    QueryStartTime = startTime,
                    QueryEndTime = endTime
                };
                if (set.Tables.Count > 0)
                {
                    for (int i = 0; i < set.Tables.Count; i++)
                    {
                        int num2 = Convert.ToInt32(set.Tables[i].Rows[0][0].ToString());
                        if (num2 != 0)
                        {
                            pageInfo.DicDeviceRecordsCount.Add(collection[i], num2);
                        }
                    }
                }
                GlobalAppModule.AddPageInfo(pageInfo);
                obj2["Result"] = true;
                obj2["Count"] = pageInfo.RecordsCount.ToString();
                obj2["Guid"] = pageInfo.OperateIdentifer;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的报警记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，记录数量=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetLiteAlarmRecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptObject obj3 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Columns", obj3);
            obj2.Add("Records", array);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                msg = this.GetLiteAlarmRecords(operateIdentifer, startIndex, count, ref obj3, ref array);
                obj2["Result"] = msg.Result;
                obj2["Message"] = msg.Message;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg GetLiteAlarmRecords(string operateIdentifer, string startIndex, string count, ref JavaScriptObject jsoColumns, ref JavaScriptArray jsaRecords)
        {
            PageInfo pageInfo = GlobalAppModule.GetPageInfo(operateIdentifer);
            if (pageInfo == null)
            {
                return new ResMsg(false, "查询操作超时");
            }
            pageInfo.LastOperateTime = DateTime.Now;
            int num = 0;
            try
            {
                num = Convert.ToInt32(startIndex);
            }
            catch
            {
                return new ResMsg(false, "参数startIndex格式不正确");
            }
            int recordsCount = 0;
            try
            {
                recordsCount = Convert.ToInt32(count);
            }
            catch
            {
                return new ResMsg(false, "参数count格式不正确");
            }
            if (num > pageInfo.RecordsCount)
            {
                return new ResMsg(false, "参数startIndex超范围");
            }
            if (((num + recordsCount) - 1) > pageInfo.RecordsCount)
            {
                recordsCount = (pageInfo.RecordsCount - num) + 1;
            }
            if (jsoColumns == null)
            {
                jsoColumns = new JavaScriptObject();
            }
            if (jsaRecords == null)
            {
                jsaRecords = new JavaScriptArray();
            }
            try
            {
                PageQueryCondition pageQueryCondition = pageInfo.GetPageQueryCondition(num, recordsCount);
                List<string> displayColumns = new List<string>(new string[] { "村庄", "设备", "设备编号", "类型", "状态", "开始时间", "结束时间", "时长" });
                for (int i = 0; i < displayColumns.Count; i++)
                {
                    JavaScriptObject obj2 = new JavaScriptObject();
                    obj2.Add("Field", ChinesePY.GetPinYinIndex(displayColumns[i]) + ((i + 1)).ToString());
                    obj2.Add("HeadText", displayColumns[i]);
                    jsoColumns.Add(displayColumns[i], obj2);
                }
                DateTime startTime = Convert.ToDateTime(pageInfo.QueryStartTime);
                DateTime endTime = Convert.ToDateTime(pageInfo.QueryEndTime);
                DataSet dsQuery = DBManageModule.AlarmDataQuery(pageQueryCondition.Ids, startTime, endTime, pageInfo.RecordType, true, false);
                //return this.LiteAlarmRecordsToJson(dsQuery, pageQueryCondition.HeadRemoveCount, pageQueryCondition.EndRemoveCount, displayColumns, ref jsaRecords);
                return this.AlarmOrEventRecordsToJson(dsQuery, pageQueryCondition.HeadRemoveCount, pageQueryCondition.EndRemoveCount, displayColumns, ref jsaRecords);
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
        }
    }
}
