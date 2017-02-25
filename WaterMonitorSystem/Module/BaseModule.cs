using Common;
using DBUtility;
using Maticsoft.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module
{
    public class BaseModule
    {
        private static Dictionary<string, BaseInfo> Conllection_AlarmType = new Dictionary<string, BaseInfo>();

        public static void LoadBaseInfo()
        {
            string strSql = "select * from BaseInfo";
            try
            {
                DataTable table = DbHelperSQL.QueryDataTable(strSql);
                if (table.Rows.Count != 0)
                {
                    ModelHandler<BaseInfo> modelHandler = new ModelHandler<BaseInfo>();
                    lock (Conllection_AlarmType)
                    {
                        Conllection_AlarmType.Clear();
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            DataRow dataRow = table.Rows[i];
                            BaseInfo baseInfo = modelHandler.FillModel(dataRow);
                            if (baseInfo.BaseType == "AlarmType")
                            {
                                if (!Conllection_AlarmType.ContainsKey(baseInfo.BaseKey))
                                {
                                    Conllection_AlarmType.Add(baseInfo.BaseKey, baseInfo);
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public static List<string> GetAllAlarmTypeDesc()
        {
            List<string> list = new List<string>();
            lock (Conllection_AlarmType)
            {
                foreach (KeyValuePair<string, BaseInfo> pair in Conllection_AlarmType)
                {
                    list.Add(pair.Value.BaseDesc);
                }
            }
            return list;
        }

        public static BaseInfo GetAlarmTypeByKey(string Key)
        {
            if (!Conllection_AlarmType.ContainsKey(Key))
            {
                return null;
            }
            return Conllection_AlarmType[Key];
        }

        public static BaseInfo GetAlarmTypeByDesc(string Desc)
        {
            foreach (KeyValuePair<string, BaseInfo> pair in Conllection_AlarmType)
            {
                if (pair.Value.BaseDesc == Desc)
                {
                    return pair.Value;
                }
            }
            return null;
        }
    }
}
