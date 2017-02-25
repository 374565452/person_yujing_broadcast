using Common;
using DBUtility;
using Maticsoft.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class MenuModule
    {
        private static Dictionary<long, Menu> dictMenuItem;
        private static Dictionary<long, List<Menu>> roleToMenu;

        public static void MenuInit()
        {
            ModelHandler<Menu> modelHandler = new ModelHandler<Menu>();

            dictMenuItem = new Dictionary<long, Menu>();
            roleToMenu = new Dictionary<long, List<Menu>>();
            string strSql = "select * from Menu";
            DataTable table = DbHelperSQL.Query(strSql).Tables[0];
            for (int num = 0; num < table.Rows.Count; num++)
            {
                Menu menu = modelHandler.FillModel(table.Rows[num]);
                menu.MenuUrl += "?" + DateTime.Now.ToString("yyyyMMddHHmmss");
                if (!dictMenuItem.ContainsKey(menu.Id))
                {
                    dictMenuItem.Add(menu.Id, menu);
                }
            }

            strSql = "select * from RoleMenu";
            table = DbHelperSQL.Query(strSql).Tables[0];
            for (int num = 0; num < table.Rows.Count; num++)
            {
                long roleId = long.Parse(table.Rows[num]["RoleId"].ToString());
                long menuId = long.Parse(table.Rows[num]["MenuId"].ToString());
                if (!roleToMenu.ContainsKey(roleId))
                {
                    roleToMenu.Add(roleId, new List<Menu>());
                }
                if (dictMenuItem.ContainsKey(menuId))
                {
                    roleToMenu[roleId].Add(dictMenuItem[menuId]);
                }
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public static bool DeleteRoleMenuByRoleId(long RoleId)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("delete from RoleMenu ");
                strSql.Append(" where RoleId=@RoleId");
                SqlParameter[] parameters = {
					new SqlParameter("@RoleId", SqlDbType.BigInt)
			    };
                parameters[0].Value = RoleId;

                DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
                roleToMenu.Remove(RoleId);
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public static bool AddRoleMenu(long RoleId, long[] MenuIds)
        {
            try
            {
                DataTable dt = DbHelperSQL.QueryDataTable("SELECT * FROM RoleMenu where 1=0");
                foreach (long MenuId in MenuIds)
                {
                    DataRow dr = dt.NewRow();
                    dr["RoleId"] = RoleId;
                    dr["MenuId"] = MenuId;
                    dt.Rows.Add(dr);
                }
                DbHelperSQL.SqlBulkCopyByDatatable("RoleMenu", dt);

                if (!roleToMenu.ContainsKey(RoleId))
                {
                    roleToMenu.Add(RoleId, new List<Menu>());
                }
                foreach (long MenuId in MenuIds)
                {
                    if (dictMenuItem.ContainsKey(MenuId))
                    {
                        roleToMenu[RoleId].Add(dictMenuItem[MenuId]);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<Menu> GetMenuItemByRoleID(long roleId)
        {
            List<Menu> list = new List<Menu>();
            if (roleToMenu.ContainsKey(roleId))
            {
                List<Menu> list2 = roleToMenu[roleId];
                foreach (Menu items in list2)
                {
                    list.Add(Tools.Copy<Menu>(items));
                }
            }
            return list;
        }

        public static List<Menu> GetMenuAll()
        {
            List<Menu> list = new List<Menu>();
            foreach (KeyValuePair<long, Menu> pair in dictMenuItem)
            {
                list.Add(Tools.Copy<Menu>(pair.Value));
            }
            return list;
        }

        public static Menu GetMenu(long Id)
        {
            if (dictMenuItem.ContainsKey(Id))
            {
                return Tools.Copy<Menu>(dictMenuItem[Id]);
            }
            return null;
        }
    }
}
