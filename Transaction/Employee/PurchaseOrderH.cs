using IDS.DataAccess;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.Employee
{
    public class PurchaseOrderH
    {
        public string POHNo { get; set; }
        public DateTime POHDate { get; set; }
        public string BranchCode { get; set; }
        public string Supplier { get; set; }
        public string Remark { get; set; }
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdate { get; set; }
        public decimal TotalAmt { get; set; }

        public List<PurchaseOrderD> PODetail { get; set; }


        public static List<PurchaseOrderH> GetData(string Branch)
        {
            List<PurchaseOrderH> list = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = @"
                        SELECT PurchaseOrderH.*,
                               (SELECT ISNULL(SUM(d.Qty * d.Price), 0)
                                FROM PurchaseOrderD d
                                WHERE d.POHNo = PurchaseOrderH.POHNo) AS TotalAmt
                        FROM PurchaseOrderH
                        WHERE (@Branch IS NULL OR PurchaseOrderH.BranchCode = @Branch)
                    ";
                if (Branch == "All" || Branch == null)
                {
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, DBNull.Value);
                }
                else
                {
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, Branch);
                }
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<PurchaseOrderH>();

                        while (dr.Read())
                        {
                            PurchaseOrderH Datatables = new PurchaseOrderH();
                            Datatables.POHNo = IDS.Tool.GeneralHelper.NullToString(dr["POHNo"]);
                            Datatables.POHDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["POHDate"], DateTime.MinValue);
                            Datatables.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            Datatables.Supplier = IDS.Tool.GeneralHelper.NullToString(dr["Supplier"]);
                            Datatables.Remark = IDS.Tool.GeneralHelper.NullToString(dr["Remark"]);
                            Datatables.Status = IDS.Tool.GeneralHelper.NullToInt(dr["Status"], 0);
                            Datatables.CreatedBy = IDS.Tool.GeneralHelper.NullToString(dr["CreatedBy"]);
                            Datatables.CreatedDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["CreatedDate"], DateTime.MinValue);
                            Datatables.LastUpdateBy = IDS.Tool.GeneralHelper.NullToString(dr["LastUpdateBy"]);
                            Datatables.LastUpdate = IDS.Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.MinValue);
                            Datatables.TotalAmt = IDS.Tool.GeneralHelper.NullToDecimal(dr["TotalAmt"], 0);

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
                    cmd.CommandText = "InsPurchaseOrder";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    var paramPOHNo = new Microsoft.Data.SqlClient.SqlParameter(
                        "@POHNo",
                        System.Data.SqlDbType.VarChar, 20
                    );
                    paramPOHNo.Direction = System.Data.ParameterDirection.InputOutput;
                    paramPOHNo.Value = (object)POHNo ?? "";
                    cmd.DbCommand.Parameters.Add(paramPOHNo);

                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    if (ExecCode == 3)
                    {
                        cmd.AddParameter("@POHDate", System.Data.SqlDbType.DateTime, DateTime.Now);

                    }
                    else
                    {
                        cmd.AddParameter("@POHDate", System.Data.SqlDbType.DateTime, POHDate);

                    }
                    cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                    cmd.AddParameter("@Supplier", System.Data.SqlDbType.VarChar, Supplier);
                    cmd.AddParameter("@Remark", System.Data.SqlDbType.VarChar, Remark);
                    cmd.AddParameter("@Status", System.Data.SqlDbType.TinyInt, Status);
                    cmd.AddParameter("@CreatedBy", System.Data.SqlDbType.VarChar, CreatedBy);
                    cmd.AddParameter("@LastUpdateBy", System.Data.SqlDbType.VarChar, LastUpdateBy);

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    if (ExecCode == 1)
                    {
                        POHNo = Convert.ToString(paramPOHNo.Value);
                    }
                    if (ExecCode != 3)
                    {
                        // Detail: hapus semua detail lama dulu (untuk edit), lalu insert ulang per baris
                        cmd.CommandText = "InsPurchaseOrderDetail";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 0);
                        cmd.AddParameter("@POHNo", System.Data.SqlDbType.VarChar, POHNo);
                        result = cmd.ExecuteNonQuery();

                        if (PODetail != null && PODetail.Count > 0)
                        {
                            int seqNo = 1;
                            foreach (var data in PODetail)
                            {
                                cmd.CommandText = "InsPurchaseOrderDetail";
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                                cmd.AddParameter("@POHNo", System.Data.SqlDbType.VarChar, POHNo);
                                cmd.AddParameter("@SeqNo", System.Data.SqlDbType.Int, seqNo);
                                cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, data.ProdCode);
                                cmd.AddParameter("@UOM", System.Data.SqlDbType.VarChar, data.UOM);
                                cmd.AddParameter("@Qty", System.Data.SqlDbType.Decimal, data.Qty);
                                cmd.AddParameter("@Price", System.Data.SqlDbType.Decimal, data.Price);
                                result = cmd.ExecuteNonQuery();
                                seqNo++;
                            }
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
                            throw new Exception("Purchase Order number is already exists. Please try again.");
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

        public static PurchaseOrderH GetDataByPOHNo(string POHNo)
        {
            PurchaseOrderH data = null;

            using (SqlServer db = new SqlServer())
            {
                // ===== 1. GET HEADER =====
                db.CommandText = "select * from PurchaseOrderH where POHNo=@POHNo";
                db.AddParameter("@POHNo", System.Data.SqlDbType.VarChar, POHNo);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (var dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows && dr.Read())
                    {
                        data = new PurchaseOrderH();
                        data.POHNo = IDS.Tool.GeneralHelper.NullToString(dr["POHNo"]);
                        data.POHDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["POHDate"], DateTime.MinValue);
                        data.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                        data.Supplier = IDS.Tool.GeneralHelper.NullToString(dr["Supplier"]);
                        data.Remark = IDS.Tool.GeneralHelper.NullToString(dr["Remark"]);
                        data.Status = IDS.Tool.GeneralHelper.NullToInt(dr["Status"], 0);
                        data.CreatedBy = IDS.Tool.GeneralHelper.NullToString(dr["CreatedBy"]);
                        data.CreatedDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["CreatedDate"], DateTime.MinValue);
                        data.LastUpdateBy = IDS.Tool.GeneralHelper.NullToString(dr["LastUpdateBy"]);
                        data.LastUpdate = IDS.Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.MinValue);
                        data.PODetail = new List<PurchaseOrderD>();
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();

                // ===== 2. GET DETAIL =====
                if (data != null)
                {
                    db.ClearParameter();
                    db.CommandText = @"
                            SELECT PurchaseOrderD.*, Product.ProdName
                            FROM PurchaseOrderD
                            LEFT JOIN Product
                                ON Product.ProdCode = PurchaseOrderD.ProdCode
                                AND Product.Branch = @Branch
                            WHERE PurchaseOrderD.POHNo = @POHNo
                            ORDER BY PurchaseOrderD.SeqNo
                        ";
                    db.AddParameter("@POHNo", System.Data.SqlDbType.VarChar, data.POHNo);
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, data.BranchCode);
                    db.CommandType = System.Data.CommandType.Text;
                    db.Open();
                    db.ExecuteReader();

                    using (var drDetail = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                    {
                        if (drDetail.HasRows)
                        {
                            while (drDetail.Read())
                            {
                                data.PODetail.Add(new PurchaseOrderD
                                {
                                    POHNo = IDS.Tool.GeneralHelper.NullToString(drDetail["POHNo"]),
                                    SeqNo = IDS.Tool.GeneralHelper.NullToInt(drDetail["SeqNo"], 0),
                                    ProdCode = IDS.Tool.GeneralHelper.NullToString(drDetail["ProdCode"]),
                                    ProdName = IDS.Tool.GeneralHelper.NullToString(drDetail["ProdName"]),
                                    UOM = IDS.Tool.GeneralHelper.NullToString(drDetail["UOM"]),
                                    Qty = IDS.Tool.GeneralHelper.NullToDecimal(drDetail["Qty"], 0),
                                    Price = IDS.Tool.GeneralHelper.NullToDecimal(drDetail["Price"], 0)
                                });
                            }
                        }

                        if (!drDetail.IsClosed)
                            drDetail.Close();
                    }

                    db.Close();
                }
            }
            return data;
        }

        public int Proses()
        {
            int result = 0;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.Open();
                    cmd.CommandText = "ProsesPurchaseOrder";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@POHNo", System.Data.SqlDbType.VarChar, POHNo);
                    cmd.AddParameter("@POHDate", System.Data.SqlDbType.DateTime, POHDate);
                    cmd.AddParameter("@LastUpdateBy", System.Data.SqlDbType.VarChar, LastUpdateBy);

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                }
                catch (SqlException)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    throw;
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

        public int CancelProses()
        {
            int result = 0;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.Open();
                    cmd.CommandText = "CancelPurchaseOrder";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@POHNo", System.Data.SqlDbType.VarChar, POHNo);
                    cmd.AddParameter("@POHDate", System.Data.SqlDbType.DateTime, POHDate);
                    cmd.AddParameter("@LastUpdateBy", System.Data.SqlDbType.VarChar, LastUpdateBy);

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                }
                catch (SqlException)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    throw;
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

    }
}
