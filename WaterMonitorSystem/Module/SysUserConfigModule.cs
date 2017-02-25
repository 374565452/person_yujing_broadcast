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
    public class SysUserConfigModule
    {
        private static Dictionary<long, Dictionary<string, List<string>>> ConfigCollection = new Dictionary<long, Dictionary<string, List<string>>>();

        public static string AddSysUserConfig(List<SysUserConfig> config)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("SysUserId", typeof(long)));
                dt.Columns.Add(new DataColumn("ValType", typeof(string)));
                dt.Columns.Add(new DataColumn("ValName", typeof(string)));
                dt.Columns.Add(new DataColumn("IsShow", typeof(string)));
                dt.Columns.Add(new DataColumn("OrderNum", typeof(int)));

                for (int i = 0; i < config.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = config[i].SysUserId;
                    dr[1] = config[i].ValType;
                    dr[2] = config[i].ValName;
                    dr[3] = config[i].IsShow;
                    dr[4] = i + 1;
                    dt.Rows.Add(dr);
                }

                DbHelperSQL.SqlBulkCopyByDatatable("SysUserConfig", dt);
                return "添加成功";
            }
            catch {
                return "添加失败，原因：写入数据库失败！";
            }
        }

        public static ResMsg DeleteUserConfig(long SysUserId)
        {
            string strSql = "delete SysUserConfig where SysUserId=@SysUserId";
            SqlParameter[] cmdParms = new SqlParameter[]{ 
                new SqlParameter("@SysUserId", SqlDbType.BigInt)
            };
            cmdParms[0].Value = SysUserId;
            try
            {
                int rows = DbHelperSQL.ExecuteSql(strSql, cmdParms);
                if (rows > 0)
                {
                    return new ResMsg(true, "删除成功");
                }
                else
                {
                    return new ResMsg(false, "删除失败，原因：写入数据库失败！");
                }
            }
            catch
            {
                return new ResMsg(false, "删除失败，原因：写入数据库失败！");
            }
        }

        public static ResMsg DeleteUserConfig(long SysUserId, string ValType)
        {
            string strSql = "delete SysUserConfig where SysUserId=@SysUserId and ValType=@ValType";
            SqlParameter[] cmdParms = new SqlParameter[]{ 
                new SqlParameter("@SysUserId", SqlDbType.BigInt),
                new SqlParameter("@ValType", SqlDbType.NVarChar,50)
            };
            cmdParms[0].Value = SysUserId;
            cmdParms[1].Value = ValType;
            try
            {
                int rows = DbHelperSQL.ExecuteSql(strSql, cmdParms);
                if (rows > 0)
                {
                    return new ResMsg(true, "删除成功");
                }
                else
                {
                    return new ResMsg(false, "删除失败，原因：写入数据库失败！");
                }
            }
            catch
            {
                return new ResMsg(false, "删除失败，原因：写入数据库失败！");
            }
        }

        public static void UpdateSysUserConfig(long UserId)
        {
            string strSql = "select * from SysUserConfig where SysUserId=@SysUserId and IsShow=1 order by SysUserId,ValType,OrderNum";
            SqlParameter[] cmdParms = new SqlParameter[]{ 
                new SqlParameter("@SysUserId", SqlDbType.BigInt)
            };
            cmdParms[0].Value = UserId;
            DataTable table = DbHelperSQL.Query(strSql, cmdParms).Tables[0];
            RemoveConfigByUserId(UserId);

            for (int i = 0; i < table.Rows.Count; i++)
            {
                long SysUserId = Convert.ToInt64(table.Rows[i]["SysUserId"].ToString());
                string ValType = table.Rows[i]["ValType"].ToString();
                string ValName = table.Rows[i]["ValName"].ToString();
                if (!ConfigCollection.ContainsKey(SysUserId))
                {
                    Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                    List<string> list = new List<string>();
                    list.Add(ValName);
                    dict.Add(ValType, list);
                    ConfigCollection.Add(SysUserId, dict);
                }
                else
                {
                    Dictionary<string, List<string>> dict = ConfigCollection[SysUserId];
                    if (!dict.ContainsKey(ValType))
                    {
                        List<string> list = new List<string>();
                        list.Add(ValName);
                        dict.Add(ValType, list);
                    }
                    else
                    {
                        List<string> list = dict[ValType];
                        list.Add(ValName);
                    }
                    ConfigCollection[SysUserId] = dict;
                }
            }
        }

        public static void UpdateSysUserConfig(long UserId, string Type)
        {
            string strSql = "select * from SysUserConfig where SysUserId=@SysUserId and ValType=@ValType and IsShow=1 order by SysUserId,ValType,OrderNum";
            SqlParameter[] cmdParms = new SqlParameter[]{ 
                new SqlParameter("@SysUserId", SqlDbType.BigInt),
                new SqlParameter("@ValType", SqlDbType.NVarChar,50)
            };
            cmdParms[0].Value = UserId;
            cmdParms[1].Value = Type;
            DataTable table = DbHelperSQL.Query(strSql, cmdParms).Tables[0];
            RemoveConfigByUserIdValType(UserId, Type);

            for (int i = 0; i < table.Rows.Count; i++)
            {
                long SysUserId = Convert.ToInt64(table.Rows[i]["SysUserId"].ToString());
                string ValType = table.Rows[i]["ValType"].ToString();
                string ValName = table.Rows[i]["ValName"].ToString();
                if (!ConfigCollection.ContainsKey(SysUserId))
                {
                    Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                    List<string> list = new List<string>();
                    list.Add(ValName);
                    dict.Add(ValType, list);
                    ConfigCollection.Add(SysUserId, dict);
                }
                else
                {
                    Dictionary<string, List<string>> dict = ConfigCollection[SysUserId];
                    if (!dict.ContainsKey(ValType))
                    {
                        List<string> list = new List<string>();
                        list.Add(ValName);
                        dict.Add(ValType, list);
                    }
                    else
                    {
                        List<string> list = dict[ValType];
                        list.Add(ValName);
                    }
                    ConfigCollection[SysUserId] = dict;
                }
            }
        }

        public static Dictionary<string, List<string>> GetConfigByUserId(long userId)
        {
            if (ConfigCollection.ContainsKey(userId))
            {
                return ConfigCollection[userId];
            }
            return null;
        }

        public static List<string> GetConfigByUserIdValType(long userId, string ValType)
        {
            if (ConfigCollection.ContainsKey(userId))
            {
                Dictionary<string, List<string>> dist = ConfigCollection[userId];
                if (dist.ContainsKey(ValType))
                {
                    return dist[ValType];
                }
            }
            return null;
        }

        public static void RemoveConfigByUserId(long userId)
        {
            lock (ConfigCollection)
            {
                if (ConfigCollection.ContainsKey(userId))
                {
                    ConfigCollection.Remove(userId);
                }
            }
        }

        public static void RemoveConfigByUserIdValType(long userId, string ValType)
        {
            lock (ConfigCollection)
            {
                if (ConfigCollection.ContainsKey(userId))
                {
                    Dictionary<string, List<string>> dist = ConfigCollection[userId];
                    if (dist.ContainsKey(ValType))
                    {
                        dist.Remove(ValType);
                    }
                }
            }
        }

        public static void LoadSysUserConfig()
        {
            ConfigCollection.Clear();
            DataTable table = new DataTable();
            string strSql = "select * from SysUserConfig where IsShow=1 order by SysUserId,ValType,OrderNum";
            table = DbHelperSQL.Query(strSql).Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                long SysUserId = Convert.ToInt64(table.Rows[i]["SysUserId"].ToString());
                string ValType = table.Rows[i]["ValType"].ToString();
                string ValName = table.Rows[i]["ValName"].ToString();
                if (!ConfigCollection.ContainsKey(SysUserId))
                {
                    Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                    List<string> list = new List<string>();
                    list.Add(ValName);
                    dict.Add(ValType, list);
                    ConfigCollection.Add(SysUserId, dict);
                }
                else
                {
                    Dictionary<string, List<string>> dict = ConfigCollection[SysUserId];
                    if (!dict.ContainsKey(ValType))
                    {
                        List<string> list = new List<string>();
                        list.Add(ValName);
                        dict.Add(ValType, list);
                    }
                    else
                    {
                        List<string> list = dict[ValType];
                        list.Add(ValName);
                        dict[ValType] = list;
                    }
                    ConfigCollection[SysUserId] = dict;
                }
            }
        }
    }
}
