using IDS.DataAccess;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Catalog
{
    public class VariantHeader
    {
        [Display(Name = "Variance Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Variance Code is required")]
        [MaxLength(5), StringLength(5)]
        public string VarianCode { get; set; }

        [Display(Name = "Variance Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Variance Name is required")]
        [MaxLength(50), StringLength(50)]
        public string VarianName { get; set; }

        public bool VarianStatus { get; set; }
        public bool VarianMode { get; set; }
        public bool VarianType { get; set; }
        [Range(short.MinValue, short.MaxValue, ErrorMessage = "Minimal harus dalam range Int16.")]
        public int Minimal { get; set; }

        [Range(short.MinValue, short.MaxValue, ErrorMessage = "Maximal harus dalam range Int16.")]
        public int Maximal { get; set; }

        public string OperatorID { get; set; }
        public DateTime LastUpdate { get; set; }

        public List<VariantDetail> VariantDetails { get; set; }

        public VariantHeader() {
        
        }

        public static List<VariantHeader> GetData()
        {
            List<VariantHeader> list = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "Select * from VariantH";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<VariantHeader>();

                        while (dr.Read())
                        {
                            VariantHeader Datatables = new VariantHeader();
                            Datatables.VarianCode = IDS.Tool.GeneralHelper.NullToString(dr["VarianCode"]);
                            Datatables.VarianName = IDS.Tool.GeneralHelper.NullToString(dr["VarianName"]);
                            Datatables.VarianStatus = IDS.Tool.GeneralHelper.NullToBool(dr["VarianStatus"],false);
                            Datatables.VarianMode = IDS.Tool.GeneralHelper.NullToBool(dr["VarianMode"], false);
                            Datatables.VarianType = IDS.Tool.GeneralHelper.NullToBool(dr["VarianType"], false);
                            Datatables.Minimal = IDS.Tool.GeneralHelper.NullToInt16(dr["Minimal"], 0);
                            Datatables.Maximal = IDS.Tool.GeneralHelper.NullToInt16(dr["Maximal"], 0);
                            Datatables.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            Datatables.LastUpdate = IDS.Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.Now);

                            list.Add(Datatables);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }

        public int InsUpDel(int ExecCode)
        {
            int result = 0;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.Open();
                    cmd.CommandText = "InsVariant";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@VariantCode", System.Data.SqlDbType.VarChar, VarianCode);
                    cmd.AddParameter("@VariantName", System.Data.SqlDbType.VarChar, VarianName);
                    cmd.AddParameter("@VariantStatus", System.Data.SqlDbType.Bit, VarianStatus);
                    cmd.AddParameter("@VariantMode", System.Data.SqlDbType.Bit, VarianMode);
                    cmd.AddParameter("@VariantType", System.Data.SqlDbType.Bit, VarianType);
                    cmd.AddParameter("@Minimal", System.Data.SqlDbType.Int, Minimal);
                    cmd.AddParameter("@Maximal", System.Data.SqlDbType.Int,Maximal);

                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();

                    if (VariantDetails!=null && VariantDetails.Count>0)
                    {
                        cmd.CommandText = "InsVariantDetail";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 0);
                        cmd.AddParameter("@SeqNo", System.Data.SqlDbType.TinyInt, 0);
                        cmd.AddParameter("@VariantCode", System.Data.SqlDbType.VarChar, VarianCode);
                        cmd.AddParameter("@Name", System.Data.SqlDbType.VarChar, "empty");
                        cmd.AddParameter("@Harga", System.Data.SqlDbType.Money, 0);
                        result = cmd.ExecuteNonQuery();

                        int count = 1;
                        foreach (var data in VariantDetails)
                        {
                            cmd.CommandText = "InsVariantDetail";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                            cmd.AddParameter("@SeqNo", System.Data.SqlDbType.TinyInt, count);
                            cmd.AddParameter("@VariantCode", System.Data.SqlDbType.VarChar, VarianCode);
                            cmd.AddParameter("@Name", System.Data.SqlDbType.VarChar, data.Name);
                            cmd.AddParameter("@Harga", System.Data.SqlDbType.Money, data.Value);
                            result = cmd.ExecuteNonQuery();
                            count++;
                        }
                    }
                    cmd.CommitTransaction();
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Category code is already exists. Please choose other loan code.");
                        default:
                            throw;
                    }
                }
                catch
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    throw;
                }
                finally
                {
                    cmd.Close();
                }
            }

            return result;
        }

        public static VariantHeader GetDataByVariantCode(string VariantCode)
        {
            VariantHeader Data1 = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select VariantH.*,VariantD.Name,VariantD.Harga from VariantH left join VariantD on variantd.variancode=varianth.VarianCode  where VariantH.VarianCode=@VariantCode";
                db.AddParameter("@VariantCode", System.Data.SqlDbType.VarChar, VariantCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    Data1 = new VariantHeader();
                    Data1.VariantDetails= new List<VariantDetail>();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            VariantDetail details = new VariantDetail();
                            Data1.VarianCode = IDS.Tool.GeneralHelper.NullToString(dr["VarianCode"]);
                            Data1.VarianName = IDS.Tool.GeneralHelper.NullToString(dr["VarianName"]);
                            Data1.VarianStatus = IDS.Tool.GeneralHelper.NullToBool(dr["VarianStatus"], true);
                            Data1.VarianType = IDS.Tool.GeneralHelper.NullToBool(dr["VarianType"], true);
                            Data1.VarianMode = IDS.Tool.GeneralHelper.NullToBool(dr["VarianMode"], true);
                            Data1.Minimal = IDS.Tool.GeneralHelper.NullToInt(dr["Minimal"], 0);
                            Data1.Maximal = IDS.Tool.GeneralHelper.NullToInt(dr["Maximal"], 0);
                            details.Name = IDS.Tool.GeneralHelper.NullToString(dr["Name"]);
                            details.Value = IDS.Tool.GeneralHelper.NullToDecimal(dr["Harga"], 0);
                            Data1.VariantDetails.Add(details);
                        }
                        
                       
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return Data1;
        }
        public static List<VariantHeader> GetVariantProduct(string ProdCode)
        {
            List<VariantHeader> result = new List<VariantHeader>();

            using (SqlServer db = new SqlServer())
            {
                // ===== 1. GET HEADER =====
                db.CommandText = @"
            SELECT DISTINCT H.*
            FROM ProductVariant P
            INNER JOIN VariantH H ON H.VarianCode = P.VariantCode
            WHERE P.ProdCode = @ProdCode";

                db.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, ProdCode);
                db.Open();

                db.ExecuteReader();
                using (var dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    while (dr.Read())
                    {
                        VariantHeader header = new VariantHeader
                        {
                            VarianCode = IDS.Tool.GeneralHelper.NullToString(dr["VarianCode"]),
                            VarianName = IDS.Tool.GeneralHelper.NullToString(dr["VarianName"]),
                            VarianStatus = IDS.Tool.GeneralHelper.NullToBool(dr["VarianStatus"], true),
                            VarianType = IDS.Tool.GeneralHelper.NullToBool(dr["VarianType"], true),
                            VarianMode = IDS.Tool.GeneralHelper.NullToBool(dr["VarianMode"], true),
                            Minimal = IDS.Tool.GeneralHelper.NullToInt(dr["Minimal"], 0),
                            Maximal = IDS.Tool.GeneralHelper.NullToInt(dr["Maximal"], 0),
                            VariantDetails = new List<VariantDetail>()
                        };

                        result.Add(header);
                    }
                }

                db.Close();

                // ===== 2. GET DETAIL PER HEADER =====
                foreach (var header in result)
                {
                    db.CommandText = @"
                SELECT D.Name, D.Harga ,D.SeqNo
                FROM ProductVariant P
                INNER JOIN VariantD D ON D.VarianCode = P.VariantCode
                WHERE P.ProdCode = @ProdCode
                AND D.VarianCode = @VarianCode";

                    db.ClearParameter();
                    db.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, ProdCode);
                    db.AddParameter("@VarianCode", System.Data.SqlDbType.VarChar, header.VarianCode);

                    db.Open();
                    db.ExecuteReader();

                    using (var drDetail = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                    {
                        while (drDetail.Read())
                        {
                            header.VariantDetails.Add(new VariantDetail
                            {
                                Name = IDS.Tool.GeneralHelper.NullToString(drDetail["Name"]),
                                SeqNo = IDS.Tool.GeneralHelper.NullToInt(drDetail["SeqNo"], 0),
                                Value = IDS.Tool.GeneralHelper.NullToDecimal(drDetail["Harga"], 0)
                            });
                        }
                    }

                    db.Close();
                }
            }

            return result;
        }

    }
}
