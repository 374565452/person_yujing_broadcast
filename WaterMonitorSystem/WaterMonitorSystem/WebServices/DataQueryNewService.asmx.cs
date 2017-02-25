using CarlosAg.ExcelXmlWriter;
using Common;
using DBUtility;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// DataQueryNewService 的摘要说明
    /// </summary>
    [Serializable, ToolboxItem(false), WebService(Description = "气象记录查询、地下水记录查询、土壤记录查询", Name = "数据查询服务", Namespace = "http://www.data86.net/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class DataQueryNewService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public DataQueryNewService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定时间段内的地下水信息记录条数</span><br/><p>输入参数：loginIdentifer=登录用户标识，StationID=设备ID（模糊），startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetGroundWaterRecordsCount(string loginIdentifer, string stationID, string startTime, string endTime)
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
            string strSql = "select StationID from T_GroundWater where StationID like '%" + stationID.Replace(" ", "") + "%' group by StationID";
            DataTable tableStationID = DbHelperSQL.Query(strSql).Tables[0];
            if ((tableStationID == null) || (tableStationID.Rows.Count == 0))
            {
                obj2["Message"] = "设备编号不存在！";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            PageInfo pageInfo = new PageInfo
            {
                OperateIdentifer = Guid.NewGuid().ToString(""),
                QueryStartTime = startTime,
                QueryEndTime = endTime
            };
            for (int i = 0; i < tableStationID.Rows.Count; i++)
            {
                DataRow dr = tableStationID.Rows[i];
                string sql = "select count(*) as 记录条数 from T_GroundWater where StationID='" + dr[0].ToString() + "' and Acq_Time>='" + startTime + "' and Acq_Time<'" + endTime + "'";
                try
                {
                    DataTable table = DbHelperSQL.Query(sql).Tables[0];
                    if (!(table.Rows[0][0].ToString() == "0"))
                    {
                        pageInfo.DicDeviceRecordsCount.Add(dr[0].ToString(), Convert.ToInt32(table.Rows[0][0].ToString()));
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的地下水信息记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetGroundWaterRecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetGroundWaterRecords(operateIdentifer, startIndex, count, ref jsaRecords);
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

        private ResMsg GetGroundWaterRecords(string operateIdentifer, string startIndex, string count, ref JavaScriptArray jsaRecords)
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
                sql = "select *" +
                    ",case when LineLength>GroundWaterLevel then (LineLength-GroundWaterLevel) else (LineLength+GroundWaterLevel) end Height" +
                    ",case when LineLength>GroundWaterLevel then 1000-(LineLength-GroundWaterLevel) else 1000-(LineLength+GroundWaterLevel) end Altitude" +
                    " from T_GroundWater where StationID='" + pageQueryCondition.Ids[i] + "' and Acq_Time>='" + pageInfo.QueryStartTime + "' and Acq_Time<'" + pageInfo.QueryEndTime + "' order by Acq_Time";
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
                            item.Add("StationID", row["StationID"].ToString());
                            item.Add("Height", row["Height"].ToString());
                            item.Add("Altitude", row["Altitude"].ToString());
                            item.Add("GroundWaterLevel", row["GroundWaterLevel"].ToString());
                            item.Add("LineLength", row["LineLength"].ToString());
                            item.Add("GroundWaterTempture", row["GroundWaterTempture"].ToString());
                            item.Add("BV", row["BV"].ToString());
                            item.Add("Acq_Time", DateTime.Parse(row["Acq_Time"].ToString()).ToString("yyyy-MM-dd HH"));
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的地下水信息记录(图表)</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetGroundWaterRecordsChart(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetGroundWaterRecords(operateIdentifer, startIndex, count, ref jsaRecords);
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>导出指定数量的地下水信息记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'ExcelURL':string}</p>")]
        public string ExportGroundWaterRecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetGroundWaterRecords(operateIdentifer, startIndex, count, ref jsaRecords);
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
            Worksheet worksheet = workbook.Worksheets.Add("地下水信息记录");
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(150));
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("地下水信息记录");
            cell.MergeAcross = 5;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("设备编号").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("高程（m）").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("线长（m）").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("水温（℃）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("电压（V）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("观测时间").StyleID = "ColumnCaptionWrapTextStyle";
            JavaScriptObject obj3 = null;
            for (int i = 0; i < jsaRecords.Count; i++)
            {
                obj3 = (JavaScriptObject)jsaRecords[i];
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(obj3["StationID"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["Height"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["LineLength"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["GroundWaterTempture"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["BV"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["Acq_Time"].ToString()).StyleID = "DefaultWrapTextStyle";
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


        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定时间段内的气象信息记录条数</span><br/><p>输入参数：loginIdentifer=登录用户标识，StationID=设备ID（模糊），startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetMeteorologicalRecordsCount(string loginIdentifer, string stationID, string startTime, string endTime)
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
            string strSql = "select StationID from T_Meteorological where StationID like '%" + stationID.Replace(" ", "") + "%' group by StationID";
            DataTable tableStationID = DbHelperSQL.Query(strSql).Tables[0];
            if ((tableStationID == null) || (tableStationID.Rows.Count == 0))
            {
                obj2["Message"] = "设备编号不存在！";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            PageInfo pageInfo = new PageInfo
            {
                OperateIdentifer = Guid.NewGuid().ToString(""),
                QueryStartTime = startTime,
                QueryEndTime = endTime
            };
            for (int i = 0; i < tableStationID.Rows.Count; i++)
            {
                DataRow dr = tableStationID.Rows[i];
                string sql = "select count(*) as 记录条数 from T_Meteorological where StationID='" + dr[0].ToString() + "' and Acq_Time>='" + startTime + "' and Acq_Time<'" + endTime + "'";
                try
                {
                    DataTable table = DbHelperSQL.Query(sql).Tables[0];
                    if (!(table.Rows[0][0].ToString() == "0"))
                    {
                        pageInfo.DicDeviceRecordsCount.Add(dr[0].ToString(), Convert.ToInt32(table.Rows[0][0].ToString()));
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的气象信息记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetMeteorologicalRecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetMeteorologicalRecords(operateIdentifer, startIndex, count, ref jsaRecords);
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

        private ResMsg GetMeteorologicalRecords(string operateIdentifer, string startIndex, string count, ref JavaScriptArray jsaRecords)
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
                sql = "select * from T_Meteorological where StationID='" + pageQueryCondition.Ids[i] + "' and Acq_Time>='" + pageInfo.QueryStartTime + "' and Acq_Time<'" + pageInfo.QueryEndTime + "' order by Acq_Time";
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
                            item.Add("StationID", row["StationID"].ToString());
                            item.Add("Temperature", row["Temperature"].ToString());
                            item.Add("WindDirection", row["WindDirection"].ToString());
                            item.Add("WindPower", row["WindPower"].ToString());
                            item.Add("AirPressure", row["AirPressure"].ToString());
                            item.Add("Rainfall", row["Rainfall"].ToString());
                            item.Add("PAR", row["PAR"].ToString());
                            item.Add("AA_AirRH", row["AA_AirRH"].ToString());
                            item.Add("BV", row["BV"].ToString());
                            item.Add("Acq_Time", DateTime.Parse(row["Acq_Time"].ToString()).ToString("yyyy-MM-dd HH"));
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的气象信息记录(图表)</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetMeteorologicalRecordsChart(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetMeteorologicalRecords(operateIdentifer, startIndex, count, ref jsaRecords);
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>导出指定数量的气象信息记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'ExcelURL':string}</p>")]
        public string ExportMeteorologicalRecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetMeteorologicalRecords(operateIdentifer, startIndex, count, ref jsaRecords);
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
            Worksheet worksheet = workbook.Worksheets.Add("气象信息记录");
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(150));
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("气象信息记录");
            cell.MergeAcross = 9;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("设备编号").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("温度（℃）").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("风向（°）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("风速（m/s）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("气压（hpa）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("降雨（mm）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("日照强度（lux）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("大气湿度（%）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("电压（V）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("观测时间").StyleID = "ColumnCaptionWrapTextStyle";
            JavaScriptObject obj3 = null;
            for (int i = 0; i < jsaRecords.Count; i++)
            {
                obj3 = (JavaScriptObject)jsaRecords[i];
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(obj3["StationID"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["Temperature"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["WindDirection"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["WindPower"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["AirPressure"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["Rainfall"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["PAR"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["AA_AirRH"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["BV"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["Acq_Time"].ToString()).StyleID = "DefaultWrapTextStyle";
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


        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定时间段内的墒情信息记录条数</span><br/><p>输入参数：loginIdentifer=登录用户标识，StationID=设备ID（模糊），startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetSOILMOISTURERecordsCount(string loginIdentifer, string stationID, string startTime, string endTime)
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
            string strSql = "select StationID from T_SOILMOISTURE where StationID like '%" + stationID.Replace(" ", "") + "%' group by StationID";
            DataTable tableStationID = DbHelperSQL.Query(strSql).Tables[0];
            if ((tableStationID == null) || (tableStationID.Rows.Count == 0))
            {
                obj2["Message"] = "设备编号不存在！";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            PageInfo pageInfo = new PageInfo
            {
                OperateIdentifer = Guid.NewGuid().ToString(""),
                QueryStartTime = startTime,
                QueryEndTime = endTime
            };
            for (int i = 0; i < tableStationID.Rows.Count; i++)
            {
                DataRow dr = tableStationID.Rows[i];
                string sql = "select count(*) as 记录条数 from T_SOILMOISTURE where StationID='" + dr[0].ToString() + "' and AcqTime>='" + startTime + "' and AcqTime<'" + endTime + "'";
                try
                {
                    DataTable table = DbHelperSQL.Query(sql).Tables[0];
                    if (!(table.Rows[0][0].ToString() == "0"))
                    {
                        pageInfo.DicDeviceRecordsCount.Add(dr[0].ToString(), Convert.ToInt32(table.Rows[0][0].ToString()));
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的墒情信息记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetSOILMOISTURERecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetSOILMOISTURERecords(operateIdentifer, startIndex, count, ref jsaRecords);
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

        private ResMsg GetSOILMOISTURERecords(string operateIdentifer, string startIndex, string count, ref JavaScriptArray jsaRecords)
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
                sql = "select * from T_SOILMOISTURE where StationID='" + pageQueryCondition.Ids[i] + "' and AcqTime>='" + pageInfo.QueryStartTime + "' and AcqTime<'" + pageInfo.QueryEndTime + "' order by AcqTime";
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
                            item.Add("StationID", row["StationID"].ToString());
                            item.Add("SoilMoisture1", row["SoilMoisture1"].ToString());
                            item.Add("SoilMoisture2", row["SoilMoisture2"].ToString());
                            item.Add("SoilMoisture3", row["SoilMoisture3"].ToString());
                            item.Add("BV", row["BV"].ToString());
                            item.Add("AcqTime", DateTime.Parse(row["AcqTime"].ToString()).ToString("yyyy-MM-dd HH"));
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的墒情信息记录(图表)</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetSOILMOISTURERecordsChart(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetSOILMOISTURERecords(operateIdentifer, startIndex, count, ref jsaRecords);
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>导出指定数量的墒情信息记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'ExcelURL':string}</p>")]
        public string ExportSOILMOISTURERecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetSOILMOISTURERecords(operateIdentifer, startIndex, count, ref jsaRecords);
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
            Worksheet worksheet = workbook.Worksheets.Add("墒情信息记录");
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(150));
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("墒情信息记录");
            cell.MergeAcross = 5;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("设备编号").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("土壤水分1（%）").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("土壤水分2（%）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("土壤水分3（%）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("电压（V）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("观测时间").StyleID = "ColumnCaptionWrapTextStyle";
            JavaScriptObject obj3 = null;
            for (int i = 0; i < jsaRecords.Count; i++)
            {
                obj3 = (JavaScriptObject)jsaRecords[i];
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(obj3["StationID"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["SoilMoisture1"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["SoilMoisture2"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["SoilMoisture3"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["BV"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["AcqTime"].ToString()).StyleID = "DefaultWrapTextStyle";
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


        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定时间段内的雨量信息记录条数</span><br/><p>输入参数：loginIdentifer=登录用户标识，StationID=设备ID（模糊），startTime=起始时间，endTime=结束时间<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetRainFallRecordsCount(string loginIdentifer, string stationID, string startTime, string endTime)
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
            string strSql = "select StationID from T_RainFall where StationID like '%" + stationID.Replace(" ", "") + "%' group by StationID";
            DataTable tableStationID = DbHelperSQL.Query(strSql).Tables[0];
            if ((tableStationID == null) || (tableStationID.Rows.Count == 0))
            {
                obj2["Message"] = "设备编号不存在！";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            PageInfo pageInfo = new PageInfo
            {
                OperateIdentifer = Guid.NewGuid().ToString(""),
                QueryStartTime = startTime,
                QueryEndTime = endTime
            };
            for (int i = 0; i < tableStationID.Rows.Count; i++)
            {
                DataRow dr = tableStationID.Rows[i];
                string sql = "select count(*) as 记录条数 from T_RainFall where StationID='" + dr[0].ToString() + "' and Acq_Time>='" + startTime + "' and Acq_Time<'" + endTime + "'";
                try
                {
                    DataTable table = DbHelperSQL.Query(sql).Tables[0];
                    if (!(table.Rows[0][0].ToString() == "0"))
                    {
                        pageInfo.DicDeviceRecordsCount.Add(dr[0].ToString(), Convert.ToInt32(table.Rows[0][0].ToString()));
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的雨量信息记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetRainFallRecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetRainFallRecords(operateIdentifer, startIndex, count, ref jsaRecords);
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

        private ResMsg GetRainFallRecords(string operateIdentifer, string startIndex, string count, ref JavaScriptArray jsaRecords)
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
                sql = "select *" +
                    ",case when WaterLevel=0 then '正常' when WaterLevel=0.01 then '三级' when WaterLevel=0.02 then '二级' else '一级' end WaterLevel1" +
                    " from T_RainFall where StationID='" + pageQueryCondition.Ids[i] + "' and Acq_Time>='" + pageInfo.QueryStartTime + "' and Acq_Time<'" + pageInfo.QueryEndTime + "' order by Acq_Time";
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
                            item.Add("StationID", row["StationID"].ToString());
                            item.Add("Rainfall", row["Rainfall"].ToString());
                            item.Add("Rainfall_Hour", row["Rainfall_Hour"].ToString());
                            item.Add("Rainfall_Day", row["Rainfall_Day"].ToString());
                            item.Add("Rainfall_Total", row["Rainfall_Total"].ToString());
                            //item.Add("WaterLevel", row["WaterLevel"].ToString());
                            item.Add("WaterLevel1", row["WaterLevel1"].ToString());
                            item.Add("BV", row["BV"].ToString());
                            item.Add("Acq_Time", DateTime.Parse(row["Acq_Time"].ToString()).ToString("yyyy-MM-dd HH:mm"));
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>查询指定数量的雨量信息记录(图表)</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=记录起始索引-从1开始，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'Count':string,'Guid':string}</p>")]
        public string GetRainFallRecordsChart(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetRainFallRecords(operateIdentifer, startIndex, count, ref jsaRecords);
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

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>导出指定数量的雨量信息记录</span><br/><p>输入参数：loginIdentifer=登录用户标识，operateIdentifer=操作标识，startIndex=起始索引，count=记录数量<br/>返回数据格式：{'Result':bool,'Message':string,'ExcelURL':string}</p>")]
        public string ExportRainFallRecords(string loginIdentifer, string operateIdentifer, string startIndex, string count)
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
            ResMsg msg = this.GetRainFallRecords(operateIdentifer, startIndex, count, ref jsaRecords);
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
            Worksheet worksheet = workbook.Worksheets.Add("雨量信息记录");
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(120));
            worksheet.Table.Columns.Add(new WorksheetColumn(150));
            WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("雨量信息记录");
            cell.MergeAcross = 5;
            cell.StyleID = "HeaderStyle";
            WorksheetRow row = worksheet.Table.Rows.Add();
            row.Cells.Add("设备编号").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("实时雨量（mm）").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("小时雨量（mm）").StyleID = "ColumnCaptionStyle";
            row.Cells.Add("日降雨量（mm").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("累计降雨量（mm").StyleID = "ColumnCaptionWrapTextStyle";
            //row.Cells.Add("水位（m）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("水位级别").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("电压（V）").StyleID = "ColumnCaptionWrapTextStyle";
            row.Cells.Add("观测时间").StyleID = "ColumnCaptionWrapTextStyle";
            JavaScriptObject obj3 = null;
            for (int i = 0; i < jsaRecords.Count; i++)
            {
                obj3 = (JavaScriptObject)jsaRecords[i];
                row = worksheet.Table.Rows.Add();
                row.Cells.Add(obj3["StationID"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["Rainfall"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["Rainfall_Hour"].ToString()).StyleID = "DefaultStyle";
                row.Cells.Add(obj3["Rainfall_Day"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["Rainfall_Total"].ToString()).StyleID = "DefaultWrapTextStyle";
                //row.Cells.Add(obj3["WaterLevel"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["WaterLevel1"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["BV"].ToString()).StyleID = "DefaultWrapTextStyle";
                row.Cells.Add(obj3["Acq_Time"].ToString()).StyleID = "DefaultWrapTextStyle";
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


        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取图片列表</span><br/><p style='text-indent:15px'>返回数据：{'Results':bool,'Message':string,'List':object}</p>")]
        public string GetFileList(string loginIdentifer, string stationID)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");

            JavaScriptArray objList = new JavaScriptArray();
            obj2.Add("List", objList);

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

            try
            {
                string rootpath = "UploadImg/" + stationID.Trim() + "/";
                string path = Server.MapPath("~/" + rootpath);
                DirectoryInfo TheFolder = new DirectoryInfo(path);
                if (TheFolder.Exists)
                {
                    foreach (FileInfo fi in TheFolder.GetFiles())
                    {
                        Image img = Image.FromFile(fi.FullName);
                        JavaScriptObject obj7 = new JavaScriptObject();
                        obj7.Add("fn", fi.Name);
                        obj7.Add("fp", rootpath + fi.Name);
                        obj7.Add("fd", fi.LastWriteTime.Ticks);
                        obj7.Add("fz", fi.Length);
                        obj7.Add("fW", img.Width);
                        obj7.Add("fH", img.Height);
                        objList.Add(obj7);
                    }

                    obj2["Result"] = true;
                    obj2["Message"] = "";
                }
                else
                {
                    obj2["Message"] = "无图片";
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取水文设备列表</span><br/><p style='text-indent:15px'>返回数据：{'Results':bool,'Message':string,'List':object}</p>")]
        public string GetDeviceRemoteStationsByMnId(string loginIdentifer, string mnID)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("RemoteStation", array);
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
            if (DistrictModule.ReturnDistrictInfo(long.Parse(mnID)) == null)
            {
                obj2["Message"] = "noExiste";
            }
            else
            {
                Device node = null;
                List<long> allDevicesForManageID = DeviceModule.GetAllDevicesForManageID(long.Parse(mnID));
                for (int i = 0; i < allDevicesForManageID.Count; i++)
                {
                    node = DeviceModule.GetDeviceByID(allDevicesForManageID[i]);
                    if (node != null)
                    {
                        if (node.RemoteStation != null && node.RemoteStation.Length > 0)
                        {
                            JavaScriptObject obj3 = new JavaScriptObject();
                            obj3.Add("ID", node.Id);
                            obj3.Add("RemoteStation", node.RemoteStation);
                            array.Add(obj3);
                        }
                    }
                }
                obj2["Result"] = true;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }
    }
}
