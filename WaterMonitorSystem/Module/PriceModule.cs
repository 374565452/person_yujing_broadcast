using Common;
using DBUtility;
using System;
using System.Collections.Generic;
using System.Data;

namespace Module
{
    public class PriceModule
    {
        private static Dictionary<string, PriceInfo> dicPriceInfos = new Dictionary<string, PriceInfo>();
        private static Dictionary<string, string> dicPriceTypes = new Dictionary<string, string>();

        public static ResMsg AddPriceInfo(PriceInfo pi)
        {
            ResMsg msg = new ResMsg(false, "");
            PriceInfo info = pi.Copy();
            if ((info.Name == null) || (info.Name.Trim() == ""))
            {
                msg.Message = "价格名称无效";
                return msg;
            }
            if (GetPriceInfoByName(info.Name) != null)
            {
                msg.Message = "存在同名价格";
                return msg;
            }
            int i = DbHelperSQL.ExecuteSql("insert into 水价表(名称,类型,设置时间) values('" + info.Name + "','" + info.Type + "','" + info.SetTime.ToString("yyyy-MM-dd HH:mm:ss") + "')");
            if (i > 0)
            {
                DataTable table = DbHelperSQL.Query("select ID From 水价表 where 名称='" + info.Name + "'").Tables[0];
                if ((table == null) || (table.Rows.Count == 0))
                {
                    DbHelperSQL.ExecuteSql("delete from 水价表 where 名称='" + info.Name + "'");
                    msg.Message = "未找到所添加的价格！";
                    return msg;
                }
                string str3 = table.Rows[0]["ID"].ToString();
                i = DbHelperSQL.ExecuteSql(string.Concat(new object[] { 
                    "insert into 时段水价表(水价ID,阶梯数量,阶梯类型,起始时间,结束时间,一阶水量,一阶价格,二阶水量,二阶价格,三阶水量,三阶价格,四阶水量,四阶价格) values('", str3, "','", info.LaddersCount, "','", info.LadderType.ToString(), "','", info.StartTime, "','", info.EndTime, "','", info.FirstVolume, "','", info.FirstPrice, "','", info.SecondVolume, 
                    "','", info.SecondPrice, "','", info.ThirdVolume, "','", info.ThirdPrice, "','", info.FourthVolume, "','", info.FourthPrice, "')"
                    }));
                if (!(i > 0))
                {
                    DbHelperSQL.ExecuteSql("delete from 水价表 where 名称='" + pi.Name + "'");
                    return msg;
                }
                info.Id = str3;
                lock (dicPriceInfos)
                {
                    dicPriceInfos.Add(info.Id, info);
                }
                msg.Result = true;
                msg.Message = "添加价格成功";
            }
            return msg;
        }

        public static ResMsg ModifyPriceInfo(PriceInfo pi)
        {
            ResMsg msg = new ResMsg(false, "");
            PriceInfo info = pi.Copy();
            if ((info.Name == null) || (info.Name.Trim() == ""))
            {
                msg.Message = "价格名称无效";
                return msg;
            }
            PriceInfo priceInfoByName = GetPriceInfoByName(info.Name);
            if ((priceInfoByName != null) && (priceInfoByName.Id != info.Id))
            {
                msg.Message = "存在同名价格";
                return msg;
            }

            int i = DbHelperSQL.ExecuteSqlTran(new List<string> { string.Concat(new object[] { "update 水价表 set 名称='", info.Name, "',设置时间='", info.SetTime, "' where ID='", info.Id, "'" }), string.Concat(new object[] { 
                "update 时段水价表 set 阶梯数量='", info.LaddersCount, "',阶梯类型='", info.LadderType, "',起始时间='", info.StartTime, "',结束时间='", info.EndTime, "',一阶水量='", info.FirstVolume, "',一阶价格='", info.FirstPrice, "',二阶水量='", info.SecondVolume, "',二阶价格='", info.SecondPrice, 
                "',三阶水量='", info.ThirdVolume, "',三阶价格='", info.ThirdPrice, "',四阶水量='", info.FourthVolume, "',四阶价格='", info.FourthPrice, "' where 水价ID='", info.Id, "'"
                }) });
            if (i > 0)
            {
                lock (dicPriceInfos)
                {
                    if (dicPriceInfos.ContainsKey(info.Id))
                    {
                        dicPriceInfos[info.Id] = info;
                    }
                }
                msg.Result = true;
                msg.Message = "更新价格成功";
                return msg;
            }
            msg.Message = "更新价格失败，原因：" + msg.Message;
            return msg;
        }

