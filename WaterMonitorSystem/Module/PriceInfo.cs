using System;

namespace Module
{
    [Serializable]
    public class PriceInfo
    {
        private string _endTime = "";
        private decimal _firstPrice;
        private decimal _firstVolume;
        private decimal _fourthPrice;
        private decimal _fourthVolume;
        private string _id = "";
        private int _laddersCount = 1;
        private LaddersType _ladderType = LaddersType.百分比;
        private string _name = "";
        private decimal _secondPrice;
        private decimal _secondVolume;
        private DateTime _setTime = Convert.ToDateTime("1900-01-01");
        private string _startTime = "";
        private decimal _thirdPrice;
        private decimal _thirdVolume;
        private string _type = "";

        public PriceInfo Copy()
        {
            return new PriceInfo { Id = this._id, Name = this._name, Type = this._type, SetTime = this._setTime, LaddersCount = this._laddersCount, LadderType = this._ladderType, StartTime = this._startTime, EndTime = this._endTime, FirstPrice = this._firstPrice, FirstVolume = this._firstVolume, SecondPrice = this._secondPrice, SecondVolume = this._secondVolume, ThirdPrice = this._thirdPrice, ThirdVolume = this._thirdVolume, FourthPrice = this._fourthPrice, FourthVolume = this._fourthVolume };
        }

        public string EndTime
        {
            get
            {
                return this._endTime;
            }
            set
            {
                this._endTime = value;
            }
        }

        public decimal FirstPrice
        {
            get
            {
                return this._firstPrice;
            }
            set
            {
                this._firstPrice = value;
            }
        }

        public decimal FirstVolume
        {
            get
            {
                return this._firstVolume;
            }
            set
            {
                this._firstVolume = value;
            }
        }

        public decimal FourthPrice
        {
            get
            {
                return this._fourthPrice;
            }
            set
            {
                this._fourthPrice = value;
            }
        }

        public decimal FourthVolume
        {
            get
            {
                return this._fourthVolume;
            }
            set
            {
                this._fourthVolume = value;
            }
        }

        public string Id
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        public int LaddersCount
        {
            get
            {
                return this._laddersCount;
            }
            set
            {
                this._laddersCount = value;
            }
        }

        public LaddersType LadderType
        {
            get
            {
                return this._ladderType;
            }
            set
            {
                this._ladderType = value;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public decimal SecondPrice
        {
            get
            {
                return this._secondPrice;
            }
            set
            {
                this._secondPrice = value;
            }
        }

        public decimal SecondVolume
        {
            get
            {
                return this._secondVolume;
            }
            set
            {
                this._secondVolume = value;
            }
        }

        public DateTime SetTime
        {
            get
            {
                return this._setTime;
            }
            set
            {
                this._setTime = value;
            }
        }

        public string StartTime
        {
            get
            {
                return this._startTime;
            }
            set
            {
                this._startTime = value;
            }
        }

        public decimal ThirdPrice
        {
            get
            {
                return this._thirdPrice;
            }
            set
            {
                this._thirdPrice = value;
            }
        }

        public decimal ThirdVolume
        {
            get
            {
                return this._thirdVolume;
            }
            set
            {
                this._thirdVolume = value;
            }
        }

        public string Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }

        public enum LaddersType
        {
            固定值,
            百分比
        }
    }
}
