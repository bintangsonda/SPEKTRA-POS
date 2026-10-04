using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Tool
{
    public static class GlobalVariable
    {
        #region User Login Session
        /// <summary>
        /// Nama session untuk data User ID yang login
        /// </summary>
        public const string SESSION_USER_ID = "Login.UserID";
        /// <summary>
        /// Nama session group code untuk user yang login
        /// </summary>
        public const string SESSION_USER_GROUP_CODE = "Logi.UserGroupCode";
        /// <summary>
        /// Nama session branch code untuk user yang login
        /// </summary>
        public const string SESSION_USER_BRANCH_CODE = "Login.UserBranchCode";
        /// <summary>
        /// Nama session untuk check apakah User merupakan User branch holding atau bukan. 0 = Bukan Holding, 1 = Holding
        /// </summary>
        public const string SESSION_USER_BRANCH_HO_STATUS = "Login.HOStatus";
        /// <summary>
        /// Nama session untuk group code dari group AM (Area Manager) jika user group dari user yang login adalah user group AM
        /// </summary>
        public const string SESSION_USER_AM_GROUP = "Login.AM_GROUP";
        /// <summary>
        /// Nama session untuk menu user
        /// </summary>
        public const string SESSION_USER_MENU = "Login.UserMenu";

        public const string SESSION_USER_EMAIL = "Login.EmailAddress";
        #endregion

        #region Cache - User Access
        public const string CACHE_USER_GROUP_ACCESS = "UserGroup.Access";
        public const int CACHE_DURATION_USER_GROUP_ACCESS = 720;
        #endregion

        #region Global Cache
        public const string CACHE_HO_BRANCH_CODE = "Branch.HOBranchCode"; // Kode Cache untuk HO Branch
        #endregion

        #region Datetime
        public const string DEFAULT_DATE_FORMAT = "dd/MMM/yyyy";
        public const string DEFAULT_DATETIME_FORMAT = "dd/MMM/yyyy HH:mm:ss";
        #endregion

        #region Directories and Files
        public const string DEFAULT_JFBATCH_DIR = "JFBatchs";
        #endregion

        //public static void CacheHOBranchCode()
        //{
        //    object result = null;

        //    // Simpan kode branch HO ke cache
        //    using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
        //    {
        //        db.CommandText = "SELECT ISNULL(BranchCode, 'KPNO') AS BranchCode FROM tblBranch WHERE HOStatus = 1";
        //        db.CommandType = System.Data.CommandType.Text;
        //        db.Open();

        //        result = Convert.ToString(db.ExecuteScalar());

        //        IDS.Tool.InMemoryCache.GetInstance().GetOrSet<string>(IDS.Tool.GlobalVariable.CACHE_HO_BRANCH_CODE, () => { return result.ToString(); });

        //        db.Close();
        //    }
        //}
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

    }
}
