using Common;
using DBUtility;
using Maticsoft.Model;
using System.Collections.Generic;
using System.Data;

namespace Module
{
    public class DeviceTypeCodeModule
    {
        private static Dictionary<long, DeviceTypeCode> _DeviceTypeCodeInfos = new Dictionary<long, DeviceTypeCode>();

        public static List<DeviceTypeCode> GetAll()
        {
            List<DeviceTypeCode> list = new List<DeviceTypeCode>();
            lock (_DeviceTypeCodeInfos)
            {
                foreach (KeyValuePair<long, DeviceTypeCode> pair in _DeviceTypeCodeInfos)
                {
                    list.Add(Tools.Copy<DeviceTypeCode>(pair.Value));
                }
            }
            return list;
        }

        public static void LoadDeviceTypeCodeInfos()
        {
            ModelHandler<DeviceTypeCode> modelHandler = new ModelHandler<DeviceTypeCode>();

            _DeviceTypeCodeInfos.Clear();
            DataTable table = new DataTable();
            string strSql = "select * from DeviceTypeCode";
            table = DbHelperSQL.Query(strSql).Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DeviceTypeCode deviceTypeCode = modelHandler.FillModel(table.Rows[i]);
                if (!_DeviceTypeCodeInfos.ContainsKey(deviceTypeCode.Id))
                {
                    _DeviceTypeCodeInfos.Add(deviceTypeCode.Id, deviceTypeCode);
                }
                else
                {
                    _DeviceTypeCodeInfos[deviceTypeCode.Id] = deviceTypeCode;
                }
            }
        }
    }
}
