using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterMonitorSystem.Src
{
    public class PageQueryCondition
    {
        private int _endRemoveCount;
        private int _headRemoveCount;
        private List<string> _ids = new List<string>();

        public int EndRemoveCount
        {
            get
            {
                return this._endRemoveCount;
            }
            set
            {
                this._endRemoveCount = value;
            }
        }

        public int HeadRemoveCount
        {
            get
            {
                return this._headRemoveCount;
            }
            set
            {
                this._headRemoveCount = value;
            }
        }

        public List<string> Ids
        {
            get
            {
                return this._ids;
            }
            set
            {
                this._ids = value;
            }
        }
    }
}