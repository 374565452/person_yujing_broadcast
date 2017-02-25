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
    public class CropModule
    {
        private static Dictionary<long, Crop> _CropInfos = new Dictionary<long, Crop>();

        /// <summary>
        /// 是否相同名称
        /// </summary>
        public static bool ExistsCropName(string CropName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Crop");
            strSql.Append(" where (CropName=@CropName)");
            SqlParameter[] parameters = {
                    new SqlParameter("@CropName", SqlDbType.VarChar,50)
			};
            parameters[0].Value = CropName;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 是否相同名称
        /// </summary>
        public static bool ExistsCropName(long Id, string CropName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Crop");
            strSql.Append(" where Id<>@Id and (CropName=@CropName)");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt),
                    new SqlParameter("@CropName", SqlDbType.VarChar,50)
			};
            parameters[0].Value = Id;
            parameters[1].Value = CropName;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        public static string AddCropInfo(Crop crop)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into Crop(");
            strSql.Append("CropName,WaterPerMu,Remark)");
            strSql.Append(" values (");
            strSql.Append("@CropName,@WaterPerMu,@Remark)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@CropName", SqlDbType.NVarChar,50),
					new SqlParameter("@WaterPerMu", SqlDbType.Decimal,9),
					new SqlParameter("@Remark", SqlDbType.NVarChar,200)};
            parameters[0].Value = crop.CropName;
            parameters[1].Value = crop.WaterPerMu;
            parameters[2].Value = crop.Remark;

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

        public static string ModifyCropInfo(Crop crop)
        {
            if (!_CropInfos.ContainsKey(crop.Id))
            {
                return ("修改失败，原因：不存在ID为" + crop.Id + "的作物！");
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Crop set ");
            strSql.Append("CropName=@CropName,");
            strSql.Append("WaterPerMu=@WaterPerMu,");
            strSql.Append("Remark=@Remark");
            strSql.Append(" where Id=@Id");
            SqlParameter[] parameters = {
					new SqlParameter("@CropName", SqlDbType.NVarChar,50),
					new SqlParameter("@WaterPerMu", SqlDbType.Decimal,9),
					new SqlParameter("@Remark", SqlDbType.NVarChar,200),
					new SqlParameter("@Id", SqlDbType.BigInt,8)};
            parameters[0].Value = crop.CropName;
            parameters[1].Value = crop.WaterPerMu;
            parameters[2].Value = crop.Remark;
            parameters[3].Value = crop.Id;

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

        public static string DeleteCropInfo(long id)
        {
            if (!_CropInfos.ContainsKey(id))
            {
                return ("删除失败，原因：不存在ID为" + id + "的作物！");
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Crop ");
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

        public static List<Crop> GetAllCrop()
        {
            List<Crop> list = new List<Crop>();
            lock (_CropInfos)
            {
                foreach (KeyValuePair<long, Crop> pair in _CropInfos)
                {
                    list.Add(Tools.Copy<Crop>(pair.Value));
                }
            }
            return list;
        }

        public static Crop GetCrop(long Id)
        {
            Crop crop = null;
            lock (_CropInfos)
            {
                if (_CropInfos.ContainsKey(Id))
                {
                    crop = Tools.Copy<Crop>(_CropInfos[Id]);
                }
            }
            return crop;
        }

        public static List<Crop> GetCropInfosByIds(string[] ids)
        {
            List<Crop> list = new List<Crop>();
            lock (_CropInfos)
            {
                foreach (KeyValuePair<long, Crop> pair in _CropInfos)
                {
                    if (ids.Contains(pair.Key.ToString()))
                        list.Add(Tools.Copy<Crop>(pair.Value));
                }
            }
            return list;
        }

        public static void LoadUnitQuotaInfos()
        {
            ModelHandler<Crop> modelHandler = new ModelHandler<Crop>();

            _CropInfos.Clear();
            DataTable table = new DataTable();
            string strSql = "select * from Crop";
            table = DbHelperSQL.Query(strSql).Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Crop crop = modelHandler.FillModel(table.Rows[i]);
                if (!_CropInfos.ContainsKey(crop.Id))
                {
                    _CropInfos.Add(crop.Id, crop);
                }
                else
                {
                    _CropInfos[crop.Id] = crop;
                }
            }
        }
    }
}
