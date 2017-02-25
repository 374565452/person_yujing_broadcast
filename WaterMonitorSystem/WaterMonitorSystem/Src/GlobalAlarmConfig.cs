namespace WaterMonitorSystem.Src
{
    public class GlobalAlarmConfig
    {
        private bool _blAutoPopup;
        private bool _blUseAlarm;
        private bool _blUseVoice;
        private string _strUserId;

        public GlobalAlarmConfig()
        {
            this._strUserId = "";
            this._blUseAlarm = true;
            this._blUseVoice = true;
            this._blAutoPopup = true;
        }

        public GlobalAlarmConfig(string strUserId, bool blUseAlarm, bool blUseVoice, bool blAutoPopup)
        {
            this._strUserId = "";
            this._blUseAlarm = true;
            this._blUseVoice = true;
            this._blAutoPopup = true;
            this._strUserId = strUserId;
            this._blUseAlarm = blUseAlarm;
            this._blUseVoice = blUseVoice;
            this._blAutoPopup = blAutoPopup;
        }

        public GlobalAlarmConfig Copy()
        {
            return new GlobalAlarmConfig { UserId = this._strUserId, UseAlarm = this._blUseAlarm, UseVoice = this._blUseVoice, AutoPopup = this._blAutoPopup };
        }

        public bool AutoPopup
        {
            get
            {
                return this._blAutoPopup;
            }
            set
            {
                this._blAutoPopup = value;
            }
        }

        public bool UseAlarm
        {
            get
            {
                return this._blUseAlarm;
            }
            set
            {
                this._blUseAlarm = value;
            }
        }

        public string UserId
        {
            get
            {
                return this._strUserId;
            }
            set
            {
                this._strUserId = value;
            }
        }

        public bool UseVoice
        {
            get
            {
                return this._blUseVoice;
            }
            set
            {
                this._blUseVoice = value;
            }
        }
    }
}