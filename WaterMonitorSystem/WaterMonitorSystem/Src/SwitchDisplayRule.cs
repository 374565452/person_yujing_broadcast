namespace WaterMonitorSystem.Src
{
    public class SwitchDisplayRule
    {
        private string _name = "";
        private string[] _switchDescription = new string[] { "正常", "报警" };
        private string[] _switchDisplay = new string[] { "Green", "Red" };
        private string _switchDisplayMode = "文字";

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

        public string[] SwitchDescription
        {
            get
            {
                return this._switchDescription;
            }
            set
            {
                this._switchDescription = value;
            }
        }

        public string[] SwitchDisplay
        {
            get
            {
                return this._switchDisplay;
            }
            set
            {
                this._switchDisplay = value;
            }
        }

        public string SwitchDisplayMode
        {
            get
            {
                return this._switchDisplayMode;
            }
            set
            {
                this._switchDisplayMode = value;
            }
        }
    }
}