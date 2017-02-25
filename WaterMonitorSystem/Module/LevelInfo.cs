namespace Module
{
    public class LevelInfo
    {
        private string _byName = "";
        private string _description = "";
        private string _id = "";
        private string _name = "";

        public LevelInfo(string id, string name, string description, string byName)
        {
            this._id = id;
            this._name = name;
            this._description = description;
            this._byName = byName;
        }

        public string ByName
        {
            get
            {
                return this._byName;
            }
        }

        public string LevelDescription
        {
            get
            {
                return this._description;
            }
        }

        public string LevelID
        {
            get
            {
                return this._id;
            }
        }

        public string LevelName
        {
            get
            {
                return this._name;
            }
        }
    }
}
