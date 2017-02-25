using Common;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// RoleRightService 的摘要说明
    /// </summary>
    [Serializable, WebService(Description = "支持角色的添加、修改、删除和角色权限、角色菜单关系的修改密码修改", Name = "角色权限服务", Namespace = "http://www.data86.net/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class RoleRightService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public RoleRightService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取所有角色信息</span><br/><p style='text-indent:15px'>输入参数：loginIdentifer=登录用户标识;userId=用户ID,当用户ID为空时按登录用户处理；返回数据格式：{'Result':bool,'Message':string,'UserRoles':[{'ID':string,'Name':string},...]</p>")]
        public string GetUserRoles(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("UserRoles", array);
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
                foreach (long str in RoleModule.GetRoleID())
                {
                    JavaScriptObject item = new JavaScriptObject();
                    item.Add("ID", str);
                    item.Add("Name", RoleModule.GetRoleName(str));
                    item.Add("UserCount", SysUserModule.GetUserListByRole(str).Count);
                    array.Add(item);
                }
            }
            catch (Exception exception)
            {
                obj2["Message"] = exception.Message;
                return JavaScriptConvert.SerializeObject(obj2);
            }
            obj2["Result"] = true;
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>添加角色</span><br/><p style='text-indent:15px'>loginIdentifer=登录用户标识;roleJSONString=角色对象JSON格式字符串:{'ID':string,'角色名':string};返回JSON数据：{'Results':bool,'Message':string}</p>")]
        public string AddRole(string loginIdentifer, string roleJSONString)
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
                JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(roleJSONString);
                if (obj3 == null)
                {
                    obj2["Message"] = "参数格式错误";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                if (RoleModule.GetRoleId(obj3["角色名"].ToString()) > 0)
                {
                    obj2["Message"] = "此角色已经存在！";
                    return JavaScriptConvert.SerializeObject(obj2);
                }
                try
                {
                    Role role = new Role();
                    role.RoleName = obj3["角色名"].ToString();
                    role.IsAllow = 1;
                    role.Weight = 1;

                    string str = "添加失败";
                    if (RoleModule.AddRole(role) > 0)
                    {
                        str = "添加成功";
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
                log.LogType = "添加角色";
                log.LogContent = obj2["Message"].ToString() + "|" + roleJSONString;
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>修改角色</span><br/><p style='text-indent:15px'>loginIdentifer=登录用户标识;roleJSONString=角色对象JSON格式字符串:{'ID':string,'角色名':string};返回JSON数据：{'Results':bool,'Message':string}</p>")]
        public string ModifyRole(string loginIdentifer, string roleJSONString)
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
                JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(roleJSONString);
                if (obj3 == null)
                {
                    obj2["Message"] = "参数格式错误";
                    return JavaScriptConvert.SerializeObject(obj2);
                }

                try
                {
                    Role role = RoleModule.GetRole(long.Parse(obj3["ID"].ToString()));
                    role.RoleName = obj3["角色名"].ToString();
                    role.IsAllow = 1;
                    role.Weight = 1;

                    long existsId = RoleModule.GetRoleId(role.RoleName);
                    if (existsId > 0 && existsId != role.Id)
                    {
                        obj2["Message"] = "此角色已经存在！";
                        return JavaScriptConvert.SerializeObject(obj2);
                    }

                    string str = "修改失败";
                    if (RoleModule.ModifyRole(role))
                    {
                        str = "修改成功";
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
                log.LogType = "修改角色";
                log.LogContent = obj2["Message"].ToString() + "|" + roleJSONString;
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>删除角色</span><br/><p style='text-indent:15px'>loginIdentifer=登录用户标识;roleId=角色ID;返回JSON数据：{'Results':bool,'Message':string}</p>")]
        public string DeleteRole(string loginIdentifer, string roleId)
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
                string str = "删除失败";
                if (SysUserModule.GetUserListByRole(long.Parse(roleId)).Count > 0)
                {
                    str = "无法删除有用户数量的角色";
                }
                else
                {
                    if (RoleModule.DeleteRole(long.Parse(roleId)))
                    {
                        str = "删除成功";
                        MenuModule.DeleteRoleMenuByRoleId(long.Parse(roleId));
                        obj2["Result"] = true;
                    }
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
                log.LogType = "删除角色";
                log.LogContent = obj2["Message"].ToString() + "|" + roleId;
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(EnableSession = true, Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>修改角色权限</span><br/><p style='text-indent:15px'>loginIdentifer=登录用户标识;roleId=角色Id;menuIds=子菜单Id;返回JSON数据：{'Results':bool,'Message':string}</p>")]
        public string SetRoleMenu(string loginIdentifer, string roleId, string menuIds)
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
                string str = "修改失败";

                try
                {
                    Role role = RoleModule.GetRole(long.Parse(roleId));
                    
                    if (MenuModule.DeleteRoleMenuByRoleId(role.Id))
                    {
                        List<long> list = new List<long>();
                        string[] strs = menuIds.Trim().Trim(',').Split(',');
                        foreach (string s in strs) {
                            long id = long.Parse(s);
                            Menu m = MenuModule.GetMenu(id);
                            if (m != null)
                            {
                                list.Add(id);

                                if(!list.Contains(m.ParentId))
                                {
                                    list.Add(m.ParentId);
                                }
                            }
                        }
                        if (strs.Length > 0)
                        {
                            if (MenuModule.AddRoleMenu(role.Id, list.ToArray()))
                            {
                                str = "修改成功";
                                obj2["Result"] = true;
                            }
                        }
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
                    log.LogType = "修改角色权限";
                    log.LogContent = obj2["Message"].ToString() + "|" + roleId + "|" + menuIds ;
                    SysLogModule.Add(log);
                }
                catch { }
            }
            catch (Exception exception2)
            {
                obj2["Message"] = exception2.Message;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

    }
}
