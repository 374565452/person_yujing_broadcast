using Common;
using DBUtility;
using Maticsoft.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module
{
    public class RoleModule
    {
        private static Dictionary<long, Role> _RoleCollection = new Dictionary<long, Role>();

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static long AddRole(Maticsoft.Model.Role model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Role(");
            strSql.Append("RoleName,IsAllow,Weight)");
            strSql.Append(" values (");
            strSql.Append("@RoleName,@IsAllow,@Weight)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@RoleName", SqlDbType.NVarChar,50),
					new SqlParameter("@IsAllow", SqlDbType.Int,4),
					new SqlParameter("@Weight", SqlDbType.Int,4)};
            parameters[0].Value = model.RoleName;
            parameters[1].Value = model.IsAllow;
            parameters[2].Value = model.Weight;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                UpdateRoleInfo();
                return Convert.ToInt64(obj);
            }
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static bool ModifyRole(Maticsoft.Model.Role model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Role set ");
            strSql.Append("RoleName=@RoleName,");
            strSql.Append("IsAllow=@IsAllow,");
            strSql.Append("Weight=@Weight");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@RoleName", SqlDbType.NVarChar,50),
					new SqlParameter("@IsAllow", SqlDbType.Int,4),
					new SqlParameter("@Weight", SqlDbType.Int,4),
					new SqlParameter("@Id", SqlDbType.BigInt,8)};
            parameters[0].Value = model.RoleName;
            parameters[1].Value = model.IsAllow;
            parameters[2].Value = model.Weight;
            parameters[3].Value = model.Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                UpdateRoleInfo();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static bool DeleteRole(long Id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Role ");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt)
			};
            parameters[0].Value = Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                UpdateRoleInfo();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetRoleName(long id)
        {
            string roleName = "";
            if (_RoleCollection.ContainsKey(id))
            {
                roleName = _RoleCollection[id].RoleName;
            }
            return roleName;
        }

        public static long[] GetRoleID()
        {
            long[] array = new long[_RoleCollection.Count];
            _RoleCollection.Keys.CopyTo(array, 0);
            return array;
        }

        public static Role GetRole(long Id)
        {
            if (_RoleCollection.ContainsKey(Id))
            {
                return Tools.Copy<Role>(_RoleCollection[Id]);
            }
            return null;
        }

        public static long GetRoleId(string roleName)
        {
            lock (_RoleCollection)
            {
                foreach (KeyValuePair<long, Role> pair in _RoleCollection)
                {
                    if (pair.Value.RoleName == roleName)
                    {
                        return pair.Value.Id;
                    }
                }
                return -1;
            }
        }

        public static void UpdateRoleInfo()
        {
            ModelHandler<Role> modelHandler = new ModelHandler<Role>();

            _RoleCollection.Clear();
            DataTable table = new DataTable();
            string strSql = "select * from Role";
            table = DbHelperSQL.Query(strSql).Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Role role = modelHandler.FillModel(table.Rows[i]);
                if (!_RoleCollection.ContainsKey(role.Id))
                {
                    _RoleCollection.Add(role.Id, role);
                }
                else
                {
                    _RoleCollection[role.Id] = role;
                }
            }
        }
    }
}
