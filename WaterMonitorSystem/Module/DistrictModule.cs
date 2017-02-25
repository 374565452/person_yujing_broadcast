using Common;
using DBUtility;
using Maticsoft.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public class DistrictModule
    {
        private static Dictionary<string, LevelInfo> _levelInfo = new Dictionary<string, LevelInfo>();
        private static Dictionary<long, District> _DistrictCollection = new Dictionary<long, District>();

        /// <summary>
        /// 是否存在同一父节点下的相同名称节点
        /// </summary>
        public static bool ExistsDistrictName(long ParentId, string DistrictName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from District");
            strSql.Append(" where ParentId=@ParentId and (DistrictName=@DistrictName)");
            SqlParameter[] parameters = {
					new SqlParameter("@ParentId", SqlDbType.BigInt),
                    new SqlParameter("@DistrictName", SqlDbType.VarChar,50)
			};
            parameters[0].Value = ParentId;
            parameters[1].Value = DistrictName;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在同一父节点下的相同编码节点
        /// </summary>
        public static bool ExistsDistrictCode(long ParentId, string DistrictCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from District");
            strSql.Append(" where ParentId=@ParentId and (DistrictCode=@DistrictCode)");
            SqlParameter[] parameters = {
					new SqlParameter("@ParentId", SqlDbType.BigInt),
                    new SqlParameter("@DistrictCode", SqlDbType.VarChar,50)
			};
            parameters[0].Value = ParentId;
            parameters[1].Value = DistrictCode;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在同一父节点下的相同名称节点
        /// </summary>
        public static bool ExistsDistrictName(long Id, long ParentId, string DistrictName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from District");
            strSql.Append(" where Id<>@Id and ParentId=@ParentId and (DistrictName=@DistrictName)");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt),
                    new SqlParameter("@ParentId", SqlDbType.BigInt),
                    new SqlParameter("@DistrictName", SqlDbType.VarChar,50)
			};
            parameters[0].Value = Id;
            parameters[1].Value = ParentId;
            parameters[2].Value = DistrictName;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否存在同一父节点下的相同编码节点
        /// </summary>
        public static bool ExistsDistrictCode(long Id, long ParentId, string DistrictCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from District");
            strSql.Append(" where Id<>@Id and ParentId=@ParentId and (DistrictCode=@DistrictCode)");
            SqlParameter[] parameters = {
                    new SqlParameter("@Id", SqlDbType.BigInt),
					new SqlParameter("@ParentId", SqlDbType.BigInt),
                    new SqlParameter("@DistrictCode", SqlDbType.VarChar,50)
			};
            parameters[0].Value = Id;
            parameters[1].Value = ParentId;
            parameters[2].Value = DistrictCode;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        public static string AddDistrictInfo(District district)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into District(");
            strSql.Append("DistrictName,LON,LAT,DistrictCode,DistrictType,ParentId)");
            strSql.Append(" values (");
            strSql.Append("@DistrictName,@LON,@LAT,@DistrictCode,@DistrictType,@ParentId)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@DistrictName", SqlDbType.VarChar,50),
					new SqlParameter("@LON", SqlDbType.BigInt,8),
					new SqlParameter("@LAT", SqlDbType.BigInt,8),
					new SqlParameter("@DistrictCode", SqlDbType.NVarChar,50),
					new SqlParameter("@DistrictType", SqlDbType.Int,4),
					new SqlParameter("@ParentId", SqlDbType.BigInt,8)};
            parameters[0].Value = district.DistrictName;
            parameters[1].Value = district.LON;
            parameters[2].Value = district.LAT;
            parameters[3].Value = district.DistrictCode;
            parameters[4].Value = district.DistrictType;
            parameters[5].Value = district.ParentId;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return "添加失败，原因：写入数据库失败！";
            }
            else
            {
                return "添加成功";
            }
        }

        public static string ModifyDistrictInfo(District district)
        {
            if (!_DistrictCollection.ContainsKey(district.Id))
            {
                return ("修改失败，原因：不存在ID为" + district.Id + "的单位！");
            }
            if (!(_DistrictCollection.ContainsKey(district.ParentId) || !(district.ParentId != 0)))
            {
                return ("修改失败，原因：不存在上级ID为" + district.ParentId + "的单位！");
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update District set ");
            strSql.Append("DistrictName=@DistrictName,");
            strSql.Append("LON=@LON,");
            strSql.Append("LAT=@LAT,");
            strSql.Append("DistrictCode=@DistrictCode,");
            strSql.Append("DistrictType=@DistrictType,");
            strSql.Append("ParentId=@ParentId");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@DistrictName", SqlDbType.VarChar,50),
					new SqlParameter("@LON", SqlDbType.BigInt,8),
					new SqlParameter("@LAT", SqlDbType.BigInt,8),
					new SqlParameter("@DistrictCode", SqlDbType.NVarChar,50),
					new SqlParameter("@DistrictType", SqlDbType.Int,4),
					new SqlParameter("@ParentId", SqlDbType.BigInt,8),
					new SqlParameter("@Id", SqlDbType.BigInt,8)};
            parameters[0].Value = district.DistrictName;
            parameters[1].Value = district.LON;
            parameters[2].Value = district.LAT;
            parameters[3].Value = district.DistrictCode;
            parameters[4].Value = district.DistrictType;
            parameters[5].Value = district.ParentId;
            parameters[6].Value = district.Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return "修改成功";
            }
            else
            {
                return "修改失败，原因：写入数据库失败！";
            }
        }

        public static string DeleteDistrictNode(long id)
        {
            if (!_DistrictCollection.ContainsKey(id))
            {
                return ("删除失败，原因：不存在ID为" + id + "的管理节点！");
            }
            if (SysUserModule.GetUserListByDistrict(id).Count != 0)
            {
                return "删除失败，原因：该管理节点下有对应的用户！";
            }
            if (GetChildrenDistrictID(id).Count != 0)
            {
                return "删除失败，原因：该管理节点下存在其他节点！";
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from District ");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return "删除成功";
            }
            else
            {
                return "删除失败，原因：写入数据库失败！";
            }
        }

        public static District ReturnDistrictInfo(long id)
        {
            if (_DistrictCollection.ContainsKey(id))
            {
                return Tools.Copy<District>(_DistrictCollection[id]);
            }
            return null;
        }

        public static District ReturnDistrictInfo_DB(long id)
        {
            ModelHandler<District> modelHandler = new ModelHandler<District>();

            DataTable table = new DataTable();
            string strSql = "select * from District where Id = " + id;
            table = DbHelperSQL.Query(strSql).Tables[0];
            if (table.Rows.Count > 0)
            {
                District district = modelHandler.FillModel(table.Rows[0]);
                if (!_DistrictCollection.ContainsKey(district.Id))
                {
                    _DistrictCollection.Add(district.Id, district);
                }
                else
                {
                    _DistrictCollection[district.Id] = district;
                }
                return district;
            }
            return null;
        }

        public static District ReturnDistrictInfo(string DistrictCode, int DistrictType, long ParentId)
        {
            foreach (KeyValuePair<long, District> pair in _DistrictCollection)
            {
                if (ParentId > 0)
                {
                    if (pair.Value.ParentId == ParentId && pair.Value.DistrictCode == DistrictCode && pair.Value.DistrictType == DistrictType)
                    {
                        return pair.Value;
                    }
                }
                else
                {
                    if (pair.Value.DistrictCode == DistrictCode && pair.Value.DistrictType == DistrictType)
                    {
                        return pair.Value;
                    }
                }
            }
            return null;
        }

        public static District ReturnDistrictInfo_DB(string DistrictCode, int DistrictType, long ParentId)
        {
            ModelHandler<District> modelHandler = new ModelHandler<District>();

            DataTable table = new DataTable();
            string strSql = "select * from District where DistrictCode='" + DistrictCode + "' and DistrictType=" + DistrictType + " and ParentId=" + ParentId;
            table = DbHelperSQL.Query(strSql).Tables[0];
            if (table.Rows.Count > 0)
            {
                District district = modelHandler.FillModel(table.Rows[0]);
                if (!_DistrictCollection.ContainsKey(district.Id))
                {
                    _DistrictCollection.Add(district.Id, district);
                }
                else
                {
                    _DistrictCollection[district.Id] = district;
                }
                return district;
            }
            return null;
        }

        public static List<long> GetChildrenDistrictID(long id)
        {
            List<long> list = new List<long>();
            foreach (KeyValuePair<long, District> pair in _DistrictCollection)
            {
                if (pair.Value.ParentId == id)
                {
                    list.Add(pair.Key);
                }
            }
            return list;
        }

        public static List<District> GetChildrenDistrict(long id)
        {
            List<District> list = new List<District>();
            foreach (KeyValuePair<long, District> pair in _DistrictCollection)
            {
                if (pair.Value.ParentId == id)
                {
                    list.Add(Tools.Copy<District>(pair.Value));
                }
            }
            return list;
        }

        public static string GetDistrictName(long id)
        {
            if (_DistrictCollection.ContainsKey(id))
            {
                return _DistrictCollection[id].DistrictName;
            }
            return null;
        }

        public static List<long> GetAllDistrictID(long id)
        {
            List<long> mnIds = new List<long>();
            GetDistrictID(id, ref mnIds);
            return mnIds;
        }

        private static void GetDistrictID(long id, ref List<long> mnIds)
        {
            mnIds.Add(id);
            foreach (KeyValuePair<long, District> pair in _DistrictCollection)
            {
                if (pair.Value.ParentId == id)
                {
                    GetDistrictID(pair.Key, ref mnIds);
                }
            }
        }

        public static List<District> GetAllDistrict(long id)
        {
            List<District> mnIds = new List<District>();
            GetDistrict(id, ref mnIds);
            return mnIds;
        }

        private static void GetDistrict(long id, ref List<District> mnIds)
        {
            mnIds.Add(ReturnDistrictInfo(id));
            foreach (KeyValuePair<long, District> pair in _DistrictCollection)
            {
                if (pair.Value.ParentId == id)
                {
                    GetDistrict(pair.Key, ref mnIds);
                }
            }
        }

        public static string GetLevelName(string id)
        {
            if (_levelInfo.ContainsKey(id))
            {
                return _levelInfo[id].LevelName;
            }
            return "";
        }

        public static string[] GetAllLevelID()
        {
            string[] array = new string[_levelInfo.Count];
            _levelInfo.Keys.CopyTo(array, 0);
            return array;
        }

        public static string GetLevelDescription(string id)
        {
            if (_levelInfo.ContainsKey(id))
            {
                return _levelInfo[id].LevelDescription;
            }
            return "";
        }

        public static List<District> GetAllDistrict()
        {
            List<District> list = new List<District>();
            lock (_DistrictCollection)
            {
                foreach (KeyValuePair<long, District> pair in _DistrictCollection)
                {
                    list.Add(Tools.Copy<District>(pair.Value));
                }
            }
            return list;
        }

        public static void UpdateLevelInfo()
        {
            _levelInfo.Clear();
            DataTable table = new DataTable();
            string strSql = "select * from 级别信息 order by ID";
            table = DbHelperSQL.Query(strSql).Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (!_levelInfo.ContainsKey(table.Rows[i]["ID"].ToString()))
                {
                    _levelInfo.Add(table.Rows[i]["ID"].ToString(), new LevelInfo(table.Rows[i]["ID"].ToString(), table.Rows[i]["级别名称"].ToString(), table.Rows[i]["级别描述"].ToString(), table.Rows[i]["显示别名"].ToString()));
                }
                else
                {
                    _levelInfo[table.Rows[i]["ID"].ToString()] = new LevelInfo(table.Rows[i]["ID"].ToString(), table.Rows[i]["级别名称"].ToString(), table.Rows[i]["级别描述"].ToString(), table.Rows[i]["显示别名"].ToString());
                }
            }
        }

        public static void UpdateDistrictInfo()
        {
            ModelHandler<District> modelHandler = new ModelHandler<District>();

            _DistrictCollection.Clear();
            DataTable table = new DataTable();
            string strSql = "select * from District";
            table = DbHelperSQL.Query(strSql).Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                District district = modelHandler.FillModel(table.Rows[i]);
                if (!_DistrictCollection.ContainsKey(district.Id))
                {
                    _DistrictCollection.Add(district.Id, district);
                }
                else
                {
                    _DistrictCollection[district.Id] = district;
                }
            }
        }
    }
}
