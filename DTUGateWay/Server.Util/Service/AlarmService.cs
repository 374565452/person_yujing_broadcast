using Maticsoft.Model;
using Module;
using System;
using System.Collections.Generic;

namespace Server.Util.Service
{
    public class AlarmService
    {
        private static Dictionary<string, DeviceAlarm> DeviceAlarmConllection = new Dictionary<string, DeviceAlarm>();

        public static void Init()
        {
            DateTime dateNow = DateTime.Now;
            List<DeviceAlarm> list = DeviceAlarmModule.GetAlarm(dateNow.Date, dateNow, 1);
            DeviceAlarmConllection.Clear();
            if (list.Count > 0)
            {
                foreach (DeviceAlarm deviceAlarm in list)
                {
                    if (deviceAlarm.State == "New")
                        DeviceAlarmConllection.Add(deviceAlarm.DeviceNo + "_" + deviceAlarm.AlarmType, deviceAlarm);
                }
            }
        }

        public static int GetCount()
        {
            return DeviceAlarmConllection.Count;
        }

        public static void DeviceAlarmAnalyse(DeviceEvent deviceEvent)
        {
            string DeviceNo = deviceEvent.DeviceNo;
            string DeviceState = deviceEvent.DeviceState;
            if (DeviceState==null || DeviceState.Length < 32) return;
            for (int i = 0; i < 32; i++)
            {
                int keyi = 31 - i + 1;
                if (BaseModule.GetAlarmTypeByKey(keyi.ToString()) == null)
                {
                    continue;
                }
                if (keyi == 5)
                {
                    string s = "";
                }
                string key = DeviceNo + "_" + keyi;
                if (DeviceAlarmConllection.ContainsKey(key))
                {
                    DeviceAlarm alarm = DeviceAlarmConllection[key];
                    if (alarm.AlarmValue != DeviceState[i].ToString())
                    {
                        alarm.EndTime = deviceEvent.EventTime;
                        alarm.Duration = Convert.ToInt64((alarm.EndTime - alarm.StartTime).TotalSeconds);
                        alarm.State = "Old";

                        DeviceAlarmModule.UpdateDeviceAlarm(alarm);
                        DeviceAlarmConllection.Remove(key);

                        DeviceAlarm alarmNew = new DeviceAlarm();
                        alarmNew.DeviceNo = DeviceNo;
                        alarmNew.AlarmValue = DeviceState[i].ToString();
                        alarmNew.StartTime = deviceEvent.EventTime;
                        alarmNew.EndTime = deviceEvent.EventTime;
                        alarmNew.Duration = 0;
                        alarmNew.SaveTime = DateTime.Now;
                        alarmNew.AlarmType = keyi.ToString();
                        alarmNew.State = "New";
                        try
                        {
                            long id = DeviceAlarmModule.AddDeviceAlarm(alarmNew);
                            if (id > 0)
                            {
                                alarmNew.Id = id;
                            }
                            DeviceAlarmConllection.Add(key, alarmNew);
                        }
                        catch { }
                    }
                }
                else
                {
                    DeviceAlarm alarm = new DeviceAlarm();
                    alarm.DeviceNo = DeviceNo;
                    alarm.AlarmValue = DeviceState[i].ToString();
                    alarm.StartTime = deviceEvent.EventTime;
                    alarm.EndTime = deviceEvent.EventTime;
                    alarm.Duration = 0;
                    alarm.SaveTime = DateTime.Now;
                    alarm.AlarmType = keyi.ToString();
                    alarm.State = "New";
                    try
                    {
                        long id = DeviceAlarmModule.AddDeviceAlarm(alarm);
                        if (id > 0)
                        {
                            alarm.Id = id;
                        }
                        DeviceAlarmConllection.Add(key, alarm);
                    }
                    catch { }
                }
            }
        }
    }
}
