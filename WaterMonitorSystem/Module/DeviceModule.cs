using Common;
using DBUtility;
using Maticsoft.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module
{
    public class DeviceModule
    {
        private static Dictionary<long, Device> _deviceNodeConllection = new Dictionary<long, Device>();
        private static Dictionary<string, Device> _deviceNodeConllection_DeviceNo = new Dictionary<string, Device>();
        private static Dictionary<long, string> _deviceConllection_IdFullDeviceNo = new Dictionary<long, string>();

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public static bool Exists(long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Device");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在该记录SimNo
        /// </summary>
        public static bool ExistsSimNo(string SimNo)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Device");
            strSql.Append(" where SimNo=@SimNo");
            SqlParameter[] parameters = {
					new SqlParameter("@SimNo", SqlDbType.NVarChar)
			};
            parameters[0].Value = SimNo;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在该记录SimNo
        /// </summary>
        public static bool ExistsSimNo(string SimNo, long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Device");
            strSql.Append(" where SimNo=@SimNo and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@SimNo", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = SimNo;
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在该记录RemoteStation
        /// </summary>
        public static bool ExistsRemoteStation(string RemoteStation)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Device");
            strSql.Append(" where RemoteStation=@RemoteStation");
            SqlParameter[] parameters = {
					new SqlParameter("@RemoteStation", SqlDbType.NVarChar)
			};
            parameters[0].Value = RemoteStation;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在该记录RemoteStation
        /// </summary>
        public static bool ExistsRemoteStation(string RemoteStation, long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Device");
            strSql.Append(" where RemoteStation=@RemoteStation and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@RemoteStation", SqlDbType.NVarChar),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = RemoteStation;
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在该记录DeviceNo
        /// </summary>
        public static bool ExistsDeviceNo(string DeviceNo, long DistrictId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Device");
            strSql.Append(" where DeviceNo=@DeviceNo and DistrictId=@DistrictId");
            SqlParameter[] parameters = {
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar),
                    new SqlParameter("@DistrictId", SqlDbType.BigInt)
			};
            parameters[0].Value = DeviceNo;
            parameters[1].Value = DistrictId;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在该记录DeviceNo
        /// </summary>
        public static bool ExistsDeviceNo(string DeviceNo, long DistrictId, long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Device");
            strSql.Append(" where DeviceNo=@DeviceNo and DistrictId=@DistrictId and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar),
                    new SqlParameter("@DistrictId", SqlDbType.BigInt),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = DeviceNo;
            parameters[1].Value = DistrictId;
            parameters[1].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在该记录DeviceName
        /// </summary>
        public static bool ExistsDeviceName(string DeviceName, long DistrictId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Device");
            strSql.Append(" where DeviceName=@DeviceName and DistrictId=@DistrictId");
            SqlParameter[] parameters = {
					new SqlParameter("@DeviceName", SqlDbType.NVarChar),
                    new SqlParameter("@DistrictId", SqlDbType.BigInt)
			};
            parameters[0].Value = DeviceName;
            parameters[1].Value = DistrictId;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在该记录DeviceName
        /// </summary>
        public static bool ExistsDeviceName(string DeviceName, long DistrictId, long Id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Device");
            strSql.Append(" where DeviceName=@DeviceName and DistrictId=@DistrictId and Id<>@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@DeviceName", SqlDbType.NVarChar),
                    new SqlParameter("@DistrictId", SqlDbType.BigInt),
                    new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = DeviceName;
            parameters[1].Value = DistrictId;
            parameters[2].Value = Id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        public static ResMsg AddDevice(Device device)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Device(");
            strSql.Append("SimNo,DeviceName,Description,SetupDate,SetupAddress,LON,LAT,IsValid,LastUpdate,DistrictId,DeviceNo,Online,OnlineTime,YearExploitation,AlertAvailableWater,AlertAvailableElectric,DeviceTypeCodeId,MeterPulse,AlertWaterLevel,TerminalState,Remark,CropId,Area,StationType,StationCode,Frequency,MainId,DeviceType,WaterUsed,RemoteStation)");
            strSql.Append(" values (");
            strSql.Append("@SimNo,@DeviceName,@Description,@SetupDate,@SetupAddress,@LON,@LAT,@IsValid,@LastUpdate,@DistrictId,@DeviceNo,@Online,@OnlineTime,@YearExploitation,@AlertAvailableWater,@AlertAvailableElectric,@DeviceTypeCodeId,@MeterPulse,@AlertWaterLevel,@TerminalState,@Remark,@CropId,@Area,@StationType,@StationCode,@Frequency,@MainId,@DeviceType,@WaterUsed,@RemoteStation)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@SimNo", SqlDbType.NVarChar,20),
					new SqlParameter("@DeviceName", SqlDbType.NVarChar,50),
					new SqlParameter("@Description", SqlDbType.VarChar,200),
					new SqlParameter("@SetupDate", SqlDbType.DateTime),
					new SqlParameter("@SetupAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LON", SqlDbType.BigInt,8),
					new SqlParameter("@LAT", SqlDbType.BigInt,8),
					new SqlParameter("@IsValid", SqlDbType.Int,4),
					new SqlParameter("@LastUpdate", SqlDbType.DateTime),
					new SqlParameter("@DistrictId", SqlDbType.BigInt,8),
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar,50),
					new SqlParameter("@Online", SqlDbType.Int,4),
					new SqlParameter("@OnlineTime", SqlDbType.DateTime),
					new SqlParameter("@YearExploitation", SqlDbType.Decimal,9),
					new SqlParameter("@AlertAvailableWater", SqlDbType.Int,4),
                    new SqlParameter("@AlertAvailableElectric", SqlDbType.Int,4),
					new SqlParameter("@DeviceTypeCodeId", SqlDbType.Int,4),
					new SqlParameter("@MeterPulse", SqlDbType.Int,4),
					new SqlParameter("@AlertWaterLevel", SqlDbType.Decimal,9),
					new SqlParameter("@TerminalState", SqlDbType.NVarChar,50),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@CropId", SqlDbType.BigInt,8),
					new SqlParameter("@Area", SqlDbType.Decimal,9),
					new SqlParameter("@StationType", SqlDbType.Int,4),
					new SqlParameter("@StationCode", SqlDbType.Int,4),
					new SqlParameter("@Frequency", SqlDbType.Int,4),
					new SqlParameter("@MainId", SqlDbType.BigInt,8),
					new SqlParameter("@DeviceType", SqlDbType.NVarChar,50),
					new SqlParameter("@WaterUsed", SqlDbType.Decimal,9),
					new SqlParameter("@RemoteStation", SqlDbType.NVarChar,10)};
            parameters[0].Value = device.SimNo;
            parameters[1].Value = device.DeviceName;
            parameters[2].Value = device.Description;
            parameters[3].Value = device.SetupDate;
            parameters[4].Value = device.SetupAddress;
            parameters[5].Value = device.LON;
            parameters[6].Value = device.LAT;
            parameters[7].Value = device.IsValid;
            parameters[8].Value = device.LastUpdate;
            parameters[9].Value = device.DistrictId;
            parameters[10].Value = device.DeviceNo;
            parameters[11].Value = device.Online;
            parameters[12].Value = device.OnlineTime;
            parameters[13].Value = device.YearExploitation;
            parameters[14].Value = device.AlertAvailableWater;
            parameters[15].Value = device.AlertAvailableElectric;
            parameters[16].Value = device.DeviceTypeCodeId;
            parameters[17].Value = device.MeterPulse;
            parameters[18].Value = device.AlertWaterLevel;
            parameters[19].Value = device.TerminalState;
            parameters[20].Value = device.Remark;
            parameters[21].Value = device.CropId;
            parameters[22].Value = device.Area;
            parameters[23].Value = device.StationType;
            parameters[24].Value = device.StationCode;
            parameters[25].Value = device.Frequency;
            parameters[26].Value = device.MainId;
            parameters[27].Value = device.DeviceType;
            parameters[28].Value = device.WaterUsed;
            parameters[29].Value = device.RemoteStation;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return new ResMsg(false, "添加失败，原因：写入数据库失败！");
            }
            else
            {
                device.Id = Convert.ToInt64(obj);
                UpdateDeviceInfo(device);
                return new ResMsg(true, Convert.ToInt64(obj).ToString());
            }
        }

        public static ResMsg ModifyDevice(Device device)
        {
            if (!_deviceNodeConllection.ContainsKey(device.Id))
            {
                return new ResMsg(false, "传入的设备ID不存在");
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Device set ");
            strSql.Append("SimNo=@SimNo,");
            strSql.Append("DeviceName=@DeviceName,");
            strSql.Append("Description=@Description,");
            strSql.Append("SetupDate=@SetupDate,");
            strSql.Append("SetupAddress=@SetupAddress,");
            strSql.Append("LON=@LON,");
            strSql.Append("LAT=@LAT,");
            strSql.Append("IsValid=@IsValid,");
            strSql.Append("LastUpdate=@LastUpdate,");
            strSql.Append("DistrictId=@DistrictId,");
            strSql.Append("DeviceNo=@DeviceNo,");
            strSql.Append("Online=@Online,");
            strSql.Append("OnlineTime=@OnlineTime,");
            strSql.Append("YearExploitation=@YearExploitation,");
            strSql.Append("AlertAvailableWater=@AlertAvailableWater,");
            strSql.Append("AlertAvailableElectric=@AlertAvailableElectric,");
            strSql.Append("DeviceTypeCodeId=@DeviceTypeCodeId,");
            strSql.Append("MeterPulse=@MeterPulse,");
            strSql.Append("AlertWaterLevel=@AlertWaterLevel,");
            strSql.Append("TerminalState=@TerminalState,");
            strSql.Append("Remark=@Remark,");
            strSql.Append("CropId=@CropId,");
            strSql.Append("Area=@Area,");
            strSql.Append("StationType=@StationType,");
            strSql.Append("StationCode=@StationCode,");
            strSql.Append("Frequency=@Frequency,");
            strSql.Append("MainId=@MainId,");
            strSql.Append("DeviceType=@DeviceType,");
            strSql.Append("WaterUsed=@WaterUsed,");
            strSql.Append("RemoteStation=@RemoteStation,");
            strSql.Append("Rainfall=@Rainfall,");
            strSql.Append("Rainfall_Hour=@Rainfall_Hour,");
            strSql.Append("Rainfall_Day=@Rainfall_Day,");
            strSql.Append("Rainfall_Total=@Rainfall_Total,");
            strSql.Append("WaterLevel=@WaterLevel,");
            strSql.Append("Acq_Time=@Acq_Time");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@SimNo", SqlDbType.NVarChar,20),
					new SqlParameter("@DeviceName", SqlDbType.NVarChar,50),
					new SqlParameter("@Description", SqlDbType.VarChar,200),
					new SqlParameter("@SetupDate", SqlDbType.DateTime),
					new SqlParameter("@SetupAddress", SqlDbType.NVarChar,200),
					new SqlParameter("@LON", SqlDbType.BigInt,8),
					new SqlParameter("@LAT", SqlDbType.BigInt,8),
					new SqlParameter("@IsValid", SqlDbType.Int,4),
					new SqlParameter("@LastUpdate", SqlDbType.DateTime),
					new SqlParameter("@DistrictId", SqlDbType.BigInt,8),
					new SqlParameter("@DeviceNo", SqlDbType.NVarChar,50),
					new SqlParameter("@Online", SqlDbType.Int,4),
					new SqlParameter("@OnlineTime", SqlDbType.DateTime),
					new SqlParameter("@YearExploitation", SqlDbType.Decimal,9),
					new SqlParameter("@AlertAvailableWater", SqlDbType.Int,4),
                    new SqlParameter("@AlertAvailableElectric", SqlDbType.Int,4),
					new SqlParameter("@DeviceTypeCodeId", SqlDbType.Int,4),
					new SqlParameter("@MeterPulse", SqlDbType.Int,4),
					new SqlParameter("@AlertWaterLevel", SqlDbType.Decimal,9),
					new SqlParameter("@TerminalState", SqlDbType.NVarChar,50),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@CropId", SqlDbType.BigInt,8),
					new SqlParameter("@Area", SqlDbType.Decimal,9),
					new SqlParameter("@StationType", SqlDbType.Int,4),
					new SqlParameter("@StationCode", SqlDbType.Int,4),
					new SqlParameter("@Frequency", SqlDbType.Int,4),
					new SqlParameter("@MainId", SqlDbType.BigInt,8),
					new SqlParameter("@DeviceType", SqlDbType.NVarChar,50),
					new SqlParameter("@WaterUsed", SqlDbType.Decimal,9),
					new SqlParameter("@RemoteStation", SqlDbType.NVarChar,10),
                    new SqlParameter("@Rainfall", SqlDbType.Decimal,9),
                    new SqlParameter("@Rainfall_Hour", SqlDbType.Decimal,9),
                    new SqlParameter("@Rainfall_Day", SqlDbType.Decimal,9),
                    new SqlParameter("@Rainfall_Total", SqlDbType.Decimal,9),
                    new SqlParameter("@WaterLevel", SqlDbType.Decimal,9),
                    new SqlParameter("@Acq_Time", SqlDbType.DateTime),
					new SqlParameter("@Id", SqlDbType.BigInt,8)};
            parameters[0].Value = device.SimNo;
            parameters[1].Value = device.DeviceName;
            parameters[2].Value = device.Description;
            parameters[3].Value = device.SetupDate;
            parameters[4].Value = device.SetupAddress;
            parameters[5].Value = device.LON;
            parameters[6].Value = device.LAT;
            parameters[7].Value = device.IsValid;
            parameters[8].Value = device.LastUpdate;
            parameters[9].Value = device.DistrictId;
            parameters[10].Value = device.DeviceNo;
            parameters[11].Value = device.Online;
            parameters[12].Value = device.OnlineTime;
            parameters[13].Value = device.YearExploitation;
            parameters[14].Value = device.AlertAvailableWater;
            parameters[15].Value = device.AlertAvailableElectric;
            parameters[16].Value = device.DeviceTypeCodeId;
            parameters[17].Value = device.MeterPulse;
            parameters[18].Value = device.AlertWaterLevel;
            parameters[19].Value = device.TerminalState;
            parameters[20].Value = device.Remark;
            parameters[21].Value = device.CropId;
            parameters[22].Value = device.Area;
            parameters[23].Value = device.StationType;
            parameters[24].Value = device.StationCode;
            parameters[25].Value = device.Frequency;
            parameters[26].Value = device.MainId;
            parameters[27].Value = device.DeviceType;
            parameters[28].Value = device.WaterUsed;
            parameters[29].Value = device.RemoteStation;
            parameters[30].Value = device.Rainfall;
            parameters[31].Value = device.Rainfall_Hour;
            parameters[32].Value = device.Rainfall_Day;
            parameters[33].Value = device.Rainfall_Total;
            parameters[34].Value = device.WaterLevel;
            parameters[35].Value = device.Acq_Time;
            parameters[36].Value = device.Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                UpdateDeviceInfo(device);
                return new ResMsg(true, "修改成功");
            }
            else
            {
                return new ResMsg(false, "修改失败，原因：写入数据库失败！");
            }
        }

        public static ResMsg DeleteDevice(long id)
        {
            if (!_deviceNodeConllection.ContainsKey(id))
            {
                return new ResMsg(false, "传入的设备ID不存在");
            }
            string strSql = "delete Device where id=@id";
            SqlParameter[] cmdParms = new SqlParameter[]{ 
                new SqlParameter("@ID", SqlDbType.BigInt)
            };
            cmdParms[0].Value = id;
            try
            {
                int rows = DbHelperSQL.ExecuteSql(strSql, cmdParms);
                if (rows > 0)
                {
                    RemoveDeviceInfo(id);
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

        public static Device GetDeviceByID(long id)
        {
            if (!_deviceNodeConllection.ContainsKey(id))
            {
                return null;
            }
            return _deviceNodeConllection[id];
        }

        public static string GetFullDeviceNoByID(long id)
        {
            if (_deviceConllection_IdFullDeviceNo.ContainsKey(id))
            {
                return _deviceConllection_IdFullDeviceNo[id];
            }
            return "";
        }

        public static Device GetDeviceByDistrictId(long districtId, string deviceNo)
        {
            foreach (KeyValuePair<long, Device> pair in _deviceNodeConllection)
            {
                if (pair.Value.DistrictId == districtId && pair.Value.DeviceNo == deviceNo)
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public static Device GetDeviceByFullDeviceNo(string FullDeviceNo)
        {
            if (_deviceNodeConllection_DeviceNo.ContainsKey(FullDeviceNo))
            {
                return _deviceNodeConllection_DeviceNo[FullDeviceNo];
            }

            return null;
        }

        public static Device GetDeviceByRemoteStation(string RemoteStation)
        {
            if (RemoteStation.Trim().Length > 0)
            {
                foreach (KeyValuePair<long, Device> pair in _deviceNodeConllection)
                {
                    if (pair.Value.RemoteStation == RemoteStation)
                    {
                        return pair.Value;
                    }
                }
            }
            return null;
        }

        public static List<long> GetDevicesForManageID(long districtId)
        {
            List<long> list = new List<long>();
            foreach (KeyValuePair<long, Device> pair in _deviceNodeConllection)
            {
                if (pair.Value.DistrictId == districtId)
                {
                    list.Add(pair.Key);
                }
            }
            return list;
        }

        public static List<long> GetAllDevicesForManageID(long districtId)
        {
            List<long> list = new List<long>();
            List<long> allLowerID = DistrictModule.GetAllDistrictID(districtId);
            foreach (KeyValuePair<long, Device> pair in _deviceNodeConllection)
            {
                if (allLowerID.Contains(pair.Value.DistrictId))
                {
                    list.Add(pair.Key);
                }
            }
            return list;
        }

        public static List<Device> GetAllDevice()
        {
            List<Device> list = new List<Device>();
            lock (_deviceNodeConllection)
            {
                foreach (KeyValuePair<long, Device> pair in _deviceNodeConllection)
                {
                    list.Add(Tools.Copy<Device>(pair.Value));
                }
            }
            return list;
        }

        public static List<Device> GetAllDeviceSubList(long mainId)
        {
            List<Device> list = new List<Device>();
            lock (_deviceNodeConllection)
            {
                foreach (KeyValuePair<long, Device> pair in _deviceNodeConllection)
                {
                    if (pair.Value.MainId == mainId)
                    {
                        list.Add(Tools.Copy<Device>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static List<string> GetAllDeviceNo()
        {
            List<string> list = new List<string>();
            lock (_deviceNodeConllection_DeviceNo)
            {
                foreach (KeyValuePair<string, Device> pair in _deviceNodeConllection_DeviceNo)
                {
                    list.Add(pair.Key);
                }
            }
            return list;
        }

        public static Device GetDeviceByID_DB(long id)
        {
            try
            {
                string strSql = "select * from Device where IsValid=1 and Id=" + id;
                DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                if (table.Rows.Count != 0)
                {
                    ModelHandler<Device> modelHandler = new ModelHandler<Device>();

                    DataRow dataRow = table.Rows[0];
                    Device device = modelHandler.FillModel(dataRow);
                    lock (_deviceNodeConllection)
                    {
                        if (!_deviceNodeConllection.ContainsKey(device.Id))
                        {
                            _deviceNodeConllection.Add(device.Id, device);
                        }
                        else
                        {
                            _deviceNodeConllection[device.Id] = device;
                        }
                    }
                    District d5 = DistrictModule.ReturnDistrictInfo(device.DistrictId);
                    District d4 = DistrictModule.ReturnDistrictInfo(d5.ParentId);
                    District d3 = DistrictModule.ReturnDistrictInfo(d4.ParentId);
                    District d2 = DistrictModule.ReturnDistrictInfo(d3.ParentId);
                    District d1 = DistrictModule.ReturnDistrictInfo(d2.ParentId);

                    string key = d1.DistrictCode + d2.DistrictCode + d3.DistrictCode + d4.DistrictCode + d5.DistrictCode +
                        device.DeviceNo.PadLeft(3, '0');

                    lock (_deviceNodeConllection_DeviceNo)
                    {
                        if (!_deviceNodeConllection_DeviceNo.ContainsKey(key))
                        {
                            _deviceNodeConllection_DeviceNo.Add(key, device);
                        }
                        else
                        {
                            _deviceNodeConllection_DeviceNo[key] = device;
                        }
                    }
                    lock (_deviceConllection_IdFullDeviceNo)
                    {
                        if (!_deviceConllection_IdFullDeviceNo.ContainsKey(device.Id))
                        {
                            _deviceConllection_IdFullDeviceNo.Add(device.Id, key);
                        }
                        else
                        {
                            _deviceConllection_IdFullDeviceNo[device.Id] = key;
                        }
                    }
                    return device;
                }
            }
            catch { }
            return null;
        }

        public static Device GetDeviceByFullDeviceNo_DB(string FullDeviceNo)
        {
            if (FullDeviceNo.Length == 15)
            {
                try
                {
                    string code1 = FullDeviceNo.Substring(0, 2);
                    string code2 = FullDeviceNo.Substring(2, 2);
                    string code3 = FullDeviceNo.Substring(4, 2);
                    string code4 = FullDeviceNo.Substring(6, 3);
                    string code5 = FullDeviceNo.Substring(9, 3);
                    string deviceNo = FullDeviceNo.Substring(12, 3);

                    District d1 = DistrictModule.ReturnDistrictInfo(code1, 2, 0);
                    if (d1 == null)
                    {
                        d1 = DistrictModule.ReturnDistrictInfo_DB(code1, 2, 0);
                    }
                    if (d1 == null)
                    {
                        return null;
                    }

                    District d2 = DistrictModule.ReturnDistrictInfo(code2, 3, d1.Id);
                    if (d2 == null)
                    {
                        d2 = DistrictModule.ReturnDistrictInfo_DB(code2, 3, d1.Id);
                    }
                    if (d2 == null)
                    {
                        return null;
                    }

                    District d3 = DistrictModule.ReturnDistrictInfo(code3, 4, d2.Id);
                    if (d3 == null)
                    {
                        d3 = DistrictModule.ReturnDistrictInfo_DB(code3, 4, d2.Id);
                    }
                    if (d3 == null)
                    {
                        return null;
                    }

                    District d4 = DistrictModule.ReturnDistrictInfo(code4, 5, d3.Id);
                    if (d4 == null)
                    {
                        d4 = DistrictModule.ReturnDistrictInfo_DB(code4, 5, d3.Id);
                    }
                    if (d4 == null)
                    {
                        return null;
                    }

                    District d5 = DistrictModule.ReturnDistrictInfo(code5, 6, d4.Id);
                    if (d5 == null)
                    {
                        d5 = DistrictModule.ReturnDistrictInfo_DB(code5, 6, d4.Id);
                    }
                    if (d5 == null)
                    {
                        return null;
                    }

                    try
                    {
                        string strSql = "select * from Device where IsValid=1 and DeviceNo='" + deviceNo + "' and DistrictId=" + d5.Id;
                        DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                        if (table.Rows.Count != 0)
                        {
                            ModelHandler<Device> modelHandler = new ModelHandler<Device>();

                            DataRow dataRow = table.Rows[0];
                            Device device = modelHandler.FillModel(dataRow);

                            lock (_deviceNodeConllection)
                            {
                                if (!_deviceNodeConllection.ContainsKey(device.Id))
                                {
                                    _deviceNodeConllection.Add(device.Id, device);
                                }
                                else
                                {
                                    _deviceNodeConllection[device.Id] = device;
                                }
                            }

                            lock (_deviceNodeConllection_DeviceNo)
                            {
                                if (!_deviceNodeConllection_DeviceNo.ContainsKey(FullDeviceNo))
                                {
                                    _deviceNodeConllection_DeviceNo.Add(FullDeviceNo, device);
                                }
                                else
                                {
                                    _deviceNodeConllection_DeviceNo[FullDeviceNo] = device;
                                }
                            }
                            lock (_deviceConllection_IdFullDeviceNo)
                            {
                                if (!_deviceConllection_IdFullDeviceNo.ContainsKey(device.Id))
                                {
                                    _deviceConllection_IdFullDeviceNo.Add(device.Id, FullDeviceNo);
                                }
                                else
                                {
                                    _deviceConllection_IdFullDeviceNo[device.Id] = FullDeviceNo;
                                }
                            }
                            return device;
                        }
                    }
                    catch { }
                }
                catch { }
            }
            return null;
        }

        public static void LoadDevices()
        {
            string strSql = "select * from Device where IsValid=1";
            try
            {
                DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                if (table.Rows.Count != 0)
                {
                    ModelHandler<Device> modelHandler = new ModelHandler<Device>();
                    lock (_deviceNodeConllection)
                    {
                        _deviceNodeConllection.Clear();
                        _deviceNodeConllection_DeviceNo.Clear();
                        _deviceConllection_IdFullDeviceNo.Clear();
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            try
                            {
                                DataRow dataRow = table.Rows[i];
                                Device device = modelHandler.FillModel(dataRow);

                                if (!_deviceNodeConllection.ContainsKey(device.Id))
                                {
                                    _deviceNodeConllection.Add(device.Id, device);
                                }
                                else
                                {
                                    _deviceNodeConllection[device.Id] = device;
                                }

                                District d5 = DistrictModule.ReturnDistrictInfo(device.DistrictId);
                                District d4 = DistrictModule.ReturnDistrictInfo(d5.ParentId);
                                District d3 = DistrictModule.ReturnDistrictInfo(d4.ParentId);
                                District d2 = DistrictModule.ReturnDistrictInfo(d3.ParentId);
                                District d1 = DistrictModule.ReturnDistrictInfo(d2.ParentId);

                                string key = d1.DistrictCode + d2.DistrictCode + d3.DistrictCode + d4.DistrictCode + d5.DistrictCode +
                                    device.DeviceNo.PadLeft(3, '0');


                                if (!_deviceNodeConllection_DeviceNo.ContainsKey(key))
                                {
                                    _deviceNodeConllection_DeviceNo.Add(key, device);
                                }
                                else
                                {
                                    _deviceNodeConllection_DeviceNo[key] = device;
                                }

                                if (!_deviceConllection_IdFullDeviceNo.ContainsKey(device.Id))
                                {
                                    _deviceConllection_IdFullDeviceNo.Add(device.Id, key);
                                }
                                else
                                {
                                    _deviceConllection_IdFullDeviceNo[device.Id] = key;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
        }

        public static void UpdateDeviceInfo(Device device)
        {
            lock (_deviceNodeConllection)
            {
                if (!_deviceNodeConllection.ContainsKey(device.Id))
                {
                    _deviceNodeConllection.Add(device.Id, device);
                }
                else
                {
                    _deviceNodeConllection[device.Id] = device;
                }
            }

            try
            {
                District d5 = DistrictModule.ReturnDistrictInfo(device.DistrictId);
                District d4 = DistrictModule.ReturnDistrictInfo(d5.ParentId);
                District d3 = DistrictModule.ReturnDistrictInfo(d4.ParentId);
                District d2 = DistrictModule.ReturnDistrictInfo(d3.ParentId);
                District d1 = DistrictModule.ReturnDistrictInfo(d2.ParentId);

                string key = d1.DistrictCode + d2.DistrictCode + d3.DistrictCode + d4.DistrictCode + d5.DistrictCode +
                    device.DeviceNo.PadLeft(3, '0');

                lock (_deviceNodeConllection_DeviceNo)
                {
                    if (!_deviceNodeConllection_DeviceNo.ContainsKey(key))
                    {
                        _deviceNodeConllection_DeviceNo.Add(key, device);
                    }
                    else
                    {
                        _deviceNodeConllection_DeviceNo[key] = device;
                    }
                }

                lock (_deviceConllection_IdFullDeviceNo)
                {
                    if (!_deviceConllection_IdFullDeviceNo.ContainsKey(device.Id))
                    {
                        _deviceConllection_IdFullDeviceNo.Add(device.Id, key);
                    }
                    else
                    {
                        _deviceConllection_IdFullDeviceNo[device.Id] = key;
                    }
                }
            }
            catch { }
        }

        public static void RemoveDeviceInfo(long DeviceId)
        {
            Device device = GetDeviceByID(DeviceId);
            if (device != null)
            {
                try
                {
                    District d5 = DistrictModule.ReturnDistrictInfo(device.DistrictId);
                    District d4 = DistrictModule.ReturnDistrictInfo(d5.ParentId);
                    District d3 = DistrictModule.ReturnDistrictInfo(d4.ParentId);
                    District d2 = DistrictModule.ReturnDistrictInfo(d3.ParentId);
                    District d1 = DistrictModule.ReturnDistrictInfo(d2.ParentId);

                    string key = d1.DistrictCode + d2.DistrictCode + d3.DistrictCode + d4.DistrictCode + d5.DistrictCode +
                        device.DeviceNo.PadLeft(3, '0');

                    lock (_deviceNodeConllection_DeviceNo)
                    {
                        if (_deviceNodeConllection_DeviceNo.ContainsKey(key))
                        {
                            _deviceNodeConllection_DeviceNo.Remove(key);
                        }
                    }
                }
                catch { }
            }

            lock (_deviceNodeConllection)
            {
                if (_deviceNodeConllection.ContainsKey(DeviceId))
                {
                    _deviceNodeConllection.Remove(DeviceId);
                }
            }

            lock (_deviceConllection_IdFullDeviceNo)
            {
                if (_deviceConllection_IdFullDeviceNo.ContainsKey(DeviceId))
                {
                    _deviceConllection_IdFullDeviceNo.Remove(DeviceId);
                }
            }
        }

        public static void RemoveDeviceInfo(Device device)
        {
            RemoveDeviceInfo(device.Id);
        }

        public static void RemoveDeviceInfo(string FullDeviceNo)
        {
            if (_deviceNodeConllection_DeviceNo.ContainsKey(FullDeviceNo))
            {
                Device device = _deviceNodeConllection_DeviceNo[FullDeviceNo];
                if (device != null)
                {
                    lock (_deviceNodeConllection)
                    {
                        if (_deviceNodeConllection.ContainsKey(device.Id))
                        {
                            _deviceNodeConllection.Remove(device.Id);
                        }
                    }

                    lock (_deviceConllection_IdFullDeviceNo)
                    {
                        if (_deviceConllection_IdFullDeviceNo.ContainsKey(device.Id))
                        {
                            _deviceConllection_IdFullDeviceNo.Remove(device.Id);
                        }
                    }
                }
            }
        }

        public static string GetDeviceNoMain(string deviceNo)
        {
            string DeviceNoMain = deviceNo;
            Device device = GetDeviceByFullDeviceNo(deviceNo);
            if (device.StationType == 2)
            {
                DeviceNoMain = GetFullDeviceNoByID(device.MainId);
            }
            return DeviceNoMain;
        }

        public static string DeviceNo_Hex2Normal(string deviceNo)
        {
            try
            {
                return deviceNo.Substring(0, 12) + Convert.ToInt32(deviceNo.Substring(12, 2), 16).ToString().PadLeft(3, '0');
            }
            catch
            {
                return "";
            }
        }

        public static string DeviceNo_Normal2Hex(string deviceNo)
        {
            try
            {
                return deviceNo.Substring(0, 12) + Convert.ToInt32(deviceNo.Substring(12, 3)).ToString("X").PadLeft(2, '0');
            }
            catch
            {
                return "";
            }
        }

        public static void SetOnline0()
        {
            string strSql = "update Device set Online=0";
            DbHelperSQL.ExecuteSql(strSql);
        }
    }
}
