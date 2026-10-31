using General.Catalog;
using IDS.DataAccess;
using IDS.GeneralTable;
using Microsoft.Data.SqlClient;
using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SPOS.Inventory.Stock
{
    public class StockTransactionH
    {
        public string TransNo { get; set; }
        public string TransCode { get; set; }
        public DateTime? TransDate { get; set; }
        public string BranchCode { get; set; }
        public string WarehouseCode { get; set; }  
        public int? Process { get; set; }
        public DateTime? ProcessDate { get;set; }
        public string voucher { get; set; }
        public string Remark { get; set; }
        public string OperatorID { get; set; }
        public List<StockTransactionD> StockTransactionDetails { get; set; }
        public StockTransactionH() { }

        public static List<StockTransactionH> GetStockTransaction(DateTime DateFrom,DateTime DateTo, string Branch)
        {
            List<StockTransactionH> list = new List<StockTransactionH>();

            using (SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "InvSelStockTransaction";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@DateFrom", System.Data.SqlDbType.DateTime, DateFrom);
                db.AddParameter("@DateTo", System.Data.SqlDbType.DateTime, DateTo);
                if (Branch == "All")
                {
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, DBNull.Value);
                }
                else
                {
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, Branch);
                }

                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        
                        while (dr.Read())
                        {
                            StockTransactionH stockTransaction = new StockTransactionH();
                            stockTransaction.TransNo = IDS.Tool.GeneralHelper.NullToString(dr["TransNo"]); ;
                            stockTransaction.TransCode = IDS.Tool.GeneralHelper.NullToString(dr["TransCode"]);
                            stockTransaction.TransDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["TransDate"], DateTime.Now);
                            stockTransaction.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["Branch"]);
                            stockTransaction.WarehouseCode = IDS.Tool.GeneralHelper.NullToString(dr["WHCode"]);
                            stockTransaction.Process = IDS.Tool.GeneralHelper.NullToInt(dr["Process"], 0);
                            stockTransaction.ProcessDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["ProcessDate"], DateTime.Now);
                            stockTransaction.voucher = IDS.Tool.GeneralHelper.NullToString(dr["Voucher"]);
                            stockTransaction.Remark = IDS.Tool.GeneralHelper.NullToString(dr["Remark"]);
                            // Tambahkan Header ke list
                            list.Add(stockTransaction);
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
                    cmd.CommandText = "InsUpDelStockTrans";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.ClearParameter();
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@TransNo", System.Data.SqlDbType.VarChar, TransNo);
                    cmd.AddParameter("@TransCode", System.Data.SqlDbType.VarChar, TransCode);
                    cmd.AddParameter("@TransDate", System.Data.SqlDbType.DateTime, TransDate);
                    cmd.AddParameter("@WHCode", System.Data.SqlDbType.VarChar, "RIB");
                    cmd.AddParameter("@Remark", System.Data.SqlDbType.VarChar, Remark);
                    cmd.AddParameter("@Branch", System.Data.SqlDbType.VarChar, BranchCode);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);

                    cmd.BeginTransaction();
                    if (ExecCode == 3)
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    else if (ExecCode == 2)
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        object obj = cmd.ExecuteScalar();
                        if (obj != null && obj != DBNull.Value)
                        {
                            TransNo = obj.ToString();
                        }
                    }
                   

                   
                    if (StockTransactionDetails != null && StockTransactionDetails.Count > 0)
                    {
                        cmd.CommandText = "InsUpDelStockTransD";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ClearParameter();
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 0);
                        cmd.AddParameter("@SeqNo", System.Data.SqlDbType.TinyInt, 0);
                        cmd.AddParameter("@TransNo", System.Data.SqlDbType.VarChar, TransNo);
                        cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, "");
                        cmd.AddParameter("@Qty", System.Data.SqlDbType.Money, 0);
                        cmd.AddParameter("@Remark", System.Data.SqlDbType.VarChar, "");

                        result = cmd.ExecuteNonQuery();

                        int count = 1;
                        foreach (var data in StockTransactionDetails)
                        {
                            cmd.CommandText = "InsUpDelStockTransD";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ClearParameter();
                            cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 1);
                            cmd.AddParameter("@SeqNo", System.Data.SqlDbType.Int, count);
                            cmd.AddParameter("@TransNo", System.Data.SqlDbType.VarChar, TransNo);
                            cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, data.ProdCode);
                            cmd.AddParameter("@Qty", System.Data.SqlDbType.Decimal, data.Qty);
                            cmd.AddParameter("@Remark", System.Data.SqlDbType.VarChar, data.Remark);
                            cmd.AddParameter("@ToBranch", System.Data.SqlDbType.VarChar,
                                string.IsNullOrWhiteSpace(data.ToBranch) ? (object)DBNull.Value : data.ToBranch);
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


        
        public static int ProcessStock(List<string> transNos,int process,ref string msg)
        {
            int result = 0;
            DateTime today = DateTime.Now;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.Open();
                 
                    cmd.CommandText = "update stocktransh set process=@Process, processdate=@Today where transno=@TransNo";
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.BeginTransaction();
                    foreach (var i in transNos)
                    {
                        cmd.ClearParameter();
                        cmd.AddParameter("@Process", System.Data.SqlDbType.TinyInt, process);
                        cmd.AddParameter("@TransNo", System.Data.SqlDbType.VarChar, i);
                        cmd.AddParameter("@Today", System.Data.SqlDbType.DateTime, today);
                        result = cmd.ExecuteNonQuery();
                        if (result < 0)
                        {
                            cmd.RollbackTransaction();
                            result = 0;
                        }
                    }
                    foreach(var i in transNos)
                    {
                        string BranchCode = "";
                        string TransCode = "";
                        DateTime TransDate = DateTime.Now;
                        List<StockTransactionD> listD = new List<StockTransactionD>();
                        cmd.CommandText = "select stocktransH.Branch,stocktransH.TransCode,stocktransH.TransDate,StockTransD.* from stocktransH inner join StockTransD on StockTransD.TransNo=StockTransH.TransNo where StockTransH.TransNo=@TransNo";
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ClearParameter();
                        cmd.AddParameter("@TransNo", System.Data.SqlDbType.VarChar, i);
                        cmd.ExecuteReader();
                        using (SqlDataReader dr = cmd.DbDataReader as SqlDataReader)
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    StockTransactionD stockTransaction = new StockTransactionD();
                                    stockTransaction.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]); ;
                                    BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["Branch"]);
                                    stockTransaction.ToBranch= IDS.Tool.GeneralHelper.NullToString(dr["ToBranch"]);
                                    TransCode = IDS.Tool.GeneralHelper.NullToString(dr["TransCode"]);
                                    TransDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["TransDate"],DateTime.Now);
                                    stockTransaction.Qty = IDS.Tool.GeneralHelper.NullToDecimal(dr["Qty"], 0);
                                    listD.Add(stockTransaction);
                                }
                            }

                            if (!dr.IsClosed)
                                dr.Close();
                        }
                        foreach(var a in listD)
                        {
                            if(TransCode == "STKTF")
                            {
                                //cek dulu ada stocknya ga di db untuk transaksi ini 
                                cmd.CommandText = "SELECT " +
                                                    "ISNULL(QtyIn, 0) + " +
                                                    "ISNULL(QtyOut, 0) + " +
                                                    "ISNULL(QtyAdj, 0) AS EndBal,ProdName " +
                                                    "FROM StockProduct " +
                                                    "inner join product on product.ProdCode = stockproduct.ProdCode " +
                                                    "WHERE[Date] = @StockDate " +
                                                    "AND stockproduct.ProdCode = @ProdCode " +
                                                    "AND stockproduct.Branch = @Branch ";
                                cmd.CommandType = System.Data.CommandType.Text;
                                cmd.ClearParameter();
                                cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, a.ProdCode);
                                cmd.AddParameter("@Branch", System.Data.SqlDbType.VarChar, BranchCode);
                                cmd.AddParameter("@StockDate", System.Data.SqlDbType.DateTime, TransDate);
                                object res = cmd.ExecuteScalar();

                                decimal endBal = (res == null || res == DBNull.Value)
                                    ? 0
                                    : Convert.ToDecimal(res);

                                if (endBal <= 0 || endBal<a.Qty)
                                {
                                    msg = "Stock End Bal untuk Product '" + a.ProdCode + "' kurang untuk di transfer, tolong cek kembali stock balance!";
                                    result = 0;
                                    cmd.RollbackTransaction();
                                    return result;
                                }
                            }
                           


                            cmd.CommandText = "AddQtyStock";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ClearParameter();
                            cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, a.ProdCode);
                            cmd.AddParameter("@Qty", System.Data.SqlDbType.Decimal, a.Qty);
                            cmd.AddParameter("@TransDate", System.Data.SqlDbType.DateTime, TransDate);

                            cmd.AddParameter("@Branch", System.Data.SqlDbType.VarChar, BranchCode);
                            if (TransCode == "STKIN")
                            {
                                cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 2);
                                cmd.AddParameter("@ToBranch", System.Data.SqlDbType.VarChar, DBNull.Value);
                            }
                            else if (TransCode == "STKAD")
                            {
                                cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 3);
                                cmd.AddParameter("@ToBranch", System.Data.SqlDbType.VarChar, DBNull.Value);
                            }
                            else if (TransCode == "STKTF")
                            {
                                cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 4);
                                cmd.AddParameter("@ToBranch", System.Data.SqlDbType.VarChar, a.ToBranch);
                            }
                          
                            result = cmd.ExecuteNonQuery();
                            if (result < 0)
                            {
                                cmd.RollbackTransaction();
                                result = 0;
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



        public static StockTransactionH GetStockTransactionHD(string TransNo)
        {
            StockTransactionH Data1 = null;
            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select StockTransH.*,StockTransD.ProdCode,StockTransD.SeqNo,StockTransD.Qty,StockTransD.Remark,StockTransD.ToBranch from StockTransH inner join stocktransD on StockTransD.TransNo=StockTransH.Transno where StockTransH.Transno=@TransNo";
                db.AddParameter("@TransNo", System.Data.SqlDbType.VarChar, TransNo);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    Data1 = new StockTransactionH();
                    Data1.StockTransactionDetails = new List<StockTransactionD>();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            StockTransactionD details = new StockTransactionD();
                            Data1.TransNo = IDS.Tool.GeneralHelper.NullToString(dr["TransNo"]);
                            Data1.TransCode = IDS.Tool.GeneralHelper.NullToString(dr["TransCode"]);
                            Data1.TransDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["TransDate"], DateTime.Now);
                            Data1.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["Branch"]);
                            Data1.WarehouseCode = IDS.Tool.GeneralHelper.NullToString(dr["WHCode"]);
                            Data1.Process = IDS.Tool.GeneralHelper.NullToInt(dr["Process"], 0);
                            Data1.ProcessDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["ProcessDate"], DateTime.Now);
                            Data1.Remark = IDS.Tool.GeneralHelper.NullToString(dr["Remark"]);
                            details.SeqNo = IDS.Tool.GeneralHelper.NullToInt(dr["SeqNo"],0);
                            details.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            details.Qty = IDS.Tool.GeneralHelper.NullToDecimal(dr["Qty"], 0);
                            details.Remark = IDS.Tool.GeneralHelper.NullToString(dr["Remark"]);
                            details.ToBranch = IDS.Tool.GeneralHelper.NullToString(dr["ToBranch"]);
                            Data1.StockTransactionDetails.Add(details);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return Data1;
        }

    }
}
