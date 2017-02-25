using CarlosAg.ExcelXmlWriter;
using Common;
using DBUtility;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// DataReportService 的摘要说明
    /// </summary>
    [WebService(Name = "数据报表服务", Description = "提供时段统计、汇总统计、用水统计、售水统计等报表数据", Namespace = "http://www.data86.net/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class DataReportService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public DataReportService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户进行指定时间段内的售水汇总统计</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserIds=用水户ID,多个用户用','隔开，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'户充值':[object1,...,objectn],'合计':object,'ExcelURL':string}</p>")]
        public string GetSaleWaterSummaryReportByWaterUsers(string loginIdentifer, string waterUserIds, string startTime, string endTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("户充值", array);
            obj2.Add("合计", obj3);
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
            if ((waterUserIds == null) || (waterUserIds.Trim() == ""))
            {
                obj2["Message"] = "用水户ID为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "充值统计", Created = DateTime.Now }
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
            Worksheet worksheet = workbook.Worksheets.Add("充值统计");
            for (int i = 0; i < 6; i++)
            {
                worksheet.Table.Columns.Add(new WorksheetColumn(80));
            }
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("户充值统计（" + startTime + "～" + endTime + "）");
            cell.MergeAcross = 5;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("用水户").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("充值金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购水金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购水量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购电金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购电量").StyleID = "ColumnCaptionStyle";
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            string[] strArray = waterUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < strArray.Length; j++)
            {
                DataTable dtResult = new DataTable();
                ResMsg msg = WaterUserModule.GetSaleWaterSummaryReportByWaterUser(strArray[j], startTime, endTime, ref dtResult);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                WaterUser waterUserById = WaterUserModule.GetWaterUserById(long.Parse(strArray[j]));
                JavaScriptObject item = new JavaScriptObject();
                item["用水户ID"] = waterUserById.id;
                item["用水户"] = (waterUserById == null) ? "" : waterUserById.UserName;
                item["充值金额"] = (dtResult.Rows[0]["应收金额"].ToString() == "") ? "0" : dtResult.Rows[0]["应收金额"].ToString();
                item["购水金额"] = (dtResult.Rows[0]["售水金额"].ToString() == "") ? "0" : dtResult.Rows[0]["售水金额"].ToString();
                item["购水量"] = (dtResult.Rows[0]["售出水量"].ToString() == "") ? "0" : dtResult.Rows[0]["售出水量"].ToString();
                item["购电金额"] = (dtResult.Rows[0]["售电金额"].ToString() == "") ? "0" : dtResult.Rows[0]["售电金额"].ToString();
                item["购电量"] = (dtResult.Rows[0]["售出电量"].ToString() == "") ? "0" : dtResult.Rows[0]["售出电量"].ToString();
                num2 += Convert.ToDouble(item["充值金额"].ToString());
                num3 += Convert.ToDouble(item["购水金额"].ToString());
                num4 += Convert.ToDouble(item["购水量"].ToString());
                num5 += Convert.ToDouble(item["购电金额"].ToString());
                num6 += Convert.ToDouble(item["购电量"].ToString());
                array.Add(item);
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(item["用水户"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["充值金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购水金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购水量"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购电金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购电量"].ToString()).StyleID = "DefaultStyle";
            }
            obj3.Add("充值金额", num2.ToString());
            obj3.Add("购水金额", num3.ToString());
            obj3.Add("购水量", num4.ToString());
            obj3.Add("购电金额", num5.ToString());
            obj3.Add("购电量", num6.ToString());
            row = worksheet.Table.Rows.Add();
            row.Cells.Add("合计").StyleID = "DefaultStyle";
            row.Cells.Add(obj3["充值金额"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购水金额"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购水量"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购电金额"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购电量"].ToString()).StyleID = "DefaultStyle";
            try
            {
                string str = DateTime.Now.Ticks.ToString() + ".xls";
                workbook.Save(this.context.Server.MapPath("~/DataReport") + @"\" + str);
                obj2["ExcelURL"] = str;
                CommonUtil.RemoveFiles(this.context.Server.MapPath("~/DataReport"), ".xls");
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户进行指定时间段内的售水汇总统计</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserIds=用水户ID,多个用户用','隔开，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'户充值':[object1,...,objectn],'合计':object,'ExcelURL':string}</p>")]
        public string GetSaleWaterTimesSummaryReportByWaterUsers(string loginIdentifer, string waterUserIds, string baseTime, bool isAsc, uint saleWaterTimes)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("户充值", array);
            obj2.Add("合计", obj3);
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
            if ((waterUserIds == null) || (waterUserIds.Trim() == ""))
            {
                obj2["Message"] = "用水户ID为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "充值统计", Created = DateTime.Now }
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
            Worksheet worksheet = workbook.Worksheets.Add("充值统计");
            for (int i = 0; i < 6; i++)
            {
                worksheet.Table.Columns.Add(new WorksheetColumn(80));
            }
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("户充值统计（" + baseTime + (isAsc ? "向后" : "向前") + saleWaterTimes.ToString() + "次）");
            cell.MergeAcross = 5;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("用水户").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("充值金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购水金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购水量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购电金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购电量").StyleID = "ColumnCaptionStyle";
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            string[] strArray = waterUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < strArray.Length; j++)
            {
                DataTable dtResult = new DataTable();
                ResMsg msg = WaterUserModule.GetSaleWaterTimesSummaryReportByWaterUser(strArray[j], baseTime, isAsc, saleWaterTimes, ref dtResult);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                WaterUser waterUserById = WaterUserModule.GetWaterUserById(long.Parse(strArray[j]));
                JavaScriptObject item = new JavaScriptObject();
                item["用水户ID"] = waterUserById.id;
                item["用水户"] = (waterUserById == null) ? "" : waterUserById.UserName;
                item["充值金额"] = (dtResult.Rows[0]["应收金额"].ToString() == "") ? "0" : dtResult.Rows[0]["应收金额"].ToString();
                item["购水金额"] = (dtResult.Rows[0]["售水金额"].ToString() == "") ? "0" : dtResult.Rows[0]["售水金额"].ToString();
                item["购水量"] = (dtResult.Rows[0]["售出水量"].ToString() == "") ? "0" : dtResult.Rows[0]["售出水量"].ToString();
                item["购电金额"] = (dtResult.Rows[0]["售电金额"].ToString() == "") ? "0" : dtResult.Rows[0]["售电金额"].ToString();
                item["购电量"] = (dtResult.Rows[0]["售出电量"].ToString() == "") ? "0" : dtResult.Rows[0]["售出电量"].ToString();
                num2 += Convert.ToDouble(item["充值金额"].ToString());
                num3 += Convert.ToDouble(item["购水金额"].ToString());
                num4 += Convert.ToDouble(item["购水量"].ToString());
                num5 += Convert.ToDouble(item["购电金额"].ToString());
                num6 += Convert.ToDouble(item["购电量"].ToString());
                array.Add(item);
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(item["用水户"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["充值金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购水金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购水量"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购电金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购电量"].ToString()).StyleID = "DefaultStyle";
            }
            obj3.Add("充值金额", num2.ToString());
            obj3.Add("购水金额", num3.ToString());
            obj3.Add("购水量", num4.ToString());
            obj3.Add("购电金额", num5.ToString());
            obj3.Add("购电量", num6.ToString());
            row = worksheet.Table.Rows.Add();
            row.Cells.Add("合计").StyleID = "DefaultStyle";
            row.Cells.Add(obj3["充值金额"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购水金额"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购水量"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购电金额"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购电量"].ToString()).StyleID = "DefaultStyle";
            try
            {
                string str = DateTime.Now.Ticks.ToString() + ".xls";
                workbook.Save(this.context.Server.MapPath("~/DataReport") + @"\" + str);
                obj2["ExcelURL"] = str;
                CommonUtil.RemoveFiles(this.context.Server.MapPath("~/DataReport"), ".xls");
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按用水户进行用水汇总统计</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserIds=用水户ID，多个用户用','隔开，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'户用水':[object1,...,objectn],'合计':object,'ExcelURL':string}</p>")]
        public string GetUseWaterSummaryReportByWaterUsers(string loginIdentifer, string waterUserIds, string startTime, string endTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("户用水", array);
            obj2.Add("合计", obj3);
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
            if ((waterUserIds == null) || (waterUserIds.Trim() == ""))
            {
                obj2["Message"] = "用水户ID为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "用水统计", Created = DateTime.Now }
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
            Worksheet worksheet = workbook.Worksheets.Add("用水统计");
            for (int i = 0; i < 4; i++)
            {
                worksheet.Table.Columns.Add(new WorksheetColumn(80));
            }
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("户用水统计（" + startTime + "～" + endTime + "）");
            cell.MergeAcross = 3;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("用水户").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("灌溉时长").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用水量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用电量").StyleID = "ColumnCaptionStyle";
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            string[] strArray = waterUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < strArray.Length; j++)
            {
                DataTable dtResult = new DataTable();
                ResMsg msg = WaterUserModule.GetUseWaterSummaryReportByWaterUser(strArray[j], startTime, endTime, ref dtResult);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                double num6 = (dtResult.Rows[0]["灌溉时长"].ToString() == "") ? 0.0 : Convert.ToDouble(dtResult.Rows[0]["灌溉时长"].ToString());
                JavaScriptObject item = new JavaScriptObject();
                WaterUser waterUserById = WaterUserModule.GetWaterUserById(long.Parse(strArray[j]));
                item["用水户ID"] = waterUserById.id;
                item["用水户"] = (waterUserById == null) ? "" : waterUserById.UserName;
                item["灌溉时长"] = CommonUtil.GetTimeFromSecond(num6.ToString());
                item["用水量"] = (dtResult.Rows[0]["用水量"].ToString() == "") ? "0" : dtResult.Rows[0]["用水量"].ToString();
                item["用电量"] = (dtResult.Rows[0]["用电量"].ToString() == "") ? "0" : dtResult.Rows[0]["用电量"].ToString();
                array.Add(item);
                num2 += num6;
                num3 += Convert.ToDouble(item["用水量"].ToString());
                num4 += Convert.ToDouble(item["用电量"].ToString());
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(item["用水户"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["灌溉时长"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["用水量"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["用电量"].ToString()).StyleID = "DefaultStyle";
            }
            obj3.Add("灌溉时长", CommonUtil.GetTimeFromSecond(num2.ToString()));
            obj3.Add("用水量", num3.ToString());
            obj3.Add("用电量", num4.ToString());
            row = worksheet.Table.Rows.Add();
            row.Cells.Add("合计").StyleID = "DefaultStyle";
            row.Cells.Add(obj3["灌溉时长"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["用水量"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["用电量"].ToString()).StyleID = "DefaultStyle";
            try
            {
                string str = DateTime.Now.Ticks.ToString() + ".xls";
                workbook.Save(this.context.Server.MapPath("~/DataReport") + @"\" + str);
                obj2["ExcelURL"] = str;
                CommonUtil.RemoveFiles(this.context.Server.MapPath("~/DataReport"), ".xls");
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>户用水按次统计报表</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserIds=用水户ID，多个用户用','隔开，baseTime=基准时间，isAsc=true:向后,false:向前，useWaterTimes=次数<br/>返回数据格式：{'Result':bool,'Message':string,'户用水':[object1,...,objectn],'合计':object,'ExcelURL':string}</p>")]
        public string GetUseWaterTimesSummaryReportByWaterUsers(string loginIdentifer, string waterUserIds, string baseTime, bool isAsc, uint useWaterTimes)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("户用水", array);
            obj2.Add("合计", obj3);
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
            if ((waterUserIds == null) || (waterUserIds.Trim() == ""))
            {
                obj2["Message"] = "用水户ID为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "用水统计", Created = DateTime.Now }
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
            Worksheet worksheet = workbook.Worksheets.Add("用水统计");
            for (int i = 0; i < 4; i++)
            {
                worksheet.Table.Columns.Add(new WorksheetColumn(80));
            }
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add(string.Concat(new object[] { "户用水统计（", baseTime, isAsc ? "向后" : "向前", useWaterTimes, "次）" }));
            cell.MergeAcross = 3;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("用水户").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("灌溉时长").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用水量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用电量").StyleID = "ColumnCaptionStyle";
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            string[] strArray = waterUserIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < strArray.Length; j++)
            {
                DataTable dtResult = new DataTable();
                ResMsg msg = WaterUserModule.GetUseWaterTimesSummaryReportByWaterUser(strArray[j], baseTime, isAsc, useWaterTimes, ref dtResult);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                double num6 = (dtResult.Rows[0]["灌溉时长"].ToString() == "") ? 0.0 : Convert.ToDouble(dtResult.Rows[0]["灌溉时长"].ToString());
                JavaScriptObject item = new JavaScriptObject();
                WaterUser waterUserById = WaterUserModule.GetWaterUserById(long.Parse(strArray[j]));
                item["用水户ID"] = waterUserById.id;
                item["用水户"] = (waterUserById == null) ? "" : waterUserById.UserName;
                item["灌溉时长"] = CommonUtil.GetTimeFromSecond(num6.ToString());
                item["用水量"] = (dtResult.Rows[0]["用水量"].ToString() == "") ? "0" : dtResult.Rows[0]["用水量"].ToString();
                item["用电量"] = (dtResult.Rows[0]["用电量"].ToString() == "") ? "0" : dtResult.Rows[0]["用电量"].ToString();
                array.Add(item);
                num2 += num6;
                num3 += Convert.ToDouble(item["用水量"].ToString());
                num4 += Convert.ToDouble(item["用电量"].ToString());
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(item["用水户"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["灌溉时长"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["用水量"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["用电量"].ToString()).StyleID = "DefaultStyle";
            }
            obj3.Add("灌溉时长", CommonUtil.GetTimeFromSecond(num2.ToString()));
            obj3.Add("用水量", num3.ToString());
            obj3.Add("用电量", num4.ToString());
            row = worksheet.Table.Rows.Add();
            row.Cells.Add("合计").StyleID = "DefaultStyle";
            row.Cells.Add(obj3["灌溉时长"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["用水量"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["用电量"].ToString()).StyleID = "DefaultStyle";
            try
            {
                string str = DateTime.Now.Ticks.ToString() + ".xls";
                workbook.Save(this.context.Server.MapPath("~/DataReport") + @"\" + str);
                obj2["ExcelURL"] = str;
                CommonUtil.RemoveFiles(this.context.Server.MapPath("~/DataReport"), ".xls");
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>户用水时段统计报表（年报表、月报表、日报表）</span><br/><p>输入参数：loginIdentifer=登录用户标识，waterUserId=用水户ID，reportType=报表类型，reportTime=报表时间<br/>返回数据格式：{'Result':bool,'Message':string,'ReportDatas':[object1,...,objectn],'TotalData':object,'ExcelURL':string}</p>")]
        public string GetUseWaterPeriodReportByWaterUser(string loginIdentifer, string waterUserId, string reportType, string reportTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("ReportDatas", array);
            obj2.Add("TotalData", obj3);
            obj2.Add("ExcelURL", "");
            try
            {
                string str;
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, false);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((waterUserId == null) || (waterUserId.Trim() == ""))
                {
                    obj2["Message"] = "用水户ID为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((reportType == null) || (reportType.Trim() == ""))
                {
                    obj2["Message"] = "报表类型为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((reportTime == null) || (reportTime.Trim() == ""))
                {
                    obj2["Message"] = "报表时间为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                DateTime now = DateTime.Now;
                try
                {
                    string str3 = reportType;
                    if (str3 == null)
                    {
                        goto Label_018F;
                    }
                    if (!(str3 == "日报表"))
                    {
                        if (str3 == "月报表")
                        {
                            goto Label_0165;
                        }
                        if (str3 == "年报表")
                        {
                            goto Label_017A;
                        }
                        goto Label_018F;
                    }
                    now = Convert.ToDateTime(reportTime);
                    goto Label_01CF;
                Label_0165:
                    now = Convert.ToDateTime(reportTime + "-01");
                    goto Label_01CF;
                Label_017A:
                    now = Convert.ToDateTime(reportTime + "-01-01");
                    goto Label_01CF;
                Label_018F:
                    obj2["Message"] = "不支持" + reportType;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                catch
                {
                    obj2["Message"] = "报表时间格式不正确";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            Label_01CF:
                str = "";
                msg = this.GetUseWaterPeriodReportByWaterUser(waterUserId, reportType, now, ref array, ref obj3, ref str);
                obj2["Result"] = msg.Result;
                obj2["Message"] = msg.Message;
                obj2["ExcelURL"] = str;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg GetUseWaterPeriodReportByWaterUser(string waterUserId, string reportType, DateTime dtReportTime, ref JavaScriptArray jsaReport, ref JavaScriptObject jsoTotal, ref string excelUrl)
        {
            WaterUser wui = WaterUserModule.GetWaterUserById(long.Parse(waterUserId));
            WorksheetCell cell;
            if (wui == null)
            {
                return new ResMsg(false, "用水户不存在");
            }
            DataTable dtResult = null;
            ResMsg msg = GetUseWaterPeriodReportByWaterUser(waterUserId, reportType, dtReportTime, ref dtResult);
            if (!msg.Result)
            {
                return msg;
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "用水统计", Created = DateTime.Now }
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
            Worksheet worksheet = workbook.Worksheets.Add("用水统计");
            for (int i = 0; i < 4; i++)
            {
                worksheet.Table.Columns.Add(new WorksheetColumn(80));
            }
            WorksheetRow row = worksheet.Table.Rows.Add();
            string text = "";
            string str2 = "";
            string str5 = reportType;
            if (str5 != null)
            {
                if (!(str5 == "日报表"))
                {
                    if (str5 == "月报表")
                    {
                        text = "户用水统计（" + wui.UserName + "，" + dtReportTime.ToString("yyyy年M月") + "）";
                        str2 = "日期";
                        goto Label_0322;
                    }
                    if (str5 == "年报表")
                    {
                        text = string.Concat(new object[] { "户用水统计（", wui.UserName, "，", dtReportTime.Year, "年）" });
                        str2 = "月份";
                        goto Label_0322;
                    }
                }
                else
                {
                    text = "户用水统计（" + wui.UserName + "，" + dtReportTime.ToString("yyyy年M月d日") + "）";
                    str2 = "小时";
                    goto Label_0322;
                }
            }
            return new ResMsg(false, "不支持" + reportType);
        Label_0322:
            cell = row.Cells.Add(text);
            cell.MergeAcross = 3;
            cell.StyleID = "HeaderStyle";
            row = worksheet.Table.Rows.Add();
            row.Cells.Add(str2).StyleID = "ColumnCaptionStyle";
            row.Cells.Add("灌溉时长").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用水量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用电量").StyleID = "ColumnCaptionStyle";
            for (int j = 0; j < (dtResult.Rows.Count - 1); j++)
            {
                JavaScriptObject item = new JavaScriptObject();
                item.Add("用水户ID", waterUserId);
                item.Add(str2, dtResult.Rows[j][str2].ToString());
                item.Add("起始时间", dtResult.Rows[j]["起始时间"].ToString());
                item.Add("灌溉时长", dtResult.Rows[j]["灌溉时长"].ToString());
                item.Add("用水量", dtResult.Rows[j]["用水量"].ToString());
                item.Add("用电量", dtResult.Rows[j]["用电量"].ToString());
                jsaReport.Add(item);
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(item[str2].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["灌溉时长"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["用水量"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["用电量"].ToString()).StyleID = "DefaultStyle";
            }
            DataRow row2 = dtResult.Rows[dtResult.Rows.Count - 1];
            row = worksheet.Table.Rows.Add();
            row.Cells.Add(row2[str2].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(row2["灌溉时长"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(row2["用水量"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(row2["用电量"].ToString()).StyleID = "DefaultStyle";
            jsoTotal.Add(str2, row2[str2].ToString());
            jsoTotal.Add("灌溉时长", row2["灌溉时长"].ToString());
            jsoTotal.Add("用水量", row2["用水量"].ToString());
            jsoTotal.Add("用电量", row2["用电量"].ToString());
            try
            {
                string str3 = DateTime.Now.Ticks.ToString() + ".xls";
                workbook.Save(this.context.Server.MapPath("~/DataReport") + @"\" + str3);
                excelUrl = str3;
                CommonUtil.RemoveFiles(this.context.Server.MapPath("~/DataReport"), ".xls");
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
            return new ResMsg(true, "");
        }

        private ResMsg GetUseWaterPeriodReportByWaterUser(string waterUserId, string reportType, DateTime dtReportTime, ref DataTable dtResult)
        {
            DateTime time;
            string columnName = "";
            string str3 = "";
            int num = 0;
            string str4 = reportType;
            if (str4 != null)
            {
                if (!(str4 == "日报表"))
                {
                    if (str4 == "月报表")
                    {
                        columnName = "日期";
                        str3 = "日";
                        num = DateTime.DaysInMonth(dtReportTime.Year, dtReportTime.Month);
                        time = Convert.ToDateTime(dtReportTime.ToString("yyyy-MM-01"));
                        goto Label_00E9;
                    }
                    if (str4 == "年报表")
                    {
                        columnName = "月份";
                        str3 = "月";
                        num = 12;
                        time = Convert.ToDateTime(dtReportTime.Year.ToString() + "-01-01");
                        goto Label_00E9;
                    }
                }
                else
                {
                    columnName = "小时";
                    str3 = "时";
                    num = 0x18;
                    time = Convert.ToDateTime(dtReportTime.ToString("yyyy-MM-dd"));
                    goto Label_00E9;
                }
            }
            return new ResMsg(false, "不支持" + reportType);
        Label_00E9:
            dtResult = new DataTable();
            dtResult.Columns.Add(columnName);
            dtResult.Columns.Add("起始时间");
            dtResult.Columns.Add("灌溉时长");
            dtResult.Columns.Add("用水量");
            dtResult.Columns.Add("用电量");
            int num2 = 0;
            int num3 = 0;
            double num4 = 0.0;
            double num5 = 0.0;
            DataTable table = null;
            DateTime now = DateTime.Now;
            for (int i = 1; i <= num; i++)
            {
                string str5 = reportType;
                if (str5 != null)
                {
                    if (!(str5 == "日报表"))
                    {
                        if (str5 == "月报表")
                        {
                            goto Label_01BA;
                        }
                        if (str5 == "年报表")
                        {
                            goto Label_01CE;
                        }
                    }
                    else
                    {
                        now = time.AddHours(1.0);
                    }
                }
                goto Label_01D8;
            Label_01BA:
                now = time.AddDays(1.0);
                goto Label_01D8;
            Label_01CE:
                now = time.AddMonths(1);
            Label_01D8: ;
            try
            {
                string sql = "select sum(Duration) as 灌溉时长, sum(WaterUsed) as 用水量,sum(ElectricUsed) as 用电量 from CardUserWaterLog where WateUserId='" + waterUserId + "' and EndTime>='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "' and EndTime<'" + now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                //table = DBManager.Query("select sum(灌溉时长) as 灌溉时长, sum(本次用水量) as 用水量,sum(本次用电量) as 用电量 from 用水记录表 where 用水户ID='" + waterUserId + "' and 关泵时间>='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "' and 关泵时间<'" + now.ToString("yyyy-MM-dd HH:mm:ss") + "'", out strMsg);;
                table = DbHelperSQL.Query(sql).Tables[0];
            }
            catch (Exception ex)
            {
                dtResult = null;
                return new ResMsg(false, ex.Message);
            }
                dtResult.Rows.Add(dtResult.NewRow());
                num2 = dtResult.Rows.Count - 1;
                dtResult.Rows[num2][columnName] = i + str3;
                dtResult.Rows[num2]["起始时间"] = time.ToString("yyyy-MM-dd HH:mm:ss");
                dtResult.Rows[num2]["灌溉时长"] = (table.Rows[0]["灌溉时长"].ToString() == "") ? "0" : table.Rows[0]["灌溉时长"].ToString();
                dtResult.Rows[num2]["用水量"] = (table.Rows[0]["用水量"].ToString() == "") ? "0" : table.Rows[0]["用水量"].ToString();
                dtResult.Rows[num2]["用电量"] = (table.Rows[0]["用电量"].ToString() == "") ? "0" : table.Rows[0]["用电量"].ToString();
                num3 += Convert.ToInt32(dtResult.Rows[num2]["灌溉时长"].ToString());
                num4 += Convert.ToDouble(dtResult.Rows[num2]["用水量"].ToString());
                num5 += Convert.ToDouble(dtResult.Rows[num2]["用电量"].ToString());
                dtResult.Rows[num2]["灌溉时长"] = CommonUtil.GetTimeFromSecond(dtResult.Rows[num2]["灌溉时长"].ToString());
                time = now;
            }
            dtResult.Rows.Add(dtResult.NewRow());
            num2 = dtResult.Rows.Count - 1;
            dtResult.Rows[num2][columnName] = "合计";
            dtResult.Rows[num2]["起始时间"] = "";
            dtResult.Rows[num2]["灌溉时长"] = CommonUtil.GetTimeFromSecond(num3.ToString());
            dtResult.Rows[num2]["用水量"] = Math.Round(num4, 2).ToString();
            dtResult.Rows[num2]["用电量"] = Math.Round(num5, 2).ToString();
            return new ResMsg(true, "");
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按村进行指定时间段内的售水汇总统计</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageIds=村ID,多个村用','隔开，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'村充值':[object1,...,objectn],'合计':object,'ExcelURL':string}</p>")]
        public string GetSaleWaterSummaryReportByVillages(string loginIdentifer, string villageIds, string startTime, string endTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("村充值", array);
            obj2.Add("合计", obj3);
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
            if ((villageIds == null) || (villageIds.Trim() == ""))
            {
                obj2["Message"] = "村庄ID为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "充值统计", Created = DateTime.Now }
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
            Worksheet worksheet = workbook.Worksheets.Add("充值统计");
            for (int i = 0; i < 6; i++)
            {
                worksheet.Table.Columns.Add(new WorksheetColumn(80));
            }
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("村充值统计（" + startTime + "～" + endTime + "）");
            cell.MergeAcross = 5;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("村庄").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("充值金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购水金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购水量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购电金额").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("购电量").StyleID = "ColumnCaptionStyle";
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            string[] strArray = villageIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < strArray.Length; j++)
            {
                DataTable dtResult = new DataTable();
                ResMsg msg = WaterUserModule.GetSaleWaterSummaryReportByVillage(strArray[j], startTime, endTime, ref dtResult);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                JavaScriptObject item = new JavaScriptObject();
                item["村庄"] = DistrictModule.GetDistrictName(long.Parse(strArray[j]));
                item["充值金额"] = (dtResult.Rows[0]["应收金额"].ToString() == "") ? "0" : dtResult.Rows[0]["应收金额"].ToString();
                item["购水金额"] = (dtResult.Rows[0]["售水金额"].ToString() == "") ? "0" : dtResult.Rows[0]["售水金额"].ToString();
                item["购水量"] = (dtResult.Rows[0]["售出水量"].ToString() == "") ? "0" : dtResult.Rows[0]["售出水量"].ToString();
                item["购电金额"] = (dtResult.Rows[0]["售电金额"].ToString() == "") ? "0" : dtResult.Rows[0]["售电金额"].ToString();
                item["购电量"] = (dtResult.Rows[0]["售出电量"].ToString() == "") ? "0" : dtResult.Rows[0]["售出电量"].ToString();
                num2 += Convert.ToDouble(item["充值金额"].ToString());
                num3 += Convert.ToDouble(item["购水金额"].ToString());
                num4 += Convert.ToDouble(item["购水量"].ToString());
                num5 += Convert.ToDouble(item["购电金额"].ToString());
                num6 += Convert.ToDouble(item["购电量"].ToString());
                array.Add(item);
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(item["村庄"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["充值金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购水金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购水量"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购电金额"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["购电量"].ToString()).StyleID = "DefaultStyle";
            }
            obj3.Add("充值金额", num2.ToString());
            obj3.Add("购水金额", num3.ToString());
            obj3.Add("购水量", num4.ToString());
            obj3.Add("购电金额", num5.ToString());
            obj3.Add("购电量", num6.ToString());
            row = worksheet.Table.Rows.Add();
            row.Cells.Add("合计").StyleID = "DefaultStyle";
            row.Cells.Add(obj3["充值金额"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购水金额"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购水量"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购电金额"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["购电量"].ToString()).StyleID = "DefaultStyle";
            try
            {
                string str = DateTime.Now.Ticks.ToString() + ".xls";
                workbook.Save(this.context.Server.MapPath("~/DataReport") + @"\" + str);
                obj2["ExcelURL"] = str;
                CommonUtil.RemoveFiles(this.context.Server.MapPath("~/DataReport"), ".xls");
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>村用水时段统计报表（年报表、月报表、日报表）</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageId=村ID，reportType=报表类型，reportTime=报表时间<br/>返回数据格式：{'Result':bool,'Message':string,'ReportDatas':[object1,...,objectn],'TotalData':object,'ExcelURL':string}</p>")]
        public string GetWaterUsersUseWaterSummaryReportByVillage(string loginIdentifer, string villageId, string reportType, string reportStartTime, string reportEndTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("ReportDatas", array);
            try
            {
                DateTime time;
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((villageId == null) || (villageId.Trim() == ""))
                {
                    obj2["Message"] = "村庄ID为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((reportType == null) || (reportType.Trim() == ""))
                {
                    obj2["Message"] = "报表类型为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                string str3 = reportType;
                if (str3 == null)
                {
                    goto Label_01B0;
                }
                if ((!(str3 == "时") && !(str3 == "日")) && !(str3 == "月"))
                {
                    if (str3 == "任意")
                    {
                        goto Label_0148;
                    }
                    goto Label_01B0;
                }
                if ((reportStartTime != null) && !(reportStartTime.Trim() == ""))
                {
                    goto Label_01B0;
                }
                obj2["Message"] = "报表时间不能为空";
                return JavaScriptConvert.SerializeObject(obj2);
            Label_0148:
                if ((reportStartTime == null) || (reportStartTime.Trim() == ""))
                {
                    obj2["Message"] = "报表起始时间不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((reportEndTime == null) || (reportEndTime.Trim() == ""))
                {
                    obj2["Message"] = "报表结束时间不能为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            Label_01B0:
                time = DateTime.Now;
                DateTime now = DateTime.Now;
                try
                {
                    time = Convert.ToDateTime(reportStartTime);
                    string str4 = reportType;
                    if (str4 == null)
                    {
                        goto Label_0244;
                    }
                    if (!(str4 == "时"))
                    {
                        if (str4 == "日")
                        {
                            goto Label_0219;
                        }
                        if (str4 == "月")
                        {
                            goto Label_022D;
                        }
                        if (str4 == "任意")
                        {
                            goto Label_0239;
                        }
                        goto Label_0244;
                    }
                    now = time.AddHours(1.0);
                    goto Label_0287;
                Label_0219:
                    now = time.AddDays(1.0);
                    goto Label_0287;
                Label_022D:
                    now = time.AddMonths(1);
                    goto Label_0287;
                Label_0239:
                    now = Convert.ToDateTime(reportEndTime);
                    goto Label_0287;
                Label_0244:
                    obj2["Message"] = "不支持" + reportType;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                catch
                {
                    obj2["Message"] = "报表时间格式不正确";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            Label_0287:
                if (DistrictModule.ReturnDistrictInfo(long.Parse(villageId)) == null)
                {
                    obj2["Message"] = "村庄不存在";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                List<string> liWaterUserIds = new List<string>();
                msg = WaterUserModule.GetWaterUserIdsByManagerId(villageId, true, ref liWaterUserIds);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                DataTable dtResult = null;
                foreach (string str in liWaterUserIds)
                {
                    msg = WaterUserModule.GetUseWaterSummaryReportByWaterUser(str, time.ToString("yyyy-MM-dd HH:mm:ss"), now.ToString("yyyy-MM-dd HH:mm:ss"), ref dtResult);
                    if (!msg.Result)
                    {
                        obj2["Message"] = msg.Message;
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                    if ((!(dtResult.Rows[0]["灌溉时长"].ToString() == "") || !(dtResult.Rows[0]["用水量"].ToString() == "")) || !(dtResult.Rows[0]["用电量"].ToString() == ""))
                    {
                        double num = (dtResult.Rows[0]["灌溉时长"].ToString() == "") ? 0.0 : Convert.ToDouble(dtResult.Rows[0]["灌溉时长"].ToString());
                        JavaScriptObject item = new JavaScriptObject();
                        WaterUser waterUserById = WaterUserModule.GetWaterUserById(long.Parse(str));
                        item["用水户ID"] = waterUserById.id;
                        item["用水户"] = (waterUserById == null) ? "" : waterUserById.UserName;
                        item["灌溉时长"] = CommonUtil.GetTimeFromSecond(num.ToString());
                        item["用水量"] = (dtResult.Rows[0]["用水量"].ToString() == "") ? "0" : dtResult.Rows[0]["用水量"].ToString();
                        item["用电量"] = (dtResult.Rows[0]["用电量"].ToString() == "") ? "0" : dtResult.Rows[0]["用电量"].ToString();
                        array.Add(item);
                    }
                }
                obj2["Result"] = msg.Result;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按村进行用水汇总统计</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageIds=村ID，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'村用水':[object1,...,objectn],'合计':object,'ExcelURL':string}</p>")]
        public string GetUseWaterSummaryReportByVillages(string loginIdentifer, string villageIds, string startTime, string endTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("村用水", array);
            obj2.Add("合计", obj3);
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
            if ((villageIds == null) || (villageIds.Trim() == ""))
            {
                obj2["Message"] = "村庄ID为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "用水统计", Created = DateTime.Now }
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
            Worksheet worksheet = workbook.Worksheets.Add("用水统计");
            for (int i = 0; i < 4; i++)
            {
                worksheet.Table.Columns.Add(new WorksheetColumn(80));
            }
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("村用水统计（" + startTime + "～" + endTime + "）");
            cell.MergeAcross = 3;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("村庄").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("灌溉时长").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用水量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用电量").StyleID = "ColumnCaptionStyle";
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            string[] strArray = villageIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < strArray.Length; j++)
            {
                DataTable dtResult = new DataTable();
                ResMsg msg = WaterUserModule.GetUseWaterSummaryReportByVillage(strArray[j], startTime, endTime, ref dtResult);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                double num6 = (dtResult.Rows[0]["灌溉时长"].ToString() == "") ? 0.0 : Convert.ToDouble(dtResult.Rows[0]["灌溉时长"].ToString());
                JavaScriptObject item = new JavaScriptObject();
                item["村庄ID"] = strArray[j];
                item["村庄"] = DistrictModule.GetDistrictName(long.Parse(strArray[j]));
                item["灌溉时长"] = CommonUtil.GetTimeFromSecond(num6.ToString());
                item["用水量"] = (dtResult.Rows[0]["用水量"].ToString() == "") ? "0" : dtResult.Rows[0]["用水量"].ToString();
                item["用电量"] = (dtResult.Rows[0]["用电量"].ToString() == "") ? "0" : dtResult.Rows[0]["用电量"].ToString();
                array.Add(item);
                num2 += num6;
                num3 += Convert.ToDouble(item["用水量"]);
                num4 += Convert.ToDouble(item["用电量"]);
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(item["村庄"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["灌溉时长"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["用水量"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["用电量"].ToString()).StyleID = "DefaultStyle";
            }
            obj3.Add("灌溉时长", CommonUtil.GetTimeFromSecond(num2.ToString()));
            obj3.Add("用水量", num3.ToString());
            obj3.Add("用电量", num4.ToString());
            row = worksheet.Table.Rows.Add();
            row.Cells.Add("合计").StyleID = "DefaultStyle";
            row.Cells.Add(obj3["灌溉时长"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["用水量"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["用电量"].ToString()).StyleID = "DefaultStyle";
            try
            {
                string str = DateTime.Now.Ticks.ToString() + ".xls";
                workbook.Save(this.context.Server.MapPath("~/DataReport") + @"\" + str);
                obj2["ExcelURL"] = str;
                CommonUtil.RemoveFiles(this.context.Server.MapPath("~/DataReport"), ".xls");
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>村用水时段统计报表（年报表、月报表、日报表）</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageId=村ID，reportType=报表类型，reportTime=报表时间<br/>返回数据格式：{'Result':bool,'Message':string,'ReportDatas':[object1,...,objectn],'TotalData':object,'ExcelURL':string}</p>")]
        public string GetUseWaterPeriodReportByVillage(string loginIdentifer, string villageId, string reportType, string reportTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("ReportDatas", array);
            obj2.Add("TotalData", obj3);
            obj2.Add("ExcelURL", "");
            try
            {
                string str;
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((villageId == null) || (villageId.Trim() == ""))
                {
                    obj2["Message"] = "村庄ID为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((reportType == null) || (reportType.Trim() == ""))
                {
                    obj2["Message"] = "报表类型为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if ((reportTime == null) || (reportTime.Trim() == ""))
                {
                    obj2["Message"] = "报表时间为空";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                DateTime now = DateTime.Now;
                try
                {
                    string str3 = reportType;
                    if (str3 == null)
                    {
                        goto Label_018F;
                    }
                    if (!(str3 == "日报表"))
                    {
                        if (str3 == "月报表")
                        {
                            goto Label_0165;
                        }
                        if (str3 == "年报表")
                        {
                            goto Label_017A;
                        }
                        goto Label_018F;
                    }
                    now = Convert.ToDateTime(reportTime);
                    goto Label_01CF;
                Label_0165:
                    now = Convert.ToDateTime(reportTime + "-01");
                    goto Label_01CF;
                Label_017A:
                    now = Convert.ToDateTime(reportTime + "-01-01");
                    goto Label_01CF;
                Label_018F:
                    obj2["Message"] = "不支持" + reportType;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                catch
                {
                    obj2["Message"] = "报表时间格式不正确";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            Label_01CF:
                str = "";
                msg = this.GetUseWaterPeriodReportByVillage(villageId, reportType, now, ref array, ref obj3, ref str);
                obj2["Result"] = msg.Result;
                obj2["Message"] = msg.Message;
                obj2["ExcelURL"] = str;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg GetUseWaterPeriodReportByVillage(string villageId, string reportType, DateTime dtReportTime, ref JavaScriptArray jsaReport, ref JavaScriptObject jsoTotal, ref string excelUrl)
        {
            WorksheetCell cell;
            District node = DistrictModule.ReturnDistrictInfo(long.Parse(villageId));
            if (node == null)
            {
                return new ResMsg(false, "村庄不存在");
            }
            DataTable dtResult = null;
            ResMsg msg = GetUseWaterPeriodReportByVillage(villageId, reportType, dtReportTime, ref dtResult);
            if (!msg.Result)
            {
                return msg;
            }
            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "用水统计", Created = DateTime.Now }
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
            Worksheet worksheet = workbook.Worksheets.Add("用水统计");
            for (int i = 0; i < 4; i++)
            {
                worksheet.Table.Columns.Add(new WorksheetColumn(80));
            }
            WorksheetRow row = worksheet.Table.Rows.Add();
            string text = "";
            string str2 = "";
            string str5 = reportType;
            if (str5 != null)
            {
                if (!(str5 == "日报表"))
                {
                    if (str5 == "月报表")
                    {
                        text = "村用水统计（" + node.DistrictName + "，" + dtReportTime.ToString("yyyy年M月") + "）";
                        str2 = "日期";
                        goto Label_0319;
                    }
                    if (str5 == "年报表")
                    {
                        text = string.Concat(new object[] { "村用水统计（", node.DistrictName, "，", dtReportTime.Year, "年）" });
                        str2 = "月份";
                        goto Label_0319;
                    }
                }
                else
                {
                    text = "村用水统计（" + node.DistrictName + "，" + dtReportTime.ToString("yyyy年M月d日") + "）";
                    str2 = "小时";
                    goto Label_0319;
                }
            }
            return new ResMsg(false, "不支持" + reportType);
        Label_0319:
            cell = row.Cells.Add(text);
            cell.MergeAcross = 3;
            cell.StyleID = "HeaderStyle";
            row = worksheet.Table.Rows.Add();
            row.Cells.Add(str2).StyleID = "ColumnCaptionStyle";
            row.Cells.Add("灌溉时长").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用水量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用电量").StyleID = "ColumnCaptionStyle";
            for (int j = 0; j < (dtResult.Rows.Count - 1); j++)
            {
                JavaScriptObject item = new JavaScriptObject();
                item.Add("村庄ID", villageId);
                item.Add(str2, dtResult.Rows[j][str2].ToString());
                item.Add("起始时间", dtResult.Rows[j]["起始时间"].ToString());
                item.Add("灌溉时长", dtResult.Rows[j]["灌溉时长"].ToString());
                item.Add("用水量", dtResult.Rows[j]["用水量"].ToString());
                item.Add("用电量", dtResult.Rows[j]["用电量"].ToString());
                jsaReport.Add(item);
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(item[str2].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["灌溉时长"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["用水量"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["用电量"].ToString()).StyleID = "DefaultStyle";
            }
            DataRow row2 = dtResult.Rows[dtResult.Rows.Count - 1];
            row = worksheet.Table.Rows.Add();
            row.Cells.Add(row2[str2].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(row2["灌溉时长"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(row2["用水量"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(row2["用电量"].ToString()).StyleID = "DefaultStyle";
            jsoTotal.Add(str2, row2[str2].ToString());
            jsoTotal.Add("灌溉时长", row2["灌溉时长"].ToString());
            jsoTotal.Add("用水量", row2["用水量"].ToString());
            jsoTotal.Add("用电量", row2["用电量"].ToString());
            try
            {
                string str3 = DateTime.Now.Ticks.ToString() + ".xls";
                workbook.Save(this.context.Server.MapPath("~/DataReport") + @"\" + str3);
                excelUrl = str3;
                CommonUtil.RemoveFiles(this.context.Server.MapPath("~/DataReport"), ".xls");
            }
            catch (Exception exception)
            {
                return new ResMsg(false, exception.Message);
            }
            return new ResMsg(true, "");
        }

        public static ResMsg GetUseWaterPeriodReportByVillage(string villageId, string reportType, DateTime dtReportTime, ref DataTable dtResult)
        {
            DateTime time;
            List<string> list;
            string strMsg = "";
            string columnName = "";
            string str3 = "";
            int num = 0;
            string str5 = reportType;
            if (str5 != null)
            {
                if (!(str5 == "日报表"))
                {
                    if (str5 == "月报表")
                    {
                        columnName = "日期";
                        str3 = "日";
                        num = DateTime.DaysInMonth(dtReportTime.Year, dtReportTime.Month);
                        time = Convert.ToDateTime(dtReportTime.ToString("yyyy-MM-01"));
                        goto Label_00E9;
                    }
                    if (str5 == "年报表")
                    {
                        columnName = "月份";
                        str3 = "月";
                        num = 12;
                        time = Convert.ToDateTime(dtReportTime.Year.ToString() + "-01-01");
                        goto Label_00E9;
                    }
                }
                else
                {
                    columnName = "小时";
                    str3 = "时";
                    num = 0x18;
                    time = Convert.ToDateTime(dtReportTime.ToString("yyyy-MM-dd"));
                    goto Label_00E9;
                }
            }
            return new ResMsg(false, "不支持" + reportType);
        Label_00E9:
            list = new List<string>();
            ResMsg msg = WaterUserModule.GetWaterUserIdsByManagerId(villageId, true, ref list);
            if (!msg.Result)
            {
                return msg;
            }
            bool flag = true;
            StringBuilder builder = new StringBuilder();
            foreach (string str4 in list)
            {
                if (flag)
                {
                    builder.Append(str4);
                    flag = false;
                }
                else
                {
                    builder.Append("," + str4);
                }
            }
            if (builder.ToString() == "")
            {
                builder.Append("-1");
            }
            dtResult = new DataTable();
            dtResult.Columns.Add(columnName);
            dtResult.Columns.Add("起始时间");
            dtResult.Columns.Add("灌溉时长");
            dtResult.Columns.Add("用水量");
            dtResult.Columns.Add("用电量");
            int num2 = 0;
            int num3 = 0;
            double num4 = 0.0;
            double num5 = 0.0;
            DataTable table = null;
            DateTime now = DateTime.Now;
            for (int i = 1; i <= num; i++)
            {
                string str6 = reportType;
                if (str6 != null)
                {
                    if (!(str6 == "日报表"))
                    {
                        if (str6 == "月报表")
                        {
                            goto Label_0236;
                        }
                        if (str6 == "年报表")
                        {
                            goto Label_024A;
                        }
                    }
                    else
                    {
                        now = time.AddHours(1.0);
                    }
                }
                goto Label_0254;
            Label_0236:
                now = time.AddDays(1.0);
                goto Label_0254;
            Label_024A:
                now = time.AddMonths(1);
            Label_0254: ;
                //table = DBManager.Query("select sum(灌溉时长) as 灌溉时长, sum(本次用水量) as 用水量,sum(本次用电量) as 用电量 from 用水记录表 where 用水户ID in (" + builder.ToString() + ") and 关泵时间>='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "' and 关泵时间<'" + now.ToString("yyyy-MM-dd HH:mm:ss") + "'", out strMsg);
            string strSql = "select sum(Duration) as 灌溉时长, sum(WaterUsed) as 用水量,sum(ElectricUsed) as 用电量 from CardUserWaterLog where WateUserId in (" + builder.ToString() + ") and EndTime>='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "' and EndTime<'" + now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                try
                {
                    table = DbHelperSQL.Query(strSql).Tables[0]; 
                }
                catch (Exception ex)
                {
                    dtResult = null;
                    return new ResMsg(false, ex.Message);
                }
                dtResult.Rows.Add(dtResult.NewRow());
                num2 = dtResult.Rows.Count - 1;
                dtResult.Rows[num2][columnName] = i + str3;
                dtResult.Rows[num2]["起始时间"] = time.ToString("yyyy-MM-dd HH:mm:ss");
                dtResult.Rows[num2]["灌溉时长"] = (table.Rows[0]["灌溉时长"].ToString() == "") ? "0" : table.Rows[0]["灌溉时长"].ToString();
                dtResult.Rows[num2]["用水量"] = (table.Rows[0]["用水量"].ToString() == "") ? "0" : table.Rows[0]["用水量"].ToString();
                dtResult.Rows[num2]["用电量"] = (table.Rows[0]["用电量"].ToString() == "") ? "0" : table.Rows[0]["用电量"].ToString();
                num3 += Convert.ToInt32(dtResult.Rows[num2]["灌溉时长"].ToString());
                num4 += Convert.ToDouble(dtResult.Rows[num2]["用水量"].ToString());
                num5 += Convert.ToDouble(dtResult.Rows[num2]["用电量"].ToString());
                dtResult.Rows[num2]["灌溉时长"] = CommonUtil.GetTimeFromSecond(dtResult.Rows[num2]["灌溉时长"].ToString());
                time = now;
            }
            dtResult.Rows.Add(dtResult.NewRow());
            num2 = dtResult.Rows.Count - 1;
            dtResult.Rows[num2][columnName] = "合计";
            dtResult.Rows[num2]["起始时间"] = "";
            dtResult.Rows[num2]["灌溉时长"] = CommonUtil.GetTimeFromSecond(num3.ToString());
            dtResult.Rows[num2]["用水量"] = Math.Round(num4, 2).ToString();
            dtResult.Rows[num2]["用电量"] = Math.Round(num5, 2).ToString();
            return new ResMsg(true, "");
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按村进行指定时间段内的售水汇总统计(图表数据)</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageIds=村ID,多个村用','隔开，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'村充值':[object1,...,objectn]}</p>")]
        public string GetSaleWaterChartDataByVillages(string loginIdentifer, string villageIds, string startTime, string endTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("村充值", array);
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
            if ((villageIds == null) || (villageIds.Trim() == ""))
            {
                obj2["Message"] = "村庄ID为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string[] strArray = villageIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < strArray.Length; j++)
            {
                DataTable dtResult = new DataTable();
                ResMsg msg = WaterUserModule.GetSaleWaterSummaryReportByVillage(strArray[j], startTime, endTime, ref dtResult);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                JavaScriptObject item = new JavaScriptObject();
                item["村庄"] = DistrictModule.GetDistrictName(long.Parse(strArray[j]));
                item["充值金额"] = (dtResult.Rows[0]["应收金额"].ToString() == "") ? "0" : dtResult.Rows[0]["应收金额"].ToString();
                item["购水金额"] = (dtResult.Rows[0]["售水金额"].ToString() == "") ? "0" : dtResult.Rows[0]["售水金额"].ToString();
                item["购水量"] = (dtResult.Rows[0]["售出水量"].ToString() == "") ? "0" : dtResult.Rows[0]["售出水量"].ToString();
                item["购电金额"] = (dtResult.Rows[0]["售电金额"].ToString() == "") ? "0" : dtResult.Rows[0]["售电金额"].ToString();
                item["购电量"] = (dtResult.Rows[0]["售出电量"].ToString() == "") ? "0" : dtResult.Rows[0]["售出电量"].ToString();
                array.Add(item);
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按村进行用水汇总统计(图表数据)</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageIds=村ID,多个村用','隔开，startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'村用水':[object1,...,objectn]}</p>")]
        public string GetUseWaterChartDataByVillages(string loginIdentifer, string villageIds, string startTime, string endTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("村用水", array);
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
            if ((villageIds == null) || (villageIds.Trim() == ""))
            {
                obj2["Message"] = "村庄ID为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string[] strArray = villageIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < strArray.Length; j++)
            {
                DataTable dtResult = new DataTable();
                ResMsg msg = WaterUserModule.GetUseWaterSummaryReportByVillage(strArray[j], startTime, endTime, ref dtResult);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                double num6 = (dtResult.Rows[0]["灌溉时长"].ToString() == "") ? 0.0 : Convert.ToDouble(dtResult.Rows[0]["灌溉时长"].ToString());
                JavaScriptObject item = new JavaScriptObject();
                item["村庄ID"] = strArray[j];
                item["村庄"] = DistrictModule.GetDistrictName(long.Parse(strArray[j]));
                item["灌溉时长"] = CommonUtil.GetTimeFromSecond(num6.ToString());
                item["用水量"] = (dtResult.Rows[0]["用水量"].ToString() == "") ? "0" : dtResult.Rows[0]["用水量"].ToString();
                item["用电量"] = (dtResult.Rows[0]["用电量"].ToString() == "") ? "0" : dtResult.Rows[0]["用电量"].ToString();
                array.Add(item);
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>按村进行用水汇总统计(图表数据)</span><br/><p>输入参数：loginIdentifer=登录用户标识，villageIds=村ID,多个村用','隔开，year=年，month=月<br/>返回数据格式：{'Result':bool,'Message':string,'村用水':[object1,...,objectn]}</p>")]
        public string GetDeviceDataByVillage(string loginIdentifer, string villageID, string startTime, string endTime)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("村用水", array);
            obj2.Add("合计", obj3);
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
            if ((villageID == null) || (villageID.Trim() == ""))
            {
                obj2["Message"] = "村庄ID为空";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            string villageNo = "";
            try
            {
                District village = DistrictModule.ReturnDistrictInfo(long.Parse(villageID));
                District p1 = DistrictModule.ReturnDistrictInfo(village.ParentId);
                District p2 = DistrictModule.ReturnDistrictInfo(p1.ParentId);
                District p3 = DistrictModule.ReturnDistrictInfo(p2.ParentId);
                District p4 = DistrictModule.ReturnDistrictInfo(p3.ParentId);
                villageNo = p4.DistrictCode + p3.DistrictCode + p2.DistrictCode + p1.DistrictCode + village.DistrictCode;
            }
            catch { }

            if (villageNo.Length != 12)
            {
                obj2["Message"] = "村庄参数错误";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            
            try
            {
                DateTime date = DateTime.Parse(startTime);
                if (date.Year < 2000)
                {
                    obj2["Message"] = "开始时间不能小于2000年";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            catch
            {
                obj2["Message"] = "开始时间格式不对";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            try
            {
                DateTime date = DateTime.Parse(endTime);
                if (date.Year < 2000)
                {
                    obj2["Message"] = "结束时间不能小于2000年";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
            }
            catch
            {
                obj2["Message"] = "结束时间格式不对";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            Workbook workbook = new Workbook
            {
                ExcelWorkbook = { WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                Properties = { Author = "PSWeb", Title = "用水统计", Created = DateTime.Now }
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
            Worksheet worksheet = workbook.Worksheets.Add("用水统计");
            for (int i = 0; i < 4; i++)
            {
                worksheet.Table.Columns.Add(new WorksheetColumn(100));
            }
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("村用水统计（" + startTime + "～" + endTime + "）");
            cell.MergeAcross = 3;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("设备编号").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("设备名称").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用水量").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("用电量").StyleID = "ColumnCaptionStyle";

            double num3 = 0;
            double num4 = 0;

            string sql = "exec p_getUserWaterByVillage '" + villageNo + "','" + villageID + "','" + startTime + "','" + endTime + "'";
            DataTable dt = DbHelperSQL.QueryDataTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                JavaScriptObject item = new JavaScriptObject();
                item["DeviceId"] = dr["DeviceId"].ToString();
                item["DeviceNo"] = dr["DeviceNo"].ToString();
                item["DeviceName"] = dr["DeviceName"].ToString();
                item["FullDeviceNo"] = dr["FullDeviceNo"].ToString();
                item["YearWaterUsed"] = dr["YearWaterUsed"].ToString();
                item["ElectricUsed"] = dr["ElectricUsed"].ToString();
                array.Add(item);

                num3 += Convert.ToDouble(item["YearWaterUsed"]);
                num4 += Convert.ToDouble(item["ElectricUsed"]);

                row = worksheet.Table.Rows.Add();
                row.Cells.Add(item["DeviceNo"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["DeviceName"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["YearWaterUsed"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(item["ElectricUsed"].ToString()).StyleID = "DefaultStyle";
            }

            obj3.Add("用水量", num3.ToString());
            obj3.Add("用电量", num4.ToString());

            row = worksheet.Table.Rows.Add();
            row.Cells.Add("合计").StyleID = "DefaultStyle";
            row.Cells.Add("").StyleID = "DefaultStyle";
            row.Cells.Add(obj3["用水量"].ToString()).StyleID = "DefaultStyle";
            row.Cells.Add(obj3["用电量"].ToString()).StyleID = "DefaultStyle";
            try
            {
                string str = DateTime.Now.Ticks.ToString() + ".xls";
                workbook.Save(this.context.Server.MapPath("~/DataReport") + @"\" + str);
                obj2["ExcelURL"] = str;
                CommonUtil.RemoveFiles(this.context.Server.MapPath("~/DataReport"), ".xls");
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }

            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

    }
}
