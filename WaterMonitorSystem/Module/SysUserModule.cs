using Common;
using DBUtility;
using Maticsoft.Model;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Module
{
    public static class SysUserModule
    {
        static log4net.ILog myLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 缓存用户信息
        /// </summary>
        private static Dictionary<long, SysUser> _userCollection = new Dictionary<long, SysUser>();

        public static string Login(string userName, string passWord, string ip, string mac, ref long userID)
        {
            string strSql = "select * from SysUser where UserName=@UserName and Password=@Password";
            SqlParameter[] cmdParms = { 
                new SqlParameter("@UserName", SqlDbType.NVarChar), 
                new SqlParameter("@Password", SqlDbType.NVarChar)
            };
            cmdParms[0].Value = userName.Trim();
            cmdParms[1].Value = passWord;

            try
            {
                DataTable table = DbHelperSQL.Query(strSql, cmdParms).Tables[0];

                if (table.Rows.Count < 1)
                {
                    return "用户名或者密码错误";
                }
                else
                {
                    ModelHandler<SysUser> modelHandler = new ModelHandler<SysUser>();
                    DataRow dataRow = table.Rows[0];
                    SysUser user = modelHandler.FillModel(dataRow);
                    userID = user.ID;
                    //OperatorLogin(user.ID, ip, mac);
                    UpdateUserInfo(user);
                    return "登陆成功";
                }
            }
            catch
            {
                return "服务器连接失败";
            }
        }

        public static string ModifyPassWord(long id, string oldPW, string newPW)
        {
            lock (_userCollection)
            {
                if (!_userCollection.ContainsKey(id))
                {
                    return "修改失败，原因：不存在此用户！";
                }
            }

            string strSql = "update SysUser set Password=@newPW where ID=@ID and Password=@oldPW";
            SqlParameter[] cmdParms = { 
                new SqlParameter("@newPW", SqlDbType.NVarChar),
                new SqlParameter("@ID", SqlDbType.BigInt),
                new SqlParameter("@oldPW", SqlDbType.NVarChar)
            };
            cmdParms[0].Value = newPW;
            cmdParms[1].Value = id;
            cmdParms[2].Value = oldPW;

            try
            {
                int i = DbHelperSQL.ExecuteSql(strSql, cmdParms);
                if (i > 0)
                {
                    if (_userCollection.ContainsKey(id))
                    {
                        _userCollection[id].Password = newPW;
                    }
                    else
                    {
                        UpdateUserInfo(id);
                    }
                    return "修改成功";
                }
                else
                {
                    return "修改失败";
                }
            }
            catch
            {
                return "服务器连接失败";
            }
        }

        public static bool Exists(string UserName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from SysUser");
            strSql.Append(" where UserName=@UserName ");
            SqlParameter[] parameters = {
					new SqlParameter("@UserName", SqlDbType.NVarChar,50)			
            };
            parameters[0].Value = UserName;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        public static string AddUser(SysUser user)
        {
            if (Exists(user.UserName))
            {
                return "添加失败，原因：用户名已存在！";
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into SysUser(");
            strSql.Append("UserName,Password,RegDate,LogonDate,LogonIP,LogonMac,IsAllow,RoleId,DistrictId,TrueName,Sex,Mobile,Address,Remark)");
            strSql.Append(" values (");
            strSql.Append("@UserName,@Password,@RegDate,@LogonDate,@LogonIP,@LogonMac,@IsAllow,@RoleId,@DistrictId,@TrueName,@Sex,@Mobile,@Address,@Remark)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@UserName", SqlDbType.NVarChar,50),
					new SqlParameter("@Password", SqlDbType.NVarChar,50),
					new SqlParameter("@RegDate", SqlDbType.DateTime),
					new SqlParameter("@LogonDate", SqlDbType.DateTime),
					new SqlParameter("@LogonIP", SqlDbType.NVarChar,50),
					new SqlParameter("@LogonMac", SqlDbType.NVarChar,50),
					new SqlParameter("@IsAllow", SqlDbType.Int,4),
					new SqlParameter("@RoleId", SqlDbType.BigInt,8),
					new SqlParameter("@DistrictId", SqlDbType.BigInt,8),
					new SqlParameter("@TrueName", SqlDbType.NVarChar,50),
					new SqlParameter("@Sex", SqlDbType.NVarChar,50),
					new SqlParameter("@Mobile", SqlDbType.NVarChar,50),
					new SqlParameter("@Address", SqlDbType.NVarChar,100),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1)
            };
            parameters[0].Value = user.UserName;
            parameters[1].Value = user.Password;
            parameters[2].Value = user.RegDate;
            parameters[3].Value = user.LogonDate;
            parameters[4].Value = user.LogonIP;
            parameters[5].Value = user.LogonMac;
            parameters[6].Value = user.IsAllow;
            parameters[7].Value = user.RoleId;
            parameters[8].Value = user.DistrictId;
            parameters[9].Value = user.TrueName;
            parameters[10].Value = user.Sex;
            parameters[11].Value = user.Mobile;
            parameters[12].Value = user.Address;
            parameters[13].Value = user.Remark;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return "添加失败，原因：写入数据库失败！";
            }
            else
            {
                UpdateUserInfo(user);
                return "添加成功";
            }
        }

        public static string ModifyUser(SysUser user)
        {
            lock (_userCollection)
            {
                if (!_userCollection.ContainsKey(user.ID))
                {
                    return "修改失败，原因：不存在此用户！";
                }
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update SysUser set ");
            strSql.Append("Password=@Password,");
            strSql.Append("RegDate=@RegDate,");
            strSql.Append("LogonDate=@LogonDate,");
            strSql.Append("LogonIP=@LogonIP,");
            strSql.Append("LogonMac=@LogonMac,");
            strSql.Append("IsAllow=@IsAllow,");
            strSql.Append("RoleId=@RoleId,");
            strSql.Append("DistrictId=@DistrictId,");
            strSql.Append("TrueName=@TrueName,");
            strSql.Append("Sex=@Sex,");
            strSql.Append("Mobile=@Mobile,");
            strSql.Append("Address=@Address,");
            strSql.Append("Remark=@Remark");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
					new SqlParameter("@Password", SqlDbType.NVarChar,50),
					new SqlParameter("@RegDate", SqlDbType.DateTime),
					new SqlParameter("@LogonDate", SqlDbType.DateTime),
					new SqlParameter("@LogonIP", SqlDbType.NVarChar,50),
					new SqlParameter("@LogonMac", SqlDbType.NVarChar,50),
					new SqlParameter("@IsAllow", SqlDbType.Int,4),
					new SqlParameter("@RoleId", SqlDbType.BigInt,8),
					new SqlParameter("@DistrictId", SqlDbType.BigInt,8),
					new SqlParameter("@TrueName", SqlDbType.NVarChar,50),
					new SqlParameter("@Sex", SqlDbType.NVarChar,50),
					new SqlParameter("@Mobile", SqlDbType.NVarChar,50),
					new SqlParameter("@Address", SqlDbType.NVarChar,100),
					new SqlParameter("@Remark", SqlDbType.NVarChar,-1),
					new SqlParameter("@ID", SqlDbType.BigInt,8),
					new SqlParameter("@UserName", SqlDbType.NVarChar,50)};
            parameters[0].Value = user.Password;
            parameters[1].Value = user.RegDate;
            parameters[2].Value = user.LogonDate;
            parameters[3].Value = user.LogonIP;
            parameters[4].Value = user.LogonMac;
            parameters[5].Value = user.IsAllow;
            parameters[6].Value = user.RoleId;
            parameters[7].Value = user.DistrictId;
            parameters[8].Value = user.TrueName;
            parameters[9].Value = user.Sex;
            parameters[10].Value = user.Mobile;
            parameters[11].Value = user.Address;
            parameters[12].Value = user.Remark;
            parameters[13].Value = user.ID;
            parameters[14].Value = user.UserName;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                UpdateUserInfo(user);
                return "修改成功";
            }
            else
            {
                return "修改失败，原因：写入数据库失败！";
            }
        }

        public static string DeleteUser(long userId)
        {
            lock (_userCollection)
            {
                if (!_userCollection.ContainsKey(userId))
                {
                    return "删除失败：原因：该用户不存在！";
                }
            }

            string strSql = "delete SysUser where ID=@ID";
            SqlParameter[] cmdParms = new SqlParameter[]{ 
                new SqlParameter("@ID", SqlDbType.BigInt)
            };
            cmdParms[0].Value = userId;
            try
            {
                int rows = DbHelperSQL.ExecuteSql(strSql, cmdParms);
                if (rows > 0)
                {
                    RemoveUserInfo(userId);
                    return "删除成功";
                }
                else
                {
                    return "删除失败，原因：写入数据库失败！";
                }
            }
            catch
            {
                return "删除用户失败，原因：写入数据库失败！";
            }
        }

        public static string OperatorLogin(long userId, string ip, string mac)
        {
            string strSql = "update SysUser set LogonDate=getDate(),LogonIP=@LogonIP,LogonMac=@LogonMac where Id=@ID";
            SqlParameter[] cmdParms = { 
                new SqlParameter("@LogonIP", SqlDbType.NVarChar, 50),
                new SqlParameter("@LogonMac", SqlDbType.NVarChar, 50),
                new SqlParameter("@ID", SqlDbType.BigInt)
            };
            cmdParms[0].Value = ip;
            cmdParms[1].Value = mac;
            cmdParms[2].Value = userId;

            try
            {
                if (DbHelperSQL.ExecuteSql(strSql, cmdParms) > 0)
                {
                    return "更新成功";

                }
            }
            catch { }

            return "更新登录时间失败"; 
        }

        #region 从缓存获取用户信息

        public static List<SysUser> GetAllUser()
        {
            List<SysUser> list = new List<SysUser>();
            lock (_userCollection)
            {
                foreach (KeyValuePair<long, SysUser> pair in _userCollection)
                {
                    list.Add(Tools.Copy<SysUser>(pair.Value));
                }
            }
            return list;
        }

        public static List<string> GetAllUsersName()
        {
            List<string> list = new List<string>();
            lock (_userCollection)
            {
                foreach (KeyValuePair<long, SysUser> pair in _userCollection)
                {
                    list.Add(pair.Value.UserName);
                }
            }
            return list;
        }

        public static List<SysUser> GetUserListByDistrict(long districtId)
        {
            List<SysUser> list = new List<SysUser>();
            lock (_userCollection)
            {
                foreach (KeyValuePair<long, SysUser> pair in _userCollection)
                {
                    if (pair.Value.DistrictId == districtId)
                    {
                        list.Add(Tools.Copy<SysUser>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static List<SysUser> GetUserListByRole(long roleId)
        {
            List<SysUser> list = new List<SysUser>();
            lock (_userCollection)
            {
                foreach (KeyValuePair<long, SysUser> pair in _userCollection)
                {
                    if (pair.Value.RoleId == roleId)
                    {
                        list.Add(Tools.Copy<SysUser>(pair.Value));
                    }
                }
            }
            return list;
        }

        public static long[] GetUserIdArray()
        {
            long[] array = new long[0];
            lock (_userCollection)
            {
                array = new long[_userCollection.Count];
                _userCollection.Keys.CopyTo(array, 0);
            }
            return array;
        }

        public static long GetUserId(string userName)
        {
            lock (_userCollection)
            {
                foreach (KeyValuePair<long, SysUser> pair in _userCollection)
                {
                    if (pair.Value.UserName == userName)
                    {
                        return pair.Value.ID;
                    }
                }
                return -1;
            }
        }

        public static SysUser GetUser(long userId)
        {
            SysUser user = null;
            lock (_userCollection)
            {
                if (_userCollection.ContainsKey(userId))
                {
                    user = Tools.Copy<SysUser>(_userCollection[userId]);
                }
            }
            return user;
        }

        public static string GetUserName(long userId)
        {
            string userName = "";
            lock (_userCollection)
            {
                if (_userCollection.ContainsKey(userId))
                {
                    userName = _userCollection[userId].UserName;
                }
            }
            return userName;
        }

        #endregion

        #region 更新缓存

        public static void UpdateUserInfo()
        {
            string strSql = "select * from SysUser";
            try
            {
                DataTable table = DbHelperSQL.Query(strSql).Tables[0];
                if (table.Rows.Count != 0)
                {
                    ModelHandler<SysUser> modelHandler = new ModelHandler<SysUser>();
                    lock (_userCollection)
                    {
                        _userCollection.Clear();

                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            DataRow dataRow = table.Rows[i];
                            SysUser user = modelHandler.FillModel(dataRow);
                            if (!_userCollection.ContainsKey(user.ID))
                            {
                                _userCollection.Add(user.ID, user);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public static void UpdateUserInfo(long userId)
        {
            string strSql = "select * from SysUser where ID=@ID";
            SqlParameter[] cmdParms = { 
                new SqlParameter("@ID", SqlDbType.BigInt)
            };
            cmdParms[0].Value = userId;

            try
            {
                DataTable table = DbHelperSQL.Query(strSql, cmdParms).Tables[0];
                if (table.Rows.Count != 0)
                {
                    ModelHandler<SysUser> modelHandler = new ModelHandler<SysUser>();
                    lock (_userCollection)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            DataRow dataRow = table.Rows[i];
                            SysUser user = modelHandler.FillModel(dataRow);
                            if (!_userCollection.ContainsKey(user.ID))
                            {
                                _userCollection.Add(user.ID, user);
                            }
                            else
                            {
                                _userCollection[user.ID] = user;
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public static void UpdateUserInfo(SysUser user)
        {
            lock (_userCollection)
            {
                if (!_userCollection.ContainsKey(user.ID))
                {
                    _userCollection.Add(user.ID, user);
                }
                else
                {
                    _userCollection[user.ID] = user;
                }
            }
        }

        public static void RemoveUserInfo(long userId)
        {
            lock (_userCollection)
            {
                if (_userCollection.ContainsKey(userId))
                {
                    _userCollection.Remove(userId);
                }
            }
        }

        public static void RemoveUserInfo(SysUser user)
        {
            RemoveUserInfo(user.ID);
        }

        #endregion
    }
}
