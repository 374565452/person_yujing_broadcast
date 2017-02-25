using System;

namespace WaterMonitorSystem.Src
{
    public class LoginUser
    {
        private string _loginIdentifer = "";
        private long _userId = 0;
        private DateTime dtLastOperateTime = DateTime.Now;
        private int lifeCycle = 0x708;
        private string _LoginName = "";

        public DateTime LastOperateTime
        {
            get
            {
                return this.dtLastOperateTime;
            }
            set
            {
                this.dtLastOperateTime = value;
            }
        }

        public int LifeCycle
        {
            get
            {
                return this.lifeCycle;
            }
            set
            {
                this.lifeCycle = value;
            }
        }

        public string LoginIdentifer
        {
            get
            {
                return this._loginIdentifer;
            }
            set
            {
                this._loginIdentifer = value;
            }
        }

        public bool LoginTimeout
        {
            get
            {
                TimeSpan span = (TimeSpan)(DateTime.Now - this.dtLastOperateTime);
                return (span.TotalSeconds > this.lifeCycle);
            }
        }

        public long UserId
        {
            get
            {
                return this._userId;
            }
            set
            {
                this._userId = value;
            }
        }

        public string LoginName
        {
            get
            {
                return this._LoginName;
            }
            set
            {
                this._LoginName = value;
            }
        }
    }
}