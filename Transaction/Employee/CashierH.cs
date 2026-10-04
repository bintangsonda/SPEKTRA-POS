using General.Catalog;
using IDS.DataAccess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Transaction.Employee
{
    public class CashierH
    {
        public string TransCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string OperatorID { get; set; }
        public DateTime TransDate { get; set; }
        public string CustID { get; set; }
        public int Status { get; set; }

        public decimal TransTotalAmt { get; set; }
        public decimal TransTotalPaid { get; set; }
        public decimal TransSubTotalAmt { get; set; }
        public decimal TransTax { get; set; }
        public decimal TransTaxPercentage { get; set; }
        public string TransPayMtd { get; set; }
        public string TransPayType { get; set; }
        public string TransRemark { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<CashierD> Details { get; set; }

        public static List<CashierH> GetData(DateTime TransDateFrom, DateTime TransDateTo,string TransMethod,string CustName)
        {
            List<CashierH> list = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select * from TransInvH where TransDate >=@TransDateFrom and TransDate <=@TransDateTo and TransPayMtd like isnull(@TransMethod,'%') and ( @CustName IS NULL OR CustID LIKE @CustName ) and Status=1";
                db.AddParameter("@TransDateFrom", System.Data.SqlDbType.DateTime, TransDateFrom);
                db.AddParameter("@TransDateTo", System.Data.SqlDbType.DateTime, TransDateTo);
                if(string.IsNullOrEmpty(TransMethod))
                    db.AddParameter("@TransMethod", System.Data.SqlDbType.VarChar, DBNull.Value);
                else
                    db.AddParameter("@TransMethod", System.Data.SqlDbType.VarChar, TransMethod);
                if (string.IsNullOrEmpty(CustName)) 
                    db.AddParameter("@CustName", System.Data.SqlDbType.VarChar, DBNull.Value);
                else
                    db.AddParameter("@CustName", System.Data.SqlDbType.VarChar, CustName);
                db.CommandType = System.Data.CommandType.Text;

                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<CashierH>();

                        while (dr.Read())
                        {
                            CashierH sourceCode = new CashierH();
                            sourceCode.TransCode = IDS.Tool.GeneralHelper.NullToString(dr["TransCode"]);
                            sourceCode.TransDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["TransDate"],DateTime.MinValue);
                            sourceCode.CustID = IDS.Tool.GeneralHelper.NullToString(dr["CustID"]);
                            sourceCode.TransTotalAmt = IDS.Tool.GeneralHelper.NullToDecimal(dr["TransTotalAmt"],0);
                            sourceCode.TransPayMtd = IDS.Tool.GeneralHelper.NullToString(dr["TransPayMtd"]);
                            sourceCode.TransPayType = IDS.Tool.GeneralHelper.NullToString(dr["TransPayType"]);
                            sourceCode.TransRemark = IDS.Tool.GeneralHelper.NullToString(dr["TransRemark"]);
                            sourceCode.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["CreateBy"]);
                            sourceCode.LastUpdate = IDS.Tool.GeneralHelper.NullToDateTime(dr["CreateDate"], DateTime.MinValue);

                            list.Add(sourceCode);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }

        public static string GetNewTransCode(DateTime TransDate, string BranchCode)
        {
           string Res="";

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "GetNewTransCode";
                db.AddParameter("@TransDate", System.Data.SqlDbType.DateTime, TransDate);
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);

                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        Res = IDS.Tool.GeneralHelper.NullToString(dr["Result"]);
                       
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return Res;
        }
        
        public static string GetBranchName(string BranchCode)
        {
            string Res = "";

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select branchname+' - '+BranchCode as BranchName from tblbranch where branchCode=@BranchCode";
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);

                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        Res = IDS.Tool.GeneralHelper.NullToString(dr["BranchName"]);

                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return Res;
        }

        
        public static string GetNewTransNo()
        {
            string Res = "";

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select max(right(transcode,4)) as Hasil from TransInvH  WHERE CAST(TransDate AS DATE) = CAST(GETDATE() AS DATE)";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        Res = IDS.Tool.GeneralHelper.NullToString(dr["Hasil"],"0");
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            string tanggal = DateTime.Now.ToString("yyMMdd");
            int hasil = Convert.ToInt32(Res);
            string running = (hasil + 1).ToString("0000");
            string hasilAkhir = tanggal+running;

            return hasilAkhir;
        }

        public int InsUpDel(int Type)
        {
            int result = 0;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.Open();
                    cmd.BeginTransaction();
                    if (Type == 2)
                    {
                        cmd.CommandText = "InsTransaction";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@TransCode", System.Data.SqlDbType.VarChar, TransCode);
                        cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                        cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 3);
                        result = cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = "InsTransaction";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@TransCode", System.Data.SqlDbType.VarChar, TransCode);
                    cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                    cmd.AddParameter("@TransDate", System.Data.SqlDbType.DateTime, DateTime.Now);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.AddParameter("@CustID", System.Data.SqlDbType.VarChar, CustID);
                    cmd.AddParameter("@Status", System.Data.SqlDbType.VarChar, Status);
                    if (Status == 1)
                    {
                        cmd.AddParameter("@TransPaid", System.Data.SqlDbType.Money, TransTotalPaid);
                        cmd.AddParameter("@TransPayMtd", System.Data.SqlDbType.VarChar, TransPayMtd);
                        if (TransPayMtd == "CASH")
                        {
                            cmd.AddParameter("@TransPayType", System.Data.SqlDbType.VarChar, "CASH");
                        }
                        else
                        {
                            cmd.AddParameter("@TransPayType", System.Data.SqlDbType.VarChar, TransPayType);
                        }
                    }
                    cmd.AddParameter("@TransTotalAmt", System.Data.SqlDbType.Money, TransTotalAmt);
                    cmd.AddParameter("@TransSubTotalAmt", System.Data.SqlDbType.Money, @TransSubTotalAmt);
                    cmd.AddParameter("@TransTax", System.Data.SqlDbType.Money, TransTax);
                    cmd.AddParameter("@TransTaxPercentage", System.Data.SqlDbType.Money, TransTaxPercentage);
                    cmd.AddParameter("@TransRemark", System.Data.SqlDbType.VarChar, "");
                    cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 1);
                    result = cmd.ExecuteNonQuery();

                    if (Details != null && Details.Count > 0)
                    {
                        cmd.CommandText = "InsTransactionDetail";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@TransCode", System.Data.SqlDbType.VarChar, TransCode);
                        cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                        cmd.AddParameter("@SeqNo", System.Data.SqlDbType.VarChar, 0);
                        cmd.AddParameter("@VariantValue", System.Data.SqlDbType.VarChar, "");
                        cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 2);
                        result = cmd.ExecuteNonQuery();
                        foreach (var data in Details)
                        {
                            cmd.CommandText = "InsTransactionDetail";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.AddParameter("@TransCode", System.Data.SqlDbType.VarChar, TransCode);
                            cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                            cmd.AddParameter("@SeqNo", System.Data.SqlDbType.Int, data.SeqNo);
                            cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, data.ProductCode);
                            cmd.AddParameter("@ProdName", System.Data.SqlDbType.VarChar, data.ProductName.Split("-")[0].TrimEnd());
                            cmd.AddParameter("@Price", System.Data.SqlDbType.Money, data.Price);
                            cmd.AddParameter("@Qty", System.Data.SqlDbType.Int, data.Qty);
                            cmd.AddParameter("@VariantCode", System.Data.SqlDbType.VarChar, data.VariantCode);
                            cmd.AddParameter("@VariantValue", System.Data.SqlDbType.VarChar, data.VariantValue);
                            cmd.AddParameter("@VariantHarga", System.Data.SqlDbType.VarChar, data.VariantHarga);
                            cmd.AddParameter("@Remarks", System.Data.SqlDbType.VarChar, data.Remark);
                            cmd.AddParameter("@OrderType", System.Data.SqlDbType.VarChar, data.OrderType);
                            cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 1);
                            result = cmd.ExecuteNonQuery();
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
                            throw new Exception("Transaction ID is already exists. Please choose other Transaction.");
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

        public static List<CashierH> GetBillList(string BranchCode)
        {
            List<CashierH> Res = new List<CashierH>();

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "SELECT TransCode, TransTotalAmt " +
                                "FROM TransInvH " +
                                "WHERE BranchCode = @BranchCode " +
                                "AND CreateDate >= CAST(GETDATE() AS DATE) " +
                                "AND CreateDate < DATEADD(DAY, 1, CAST(GETDATE() AS DATE)) and status=0";
                db.AddParameter("@BranchCode", SqlDbType.VarChar, BranchCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            CashierH Data = new CashierH();
                            Data.TransCode = IDS.Tool.GeneralHelper.NullToString(dr["TransCode"], "0");
                            Data.TransTotalAmt = IDS.Tool.GeneralHelper.NullToDecimal(dr["TransTotalAmt"], 0);
                            Res.Add(Data);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return Res;
        }

        public static CashierH GetDataBill(string TransCode)
        {
            CashierH Res = new CashierH();

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select H.TransCode,H.BranchCode,H.TransTotalAmt,D.SeqNo,D.ProdCode,D.ProdName,D.ProdAmt,D.Qty,D.OrderType,D.VariantCode,D.VariantValue,D.Remarks from TransInvH H inner join TransInvD D on D.TransCode=H.TransCode and H.BranchCode=D.BranchCode and H.TransCode=@TransCode";
                db.AddParameter("@TransCode", SqlDbType.VarChar, TransCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        Res.Details = new List<CashierD>();
                        while (dr.Read())
                        {
                            Res.TransCode = IDS.Tool.GeneralHelper.NullToString(dr["TransCode"], "0"); 
                            Res.TransTotalAmt= IDS.Tool.GeneralHelper.NullToDecimal(dr["TransTotalAmt"], 0);
                            Res.TransSubTotalAmt = IDS.Tool.GeneralHelper.NullToDecimal(dr["TransTotalAmt"], 0);
                            CashierD DataDetail = new CashierD();
                            DataDetail.SeqNo= IDS.Tool.GeneralHelper.NullToInt(dr["SeqNo"], 0);
                            DataDetail.ProductCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            DataDetail.ProductName = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]);
                            DataDetail.Price = IDS.Tool.GeneralHelper.NullToDecimal(dr["ProdAmt"],0);
                            DataDetail.Qty = IDS.Tool.GeneralHelper.NullToInt(dr["Qty"], 0);
                            DataDetail.VariantCode = IDS.Tool.GeneralHelper.NullToString(dr["VariantCode"]);
                            DataDetail.VariantValue = IDS.Tool.GeneralHelper.NullToString(dr["VariantValue"]);
                            DataDetail.Remark = IDS.Tool.GeneralHelper.NullToString(dr["Remarks"]);
                            DataDetail.OrderType = IDS.Tool.GeneralHelper.NullToString(dr["OrderType"]);
                            Res.Details.Add(DataDetail);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return Res;
        }

        public static List<CashierH> GetDataTrans(string TransCode)
        {
            List<CashierH> data = new List<CashierH>();

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select H.TransCode,H.BranchCode,H.TransTotalAmt,D.SeqNo,D.ProdCode,D.ProdName,D.ProdAmt,D.Qty,D.OrderType,D.VariantCode,D.VariantValue,D.Remarks from TransInvH H inner join TransInvD D on D.TransCode=H.TransCode and H.BranchCode=D.BranchCode and H.TransCode=@TransCode";
                db.AddParameter("@TransCode", SqlDbType.VarChar, TransCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        CashierH Res = new CashierH();
                        Res.Details = new List<CashierD>();
                        while (dr.Read())
                        {
                            Res.TransCode = IDS.Tool.GeneralHelper.NullToString(dr["TransCode"], "0");
                            Res.TransTotalAmt = IDS.Tool.GeneralHelper.NullToDecimal(dr["TransTotalAmt"], 0);
                            Res.TransSubTotalAmt = IDS.Tool.GeneralHelper.NullToDecimal(dr["TransTotalAmt"], 0);
                            CashierD DataDetail = new CashierD();
                            DataDetail.SeqNo = IDS.Tool.GeneralHelper.NullToInt(dr["SeqNo"], 0);
                            DataDetail.ProductCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            DataDetail.ProductName = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]);
                            DataDetail.Price = IDS.Tool.GeneralHelper.NullToDecimal(dr["ProdAmt"], 0);
                            DataDetail.Qty = IDS.Tool.GeneralHelper.NullToInt(dr["Qty"], 0);
                            DataDetail.VariantCode = IDS.Tool.GeneralHelper.NullToString(dr["VariantCode"]);
                            DataDetail.VariantValue = IDS.Tool.GeneralHelper.NullToString(dr["VariantValue"]);
                            DataDetail.Remark = IDS.Tool.GeneralHelper.NullToString(dr["Remarks"]);
                            DataDetail.OrderType = IDS.Tool.GeneralHelper.NullToString(dr["OrderType"]);
                            Res.Details.Add(DataDetail);
                            data.Add(Res);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return data;
        }

        public static int CekExistTransCode(string TransCode)
        {
            string existTransCode = "";
            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select transCode from TransInvH where TransCode=@TransCode";
                db.AddParameter("@TransCode", SqlDbType.VarChar, TransCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        existTransCode = IDS.Tool.GeneralHelper.NullToString(dr["transCode"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            if (existTransCode != "")
            {
                return 2;//exists
            }
            else
            {
                return 1;//not existst jadi create
            }
        }

        
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetPayType(string Paymentmethod)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> paytype = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "select * from paymenttype where paymentmethodCode=@PaymentMethod";
                db.AddParameter("PaymentMethod", SqlDbType.VarChar, Paymentmethod);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem a = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();

                        while (dr.Read())
                        {
                            SelectListItem dataPayType = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            dataPayType.Text = IDS.Tool.GeneralHelper.NullToString(dr["PaymentTypeCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["PaymentTypeName"]);
                            dataPayType.Value = IDS.Tool.GeneralHelper.NullToString(dr["PaymentTypeCode"]);
                            paytype.Add(dataPayType);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            paytype = paytype.OrderBy(x => x.Text).ToList();

            return paytype;
        }

        
       
    }
}
