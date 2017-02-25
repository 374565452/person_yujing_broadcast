namespace Common
{
    public class ResMsg
    {
        private string _message;
        private bool _result;

        public ResMsg()
        {
            this._result = true;
            this._message = "";
        }

        public ResMsg(bool Result, string Message)
        {
            this._result = Result;
            this._message = Message;
        }

        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
            }
        }

        public bool Result
        {
            get
            {
                return this._result;
            }
            set
            {
                this._result = value;
            }
        }
    }
}
