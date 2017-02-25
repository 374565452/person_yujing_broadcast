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
    public class DBManageModule
    {
        public static DataSet OperateDataQueryByUser(List<string> userIDs, DateTime startTime, DateTime endTime)
        {
            DataSet set = new DataSet();
            for (int i = 0; i < userIDs.Count; i++)
            {
                try
                {
                    string strSql = "select UserName 用户名,OperationTime 记录时间,OperationType 操作名称,DeviceName 设备名称,Remark 操作描述,RawData 发送数据,State 发送状态 from DeviceOperation where UserId = '" + userIDs[i] + "' and OperationTime<='" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and OperationTime>='" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' order by OperationTime desc";
                    DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                    if (table.Rows.Count != 0)
                    {
                        set.Tables.Add(table.Copy());
                    }
                }
                catch { }
            }
            return set;
        }

        public static bool CheckTableExiste(string strTableName)
        {
            if (DbHelperSQL.GetSingle("if object_id('" + strTableName + "') is not null select 1 else select 0", null).ToString() == "1")
            {
                return true;
            }
            return false;
        }

        public static DataSet EventRecordsCount(List<string> deviceIDs, DateTime startTime, DateTime endTime, string eventType)
        {
            DataSet set = new DataSet();
            DataTable table = new DataTable();
            for (int i = 0; i < deviceIDs.Count; i++)
            {
                table.Clear();
                table.Columns.Clear();
                string strSql = "";
                bool flag = true;
                for (int j = startTime.Year; j < (endTime.Year + 1); j++)
                {
                    string strTableName = "DeviceEvent_" + j.ToString();
                    if (CheckTableExiste(strTableName))
                    {
                        if (flag)
                        {
                            flag = false;
                        }
                        else
                        {
                            strSql = strSql + " union ";
                        }
                        string str3 = strSql;
                        strSql = str3 + "select count(*) as 记录条数 from " + strTableName + " where DeviceNo ='" + DeviceModule.GetFullDeviceNoByID(long.Parse(deviceIDs[i])) + "' and DeviceTime>='" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and DeviceTime<='" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        if ((eventType != "") && (eventType != "全部"))
                        {
                            strSql = strSql + " and EventType='" + eventType + "'";
                        }
                    }
                }
                if(strSql!="")
                {
                    table = DbHelperSQL.QueryDataTable(strSql);
                }
                if (table.Rows.Count == 0)
                {
                    table = new DataTable();
                    table.Columns.Add("记录条数");
                    table.Rows.Add(table.NewRow());
                    table.Rows[0][0] = 0;
                }
                else if (table.Rows.Count > 1)
                {
                    int num4 = 0;
                    for (int m = 0; m < table.Rows.Count; m++)
                    {
                        num4 += Convert.ToInt32(table.Rows[m]["记录条数"].ToString());
                    }
                    table.Rows.Clear();
                    table.Rows.Add(new object[] { num4.ToString() });
                }
                set.Tables.Add(table.Copy());
            }
            return set;
        }

        public static DataSet EventDataQuery(List<string> deviceIDs, DateTime startTime, DateTime endTime, string eventType)
        {
            int num;
            DataSet set = new DataSet();
            DataTable table = new DataTable();
            string[] strArray = new string[(endTime.Year - startTime.Year) + 1];
            for (num = 0; num < deviceIDs.Count; num++)
            {
                string cz = "";
                string name = "";
                Device device = DeviceModule.GetDeviceByID(long.Parse(deviceIDs[num]));
                if (device != null)
                {
                    name = device.DeviceName;
                    cz = DistrictModule.GetDistrictName(device.DistrictId);
                }
                table.Clear();
                table.Columns.Clear();
                string strSql = "";
                string str7 = "";
                bool flag = true;
                for (int i = startTime.Year; i < (endTime.Year + 1); i++)
                {
                    string strTableName = "DeviceEvent_" + i.ToString();
                    if (CheckTableExiste(strTableName))
                    {
                        if (flag)
                        {
                            strSql = "select '" + cz + "' 村庄,'" + name + "' 设备,DeviceNo 设备编号,CONVERT(varchar(100),DeviceTime,120) 事件时间,EventType 事件类型,Remark 事件描述,RawData 事件数据 from " + strTableName + " where DeviceTime<='" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and DeviceTime>='" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and DeviceNo='" + DeviceModule.GetFullDeviceNoByID(long.Parse(deviceIDs[num])) + "'";
                            if ((eventType != "") && (eventType != "全部"))
                            {
                                strSql = strSql + " and EventType='" + eventType + "'";
                            }
                            str7 = strSql;
                            flag = false;
                        }
                        else
                        {
                            string str9 = strSql;
                            strSql = str9 + " union select '" + cz + "' 村庄,'" + name + "' 设备,DeviceNo 设备编号,CONVERT(varchar(100),DeviceTime,120) 事件时间,EventType 事件类型,Remark 事件描述,RawData 事件数据 from " + strTableName + " where DeviceTime<='" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and DeviceTime>='" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and DeviceNo='" + DeviceModule.GetFullDeviceNoByID(long.Parse(deviceIDs[num])) + "'";
                            if ((eventType != "") && (eventType != "全部"))
                            {
                                strSql = strSql + " and EventType='" + eventType + "'";
                            }
                        }
                    }
                }
                if (strSql != "")
                    strSql = strSql + " order by DeviceTime desc";
                if (strSql != "")
                {
                    table = DbHelperSQL.QueryDataTable(strSql);
                }
                if (table.Rows.Count != 0)
                {
                    set.Tables.Add(table.Copy());
                }
            }
            return set;
        }

        public static DataSet EventDataQuery2(List<string> deviceIDs, DateTime startTime, DateTime endTime, string eventType)
        {
            int num;
            DataSet set = new DataSet();
            DataTable table = new DataTable();
            string[] strArray = new string[(endTime.Year - startTime.Year) + 1];
            for (num = 0; num < deviceIDs.Count; num++)
            {
                string cz = "";
                string name = "";
                Device device = DeviceModule.GetDeviceByID(long.Parse(deviceIDs[num]));
                if (device != null)
                {
                    name = device.DeviceName;
                    cz = DistrictModule.GetDistrictName(device.DistrictId);
                }
                table.Clear();
                table.Columns.Clear();
                string strSql = "";
                string str7 = "";
                bool flag = true;
                for (int i = startTime.Year; i < (endTime.Year + 1); i++)
                {
                    string strTableName = "DeviceEvent_" + i.ToString();
                    if (CheckTableExiste(strTableName))
                    {
                        if (flag)
                        {
                            strSql = "select '" + cz + "' 村庄,'" + name + "' 设备,DeviceNo 设备编号,CONVERT(varchar(100),StartTime,120) 开泵时间,CONVERT(varchar(100),EndTime,120) 关泵时间,DATEDIFF(second,StartTime,EndTime) 灌溉时长,WaterUsed 用水量 from " + strTableName + " where EndTime>StartTime and DeviceTime<='" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and DeviceTime>='" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and DeviceNo='" + DeviceModule.GetFullDeviceNoByID(long.Parse(deviceIDs[num])) + "'";
                            if ((eventType != "") && (eventType != "全部"))
                            {
                                strSql = strSql + " and EventType='" + eventType + "'";
                            }
                            str7 = strSql;
                            flag = false;
                        }
                        else
                        {
                            string str9 = strSql;
                            strSql = str9 + " union select '" + cz + "' 村庄,'" + name + "' 设备,DeviceNo 设备编号,CONVERT(varchar(100),StartTime,120) 开泵时间,CONVERT(varchar(100),EndTime,120) 关泵时间,DATEDIFF(second,StartTime,EndTime) 灌溉时长,WaterUsed 用水量 from " + strTableName + " where EndTime>StartTime and DeviceTime<='" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and DeviceTime>='" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and DeviceNo='" + DeviceModule.GetFullDeviceNoByID(long.Parse(deviceIDs[num])) + "'";
                            if ((eventType != "") && (eventType != "全部"))
                            {
                                strSql = strSql + " and EventType='" + eventType + "'";
                            }
                        }
                    }
                }
                if (strSql != "")
                    strSql = strSql + " order by DeviceTime desc";
                if (strSql != "")
                {
                    table = DbHelperSQL.QueryDataTable(strSql);
                }
                if (table.Rows.Count != 0)
                {
                    set.Tables.Add(table.Copy());
                }
            }
            return set;
        }

        public static DataSet AlarmRecordsCount(List<string> deviceIDs, DateTime startTime, DateTime endTime, string alarmType, bool IsNew)
        {
            DataSet set = new DataSet();
            DataTable table = new DataTable();
            for (int i = 0; i < deviceIDs.Count; i++)
            {
                table.Clear();
                table.Columns.Clear();
                string strSql = "";
                bool flag = true;
                for (int j = startTime.Year; j < (endTime.Year + 1); j++)
                {
                    string strTableName = "DeviceAlarm_" + j.ToString();
                    if (CheckTableExiste(strTableName))
                    {
                        if (flag)
                        {
                            flag = false;
                        }
                        else
                        {
                            strSql = strSql + " union ";
                        }
                        string str3 = strSql;
                        strSql = str3 + "select count(*) as 记录条数 from " + strTableName + " where DeviceNo = '" + DeviceModule.GetFullDeviceNoByID(long.Parse(deviceIDs[i])) + "' and StartTime>='" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and StartTime<='" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        if ((alarmType.Trim() != "") && (alarmType != "全部"))
                        {
                            strSql = strSql + " and AlarmType='" + BaseModule.GetAlarmTypeByDesc(alarmType).BaseKey + "'";
                        }
                        if (IsNew)
                        {
                            strSql = strSql + " and State = 'New'";
                        }
                    }
                }
                for (int k = 0; ((table.Columns.Count == 0) && (k < 3)) && (strSql != ""); k++)
                {
                    table = DbHelperSQL.QueryDataTable(strSql);
                }
                if (table.Rows.Count == 0)
                {
                    table = new DataTable();
                    table.Columns.Add("记录条数");
                    table.Rows.Add(table.NewRow());
                    table.Rows[0][0] = 0;
                }
                else if (table.Rows.Count > 1)
                {
                    int num4 = 0;
                    for (int m = 0; m < table.Rows.Count; m++)
                    {
                        num4 += Convert.ToInt32(table.Rows[m]["记录条数"].ToString());
                    }
                    table.Rows.Clear();
                    table.Rows.Add(new object[] { num4.ToString() });
                }
                set.Tables.Add(table.Copy());
            }
            return set;
        }

        public static DataSet AlarmDataQuery(List<string> deviceIDs, DateTime startTime, DateTime endTime, string alarmType, bool isID, bool IsNew)
        {
            int num;
            DataSet set = new DataSet();
            DataTable table = new DataTable();
            
            for (num = 0; num < deviceIDs.Count; num++)
            {
                string cz = "";
                string name = "";
                Device device = DeviceModule.GetDeviceByID(long.Parse(deviceIDs[num]));
                if (device != null)
                {
                    name = device.DeviceName;
                    cz = DistrictModule.GetDistrictName(device.DistrictId);
                }
                table.Clear();
                table.Columns.Clear();
                string[] strArray = new string[(endTime.Year - startTime.Year) + 1];
                int year = startTime.Year;
                while (year < (endTime.Year + 1))
                {
                    strArray[year - startTime.Year] = "DeviceAlarm_" + year.ToString();
                    year++;
                }
                string strSql = "";
                for (year = 0; year < strArray.Length; year++)
                {
                    if (CheckTableExiste(strArray[year]))
                    {
                        strSql = "select '" + cz + "' 村庄,'" + name + "' 设备,DeviceNo 设备编号"+
                            ",CONVERT(varchar(100),StartTime,120) 开始时间,case when State='New' then '-' else CONVERT(varchar(100),EndTime,120) end 结束时间"+
                            ",(select top 1 BaseDesc from BaseInfo where BaseType='AlarmType' and BaseKey=AlarmType) 类型,AlarmValue" +
                            ",case when AlarmValue=0 then (select top 1 BaseValue0 from BaseInfo where BaseType='AlarmType' and BaseKey=AlarmType) else (select top 1 BaseValue1 from BaseInfo where BaseType='AlarmType' and BaseKey=AlarmType) end 状态"+
                            ",case when State='New' then '持续中' else cast(Duration as varchar)+'秒' end 时长"+
                            " from " + strArray[year] + 
                            " where DeviceNo = '" + DeviceModule.GetFullDeviceNoByID(long.Parse(deviceIDs[num])) + "'"+
                            " and StartTime<='" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and StartTime>='" + startTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        if ((alarmType.Trim() != "") && (alarmType != "全部"))
                        {
                            strSql = strSql + " and AlarmType='" + BaseModule.GetAlarmTypeByDesc(alarmType).BaseKey + "'";
                        }
                        if (IsNew)
                        {
                            strSql = strSql + " and State = 'New'";
                        }
                    }
                }
                strSql = strSql + " order by StartTime desc";
                for (int i = 0; (table.Columns.Count == 0) && (i < 3); i++)
                {
                    table = DbHelperSQL.QueryDataTable(strSql);
                }
                if (table.Rows.Count != 0)
                {
                    set.Tables.Add(table.Copy());
                }
            }
            return set;
        }
    }
}
