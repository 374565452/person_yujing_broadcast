using System.Collections.Generic;

namespace WaterMonitorSystem.Src
{
    public class DeviceAppGroup
    {
        private List<DeviceAppGroup> _childDeviceGroup = new List<DeviceAppGroup>();
        private string _groupName = "";
        private string _mapUrl = "";
        private string _monitorUrl = "";
        private List<string> _userStationParms = new List<string>();

        public List<DeviceAppGroup> ChildDeviceGroup
        {
            get
            {
                return this._childDeviceGroup;
            }
            set
            {
                this._childDeviceGroup = value;
            }
        }

        public string GroupName
        {
            get
            {
                return this._groupName;
            }
            set
            {
                this._groupName = value;
            }
        }

        public string MapURL
        {
            get
            {
                return this._mapUrl;
            }
            set
            {
                this._mapUrl = value;
            }
        }

        public string MonitorUrl
        {
            get
            {
                return this._monitorUrl;
            }
            set
            {
                this._monitorUrl = value;
            }
        }

        public List<string> UserStationParms
        {
            get
            {
                return this._userStationParms;
            }
            set
            {
                this._userStationParms = value;
            }
        }
    }
}