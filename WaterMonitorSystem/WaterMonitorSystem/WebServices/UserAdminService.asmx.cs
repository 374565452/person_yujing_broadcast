using CarlosAg.ExcelXmlWriter;
using Common;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// UserAdminService 的摘要说明
    /// </summary>
    [WebService(Description = "支持网站用户的添加、修改、删除、密码修改；登录、退出", Name = "用户管理服务", Namespace = "http://www.data86.net/")]
    [Serializable, WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class UserAdminService : System.Web.Services.WebService
    {
        log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HttpContext context = HttpContext.Current;

        public UserAdminService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>添加用户</span><br/><p style='text-indent:15px'>loginIdentifer=登录用户标识;userJSONString=用户对象JSON格式字符串:{'用户名':string,'角色ID':string,'管理ID':string,'自定义测点':'是/否','管理ID列表':'mid1,mid2,...midn','设备ID列表':'did1,did2,...didn'};返回JSON数据：{'Results':bool,'Message':string}</p>")]
        public string AddUser(string loginIdentifer, string userJSONString)
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
            try
            {
                JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(userJSONString);
                if (obj3 == null)
                {
                    obj2["Message"] = "参数格式错误";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (SysUserModule.GetUserId(obj3["用户名"].ToString()) > 0)
                {
                    obj2["Message"] = "此用户已经存在！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                try
                {
                    SysUser user = new SysUser();
                    user.UserName = obj3["用户名"].ToString();
                    user.Password = "888888";
                    user.RegDate = DateTime.Now;
                    user.LogonDate = DateTime.Parse("2000-1-1");
                    user.IsAllow = 1;
                    user.RoleId = long.Parse(obj3["角色ID"].ToString());
                    user.DistrictId = long.Parse(obj3["管理ID"].ToString());
                    user.TrueName = obj3["用户名"].ToString();
                    user.Sex = "";
                    user.Mobile = "";
                    user.Address = "";
                    user.Remark = "";

                    string str = SysUserModule.AddUser(user);
                    if (str.Contains("添加成功"))
                    {
                        obj2["Result"] = true;
                    }
                    obj2["Message"] = str;

                    try
                    {
                        SysLog log = new SysLog();
                        log.LogUserId = loginUser.UserId;
                        log.LogUserName = loginUser.LoginName;
                        log.LogAddress = ToolsWeb.GetIP(context.Request);
                        log.LogTime = DateTime.Now;
                        log.LogType = "添加用户";
                        log.LogContent = str + "|" + ModelHandler<SysUser>.ToString(user);
                        SysLogModule.Add(log);
                    }
                    catch { }
                }
                catch (Exception exception)
                {
                    obj2["Message"] = exception.Message;
                }
            }
            catch (Exception exception2)
            {
                obj2["Message"] = exception2.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>修改用户</span><br/><p style='text-indent:15px'>loginIdentifer=登录用户标识;userJSONString=用户对象JSON格式字符串:{'ID':string,'用户名':string,'角色ID':string,'管理ID':string,'自定义测点':'是/否','管理ID列表':'mid1,mid2,...midn','设备ID列表':'did1,did2,...didn'};返回JSON数据：{'Results':bool,'Message':string}</p>")]
        public string ModifyUser(string loginIdentifer, string userJSONString)
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
            try
            {
                JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(userJSONString);
                if (obj3 == null)
                {
                    obj2["Message"] = "参数格式错误";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                
                try
                {
                    SysUser user = SysUserModule.GetUser(long.Parse(obj3["ID"].ToString()));
                    user.UserName = obj3["用户名"].ToString();
                    user.RoleId = long.Parse(obj3["角色ID"].ToString());
                    user.DistrictId = long.Parse(obj3["管理ID"].ToString());
                    user.TrueName = obj3["用户名"].ToString();

                    long existsId = SysUserModule.GetUserId(user.UserName);
                    if (existsId>0 && existsId != user.ID)
                    {
                        obj2["Message"] = "此用户已经存在！";
                        return JavaScriptConvert.SerializeObject(obj2);
                    }

                    string str = SysUserModule.ModifyUser(user);
                    if (str.Contains("修改成功"))
                    {
                        obj2["Result"] = true;
                    }
                    obj2["Message"] = str;
                }
                catch (Exception exception)
                {
                    obj2["Message"] = exception.Message;
                }
            }
            catch (Exception exception2)
            {
                obj2["Message"] = exception2.Message;
            }

            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "修改用户";
                log.LogContent = obj2["Message"].ToString() + "|" + userJSONString;
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>删除用户</span><br/><p style='text-indent:15px'>loginIdentifer=登录用户标识;id=用户ID;返回JSON数据：{'Results':bool,'Message':string}</p>")]
        public string DeleteUser(string loginIdentifer, string id)
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
            try
            {
                string str = SysUserModule.DeleteUser(long.Parse(id));
                if (str.Contains("删除成功"))
                {
                    obj2["Result"] = true;
                }
                obj2["Message"] = str;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }

            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "删除用户";
                log.LogContent = obj2["Message"].ToString() + "|" + id;
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>用户登录方法</span><br/><p style='text-indent:15px'>输入参数：LoginName=用户名；LoginPwd=密码；ValidateCode=验证码；返回数据：{'Results':bool,'Message':string,'Guid':string}</p>")]
        public string LoginEx(string LoginName, string LoginPwd, string ValidateCode)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("Guid", "");
            /*
            string cookieName = CommonUtil.GetCookieName(this.context.Request, "VCode");
            if ((base.Session[cookieName] == null) || (base.Session[cookieName].ToString() == ""))
            {
                obj2["Message"] = "请刷新验证码";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (base.Session[cookieName].ToString() != ValidateCode)
            {
                obj2["Message"] = "验证码不正确，请重新输入";
                return JavaScriptConvert.SerializeObject(obj2);
            }
             * */
            /*
            if (SysInfo.IsReg && !SysInfo.IsRegSuccess)
            {
                obj2["Message"] = "系统未注册";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            */

            if (SysInfo.IsReg)
            {
                string path = context.Server.MapPath("~/");

                SysInfo.SetFilePath(path);

                SysInfo.IsRegSuccess = false;
                string regStr = "";
                if (FileHelper.IsExists(SysInfo.fileName))
                {
                    regStr = FileHelper.ReadFile(SysInfo.fileName);
                }
                else
                {
                    regStr = "00000000000000000000000000000000";
                    FileHelper.writeFile(SysInfo.fileName, regStr);
                }

                if (regStr != SysInfo.GetRegStr2())
                {
                    myLogger.Info("注册码不对！序列号为：" + SysInfo.GetRegStr1());
                    obj2["Message"] = "系统未注册";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                else
                {
                    SysInfo.IsRegSuccess = true;
                }
            }

            try
            {
                long userID = 0;
                string str3 = SysUserModule.Login(LoginName, LoginPwd, ToolsWeb.GetIP(context.Request), "", ref userID);
                if (str3 == "登陆成功")
                {
                    SysUserModule.OperatorLogin(userID, ToolsWeb.GetIP(context.Request), "");
                    LoginUser loginUser = new LoginUser
                    {
                        UserId = userID,
                        LoginName = LoginName,
                        LoginIdentifer = Guid.NewGuid().ToString()
                    };
                    GlobalAppModule.AddLoginUser(loginUser);

                    obj2["Result"] = true;
                    obj2["Guid"] = loginUser.LoginIdentifer;
                }
                obj2["Message"] = str3;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }

            try
            {
                SysLog log = new SysLog();
                log.LogUserId = 0;
                log.LogUserName = LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "登录";
                log.LogContent = obj2["Message"].ToString();
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>用户退出登录</span><br/><p style='text-indent:15px'>返回JSON数据：{'Results':bool,'Message':string}</p>")]
        public string Logout(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            try
            {
                LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
                if (loginUser != null)
                {
                    GlobalAppModule.RemoveLoginUser(loginIdentifer);
                }
                try
                {
                    SysLog log = new SysLog();
                    log.LogUserId = loginUser.UserId;
                    log.LogUserName = loginUser.LoginName;
                    log.LogAddress = ToolsWeb.GetIP(context.Request);
                    log.LogTime = DateTime.Now;
                    log.LogType = "退出登录";
                    log.LogContent = "";
                    SysLogModule.Add(log);
                }
                catch { }
                obj2["Result"] = true;
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>修改密码</span><br/><p style='text-indent:15px'>loginIdentifer=登录用户标识;id=用户ID;oldPwd=原密码;newPwd=新密码;返回JSON数据：{'Results':bool,'Message':string}</p>")]
        public string ModifyPassWord(string loginIdentifer, string id, string oldPwd, string newPwd)
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
            try
            {
                string message = "";

                if ((id == null) || (id.Trim() == ""))
                {
                    if (SysUserModule.GetUser(long.Parse(message)) == null)
                    {
                        obj2["Message"] = "要修改密码的用户不存在";
                        return JavaScriptConvert.SerializeObject(obj2);
                    }
                    id = message;
                }
                try
                {
                    string str2 = SysUserModule.ModifyPassWord(long.Parse(id), oldPwd, newPwd);
                    if (str2.Contains("修改成功"))
                    {
                        obj2["Result"] = true;
                    }
                    obj2["Message"] = str2;
                }
                catch (Exception exception)
                {
                    obj2["Message"] = exception.Message;
                }
            }
            catch (Exception exception2)
            {
                obj2["Message"] = exception2.Message;
            }
            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "修改用户密码";
                log.LogContent = obj2["Message"].ToString() + "|" + id + "【" + oldPwd + "】【" + newPwd + "】";
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod]
        public string GetUserInfosByIds(string loginIdentifer, string ids)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("UserNodes", new JavaScriptObject());
            string[] strArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                try
                {
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        SysUser userInfo = SysUserModule.GetUser(long.Parse(((strArray[i] == null) || (strArray[i].Trim() == "")) ? msg.Message : strArray[i]));
                        if (userInfo != null)
                        {
                            array.Add(this.UserInfoToJSON(userInfo));
                        }
                    }
                    obj2["UserNodes"] = array;
                    obj2["Result"] = true;
                }
                catch (Exception exception)
                {
                    obj2["Message"] = exception.Message;
                }
            }
            catch (Exception exception2)
            {
                obj2["Message"] = exception2.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据管理ID得到用户信息列表</span><br/><p style='text-indent:15px'>loginIdentifer=登录用户标识;id=管理ID;isRecursive=是否进行递归处理，true:获取管理及其子管理下所有用户，false:只获取管理下的直接用户;isExport:是否导出到Excel;返回JSON数据：{'Results':bool,'Message':string,'UserNodes':[Object,Object,……],'ExcelURL':string}</p>")]
        public string GetUserInfos(string loginIdentifer, string managerId, bool isRecursive, bool isExport)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("UserNodes", array);
            obj2.Add("ExcelURL", "");
            try
            {
                ResMsg msg = CommonUtil.CheckLoginState(loginIdentifer, true);
                if (!msg.Result)
                {
                    obj2["Message"] = msg.Message;
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                try
                {
                    List<long> allLowerID = new List<long>();
                    if (!isRecursive)
                    {
                        allLowerID.Add(long.Parse(managerId));
                    }
                    else
                    {
                        allLowerID = DistrictModule.GetAllDistrictID(long.Parse(managerId));
                    }
                    DataTable dtExcel = null;
                    if (isExport)
                    {
                        dtExcel = new DataTable();
                        dtExcel.Columns.Add("用户名");
                        dtExcel.Columns.Add("角色名称");
                        dtExcel.Columns.Add("单位名称");
                    }
                    foreach (long str in allLowerID)
                    {
                        foreach (SysUser user in SysUserModule.GetUserListByDistrict(str))
                        {
                            if (user != null)
                            {
                                array.Add(this.UserInfoToJSON(user, ref dtExcel));
                            }
                        }
                    }
                    if (isExport)
                    {
                        msg = this.CreateExcel(dtExcel);
                        if (msg.Result)
                        {
                            obj2["ExcelURL"] = msg.Message;
                        }
                    }
                    obj2["Result"] = true;
                }
                catch (Exception exception)
                {
                    obj2["Message"] = exception.Message;
                }
            }
            catch (Exception exception2)
            {
                obj2["Message"] = exception2.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private ResMsg CreateExcel(DataTable dt)
        {
            ResMsg msg = new ResMsg(false, "");
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                DataTable table = dt;
                int count = table.Columns.Count;
                Workbook workbook = new Workbook
                {
                    ExcelWorkbook = { ActiveSheetIndex = 1, WindowTopX = 0, WindowTopY = 0, WindowHeight = 0x1b58, WindowWidth = 0x1f40 },
                    Properties = { Author = "PSWeb", Title = "User Information List", Created = DateTime.Now }
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
                Worksheet worksheet = workbook.Worksheets.Add("用户信息列表");
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (i == (table.Columns.Count - 1))
                    {
                        worksheet.Table.Columns.Add(new WorksheetColumn(130));
                    }
                    else
                    {
                        worksheet.Table.Columns.Add(new WorksheetColumn(0x41));
                    }
                }
                WorksheetCell cell = worksheet.Table.Rows.Add().Cells.Add("用户信息列表");
                cell.MergeAcross = count - 1;
                cell.StyleID = "HeaderStyle";
                WorksheetRow row = worksheet.Table.Rows.Add();
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    row.Cells.Add(table.Columns[j].Caption).StyleID = "ColumnCaptionStyle";
                }
                for (int k = 0; k < table.Rows.Count; k++)
                {
                    row = worksheet.Table.Rows.Add();
                    for (int m = 0; m < table.Columns.Count; m++)
                    {
                        row.Cells.Add(table.Rows[k][m].ToString()).StyleID = "Default";
                    }
                }
                try
                {
                    string str = DateTime.Now.Ticks.ToString() + ".xls";
                    workbook.Save(this.context.Server.MapPath(".").Replace("WebServices", "WebAdmin") + @"\" + str);
                    CommonUtil.RemoveFiles(this.context.Server.MapPath("."), ".xls");
                    msg.Result = true;
                    msg.Message = str;
                }
                catch (Exception exception)
                {
                    msg.Message = exception.Message;
                }
            }
            return msg;
        }

        private JavaScriptObject UserInfoToJSON(SysUser user)
        {
            DataTable dtExcel = null;
            return this.UserInfoToJSON(user, ref dtExcel);
        }

        private JavaScriptObject UserInfoToJSON(SysUser user, ref DataTable dtExcel)
        {
            if (user == null)
            {
                return null;
            }
            JavaScriptObject obj2 = new JavaScriptObject();
            /*
            foreach (KeyValuePair<string, string> pair in user.GetUserInfo())
            {
                obj2.Add(pair.Key, pair.Value);
            }
             * */
            obj2.Add("ID", user.ID);
            obj2.Add("用户名", user.UserName);
            obj2.Add("角色ID", user.RoleId);
            obj2.Add("管理ID", user.DistrictId);
            string managelName = DistrictModule.GetDistrictName(user.DistrictId);
            obj2.Add("管理名称", managelName);
            string roleName = RoleModule.GetRoleName(user.RoleId);
            obj2.Add("角色名称", roleName);
            if (dtExcel != null)
            {
                dtExcel.Rows.Add(new object[0]);
                int num = dtExcel.Rows.Count - 1;
                dtExcel.Rows[num]["用户名"] = user.UserName;
                dtExcel.Rows[num]["角色名称"] = roleName;
                dtExcel.Rows[num]["单位名称"] = managelName;
            }
             
            return obj2;
        }
    }
}
