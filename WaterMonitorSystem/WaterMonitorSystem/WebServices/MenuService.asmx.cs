using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// MenuService 的摘要说明
    /// </summary>
    [Serializable, WebService(Description = "提供菜单操作服务，包括获取菜单、添加菜单、修改菜单、删除菜单", Name = "菜单服务", Namespace = "http://www.data86.net/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class MenuService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public MenuService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取所有用户菜单</span><br/><p style='text-indent:15px'>返回数据格式：{'Result':false,'Message':'','AlarmConfig':{'UseAlarm':'','UseVoice':'','AutoPopup':''}</p>")]
        public string GetMenuAll(string loginIdentifer)
        {
            JavaScriptArray array = new JavaScriptArray();
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            try
            {
                if (loginUser != null)
                {
                    DataTable table = new DataTable();
                    table.Columns.Add("ID");
                    table.Columns.Add("Order", typeof(int));
                    Dictionary<string, JavaScriptObject> dictionary = new Dictionary<string, JavaScriptObject>();

                    List<Menu> menuItemByRoleID = MenuModule.GetMenuAll();
                    foreach (Menu items in menuItemByRoleID)
                    {
                        if (items.ParentId == 0)
                        {
                            JavaScriptObject obj2 = new JavaScriptObject();
                            JavaScriptArray array2 = new JavaScriptArray();
                            table.Rows.Add(new object[] { items.Id, Convert.ToInt32(items.OrderNumber) });
                            obj2.Add("菜单ID", items.Id);
                            obj2.Add("父菜单ID", items.ParentId);
                            obj2.Add("菜单名称", items.MenuName);
                            obj2.Add("入口URL", items.MenuUrl.Trim());
                            obj2.Add("相关文件", items.CssFile.Trim());
                            obj2.Add("子菜单", array2);
                            DataTable table2 = table.Clone();
                            Dictionary<string, JavaScriptObject> dictionary2 = new Dictionary<string, JavaScriptObject>();
                            foreach (Menu items2 in menuItemByRoleID)
                            {
                                if (items2.ParentId == items.Id)
                                {
                                    table2.Rows.Add(new object[] { items2.Id, Convert.ToInt32(items2.OrderNumber) });
                                    JavaScriptObject obj3 = new JavaScriptObject();
                                    obj3.Add("菜单ID", items2.Id);
                                    obj3.Add("父菜单ID", items2.ParentId);
                                    obj3.Add("菜单名称", items2.MenuName);
                                    obj3.Add("入口URL", items2.MenuUrl.Trim());
                                    obj3.Add("相关文件", items2.CssFile.Trim());
                                    dictionary2.Add(items2.Id.ToString(), obj3);
                                }
                            }
                            table2.DefaultView.Sort = "Order Asc";
                            table2 = table2.DefaultView.ToTable();
                            for (int j = 0; j < table2.Rows.Count; j++)
                            {
                                array2.Add(dictionary2[table2.Rows[j]["ID"].ToString()]);
                            }
                            dictionary.Add(items.Id.ToString(), obj2);
                        }
                    }
                    table.DefaultView.Sort = "Order Asc";
                    table = table.DefaultView.ToTable();
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        array.Add(dictionary[table.Rows[i]["ID"].ToString()]);
                    }
                }
            }
            catch
            {
            }
            return JavaScriptConvert.SerializeObject(array);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取当前登录用户菜单</span><br/><p style='text-indent:15px'>返回数据格式：{'Result':false,'Message':'','AlarmConfig':{'UseAlarm':'','UseVoice':'','AutoPopup':''}</p>")]
        public string GetMainMenu(string loginIdentifer)
        {
            JavaScriptArray array = new JavaScriptArray();
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            try
            {
                if (loginUser != null)
                {
                    DataTable table = new DataTable();
                    table.Columns.Add("ID");
                    table.Columns.Add("Order", typeof(int));
                    Dictionary<string, JavaScriptObject> dictionary = new Dictionary<string, JavaScriptObject>();
                    SysUser user = SysUserModule.GetUser(loginUser.UserId);
                    long roleId = user.RoleId;
                    List<Menu> menuItemByRoleID = MenuModule.GetMenuItemByRoleID(roleId);
                    foreach (Menu items in menuItemByRoleID)
                    {
                        if (items.ParentId == 0)
                        {
                            JavaScriptObject obj2 = new JavaScriptObject();
                            JavaScriptArray array2 = new JavaScriptArray();
                            table.Rows.Add(new object[] { items.Id, Convert.ToInt32(items.OrderNumber) });
                            obj2.Add("菜单ID", items.Id);
                            obj2.Add("父菜单ID", items.ParentId);
                            obj2.Add("菜单名称", items.MenuName);
                            obj2.Add("入口URL", items.MenuUrl.Trim());
                            obj2.Add("相关文件", items.CssFile.Trim());
                            obj2.Add("子菜单", array2);
                            DataTable table2 = table.Clone();
                            Dictionary<string, JavaScriptObject> dictionary2 = new Dictionary<string, JavaScriptObject>();
                            foreach (Menu items2 in menuItemByRoleID)
                            {
                                if (items2.ParentId == items.Id)
                                {
                                    table2.Rows.Add(new object[] { items2.Id, Convert.ToInt32(items2.OrderNumber) });
                                    JavaScriptObject obj3 = new JavaScriptObject();
                                    obj3.Add("菜单ID", items2.Id);
                                    obj3.Add("父菜单ID", items2.ParentId);
                                    obj3.Add("菜单名称", items2.MenuName);
                                    obj3.Add("入口URL", items2.MenuUrl.Trim());
                                    obj3.Add("相关文件", items2.CssFile.Trim());
                                    dictionary2.Add(items2.Id.ToString(), obj3);
                                }
                            }
                            table2.DefaultView.Sort = "Order Asc";
                            table2 = table2.DefaultView.ToTable();
                            for (int j = 0; j < table2.Rows.Count; j++)
                            {
                                array2.Add(dictionary2[table2.Rows[j]["ID"].ToString()]);
                            }
                            dictionary.Add(items.Id.ToString(), obj2);
                        }
                    }
                    table.DefaultView.Sort = "Order Asc";
                    table = table.DefaultView.ToTable();
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        array.Add(dictionary[table.Rows[i]["ID"].ToString()]);
                    }
                }
            }
            catch
            {
            }
            return JavaScriptConvert.SerializeObject(array);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据角色id获取菜单</span><br/><p style='text-indent:15px'>返回数据格式：{'Result':false,'Message':'','AlarmConfig':{'UseAlarm':'','UseVoice':'','AutoPopup':''}</p>")]
        public string GetMenuByRoleId(string roleId)
        {
            JavaScriptArray array = new JavaScriptArray();
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("ID");
                table.Columns.Add("Order", typeof(int));
                Dictionary<string, JavaScriptObject> dictionary = new Dictionary<string, JavaScriptObject>();
                List<Menu> menuItemByRoleID = MenuModule.GetMenuItemByRoleID(long.Parse(roleId));
                foreach (Menu items in menuItemByRoleID)
                {
                    if (items.ParentId == 0)
                    {
                        JavaScriptObject obj2 = new JavaScriptObject();
                        JavaScriptArray array2 = new JavaScriptArray();
                        table.Rows.Add(new object[] { items.Id, Convert.ToInt32(items.OrderNumber) });
                        obj2.Add("菜单ID", items.Id);
                        obj2.Add("父菜单ID", items.ParentId);
                        obj2.Add("菜单名称", items.MenuName);
                        obj2.Add("入口URL", items.MenuUrl.Trim());
                        obj2.Add("相关文件", items.CssFile.Trim());
                        obj2.Add("子菜单", array2);
                        DataTable table2 = table.Clone();
                        Dictionary<string, JavaScriptObject> dictionary2 = new Dictionary<string, JavaScriptObject>();
                        foreach (Menu items2 in menuItemByRoleID)
                        {
                            if (items2.ParentId == items.Id)
                            {
                                table2.Rows.Add(new object[] { items2.Id, Convert.ToInt32(items2.OrderNumber) });
                                JavaScriptObject obj3 = new JavaScriptObject();
                                obj3.Add("菜单ID", items2.Id);
                                obj3.Add("父菜单ID", items2.ParentId);
                                obj3.Add("菜单名称", items2.MenuName);
                                obj3.Add("入口URL", items2.MenuUrl.Trim());
                                obj3.Add("相关文件", items2.CssFile.Trim());
                                dictionary2.Add(items2.Id.ToString(), obj3);
                            }
                        }
                        table2.DefaultView.Sort = "Order Asc";
                        table2 = table2.DefaultView.ToTable();
                        for (int j = 0; j < table2.Rows.Count; j++)
                        {
                            array2.Add(dictionary2[table2.Rows[j]["ID"].ToString()]);
                        }
                        dictionary.Add(items.Id.ToString(), obj2);
                    }
                }
                table.DefaultView.Sort = "Order Asc";
                table = table.DefaultView.ToTable();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    array.Add(dictionary[table.Rows[i]["ID"].ToString()]);
                }
            }
            catch
            {
            }
            return JavaScriptConvert.SerializeObject(array);
        }
    }
}