        public static ResMsg DeletePriceInfoById(string priceId)
        {
            ResMsg msg = new ResMsg(false, "");
            //删除前判断是否有用水户使用
            string strSql1 = "delete from 时段水价表 where 水价ID='" + priceId + "'";
            string strSql2 = "delete from 水价表 where ID='" + priceId + "'";
            int i = DbHelperSQL.ExecuteSqlTran(new List<string> { strSql1, strSql2 });
            if (i > 0)
            {
                lock (dicPriceInfos)
                {
                    if (dicPriceInfos.ContainsKey(priceId))
                    {
                        dicPriceInfos.Remove(priceId);
                    }
                }
                msg.Result = true;
                msg.Message = "删除价格成功";
                return msg;
            }
            msg.Message = "删除价格失败，原因：" + msg.Message;
            return msg;
        }

        public static List<PriceInfo> GetAllPriceInfos()
        {
            List<PriceInfo> list = new List<PriceInfo>();
            lock (dicPriceInfos)
            {
                foreach (KeyValuePair<string, PriceInfo> pair in dicPriceInfos)
                {
                    list.Add(pair.Value.Copy());
                }
            }
            return list;
        }

        public static PriceInfo GetPriceInfoById(string priceId)
        {
            lock (dicPriceInfos)
            {
                if (dicPriceInfos.ContainsKey(priceId))
                {
                    return Tools.Copy<PriceInfo>(dicPriceInfos[priceId]);
                }
            }
            return null;
        }

        public static PriceInfo GetPriceInfoByName(string priceName)
        {
            lock (dicPriceInfos)
            {
                foreach (KeyValuePair<string, PriceInfo> pair in dicPriceInfos)
                {
                    if (pair.Value.Name == priceName)
                    {
                        return pair.Value.Copy();
                    }
                }
            }
            return null;
        }

        public static void LoadPriceInfos()
        {
            string strSql = "select a.ID,a.名称,a.类型,b.阶梯数量,b.阶梯类型,b.起始时间,b.结束时间,b.一阶水量,b.一阶价格,b.二阶水量,b.二阶价格,b.三阶水量,b.三阶价格,b.四阶水量,b.四阶价格,a.设置时间 from 水价表 a,时段水价表 b where b.水价ID=a.ID order by a.ID";
            DataTable table = DbHelperSQL.Query(strSql).Tables[0];
            if (table != null)
            {
                lock (dicPriceInfos)
                {
                    dicPriceInfos.Clear();
                    foreach (DataRow row in table.Rows)
                    {
                        PriceInfo info = new PriceInfo
                        {
                            Id = row["ID"].ToString(),
                            Name = row["名称"].ToString(),
                            Type = row["类型"].ToString(),
                            SetTime = Convert.ToDateTime(row["设置时间"].ToString()),
                            LaddersCount = Convert.ToInt32(row["阶梯数量"].ToString()),
                            LadderType = (PriceInfo.LaddersType)Enum.Parse(typeof(PriceInfo.LaddersType), row["阶梯类型"].ToString()),
                            StartTime = row["起始时间"].ToString(),
                            EndTime = row["结束时间"].ToString(),
                            FirstPrice = (row["一阶价格"].ToString() != "") ? Convert.ToDecimal(row["一阶价格"].ToString()) : Convert.ToDecimal(0.0),
                            FirstVolume = (row["一阶水量"].ToString() != "") ? Convert.ToDecimal(row["一阶水量"].ToString()) : Convert.ToDecimal(0.0),
                            SecondPrice = (row["二阶价格"].ToString() != "") ? Convert.ToDecimal(row["二阶价格"].ToString()) : Convert.ToDecimal(0.0),
                            SecondVolume = (row["二阶水量"].ToString() != "") ? Convert.ToDecimal(row["二阶水量"].ToString()) : Convert.ToDecimal(0.0),
                            ThirdPrice = (row["三阶价格"].ToString() != "") ? Convert.ToDecimal(row["三阶价格"].ToString()) : Convert.ToDecimal(0.0),
                            ThirdVolume = (row["三阶水量"].ToString() != "") ? Convert.ToDecimal(row["三阶水量"].ToString()) : Convert.ToDecimal(0.0),
                            FourthPrice = (row["四阶价格"].ToString() != "") ? Convert.ToDecimal(row["四阶价格"].ToString()) : Convert.ToDecimal(0.0),
                            FourthVolume = (row["四阶水量"].ToString() != "") ? Convert.ToDecimal(row["四阶水量"].ToString()) : Convert.ToDecimal(0.0)
                        };
                        dicPriceInfos.Add(info.Id, info);
                    }
                }
            }
        }

        public static void LoadPriceTypes()
        {
            string strSql = "select * from 价格类型";
            DataTable table = DbHelperSQL.Query(strSql).Tables[0];
            if (table != null)
            {
                lock (dicPriceTypes)
                {
                    dicPriceTypes.Clear();
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        dicPriceTypes.Add(table.Rows[i]["ID"].ToString(), table.Rows[i]["名称"].ToString());
                    }
                }
            }
        }
    }
}
