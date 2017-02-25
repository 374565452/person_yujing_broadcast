using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterMonitorSystem.Src
{
    public class PageInfo
    {
        private string _dataSource = "";
        private Dictionary<string, int> _dicDeviceRecordsCount = new Dictionary<string, int>();
        private DateTime _dtLastOperateTime = DateTime.Now;
        private int _lifeCycle = GlobalAppModule.OperateTimeOut;
        private string _operateIdentifer = "";
        private string _operatorId = "";
        private string _queryEndTime = "";
        private string _queryStartTime = "";
        private int _recordsCount;
        private string _recordType = "";

        public PageQueryCondition GetPageQueryCondition(int startIndex, int recordsCount)
        {
            PageQueryCondition condition = new PageQueryCondition();
            bool flag = true;
            int num = 0;
            foreach (KeyValuePair<string, int> pair in this._dicDeviceRecordsCount)
            {
                num += pair.Value;
                if (num >= startIndex)
                {
                    condition.Ids.Add(pair.Key);
                    if (flag)
                    {
                        condition.HeadRemoveCount = pair.Value - (num - (startIndex - 1));
                        flag = false;
                    }
                    if (num >= ((startIndex + recordsCount) - 1))
                    {
                        condition.EndRemoveCount = pair.Value - (num - ((startIndex + recordsCount) - 1));
                        return condition;
                    }
                }
            }
            return condition;
        }

        public string DataSource
        {
            get
            {
                return this._dataSource;
            }
            set
            {
                this._dataSource = value;
            }
        }

        public Dictionary<string, int> DicDeviceRecordsCount
        {
            get
            {
                return this._dicDeviceRecordsCount;
            }
            set
            {
                this._dicDeviceRecordsCount = value;
            }
        }

        public DateTime LastOperateTime
        {
            get
            {
                return this._dtLastOperateTime;
            }
            set
            {
                this._dtLastOperateTime = value;
            }
        }

        public int LifeCycle
        {
            get
            {
                return this._lifeCycle;
            }
            set
            {
                this._lifeCycle = value;
            }
        }

        public string OperateIdentifer
        {
            get
            {
                return this._operateIdentifer;
            }
            set
            {
                this._operateIdentifer = value;
            }
        }

        public bool OperateTimeout
        {
            get
            {
                TimeSpan span = (TimeSpan)(DateTime.Now - this._dtLastOperateTime);
                return (span.TotalSeconds > this._lifeCycle);
            }
        }

        public string OperatorId
        {
            get
            {
                return this._operatorId;
            }
            set
            {
                this._operatorId = value;
            }
        }

        public string QueryEndTime
        {
            get
            {
                return this._queryEndTime;
            }
            set
            {
                this._queryEndTime = value;
            }
        }

        public string QueryStartTime
        {
            get
            {
                return this._queryStartTime;
            }
            set
            {
                this._queryStartTime = value;
            }
        }

        public int RecordsCount
        {
            get
            {
                if (this._dicDeviceRecordsCount == null)
                {
                    return 0;
                }
                int num = 0;
                lock (this._dicDeviceRecordsCount)
                {
                    foreach (KeyValuePair<string, int> pair in this._dicDeviceRecordsCount)
                    {
                        num += pair.Value;
                    }
                }
                return num;
            }
        }

        public string RecordType
        {
            get
            {
                return this._recordType;
            }
            set
            {
                this._recordType = value;
            }
        }
    }
}