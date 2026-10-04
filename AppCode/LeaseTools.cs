//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using IDS.Tool;
//using System.Security.Policy;
//using System.Diagnostics.Contracts;
//using Microsoft.Data.SqlClient;

//namespace AppCode
//{
//    public class LeaseTools
//    {
//        private static LeaseTools INSTANCE;

//        public static LeaseTools GetINSTANCE()
//        {
//            if (INSTANCE == null)
//            {
//                INSTANCE = new LeaseTools();
//            }
//            return INSTANCE;
//        }
//        public void CreateGLTfTransNew(
//        string strTransNo,
//        string strBranchCode,
//        string strSCode,
//        long NoProcess,
//        DateTime Period,
//        string strVoucher,
//        ref int message,
//        int xPeriod,
//        string Tkey
//    )
//        {
//            string VchNo, SCode, JCode, vLeNo, sAccNo;
//            string sAccCashBasis;
//            string sCcy, sTCurrCode;
//            Byte X, I, bCounter;
//            long Done;
//            int DbCr, ErrCode, GLSeqNo = 0;
//            double Portion = 0;
//            Byte nPeriod;
//            double TotDb, TotCr, vAmt, cExchRateJournal;
//            double cExchRateVoucher, cAmount, cExchRate;
//            DateTime CSLTransDate;
//            string sDesc;
//            DateTime StopAccrueDate;
//            int Action = 0;
//            string sBaseCcy;
//            DataTable dtJTD;
//            DataTable dtGLTrans;
//            DataTable dtLease;

//            //Variable For GLTrans
//            string GL_SCode, GL_Voucher, GL_Account, GL_Ccy;
//            string GL_Dept = "", GL_Description, GL_LeaseNo, GL_DocNo;
//            byte GL_Counter;
//            DateTime GL_EntryDate, GL_OriTransDate, GL_TransDate;
//            bool GL_Reverse, GL_TransferToGL;
//            double GL_Amount;
//            int GL_ErrCode;
//            string GLBranchCode, GLCashBasis;

//            string ContractNo = "";
//            string CustNo = "";
//            string CustBranch = "";
//            int GrpNo = 0;
//            int GroupType = 0;

//            sBaseCcy = IDS.GeneralTable.Syspar.GetInstance().BaseCCy;

//            // Get Contract No 
//            ContractNo = GetFieldValue(
//                 "EntityNo",
//                 "LeTrans",
//                 "TransNo='" + strTransNo + "'"
//                ).ToString();

//            // Get Customer No
//            CustNo = GetFieldValue(
//                 "LesseeNo",
//                 "Lease",
//                 "LeaseNo='" + ContractNo + "'"
//                ).ToString();

//            // Get Customer Branch
//            CustBranch = GetFieldValue(
//                 "LesseeBranch",
//                 "Lease",
//                 "LeaseNo='" + ContractNo + "'"
//                ).ToString();

//            // Get Customer Group No
//            GrpNo = Convert.ToInt32(GetFieldValue(
//                 "isnull(GrpNo,0)",
//                 "Lessee",
//                 "LesseeNo='" + CustNo + "' And BranchCode='" + CustBranch + "'"
//                ));

//            // Get Customer Group Type
//            if (GrpNo == 0)
//            {
//                GroupType = 0;
//            }
//            else
//            {
//                GroupType = IDS.Tool.GeneralHelper.NullToInt(GetFieldValue(
//                     "isnull(Category,0)",
//                     "tblGroup",
//                     "GrpNo=" + GrpNo + ""
//                    ),0);

//                if (GroupType == 2)
//                {
//                    GroupType = 0;
//                }
//            }

//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                SqlTransaction trans = null;
//                trans = conn.BeginTransaction();
//                clsModule mdl = new clsModule();
//                SqlCommand cmd = new SqlCommand();
//                cmd.Connection = conn;
//                cmd.Transaction = trans;
//                cmd.CommandType = CommandType.StoredProcedure;
//                try
//                {
//                    string strSQL;
//                    strSQL = "Le_SPpruCreateGLTfTransNewPrs '" + Period + "','" + strVoucher + "','" + strTransNo + "'";
//                    SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
//                    da.SelectCommand.Transaction = trans;
//                    DataTable dtTrans = new DataTable();
//                    da.Fill(dtTrans);
//                    NoProcess = NoProcess + dtTrans.Rows.Count;
//                    if (dtTrans.Rows.Count == 0)
//                    {
//                        // WebMsgBox.Show("No Transaction To Be Created!");
//                        return;
//                    }

//                    ErrCode = 0;
//                    Done = 0;
//                    for (int i = 0; i < dtTrans.Rows.Count; i++)
//                    {
//                        DataRow rowTrans = dtTrans.Rows[i];
//                        Done = Done + 1;
//                        if (mdl.Left(rowTrans["JCode"].ToString(), 2) != "LT")
//                            JCode = rowTrans["JCode"].ToString(); //mdl.Left(rowTrans["JCode"].ToString(), 1) + "1" + mdl.Right(rowTrans["JCode"].ToString(), 2);
//                        else
//                            JCode = rowTrans["JCode"].ToString();

//                        sTCurrCode = rowTrans["CurrCode"].ToString();

//                        string Vch;
//                        string strKondisi;

//                        strKondisi = "SCode='" + JCode + "' AND YEAR(EntryDate)='" + DateTime.Today.ToString("yyyy") + "' AND MONTH(EntryDate)='" + DateTime.Today.ToString("MM") + "' AND BranchCode='" + strBranchCode + "'";
//                        Vch = GetMaxFieldValue("right(Voucher,3)", "LeGLTrans", strKondisi).ToString();
//                        if (!string.IsNullOrEmpty(Vch) && Vch != "0")
//                        {
//                            GLSeqNo = Convert.ToInt16(Vch.Substring(Vch.Length - 3));
//                        }
//                        else
//                        {
//                            GLSeqNo = 0;
//                        }

//                        switch (JCode)
//                        {
//                            // New Contract 
//                            // Leasing Normal

//                            // Alter by Anthony (Disburse) - Perubahan kode disesuaikan dengan kontrak OJK
//                            //case "1L1":
//                            //case "1L2":
//                            //case "1L3":
//                            //case "1L4":
//                            //case "1L5":
//                            //case "1L6":

//                            case "1LIF1": // Investasi - Finance Lease - Normal
//                            case "1LPF1": // Proyek - Finance Lease - Normal
//                            case "1LGF1": // Multiguna - Finance Lease - Normal
//                            case "1LIS1": // Investasi - Sale & Leaseback - Normal
//                            case "1LPS1": // Proyek - Sale & Leaseback - Normal
//                            case "1LMS1": // Modal Kerja - Sale & Leaseback - Normal

//                            // Leasing Receivable Assignment
//                            case "1LIF4": // Investasi - Finance Lease - Receivable Assignment
//                            case "1LPF4": // Proyek - Finance Lease - Receivable Assignment
//                            case "1LGF4": // Multiguna - Finance Lease - Receivable Assignment
//                            case "1LIS4": // Investasi - Sale & Leaseback - Receivable Assignment
//                            case "1LPS4": // Proyek - Sale & Leaseback - Receivable Assignment
//                            case "1LMS4": // Modal Kerja - Sale & Leaseback - Receivable Assignment

//                            //case "1C1":
//                            //case "1C2":
//                            //case "1C3":
//                            //case "1C4":
//                            //case "1C5":
//                            //case "1C6":

//                            // CF Normal
//                            case "1CIB1": // Investasi - Installment Barang - Normal
//                            case "1CPB1": // Proyek - Installment Barang - Normal
//                            case "1CGB1": // Multiguna - Installment Barang - Normal
//                            case "1CPJ1": // Proyek - Installment Jasa - Normal
//                            case "1CGJ1": // Multiguna - Installment Jasa - Normal
//                            case "1CMU1": // Modal Kerja - Modal Usaha - Normal
//                            case "1CGG1": // Multiguna - Fasilitas Dana - Barang - Normal
//                            case "1CGS1": // Multiguna - Fasilitas Dana - Jasa - Normal

//                            // CF Receivable Assigment
//                            case "1CIB4": // Investasi - Installment Barang - Receivable Assigment
//                            case "1CPB4": // Proyek - Installment Barang - Receivable Assigment
//                            case "1CGB4": // Multiguna - Installment Barang - Receivable Assigment
//                            case "1CPJ4": // Proyek - Installment Jasa - Receivable Assigment
//                            case "1CGJ4": // Multiguna - Installment Jasa - Receivable Assigment
//                            case "1CMU4": // Modal Kerja - Modal Usaha - Receivable Assigment
//                            case "1CGG4": // Multiguna - Fasilitas Dana - Barang - Receivable Assignment
//                            case "1CGS4": // Multiguna - Fasilitas Dana - Jasa - Receivable Assignment
//                                          // End of alter by Anthony (Disburse)




//                            // SLB
//                            //case "1S1":
//                            //case "1S2":
//                            //case "1S3":
//                            //case "1S4":
//                            //case "1S5":

//                            // Alter by Anthony - 20151229 - OJK
//                            // Partial Terminate
//                            //// L
//                            //case "2L1":
//                            //case "2L2":
//                            //case "2L3":
//                            //case "2L4":
//                            //case "2L5":
//                            //case "2L6":

//                            //// CF
//                            //case "2C1":
//                            //case "2C2":
//                            //case "2C3":
//                            //case "2C4":
//                            //case "2C5":
//                            //case "2C6":
//                            // L
//                            case "2LIF1":
//                            case "2LPF1":
//                            case "2LGF1":
//                            case "2LIS1":
//                            case "2LPS1":
//                            case "2LMS1":

//                            // CF
//                            case "2CIB1":
//                            case "2CPB1":
//                            case "2CGB1":
//                            case "2CPJ1":
//                            case "2CMJ1":
//                            case "2CMU1":
//                            case "2CGG1":
//                            case "2CGS1":

//                            //Receivable Assignment
//                            // L
//                            case "2LIF4":
//                            case "2LPF4":
//                            case "2LGF4":
//                            case "2LIS4":
//                            case "2LPS4":
//                            case "2LMS4":

//                            // CF
//                            case "2CIB4":
//                            case "2CPB4":
//                            case "2CGB4":
//                            case "2CPJ4":
//                            case "2CMJ4":
//                            case "2CMU4":
//                            case "2CGG4":
//                            case "2CGS4":
//                            // End of alter by Anthony - OJK

//                            // SLB
//                            //case "2S1":
//                            //case "2S2":
//                            //case "2S3":
//                            //case "2S4":
//                            //case "2S5":

//                            // Alter by Anthony (Full Terminate) - OJK
//                            //// Full Terminate
//                            //// L
//                            //case "3L1":
//                            //case "3L2":
//                            //case "3L3":
//                            //case "3L4":
//                            //case "3L5":
//                            //case "3L6":

//                            case "3LIF1":
//                            case "3LPF1":
//                            case "3LGF1":
//                            case "3LIS1":
//                            case "3LPS1":
//                            case "3LMS1":

//                            //// CF
//                            //case "3C1":
//                            //case "3C2":
//                            //case "3C3":
//                            //case "3C4":
//                            //case "3C5":
//                            //case "3C6":

//                            case "3CIB1":
//                            case "3CPB1":
//                            case "3CGB1":
//                            case "3CPJ1":
//                            case "3CGJ1":
//                            case "3CMU1":
//                            case "3CGG1":
//                            case "3CGS1":
//                            // End of alter by Anthony (Full Terminate)

//                            // SLB
//                            //case "3S1":
//                            //case "3S2":
//                            //case "3S3":
//                            //case "3S4":
//                            //case "3S5":


//                            // Alter by Anthony (Floating Interest) - OJK
//                            // Floating Interest
//                            // L
//                            //case "4L1":
//                            //case "4L2":
//                            //case "4L3":
//                            //case "4L4":
//                            //case "4L5":
//                            //case "4L6":

//                            case "4LIF1":
//                            case "4LPF1":
//                            case "4LGF1":
//                            case "4LIS1":
//                            case "4LPS1":
//                            case "4LMS1":

//                            // CF
//                            //case "4C1":
//                            //case "4C2":
//                            //case "4C3":
//                            //case "4C4":
//                            //case "4C5":
//                            //case "4C6":

//                            case "4CIB1":
//                            case "4CPB1":
//                            case "4CGB1":
//                            case "4CPJ1":
//                            case "4CGJ1":
//                            case "4CMU1":
//                            case "4CGG1":
//                            case "4CGS1":
//                            // End of alter by Anthony (Floating)

//                            // SLB
//                            //case "4S1":
//                            //case "4S2":
//                            //case "4S3":
//                            //case "4S4":
//                            //case "4S5":


//                            // Alter by Anthony (Rescheduling) - Perubahan kode disesuaikan dengan kontrak OJK
//                            // Rescheduling
//                            // L
//                            //case "21L1":
//                            //case "21L2":
//                            //case "21L3":
//                            //case "21L4":
//                            //case "21L5":
//                            //case "21L6":

//                            case "21LIF1":
//                            case "21LPF1":
//                            case "21LGF1":
//                            case "21LIS1":
//                            case "21LPS1":
//                            case "21LMS1":

//                            // CF
//                            //case "21C1":
//                            //case "21C2":
//                            //case "21C3":
//                            //case "21C4":
//                            //case "21C5":
//                            //case "21C6":

//                            case "21CIB1":
//                            case "21CPB1":
//                            case "21CGB1":
//                            case "21CPJ1":
//                            case "21CGJ1":
//                            case "21CMU1":
//                            case "21CGG1":
//                            case "21CGS1":
//                            // End of alter by Anthony (rescheduling)


//                            // Contract Interest Accrued
//                            // L
//                            case "24L4":

//                            // CF
//                            case "24C4":

//                                // SLB
//                                //case "21S1":
//                                //case "21S2":
//                                //case "21S3":
//                                //case "21S4":
//                                //case "21S5":

//                                X = 1;
//                                bCounter = 1;
//                                vLeNo = rowTrans["EntityNo"].ToString();
//                                strSQL = "SELECT * FROM LeJournalTD where JCode='" + JCode + "' and TCurrCode='" + sTCurrCode + "' and GroupType=" + GroupType + " ORDER BY Counter ";

//                                //strSQL += "and Counter=" + X;
//                                da = new SqlDataAdapter(strSQL, conn);
//                                da.SelectCommand.Transaction = trans;
//                                dtJTD = new DataTable();
//                                da.Fill(dtJTD);

//                                if (dtJTD.Rows.Count == 0)
//                                {
//                                    //WebMsgBox.Show("Seek Failure in Journal for " + JCode);
//                                }
//                                else
//                                {
//                                    TotDb = 0;
//                                    TotCr = 0;

//                                    SCode = rowTrans["JCode"].ToString();
//                                    GLSeqNo = GLSeqNo + 1;
//                                    VchNo = DateTime.Today.ToString("yyMM") + GLSeqNo.ToString("000");

//                                    for (int c = 0; c < dtJTD.Rows.Count; c++)
//                                    {
//                                        DataRow rowJTD = dtJTD.Rows[c];
//                                        ErrCode = 0;
//                                        DbCr = 1;
//                                        vAmt = 0;

//                                        if (Convert.ToInt16(rowJTD["RetrieveType"]) == 1)
//                                        {
//                                            if (!string.IsNullOrEmpty(rowTrans["OuterCode"].ToString()))
//                                            {
//                                                sAccNo = GetFieldValue("GLNo", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                                sCcy = rowJTD["CurrCode"].ToString();
//                                                sDesc = GetFieldValue("BankName", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                                sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            }
//                                            else
//                                            {
//                                                sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                                sAccNo = rowJTD["Acc"].ToString();
//                                                sCcy = rowJTD["CurrCode"].ToString();
//                                                sDesc = rowJTD["Description"].ToString();
//                                            }

//                                        }
//                                        else
//                                        {
//                                            sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            sAccNo = rowJTD["Acc"].ToString();
//                                            sCcy = rowJTD["CurrCode"].ToString();
//                                            sDesc = string.IsNullOrEmpty(rowJTD["Description"].ToString()) ? "0" : rowJTD["Description"].ToString();
//                                        }

//                                        switch (strSCode)
//                                        {
//                                            //Reverse Disburst
//                                            case "L006":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            //Disburst
//                                            case "L001":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Reverse Partial 
//                                            case "L007":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            //Partial
//                                            case "L002":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Reverse Full
//                                            case "L008":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            case "L003":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Add By: Tomy
//                                            //Floating Interest
//                                            case "L004":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Rescheduling
//                                            case "L021":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Contract Interest Accrued
//                                            case "L024":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;
//                                        }

//                                        GLCashBasis = sAccCashBasis;
//                                        GLBranchCode = strBranchCode;

//                                        GL_SCode = SCode;
//                                        GL_Voucher = VchNo;
//                                        GL_Counter = bCounter;
//                                        GL_EntryDate = DateTime.Today;
//                                        GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                        GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                        GL_Reverse = false;
//                                        GL_Account = sAccNo;
//                                        GL_Ccy = sCcy;
//                                        // Alter - Anthony - 20200709
//                                        //GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString();

//                                        using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
//                                        {
//                                            db.CommandText = "SELECT BranchCode, RepresentativeOffice FROM Lease WHERE LeaseNo = @leaseno";
//                                            db.CommandType = System.Data.CommandType.Text;
//                                            db.AddParameter("@leaseno", SqlDbType.VarChar, vLeNo);
//                                            db.Open();

//                                            db.ExecuteReader();

//                                            using (SqlDataReader rd = db.DbDataReader as SqlDataReader)
//                                            {
//                                                if (rd.HasRows)
//                                                {
//                                                    while (rd.Read())
//                                                    {
//                                                        if (rd["RepresentativeOffice"] == DBNull.Value || string.IsNullOrEmpty(rd["RepresentativeOffice"] as string))
//                                                            GL_Dept = rd["BranchCode"] as string;
//                                                        else
//                                                            GL_Dept = rd["RepresentativeOffice"] as string;
//                                                    }
//                                                }
//                                            }

//                                            db.Close();
//                                        }


//                                        // End alter - Anthony - 20200709

//                                        if (strSCode == "L021" || strSCode == "L024")
//                                        {
//                                            //GetvAmtErrCode For Approve (status=0)
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 2)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType, Tkey);
//                                        }
//                                        else
//                                        {
//                                            //GetvAmtErrCode For Approve (status=0)
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType, Tkey);
//                                        }

//                                        cExchRateJournal = sCcy != sBaseCcy ? GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;
//                                        cExchRateVoucher = sTCurrCode != sBaseCcy ? GetExchangeRate(sTCurrCode, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;

//                                        if (sCcy != sTCurrCode)
//                                            vAmt = (vAmt * cExchRateVoucher / cExchRateJournal);
//                                        vAmt = vAmt * DbCr;
//                                        GL_Amount = vAmt;
//                                        GL_Description = sDesc + " " + vLeNo;
//                                        GL_LeaseNo = vLeNo;
//                                        //GL_DocNo = rowTrans["TransCode"].ToString();
//                                        GL_DocNo = strSCode;
//                                        GL_TransferToGL = false;
//                                        GL_ErrCode = ErrCode;

//                                        // Alter - Anthony - 20220521
//                                        //if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
//                                        switch (JCode)
//                                        {
//                                            case "1LIF4":
//                                            case "1LPF4":
//                                            case "1LGF4":
//                                            case "1LIS4":
//                                            case "1LPS4":
//                                            case "1LMS4":
//                                            case "1CIB4":
//                                            case "1CPB4":
//                                            case "1CGB4":
//                                            case "1CPJ4":
//                                            case "1CGJ4":
//                                            case "1CMU4":
//                                            case "1CGG4":
//                                            case "1CGS4":

//                                            case "1LIF6":
//                                            case "1LPF6":
//                                            case "1LGF6":
//                                            case "1LIS6":
//                                            case "1LPS6":
//                                            case "1LMS6":
//                                            case "1CIB6":
//                                            case "1CPB6":
//                                            case "1CGB6":
//                                            case "1CPJ6":
//                                            case "1CGJ6":
//                                            case "1CMU6":
//                                            case "1CGG6":
//                                            case "1CGS6":
//                                                if (rowJTD["RetrieveAmt"].ToString() == "HOP")
//                                                {
//                                                    // Get HOPNO
//                                                    string HOPNO, HOPBranchName;

//                                                    HOPNO = "";
//                                                    HOPBranchName = "";

//                                                    // Get HOPNO
//                                                    HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "HopNo",
//                                                        "tblBranch",
//                                                        "BranchCode='" + GLBranchCode + "'").ToString();

//                                                    // Get HOPBranchName
//                                                    HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "Name",
//                                                        "ACFGLMH",
//                                                        "ACC='" + HOPNO + "'").ToString();

//                                                    GL_Account = HOPNO;
//                                                    GL_Description = HOPBranchName;
//                                                }
//                                                break;
//                                        }


//                                        int msg1 = 1;
//                                        Le_SPUpdateGLTrans(
//                                            strTransNo,
//                                            GL_SCode,
//                                            GL_Voucher,
//                                            GL_Counter,
//                                            GL_EntryDate,
//                                            GL_TransDate,
//                                            GL_Reverse,
//                                            GL_Dept,
//                                            GL_Account,
//                                            GL_Ccy,
//                                            GL_Amount,
//                                            GL_Description,
//                                            GL_LeaseNo,
//                                            GL_DocNo,
//                                            GL_TransferToGL,
//                                            GL_ErrCode,
//                                            GL_OriTransDate,
//                                            GLBranchCode,
//                                            GLCashBasis,
//                                            ref msg1);

//                                        if (msg1 == 0)
//                                        {
//                                            //WebMsgBox.Show("Update LeGLTrans Failed!");
//                                        }

//                                        if (sCcy != sBaseCcy)
//                                        {
//                                            //CreateEqvJournal
//                                            cAmount = vAmt;
//                                            cExchRate = GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"]));
//                                            bCounter++;

//                                            GLCashBasis = sAccCashBasis;
//                                            GLBranchCode = strBranchCode;

//                                            GL_SCode = SCode;
//                                            GL_Voucher = VchNo;
//                                            GL_Counter = bCounter;
//                                            GL_EntryDate = DateTime.Today;
//                                            GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                            GL_Reverse = false;
//                                            GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString();
//                                            GL_Account = sAccNo;
//                                            GL_Ccy = sBaseCcy;
//                                            GL_Amount = cAmount * cExchRate;
//                                            GL_Description = rowJTD["Description"].ToString();
//                                            GL_LeaseNo = vLeNo;
//                                            //GL_DocNo = rowTrans["TransCode"].ToString();
//                                            GL_DocNo = strSCode;
//                                            GL_TransferToGL = false;
//                                            GL_ErrCode = ErrCode;
//                                            GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);

//                                            if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
//                                            {
//                                                // Get HOPNO
//                                                string HOPNO, HOPBranchName;

//                                                HOPNO = "";
//                                                HOPBranchName = "";

//                                                // Get HOPNO
//                                                HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                    "HopNo",
//                                                    "tblBranch",
//                                                    "BranchCode='" + GLBranchCode + "'").ToString();

//                                                // Get HOPBranchName
//                                                HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                    "Name",
//                                                    "ACFGLMH",
//                                                    "ACC='" + HOPNO + "'").ToString();

//                                                GL_Account = HOPNO;
//                                                GL_Description = HOPBranchName;
//                                            }
//                                            int msg2 = 1;
//                                            Le_SPUpdateGLTrans(
//                                                strTransNo,
//                                                GL_SCode,
//                                                GL_Voucher,
//                                                GL_Counter,
//                                                GL_EntryDate,
//                                                GL_TransDate,
//                                                GL_Reverse,
//                                                GL_Dept,
//                                                GL_Account,
//                                                GL_Ccy,
//                                                GL_Amount,
//                                                GL_Description,
//                                                GL_LeaseNo,
//                                                GL_DocNo,
//                                                GL_TransferToGL,
//                                                GL_ErrCode,
//                                                GL_OriTransDate,
//                                                GLBranchCode,
//                                                GLCashBasis,
//                                                ref msg2);
//                                            if (msg2 == 0)
//                                            {
//                                                //WebMsgBox.Show("Update LeGLTrans Failed!");
//                                            }
//                                        }

//                                        if (vAmt < 0)
//                                            TotCr = TotCr + vAmt;
//                                        else
//                                            TotDb = TotDb + vAmt;

//                                        bCounter++;
//                                        X++;
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                    trans.Commit();
//                }
//                catch (Exception ex)
//                {
//                    message = 0;
//                    trans.Rollback();
//                    //WebMsgBox.Show(ex.Message);
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }

//        public void DisbursementContarct(string LeaseNo, int status)
//        {
//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                SqlTransaction trans = null;
//                try
//                {
//                    conn.Open();
//                    SqlCommand cmd = new SqlCommand();
//                    cmd.Connection = conn;
//                    cmd.CommandType = CommandType.Text;

//                    string query = "exec ";
//                    query += "Le_SPUPDATELeStatus ";
//                    query += "@LeaseNo, @Status ";

//                    cmd.CommandText = query;
//                    cmd.Parameters.Clear();
//                    cmd.Parameters.Add("@LeaseNo", SqlDbType.VarChar).Value = LeaseNo;
//                    cmd.Parameters.Add("@Status", SqlDbType.SmallInt).Value = status;

//                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
//                    cmd.Transaction = trans;
//                    cmd.ExecuteNonQuery();
//                    trans.Commit();
//                }
//                catch (Exception ex)
//                {
//                    if (trans != null)
//                        trans.Rollback();
                  
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }
//        //Add by Jeremi 14 Juni 2025
//        public void DisbursementContarctLoan(string LeaseNo, int status, string UserID)
//        {
//            //using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            //{
//            //    SqlTransaction trans = null;
//            //    try
//            //    {
//            //        conn.Open();

//            //        SqlCommand cmd = new SqlCommand();
//            //        cmd.Connection = conn;
//            //        cmd.CommandType = CommandType.Text;

//            //        string query = "exec ";
//            //        query += "Loan_SPUPDATELStatus ";
//            //        query += "@LeaseNo, @Status,@OperatorID ";

//            //        cmd.CommandText = query;
//            //        cmd.Parameters.Clear();
//            //        cmd.Parameters.Add("@LeaseNo", SqlDbType.VarChar).Value = LeaseNo;
//            //        cmd.Parameters.Add("@Status", SqlDbType.SmallInt).Value = status;
//            //        cmd.Parameters.Add("@OperatorID", SqlDbType.VarChar).Value = UserID;
//            //        trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
//            //        cmd.Transaction = trans;
//            //        int hasil=cmd.ExecuteNonQuery();
//            //        trans.Commit();
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        if (trans != null)
//            //            trans.Rollback();

//            //    }
//            //    finally
//            //    {
//            //        conn.Close();
//            //    }
//            //}

//            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
//            {
//                try
//                {
//                    cmd.CommandText = "Loan_SPUPDATELStatus";
//                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
//                    cmd.AddParameter("@LeaseNo", System.Data.SqlDbType.VarChar, LeaseNo);
//                    cmd.AddParameter("@Status", System.Data.SqlDbType.SmallInt, status);
//                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, UserID);

//                    cmd.Open();
//                    cmd.BeginTransaction();
//                    int hasil = 0;
//                    hasil = cmd.ExecuteNonQuery();
//                    cmd.CommitTransaction();
//                    cmd.Close();
//                }
//                catch (Exception ex)
//                {
//                    cmd.RollbackTransaction();
//                }
//            }
//        }

//        public void CreateGLTfTransNewLoan(
//        string strTransNo,
//        string strBranchCode,
//        string strSCode,
//        long NoProcess,
//        DateTime Period,
//        string strVoucher,
//        ref int message
//    )
//        {
//            CreateGLTfTransNewLoan(
//                strTransNo,
//                strBranchCode,
//                strSCode,
//                NoProcess,
//                Period,
//                strVoucher,
//                ref message,
//                0
//            );
//        }

//        public void CreateGLTfTransNewLoan(
//            string strTransNo,
//            string strBranchCode,
//            string strSCode,
//            long NoProcess,
//            DateTime Period,
//            string strVoucher,
//            ref int message,
//            int xPeriod
//        )
//        {
//            string VchNo, SCode, JCode, vLeNo, sAccNo;
//            string sAccCashBasis;
//            string sCcy, sTCurrCode;
//            Byte X, I, bCounter;
//            long Done;
//            int DbCr, ErrCode, GLSeqNo = 0;
//            double Portion = 0;
//            Byte nPeriod;
//            double TotDb, TotCr, vAmt, cExchRateJournal;
//            double cExchRateVoucher, cAmount, cExchRate;
//            DateTime CSLTransDate;
//            string sDesc;
//            DateTime StopAccrueDate;
//            int Action = 0;
//            string sBaseCcy;
//            DataTable dtJTD;
//            DataTable dtGLTrans;
//            DataTable dtLease;

//            //Variable For GLTrans
//            string GL_SCode, GL_Voucher, GL_Account, GL_Ccy;
//            string GL_Dept = "", GL_Description, GL_LeaseNo, GL_DocNo;
//            byte GL_Counter;
//            DateTime GL_EntryDate, GL_OriTransDate, GL_TransDate;
//            bool GL_Reverse, GL_TransferToGL;
//            double GL_Amount;
//            int GL_ErrCode;
//            string GLBranchCode, GLCashBasis;

//            string ContractNo = "";
//            string CustNo = "";
//            string CustBranch = "";
//            int GrpNo = 0;
//            int GroupType = 0;
//            string Tkey = "";

//            sBaseCcy = IDS.GeneralTable.Syspar.GetInstance().BaseCCy;

//            // Get Contract No 
//            ContractNo = GetFieldValue(
//                 "EntityNo",
//                 "LoanTrans",
//                 "TransNo='" + strTransNo + "'"
//                ).ToString();

//            // Get Subentity No 
//            Tkey = GetFieldValue(
//                 "SubEntityNo",
//                 "LoanTrans",
//                 "TransNo='" + strTransNo + "'"
//                ).ToString();

//            // Get Customer No
//            CustNo = GetFieldValue(
//                 "FunderNo",
//                 "Loan",
//                 "Lno='" + ContractNo + "'"
//                ).ToString();

//            //// Get Customer Branch
//            //CustBranch = GetFieldValue(
//            //     "LesseeBranch",
//            //     "Lease",
//            //     "LeaseNo='" + ContractNo + "'"
//            //    ).ToString();
//            CustBranch = "";

//            // Get Customer Group No
//            //GrpNo = Convert.ToInt32(GetFieldValue(
//            //     "isnull(GrpNo,0)",
//            //     "Lessee",
//            //     "LesseeNo='" + CustNo + "' And BranchCode='" + CustBranch + "'"
//            //    ));
//            GrpNo = 0;
//            // Get Customer Group Type
//            //if (GrpNo == 0)
//            //{
//            //    GroupType = 0;
//            //}
//            //else
//            //{
//            //    //GroupType = IDS.Tool.GeneralHelper.NullToInt((GetFieldValue(
//            //    //     "isnull(Category,0)",
//            //    //     "tblGroup",
//            //    //     "GrpNo=" + GrpNo + ""
//            //    //    )),0);

//            //    //if (GroupType == 2)
//            //    //{
//            //    //    GroupType = 0;
//            //    //}
//            //}
//            GroupType = 0;
//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                SqlTransaction trans = null;
//                trans = conn.BeginTransaction();
//                clsModule mdl = new clsModule();
//                SqlCommand cmd = new SqlCommand();
//                cmd.Connection = conn;
//                cmd.Transaction = trans;
//                cmd.CommandType = CommandType.StoredProcedure;
//                try
//                {
//                    string strSQL;
//                    string PeriodSQL = Period.ToString("yyyy-MM-dd HH:mm:ss");
//                    strSQL = "Le_SPpruCreateGLTfTransNewPrsLoan '" + PeriodSQL + "','" + strVoucher + "','" + strTransNo + "'";
//                    SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
//                    da.SelectCommand.Transaction = trans;
//                    DataTable dtTrans = new DataTable();
//                    da.Fill(dtTrans);
//                    NoProcess = NoProcess + dtTrans.Rows.Count;
//                    if (dtTrans.Rows.Count == 0)
//                    {
//                        message = 0;
//                        // WebMsgBox.Show("No Transaction To Be Created!");
//                        return;
//                    }

//                    ErrCode = 0;
//                    Done = 0;
//                    for (int i = 0; i < dtTrans.Rows.Count; i++)
//                    {
//                        DataRow rowTrans = dtTrans.Rows[i];
//                        Done = Done + 1;
//                        if (mdl.Left(rowTrans["JCode"].ToString(), 2) != "LT")
//                            JCode = rowTrans["JCode"].ToString(); //mdl.Left(rowTrans["JCode"].ToString(), 1) + "1" + mdl.Right(rowTrans["JCode"].ToString(), 2);
//                        else
//                            JCode = rowTrans["JCode"].ToString();

//                        sTCurrCode = rowTrans["CurrCode"].ToString();

//                        string Vch;
//                        string strKondisi;

//                        strKondisi = "SCode='" + JCode + "' AND YEAR(TransDate)='" + Period.ToString("yyyy") + "' AND BranchCode='" + strBranchCode + "'";
//                        // "' AND MONTH(EntryDate)='" + Period.ToString("MM") +
//                        Vch = GetMaxFieldValue("right(Voucher,3)", "LoanGlTrans", strKondisi).ToString();
//                        if (!string.IsNullOrEmpty(Vch) && Vch != "0")
//                        {
//                            GLSeqNo = Convert.ToInt16(Vch.Substring(Vch.Length - 3));
//                        }
//                        else
//                        {
//                            GLSeqNo = 0;
//                        }

//                        switch (JCode)
//                        {
//                            // New Contract 
//                            // Leasing Normal

//                            // Alter by Anthony (Disburse) - Perubahan kode disesuaikan dengan kontrak OJK
//                            //case "1L1":
//                            //case "1L2":
//                            //case "1L3":
//                            //case "1L4":
//                            //case "1L5":
//                            //case "1L6":

//                            case "1LIF1": // Investasi - Finance Lease - Normal
//                            case "1LPF1": // Proyek - Finance Lease - Normal
//                            case "1LGF1": // Multiguna - Finance Lease - Normal
//                            case "1LIS1": // Investasi - Sale & Leaseback - Normal
//                            case "1LPS1": // Proyek - Sale & Leaseback - Normal
//                            case "1LMS1": // Modal Kerja - Sale & Leaseback - Normal
//                            case "1SIF1":
//                            // Leasing Receivable Assignment
//                            case "1LIF4": // Investasi - Finance Lease - Receivable Assignment
//                            case "1LPF4": // Proyek - Finance Lease - Receivable Assignment
//                            case "1LGF4": // Multiguna - Finance Lease - Receivable Assignment
//                            case "1LIS4": // Investasi - Sale & Leaseback - Receivable Assignment
//                            case "1LPS4": // Proyek - Sale & Leaseback - Receivable Assignment
//                            case "1LMS4": // Modal Kerja - Sale & Leaseback - Receivable Assignment

//                            //case "1C1":
//                            //case "1C2":
//                            //case "1C3":
//                            //case "1C4":
//                            //case "1C5":
//                            //case "1C6":

//                            // CF Normal
//                            case "1CIB1": // Investasi - Installment Barang - Normal
//                            case "1CPB1": // Proyek - Installment Barang - Normal
//                            case "1CGB1": // Multiguna - Installment Barang - Normal
//                            case "1CPJ1": // Proyek - Installment Jasa - Normal
//                            case "1CGJ1": // Multiguna - Installment Jasa - Normal
//                            case "1CMU1": // Modal Kerja - Modal Usaha - Normal
//                            case "1CGG1": // Multiguna - Fasilitas Dana - Barang - Normal
//                            case "1CGS1": // Multiguna - Fasilitas Dana - Jasa - Normal

//                            // CF Receivable Assigment
//                            case "1CIB4": // Investasi - Installment Barang - Receivable Assigment
//                            case "1CPB4": // Proyek - Installment Barang - Receivable Assigment
//                            case "1CGB4": // Multiguna - Installment Barang - Receivable Assigment
//                            case "1CPJ4": // Proyek - Installment Jasa - Receivable Assigment
//                            case "1CGJ4": // Multiguna - Installment Jasa - Receivable Assigment
//                            case "1CMU4": // Modal Kerja - Modal Usaha - Receivable Assigment
//                            case "1CGG4": // Multiguna - Fasilitas Dana - Barang - Receivable Assignment
//                            case "1CGS4": // Multiguna - Fasilitas Dana - Jasa - Receivable Assignment
//                                          // End of alter by Anthony (Disburse)




//                            // SLB
//                            //case "1S1":
//                            //case "1S2":
//                            //case "1S3":
//                            //case "1S4":
//                            //case "1S5":

//                            // Alter by Anthony - 20151229 - OJK
//                            // Partial Terminate
//                            //// L
//                            //case "2L1":
//                            //case "2L2":
//                            //case "2L3":
//                            //case "2L4":
//                            //case "2L5":
//                            //case "2L6":

//                            //// CF
//                            //case "2C1":
//                            //case "2C2":
//                            //case "2C3":
//                            //case "2C4":
//                            //case "2C5":
//                            //case "2C6":
//                            // L
//                            case "2LIF1":
//                            case "2LPF1":
//                            case "2LGF1":
//                            case "2LIS1":
//                            case "2LPS1":
//                            case "2LMS1":

//                            // CF
//                            case "2CIB1":
//                            case "2CPB1":
//                            case "2CGB1":
//                            case "2CPJ1":
//                            case "2CMJ1":
//                            case "2CMU1":
//                            case "2CGG1":
//                            case "2CGS1":

//                            //Receivable Assignment
//                            // L
//                            case "2LIF4":
//                            case "2LPF4":
//                            case "2LGF4":
//                            case "2LIS4":
//                            case "2LPS4":
//                            case "2LMS4":

//                            // CF
//                            case "2CIB4":
//                            case "2CPB4":
//                            case "2CGB4":
//                            case "2CPJ4":
//                            case "2CMJ4":
//                            case "2CMU4":
//                            case "2CGG4":
//                            case "2CGS4":
//                            // End of alter by Anthony - OJK

//                            // SLB
//                            //case "2S1":
//                            //case "2S2":
//                            //case "2S3":
//                            //case "2S4":
//                            //case "2S5":

//                            // Alter by Anthony (Full Terminate) - OJK
//                            //// Full Terminate
//                            //// L
//                            //case "3L1":
//                            //case "3L2":
//                            //case "3L3":
//                            //case "3L4":
//                            //case "3L5":
//                            //case "3L6":

//                            case "3LIF1":
//                            case "3LPF1":
//                            case "3LGF1":
//                            case "3LIS1":
//                            case "3LPS1":
//                            case "3LMS1":
//                            case "3LGB1":

//                            //// CF
//                            //case "3C1":
//                            //case "3C2":
//                            //case "3C3":
//                            //case "3C4":
//                            //case "3C5":
//                            //case "3C6":

//                            case "3CIB1":
//                            case "3CPB1":
//                            case "3CGB1":
//                            case "3CPJ1":
//                            case "3CGJ1":
//                            case "3CMU1":
//                            case "3CGG1":
//                            case "3CGS1":
//                            // End of alter by Anthony (Full Terminate)

//                            // SLB
//                            //case "3S1":
//                            //case "3S2":
//                            //case "3S3":
//                            //case "3S4":
//                            //case "3S5":


//                            // Alter by Anthony (Floating Interest) - OJK
//                            // Floating Interest
//                            // L
//                            //case "4L1":
//                            //case "4L2":
//                            //case "4L3":
//                            //case "4L4":
//                            //case "4L5":
//                            //case "4L6":

//                            case "4LIF1":
//                            case "4LPF1":
//                            case "4LGF1":
//                            case "4LIS1":
//                            case "4LPS1":
//                            case "4LMS1":

//                            // CF
//                            //case "4C1":
//                            //case "4C2":
//                            //case "4C3":
//                            //case "4C4":
//                            //case "4C5":
//                            //case "4C6":

//                            case "4CIB1":
//                            case "4CPB1":
//                            case "4CGB1":
//                            case "4CPJ1":
//                            case "4CGJ1":
//                            case "4CMU1":
//                            case "4CGG1":
//                            case "4CGS1":
//                            // End of alter by Anthony (Floating)

//                            // SLB
//                            //case "4S1":
//                            //case "4S2":
//                            //case "4S3":
//                            //case "4S4":
//                            //case "4S5":


//                            // Alter by Anthony (Rescheduling) - Perubahan kode disesuaikan dengan kontrak OJK
//                            // Rescheduling
//                            // L
//                            //case "21L1":
//                            //case "21L2":
//                            //case "21L3":
//                            //case "21L4":
//                            //case "21L5":
//                            //case "21L6":

//                            case "21LIF1":
//                            case "21LPF1":
//                            case "21LGF1":
//                            case "21LIS1":
//                            case "21LPS1":
//                            case "21LMS1":
//                            case "29CGB1":
//                            case "29LMU1":
//                            case "29CGG1":
//                            case "29CGJ1":
//                            case "29CGS1":
//                            case "29LGF1":
//                            case "29LGB1":
//                            case "29CIB1":
//                            case "29CIS1":
//                            case "29LIB1":
//                            case "29LIF1":
//                            case "29LIS1":
//                            case "29LMS1":


//                            // CF
//                            //case "21C1":
//                            //case "21C2":
//                            //case "21C3":
//                            //case "21C4":
//                            //case "21C5":
//                            //case "21C6":

//                            case "21CIB1":
//                            case "21CPB1":
//                            case "21CGB1":
//                            case "21CPJ1":
//                            case "21CGJ1":
//                            case "21CMU1":
//                            case "21CGG1":
//                            case "21CGS1":
//                            // End of alter by Anthony (rescheduling)

//                            // ALter by bintang
//                            case "30LIF1":
//                            case "30LPF1":
//                            case "30LGF1":
//                            case "30LIS1":
//                            case "30LPS1":
//                            case "30LMS1":
//                            case "30CGB1":
//                            case "30LMU1":
//                            case "30CGG1":
//                            case "30CGJ1":
//                            case "30CGS1":
//                            case "30LGB1":
//                            case "30CIB1":
//                            case "30CIS1":
//                            case "30LIB1":
//                            case "21LMU1":
//                            case "21LGB1":
//                            case "21CIS1":
//                            case "21LIB1":
//                            case "FD1PP1":
//                            case "FD1PU1":
//                            case "FD1BL1":

//                            //end alter by bintang

//                            // Contract Interest Accrued
//                            // L
//                            case "24L4":

//                            // CF
//                            case "24C4":

//                                // SLB
//                                //case "21S1":
//                                //case "21S2":
//                                //case "21S3":
//                                //case "21S4":
//                                //case "21S5":

//                                X = 1;
//                                bCounter = 1;
//                                vLeNo = rowTrans["EntityNo"].ToString();
//                                //int PeriodFloating = 0;
//                                //PeriodFloating= Convert.ToInt16(rowTrans["SubEntityNo"]);
//                                int PeriodFloating = 0;
//                                object subEntityNo = rowTrans["SubEntityNo"];
//                                if (subEntityNo != null && subEntityNo != DBNull.Value)
//                                {
//                                    if (subEntityNo is int)
//                                    {
//                                        PeriodFloating = (int)subEntityNo; // Jika subEntityNo adalah int, ambil nilainya
//                                    }
//                                    else
//                                    {
//                                        // Jika subEntityNo bukan int, coba konversi ke int
//                                        if (int.TryParse(subEntityNo.ToString(), out int result))
//                                        {
//                                            PeriodFloating = result; // Jika berhasil dikonversi, ambil nilai hasil konversi
//                                        }
//                                        // Jika tidak berhasil dikonversi, periodFloating tetap 0 (nilai default)
//                                    }
//                                }
//                                //PeriodFloating = Convert.ToInt16(rowTrans["SubEntityNo"]);
//                                strSQL = "SELECT * FROM LoanJournalTD where JCode='" + JCode + "' and TCurrCode='" + sTCurrCode + "' and GroupType=" + GroupType + " ORDER BY Counter ";

//                                //strSQL += "and Counter=" + X;
//                                da = new SqlDataAdapter(strSQL, conn);
//                                da.SelectCommand.Transaction = trans;
//                                dtJTD = new DataTable();
//                                da.Fill(dtJTD);

//                                if (dtJTD.Rows.Count == 0)
//                                {
//                                    //WebMsgBox.Show("Seek Failure in Journal for " + JCode);
//                                }
//                                else
//                                {
//                                    TotDb = 0;
//                                    TotCr = 0;

//                                    SCode = rowTrans["JCode"].ToString();
//                                    GLSeqNo = GLSeqNo + 1;
//                                    //VchNo = DateTime.Today.ToString("yyMM") + GLSeqNo.ToString("000");
//                                    VchNo = Period.ToString("yyMM") + GLSeqNo.ToString("000");


//                                    for (int c = 0; c < dtJTD.Rows.Count; c++)
//                                    {
//                                        DataRow rowJTD = dtJTD.Rows[c];
//                                        ErrCode = 0;
//                                        DbCr = 1;
//                                        vAmt = 0;

//                                        if (Convert.ToInt16(rowJTD["RetrieveType"]) == 1)
//                                        {
//                                            //if (!string.IsNullOrEmpty(rowTrans["OuterCode"].ToString()))
//                                            //{
//                                            //    sAccNo = GetFieldValue("GelAcc", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                            //    sCcy = rowJTD["CurrCode"].ToString();
//                                            //    sDesc = GetFieldValue("BankName", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                            //    sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            //}
//                                            //else
//                                            //{
//                                            //    sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            //    sAccNo = rowJTD["Acc"].ToString();
//                                            //    sCcy = rowJTD["CurrCode"].ToString();
//                                            //    sDesc = rowJTD["Description"].ToString();
//                                            //}
//                                            sAccNo = GetFieldValue("GelAcc", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                            sCcy = rowJTD["CurrCode"].ToString();
//                                            sDesc = GetFieldValue("BankName", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                            sAccCashBasis = rowJTD["AccCashBasis"].ToString();

//                                        }
//                                        else
//                                        {
//                                            sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            sAccNo = rowJTD["Acc"].ToString();
//                                            sCcy = rowJTD["CurrCode"].ToString();
//                                            sDesc = string.IsNullOrEmpty(rowJTD["Description"].ToString()) ? "0" : rowJTD["Description"].ToString();
//                                        }

//                                        switch (strSCode)
//                                        {
//                                            //Reverse Disburst
//                                            case "L006":
//                                            case "FD02":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;


//                                            //Disburst
//                                            case "L001":
//                                            case "FD01":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Reverse Partial 
//                                            case "L007":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            //Partial
//                                            case "L002":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Reverse Full
//                                            case "L008":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            case "L003":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Add By: Tomy
//                                            //Floating Interest
//                                            case "L004":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Rescheduling
//                                            case "L021":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Contract Interest Accrued
//                                            case "L024":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Write OFF
//                                            case "L023":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            case "L030":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;
//                                        }

//                                        GLCashBasis = sAccCashBasis;
//                                        GLBranchCode = strBranchCode;

//                                        GL_SCode = SCode;
//                                        GL_Voucher = VchNo;
//                                        GL_Counter = bCounter;
//                                        GL_EntryDate = DateTime.Today;
//                                        GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                        GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                        GL_Reverse = false;
//                                        GL_Account = sAccNo;
//                                        GL_Ccy = sCcy;
//                                        // Alter - Anthony - 20200709
//                                        //GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString();

//                                        using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
//                                        {
//                                            db.CommandText = "SELECT BranchCode, RepresentativeOffice FROM Lease WHERE LeaseNo = @leaseno";
//                                            db.CommandType = System.Data.CommandType.Text;
//                                            db.AddParameter("@leaseno", SqlDbType.VarChar, vLeNo);
//                                            db.Open();

//                                            db.ExecuteReader();

//                                            using (SqlDataReader rd = db.DbDataReader as SqlDataReader)
//                                            {
//                                                if (rd.HasRows)
//                                                {
//                                                    while (rd.Read())
//                                                    {
//                                                        if (rd["RepresentativeOffice"] == DBNull.Value || string.IsNullOrEmpty(rd["RepresentativeOffice"] as string))
//                                                            GL_Dept = rd["BranchCode"] as string;
//                                                        else
//                                                            GL_Dept = rd["RepresentativeOffice"] as string;
//                                                    }
//                                                }
//                                            }

//                                            db.Close();
//                                        }


//                                        // End alter - Anthony - 20200709

//                                        if (strSCode == "L021" || strSCode == "L024" || strSCode == "L023")
//                                        {
//                                            //GetvAmtErrCode For Approve (status=0)
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 2)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                        }
//                                        else if (strSCode == "L030")
//                                        {
//                                            //ini Vleno nya jadi TKey untuk principal
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 2)), rowJTD["RetrieveAmt"].ToString(), Tkey, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                        }
//                                        else if (GL_SCode.Substring(0, 2) == "FD")//jika funding
//                                        {
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(2, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, PeriodFloating, GroupType);
//                                        }
//                                        else
//                                        {
//                                            //GetvAmtErrCode For Approve (status=0)
//                                            //ini diganti doang xperiod menjadi PeriodFloating
//                                            //GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, PeriodFloating, GroupType);
//                                        }

//                                        cExchRateJournal = sCcy != sBaseCcy ? GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;
//                                        cExchRateVoucher = sTCurrCode != sBaseCcy ? GetExchangeRate(sTCurrCode, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;

//                                        if (sCcy != sTCurrCode)
//                                            vAmt = (vAmt * cExchRateVoucher / cExchRateJournal);
//                                        vAmt = vAmt * DbCr;
//                                        GL_Amount = vAmt;
//                                        GL_Description = sDesc + " - " + vLeNo;
//                                        GL_LeaseNo = vLeNo;
//                                        //GL_DocNo = rowTrans["TransCode"].ToString();
//                                        GL_DocNo = strSCode;
//                                        GL_TransferToGL = false;
//                                        GL_ErrCode = ErrCode;

//                                        // Alter - Anthony - 20220521
//                                        //if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
//                                        switch (JCode)
//                                        {
//                                            case "1LIF4":
//                                            case "1LPF4":
//                                            case "1LGF4":
//                                            case "1LIS4":
//                                            case "1LPS4":
//                                            case "1LMS4":
//                                            case "1CIB4":
//                                            case "1CPB4":
//                                            case "1CGB4":
//                                            case "1CPJ4":
//                                            case "1CGJ4":
//                                            case "1CMU4":
//                                            case "1CGG4":
//                                            case "1CGS4":

//                                            case "1LIF6":
//                                            case "1LPF6":
//                                            case "1LGF6":
//                                            case "1LIS6":
//                                            case "1LPS6":
//                                            case "1LMS6":
//                                            case "1CIB6":
//                                            case "1CPB6":
//                                            case "1CGB6":
//                                            case "1CPJ6":
//                                            case "1CGJ6":
//                                            case "1CMU6":
//                                            case "1CGG6":
//                                            case "1CGS6":
//                                                if (rowJTD["RetrieveAmt"].ToString() == "HOP")
//                                                {
//                                                    // Get HOPNO
//                                                    string HOPNO, HOPBranchName;

//                                                    HOPNO = "";
//                                                    HOPBranchName = "";

//                                                    // Get HOPNO
//                                                    HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "HopNo",
//                                                        "tblBranch",
//                                                        "BranchCode='" + GLBranchCode + "'").ToString();

//                                                    // Get HOPBranchName
//                                                    HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "Name",
//                                                        "ACFGLMH",
//                                                        "ACC='" + HOPNO + "'").ToString();

//                                                    GL_Account = HOPNO;
//                                                    GL_Description = HOPBranchName;
//                                                }
//                                                break;
//                                        }


//                                        int msg1 = 1;
//                                        if (GL_Amount != 0)
//                                        {
//                                            Loan_SPUpdateGLTrans(
//                                           strTransNo,
//                                           GL_SCode,
//                                           GL_Voucher,
//                                           GL_Counter,
//                                           GL_EntryDate,
//                                           GL_TransDate,
//                                           GL_Reverse,
//                                           GL_Dept,
//                                           GL_Account,
//                                           GL_Ccy,
//                                           GL_Amount,
//                                           GL_Description,
//                                           GL_LeaseNo,
//                                           GL_DocNo,
//                                           GL_TransferToGL,
//                                           GL_ErrCode,
//                                           GL_OriTransDate,
//                                           GLBranchCode,
//                                           GLCashBasis,
//                                           ref msg1);
//                                        }


//                                        if (msg1 == 0)
//                                        {
//                                            //WebMsgBox.Show("Update LeGLTrans Failed!");
//                                        }

//                                        if (sCcy != sBaseCcy)
//                                        {
//                                            //CreateEqvJournal
//                                            cAmount = vAmt;
//                                            cExchRate = GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"]));
//                                            bCounter++;

//                                            GLCashBasis = sAccCashBasis;
//                                            GLBranchCode = strBranchCode;

//                                            GL_SCode = SCode;
//                                            GL_Voucher = VchNo;
//                                            GL_Counter = bCounter;
//                                            GL_EntryDate = DateTime.Today;
//                                            GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                            GL_Reverse = false;
//                                            GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Loan", "LNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Loan", "LNo='" + vLeNo + "'").ToString();
//                                            GL_Account = sAccNo;
//                                            GL_Ccy = sBaseCcy;
//                                            GL_Amount = cAmount * cExchRate;
//                                            GL_Description = rowJTD["Description"].ToString();
//                                            GL_LeaseNo = vLeNo;
//                                            //GL_DocNo = rowTrans["TransCode"].ToString();
//                                            GL_DocNo = strSCode;
//                                            GL_TransferToGL = false;
//                                            GL_ErrCode = ErrCode;
//                                            GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);

//                                            if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
//                                            {
//                                                // Get HOPNO
//                                                string HOPNO, HOPBranchName;

//                                                HOPNO = "";
//                                                HOPBranchName = "";

//                                                // Get HOPNO
//                                                HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                    "HopNo",
//                                                    "tblBranch",
//                                                    "BranchCode='" + GLBranchCode + "'").ToString();

//                                                // Get HOPBranchName
//                                                HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                    "Name",
//                                                    "ACFGLMH",
//                                                    "ACC='" + HOPNO + "'").ToString();

//                                                GL_Account = HOPNO;
//                                                GL_Description = HOPBranchName;
//                                            }
//                                            int msg2 = 1;
//                                            Loan_SPUpdateGLTrans(
//                                                strTransNo,
//                                                GL_SCode,
//                                                GL_Voucher,
//                                                GL_Counter,
//                                                GL_EntryDate,
//                                                GL_TransDate,
//                                                GL_Reverse,
//                                                GL_Dept,
//                                                GL_Account,
//                                                GL_Ccy,
//                                                GL_Amount,
//                                                GL_Description,
//                                                GL_LeaseNo,
//                                                GL_DocNo,
//                                                GL_TransferToGL,
//                                                GL_ErrCode,
//                                                GL_OriTransDate,
//                                                GLBranchCode,
//                                                GLCashBasis,
//                                                ref msg2);
//                                            if (msg2 == 0)
//                                            {
//                                                //WebMsgBox.Show("Update LeGLTrans Failed!");
//                                            }
//                                        }

//                                        if (vAmt < 0)
//                                            TotCr = TotCr + vAmt;
//                                        else
//                                            TotDb = TotDb + vAmt;

//                                        bCounter++;
//                                        X++;
//                                    }
//                                    int isReverse = 1;
//                                    switch (strSCode)
//                                    {
//                                        //Reverse Disburst
//                                        case "L006":
//                                            isReverse = -1;
//                                            break;
//                                    }
//                                    if (strSCode == "L006" || strSCode == "L001")
//                                    {
//                                        var fees = IDS.LeaseTrans.FeesDetail.GetFeesDetails(ContractNo);
//                                        var le = IDS.LeaseTrans.Lease.GetLease(ContractNo);
//                                        var mstFee = IDS.GeneralTable.Fees.GetFees();
//                                        if (fees.Count() > 0)
//                                        {
//                                            foreach (var fee in fees)
//                                            {
//                                                if (fee.FeesType == 6)
//                                                {
//                                                    IDSTools.UpdateCustomerDepositLease(fee.LeaseNo, fee.InAmount);
//                                                }
//                                                int msg1 = 1;
//                                                GL_SCode = SCode;
//                                                GL_Voucher = VchNo;
//                                                GL_Counter = X;
//                                                GL_EntryDate = DateTime.Today.Date;
//                                                GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_Reverse = false;
//                                                GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccPayable;
//                                                GL_Ccy = "IDR";
//                                                GL_Description = "Payable To " + mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).FeeName + " " + vLeNo;
//                                                GL_LeaseNo = vLeNo;
//                                                //GL_DocNo = rowTrans["TransCode"].ToString();
//                                                GL_DocNo = strSCode;
//                                                GL_TransferToGL = false;
//                                                GL_ErrCode = ErrCode;
//                                                GLBranchCode = "IDS";
//                                                GLCashBasis = "";
//                                                GL_Amount = fee.ThirdPartyAmt;

//                                                GL_LeaseNo = ContractNo;

//                                                if (GL_Amount != 0)
//                                                {
//                                                    Loan_SPUpdateGLTrans(
//                                                    strTransNo,
//                                                    GL_SCode,
//                                                    GL_Voucher,
//                                                    GL_Counter,
//                                                    GL_EntryDate,
//                                                    GL_TransDate,
//                                                    GL_Reverse,
//                                                    GL_Dept,
//                                                    GL_Account,
//                                                    GL_Ccy,
//                                                    GL_Amount * -1 * isReverse,
//                                                    GL_Description,
//                                                    GL_LeaseNo,
//                                                    GL_DocNo,
//                                                    GL_TransferToGL,
//                                                    GL_ErrCode,
//                                                    GL_OriTransDate,
//                                                    GLBranchCode,
//                                                    GLCashBasis,
//                                                    ref msg1);
//                                                    X++;
//                                                }

//                                                GL_SCode = SCode;
//                                                GL_Voucher = VchNo;
//                                                GL_Counter = X;
//                                                GL_EntryDate = DateTime.Today.Date;
//                                                GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_Reverse = false;
//                                                if ((int)le.FinanceMethod == 1)
//                                                {
//                                                    if ((int)le.LeaseType == 2)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVSaleLeaseback;
//                                                    }
//                                                    else if ((int)le.LeaseType == 3)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVInstallment;
//                                                    }
//                                                    else
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVFinanceLease;
//                                                    }
//                                                }
//                                                else if ((int)le.FinanceMethod == 2)
//                                                {
//                                                    if ((int)le.LeaseType == 4)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMKModalUsaha;
//                                                    }
//                                                    else
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMKSaleLeaseback;

//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    if ((int)le.LeaseType == 3)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGInstallment;
//                                                    }
//                                                    else if ((int)le.LeaseType == 5)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGFasilitasDana;
//                                                    }
//                                                    else if ((int)le.LeaseType == 1)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGFinanceLease;
//                                                    }
//                                                }
//                                                GL_Ccy = "IDR";
//                                                GL_Description = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).Description + " " + vLeNo;
//                                                GL_LeaseNo = vLeNo;
//                                                //GL_DocNo = rowTrans["TransCode"].ToString();
//                                                GL_DocNo = strSCode;
//                                                GL_TransferToGL = false;
//                                                GL_ErrCode = ErrCode;
//                                                GLBranchCode = "IDS";
//                                                GLCashBasis = "";
//                                                GL_Amount = fee.InAmount - fee.ThirdPartyAmt;

//                                                GL_LeaseNo = ContractNo;

//                                                if (GL_Amount != 0)
//                                                {
//                                                    Loan_SPUpdateGLTrans(
//                                                    strTransNo,
//                                                    GL_SCode,
//                                                    GL_Voucher,
//                                                    GL_Counter,
//                                                    GL_EntryDate,
//                                                    GL_TransDate,
//                                                    GL_Reverse,
//                                                    GL_Dept,
//                                                    GL_Account,
//                                                    GL_Ccy,
//                                                    GL_Amount * -1 * isReverse,
//                                                    GL_Description,
//                                                    GL_LeaseNo,
//                                                    GL_DocNo,
//                                                    GL_TransferToGL,
//                                                    GL_ErrCode,
//                                                    GL_OriTransDate,
//                                                    GLBranchCode,
//                                                    GLCashBasis,
//                                                    ref msg1);
//                                                    X++;
//                                                }
//                                            }
//                                        }
//                                    }


//                                }
//                                break;
//                        }
//                    }
//                    trans.Commit();
//                    message = 1;
//                }
//                catch (Exception ex)
//                {
//                    message = 0;
//                    trans.Rollback();
//                    //WebMsgBox.Show(ex.Message);
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }



//        public void CreateGLTfTransNewRA(
//        string strTransNo,
//        string strBranchCode,
//        string strSCode,
//        long NoProcess,
//        DateTime Period,
//        string strVoucher,
//        string ContractChange,
//        ref int message
//    )
//        {
//            CreateGLTfTransNewRA(
//                strTransNo,
//                strBranchCode,
//                strSCode,
//                NoProcess,
//                Period,
//                strVoucher,
//                ContractChange,
//                ref message,
//                0
//            );
//        }

//        public void CreateGLTfTransNewRA(
//            string strTransNo,
//            string strBranchCode,
//            string strSCode,
//            long NoProcess,
//            DateTime Period,
//            string strVoucher,
//            string ContractChange,
//            ref int message,
//            int xPeriod
//        )
//        {
//            string VchNo, SCode, JCode, vLeNo, sAccNo;
//            string sAccCashBasis;
//            string sCcy, sTCurrCode;
//            Byte X, I, bCounter;
//            long Done;
//            int DbCr, ErrCode, GLSeqNo = 0;
//            double Portion = 0;
//            Byte nPeriod;
//            double TotDb, TotCr, vAmt, cExchRateJournal;
//            double cExchRateVoucher, cAmount, cExchRate;
//            DateTime CSLTransDate;
//            string sDesc;
//            DateTime StopAccrueDate;
//            int Action = 0;
//            string sBaseCcy;
//            DataTable dtJTD;
//            DataTable dtGLTrans;
//            DataTable dtLease;

//            //Variable For GLTrans
//            string GL_SCode, GL_Voucher, GL_Account, GL_Ccy;
//            string GL_Dept = "", GL_Description, GL_LeaseNo, GL_DocNo;
//            byte GL_Counter;
//            DateTime GL_EntryDate, GL_OriTransDate, GL_TransDate;
//            bool GL_Reverse, GL_TransferToGL;
//            double GL_Amount;
//            int GL_ErrCode;
//            string GLBranchCode, GLCashBasis;

//            string ContractNo = "";
//            string CustNo = "";
//            string CustBranch = "";
//            int GrpNo = 0;
//            int GroupType = 0;
//            string Tkey = "";

//            sBaseCcy = IDS.GeneralTable.Syspar.GetInstance().BaseCCy;

//            // Get Contract No 
//            ContractNo = GetFieldValue(
//                 "EntityNo",
//                 "LoanTrans",
//                 "TransNo='" + strTransNo + "'"
//                ).ToString();

//            // Get Subentity No 
//            Tkey = GetFieldValue(
//                 "SubEntityNo",
//                 "LoanTrans",
//                 "TransNo='" + strTransNo + "'"
//                ).ToString();

//            // Get Customer No
//            CustNo = GetFieldValue(
//                 "FunderCode",
//                 "LeRecvAssignment",
//                 "AssignNo='" + ContractNo + "'"
//                ).ToString();
//            CustBranch = "";
//            GrpNo = 0;
//            GroupType = 0;
//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                SqlTransaction trans = null;
//                trans = conn.BeginTransaction();
//                clsModule mdl = new clsModule();
//                SqlCommand cmd = new SqlCommand();
//                cmd.Connection = conn;
//                cmd.Transaction = trans;
//                cmd.CommandType = CommandType.StoredProcedure;
//                try
//                {
//                    string strSQL;
//                    strSQL = "RA_SPpruCreateGLTfTransNewPrsLoan '" + Period + "','" + strVoucher + "','" + strTransNo + "'";
//                    SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
//                    da.SelectCommand.Transaction = trans;
//                    DataTable dtTrans = new DataTable();
//                    da.Fill(dtTrans);
//                    NoProcess = NoProcess + dtTrans.Rows.Count;
//                    if (dtTrans.Rows.Count == 0)
//                    {
//                        message = 0;
//                        // WebMsgBox.Show("No Transaction To Be Created!");
//                        return;
//                    }

//                    ErrCode = 0;
//                    Done = 0;
//                    for (int i = 0; i < dtTrans.Rows.Count; i++)
//                    {
//                        DataRow rowTrans = dtTrans.Rows[i];
//                        Done = Done + 1;
//                        if (mdl.Left(rowTrans["JCode"].ToString(), 2) != "LT")
//                            JCode = rowTrans["JCode"].ToString(); //mdl.Left(rowTrans["JCode"].ToString(), 1) + "1" + mdl.Right(rowTrans["JCode"].ToString(), 2);
//                        else
//                            JCode = rowTrans["JCode"].ToString();

//                        sTCurrCode = rowTrans["CurrCode"].ToString();

//                        string Vch;
//                        string strKondisi;

//                        strKondisi = "SCode='" + JCode + "' AND YEAR(TransDate)='" + Period.ToString("yyyy") + "' AND Month(TransDate)='" + Convert.ToInt16(Period.ToString("MM")) + "' AND BranchCode='" + strBranchCode + "'";
//                        // "' AND MONTH(EntryDate)='" + Period.ToString("MM") +
//                        Vch = GetMaxFieldValue("right(Voucher,3)", "LoanGlTrans", strKondisi).ToString();
//                        if (!string.IsNullOrEmpty(Vch) && Vch != "0")
//                        {
//                            GLSeqNo = Convert.ToInt16(Vch.Substring(Vch.Length - 3));
//                        }
//                        else
//                        {
//                            GLSeqNo = 0;
//                        }

//                        switch (JCode)
//                        {
//                            // New Contract 
//                            // Leasing Normal

//                            // Alter by Anthony (Disburse) - Perubahan kode disesuaikan dengan kontrak OJK
//                            //case "1L1":
//                            //case "1L2":
//                            //case "1L3":
//                            //case "1L4":
//                            //case "1L5":
//                            //case "1L6":

//                            case "1LIF1": // Investasi - Finance Lease - Normal
//                            case "1LPF1": // Proyek - Finance Lease - Normal
//                            case "1LGF1": // Multiguna - Finance Lease - Normal
//                            case "1LIS1": // Investasi - Sale & Leaseback - Normal
//                            case "1LPS1": // Proyek - Sale & Leaseback - Normal
//                            case "1LMS1": // Modal Kerja - Sale & Leaseback - Normal
//                            case "1SIF1":
//                            // Leasing Receivable Assignment
//                            case "1LIF4": // Investasi - Finance Lease - Receivable Assignment
//                            case "1LPF4": // Proyek - Finance Lease - Receivable Assignment
//                            case "1LGF4": // Multiguna - Finance Lease - Receivable Assignment
//                            case "1LIS4": // Investasi - Sale & Leaseback - Receivable Assignment
//                            case "1LPS4": // Proyek - Sale & Leaseback - Receivable Assignment
//                            case "1LMS4": // Modal Kerja - Sale & Leaseback - Receivable Assignment

//                            //case "1C1":
//                            //case "1C2":
//                            //case "1C3":
//                            //case "1C4":
//                            //case "1C5":
//                            //case "1C6":

//                            // CF Normal
//                            case "1CIB1": // Investasi - Installment Barang - Normal
//                            case "1CPB1": // Proyek - Installment Barang - Normal
//                            case "1CGB1": // Multiguna - Installment Barang - Normal
//                            case "1CPJ1": // Proyek - Installment Jasa - Normal
//                            case "1CGJ1": // Multiguna - Installment Jasa - Normal
//                            case "1CMU1": // Modal Kerja - Modal Usaha - Normal
//                            case "1CGG1": // Multiguna - Fasilitas Dana - Barang - Normal
//                            case "1CGS1": // Multiguna - Fasilitas Dana - Jasa - Normal

//                            // CF Receivable Assigment
//                            case "1CIB4": // Investasi - Installment Barang - Receivable Assigment
//                            case "1CPB4": // Proyek - Installment Barang - Receivable Assigment
//                            case "1CGB4": // Multiguna - Installment Barang - Receivable Assigment
//                            case "1CPJ4": // Proyek - Installment Jasa - Receivable Assigment
//                            case "1CGJ4": // Multiguna - Installment Jasa - Receivable Assigment
//                            case "1CMU4": // Modal Kerja - Modal Usaha - Receivable Assigment
//                            case "1CGG4": // Multiguna - Fasilitas Dana - Barang - Receivable Assignment
//                            case "1CGS4": // Multiguna - Fasilitas Dana - Jasa - Receivable Assignment
//                                          // End of alter by Anthony (Disburse)




//                            // SLB
//                            //case "1S1":
//                            //case "1S2":
//                            //case "1S3":
//                            //case "1S4":
//                            //case "1S5":

//                            // Alter by Anthony - 20151229 - OJK
//                            // Partial Terminate
//                            //// L
//                            //case "2L1":
//                            //case "2L2":
//                            //case "2L3":
//                            //case "2L4":
//                            //case "2L5":
//                            //case "2L6":

//                            //// CF
//                            //case "2C1":
//                            //case "2C2":
//                            //case "2C3":
//                            //case "2C4":
//                            //case "2C5":
//                            //case "2C6":
//                            // L
//                            case "2LIF1":
//                            case "2LPF1":
//                            case "2LGF1":
//                            case "2LIS1":
//                            case "2LPS1":
//                            case "2LMS1":

//                            // CF
//                            case "2CIB1":
//                            case "2CPB1":
//                            case "2CGB1":
//                            case "2CPJ1":
//                            case "2CMJ1":
//                            case "2CMU1":
//                            case "2CGG1":
//                            case "2CGS1":

//                            //Receivable Assignment
//                            // L
//                            case "2LIF4":
//                            case "2LPF4":
//                            case "2LGF4":
//                            case "2LIS4":
//                            case "2LPS4":
//                            case "2LMS4":

//                            // CF
//                            case "2CIB4":
//                            case "2CPB4":
//                            case "2CGB4":
//                            case "2CPJ4":
//                            case "2CMJ4":
//                            case "2CMU4":
//                            case "2CGG4":
//                            case "2CGS4":
//                            // End of alter by Anthony - OJK

//                            // SLB
//                            //case "2S1":
//                            //case "2S2":
//                            //case "2S3":
//                            //case "2S4":
//                            //case "2S5":

//                            // Alter by Anthony (Full Terminate) - OJK
//                            //// Full Terminate
//                            //// L
//                            //case "3L1":
//                            //case "3L2":
//                            //case "3L3":
//                            //case "3L4":
//                            //case "3L5":
//                            //case "3L6":

//                            case "3LIF1":
//                            case "3LPF1":
//                            case "3LGF1":
//                            case "3LIS1":
//                            case "3LPS1":
//                            case "3LMS1":
//                            case "3LGB1":

//                            //// CF
//                            //case "3C1":
//                            //case "3C2":
//                            //case "3C3":
//                            //case "3C4":
//                            //case "3C5":
//                            //case "3C6":

//                            case "3CIB1":
//                            case "3CPB1":
//                            case "3CGB1":
//                            case "3CPJ1":
//                            case "3CGJ1":
//                            case "3CMU1":
//                            case "3CGG1":
//                            case "3CGS1":
//                            // End of alter by Anthony (Full Terminate)

//                            // SLB
//                            //case "3S1":
//                            //case "3S2":
//                            //case "3S3":
//                            //case "3S4":
//                            //case "3S5":


//                            // Alter by Anthony (Floating Interest) - OJK
//                            // Floating Interest
//                            // L
//                            //case "4L1":
//                            //case "4L2":
//                            //case "4L3":
//                            //case "4L4":
//                            //case "4L5":
//                            //case "4L6":

//                            case "4LIF1":
//                            case "4LPF1":
//                            case "4LGF1":
//                            case "4LIS1":
//                            case "4LPS1":
//                            case "4LMS1":

//                            // CF
//                            //case "4C1":
//                            //case "4C2":
//                            //case "4C3":
//                            //case "4C4":
//                            //case "4C5":
//                            //case "4C6":

//                            case "4CIB1":
//                            case "4CPB1":
//                            case "4CGB1":
//                            case "4CPJ1":
//                            case "4CGJ1":
//                            case "4CMU1":
//                            case "4CGG1":
//                            case "4CGS1":
//                            // End of alter by Anthony (Floating)

//                            // SLB
//                            //case "4S1":
//                            //case "4S2":
//                            //case "4S3":
//                            //case "4S4":
//                            //case "4S5":


//                            // Alter by Anthony (Rescheduling) - Perubahan kode disesuaikan dengan kontrak OJK
//                            // Rescheduling
//                            // L
//                            //case "21L1":
//                            //case "21L2":
//                            //case "21L3":
//                            //case "21L4":
//                            //case "21L5":
//                            //case "21L6":

//                            case "21LIF1":
//                            case "21LPF1":
//                            case "21LGF1":
//                            case "21LIS1":
//                            case "21LPS1":
//                            case "21LMS1":
//                            case "29CGB1":
//                            case "29LMU1":
//                            case "29CGG1":
//                            case "29CGJ1":
//                            case "29CGS1":
//                            case "29LGF1":
//                            case "29LGB1":
//                            case "29CIB1":
//                            case "29CIS1":
//                            case "29LIB1":
//                            case "29LIF1":
//                            case "29LIS1":
//                            case "29LMS1":


//                            // CF
//                            //case "21C1":
//                            //case "21C2":
//                            //case "21C3":
//                            //case "21C4":
//                            //case "21C5":
//                            //case "21C6":

//                            case "21CIB1":
//                            case "21CPB1":
//                            case "21CGB1":
//                            case "21CPJ1":
//                            case "21CGJ1":
//                            case "21CMU1":
//                            case "21CGG1":
//                            case "21CGS1":
//                            // End of alter by Anthony (rescheduling)

//                            // ALter by bintang
//                            case "30LIF1":
//                            case "30LPF1":
//                            case "30LGF1":
//                            case "30LIS1":
//                            case "30LPS1":
//                            case "30LMS1":
//                            case "30CGB1":
//                            case "30LMU1":
//                            case "30CGG1":
//                            case "30CGJ1":
//                            case "30CGS1":
//                            case "30LGB1":
//                            case "30CIB1":
//                            case "30CIS1":
//                            case "30LIB1":
//                            case "21LMU1":
//                            case "21LGB1":
//                            case "21CIS1":
//                            case "21LIB1":
//                            case "FD1PP1":
//                            case "FD1PU1":
//                            case "FD1BL1":

//                            //end alter by bintang

//                            // Contract Interest Accrued
//                            // L
//                            case "24L4":

//                            // CF
//                            case "24C4":

//                                // SLB
//                                //case "21S1":
//                                //case "21S2":
//                                //case "21S3":
//                                //case "21S4":
//                                //case "21S5":

//                                X = 1;
//                                bCounter = 1;
//                                vLeNo = rowTrans["EntityNo"].ToString();
//                                //int PeriodFloating = 0;
//                                //PeriodFloating= Convert.ToInt16(rowTrans["SubEntityNo"]);
//                                int PeriodFloating = 0;
//                                object subEntityNo = rowTrans["SubEntityNo"];
//                                if (subEntityNo != null && subEntityNo != DBNull.Value)
//                                {
//                                    if (subEntityNo is int)
//                                    {
//                                        PeriodFloating = (int)subEntityNo; // Jika subEntityNo adalah int, ambil nilainya
//                                    }
//                                    else
//                                    {
//                                        // Jika subEntityNo bukan int, coba konversi ke int
//                                        if (int.TryParse(subEntityNo.ToString(), out int result))
//                                        {
//                                            PeriodFloating = result; // Jika berhasil dikonversi, ambil nilai hasil konversi
//                                        }
//                                        // Jika tidak berhasil dikonversi, periodFloating tetap 0 (nilai default)
//                                    }
//                                }
//                                //PeriodFloating = Convert.ToInt16(rowTrans["SubEntityNo"]);
//                                strSQL = "SELECT * FROM LoanJournalTD where JCode='" + JCode + "' and TCurrCode='" + sTCurrCode + "' and GroupType=" + GroupType + " ORDER BY Counter ";

//                                //strSQL += "and Counter=" + X;
//                                da = new SqlDataAdapter(strSQL, conn);
//                                da.SelectCommand.Transaction = trans;
//                                dtJTD = new DataTable();
//                                da.Fill(dtJTD);

//                                if (dtJTD.Rows.Count == 0)
//                                {
//                                    //WebMsgBox.Show("Seek Failure in Journal for " + JCode);
//                                }
//                                else
//                                {
//                                    TotDb = 0;
//                                    TotCr = 0;

//                                    SCode = rowTrans["JCode"].ToString();
//                                    GLSeqNo = GLSeqNo + 1;
//                                    //VchNo = DateTime.Today.ToString("yyMM") + GLSeqNo.ToString("000");
//                                    VchNo = Period.ToString("yyMM") + GLSeqNo.ToString("000");


//                                    for (int c = 0; c < dtJTD.Rows.Count; c++)
//                                    {
//                                        DataRow rowJTD = dtJTD.Rows[c];
//                                        ErrCode = 0;
//                                        DbCr = 1;
//                                        vAmt = 0;

//                                        if (Convert.ToInt16(rowJTD["RetrieveType"]) == 1)
//                                        {
//                                            if (!string.IsNullOrEmpty(rowTrans["OuterCode"].ToString()))
//                                            {
//                                                sAccNo = GetFieldValue("GelAcc", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                                sCcy = rowJTD["CurrCode"].ToString();
//                                                sDesc = GetFieldValue("BankName", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                                sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            }
//                                            else
//                                            {
//                                                sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                                sAccNo = rowJTD["Acc"].ToString();
//                                                sCcy = rowJTD["CurrCode"].ToString();
//                                                sDesc = rowJTD["Description"].ToString();
//                                            }

//                                        }
//                                        else
//                                        {
//                                            sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            sAccNo = rowJTD["Acc"].ToString();
//                                            sCcy = rowJTD["CurrCode"].ToString();
//                                            sDesc = string.IsNullOrEmpty(rowJTD["Description"].ToString()) ? "0" : rowJTD["Description"].ToString();
//                                        }

//                                        switch (strSCode)
//                                        {
//                                            //Reverse Disburst
//                                            case "L006":
//                                            case "FD02":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;


//                                            //Disburst
//                                            case "L001":
//                                            case "FD01":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Reverse Partial 
//                                            case "L007":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            //Partial
//                                            case "L002":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Reverse Full
//                                            case "L008":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            case "L003":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Add By: Tomy
//                                            //Floating Interest
//                                            case "L004":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Rescheduling
//                                            case "L021":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Contract Interest Accrued
//                                            case "L024":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Write OFF
//                                            case "L023":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            case "L030":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;
//                                        }

//                                        GLCashBasis = sAccCashBasis;
//                                        GLBranchCode = strBranchCode;

//                                        GL_SCode = SCode;
//                                        GL_Voucher = VchNo;
//                                        GL_Counter = bCounter;
//                                        GL_EntryDate = DateTime.Today;
//                                        GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                        GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                        GL_Reverse = false;
//                                        GL_Account = sAccNo;
//                                        GL_Ccy = sCcy;
//                                        // Alter - Anthony - 20200709
//                                        //GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString();

//                                        using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
//                                        {
//                                            db.CommandText = "SELECT BranchCode, RepresentativeOffice FROM Lease WHERE LeaseNo = @leaseno";
//                                            db.CommandType = System.Data.CommandType.Text;
//                                            db.AddParameter("@leaseno", SqlDbType.VarChar, vLeNo);
//                                            db.Open();

//                                            db.ExecuteReader();

//                                            using (SqlDataReader rd = db.DbDataReader as SqlDataReader)
//                                            {
//                                                if (rd.HasRows)
//                                                {
//                                                    while (rd.Read())
//                                                    {
//                                                        if (rd["RepresentativeOffice"] == DBNull.Value || string.IsNullOrEmpty(rd["RepresentativeOffice"] as string))
//                                                            GL_Dept = rd["BranchCode"] as string;
//                                                        else
//                                                            GL_Dept = rd["RepresentativeOffice"] as string;
//                                                    }
//                                                }
//                                            }

//                                            db.Close();
//                                        }


//                                        // End alter - Anthony - 20200709

//                                        if (strSCode == "L021" || strSCode == "L024" || strSCode == "L023")
//                                        {
//                                            //GetvAmtErrCode For Approve (status=0)
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 2)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                        }
//                                        else if (strSCode == "L030")
//                                        {
//                                            //ini Vleno nya jadi TKey untuk principal
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 2)), rowJTD["RetrieveAmt"].ToString(), Tkey, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                        }
//                                        else if (GL_SCode.Substring(0, 2) == "FD")//jika funding
//                                        {
//                                            if (ContractChange == "")
//                                            {
//                                                GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(2, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, PeriodFloating, GroupType);
//                                            }
//                                            else
//                                            {
//                                                if (rowJTD["RetrieveAmt"].ToString() == "RAB" || rowJTD["RetrieveAmt"].ToString() == "BRA")
//                                                    vAmt = IDS.LoanTrans.LeRecvAssignment.GetAmountContractSwap(vLeNo, ContractChange);
//                                            }
//                                        }
//                                        else
//                                        {
//                                            //GetvAmtErrCode For Approve (status=0)
//                                            //ini diganti doang xperiod menjadi PeriodFloating
//                                            //GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, PeriodFloating, GroupType);
//                                        }

//                                        cExchRateJournal = sCcy != sBaseCcy ? GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;
//                                        cExchRateVoucher = sTCurrCode != sBaseCcy ? GetExchangeRate(sTCurrCode, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;

//                                        if (sCcy != sTCurrCode)
//                                            vAmt = (vAmt * cExchRateVoucher / cExchRateJournal);
//                                        vAmt = vAmt * DbCr;
//                                        GL_Amount = vAmt;
//                                        if (ContractChange == "")
//                                        {
//                                            GL_Description = sDesc + " " + vLeNo;
//                                        }
//                                        else
//                                        {
//                                            GL_Description = sDesc + " " + vLeNo + " For Contract Change : " + ContractChange;

//                                        }
//                                        GL_LeaseNo = vLeNo;
//                                        //GL_DocNo = rowTrans["TransCode"].ToString();
//                                        GL_DocNo = strSCode;
//                                        GL_TransferToGL = false;
//                                        GL_ErrCode = ErrCode;

//                                        // Alter - Anthony - 20220521
//                                        //if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
//                                        switch (JCode)
//                                        {
//                                            case "1LIF4":
//                                            case "1LPF4":
//                                            case "1LGF4":
//                                            case "1LIS4":
//                                            case "1LPS4":
//                                            case "1LMS4":
//                                            case "1CIB4":
//                                            case "1CPB4":
//                                            case "1CGB4":
//                                            case "1CPJ4":
//                                            case "1CGJ4":
//                                            case "1CMU4":
//                                            case "1CGG4":
//                                            case "1CGS4":

//                                            case "1LIF6":
//                                            case "1LPF6":
//                                            case "1LGF6":
//                                            case "1LIS6":
//                                            case "1LPS6":
//                                            case "1LMS6":
//                                            case "1CIB6":
//                                            case "1CPB6":
//                                            case "1CGB6":
//                                            case "1CPJ6":
//                                            case "1CGJ6":
//                                            case "1CMU6":
//                                            case "1CGG6":
//                                            case "1CGS6":
//                                                if (rowJTD["RetrieveAmt"].ToString() == "HOP")
//                                                {
//                                                    // Get HOPNO
//                                                    string HOPNO, HOPBranchName;

//                                                    HOPNO = "";
//                                                    HOPBranchName = "";

//                                                    // Get HOPNO
//                                                    HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "HopNo",
//                                                        "tblBranch",
//                                                        "BranchCode='" + GLBranchCode + "'").ToString();

//                                                    // Get HOPBranchName
//                                                    HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "Name",
//                                                        "ACFGLMH",
//                                                        "ACC='" + HOPNO + "'").ToString();

//                                                    GL_Account = HOPNO;
//                                                    GL_Description = HOPBranchName;
//                                                }
//                                                break;
//                                        }


//                                        int msg1 = 1;
//                                        if (GL_Amount != 0)
//                                        {
//                                            RA_SPUpdateGLTrans(
//                                           strTransNo,
//                                           GL_SCode,
//                                           GL_Voucher,
//                                           GL_Counter,
//                                           GL_EntryDate,
//                                           GL_TransDate,
//                                           GL_Reverse,
//                                           GL_Dept,
//                                           GL_Account,
//                                           GL_Ccy,
//                                           GL_Amount,
//                                           GL_Description,
//                                           GL_LeaseNo,
//                                           GL_DocNo,
//                                           GL_TransferToGL,
//                                           GL_ErrCode,
//                                           GL_OriTransDate,
//                                           GLBranchCode,
//                                           GLCashBasis,
//                                           ref msg1);
//                                        }


//                                        if (msg1 == 0)
//                                        {
//                                            //WebMsgBox.Show("Update LeGLTrans Failed!");
//                                        }

//                                        if (sCcy != sBaseCcy)
//                                        {
//                                            //CreateEqvJournal
//                                            cAmount = vAmt;
//                                            cExchRate = GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"]));
//                                            bCounter++;

//                                            GLCashBasis = sAccCashBasis;
//                                            GLBranchCode = strBranchCode;

//                                            GL_SCode = SCode;
//                                            GL_Voucher = VchNo;
//                                            GL_Counter = bCounter;
//                                            GL_EntryDate = DateTime.Today;
//                                            GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                            GL_Reverse = false;
//                                            GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Loan", "LNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Loan", "LNo='" + vLeNo + "'").ToString();
//                                            GL_Account = sAccNo;
//                                            GL_Ccy = sBaseCcy;
//                                            GL_Amount = cAmount * cExchRate;
//                                            GL_Description = rowJTD["Description"].ToString();
//                                            GL_LeaseNo = vLeNo;
//                                            //GL_DocNo = rowTrans["TransCode"].ToString();
//                                            GL_DocNo = strSCode;
//                                            GL_TransferToGL = false;
//                                            GL_ErrCode = ErrCode;
//                                            GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);

//                                            if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
//                                            {
//                                                // Get HOPNO
//                                                string HOPNO, HOPBranchName;

//                                                HOPNO = "";
//                                                HOPBranchName = "";

//                                                // Get HOPNO
//                                                HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                    "HopNo",
//                                                    "tblBranch",
//                                                    "BranchCode='" + GLBranchCode + "'").ToString();

//                                                // Get HOPBranchName
//                                                HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                    "Name",
//                                                    "ACFGLMH",
//                                                    "ACC='" + HOPNO + "'").ToString();

//                                                GL_Account = HOPNO;
//                                                GL_Description = HOPBranchName;
//                                            }
//                                            int msg2 = 1;
//                                            RA_SPUpdateGLTrans(
//                                                strTransNo,
//                                                GL_SCode,
//                                                GL_Voucher,
//                                                GL_Counter,
//                                                GL_EntryDate,
//                                                GL_TransDate,
//                                                GL_Reverse,
//                                                GL_Dept,
//                                                GL_Account,
//                                                GL_Ccy,
//                                                GL_Amount,
//                                                GL_Description,
//                                                GL_LeaseNo,
//                                                GL_DocNo,
//                                                GL_TransferToGL,
//                                                GL_ErrCode,
//                                                GL_OriTransDate,
//                                                GLBranchCode,
//                                                GLCashBasis,
//                                                ref msg2);
//                                            if (msg2 == 0)
//                                            {
//                                                //WebMsgBox.Show("Update LeGLTrans Failed!");
//                                            }
//                                        }

//                                        if (vAmt < 0)
//                                            TotCr = TotCr + vAmt;
//                                        else
//                                            TotDb = TotDb + vAmt;

//                                        bCounter++;
//                                        X++;
//                                    }
//                                    int isReverse = 1;
//                                    switch (strSCode)
//                                    {
//                                        //Reverse Disburst
//                                        case "L006":
//                                            isReverse = -1;
//                                            break;
//                                    }
//                                    if (strSCode == "L006" || strSCode == "L001")
//                                    {
//                                        var fees = IDS.LeaseTrans.FeesDetail.GetFeesDetails(ContractNo);
//                                        var le = IDS.LeaseTrans.Lease.GetLease(ContractNo);
//                                        var mstFee = IDS.GeneralTable.Fees.GetFees();
//                                        if (fees.Count() > 0)
//                                        {
//                                            foreach (var fee in fees)
//                                            {
//                                                if (fee.FeesType == 6)
//                                                {
//                                                    IDSTools.UpdateCustomerDepositLease(fee.LeaseNo, fee.InAmount);
//                                                }
//                                                int msg1 = 1;
//                                                GL_SCode = SCode;
//                                                GL_Voucher = VchNo;
//                                                GL_Counter = X;
//                                                GL_EntryDate = DateTime.Today.Date;
//                                                GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_Reverse = false;
//                                                GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccPayable;
//                                                GL_Ccy = "IDR";
//                                                GL_Description = "Payable To " + mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).FeeName + " " + vLeNo;
//                                                GL_LeaseNo = vLeNo;
//                                                //GL_DocNo = rowTrans["TransCode"].ToString();
//                                                GL_DocNo = strSCode;
//                                                GL_TransferToGL = false;
//                                                GL_ErrCode = ErrCode;
//                                                GLBranchCode = "KPNO";
//                                                GLCashBasis = "";
//                                                GL_Amount = fee.ThirdPartyAmt;

//                                                GL_LeaseNo = ContractNo;

//                                                if (GL_Amount != 0)
//                                                {
//                                                    RA_SPUpdateGLTrans(
//                                                    strTransNo,
//                                                    GL_SCode,
//                                                    GL_Voucher,
//                                                    GL_Counter,
//                                                    GL_EntryDate,
//                                                    GL_TransDate,
//                                                    GL_Reverse,
//                                                    GL_Dept,
//                                                    GL_Account,
//                                                    GL_Ccy,
//                                                    GL_Amount * -1 * isReverse,
//                                                    GL_Description,
//                                                    GL_LeaseNo,
//                                                    GL_DocNo,
//                                                    GL_TransferToGL,
//                                                    GL_ErrCode,
//                                                    GL_OriTransDate,
//                                                    GLBranchCode,
//                                                    GLCashBasis,
//                                                    ref msg1);
//                                                    X++;
//                                                }

//                                                GL_SCode = SCode;
//                                                GL_Voucher = VchNo;
//                                                GL_Counter = X;
//                                                GL_EntryDate = DateTime.Today.Date;
//                                                GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_Reverse = false;
//                                                if ((int)le.FinanceMethod == 1)
//                                                {
//                                                    if ((int)le.LeaseType == 2)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVSaleLeaseback;
//                                                    }
//                                                    else if ((int)le.LeaseType == 3)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVInstallment;
//                                                    }
//                                                    else
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVFinanceLease;
//                                                    }
//                                                }
//                                                else if ((int)le.FinanceMethod == 2)
//                                                {
//                                                    if ((int)le.LeaseType == 4)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMKModalUsaha;
//                                                    }
//                                                    else
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMKSaleLeaseback;

//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    if ((int)le.LeaseType == 3)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGInstallment;
//                                                    }
//                                                    else if ((int)le.LeaseType == 5)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGFasilitasDana;
//                                                    }
//                                                    else if ((int)le.LeaseType == 1)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGFinanceLease;
//                                                    }
//                                                }
//                                                GL_Ccy = "IDR";
//                                                GL_Description = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).Description + " " + vLeNo;
//                                                GL_LeaseNo = vLeNo;
//                                                //GL_DocNo = rowTrans["TransCode"].ToString();
//                                                GL_DocNo = strSCode;
//                                                GL_TransferToGL = false;
//                                                GL_ErrCode = ErrCode;
//                                                GLBranchCode = "KPNO";
//                                                GLCashBasis = "";
//                                                GL_Amount = fee.InAmount - fee.ThirdPartyAmt;

//                                                GL_LeaseNo = ContractNo;

//                                                if (GL_Amount != 0)
//                                                {
//                                                    RA_SPUpdateGLTrans(
//                                                    strTransNo,
//                                                    GL_SCode,
//                                                    GL_Voucher,
//                                                    GL_Counter,
//                                                    GL_EntryDate,
//                                                    GL_TransDate,
//                                                    GL_Reverse,
//                                                    GL_Dept,
//                                                    GL_Account,
//                                                    GL_Ccy,
//                                                    GL_Amount * -1 * isReverse,
//                                                    GL_Description,
//                                                    GL_LeaseNo,
//                                                    GL_DocNo,
//                                                    GL_TransferToGL,
//                                                    GL_ErrCode,
//                                                    GL_OriTransDate,
//                                                    GLBranchCode,
//                                                    GLCashBasis,
//                                                    ref msg1);
//                                                    X++;
//                                                }
//                                            }
//                                        }
//                                    }


//                                }
//                                break;
//                        }
//                    }
//                    trans.Commit();
//                    message = 1;
//                }
//                catch (Exception ex)
//                {
//                    message = 0;
//                    trans.Rollback();
//                    //WebMsgBox.Show(ex.Message);
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }
//        //End Jeremi

//        public void CreateGLTfTransNew(
//        string strTransNo,
//        string strBranchCode,
//        string strSCode,
//        long NoProcess,
//        DateTime Period,
//        string strVoucher,
//        ref int message
//    )
//        {
//            CreateGLTfTransNew(
//                strTransNo,
//                strBranchCode,
//                strSCode,
//                NoProcess,
//                Period,
//                strVoucher,
//                ref message,
//                0
//            );
//        }

//        public void CreateGLTfTransNew(
//            string strTransNo,
//            string strBranchCode,
//            string strSCode,
//            long NoProcess,
//            DateTime Period,
//            string strVoucher,
//            ref int message,
//            int xPeriod
//        )
//        {
//            string VchNo, SCode, JCode, vLeNo, sAccNo;
//            string sAccCashBasis;
//            string sCcy, sTCurrCode;
//            Byte X, I, bCounter;
//            long Done;
//            int DbCr, ErrCode, GLSeqNo = 0;
//            double Portion = 0;
//            Byte nPeriod;
//            double TotDb, TotCr, vAmt, cExchRateJournal;
//            double cExchRateVoucher, cAmount, cExchRate;
//            DateTime CSLTransDate;
//            string sDesc;
//            DateTime StopAccrueDate;
//            int Action = 0;
//            string sBaseCcy;
//            DataTable dtJTD;
//            DataTable dtGLTrans;
//            DataTable dtLease;

//            //Variable For GLTrans
//            string GL_SCode, GL_Voucher, GL_Account, GL_Ccy;
//            string GL_Dept = "", GL_Description, GL_LeaseNo, GL_DocNo;
//            byte GL_Counter;
//            DateTime GL_EntryDate, GL_OriTransDate, GL_TransDate;
//            bool GL_Reverse, GL_TransferToGL;
//            double GL_Amount;
//            int GL_ErrCode;
//            string GLBranchCode, GLCashBasis;

//            string ContractNo = "";
//            string CustNo = "";
//            string CustBranch = "";
//            int GrpNo = 0;
//            int GroupType = 0;
//            string Tkey = "";

//            sBaseCcy = IDS.GeneralTable.Syspar.GetInstance().BaseCCy;

//            // Get Contract No 
//            ContractNo = GetFieldValue(
//                 "EntityNo",
//                 "LeTrans",
//                 "TransNo='" + strTransNo + "'"
//                ).ToString();

//            // Get Subentity No 
//            Tkey = GetFieldValue(
//                 "SubEntityNo",
//                 "LeTrans",
//                 "TransNo='" + strTransNo + "'"
//                ).ToString();

//            // Get Customer No
//            CustNo = GetFieldValue(
//                 "LesseeNo",
//                 "Lease",
//                 "LeaseNo='" + ContractNo + "'"
//                ).ToString();

//            // Get Customer Branch
//            CustBranch = GetFieldValue(
//                 "LesseeBranch",
//                 "Lease",
//                 "LeaseNo='" + ContractNo + "'"
//                ).ToString();

//            // Get Customer Group No
//            GrpNo = Convert.ToInt32(GetFieldValue(
//                 "isnull(GrpNo,0)",
//                 "Lessee",
//                 "LesseeNo='" + CustNo + "' And BranchCode='" + CustBranch + "'"
//                ));

//            // Get Customer Group Type
//            if (GrpNo == 0)
//            {
//                GroupType = 0;
//            }
//            else
//            {
//                GroupType = IDS.Tool.GeneralHelper.NullToInt((GetFieldValue(
//                     "isnull(Category,0)",
//                     "tblGroup",
//                     "GrpNo=" + GrpNo + ""
//                    )),0);

//                if (GroupType == 2)
//                {
//                    GroupType = 0;
//                }
//            }

//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                SqlTransaction trans = null;
//                trans = conn.BeginTransaction();
//                clsModule mdl = new clsModule();
//                SqlCommand cmd = new SqlCommand();
//                cmd.Connection = conn;
//                cmd.Transaction = trans;
//                cmd.CommandType = CommandType.StoredProcedure;
//                try
//                {
//                    string strSQL;
//                    strSQL = "Le_SPpruCreateGLTfTransNewPrs '" + Period + "','" + strVoucher + "','" + strTransNo + "'";
//                    SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
//                    da.SelectCommand.Transaction = trans;
//                    DataTable dtTrans = new DataTable();
//                    da.Fill(dtTrans);
//                    NoProcess = NoProcess + dtTrans.Rows.Count;
//                    if (dtTrans.Rows.Count == 0)
//                    {
                        
//                       // WebMsgBox.Show("No Transaction To Be Created!");
//                        return;
//                    }

//                    ErrCode = 0;
//                    Done = 0;
//                    for (int i = 0; i < dtTrans.Rows.Count; i++)
//                    {
//                        DataRow rowTrans = dtTrans.Rows[i];
//                        Done = Done + 1;
//                        if (mdl.Left(rowTrans["JCode"].ToString(), 2) != "LT")
//                            JCode = rowTrans["JCode"].ToString(); //mdl.Left(rowTrans["JCode"].ToString(), 1) + "1" + mdl.Right(rowTrans["JCode"].ToString(), 2);
//                        else
//                            JCode = rowTrans["JCode"].ToString();

//                        sTCurrCode = rowTrans["CurrCode"].ToString();

//                        string Vch;
//                        string strKondisi;

//                        strKondisi = "SCode='" + JCode + "' AND YEAR(TransDate)='" + Period.ToString("yyyy") + "' AND BranchCode='" + strBranchCode + "'";
//                       // "' AND MONTH(EntryDate)='" + Period.ToString("MM") +
//                        Vch = GetMaxFieldValue("right(Voucher,5)", "LeGLTrans", strKondisi).ToString();
//                        if (!string.IsNullOrEmpty(Vch) && Vch != "0")
//                        {
//                            GLSeqNo = Convert.ToInt32(Vch.Substring(Vch.Length - 5));
//                        }
//                        else
//                        {
//                            GLSeqNo = 0;
//                        }

//                        switch (JCode)
//                        {
//                            // New Contract 
//                            // Leasing Normal

//                            // Alter by Anthony (Disburse) - Perubahan kode disesuaikan dengan kontrak OJK
//                            //case "1L1":
//                            //case "1L2":
//                            //case "1L3":
//                            //case "1L4":
//                            //case "1L5":
//                            //case "1L6":

//                            case "1LIF1": // Investasi - Finance Lease - Normal
//                            case "1LPF1": // Proyek - Finance Lease - Normal
//                            case "1LGF1": // Multiguna - Finance Lease - Normal
//                            case "1LIS1": // Investasi - Sale & Leaseback - Normal
//                            case "1LPS1": // Proyek - Sale & Leaseback - Normal
//                            case "1LMS1": // Modal Kerja - Sale & Leaseback - Normal
//                            case "1SIF1":
//                            // Leasing Receivable Assignment
//                            case "1LIF4": // Investasi - Finance Lease - Receivable Assignment
//                            case "1LPF4": // Proyek - Finance Lease - Receivable Assignment
//                            case "1LGF4": // Multiguna - Finance Lease - Receivable Assignment
//                            case "1LIS4": // Investasi - Sale & Leaseback - Receivable Assignment
//                            case "1LPS4": // Proyek - Sale & Leaseback - Receivable Assignment
//                            case "1LMS4": // Modal Kerja - Sale & Leaseback - Receivable Assignment

//                            //case "1C1":
//                            //case "1C2":
//                            //case "1C3":
//                            //case "1C4":
//                            //case "1C5":
//                            //case "1C6":

//                            // CF Normal
//                            case "1CIB1": // Investasi - Installment Barang - Normal
//                            case "1CPB1": // Proyek - Installment Barang - Normal
//                            case "1CGB1": // Multiguna - Installment Barang - Normal
//                            case "1CPJ1": // Proyek - Installment Jasa - Normal
//                            case "1CGJ1": // Multiguna - Installment Jasa - Normal
//                            case "1CMU1": // Modal Kerja - Modal Usaha - Normal
//                            case "1CGG1": // Multiguna - Fasilitas Dana - Barang - Normal
//                            case "1CGS1": // Multiguna - Fasilitas Dana - Jasa - Normal

//                            // CF Receivable Assigment
//                            case "1CIB4": // Investasi - Installment Barang - Receivable Assigment
//                            case "1CPB4": // Proyek - Installment Barang - Receivable Assigment
//                            case "1CGB4": // Multiguna - Installment Barang - Receivable Assigment
//                            case "1CPJ4": // Proyek - Installment Jasa - Receivable Assigment
//                            case "1CGJ4": // Multiguna - Installment Jasa - Receivable Assigment
//                            case "1CMU4": // Modal Kerja - Modal Usaha - Receivable Assigment
//                            case "1CGG4": // Multiguna - Fasilitas Dana - Barang - Receivable Assignment
//                            case "1CGS4": // Multiguna - Fasilitas Dana - Jasa - Receivable Assignment
//                                          // End of alter by Anthony (Disburse)




//                            // SLB
//                            //case "1S1":
//                            //case "1S2":
//                            //case "1S3":
//                            //case "1S4":
//                            //case "1S5":

//                            // Alter by Anthony - 20151229 - OJK
//                            // Partial Terminate
//                            //// L
//                            //case "2L1":
//                            //case "2L2":
//                            //case "2L3":
//                            //case "2L4":
//                            //case "2L5":
//                            //case "2L6":

//                            //// CF
//                            //case "2C1":
//                            //case "2C2":
//                            //case "2C3":
//                            //case "2C4":
//                            //case "2C5":
//                            //case "2C6":
//                            // L
//                            case "2LIF1":
//                            case "2LPF1":
//                            case "2LGF1":
//                            case "2LIS1":
//                            case "2LPS1":
//                            case "2LMS1":

//                            // CF
//                            case "2CIB1":
//                            case "2CPB1":
//                            case "2CGB1":
//                            case "2CPJ1":
//                            case "2CMJ1":
//                            case "2CMU1":
//                            case "2CGG1":
//                            case "2CGS1":

//                            //Receivable Assignment
//                            // L
//                            case "2LIF4":
//                            case "2LPF4":
//                            case "2LGF4":
//                            case "2LIS4":
//                            case "2LPS4":
//                            case "2LMS4":

//                            // CF
//                            case "2CIB4":
//                            case "2CPB4":
//                            case "2CGB4":
//                            case "2CPJ4":
//                            case "2CMJ4":
//                            case "2CMU4":
//                            case "2CGG4":
//                            case "2CGS4":
//                            // End of alter by Anthony - OJK

//                            // SLB
//                            //case "2S1":
//                            //case "2S2":
//                            //case "2S3":
//                            //case "2S4":
//                            //case "2S5":

//                            // Alter by Anthony (Full Terminate) - OJK
//                            //// Full Terminate
//                            //// L
//                            //case "3L1":
//                            //case "3L2":
//                            //case "3L3":
//                            //case "3L4":
//                            //case "3L5":
//                            //case "3L6":

//                            case "3LIF1":
//                            case "3LPF1":
//                            case "3LGF1":
//                            case "3LIS1":
//                            case "3LPS1":
//                            case "3LMS1":
//                            case "3LGB1":

//                            //// CF
//                            //case "3C1":
//                            //case "3C2":
//                            //case "3C3":
//                            //case "3C4":
//                            //case "3C5":
//                            //case "3C6":

//                            case "3CIB1":
//                            case "3CPB1":
//                            case "3CGB1":
//                            case "3CPJ1":
//                            case "3CGJ1":
//                            case "3CMU1":
//                            case "3CGG1":
//                            case "3CGS1":
//                            // End of alter by Anthony (Full Terminate)

//                            // SLB
//                            //case "3S1":
//                            //case "3S2":
//                            //case "3S3":
//                            //case "3S4":
//                            //case "3S5":


//                            // Alter by Anthony (Floating Interest) - OJK
//                            // Floating Interest
//                            // L
//                            //case "4L1":
//                            //case "4L2":
//                            //case "4L3":
//                            //case "4L4":
//                            //case "4L5":
//                            //case "4L6":

//                            case "4LIF1":
//                            case "4LPF1":
//                            case "4LGF1":
//                            case "4LIS1":
//                            case "4LPS1":
//                            case "4LMS1":

//                            // CF
//                            //case "4C1":
//                            //case "4C2":
//                            //case "4C3":
//                            //case "4C4":
//                            //case "4C5":
//                            //case "4C6":

//                            case "4CIB1":
//                            case "4CPB1":
//                            case "4CGB1":
//                            case "4CPJ1":
//                            case "4CGJ1":
//                            case "4CMU1":
//                            case "4CGG1":
//                            case "4CGS1":
//                            // End of alter by Anthony (Floating)

//                            // SLB
//                            //case "4S1":
//                            //case "4S2":
//                            //case "4S3":
//                            //case "4S4":
//                            //case "4S5":


//                            // Alter by Anthony (Rescheduling) - Perubahan kode disesuaikan dengan kontrak OJK
//                            // Rescheduling
//                            // L
//                            //case "21L1":
//                            //case "21L2":
//                            //case "21L3":
//                            //case "21L4":
//                            //case "21L5":
//                            //case "21L6":

//                            case "21LIF1":
//                            case "21LPF1":
//                            case "21LGF1":
//                            case "21LIS1":
//                            case "21LPS1":
//                            case "21LMS1":
//                            case "29CGB1":
//                            case "29LMU1":
//                            case "29CGG1":
//                            case "29CGJ1":
//                            case "29CGS1":
//                            case "29LGF1":
//                            case "29LGB1":
//                            case "29CIB1":
//                            case "29CIS1":
//                            case "29LIB1":
//                            case "29LIF1":
//                            case "29LIS1":
//                            case "29LMS1":


//                            // CF
//                            //case "21C1":
//                            //case "21C2":
//                            //case "21C3":
//                            //case "21C4":
//                            //case "21C5":
//                            //case "21C6":

//                            case "21CIB1":
//                            case "21CPB1":
//                            case "21CGB1":
//                            case "21CPJ1":
//                            case "21CGJ1":
//                            case "21CMU1":
//                            case "21CGG1":
//                            case "21CGS1":
//                            // End of alter by Anthony (rescheduling)

//                            // ALter by bintang
//                            case "30LIF1":
//                            case "30LPF1":
//                            case "30LGF1":
//                            case "30LIS1":
//                            case "30LPS1":
//                            case "30LMS1":
//                            case "30CGB1":
//                            case "30LMU1":
//                            case "30CGG1":
//                            case "30CGJ1":
//                            case "30CGS1":
//                            case "30LGB1":
//                            case "30CIB1":
//                            case "30CIS1":
//                            case "30LIB1":
//                            case "21LMU1":
//                            case "21LGB1":
//                            case "21CIS1":
//                            case "21LIB1":

//                            //end alter by bintang

//                            // Contract Interest Accrued
//                            // L
//                            case "24L4":

//                            // CF
//                            case "24C4":

//                                // SLB
//                                //case "21S1":
//                                //case "21S2":
//                                //case "21S3":
//                                //case "21S4":
//                                //case "21S5":

//                                X = 1;
//                                bCounter = 1;
//                                vLeNo = rowTrans["EntityNo"].ToString();
//                                //int PeriodFloating = 0;
//                                //PeriodFloating= Convert.ToInt16(rowTrans["SubEntityNo"]);
//                                int PeriodFloating = 0;
//                                object subEntityNo = rowTrans["SubEntityNo"];
//                                if (subEntityNo != null && subEntityNo != DBNull.Value)
//                                {
//                                    if (subEntityNo is int)
//                                    {
//                                        PeriodFloating = (int)subEntityNo; // Jika subEntityNo adalah int, ambil nilainya
//                                    }
//                                    else
//                                    {
//                                        // Jika subEntityNo bukan int, coba konversi ke int
//                                        if (int.TryParse(subEntityNo.ToString(), out int result))
//                                        {
//                                            PeriodFloating = result; // Jika berhasil dikonversi, ambil nilai hasil konversi
//                                        }
//                                        // Jika tidak berhasil dikonversi, periodFloating tetap 0 (nilai default)
//                                    }
//                                }
//                                //PeriodFloating = Convert.ToInt16(rowTrans["SubEntityNo"]);
//                                strSQL = "SELECT * FROM LeJournalTD where JCode='" + JCode + "' and TCurrCode='" + sTCurrCode + "' and GroupType=" + GroupType + " ORDER BY Counter ";
//                                var custname = IDSTools.GetCustNameFromLeaseNo(ContractNo);
//                                //strSQL += "and Counter=" + X;
//                                da = new SqlDataAdapter(strSQL, conn);
//                                da.SelectCommand.Transaction = trans;
//                                dtJTD = new DataTable();
//                                da.Fill(dtJTD);

//                                if (dtJTD.Rows.Count == 0)
//                                {
//                                    //WebMsgBox.Show("Seek Failure in Journal for " + JCode);
//                                }
//                                else
//                                {
//                                    TotDb = 0;
//                                    TotCr = 0;

//                                    SCode = rowTrans["JCode"].ToString();
//                                    GLSeqNo = GLSeqNo + 1;
//                                    //VchNo = DateTime.Today.ToString("yyMM") + GLSeqNo.ToString("000");
//                                    VchNo = Period.ToString("yyMM") + GLSeqNo.ToString("00000");


//                                    for (int c = 0; c < dtJTD.Rows.Count; c++)
//                                    {
//                                        DataRow rowJTD = dtJTD.Rows[c];
//                                        ErrCode = 0;
//                                        DbCr = 1;
//                                        vAmt = 0;

//                                        if (Convert.ToInt16(rowJTD["RetrieveType"]) == 1)
//                                        {
//                                            if (!string.IsNullOrEmpty(rowTrans["OuterCode"].ToString()))
//                                            {
//                                                sAccNo = GetFieldValue("GelAcc", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                                sCcy = rowJTD["CurrCode"].ToString();
//                                                sDesc = GetFieldValue("BankName", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString() + " atas nama " + custname;
//                                                sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            }
//                                            else
//                                            {
//                                                sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                                sAccNo = rowJTD["Acc"].ToString();
//                                                sCcy = rowJTD["CurrCode"].ToString();
//                                                sDesc = rowJTD["Description"].ToString() + " atas nama " + custname;
//                                            }

//                                        }
//                                        else
//                                        {
//                                            sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            sAccNo = rowJTD["Acc"].ToString();
//                                            sCcy = rowJTD["CurrCode"].ToString();
//                                            sDesc = string.IsNullOrEmpty(rowJTD["Description"].ToString()) ? "0" : rowJTD["Description"].ToString();
//                                        }

//                                        switch (strSCode)
//                                        {
//                                            //Reverse Disburst
//                                            case "L006":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            //Disburst
//                                            case "L001":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Reverse Partial 
//                                            case "L007":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            //Partial
//                                            case "L002":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Reverse Full
//                                            case "L008":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            case "L003":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Add By: Tomy
//                                            //Floating Interest
//                                            case "L004":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Rescheduling
//                                            case "L021":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Contract Interest Accrued
//                                            case "L024":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Write OFF
//                                            case "L023":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            case "L030":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;
//                                        }

//                                        GLCashBasis = sAccCashBasis;
//                                        GLBranchCode = strBranchCode;

//                                        GL_SCode = SCode;
//                                        GL_Voucher = VchNo;
//                                        GL_Counter = bCounter;
//                                        GL_EntryDate = DateTime.Today;
//                                        GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                        GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                        GL_Reverse = false;
//                                        GL_Account = sAccNo;
//                                        GL_Ccy = sCcy;
//                                        // Alter - Anthony - 20200709
//                                        //GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString();

//                                        using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
//                                        {
//                                            db.CommandText = "SELECT BranchCode, RepresentativeOffice FROM Lease WHERE LeaseNo = @leaseno";
//                                            db.CommandType = System.Data.CommandType.Text;
//                                            db.AddParameter("@leaseno", SqlDbType.VarChar, vLeNo);
//                                            db.Open();

//                                            db.ExecuteReader();

//                                            using (SqlDataReader rd = db.DbDataReader as SqlDataReader)
//                                            {
//                                                if (rd.HasRows)
//                                                {
//                                                    while (rd.Read())
//                                                    {
//                                                        if (rd["RepresentativeOffice"] == DBNull.Value || string.IsNullOrEmpty(rd["RepresentativeOffice"] as string))
//                                                            GL_Dept = rd["BranchCode"] as string;
//                                                        else
//                                                            GL_Dept = rd["RepresentativeOffice"] as string;
//                                                    }
//                                                }
//                                            }

//                                            db.Close();
//                                        }


//                                        // End alter - Anthony - 20200709

//                                        if (strSCode == "L021" || strSCode == "L024" || strSCode == "L023")
//                                        {
//                                            //GetvAmtErrCode For Approve (status=0)
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 2)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                        }
//                                        else if (strSCode == "L030")
//                                        {
//                                            //ini Vleno nya jadi TKey untuk principal
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 2)), rowJTD["RetrieveAmt"].ToString(), Tkey, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                        }
//                                        else
//                                        {
//                                            //GetvAmtErrCode For Approve (status=0)
//                                            //ini diganti doang xperiod menjadi PeriodFloating
//                                            //GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, PeriodFloating, GroupType);

//                                        }

//                                        cExchRateJournal = sCcy != sBaseCcy ? GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;
//                                        cExchRateVoucher = sTCurrCode != sBaseCcy ? GetExchangeRate(sTCurrCode, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;

//                                        if (sCcy != sTCurrCode)
//                                            vAmt = (vAmt * cExchRateVoucher / cExchRateJournal);
//                                        vAmt = vAmt * DbCr;
//                                        GL_Amount = vAmt;
//                                        GL_Description = sDesc + " " + vLeNo;
//                                        GL_LeaseNo = vLeNo;
//                                        //GL_DocNo = rowTrans["TransCode"].ToString();
//                                        GL_DocNo = strSCode;
//                                        GL_TransferToGL = false;
//                                        GL_ErrCode = ErrCode;

//                                        // Alter - Anthony - 20220521
//                                        //if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
//                                        switch (JCode)
//                                        {
//                                            case "1LIF4":
//                                            case "1LPF4":
//                                            case "1LGF4":
//                                            case "1LIS4":
//                                            case "1LPS4":
//                                            case "1LMS4":
//                                            case "1CIB4":
//                                            case "1CPB4":
//                                            case "1CGB4":
//                                            case "1CPJ4":
//                                            case "1CGJ4":
//                                            case "1CMU4":
//                                            case "1CGG4":
//                                            case "1CGS4":

//                                            case "1LIF6":
//                                            case "1LPF6":
//                                            case "1LGF6":
//                                            case "1LIS6":
//                                            case "1LPS6":
//                                            case "1LMS6":
//                                            case "1CIB6":
//                                            case "1CPB6":
//                                            case "1CGB6":
//                                            case "1CPJ6":
//                                            case "1CGJ6":
//                                            case "1CMU6":
//                                            case "1CGG6":
//                                            case "1CGS6":
//                                                if (rowJTD["RetrieveAmt"].ToString() == "HOP")
//                                                {
//                                                    // Get HOPNO
//                                                    string HOPNO, HOPBranchName;

//                                                    HOPNO = "";
//                                                    HOPBranchName = "";

//                                                    // Get HOPNO
//                                                    HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "HopNo",
//                                                        "tblBranch",
//                                                        "BranchCode='" + GLBranchCode + "'").ToString();

//                                                    // Get HOPBranchName
//                                                    HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "Name",
//                                                        "ACFGLMH",
//                                                        "ACC='" + HOPNO + "'").ToString();

//                                                    GL_Account = HOPNO;
//                                                    GL_Description = HOPBranchName;
//                                                }
//                                                break;
//                                        }


//                                        int msg1 = 1;
//                                        if(GL_Amount != 0)
//                                        {
//                                            Le_SPUpdateGLTrans(
//                                           strTransNo,
//                                           GL_SCode,
//                                           GL_Voucher,
//                                           GL_Counter,
//                                           GL_EntryDate,
//                                           GL_TransDate,
//                                           GL_Reverse,
//                                           GL_Dept,
//                                           GL_Account,
//                                           GL_Ccy,
//                                           GL_Amount,
//                                           GL_Description,
//                                           GL_LeaseNo,
//                                           GL_DocNo,
//                                           GL_TransferToGL,
//                                           GL_ErrCode,
//                                           GL_OriTransDate,
//                                           GLBranchCode,
//                                           GLCashBasis,
//                                           ref msg1);
//                                        }
                                       

//                                        if (msg1 == 0)
//                                        {
//                                            //WebMsgBox.Show("Update LeGLTrans Failed!");
//                                        }

//                                        if (sCcy != sBaseCcy)
//                                        {
//                                            //CreateEqvJournal
//                                            cAmount = vAmt;
//                                            cExchRate = GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"]));
//                                            bCounter++;

//                                            GLCashBasis = sAccCashBasis;
//                                            GLBranchCode = strBranchCode;

//                                            GL_SCode = SCode;
//                                            GL_Voucher = VchNo;
//                                            GL_Counter = bCounter;
//                                            GL_EntryDate = DateTime.Today;
//                                            GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                            GL_Reverse = false;
//                                            GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString();
//                                            GL_Account = sAccNo;
//                                            GL_Ccy = sBaseCcy;
//                                            GL_Amount = cAmount * cExchRate;
//                                            GL_Description = rowJTD["Description"].ToString();
//                                            GL_LeaseNo = vLeNo;
//                                            //GL_DocNo = rowTrans["TransCode"].ToString();
//                                            GL_DocNo = strSCode;
//                                            GL_TransferToGL = false;
//                                            GL_ErrCode = ErrCode;
//                                            GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);

//                                            if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
//                                            {
//                                                // Get HOPNO
//                                                string HOPNO, HOPBranchName;

//                                                HOPNO = "";
//                                                HOPBranchName = "";

//                                                // Get HOPNO
//                                                HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                    "HopNo",
//                                                    "tblBranch",
//                                                    "BranchCode='" + GLBranchCode + "'").ToString();

//                                                // Get HOPBranchName
//                                                HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                    "Name",
//                                                    "ACFGLMH",
//                                                    "ACC='" + HOPNO + "'").ToString();

//                                                GL_Account = HOPNO;
//                                                GL_Description = HOPBranchName;
//                                            }
//                                            int msg2 = 1;
//                                            Le_SPUpdateGLTrans(
//                                                strTransNo,
//                                                GL_SCode,
//                                                GL_Voucher,
//                                                GL_Counter,
//                                                GL_EntryDate,
//                                                GL_TransDate,
//                                                GL_Reverse,
//                                                GL_Dept,
//                                                GL_Account,
//                                                GL_Ccy,
//                                                GL_Amount,
//                                                GL_Description,
//                                                GL_LeaseNo,
//                                                GL_DocNo,
//                                                GL_TransferToGL,
//                                                GL_ErrCode,
//                                                GL_OriTransDate,
//                                                GLBranchCode,
//                                                GLCashBasis,
//                                                ref msg2);
//                                            if (msg2 == 0)
//                                            {
//                                                //WebMsgBox.Show("Update LeGLTrans Failed!");
//                                            }
//                                        }

//                                        if (vAmt < 0)
//                                            TotCr = TotCr + vAmt;
//                                        else
//                                            TotDb = TotDb + vAmt;

//                                        bCounter++;
//                                        X++;
//                                    }
//                                    int isReverse = 1;
//                                    switch (strSCode)
//                                    {
//                                        //Reverse Disburst
//                                        case "L006":
//                                                isReverse = -1;
//                                            break;
//                                    }
//                                    if(strSCode == "L006" || strSCode == "L001")
//                                    {
//                                        var fees = IDS.LeaseTrans.FeesDetail.GetFeesDetails(ContractNo);
//                                        var le = IDS.LeaseTrans.Lease.GetLease(ContractNo);
//                                        var mstFee = IDS.GeneralTable.Fees.GetFees();
//                                        if (fees.Count() > 0)
//                                        {
//                                            foreach (var fee in fees)
//                                            {
//                                                if(fee.FeesType == 6)
//                                                {
//                                                    IDSTools.UpdateCustomerDepositLease(fee.LeaseNo, fee.InAmount);
//                                                }
//                                                int msg1 = 1;
//                                                GL_SCode = SCode;
//                                                GL_Voucher = VchNo;
//                                                GL_Counter = X;
//                                                GL_EntryDate = DateTime.Today.Date;
//                                                GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_Reverse = false;
//                                                GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccPayable;
//                                                GL_Ccy = "IDR";
//                                                GL_Description = "Payable To " + mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).FeeName + " " + vLeNo;
//                                                GL_LeaseNo = vLeNo;
//                                                //GL_DocNo = rowTrans["TransCode"].ToString();
//                                                GL_DocNo = strSCode;
//                                                GL_TransferToGL = false;
//                                                GL_ErrCode = ErrCode;
//                                                GLBranchCode = "IDS";
//                                                GLCashBasis = "";
//                                                GL_Amount = fee.ThirdPartyAmt;

//                                                GL_LeaseNo = ContractNo;

//                                                if (GL_Amount != 0)
//                                                {
//                                                    Le_SPUpdateGLTrans(
//                                                    strTransNo,
//                                                    GL_SCode,
//                                                    GL_Voucher,
//                                                    GL_Counter,
//                                                    GL_EntryDate,
//                                                    GL_TransDate,
//                                                    GL_Reverse,
//                                                    GL_Dept,
//                                                    GL_Account,
//                                                    GL_Ccy,
//                                                    GL_Amount * -1 * isReverse,
//                                                    GL_Description,
//                                                    GL_LeaseNo,
//                                                    GL_DocNo,
//                                                    GL_TransferToGL,
//                                                    GL_ErrCode,
//                                                    GL_OriTransDate,
//                                                    GLBranchCode,
//                                                    GLCashBasis,
//                                                    ref msg1);
//                                                    X++;
//                                                }

//                                                GL_SCode = SCode;
//                                                GL_Voucher = VchNo;
//                                                GL_Counter = X;
//                                                GL_EntryDate = DateTime.Today.Date;
//                                                GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_Reverse = false;
//                                                if ((int)le.FinanceMethod == 1)
//                                                {
//                                                    if((int)le.ProjectFinancingType == 0)
//                                                    {
//                                                        if ((int)le.LeaseType == 2)
//                                                        {
//                                                            GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVSaleLeaseback;
//                                                        }
//                                                        else if ((int)le.LeaseType == 3)
//                                                        {
//                                                            GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVInstallment;
//                                                        }
//                                                        else
//                                                        {
//                                                            GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVFinanceLease;
//                                                        }
//                                                    }
//                                                    else if((int)le.ProjectFinancingType == 1)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVProject;
//                                                    }
//                                                    else
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVInfra;
//                                                    }

//                                                }
//                                                else if ((int)le.FinanceMethod == 2)
//                                                {
//                                                    if ((int)le.LeaseType == 4)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMKModalUsaha;
//                                                    }
//                                                    else
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMKSaleLeaseback;

//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    if ((int)le.LeaseType == 3)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGInstallment;
//                                                    }
//                                                    else if ((int)le.LeaseType == 5)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGFasilitasDana;
//                                                    }
//                                                    else if ((int)le.LeaseType == 1)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGFinanceLease;
//                                                    }
//                                                }
//                                                GL_Ccy = "IDR";
//                                                GL_Description = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).Description + " " + vLeNo;
//                                                GL_LeaseNo = vLeNo;
//                                                //GL_DocNo = rowTrans["TransCode"].ToString();
//                                                GL_DocNo = strSCode;
//                                                GL_TransferToGL = false;
//                                                GL_ErrCode = ErrCode;
//                                                GLBranchCode = "IDS";
//                                                GLCashBasis = "";
//                                                GL_Amount = fee.InAmount - fee.ThirdPartyAmt;

//                                                GL_LeaseNo = ContractNo;

//                                                if (GL_Amount != 0)
//                                                {
//                                                    Le_SPUpdateGLTrans(
//                                                    strTransNo,
//                                                    GL_SCode,
//                                                    GL_Voucher,
//                                                    GL_Counter,
//                                                    GL_EntryDate,
//                                                    GL_TransDate,
//                                                    GL_Reverse,
//                                                    GL_Dept,
//                                                    GL_Account,
//                                                    GL_Ccy,
//                                                    GL_Amount * -1 * isReverse,
//                                                    GL_Description,
//                                                    GL_LeaseNo,
//                                                    GL_DocNo,
//                                                    GL_TransferToGL,
//                                                    GL_ErrCode,
//                                                    GL_OriTransDate,
//                                                    GLBranchCode,
//                                                    GLCashBasis,
//                                                    ref msg1);
//                                                    X++;
//                                                }
//                                            }
//                                        }
//                                    }

                                   
//                                }
//                                break;
//                        }
//                    }
//                    trans.Commit();
//                    message = 1;
//                }
//                catch (Exception ex)
//                {
//                    message = 0;
//                    trans.Rollback();
//                    //WebMsgBox.Show(ex.Message);
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }

//        public int Le_SPUpdateLeaseOutstanding(
//        string LeaseNo,
//        string cbxBankCode,
//        int cbxDocType,
//        string Remark,
//        string UserID,
//        string LastUpdated,
//        int status,
//        ref int message)
//        {
//            int result = 1;
//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                SqlTransaction trans = null;
//                try
//                {
//                    conn.Open();
//                    SqlCommand cmd = new SqlCommand();
//                    cmd.Connection = conn;
//                    cmd.CommandType = CommandType.Text;

//                    string query = "exec ";
//                    query += "Le_SPUpdateLeaseOutstanding ";
//                    query += "@LeaseNo, @InstType, @BankCode, @Remark, @LogUserID, @LogDate, @Status ";
//                    cmd.CommandText = query;
//                    cmd.Parameters.Clear();
//                    cmd.Parameters.Add("@LeaseNo", SqlDbType.VarChar).Value = LeaseNo;
//                    cmd.Parameters.Add("@InstType", SqlDbType.SmallInt).Value = cbxDocType;
//                    cmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = cbxBankCode;
//                    cmd.Parameters.Add("@Remark", SqlDbType.VarChar).Value = Remark;
//                    cmd.Parameters.Add("@LogUserID", SqlDbType.VarChar).Value = UserID;
//                    cmd.Parameters.Add("@LogDate", SqlDbType.DateTime).Value = Convert.ToDateTime(LastUpdated);
//                    cmd.Parameters.Add("@Status", SqlDbType.SmallInt).Value = status;

//                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
//                    cmd.Transaction = trans;
//                    cmd.ExecuteNonQuery();
//                    trans.Commit();
//                    result = 1;
//                }
//                catch (Exception ex)
//                {
//                    result = 0;
//                    message = 0;
                  
//                    if (trans != null)
//                        trans.Rollback();
//                }
//                finally
//                {
//                    conn.Close();
//                }
//                return result;
//            }
//        }

       
//        public object GetFieldValue(
//        string strFieldName,
//        string strTableName,
//        string strCondition
//    )
//        {
//            return GetFieldValue(
//                strFieldName,
//                strTableName,
//                strCondition, null
//            );
//        }

//        public object GetFieldValue(
//            string strFieldName,
//            string strTableName,
//            string strCondition,
//            SqlTransaction trans
//        )
//        {
//            SqlConnection conn;
//            if (trans == null)
//            {
//                conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString());
//                conn.Open();
//            }
//            else
//                conn = trans.Connection;
//            object obj;

//            // Add - Anthony - 20180913
//            StringBuilder sb = new StringBuilder();
//            try
//            {
//                // Alter - Anthony - 20180913
//                //string strSQL;
//                //strSQL = "SELECT " + strFieldName + " FROM " + strTableName + " WHERE " + strCondition;
//                //SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);

//                sb.Append("SELECT ")
//                    .Append(strFieldName)
//                    .Append(" FROM ")
//                    .Append(strTableName)
//                    .Append(" WHERE ")
//                    .Append(strCondition);
//                // End alter - Anthony - 20180913

//                SqlDataAdapter da = new SqlDataAdapter(sb.ToString(), conn);
//                if (trans != null)
//                    da.SelectCommand.Transaction = trans;
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                if (dt.Rows.Count == 0)
//                    obj = "";
//                else
//                    obj = dt.Rows[0][0];
//            }
//            catch (Exception ex)
//            {
//                obj = "";
          
//            }
//            finally
//            {
//                // Add - Anthony - 20180913
//                sb.Remove(0, sb.Length);

//                if (trans == null)
//                {
//                    conn.Close();
//                    // Add by Anthony - 20160916
//                    conn.Dispose();
//                    GC.WaitForPendingFinalizers();
//                    // End of add by Anthony - 20160916
//                }
//            }
//            return obj;
//        }

//        public void GetvAmtErrCode(
//    int Status,
//    string RetrieveAmt,
//    string vLeNo,
//    DataRow rowTrans,
//    double TotDb,
//    double TotCr,
//    double Portion,
//    ref double vAmt,
//    ref int ErrCode,
//    int xPer,
//    int CustGroupType,
//    string Tkey
//)
//        {
//            string strSQL;
//            byte nPeriod;

//            double LReceivable;
//            double UnLeIncome;
//            double AdminFee;
//            double Instosupplier;
//            double Instosales;
//            double Instocust;
//            double InsurAmt;
//            double InsAmtDisc;
//            double provision;
//            double Residual;
//            double Rental;
//            double Notary;
//            double provisionFee = 0;

//            // Add by Anthony - 20160304
//            double BiSurvei = 0;
//            // End of add by Anthony - 20160304

//            double TD, TC;
//            object advarr;
//            string ContractType;

//            clsModule mdl = new clsModule();
//            ContractType = mdl.GetJCodeLeasing(1, vLeNo);

//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                try
//                {
//                    SqlDataAdapter da;
//                    DataTable dtSchedule;
//                    DataTable dtScheduleIns;
//                    DataTable dtLease;



//                    #region Disbursement
//                    // Disbursement
//                    if (Status == 1)
//                    {
//                        // Add by Yusuf(21 Mar 2011)
//                        advarr = LeaseTools.GetINSTANCE().GetFieldValue(
//                                "AdvArr",
//                                "Lease",
//                                "LeaseNo='" + vLeNo + "'"
//                            );

//                        // Jika Contract Type Join Financing
//                        // Alter - Anthony - 20220520
//                        //if (ContractType == "1L4" || ContractType == "1C4")

//                        if (ContractType == "1CIB4" // CF Investasi Installment Financing Barang
//                            || ContractType == "1CGB4" // CF Multiguna Barang 
//                            || ContractType == "1CPB4" // CF Project Barang
//                            || ContractType == "1CPJ4" // CF Project Jasa
//                            || ContractType == "1CGJ4" // CF Multiguna Jasa
//                            || ContractType == "1CMU4" // CF Modal Kerja Modal Usaha
//                            || ContractType == "1CGG4" // CF Multiguna Fasilitas Dana Barang
//                            || ContractType == "1CGS4" // CF Multiguna Fasilitas Dana Jasa
//                            || ContractType == "1LIF4" // Leasing Investasi Finance Lease
//                            || ContractType == "1LPF4" // Leasing Project Finance Lease
//                            || ContractType == "1LGF4" // Leasing Multiguna Finance Lease
//                            || ContractType == "1LIS4" // Leasing Investasi Sale & Leaseback
//                            || ContractType == "1LPS4" // Leasing Project Sales & Leaseback 
//                            || ContractType == "1LMS4" // Leasing Modal Kerja Sale & Leaseback
//                            )
//                        // End alter - Anthony - 20220520
//                        {
//                            if (Convert.ToInt16(advarr) == 1)
//                            {
//                                switch (RetrieveAmt)
//                                {
//                                    case "CFC": // Porsi Perusahaan Pembiayaan
//                                                // Alter - Anthony - 20220520
//                                                //strSQL = "select S.Rental-SJF.Rental As Rental from Schedule as S ";
//                                                //strSQL += "Inner Join ScheduleJF as SJF on S.LeaseNo=SJF.LeaseNo AND S.Period=SJF.Period ";
//                                                //strSQL += "where S.LeaseNo='" + vLeNo + "' and S.Period=1";
//                                        strSQL = "SELECT ISNULL(Rental, 0) As Rental FROM SchedulePP WHERE LeaseNo='" + vLeNo + "' AND Period=1";
//                                        // End alter - Anthony - 20220520

//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);

//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        break;
//                                    // Add - Anthony - 20220520
//                                    case "CFCJF": // Rental Porsi Funder
//                                        strSQL = "SELECT ISNULL(Rental, 0) As Rental FROM ScheduleJF WHERE LeaseNo='" + vLeNo + "' AND Period=1";

//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);

//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        break;
//                                    case "CFCFL": // Rental Porsi Full
//                                        strSQL = "SELECT ISNULL(Rental, 0) As Rental FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=1";

//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);

//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        break;
//                                }
//                            }

//                            switch (RetrieveAmt)
//                            {
//                                case "CFD": // Porsi Perusahan Pembiayaan
//                                            // Alter - Anthony - 20220520
//                                            //strSQL = "select (S.LeReceivable-SJF.LeReceivable) as LeReceivable from Schedule as S ";
//                                            //strSQL += "Inner Join ScheduleJF as SJF on S.LeaseNo=SJF.LeaseNo And S.Period=SJF.Period ";
//                                            //strSQL += "where S.LeaseNo='" + vLeNo + "' and S.Period=0 And SJF.Period=0";
//                                    strSQL = "SELECT ISNULL(LeReceivable, 0) AS LeReceivable FROM SchedulePP WHERE LeaseNo='" + vLeNo + "' AND Period=0";
//                                    // End alter - Anthony - 20220520

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    break;
//                                // Add - Anthony - 20220520
//                                case "CFDJF": // Porsi Funder / JF
//                                    strSQL = "SELECT ISNULL(LeReceivable, 0) AS LeReceivable FROM ScheduleJF WHERE LeaseNo='" + vLeNo + "' AND Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    break;
//                                case "CFDFL": // Porsi Full
//                                    strSQL = "SELECT ISNULL(LeReceivable, 0) AS LeReceivable FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    break;
//                                // End add - Anthony - 20220520

//                                case "UCF": // Porsi Perusahan Pembiayaan
//                                            // Alter - Anthony - 20220520
//                                            //strSQL = "select (S.UnLeIncome-SJF.UnLeIncome) as UnLeIncome from Schedule as S ";
//                                            //strSQL += "Inner Join ScheduleJF as SJF on S.LeaseNo=SJF.LeaseNo And S.Period=SJF.Period ";
//                                            //strSQL += "where S.LeaseNo='" + vLeNo + "' and S.Period=0 And SJF.Period=0";
//                                    strSQL = "SELECT ISNULL(UnLeIncome, 0) AS UnLeIncome FROM SchedulePP WHERE LeaseNo='" + vLeNo + "' AND Period=0";
//                                    // End alter - Anthony - 20220520

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;
//                                // Add - Anthony - 20220520
//                                case "UCFJF": // Porsi Funder
//                                    strSQL = "SELECT ISNULL(UnLeIncome, 0) AS UnLeIncome FROM ScheduleJF WHERE LeaseNo='" + vLeNo + "' AND Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;
//                                case "UCFFL":
//                                    strSQL = "SELECT ISNULL(UnLeIncome, 0) AS UnLeIncome FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;
//                                // End add - Anthony - 20220520


//                                case "AFC":
//                                    strSQL = "select ISNULL(AdminFee, 0) AS AdminFee from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AdminFee"]);
//                                    break;

//                                // For Lasing & CF
//                                case "LFT":
//                                    strSQL = "select ISNULL(Notary, 0) AS Notary from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Notary"]);
//                                    break;

//                                case "IPT":
//                                    strSQL = "select (InsurAmt + InsurIntRate)-InsDisc As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["InsurAmt"]);
//                                    break;

//                                case "IPS":
//                                    strSQL = "select ISNULL(Instosupplier, 0) AS Instosupplier from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosupplier"]);
//                                    break;

//                                case "ISM":
//                                    strSQL = "select ISNULL(Instosalesman, 0) AS Instosalesman from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosalesman"]);
//                                    break;

//                                case "IPC":
//                                    strSQL = "select ISNULL(Instocust, 0) AS Instocust from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
//                                    break;

//                                case "IPD":
//                                    strSQL = "select ISNULL(Instocust, 0) AS Instocust from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
//                                    break;
//                                // Add - Anthony - 20220520
//                                case "IPDC": // Insurance Discount
//                                    strSQL = "SELECT ISNULL(InsDisc, 0) - (ISNULL(Instosupplier, 0) + ISNULL(Instosalesman, 0) + ISNULL(Instocust, 0)) AS InsDisc from Lease where LeaseNo='" + vLeNo + "'";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["InsDisc"]);
//                                    break;
//                                // End add - Anthon - 20220520
//                                case "P":
//                                    object SisaIns;
//                                    SisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                            "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
//                                            "Lease",
//                                            "LeaseNo='" + vLeNo + "'"
//                                        );

//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]) + Convert.ToDouble(SisaIns);
//                                    break;
//                                case "PAF": // Provision Fee + Admin Fee
//                                    strSQL = "SELECT (ISNULL(ProvisionFee, 0) + ISNULL(AdminFee, 0)) AS ProvisionFee FROM Lease WHERE LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);

//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["ProvisionFee"]);
//                                    break;
//                                // Add - Anthony - 20220520
//                                case "B": // Biaya Blokir
//                                    strSQL = "SELECT ISNULL(BIBlokir, 0) AS BIBlokir FROM Lease WHERE LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["BIBlokir"]);
//                                    break;
//                                case "S":
//                                    strSQL = "SELECT ISNULL(BISurvei, 0) AS BISurvei FROM Lease WHERE LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["BISurvei"]);
//                                    break;
//                                // End add - Anthony - 20220520

//                                case "HOP":
//                                case "BPH": // Branch Payable to Holding


//                                    LReceivable = 0;
//                                    UnLeIncome = 0;
//                                    AdminFee = 0;
//                                    Instosupplier = 0;
//                                    Instosales = 0;
//                                    Instocust = 0;
//                                    InsurAmt = 0;
//                                    InsAmtDisc = 0;
//                                    provision = 0;
//                                    Notary = 0;
//                                    Rental = 0;
//                                    TD = 0;
//                                    TC = 0;

//                                    object BCFSisaIns;
//                                    BCFSisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                            "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
//                                            "Lease",
//                                            "LeaseNo='" + vLeNo + "'"
//                                        );

//                                    // Biaya Asuransi
//                                    double IPT = 0;
//                                    strSQL = "SELECT ISNULL(ISNULL(InsurAmt, 0) + ISNULL(InsurIntRate, 0), 0) AS IPT FROM Lease WHERE LeaseNo = '" + vLeNo + "'";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);

//                                    if (dtSchedule.Rows.Count == 0)
//                                    {
//                                        ErrCode = 2;
//                                    }
//                                    else
//                                    {
//                                        IPT = Convert.ToDouble(dtSchedule.Rows[0]["IPT"]);
//                                    }

//                                    advarr = LeaseTools.GetINSTANCE().GetFieldValue(
//                                        "AdvArr",
//                                        "Lease",
//                                        "LeaseNo='" + vLeNo + "'"
//                                        );

//                                    if (Convert.ToInt16(advarr) == 1)
//                                    {
//                                        strSQL = "SELECT ISNULL(Rental, 0) AS Rental FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=1";

//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);

//                                        if (dtSchedule.Rows.Count == 0)
//                                        {
//                                            ErrCode = 2;
//                                        }
//                                        else
//                                        {
//                                            Rental = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        }
//                                    }

//                                    double surveyFee = 0;

//                                    strSQL = "SELECT s.LeReceivable as LeReceivable, s.UnLeIncome as UnLeIncome, l.Notary, ISNULL(l.BISurvei, 0) AS BiSurvei, ";
//                                    strSQL += "ISNULL(l.AdminFee, 0) AS AdminFee, ";
//                                    strSQL += "l.Instosupplier, l.Instosalesman, ";
//                                    strSQL += "l.Instocust, (l.InsurAmt + l.InsurIntRate)-l.Instocust As InsurAmt, l.Provision, l.ProvisionFee, l.Residual, l.InsDisc As InsDisc ";
//                                    strSQL += "FROM Lease AS l ";
//                                    strSQL += "INNER JOIN Schedule s ON s.LeaseNo = l.LeaseNo ";
//                                    strSQL += "WHERE l.LeaseNo = '" + vLeNo + "' AND s.Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);

//                                    if (dtLease.Rows.Count == 0)
//                                    {
//                                        ErrCode = 1;
//                                    }
//                                    else
//                                    {
//                                        LReceivable = Convert.ToDouble(dtLease.Rows[0]["LeReceivable"]);
//                                        UnLeIncome = Convert.ToDouble(dtLease.Rows[0]["UnLeIncome"]);
//                                        AdminFee = Convert.ToDouble(dtLease.Rows[0]["AdminFee"]);
//                                        Instosupplier = Convert.ToDouble(dtLease.Rows[0]["Instosupplier"]);
//                                        Instosales = Convert.ToDouble(dtLease.Rows[0]["Instosalesman"]);
//                                        Instocust = Convert.ToDouble(dtLease.Rows[0]["Instocust"]);
//                                        InsurAmt = (Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]) - Convert.ToDouble(dtLease.Rows[0]["InsDisc"]));
//                                        provision = Convert.ToDouble(dtLease.Rows[0]["Provision"]) + Convert.ToDouble(BCFSisaIns);
//                                        provisionFee = Convert.ToDouble(dtLease.Rows[0]["ProvisionFee"]);
//                                        Residual = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                        Notary = Convert.ToDouble(dtLease.Rows[0]["Notary"]);
//                                        InsAmtDisc = Convert.ToDouble(dtLease.Rows[0]["InsDisc"]);
//                                        surveyFee = Convert.ToDouble(dtLease.Rows[0]["BISurvei"]);
//                                    }

//                                    TD = LReceivable;
//                                    TC = UnLeIncome + AdminFee + provisionFee + surveyFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Notary + Rental;
//                                    vAmt = TD - TC;


//                                    break;
//                            }
//                        }
//                        else // Normal
//                        {
//                            //Add by Anthony - 20151201
//                            if (Convert.ToInt16(advarr) == 1)
//                            {
//                                switch (RetrieveAmt)
//                                {
//                                    case "CFC":
//                                        strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);

//                                        //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=1";
//                                        //da = new SqlDataAdapter(strSQL, conn);
//                                        //dtScheduleIns = new DataTable();
//                                        //da.Fill(dtScheduleIns);
//                                        //if (dtScheduleIns.Rows.Count == 0)
//                                        //    ErrCode = 0;
//                                        //else
//                                        //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["Rental"]);
//                                        break;
//                                    case "LRC":
//                                        if (Convert.ToInt16(advarr) == 1)
//                                        {
//                                            strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
//                                            da = new SqlDataAdapter(strSQL, conn);
//                                            dtSchedule = new DataTable();
//                                            da.Fill(dtSchedule);
//                                            if (dtSchedule.Rows.Count == 0)
//                                                ErrCode = 2;
//                                            else
//                                                vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        }
//                                        break;
//                                }
//                            }
//                            // End of add by Anthony - 20151201

//                            #region Hasil Pindahan untuk dipakai leasing dan CF - OJK - Anthony 20151202
//                            switch (RetrieveAmt)
//                            {
//                                // Leasing 
//                                case "LRD":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);

//                                    //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtScheduleIns = new DataTable();
//                                    //da.Fill(dtScheduleIns);
//                                    //if (dtScheduleIns.Rows.Count == 0)
//                                    //    ErrCode = 0;
//                                    //else
//                                    //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["LeReceivable"]);
//                                    break;

//                                case "ULI":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);


//                                    break;

//                                case "DOL":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Residual"]);
//                                    break;

//                                case "AFL":
//                                    strSQL = "select ISNULL(AdminFee, 0) AS AdminFee from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AdminFee"]);
//                                    break;

//                                case "RV":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                    break;

//                                // CF 
//                                case "CFD":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);

//                                    //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtScheduleIns = new DataTable();
//                                    //da.Fill(dtScheduleIns);
//                                    //if (dtScheduleIns.Rows.Count == 0)
//                                    //    ErrCode = 0;
//                                    //else
//                                    //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["LeReceivable"]);
//                                    break;

//                                case "UCF":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);

//                                    //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtScheduleIns = new DataTable();
//                                    //da.Fill(dtScheduleIns);
//                                    //if (dtScheduleIns.Rows.Count == 0)
//                                    //    ErrCode = 0;
//                                    //else
//                                    //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "AFC":
//                                    strSQL = "select ISNULL(AdminFee, 0) AS AdminFee from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AdminFee"]);
//                                    break;

//                                // Add by Anthony - 20160321
//                                // Provision (Pengganti Admin Fee semenjak OJK)
//                                case "APF":
//                                    strSQL = "SELECT ISNULL(ProvisionFee, 0) AS ProvisionFee FROM Lease WHERE LeaseNo = '" + vLeNo + "';";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    DataTable dtProvision = new DataTable();
//                                    da.Fill(dtProvision);

//                                    if (dtProvision.Rows.Count > 0)
//                                        vAmt = Convert.ToDouble(dtProvision.Rows[0]["ProvisionFee"]);
//                                    else
//                                        vAmt = 0;
//                                    break;
//                                // End of add by Anthony - 20160321

//                                // Add by Anthony - 20160225 - Biaya Survei
//                                case "SFC":
//                                    using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
//                                    {
//                                        db.CommandText = "SELECT ISNULL(BISurvei, 0) FROM Lease WHERE LeaseNo = @leaseNo";
//                                        db.AddParameter("@leaseNo", SqlDbType.VarChar, vLeNo);
//                                        db.CommandType = CommandType.Text;
//                                        db.Open();

//                                        object biayaSurvei = db.ExecuteScalar();

//                                        if (biayaSurvei == null)
//                                        {
//                                            ErrCode = 2;
//                                        }
//                                        else
//                                        {
//                                            vAmt = Convert.ToDouble(biayaSurvei);
//                                        }
//                                    }
//                                    break;
//                                // End of add by Anthony - 20160225

//                                // For Lasing & CF

//                                case "LFT":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Notary"]);
//                                    break;

//                                case "IPT":
//                                    //strSQL = "select ((InsurAmt - InsDisc) + InsurIntRate) - InsDisc As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
//                                    // Modify by Yusuf (21 JUN 2011)
//                                    //double NProvision=0;
//                                    //object SisaHslIns;
//                                    //SisaHslIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                    //        "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
//                                    //        "Lease",
//                                    //        "LeaseNo='" + vLeNo + "'"
//                                    //    );

//                                    //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtSchedule = new DataTable();
//                                    //da.Fill(dtSchedule);
//                                    //if (dtSchedule.Rows.Count == 0)
//                                    //    ErrCode = 2;
//                                    //else
//                                    //    NProvision = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]) + Convert.ToDouble(SisaHslIns);

//                                    strSQL = "select (InsurAmt + InsurIntRate)-InsDisc As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["InsurAmt"]);
//                                    break;

//                                case "IPS":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosupplier"]);
//                                    break;

//                                case "ISM":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosalesman"]);
//                                    break;

//                                case "IPC":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
//                                    break;

//                                case "IPD":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
//                                    break;

//                                case "P":
//                                    // Alter by Anthony - 20160427
//                                    #region OLD - Sebelum Perubahan OJK
//                                    //object SisaIns;
//                                    //SisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                    //        "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
//                                    //        "Lease",
//                                    //        "LeaseNo='" + vLeNo + "'"
//                                    //    );

//                                    //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtSchedule = new DataTable();
//                                    //da.Fill(dtSchedule);
//                                    //if (dtSchedule.Rows.Count == 0)
//                                    //    ErrCode = 2;
//                                    //else
//                                    //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]) + Convert.ToDouble(SisaIns);
//                                    // End of alter by Anthony - 20160427
//                                    #endregion

//                                    strSQL = "SELECT ISNULL(Provision, 0) AS Provision FROM Lease WHERE LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]);
//                                    // End of alter by Anthony - 20160427
//                                    break;

//                                case "CTL":
//                                    LReceivable = 0;
//                                    UnLeIncome = 0;
//                                    AdminFee = 0;
//                                    Instosupplier = 0;
//                                    Instosales = 0;
//                                    Instocust = 0;
//                                    InsurAmt = 0;
//                                    provision = 0;

//                                    InsAmtDisc = 0;
//                                    Residual = 0;
//                                    Rental = 0;
//                                    Notary = 0;

//                                    TD = 0;
//                                    TC = 0;

//                                    // Add by Anthony - 20160321
//                                    provisionFee = 0;
//                                    // End of add by Anthony - 20160321

//                                    object BLSisaIns;
//                                    BLSisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                            "(ISNULL(InsDisc, 0) -(ISNULL(Instosupplier, 0) + ISNULL(Instosalesman, 0) + ISNULL(Instocust, 0))) As TotalSisaIns",
//                                            "Lease",
//                                            "LeaseNo='" + vLeNo + "'"
//                                        );

//                                    advarr = LeaseTools.GetINSTANCE().GetFieldValue(
//                                        "AdvArr",
//                                        "Lease",
//                                        "LeaseNo='" + vLeNo + "'"
//                                        );

//                                    if (Convert.ToInt16(advarr) == 1)
//                                    {
//                                        strSQL = "";
//                                        strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                        {
//                                            ErrCode = 2;
//                                        }
//                                        else
//                                        {
//                                            Rental = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        }
//                                    }

//                                    strSQL = "";
//                                    strSQL = "select s.LeReceivable, s.UnLeIncome, l.Notary, ";
//                                    strSQL += "ISNULL(l.AdminFee, 0) AS AdminFee, l.Instosupplier, l.Instosalesman, ";
//                                    strSQL += "l.Instocust, (l.InsurAmt + l.InsurIntRate)-l.Instocust As InsurAmt, l.Provision, l.Residual, l.InsDisc As InsDisc ";
//                                    // Add by Anthony - 20160304
//                                    strSQL += ", BISurvei, ProvisionFee ";
//                                    // End of add by Anthony - 20160304
//                                    strSQL += "from Lease as l ";
//                                    strSQL += "Inner Join Schedule as s on l.LeaseNo=s.LeaseNo  ";
//                                    strSQL += "where l.LeaseNo='" + vLeNo + "' And s.Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                    {
//                                        ErrCode = 1;
//                                    }
//                                    else
//                                    {
//                                        LReceivable = Convert.ToDouble(dtLease.Rows[0]["LeReceivable"]);
//                                        UnLeIncome = Convert.ToDouble(dtLease.Rows[0]["UnLeIncome"]);
//                                        AdminFee = Convert.ToDouble(dtLease.Rows[0]["AdminFee"]);
//                                        Instosupplier = Convert.ToDouble(dtLease.Rows[0]["Instosupplier"]);
//                                        Instosales = Convert.ToDouble(dtLease.Rows[0]["Instosalesman"]);
//                                        Instocust = Convert.ToDouble(dtLease.Rows[0]["Instocust"]);
//                                        InsurAmt = (Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]) - Convert.ToDouble(dtLease.Rows[0]["InsDisc"]));
//                                        provision = Convert.ToDouble(dtLease.Rows[0]["Provision"]) + Convert.ToDouble(BLSisaIns);
//                                        Residual = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                        Notary = Convert.ToDouble(dtLease.Rows[0]["Notary"]);
//                                        InsAmtDisc = Convert.ToDouble(dtLease.Rows[0]["InsDisc"]);
//                                        BiSurvei = Convert.ToDouble(dtLease.Rows[0]["BISurvei"]);

//                                        // Add by Anthony - 20160321
//                                        provisionFee = Convert.ToDouble(dtLease.Rows[0]["ProvisionFee"]);
//                                        // End of add by Anthony - 20160321
//                                    }

//                                    TD = LReceivable + Residual;

//                                    // Alter by Anthony - 20160321
//                                    //TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Residual + Notary + BiSurvei + Rental;
//                                    TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Residual + Notary + BiSurvei + Rental + provisionFee - Convert.ToDouble(BLSisaIns);
//                                    // End of alter by Anthony - 20160321
//                                    //TC = UnLeIncome + AdminFee + InsurAmt + Notary + Rental;
//                                    vAmt = TD - TC;
//                                    break;

//                                case "CTC":
//                                    // Alter - Anthony - 20160321
//                                    //if (ContractType != "1C2") // Pakar
//                                    if (ContractType != "1CIB2"
//                                        || ContractType != "1CPB2"
//                                        || ContractType != "1CGB2"
//                                        || ContractType != "1CPJ2"
//                                        || ContractType != "1CGJ2"
//                                        || ContractType != "1CMU2"
//                                        || ContractType != "1CGG2"
//                                        || ContractType != "1CGS2") // Pakar
//                                                                    // End alter - Anthony - 20160321
//                                    {
//                                        LReceivable = 0;
//                                        UnLeIncome = 0;
//                                        AdminFee = 0;
//                                        Instosupplier = 0;
//                                        Instosales = 0;
//                                        Instocust = 0;
//                                        InsurAmt = 0;
//                                        InsAmtDisc = 0;
//                                        provision = 0;
//                                        Notary = 0;
//                                        Rental = 0;
//                                        TD = 0;
//                                        TC = 0;

//                                        // Add by Anthony - 20160321
//                                        provisionFee = 0;
//                                        // End of add by Anthony - 20160321

//                                        object BCFSisaIns;
//                                        BCFSisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                "(ISNULL(InsDisc, 0) -(ISNULL(Instosupplier, 0) + ISNULL(Instosalesman, 0) + ISNULL(Instocust, 0))) As TotalSisaIns",
//                                                "Lease",
//                                                "LeaseNo='" + vLeNo + "'"
//                                            );

//                                        advarr = LeaseTools.GetINSTANCE().GetFieldValue(
//                                            "AdvArr",
//                                            "Lease",
//                                            "LeaseNo='" + vLeNo + "'"
//                                            );

//                                        if (Convert.ToInt16(advarr) == 1)
//                                        {
//                                            strSQL = "";
//                                            strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
//                                            da = new SqlDataAdapter(strSQL, conn);
//                                            dtSchedule = new DataTable();
//                                            da.Fill(dtSchedule);
//                                            if (dtSchedule.Rows.Count == 0)
//                                            {
//                                                ErrCode = 2;
//                                            }
//                                            else
//                                            {
//                                                Rental = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                            }
//                                        }

//                                        strSQL = "";
//                                        strSQL = "select s.LeReceivable, s.UnLeIncome, l.Notary, ";
//                                        strSQL += "ISNULL(l.AdminFee, 0) AS AdminFee, l.Instosupplier, l.Instosalesman, ";
//                                        strSQL += "l.Instocust, (l.InsurAmt + l.InsurIntRate)-l.Instocust As InsurAmt, l.Provision, l.Residual, l.InsDisc As InsDisc ";
//                                        strSQL += ", BISurvei, ProvisionFee ";
//                                        strSQL += "from Lease as l ";
//                                        strSQL += "Inner Join Schedule as s on l.LeaseNo=s.LeaseNo  ";
//                                        strSQL += "where l.LeaseNo='" + vLeNo + "' And s.Period=0";

//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtLease = new DataTable();
//                                        da.Fill(dtLease);
//                                        if (dtLease.Rows.Count == 0)
//                                        {
//                                            ErrCode = 1;
//                                        }
//                                        else
//                                        {
//                                            LReceivable = Convert.ToDouble(dtLease.Rows[0]["LeReceivable"]);
//                                            UnLeIncome = Convert.ToDouble(dtLease.Rows[0]["UnLeIncome"]);
//                                            AdminFee = Convert.ToDouble(dtLease.Rows[0]["AdminFee"]);
//                                            Instosupplier = Convert.ToDouble(dtLease.Rows[0]["Instosupplier"]);
//                                            Instosales = Convert.ToDouble(dtLease.Rows[0]["Instosalesman"]);
//                                            Instocust = Convert.ToDouble(dtLease.Rows[0]["Instocust"]);
//                                            InsurAmt = (Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]) - Convert.ToDouble(dtLease.Rows[0]["InsDisc"]));
//                                            provision = Convert.ToDouble(dtLease.Rows[0]["Provision"]) + Convert.ToDouble(BCFSisaIns);
//                                            Residual = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                            Notary = Convert.ToDouble(dtLease.Rows[0]["Notary"]);
//                                            InsAmtDisc = Convert.ToDouble(dtLease.Rows[0]["InsDisc"]);
//                                            BiSurvei = Convert.ToDouble(dtLease.Rows[0]["BISurvei"]);

//                                            // Add by Anthony - 20160321
//                                            provisionFee = Convert.ToDouble(dtLease.Rows[0]["ProvisionFee"]);
//                                            // End of add by Anthony - 20160321
//                                        }

//                                        TD = LReceivable;
//                                        // Alter by Anthony - 20160321
//                                        //TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Notary + BiSurvei + Rental;
//                                        TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Notary + BiSurvei + Rental + provisionFee - Convert.ToDouble(BCFSisaIns);
//                                        // End of add by Anthony - 20160321
//                                        //TC = UnLeIncome + AdminFee + InsurAmt + Notary + Rental;
//                                        vAmt = TD - TC;
//                                    }
//                                    break;
//                                // End

//                                case "LRI":
//                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count == 0)
//                                        ErrCode = 0;
//                                    else
//                                        vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["LeReceivable"]);
//                                    break;

//                                case "LRL":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    break;

//                                case "PRL":
//                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count == 0)
//                                        ErrCode = 0;
//                                    else
//                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["OutPrinc"]);

//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = vAmt + Convert.ToDouble(dtSchedule.Rows[0]["OutPrinc"]);
//                                    break;

//                                case "UEL":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "UEI":
//                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count == 0)
//                                        ErrCode = 0;
//                                    else
//                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "AP":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["LeAmount"]) + Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]);
//                                    break;

//                                case "APL":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["LeAmount"]);
//                                    break;

//                                case "API":
//                                    strSQL = "select ((InsurAmt - InsDisc) + InsurIntRate) As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]);
//                                    break;

//                                case "OPL":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["LeAmount"]);
//                                    break;

//                                case "SD":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Security"]);
//                                    break;

//                                case "BL":
//                                    vAmt = (TotCr + TotDb) * (-1);
//                                    break;

//                                case "UNI":
//                                    vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutIncome_Int", "Lease", "LeaseNo='" + vLeNo + "'"));
//                                    break;

//                                case "UT":
//                                    vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutIncome", "Lease", "LeaseNo='" + vLeNo + "'"));
//                                    if (!string.IsNullOrEmpty(LeaseTools.GetINSTANCE().GetMaxFieldValue("Period", "dbo.Schedule", "LeaseNo='" + vLeNo + "' and Payment<>0").ToString()))
//                                        vAmt = vAmt + Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("Accrual2", "dbo.Schedule", "LeaseNo='" + vLeNo + "' and Period=" + LeaseTools.GetINSTANCE().GetMaxFieldValue("Period", "dbo.Schedule", "LeaseNo='" + vLeNo + "' and Payment<>0").ToString()));
//                                    if (Convert.ToInt16(LeaseTools.GetINSTANCE().GetFieldValue("LeStatus", "Lease", "LeaseNo='" + vLeNo + "'")) == 7)
//                                    {
//                                        //StopAccrueDate = Convert.ToDateTime(GetFieldValue("ValueDate", "Trans", "EntityNo='" + vLeNo + "' and TransCode='LL13'"));
//                                        //StopAccrueDate = Convert.ToDateTime(GetFieldValue("DueDate", "Schedule", "LeaseNo='" + vLeNo + "' and datediff(month,DueDate,'" + StopAccrueDate + "')=1"));
//                                        vAmt = vAmt + Convert.ToDouble(LeaseTools.GetINSTANCE().GetSumFieldValue("Accrual1", "Schedule", "LeaseNo='10-LS-0003183-001' and datediff(month,DueDate,'6/30/2004')>=0 and datediff(month,DueDate,'6/30/2004')<=2")) + Convert.ToDouble(LeaseTools.GetINSTANCE().GetSumFieldValue("Accrual2", "Schedule", "LeaseNo='10-LS-0003183-001' and datediff(month,DueDate,'6/30/2004')>=0 and datediff(month,DueDate,'6/30/2004')<=2"));
//                                    }
//                                    break;

//                                case "UTL":
//                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "UTI":
//                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
//                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count == 0)
//                                        ErrCode = 0;
//                                    else
//                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "RT":
//                                    vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutLR", "Lease", "LeaseNo='" + vLeNo + "'"));
//                                    break;

//                                //case "ADT":
//                                //    'Accumulated depreciation-equipment for lease
//                                //    'Operating Lease

//                                case "ARL":
//                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
//                                    break;

//                                case "AUL":
//                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
//                                    break;

//                                case "CFI":
//                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "LRT":
//                                case "UIT":
//                                case "LIT":
//                                case "OIT":
//                                case "PT":
//                                    ErrCode = 6;
//                                    break;

//                                case "OP":
//                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutPrinc"]);
//                                    break;

//                                case "UI":
//                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "OLR":
//                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    break;

//                                case "RLI":
//                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                    {
//                                        int x = 0;
//                                        for (byte I = Convert.ToByte(rowTrans["InnerCode"]); I < Convert.ToByte(rowTrans["OuterCode"]); I++)
//                                        {
//                                            vAmt = vAmt + Convert.ToDouble(dtSchedule.Rows[x]);
//                                            x++;
//                                        }
//                                    }
//                                    break;

//                                case "GL":
//                                    if (Convert.ToInt16(LeaseTools.GetINSTANCE().GetFieldValue("NovasiStatus", "dbo.Lease", "LeaseNo='" + vLeNo + "'")) == 0)
//                                        vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("GainLoss", "dbo.tTermination", "LeaseNo='" + vLeNo + "' and year(ProcessDate)='" + Convert.ToDateTime(rowTrans["ValueDate"]).ToString("yyyy") + "' and month(ProcessDate)='" + Convert.ToDateTime(rowTrans["ValueDate"]).ToString("mm") + "' and day(ProcessDate)='" + Convert.ToDateTime(rowTrans["ValueDate"]).ToString("dd") + "'"));
//                                    break;

//                                case "GLN":
//                                    vAmt = Convert.ToDouble(rowTrans["Amount"]);
//                                    vAmt = vAmt + Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutIncome_Int", "Lease", "LeaseNo='" + vLeNo + "'"));
//                                    vAmt = vAmt - Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutLR_Int", "Lease", "LeaseNo='" + vLeNo + "'"));
//                                    break;

//                                case "LRS":
//                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0) as sumLR from Schedule where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count > 0)
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["sumLR"]);

//                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0)) as sumLR from ScheduleInsur where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count > 0)
//                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["sumLR"]);
//                                    break;

//                                case "UES":
//                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0) as sumLR from Schedule where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count > 0)
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["sumLR"]);

//                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0)) as sumLR from ScheduleInsur where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count > 0)
//                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["sumLR"]);
//                                    vAmt = vAmt - Convert.ToDouble(rowTrans["Amount"]);
//                                    break;

//                                case "UER":
//                                    vAmt = Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescAfter2"].ToString()) ? "0" : rowTrans["DescAfter2"].ToString()) -
//                                         Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescBefore2"].ToString()) ? "0" : rowTrans["DescBefore2"].ToString());
//                                    break;

//                                case "LRR":
//                                    vAmt = Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescAfter3"].ToString()) ? "0" : rowTrans["DescAfter3"].ToString()) -
//                                         Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescBefore3"].ToString()) ? "0" : rowTrans["DescBefore3"].ToString());
//                                    break;

//                                case "OPR":
//                                    vAmt = Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescAfter1"].ToString()) ? "0" : rowTrans["DescAfter1"].ToString()) -
//                                         Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescBefore1"].ToString()) ? "0" : rowTrans["DescBefore1"].ToString());
//                                    break;

//                                default:
//                                    //vAmt = vAmt - Convert.ToDouble(rowTrans["Amount"]);
//                                    break;
//                            }
//                            #endregion




//                            //Add By : Tomy (20 Agust 2011)
//                            //For Receivable Assignment
//                            if (ContractType == "1L6" || ContractType == "1C6")
//                            {
//                                switch (RetrieveAmt)
//                                {
//                                    case "HOP":
//                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutPrincipal"]);
//                                        break;

//                                    case "DOL":
//                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(0);
//                                        break;

//                                    case "ULI":
//                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutIncome"]);
//                                        break;

//                                    case "LR":
//                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutLR"]);
//                                        break;

//                                    case "RV":
//                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(0);
//                                        break;
//                                }
//                            }
//                            //End Add
//                            //}
//                        }
//                    }
//                    #endregion

//                    #region Partial Termination
//                    // Partial Termination
//                    else if (Status == 2)
//                    {
//                        object period, BungaDenda, hslp;
//                        period = xPer;
//                        hslp = Convert.ToInt32(xPer - 1);

//                        BungaDenda = LeaseTools.GetINSTANCE().GetFieldValue(
//                                "CurrInt",
//                                "LetTermination",
//                                "LeaseNo='" + vLeNo + "' AND Type=0 AND Period='" + Convert.ToInt32(period) + "'"
//                            );

//                        switch (RetrieveAmt)
//                        {
//                            // Leasing
//                            case "ULI":
//                                //Commented By Marsolim 6 December 2011    
//                                //strSQL = "select abs(Adjustment) As Adjustment From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                //da = new SqlDataAdapter(strSQL, conn);
//                                //dtSchedule = new DataTable();
//                                //da.Fill(dtSchedule);
//                                //if (dtSchedule.Rows.Count == 0)
//                                //    ErrCode = 2;
//                                //else
//                                //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
//                                //End Comment
//                                //Added By Marsolim 6 Dec 2011
//                                vAmt = Math.Abs(Convert.ToDouble(rowTrans["DescAfter3"]));
//                                //End Add
//                                break;

//                            case "LRL":
//                                #region Commented
//                                //if (Convert.ToDouble(BungaDenda) == 0)
//                                //{
//                                //    //strSQL  = "SELECT (SELECT DescBefore4 From LeTrans ";
//                                //    //strSQL += "WHERE EntityNo='" + vLeNo + "' AND SubEntityNo='" + Convert.ToInt32(period) + "') ";
//                                //    ////strSQL += " - (LeReceivable) + (Rental) ";
//                                //    //strSQL += " - (LeReceivable + (Rental-Payment) ";
//                                //    //strSQL += "As Hsl ";
//                                //    //strSQL += "From Schedule ";
//                                //    //strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

//                                //    strSQL = "SELECT ((SELECT LeReceivable From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period ='" + (Convert.ToInt32(period) - 1) + "') - LeReceivable) - Payment As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

//                                //    da = new SqlDataAdapter(strSQL, conn);
//                                //    dtSchedule = new DataTable();
//                                //    da.Fill(dtSchedule);
//                                //    if (dtSchedule.Rows.Count == 0)
//                                //        ErrCode = 2;
//                                //    else
//                                //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                //}
//                                //else
//                                //{
//                                //    //strSQL = "SELECT (LeReceivable - (SELECT DescBefore4 From LeTrans WHERE EntityNo='" + vLeNo + "' AND SubEntityNo='" + Convert.ToInt32(period) + "')) * -1 As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                //    strSQL  = "SELECT (SELECT DescBefore4 From LeTrans ";
//                                //    strSQL += "WHERE EntityNo='" + vLeNo + "' AND SubEntityNo='" + Convert.ToInt32(period) + "') - (LeReceivable) ";
//                                //    strSQL += "As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

//                                //    da = new SqlDataAdapter(strSQL, conn);
//                                //    dtSchedule = new DataTable();
//                                //    da.Fill(dtSchedule);
//                                //    if (dtSchedule.Rows.Count == 0)
//                                //        ErrCode = 2;
//                                //    else
//                                //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                //}
//                                #endregion

//                                //Altered By Yusuf 24 Okt 2011
//                                //strSQL = "SELECT ((SELECT LeReceivable From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period ='" + (Convert.ToInt32(period) - 1) + "') - LeReceivable) - Payment As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

//                                //Altered By Marsolim 6 December 2011
//                                strSQL = "SELECT (" + Convert.ToDouble(rowTrans["DescBefore4"]) + " - LeReceivable) As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                //End Alter

//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);

//                                break;

//                            // CF
//                            case "LIC":
//                                //Commented By Marsolim 6 Dec 2011
//                                //strSQL = "select abs(Adjustment) As Adjustment From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                //da = new SqlDataAdapter(strSQL, conn);
//                                //dtSchedule = new DataTable();
//                                //da.Fill(dtSchedule);
//                                //if (dtSchedule.Rows.Count == 0)
//                                //    ErrCode = 2;
//                                //else
//                                //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
//                                //End Comment

//                                //Added By Marsolim 6 Dec 2011
//                                vAmt = Math.Abs(Convert.ToDouble(rowTrans["DescAfter3"]));
//                                //End Add
//                                break;

//                            case "CFR":


//                                //Altered By Yusuf 24 Okt 2011
//                                //strSQL = "SELECT ((SELECT LeReceivable From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period ='" + (Convert.ToInt32(period) - 1) + "') - LeReceivable) - Payment As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

//                                //Altered By Marsolim 6 December 2011
//                                strSQL = "SELECT (" + Convert.ToDouble(rowTrans["DescBefore4"]) + " - LeReceivable) As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                //End Alter

//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                break;

//                            // For All
//                            case "DOL":
//                                //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                strSQL = "Select (AssetOutPrinc / TotalPrincipal) * ";
//                                strSQL += "(Select Security From Lease Where LeaseNo='" + vLeNo + "') As S ";
//                                strSQL += "From LetTermination ";
//                                strSQL += "Where LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND tKey = " + Tkey;
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["S"]);
//                                break;

//                            case "RV":
//                                //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                strSQL = "Select (AssetOutPrinc / TotalPrincipal) * ";
//                                strSQL += "(Select Residual From Lease Where LeaseNo='" + vLeNo + "') As D ";
//                                strSQL += "From LetTermination ";
//                                strSQL += "Where LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND tKey = " + Tkey;
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtLease = new DataTable();
//                                da.Fill(dtLease);
//                                if (dtLease.Rows.Count == 0)
//                                    ErrCode = 1;
//                                else
//                                    vAmt = Convert.ToDouble(dtLease.Rows[0]["D"]);
//                                break;

//                            case "OVI":
//                                strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=0 and Period='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);
//                                break;

//                            case "GLS":
//                                strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=0 and Period='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["GainLoss"]);
//                                break;

//                            case "CTL":
//                                strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=0 ";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                break;

//                            case "CTC":
//                                strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=0 ";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                break;
//                        }
//                    }
//                    #endregion

//                    #region Early Termination
//                    // Early Terminate
//                    else if (Status == 3)
//                    {
//                        object period, hslp, BungaDenda;
//                        int TotPeriod;

//                        period = xPer;
//                        hslp = Convert.ToInt32(xPer - 1);

//                        BungaDenda = LeaseTools.GetINSTANCE().GetFieldValue(
//                                "CurrInt",
//                                "LetTermination",
//                                "LeaseNo='" + vLeNo + "' AND Type=1 AND Period='" + Convert.ToInt32(period) + "'"
//                            );

//                        TotPeriod = Convert.ToInt32(LeaseTools.GetINSTANCE().GetFieldValue(
//                                "COUNT(*)-3",
//                                "Schedule",
//                                "LeaseNo='" + vLeNo + "'"
//                            ));

//                        if (TotPeriod == Convert.ToInt32(period))
//                        {
//                            double vCurrIntAmt = 0;
//                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                            da = new SqlDataAdapter(strSQL, conn);
//                            dtSchedule = new DataTable();
//                            da.Fill(dtSchedule);
//                            if (dtSchedule.Rows.Count == 0)
//                                ErrCode = 2;
//                            else
//                                vCurrIntAmt = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);

//                            double vCFLeaseIncomeAmt = 0;
//                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                            da = new SqlDataAdapter(strSQL, conn);
//                            dtSchedule = new DataTable();
//                            da.Fill(dtSchedule);
//                            if (dtSchedule.Rows.Count == 0)
//                                ErrCode = 2;
//                            else
//                                vCFLeaseIncomeAmt = Convert.ToDouble(dtSchedule.Rows[0]["CFLeaseIncome"]);

//                            switch (RetrieveAmt)
//                            {
//                                // Leasing
//                                case "ULI":
//                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    //Altered By Marsolim 24 Okt 2011
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt;
//                                    vAmt = 0;
//                                    break;

//                                case "LRL":
//                                    //if (Convert.ToDouble(BungaDenda) == 0)
//                                    //{
//                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(hslp) + "'";
//                                    //    da = new SqlDataAdapter(strSQL, conn);
//                                    //    dtSchedule = new DataTable();
//                                    //    da.Fill(dtSchedule);
//                                    //    if (dtSchedule.Rows.Count == 0)
//                                    //        ErrCode = 2;
//                                    //    else
//                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    //}
//                                    //else
//                                    //{
//                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                    //    da = new SqlDataAdapter(strSQL, conn);
//                                    //    dtSchedule = new DataTable();
//                                    //    da.Fill(dtSchedule);
//                                    //    if (dtSchedule.Rows.Count == 0)
//                                    //        ErrCode = 2;
//                                    //    else
//                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    //}
//                                    if (Convert.ToDouble(BungaDenda) == 0)
//                                    {
//                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    }
//                                    else
//                                    {
//                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]) - (Convert.ToDouble(BungaDenda));
//                                    }
//                                    break;

//                                case "LIL":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        //vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCurrIntAmt + vCFLeaseIncomeAmt;
//                                        //Altered By Marsolim 24 Okt 2011
//                                        vAmt = vCurrIntAmt + vCFLeaseIncomeAmt;
//                                    //vAmt = 0;
//                                    break;

//                                //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                //da = new SqlDataAdapter(strSQL, conn);
//                                //dtSchedule = new DataTable();
//                                //da.Fill(dtSchedule);
//                                //if (dtSchedule.Rows.Count == 0)
//                                //    ErrCode = 2;
//                                //else
//                                //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeIncome"]);

//                                // CF
//                                case "LIC":
//                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    //Altered By Marsolim 24 Okt 2011
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt;
//                                    //vAmt = 0;
//                                    break;

//                                case "CFR":
//                                    //if (Convert.ToDouble(BungaDenda) == 0)
//                                    //{
//                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(hslp) + "'";
//                                    //    da = new SqlDataAdapter(strSQL, conn);
//                                    //    dtSchedule = new DataTable();
//                                    //    da.Fill(dtSchedule);
//                                    //    if (dtSchedule.Rows.Count == 0)
//                                    //        ErrCode = 2;
//                                    //    else
//                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    //}
//                                    //else
//                                    //{
//                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                    //    da = new SqlDataAdapter(strSQL, conn);
//                                    //    dtSchedule = new DataTable();
//                                    //    da.Fill(dtSchedule);
//                                    //    if (dtSchedule.Rows.Count == 0)
//                                    //        ErrCode = 2;
//                                    //    else
//                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    //}
//                                    if (Convert.ToDouble(BungaDenda) == 0)
//                                    {
//                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    }
//                                    else
//                                    {
//                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]) - (Convert.ToDouble(BungaDenda));
//                                    }

//                                    break;

//                                case "CFI":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        //vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCurrIntAmt + vCFLeaseIncomeAmt;
//                                        //Altered by Marsolim 24 Okt 2011
//                                        vAmt = vCurrIntAmt + vCFLeaseIncomeAmt;
//                                    break;
//                                ////strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                ////da = new SqlDataAdapter(strSQL, conn);
//                                ////dtSchedule = new DataTable();
//                                ////da.Fill(dtSchedule);
//                                ////if (dtSchedule.Rows.Count == 0)
//                                ////    ErrCode = 2;
//                                ////else
//                                ////    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeIncome"]);
//                                //vAmt = 0;
//                                //break;

//                                // For All
//                                case "DOL":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Security"]);
//                                    break;

//                                case "RV":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                    break;

//                                case "OVI":
//                                    vAmt = 0;
//                                    //strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtSchedule = new DataTable();
//                                    //da.Fill(dtSchedule);
//                                    //if (dtSchedule.Rows.Count == 0)
//                                    //    ErrCode = 2;
//                                    //else
//                                    //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);
//                                    break;

//                                case "GLS":
//                                    //strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtSchedule = new DataTable();
//                                    //da.Fill(dtSchedule);
//                                    //if (dtSchedule.Rows.Count == 0)
//                                    //    ErrCode = 2;
//                                    //else
//                                    //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["GainLoss"]);
//                                    vAmt = 0;
//                                    break;

//                                case "ADF":
//                                    strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AllowancedblfullAmt"]);
//                                    break;

//                                case "CTL":
//                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    break;

//                                case "CTC":
//                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    break;
//                            }
//                        }
//                        else
//                        {
//                            double vCurrIntAmt1 = 0;
//                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                            da = new SqlDataAdapter(strSQL, conn);
//                            dtSchedule = new DataTable();
//                            da.Fill(dtSchedule);
//                            if (dtSchedule.Rows.Count == 0)
//                                ErrCode = 2;
//                            else
//                                vCurrIntAmt1 = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);


//                            double vCFLeaseIncomeAmt1 = 0;
//                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                            da = new SqlDataAdapter(strSQL, conn);
//                            dtSchedule = new DataTable();
//                            da.Fill(dtSchedule);
//                            if (dtSchedule.Rows.Count == 0)
//                                ErrCode = 2;
//                            else
//                                vCFLeaseIncomeAmt1 = Convert.ToDouble(dtSchedule.Rows[0]["CFLeaseIncome"]);

//                            switch (RetrieveAmt)
//                            {
//                                // Leasing
//                                case "ULI":
//                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    //Altered By Marsolim 24 Okt 2011
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt1;
//                                    break;

//                                case "LRL":

//                                    strSQL = "Select (LeReceivable + Rental - Payment) as LeRec From Schedule Where LeaseNo='" + vLeNo +
//                                        "' And Period='" + (period) + "'";
//                                    //End Alter

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeRec"]);

//                                    break;

//                                case "LIL":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else

//                                        vAmt = vCurrIntAmt1 + vCFLeaseIncomeAmt1;
//                                    break;

//                                // CF
//                                case "LIC":
//                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    //Altered By Marsolim 24 Okt 2011
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt1;
//                                    break;

//                                case "CFR":
//                                    strSQL = "select LeReceivable-(Select Payment From Schedule Where LeaseNo='" + vLeNo +
//                                    "' And Period='" + (period) + "') As LeRec from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(hslp) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeRec"]);

//                                    break;

//                                case "CFI":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else

//                                        vAmt = vCurrIntAmt1 + vCFLeaseIncomeAmt1;
//                                    break;

//                                // For All
//                                case "DOL":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Security"]);
//                                    break;

//                                case "RV":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                    break;

//                                case "OVI":
//                                    vAmt = 0;

//                                    break;

//                                case "GLS":
//                                    strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["GainLoss"]);
//                                    break;

//                                case "ADF":
//                                    strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AllowancedblfullAmt"]);
//                                    break;

//                                case "CTL":
//                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    break;

//                                case "CTC":
//                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    break;
//                            }
//                        }
//                    }
//                    #endregion

//                    #region Floating
//                    // Floating
//                    else if (Status == 4)
//                    {
//                        // Leasing
//                        object period;
//                        period = xPer;

//                        switch (RetrieveAmt)
//                        {
//                            case "LRL":
//                            case "ULI":
//                            case "CRC":
//                            case "UCF":
//                                strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = (dtSchedule.Rows[0]["Adjustment"] == DBNull.Value ? 0D : Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]));
//                                break;
//                        }
//                    }
//                    #endregion

//                    #region Rescheduling
//                    // Rescheduling
//                    else if (Status == 21)
//                    {
//                        // Leasing
//                        object period;
//                        period = xPer;

//                        switch (RetrieveAmt)
//                        {
//                            case "LRL":
//                            case "ULI":
//                            case "CRC":
//                            case "UCF":
//                                strSQL = "select RescAdjust from LeRescheduling where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["RescAdjust"]);


//                                break;

//                            // Add by Anthony - 20160321
//                            case "APF":
//                                strSQL = "SELECT ISNULL(ProvisionFee, 0) AS ProvisionFee FROM Lease WHERE LeaseNo = @leaseno;";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);

//                                if (dtSchedule.Rows.Count > 0)
//                                {
//                                    vAmt = 0;
//                                }
//                                else
//                                {
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["ProvisionFee"]);

//                                }
//                                break;
//                                // End of add by Anthony - 20160321

//                        }
//                    }
//                    #endregion

//                    #region Contract Interest Accrued For JF
//                    // Contract Interest Accrued
//                    else if (Status == 24)
//                    {

//                        if (rowTrans["Amount"] == null)
//                            ErrCode = 2;
//                        else
//                            vAmt = Convert.ToDouble(rowTrans["Amount"]);
//                    }
//                    #endregion

//                }
//                catch (Exception ex)
//                {

//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }


//        // Get Max Field Value
//        public object GetMaxFieldValue(
//            string strFieldName,
//            string strTableName,
//            string strCondition
//        )
//        {
//            return GetMaxFieldValue(
//                strFieldName,
//                strTableName,
//                strCondition,
//                null
//            );
//        }

//        public object GetMaxFieldValue(
//            string strFieldName,
//            string strTableName,
//            string strCondition,
//            SqlTransaction trans
//        )
//        {
//            SqlConnection conn;
//            if (trans == null)
//            {
//                conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString());
//                conn.Open();
//            }
//            else
//                conn = trans.Connection;
//            object obj;
//            try
//            {
//                string strSQL;
//                strSQL = "SELECT ISNULL(MAX(" + strFieldName + "),0) FROM " + strTableName + " WHERE " + strCondition;
//                SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
//                if (trans != null)
//                    da.SelectCommand.Transaction = trans;
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                if (dt.Rows.Count == 0)
//                    obj = 0;
//                else
//                    obj = dt.Rows[0][0];
//            }
//            catch (Exception ex)
//            {
//                obj = 0;
            
//            }
//            finally
//            {
//                if (trans == null)
//                    conn.Close();
//            }
//            return obj;
//        }


//        // Get Sum Field Value
//        public double GetSumFieldValue(
//            string strFieldName,
//            string strTableName,
//            string strCondition
//        )
//        {
//            return GetSumFieldValue(
//                strFieldName,
//                strTableName,
//                strCondition,
//                null
//            );
//        }


//        public double GetSumFieldValue(
//            string strFieldName,
//            string strTableName,
//            string strCondition,
//            SqlTransaction trans
//        )
//        {
//            SqlConnection conn;
//            if (trans == null)
//            {
//                conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString());
//                conn.Open();
//            }
//            else
//                conn = trans.Connection;
//            double d;

//            try
//            {
//                string strSQL;
//                strSQL = "SELECT ISNULL(SUM(" + strFieldName + "),0) FROM " + strTableName + " WHERE " + strCondition;
//                SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
//                if (trans != null)
//                    da.SelectCommand.Transaction = trans;
//                DataTable dt = new DataTable();
//                da.Fill(dt);
//                if (dt.Rows.Count == 0)
//                    d = 0;
//                else
//                    d = Convert.ToDouble(dt.Rows[0][0]);
//            }
//            catch (Exception ex)
//            {
//                d = 0;
               
//            }
//            finally
//            {
//                if (trans == null)
//                    conn.Close();
//            }
//            return d;
//        }

//        public void UpdateBankGLTrans(
//          string AgreementNo,
//          int MaxTransNo,
//          string BRCode,
//          string HJCode,
//          ref int MsgHsl
//          )
//            {
//                string BCode = "";
//                string GelAcc = "";
//                string GelAccName = "";
//                string GetNewVoucherNo = "";

//                // Get Bank Code
//                BCode = LeaseTools.GetINSTANCE().GetFieldValue(
//                     "BankCode",
//                     "Lease",
//                     "LeaseNo='" + AgreementNo + "'"
//                    ).ToString();

//                if (BCode != "BCA")
//                {
//                    // Get GelAcc No
//                    GelAcc = LeaseTools.GetINSTANCE().GetFieldValue(
//                         "GelAcc",
//                         "tblBANK",
//                         "BankCode='" + BCode + "'"
//                        ).ToString();

//                    // Get GelAcc Name
//                    GelAccName = LeaseTools.GetINSTANCE().GetFieldValue(
//                         "Name",
//                         "ACFGLMH",
//                         "ACC='" + GelAcc + "'"
//                        ).ToString();

//                    // Get Voucher No.
//                    GetNewVoucherNo = LeaseTools.GetINSTANCE().GetFieldValue(
//                          "Voucher",
//                          "LeTrans",
//                          "TransNo = " + MaxTransNo + ""
//                        ).ToString();

//                    // Update Bank in GL Trans
//                    LeaseTools.GetINSTANCE().Le_SPUpdateBankGLTrans(
//                        GetNewVoucherNo,
//                        BRCode,
//                        HJCode,
//                        GelAcc,
//                        GelAccName,
//                        ref MsgHsl
//                        );
//                }
//            }
//        public void Le_SPUpdateBankGLTrans(
//        string VoucherNo,
//        string BranchCode,
//        string JCode,
//        string AccNo,
//        string AccName,
//        ref int Msg
//        )
//        {
//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                SqlTransaction trans = null;
//                try
//                {
//                    conn.Open();
//                    SqlCommand cmd = new SqlCommand();
//                    cmd.Connection = conn;
//                    cmd.CommandType = CommandType.StoredProcedure;

//                    cmd.CommandText = "Le_SPUpdateBankGLTrans ";
//                    cmd.Parameters.AddWithValue("@VoucherNo", VoucherNo);
//                    cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
//                    cmd.Parameters.AddWithValue("@JCode", JCode);
//                    cmd.Parameters.AddWithValue("@AccNo", AccNo);
//                    cmd.Parameters.AddWithValue("@AccName", AccName);

//                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
//                    cmd.Transaction = trans;
//                    cmd.ExecuteNonQuery();
//                    trans.Commit();
//                }
//                catch (Exception ex)
//                {
//                    if (trans != null)
//                        trans.Rollback();
//                    Msg = 0;
//                    throw ex;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }
//        //Add by Jeremi 14 Juni 2025
//        public void Loan_SPUpdateGLTrans(
//            string strTransNo,
//            string Scode,
//            string Voucher,
//            byte Counter,
//            DateTime EntryDate,
//            DateTime TransDate,
//            bool Reverse,
//            string Dept,
//            string Account,
//            string Ccy,
//            double Amount,
//            string Description,
//            string LeaseNo,
//            string DocNo,
//            bool TransferToGL,
//            int ErrCode,
//            DateTime OriTransDate,
//            string strBranchCode,
//            string strAccCashBasis,
//            ref int message
//        )
//        {
//            int Action;

//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                int hasil = 0;
//                SqlTransaction trans = null;
//                trans = conn.BeginTransaction();
//                clsModule mdl = new clsModule();
//                SqlCommand cmd = new SqlCommand();
//                cmd.Connection = conn;
//                cmd.Transaction = trans;
//                cmd.CommandType = CommandType.StoredProcedure;
//                try
//                {
//                    string strSQL;
//                    strSQL = "SELECT * FROM LoanGLTrans WHERE SCODE='" + Scode + "' and Voucher='" + Voucher + "' and BranchCode='" + strBranchCode + "' and Counter=" + Counter.ToString();
//                    if (!mdl.checkExistingData(strSQL))
//                        Action = 0; //Insert LeGLTrans
//                    else
//                        Action = 1; //Update LeGLTrans
//                    cmd.Parameters.Clear();
//                    cmd.CommandText = "Loan_SPUpdateGLTrans";
//                    cmd.Parameters.AddWithValue("@TransNo", strTransNo);
//                    cmd.Parameters.AddWithValue("@Action", Action);
//                    cmd.Parameters.AddWithValue("@Scode", Scode);
//                    cmd.Parameters.AddWithValue("@Voucher", Voucher);
//                    cmd.Parameters.AddWithValue("@Counter", Counter);
//                    cmd.Parameters.AddWithValue("@EntryDate", EntryDate);
//                    cmd.Parameters.AddWithValue("@TransDate", TransDate);
//                    cmd.Parameters.AddWithValue("@Reverse", Reverse);
//                    cmd.Parameters.AddWithValue("@Dept", Dept);
//                    cmd.Parameters.AddWithValue("@Account", Account);
//                    cmd.Parameters.AddWithValue("@Ccy", Ccy);
//                    cmd.Parameters.AddWithValue("@Amount", Amount);
//                    cmd.Parameters.AddWithValue("@Description", Description);
//                    cmd.Parameters.AddWithValue("@LeaseNo", LeaseNo);
//                    cmd.Parameters.AddWithValue("@DocNo", DocNo);
//                    cmd.Parameters.AddWithValue("@TransferToGL", TransferToGL);
//                    cmd.Parameters.AddWithValue("@ErrCode", ErrCode);
//                    cmd.Parameters.AddWithValue("@OriTransDate", OriTransDate);
//                    cmd.Parameters.AddWithValue("@BranchCode", strBranchCode);
//                    cmd.Parameters.AddWithValue("@AccCashBasis", strAccCashBasis);
//                    hasil = cmd.ExecuteNonQuery();
//                    trans.Commit();
//                }
//                catch (Exception ex)
//                {
//                    message = 0;
//                    trans.Rollback();
//                    //WebMsgBox.Show(ex.Message);
//                }
//                finally
//                {
//                    conn.Close();
//                    if (hasil > 0)
//                    {
//                        int a = 0;

//                        if (Scode.Substring(0, 2) == "FD")
//                        {
//                            a = IDS.LeaseTrans.Disbursement.SaveVoucherLoan(Voucher, LeaseNo);

//                        }
//                        else
//                        {
//                            a = IDS.LeaseTrans.Disbursement.SaveVoucher(Voucher, LeaseNo);

//                        }
//                    }
//                }
//            }
//        }


//        public void RA_SPUpdateGLTrans(
//            string strTransNo,
//            string Scode,
//            string Voucher,
//            byte Counter,
//            DateTime EntryDate,
//            DateTime TransDate,
//            bool Reverse,
//            string Dept,
//            string Account,
//            string Ccy,
//            double Amount,
//            string Description,
//            string LeaseNo,
//            string DocNo,
//            bool TransferToGL,
//            int ErrCode,
//            DateTime OriTransDate,
//            string strBranchCode,
//            string strAccCashBasis,
//            ref int message
//        )
//        {
//            int Action;

//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                int hasil = 0;
//                SqlTransaction trans = null;
//                trans = conn.BeginTransaction();
//                clsModule mdl = new clsModule();
//                SqlCommand cmd = new SqlCommand();
//                cmd.Connection = conn;
//                cmd.Transaction = trans;
//                cmd.CommandType = CommandType.StoredProcedure;
//                try
//                {
//                    string strSQL;
//                    strSQL = "SELECT * FROM LoanGLTrans WHERE SCODE='" + Scode + "' and Voucher='" + Voucher + "' and BranchCode='" + strBranchCode + "' and Counter=" + Counter.ToString();
//                    if (!mdl.checkExistingData(strSQL))
//                        Action = 0; //Insert LeGLTrans
//                    else
//                        Action = 1; //Update LeGLTrans
//                    cmd.Parameters.Clear();
//                    cmd.CommandText = "Loan_SPUpdateGLTrans";
//                    cmd.Parameters.AddWithValue("@TransNo", strTransNo);
//                    cmd.Parameters.AddWithValue("@Action", Action);
//                    cmd.Parameters.AddWithValue("@Scode", Scode);
//                    cmd.Parameters.AddWithValue("@Voucher", Voucher);
//                    cmd.Parameters.AddWithValue("@Counter", Counter);
//                    cmd.Parameters.AddWithValue("@EntryDate", EntryDate);
//                    cmd.Parameters.AddWithValue("@TransDate", TransDate);
//                    cmd.Parameters.AddWithValue("@Reverse", Reverse);
//                    cmd.Parameters.AddWithValue("@Dept", Dept);
//                    cmd.Parameters.AddWithValue("@Account", Account);
//                    cmd.Parameters.AddWithValue("@Ccy", Ccy);
//                    cmd.Parameters.AddWithValue("@Amount", Amount);
//                    cmd.Parameters.AddWithValue("@Description", Description);
//                    cmd.Parameters.AddWithValue("@LeaseNo", LeaseNo);
//                    cmd.Parameters.AddWithValue("@DocNo", DocNo);
//                    cmd.Parameters.AddWithValue("@TransferToGL", TransferToGL);
//                    cmd.Parameters.AddWithValue("@ErrCode", ErrCode);
//                    cmd.Parameters.AddWithValue("@OriTransDate", OriTransDate);
//                    cmd.Parameters.AddWithValue("@BranchCode", strBranchCode);
//                    cmd.Parameters.AddWithValue("@AccCashBasis", strAccCashBasis);
//                    hasil = cmd.ExecuteNonQuery();
//                    trans.Commit();
//                }
//                catch (Exception ex)
//                {
//                    message = 0;
//                    trans.Rollback();
//                    //WebMsgBox.Show(ex.Message);
//                }
//                finally
//                {
//                    conn.Close();
//                    if (hasil > 0)
//                    {
//                        int a = 0;

//                        if (Scode.Substring(0, 2) == "FD")
//                        {
//                            a = IDS.LeaseTrans.Disbursement.SaveVoucherRA(Voucher, LeaseNo);

//                        }
//                        else
//                        {
//                            a = IDS.LeaseTrans.Disbursement.SaveVoucher(Voucher, LeaseNo);

//                        }
//                    }
//                }
//            }
//        }
//        //End Jeremi

//        //Update GL Trans
//        public void Le_SPUpdateGLTrans(
//            string strTransNo,
//            string Scode,
//            string Voucher,
//            byte Counter,
//            DateTime EntryDate,
//            DateTime TransDate,
//            bool Reverse,
//            string Dept,
//            string Account,
//            string Ccy,
//            double Amount,
//            string Description,
//            string LeaseNo,
//            string DocNo,
//            bool TransferToGL,
//            int ErrCode,
//            DateTime OriTransDate,
//            string strBranchCode,
//            string strAccCashBasis,
//            ref int message
//        )
//        {
//            int Action;

//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                int hasil = 0;
//                SqlTransaction trans = null;
//                trans = conn.BeginTransaction();
//                clsModule mdl = new clsModule();
//                SqlCommand cmd = new SqlCommand();
//                cmd.Connection = conn;
//                cmd.Transaction = trans;
//                cmd.CommandType = CommandType.StoredProcedure;
//                try
//                {
//                    string strSQL;
//                    strSQL = "SELECT * FROM LeGLTRANS WHERE SCODE='" + Scode + "' and Voucher='" + Voucher + "' and BranchCode='" + strBranchCode + "' and Counter=" + Counter.ToString();
//                    if (!mdl.checkExistingData(strSQL))
//                        Action = 0; //Insert LeGLTrans
//                    else
//                        Action = 1; //Update LeGLTrans
//                    cmd.Parameters.Clear();
//                    cmd.CommandText = "Le_SPUpdateGLTrans";
//                    cmd.Parameters.AddWithValue("@TransNo", strTransNo);
//                    cmd.Parameters.AddWithValue("@Action", Action);
//                    cmd.Parameters.AddWithValue("@Scode", Scode);
//                    cmd.Parameters.AddWithValue("@Voucher", Voucher);
//                    cmd.Parameters.AddWithValue("@Counter", Counter);
//                    cmd.Parameters.AddWithValue("@EntryDate", EntryDate);
//                    cmd.Parameters.AddWithValue("@TransDate", TransDate);
//                    cmd.Parameters.AddWithValue("@Reverse", Reverse);
//                    cmd.Parameters.AddWithValue("@Dept", Dept);
//                    cmd.Parameters.AddWithValue("@Account", Account);
//                    cmd.Parameters.AddWithValue("@Ccy", Ccy);
//                    cmd.Parameters.AddWithValue("@Amount", Amount);
//                    cmd.Parameters.AddWithValue("@Description", Description);
//                    cmd.Parameters.AddWithValue("@LeaseNo", LeaseNo);
//                    cmd.Parameters.AddWithValue("@DocNo", DocNo);
//                    cmd.Parameters.AddWithValue("@TransferToGL", TransferToGL);
//                    cmd.Parameters.AddWithValue("@ErrCode", ErrCode);
//                    cmd.Parameters.AddWithValue("@OriTransDate", OriTransDate);
//                    cmd.Parameters.AddWithValue("@BranchCode", strBranchCode);
//                    cmd.Parameters.AddWithValue("@AccCashBasis", strAccCashBasis);
//                    hasil =cmd.ExecuteNonQuery();
//                    trans.Commit();
//                }
//                catch (Exception ex)
//                {
//                    message = 0;
//                    trans.Rollback();
//                    //WebMsgBox.Show(ex.Message);
//                }
//                finally
//                {
//                    conn.Close();
//                    if (hasil > 0)
//                    {
//                        int a = IDS.LeaseTrans.Disbursement.SaveVoucher(Voucher, LeaseNo);
//                    }
//                }
//            }
//        }

//        // Add - Anthony - 20200513
//        public void Le_SPUpdateGLTrans(IDS.DataAccess.SqlServer cmd,
//            string strTransNo,
//            string Scode,
//            string Voucher,
//            byte Counter,
//            DateTime EntryDate,
//            DateTime TransDate,
//            bool Reverse,
//            string Dept,
//            string Account,
//            string Ccy,
//            double Amount,
//            string Description,
//            string LeaseNo,
//            string DocNo,
//            bool TransferToGL,
//            int ErrCode,
//            DateTime OriTransDate,
//            string strBranchCode,
//            string strAccCashBasis,
//            ref int message
//        )
//        {
//            int Action = 0;

//            IDS.DataAccess.SqlServer db = cmd;

//            try
//            {
//                db.CommandText = "SELECT CASE WHEN EXISTS (SELECT * FROM LeGLTRANS WHERE SCODE = @SCode AND Voucher=@Voucher AND BranchCode = @BranchCode AND Counter = @Counter) THEN 1 ELSE 0 END";
//                db.AddParameter("@SCode", SqlDbType.VarChar, Scode);
//                db.AddParameter("@Voucher", SqlDbType.VarChar, Voucher);
//                db.AddParameter("@BranchCode", SqlDbType.VarChar, strBranchCode);
//                db.AddParameter("@Counter", SqlDbType.Int, Counter);
//                db.CommandType = CommandType.Text;
//                db.Open();

//                int count = Convert.ToInt32(db.ExecuteScalar());

//                if (count == 0)
//                    Action = 0;
//                else
//                    Action = 1;

//                db.CommandText = "Le_SPUpdateGLTrans";
//                db.AddParameter("@TransNo", SqlDbType.VarChar, strTransNo);
//                db.AddParameter("@Action", SqlDbType.Int, Action);
//                db.AddParameter("@SCode", SqlDbType.VarChar, Scode);
//                db.AddParameter("@Voucher", SqlDbType.VarChar, Voucher);
//                db.AddParameter("@Counter", SqlDbType.Int, Counter);
//                db.AddParameter("@EntryDate", SqlDbType.DateTime, EntryDate);
//                db.AddParameter("@TransDate", SqlDbType.Date, TransDate);
//                db.AddParameter("@Reverse", SqlDbType.Bit, Reverse);
//                db.AddParameter("@Dept", SqlDbType.VarChar, Dept);
//                db.AddParameter("@Account", SqlDbType.VarChar, Account);
//                db.AddParameter("@Ccy", SqlDbType.VarChar, Ccy);
//                db.AddParameter("@Amount", SqlDbType.Money, Amount);
//                db.AddParameter("@Description", SqlDbType.VarChar, Description);
//                db.AddParameter("@LeaseNo", SqlDbType.VarChar, LeaseNo);
//                db.AddParameter("@DocNo", SqlDbType.VarChar, DocNo);
//                db.AddParameter("@TransferToGL", SqlDbType.Bit, TransferToGL);
//                db.AddParameter("@ErrCode", SqlDbType.Int, ErrCode);
//                db.AddParameter("@OriTransDate", SqlDbType.DateTime, OriTransDate);
//                db.AddParameter("@BranchCode", SqlDbType.VarChar, strBranchCode);
//                db.AddParameter("@AccCashBasis", SqlDbType.VarChar, string.IsNullOrEmpty(strAccCashBasis) ? "" : strAccCashBasis);

//                db.CommandType = CommandType.StoredProcedure;
//                db.Open();

//                cmd.ExecuteNonQuery();
//            }
//            catch (Exception ex)
//            {
//                message = 0;
//                db.RollbackTransaction();
//               // WebMsgBox.Show(ex.Message);
//            }
//        }
//        // End
//        public double GetExchangeRate(string strCurrCode,DateTime date)
//        {
//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                try
//                {
//                    string strSQL;
//                    strSQL = "SELECT ISNULL((SELECT TOP 1 MidRate FROM tblExchangeRate ";
//                    strSQL += "WHERE CurrencyCode1 = '" + strCurrCode + "' AND CurrencyCode2=";
//                    strSQL += "(SELECT BaseCcy FROM SYSPAR) AND ExchangeDate <= GETDATE() ";
//                    strSQL += "ORDER BY ExchangeDate DESC),1) AS CurrentRate";
//                    SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
//                    DataTable dt = new DataTable();
//                    da.Fill(dt);
//                    return Convert.ToDouble(dt.Rows[0][0]);
//                }
//                catch (Exception ex)
//                {
//                    //WebMsgBox.Show(ex.Message);
//                    return 0;
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }

//        public void CreateGLTfTransNewPrincTerminate(
//            string strTransNo,
//            string strBranchCode,
//            string strSCode,
//            long NoProcess,
//            DateTime Period,
//            string strVoucher,
//            ref int message,
//            int xPeriod,
//            int Tkey
//        )
//        {
//            string VchNo, SCode, JCode, vLeNo, sAccNo;
//            string sAccCashBasis;
//            string sCcy, sTCurrCode;
//            Byte X, I, bCounter;
//            long Done;
//            int DbCr, ErrCode, GLSeqNo = 0;
//            double Portion = 0;
//            Byte nPeriod;
//            double TotDb, TotCr, vAmt, cExchRateJournal;
//            double cExchRateVoucher, cAmount, cExchRate;
//            DateTime CSLTransDate;
//            string sDesc;
//            DateTime StopAccrueDate;
//            int Action = 0;
//            string sBaseCcy;
//            DataTable dtJTD;
//            DataTable dtGLTrans;
//            DataTable dtLease;

//            //Variable For GLTrans
//            string GL_SCode, GL_Voucher, GL_Account, GL_Ccy;
//            string GL_Dept = "", GL_Description, GL_LeaseNo, GL_DocNo;
//            byte GL_Counter;
//            DateTime GL_EntryDate, GL_OriTransDate, GL_TransDate;
//            bool GL_Reverse, GL_TransferToGL;
//            double GL_Amount;
//            int GL_ErrCode;
//            string GLBranchCode, GLCashBasis;

//            string ContractNo = "";
//            string CustNo = "";
//            string CustBranch = "";
//            int GrpNo = 0;
//            int GroupType = 0;

//            sBaseCcy = IDS.GeneralTable.Syspar.GetInstance().BaseCCy;

//            // Get Contract No 
//            ContractNo = GetFieldValue(
//                 "EntityNo",
//                 "LeTrans",
//                 "TransNo='" + strTransNo + "'"
//                ).ToString();

//            // Get Customer No
//            CustNo = GetFieldValue(
//                 "LesseeNo",
//                 "Lease",
//                 "LeaseNo='" + ContractNo + "'"
//                ).ToString();

//            // Get Customer Branch
//            CustBranch = GetFieldValue(
//                 "LesseeBranch",
//                 "Lease",
//                 "LeaseNo='" + ContractNo + "'"
//                ).ToString();

//            // Get Customer Group No
//            GrpNo = Convert.ToInt32(GetFieldValue(
//                 "isnull(GrpNo,0)",
//                 "Lessee",
//                 "LesseeNo='" + CustNo + "' And BranchCode='" + CustBranch + "'"
//                ));

//            // Get Customer Group Type
//            if (GrpNo == 0)
//            {
//                GroupType = 0;
//            }
//            else
//            {
//                GroupType = IDS.Tool.GeneralHelper.NullToInt((GetFieldValue(
//                     "isnull(Category,0)",
//                     "tblGroup",
//                     "GrpNo=" + GrpNo + ""
//                    )), 0);

//                if (GroupType == 2)
//                {
//                    GroupType = 0;
//                }
//            }

//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                SqlTransaction trans = null;
//                trans = conn.BeginTransaction();
//                clsModule mdl = new clsModule();
//                SqlCommand cmd = new SqlCommand();
//                cmd.Connection = conn;
//                cmd.Transaction = trans;
//                cmd.CommandType = CommandType.StoredProcedure;
//                try
//                {
//                    string strSQL;
//                    strSQL = "Le_SPpruCreateGLTfTransNewPrs '" + Period + "','" + strVoucher + "','" + strTransNo + "'";
//                    SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
//                    da.SelectCommand.Transaction = trans;
//                    DataTable dtTrans = new DataTable();
//                    da.Fill(dtTrans);
//                    NoProcess = NoProcess + dtTrans.Rows.Count;
//                    if (dtTrans.Rows.Count == 0)
//                    {
//                        // WebMsgBox.Show("No Transaction To Be Created!");
//                        return;
//                    }

//                    ErrCode = 0;
//                    Done = 0;
//                    for (int i = 0; i < dtTrans.Rows.Count; i++)
//                    {
//                        DataRow rowTrans = dtTrans.Rows[i];
//                        Done = Done + 1;
//                        if (mdl.Left(rowTrans["JCode"].ToString(), 2) != "LT")
//                            JCode = rowTrans["JCode"].ToString(); //mdl.Left(rowTrans["JCode"].ToString(), 1) + "1" + mdl.Right(rowTrans["JCode"].ToString(), 2);
//                        else
//                            JCode = rowTrans["JCode"].ToString();

//                        sTCurrCode = rowTrans["CurrCode"].ToString();

//                        string Vch;
//                        string strKondisi;

//                        strKondisi = "SCode='" + JCode + "' AND YEAR(TransDate)='" + Period.ToString("yyyy") + "' AND BranchCode='" + strBranchCode + "'";
//                        // "' AND MONTH(EntryDate)='" + Period.ToString("MM") +
//                        Vch = GetMaxFieldValue("right(Voucher,3)", "LeGLTrans", strKondisi).ToString();
//                        if (!string.IsNullOrEmpty(Vch) && Vch != "0")
//                        {
//                            GLSeqNo = Convert.ToInt16(Vch.Substring(Vch.Length - 3));
//                        }
//                        else
//                        {
//                            GLSeqNo = 0;
//                        }

//                        switch (JCode)
//                        {
//                            // New Contract 
//                            // Leasing Normal

//                            // Alter by Anthony (Disburse) - Perubahan kode disesuaikan dengan kontrak OJK
//                            //case "1L1":
//                            //case "1L2":
//                            //case "1L3":
//                            //case "1L4":
//                            //case "1L5":
//                            //case "1L6":

//                            case "1LIF1": // Investasi - Finance Lease - Normal
//                            case "1LPF1": // Proyek - Finance Lease - Normal
//                            case "1LGF1": // Multiguna - Finance Lease - Normal
//                            case "1LIS1": // Investasi - Sale & Leaseback - Normal
//                            case "1LPS1": // Proyek - Sale & Leaseback - Normal
//                            case "1LMS1": // Modal Kerja - Sale & Leaseback - Normal
//                            case "1SIF1":
//                            // Leasing Receivable Assignment
//                            case "1LIF4": // Investasi - Finance Lease - Receivable Assignment
//                            case "1LPF4": // Proyek - Finance Lease - Receivable Assignment
//                            case "1LGF4": // Multiguna - Finance Lease - Receivable Assignment
//                            case "1LIS4": // Investasi - Sale & Leaseback - Receivable Assignment
//                            case "1LPS4": // Proyek - Sale & Leaseback - Receivable Assignment
//                            case "1LMS4": // Modal Kerja - Sale & Leaseback - Receivable Assignment

//                            //case "1C1":
//                            //case "1C2":
//                            //case "1C3":
//                            //case "1C4":
//                            //case "1C5":
//                            //case "1C6":

//                            // CF Normal
//                            case "1CIB1": // Investasi - Installment Barang - Normal
//                            case "1CPB1": // Proyek - Installment Barang - Normal
//                            case "1CGB1": // Multiguna - Installment Barang - Normal
//                            case "1CPJ1": // Proyek - Installment Jasa - Normal
//                            case "1CGJ1": // Multiguna - Installment Jasa - Normal
//                            case "1CMU1": // Modal Kerja - Modal Usaha - Normal
//                            case "1CGG1": // Multiguna - Fasilitas Dana - Barang - Normal
//                            case "1CGS1": // Multiguna - Fasilitas Dana - Jasa - Normal

//                            // CF Receivable Assigment
//                            case "1CIB4": // Investasi - Installment Barang - Receivable Assigment
//                            case "1CPB4": // Proyek - Installment Barang - Receivable Assigment
//                            case "1CGB4": // Multiguna - Installment Barang - Receivable Assigment
//                            case "1CPJ4": // Proyek - Installment Jasa - Receivable Assigment
//                            case "1CGJ4": // Multiguna - Installment Jasa - Receivable Assigment
//                            case "1CMU4": // Modal Kerja - Modal Usaha - Receivable Assigment
//                            case "1CGG4": // Multiguna - Fasilitas Dana - Barang - Receivable Assignment
//                            case "1CGS4": // Multiguna - Fasilitas Dana - Jasa - Receivable Assignment
//                                          // End of alter by Anthony (Disburse)




//                            // SLB
//                            //case "1S1":
//                            //case "1S2":
//                            //case "1S3":
//                            //case "1S4":
//                            //case "1S5":

//                            // Alter by Anthony - 20151229 - OJK
//                            // Partial Terminate
//                            //// L
//                            //case "2L1":
//                            //case "2L2":
//                            //case "2L3":
//                            //case "2L4":
//                            //case "2L5":
//                            //case "2L6":

//                            //// CF
//                            //case "2C1":
//                            //case "2C2":
//                            //case "2C3":
//                            //case "2C4":
//                            //case "2C5":
//                            //case "2C6":
//                            // L
//                            case "2LIF1":
//                            case "2LPF1":
//                            case "2LGF1":
//                            case "2LIS1":
//                            case "2LPS1":
//                            case "2LMS1":

//                            // CF
//                            case "2CIB1":
//                            case "2CPB1":
//                            case "2CGB1":
//                            case "2CPJ1":
//                            case "2CMJ1":
//                            case "2CMU1":
//                            case "2CGG1":
//                            case "2CGS1":

//                            //Receivable Assignment
//                            // L
//                            case "2LIF4":
//                            case "2LPF4":
//                            case "2LGF4":
//                            case "2LIS4":
//                            case "2LPS4":
//                            case "2LMS4":

//                            // CF
//                            case "2CIB4":
//                            case "2CPB4":
//                            case "2CGB4":
//                            case "2CPJ4":
//                            case "2CMJ4":
//                            case "2CMU4":
//                            case "2CGG4":
//                            case "2CGS4":
//                            // End of alter by Anthony - OJK

//                            // SLB
//                            //case "2S1":
//                            //case "2S2":
//                            //case "2S3":
//                            //case "2S4":
//                            //case "2S5":

//                            // Alter by Anthony (Full Terminate) - OJK
//                            //// Full Terminate
//                            //// L
//                            //case "3L1":
//                            //case "3L2":
//                            //case "3L3":
//                            //case "3L4":
//                            //case "3L5":
//                            //case "3L6":

//                            case "3LIF1":
//                            case "3LPF1":
//                            case "3LGF1":
//                            case "3LIS1":
//                            case "3LPS1":
//                            case "3LMS1":
//                            case "3LGB1":

//                            //// CF
//                            //case "3C1":
//                            //case "3C2":
//                            //case "3C3":
//                            //case "3C4":
//                            //case "3C5":
//                            //case "3C6":

//                            case "3CIB1":
//                            case "3CPB1":
//                            case "3CGB1":
//                            case "3CPJ1":
//                            case "3CGJ1":
//                            case "3CMU1":
//                            case "3CGG1":
//                            case "3CGS1":
//                            // End of alter by Anthony (Full Terminate)

//                            // SLB
//                            //case "3S1":
//                            //case "3S2":
//                            //case "3S3":
//                            //case "3S4":
//                            //case "3S5":


//                            // Alter by Anthony (Floating Interest) - OJK
//                            // Floating Interest
//                            // L
//                            //case "4L1":
//                            //case "4L2":
//                            //case "4L3":
//                            //case "4L4":
//                            //case "4L5":
//                            //case "4L6":

//                            case "4LIF1":
//                            case "4LPF1":
//                            case "4LGF1":
//                            case "4LIS1":
//                            case "4LPS1":
//                            case "4LMS1":

//                            // CF
//                            //case "4C1":
//                            //case "4C2":
//                            //case "4C3":
//                            //case "4C4":
//                            //case "4C5":
//                            //case "4C6":

//                            case "4CIB1":
//                            case "4CPB1":
//                            case "4CGB1":
//                            case "4CPJ1":
//                            case "4CGJ1":
//                            case "4CMU1":
//                            case "4CGG1":
//                            case "4CGS1":
//                            // End of alter by Anthony (Floating)

//                            // SLB
//                            //case "4S1":
//                            //case "4S2":
//                            //case "4S3":
//                            //case "4S4":
//                            //case "4S5":


//                            // Alter by Anthony (Rescheduling) - Perubahan kode disesuaikan dengan kontrak OJK
//                            // Rescheduling
//                            // L
//                            //case "21L1":
//                            //case "21L2":
//                            //case "21L3":
//                            //case "21L4":
//                            //case "21L5":
//                            //case "21L6":

//                            case "21LIF1":
//                            case "21LPF1":
//                            case "21LGF1":
//                            case "21LIS1":
//                            case "21LPS1":
//                            case "21LMS1":
//                            case "29CGB1":
//                            case "29LMU1":
//                            case "29CGG1":
//                            case "29CGJ1":
//                            case "29CGS1":
//                            case "29LGF1":
//                            case "29LGB1":
//                            case "29CIB1":
//                            case "29CIS1":
//                            case "29LIB1":
//                            case "29LIF1":
//                            case "29LIS1":
//                            case "29LMS1":



//                            // CF
//                            //case "21C1":
//                            //case "21C2":
//                            //case "21C3":
//                            //case "21C4":
//                            //case "21C5":
//                            //case "21C6":

//                            case "21CIB1":
//                            case "21CPB1":
//                            case "21CGB1":
//                            case "21CPJ1":
//                            case "21CGJ1":
//                            case "21CMU1":
//                            case "21CGG1":
//                            case "21CGS1":
//                            // End of alter by Anthony (rescheduling)

//                            case "30LIF1":
//                            case "30LPF1":
//                            case "30LGF1":
//                            case "30LIS1":
//                            case "30LPS1":
//                            case "30LMS1":
//                            case "30CGB1":
//                            case "30LMU1":
//                            case "30CGG1":
//                            case "30CGJ1":
//                            case "30CGS1":
//                            case "30LGB1":
//                            case "30CIB1":
//                            case "30CIS1":
//                            case "30LIB1":
                       

//                            // Contract Interest Accrued
//                            // L
//                            case "24L4":

//                            // CF
//                            case "24C4":

//                                // SLB
//                                //case "21S1":
//                                //case "21S2":
//                                //case "21S3":
//                                //case "21S4":
//                                //case "21S5":

//                                X = 1;
//                                bCounter = 1;
//                                vLeNo = rowTrans["EntityNo"].ToString();
//                                //int PeriodFloating = 0;
//                                //PeriodFloating= Convert.ToInt16(rowTrans["SubEntityNo"]);
//                                int PeriodFloating = 0;
//                                object subEntityNo = rowTrans["SubEntityNo"];
//                                if (subEntityNo != null && subEntityNo != DBNull.Value)
//                                {
//                                    if (subEntityNo is int)
//                                    {
//                                        PeriodFloating = (int)subEntityNo; // Jika subEntityNo adalah int, ambil nilainya
//                                    }
//                                    else
//                                    {
//                                        // Jika subEntityNo bukan int, coba konversi ke int
//                                        if (int.TryParse(subEntityNo.ToString(), out int result))
//                                        {
//                                            PeriodFloating = result; // Jika berhasil dikonversi, ambil nilai hasil konversi
//                                        }
//                                        // Jika tidak berhasil dikonversi, periodFloating tetap 0 (nilai default)
//                                    }
//                                }
//                                //PeriodFloating = Convert.ToInt16(rowTrans["SubEntityNo"]);
//                                strSQL = "SELECT * FROM LeJournalTD where JCode='" + JCode + "' and TCurrCode='" + sTCurrCode + "' and GroupType=" + GroupType + " ORDER BY Counter ";

//                                //strSQL += "and Counter=" + X;
//                                da = new SqlDataAdapter(strSQL, conn);
//                                da.SelectCommand.Transaction = trans;
//                                dtJTD = new DataTable();
//                                da.Fill(dtJTD);

//                                if (dtJTD.Rows.Count == 0)
//                                {
//                                    //WebMsgBox.Show("Seek Failure in Journal for " + JCode);
//                                }
//                                else
//                                {
//                                    TotDb = 0;
//                                    TotCr = 0;

//                                    SCode = rowTrans["JCode"].ToString();
//                                    GLSeqNo = GLSeqNo + 1;
//                                    //VchNo = DateTime.Today.ToString("yyMM") + GLSeqNo.ToString("000");
//                                    VchNo = Period.ToString("yyMM") + GLSeqNo.ToString("00000");


//                                    for (int c = 0; c < dtJTD.Rows.Count; c++)
//                                    {
//                                        DataRow rowJTD = dtJTD.Rows[c];
//                                        ErrCode = 0;
//                                        DbCr = 1;
//                                        vAmt = 0;

//                                        if (Convert.ToInt16(rowJTD["RetrieveType"]) == 1)
//                                        {
//                                            if (!string.IsNullOrEmpty(rowTrans["OuterCode"].ToString()))
//                                            {
//                                                sAccNo = GetFieldValue("GelAcc", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                                sCcy = rowJTD["CurrCode"].ToString();
//                                                sDesc = GetFieldValue("BankName", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
//                                                sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            }
//                                            else
//                                            {
//                                                sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                                sAccNo = rowJTD["Acc"].ToString();
//                                                sCcy = rowJTD["CurrCode"].ToString();
//                                                sDesc = rowJTD["Description"].ToString();
//                                            }

//                                        }
//                                        else
//                                        {
//                                            sAccCashBasis = rowJTD["AccCashBasis"].ToString();
//                                            sAccNo = rowJTD["Acc"].ToString();
//                                            sCcy = rowJTD["CurrCode"].ToString();
//                                            sDesc = string.IsNullOrEmpty(rowJTD["Description"].ToString()) ? "0" : rowJTD["Description"].ToString();
//                                        }

//                                        switch (strSCode)
//                                        {
//                                            //Reverse Disburst
//                                            case "L006":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            //Disburst
//                                            case "L001":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Reverse Partial 
//                                            case "L007":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            //Partial
//                                            case "L002":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Reverse Full
//                                            case "L008":
//                                                if (rowJTD["DC"].ToString() == "D")
//                                                    DbCr = -1;
//                                                break;

//                                            case "L003":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Add By: Tomy
//                                            //Floating Interest
//                                            case "L004":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Rescheduling
//                                            case "L021":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Contract Interest Accrued
//                                            case "L024":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;

//                                            //Write OFF
//                                            case "L023":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;
//                                            //Write OFF
//                                            case "L030":
//                                                if (rowJTD["DC"].ToString() == "C")
//                                                    DbCr = -1;
//                                                break;
//                                        }

//                                        GLCashBasis = sAccCashBasis;
//                                        GLBranchCode = strBranchCode;

//                                        GL_SCode = SCode;
//                                        GL_Voucher = VchNo;
//                                        GL_Counter = bCounter;
//                                        GL_EntryDate = DateTime.Today;
//                                        GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                        GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                        GL_Reverse = false;
//                                        GL_Account = sAccNo;
//                                        GL_Ccy = sCcy;
//                                        // Alter - Anthony - 20200709
//                                        //GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString();

//                                        using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
//                                        {
//                                            db.CommandText = "SELECT BranchCode, RepresentativeOffice FROM Lease WHERE LeaseNo = @leaseno";
//                                            db.CommandType = System.Data.CommandType.Text;
//                                            db.AddParameter("@leaseno", SqlDbType.VarChar, vLeNo);
//                                            db.Open();

//                                            db.ExecuteReader();

//                                            using (SqlDataReader rd = db.DbDataReader as SqlDataReader)
//                                            {
//                                                if (rd.HasRows)
//                                                {
//                                                    while (rd.Read())
//                                                    {
//                                                        if (rd["RepresentativeOffice"] == DBNull.Value || string.IsNullOrEmpty(rd["RepresentativeOffice"] as string))
//                                                            GL_Dept = rd["BranchCode"] as string;
//                                                        else
//                                                            GL_Dept = rd["RepresentativeOffice"] as string;
//                                                    }
//                                                }
//                                            }

//                                            db.Close();
//                                        }


//                                        // End alter - Anthony - 20200709

//                                        if (strSCode == "L021" || strSCode == "L024" || strSCode == "L023")
//                                        {
//                                            //GetvAmtErrCode For Approve (status=0)
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 2)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                        }
//                                        else if(strSCode == "L030")
//                                        {
//                                            GetvAmtErrCodePrincTerminate(Convert.ToInt16(GL_SCode.Substring(0, 2)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType,Tkey);
//                                        }
//                                        else
//                                        {
//                                            //GetvAmtErrCode For Approve (status=0)
//                                            //ini diganti doang xperiod menjadi PeriodFloating
//                                            //GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
//                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, PeriodFloating, GroupType);

//                                        }

//                                        cExchRateJournal = sCcy != sBaseCcy ? GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;
//                                        cExchRateVoucher = sTCurrCode != sBaseCcy ? GetExchangeRate(sTCurrCode, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;

//                                        if (sCcy != sTCurrCode)
//                                            vAmt = (vAmt * cExchRateVoucher / cExchRateJournal);
//                                        vAmt = vAmt * DbCr;
//                                        GL_Amount = vAmt;
//                                        GL_Description = sDesc + " " + vLeNo;
//                                        GL_LeaseNo = vLeNo;
//                                        //GL_DocNo = rowTrans["TransCode"].ToString();
//                                        GL_DocNo = strSCode;
//                                        GL_TransferToGL = false;
//                                        GL_ErrCode = ErrCode;

//                                        // Alter - Anthony - 20220521
//                                        //if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
//                                        switch (JCode)
//                                        {
//                                            case "1LIF4":
//                                            case "1LPF4":
//                                            case "1LGF4":
//                                            case "1LIS4":
//                                            case "1LPS4":
//                                            case "1LMS4":
//                                            case "1CIB4":
//                                            case "1CPB4":
//                                            case "1CGB4":
//                                            case "1CPJ4":
//                                            case "1CGJ4":
//                                            case "1CMU4":
//                                            case "1CGG4":
//                                            case "1CGS4":

//                                            case "1LIF6":
//                                            case "1LPF6":
//                                            case "1LGF6":
//                                            case "1LIS6":
//                                            case "1LPS6":
//                                            case "1LMS6":
//                                            case "1CIB6":
//                                            case "1CPB6":
//                                            case "1CGB6":
//                                            case "1CPJ6":
//                                            case "1CGJ6":
//                                            case "1CMU6":
//                                            case "1CGG6":
//                                            case "1CGS6":
//                                                if (rowJTD["RetrieveAmt"].ToString() == "HOP")
//                                                {
//                                                    // Get HOPNO
//                                                    string HOPNO, HOPBranchName;

//                                                    HOPNO = "";
//                                                    HOPBranchName = "";

//                                                    // Get HOPNO
//                                                    HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "HopNo",
//                                                        "tblBranch",
//                                                        "BranchCode='" + GLBranchCode + "'").ToString();

//                                                    // Get HOPBranchName
//                                                    HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "Name",
//                                                        "ACFGLMH",
//                                                        "ACC='" + HOPNO + "'").ToString();

//                                                    GL_Account = HOPNO;
//                                                    GL_Description = HOPBranchName;
//                                                }
//                                                break;
//                                        }


//                                        int msg1 = 1;
//                                        if (GL_Amount != 0)
//                                        {
//                                            Le_SPUpdateGLTrans(
//                                           strTransNo,
//                                           GL_SCode,
//                                           GL_Voucher,
//                                           GL_Counter,
//                                           GL_EntryDate,
//                                           GL_TransDate,
//                                           GL_Reverse,
//                                           GL_Dept,
//                                           GL_Account,
//                                           GL_Ccy,
//                                           GL_Amount,
//                                           GL_Description,
//                                           GL_LeaseNo,
//                                           GL_DocNo,
//                                           GL_TransferToGL,
//                                           GL_ErrCode,
//                                           GL_OriTransDate,
//                                           GLBranchCode,
//                                           GLCashBasis,
//                                           ref msg1);
//                                        }


//                                        if (msg1 == 0)
//                                        {
//                                            //WebMsgBox.Show("Update LeGLTrans Failed!");
//                                        }

//                                        if (sCcy != sBaseCcy)
//                                        {
//                                            //CreateEqvJournal
//                                            cAmount = vAmt;
//                                            cExchRate = GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"]));
//                                            bCounter++;

//                                            GLCashBasis = sAccCashBasis;
//                                            GLBranchCode = strBranchCode;

//                                            GL_SCode = SCode;
//                                            GL_Voucher = VchNo;
//                                            GL_Counter = bCounter;
//                                            GL_EntryDate = DateTime.Today;
//                                            GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                            GL_Reverse = false;
//                                            GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString();
//                                            GL_Account = sAccNo;
//                                            GL_Ccy = sBaseCcy;
//                                            GL_Amount = cAmount * cExchRate;
//                                            GL_Description = rowJTD["Description"].ToString();
//                                            GL_LeaseNo = vLeNo;
//                                            //GL_DocNo = rowTrans["TransCode"].ToString();
//                                            GL_DocNo = strSCode;
//                                            GL_TransferToGL = false;
//                                            GL_ErrCode = ErrCode;
//                                            GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);

//                                            if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
//                                            {
//                                                // Get HOPNO
//                                                string HOPNO, HOPBranchName;

//                                                HOPNO = "";
//                                                HOPBranchName = "";

//                                                // Get HOPNO
//                                                HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                    "HopNo",
//                                                    "tblBranch",
//                                                    "BranchCode='" + GLBranchCode + "'").ToString();

//                                                // Get HOPBranchName
//                                                HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                    "Name",
//                                                    "ACFGLMH",
//                                                    "ACC='" + HOPNO + "'").ToString();

//                                                GL_Account = HOPNO;
//                                                GL_Description = HOPBranchName;
//                                            }
//                                            int msg2 = 1;
//                                            Le_SPUpdateGLTrans(
//                                                strTransNo,
//                                                GL_SCode,
//                                                GL_Voucher,
//                                                GL_Counter,
//                                                GL_EntryDate,
//                                                GL_TransDate,
//                                                GL_Reverse,
//                                                GL_Dept,
//                                                GL_Account,
//                                                GL_Ccy,
//                                                GL_Amount,
//                                                GL_Description,
//                                                GL_LeaseNo,
//                                                GL_DocNo,
//                                                GL_TransferToGL,
//                                                GL_ErrCode,
//                                                GL_OriTransDate,
//                                                GLBranchCode,
//                                                GLCashBasis,
//                                                ref msg2);
//                                            if (msg2 == 0)
//                                            {
//                                                //WebMsgBox.Show("Update LeGLTrans Failed!");
//                                            }
//                                        }

//                                        if (vAmt < 0)
//                                            TotCr = TotCr + vAmt;
//                                        else
//                                            TotDb = TotDb + vAmt;

//                                        bCounter++;
//                                        X++;
//                                    }
//                                    int isReverse = 1;
//                                    switch (strSCode)
//                                    {
//                                        //Reverse Disburst
//                                        case "L006":
//                                            isReverse = -1;
//                                            break;
//                                    }
//                                    if (strSCode == "L006" || strSCode == "L001")
//                                    {
//                                        var fees = IDS.LeaseTrans.FeesDetail.GetFeesDetails(ContractNo);
//                                        var le = IDS.LeaseTrans.Lease.GetLease(ContractNo);
//                                        var mstFee = IDS.GeneralTable.Fees.GetFees();
//                                        if (fees.Count() > 0)
//                                        {
//                                            foreach (var fee in fees)
//                                            {
//                                                if (fee.FeesType == 6)
//                                                {
//                                                    IDSTools.UpdateCustomerDepositLease(fee.LeaseNo, fee.InAmount);
//                                                }
//                                                int msg1 = 1;
//                                                GL_SCode = SCode;
//                                                GL_Voucher = VchNo;
//                                                GL_Counter = X;
//                                                GL_EntryDate = DateTime.Today.Date;
//                                                GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_Reverse = false;
//                                                GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccPayable;
//                                                GL_Ccy = "IDR";
//                                                GL_Description = "Payable To " + mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).FeeName + " " + vLeNo;
//                                                GL_LeaseNo = vLeNo;
//                                                //GL_DocNo = rowTrans["TransCode"].ToString();
//                                                GL_DocNo = strSCode;
//                                                GL_TransferToGL = false;
//                                                GL_ErrCode = ErrCode;
//                                                GLBranchCode = "IDS";
//                                                GLCashBasis = "";
//                                                GL_Amount = fee.ThirdPartyAmt;

//                                                GL_LeaseNo = ContractNo;

//                                                if (GL_Amount != 0)
//                                                {
//                                                    Le_SPUpdateGLTrans(
//                                                    strTransNo,
//                                                    GL_SCode,
//                                                    GL_Voucher,
//                                                    GL_Counter,
//                                                    GL_EntryDate,
//                                                    GL_TransDate,
//                                                    GL_Reverse,
//                                                    GL_Dept,
//                                                    GL_Account,
//                                                    GL_Ccy,
//                                                    GL_Amount * -1 * isReverse,
//                                                    GL_Description,
//                                                    GL_LeaseNo,
//                                                    GL_DocNo,
//                                                    GL_TransferToGL,
//                                                    GL_ErrCode,
//                                                    GL_OriTransDate,
//                                                    GLBranchCode,
//                                                    GLCashBasis,
//                                                    ref msg1);
//                                                    X++;
//                                                }

//                                                GL_SCode = SCode;
//                                                GL_Voucher = VchNo;
//                                                GL_Counter = X;
//                                                GL_EntryDate = DateTime.Today.Date;
//                                                GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
//                                                GL_Reverse = false;
//                                                if ((int)le.FinanceMethod == 1)
//                                                {
//                                                    if ((int)le.LeaseType == 2)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVSaleLeaseback;
//                                                    }
//                                                    else if ((int)le.LeaseType == 3)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVInstallment;
//                                                    }
//                                                    else
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccIVFinanceLease;
//                                                    }
//                                                }
//                                                else if ((int)le.FinanceMethod == 2)
//                                                {
//                                                    if ((int)le.LeaseType == 4)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMKModalUsaha;
//                                                    }
//                                                    else
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMKSaleLeaseback;

//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    if ((int)le.LeaseType == 3)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGInstallment;
//                                                    }
//                                                    else if ((int)le.LeaseType == 5)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGFasilitasDana;
//                                                    }
//                                                    else if ((int)le.LeaseType == 1)
//                                                    {
//                                                        GL_Account = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).AccMGFinanceLease;
//                                                    }
//                                                }
//                                                GL_Ccy = "IDR";
//                                                GL_Description = mstFee.FirstOrDefault(x => x.FeeType == fee.FeesType).Description + " " + vLeNo;
//                                                GL_LeaseNo = vLeNo;
//                                                //GL_DocNo = rowTrans["TransCode"].ToString();
//                                                GL_DocNo = strSCode;
//                                                GL_TransferToGL = false;
//                                                GL_ErrCode = ErrCode;
//                                                GLBranchCode = "IDS";
//                                                GLCashBasis = "";
//                                                GL_Amount = fee.InAmount - fee.ThirdPartyAmt;

//                                                GL_LeaseNo = ContractNo;

//                                                if (GL_Amount != 0)
//                                                {
//                                                    Le_SPUpdateGLTrans(
//                                                    strTransNo,
//                                                    GL_SCode,
//                                                    GL_Voucher,
//                                                    GL_Counter,
//                                                    GL_EntryDate,
//                                                    GL_TransDate,
//                                                    GL_Reverse,
//                                                    GL_Dept,
//                                                    GL_Account,
//                                                    GL_Ccy,
//                                                    GL_Amount * -1 * isReverse,
//                                                    GL_Description,
//                                                    GL_LeaseNo,
//                                                    GL_DocNo,
//                                                    GL_TransferToGL,
//                                                    GL_ErrCode,
//                                                    GL_OriTransDate,
//                                                    GLBranchCode,
//                                                    GLCashBasis,
//                                                    ref msg1);
//                                                    X++;
//                                                }
//                                                message = msg1;
//                                                if (message < 1)
//                                                {
//                                                    trans.Rollback();
//                                                }
//                                            }
//                                        }
//                                    }


//                                }
//                                break;
//                        }
//                    }
                   
//                    trans.Commit();

                   
//                }
//                catch (Exception ex)
//                {
//                    message = 0;
//                    trans.Rollback();
//                    //WebMsgBox.Show(ex.Message);
//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }

//        public void GetvAmtErrCodePrincTerminate(//ini buat princ trminate
//      int Status,
//      string RetrieveAmt,
//      string vLeNo,
//      DataRow rowTrans,
//      double TotDb,
//      double TotCr,
//      double Portion,
//      ref double vAmt,
//      ref int ErrCode,
//      int xPer,
//      int CustGroupType,
//      int Tkey
//  )
//        {
//            string strSQL;
//            byte nPeriod;

//            double LReceivable;
//            double UnLeIncome;
//            double AdminFee;
//            double Instosupplier;
//            double Instosales;
//            double Instocust;
//            double InsurAmt;
//            double InsAmtDisc;
//            double provision;
//            double Residual;
//            double Rental;
//            double Notary;
//            double provisionFee = 0;

//            // Add by Anthony - 20160304
//            double BiSurvei = 0;
//            // End of add by Anthony - 20160304

//            double TD, TC;
//            object advarr;
//            string ContractType;

//            clsModule mdl = new clsModule();
//            ContractType = mdl.GetJCodeLeasing(1, vLeNo);

//            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
//            {
//                conn.Open();
//                try
//                {
//                    SqlDataAdapter da;
//                    DataTable dtSchedule;
//                    DataTable dtScheduleIns;
//                    DataTable dtLease;



//                    #region Disbursement
//                    // Disbursement
//                    if (Status == 1)
//                    {
//                        // Add by Yusuf(21 Mar 2011)
//                        advarr = LeaseTools.GetINSTANCE().GetFieldValue(
//                                "AdvArr",
//                                "Lease",
//                                "LeaseNo='" + vLeNo + "'"
//                            );

//                        // Jika Contract Type Join Financing
//                        // Alter - Anthony - 20220520
//                        //if (ContractType == "1L4" || ContractType == "1C4")

//                        if (ContractType == "1CIB4" // CF Investasi Installment Financing Barang
//                            || ContractType == "1CGB4" // CF Multiguna Barang 
//                            || ContractType == "1CPB4" // CF Project Barang
//                            || ContractType == "1CPJ4" // CF Project Jasa
//                            || ContractType == "1CGJ4" // CF Multiguna Jasa
//                            || ContractType == "1CMU4" // CF Modal Kerja Modal Usaha
//                            || ContractType == "1CGG4" // CF Multiguna Fasilitas Dana Barang
//                            || ContractType == "1CGS4" // CF Multiguna Fasilitas Dana Jasa
//                            || ContractType == "1LIF4" // Leasing Investasi Finance Lease
//                            || ContractType == "1LPF4" // Leasing Project Finance Lease
//                            || ContractType == "1LGF4" // Leasing Multiguna Finance Lease
//                            || ContractType == "1LIS4" // Leasing Investasi Sale & Leaseback
//                            || ContractType == "1LPS4" // Leasing Project Sales & Leaseback 
//                            || ContractType == "1LMS4" // Leasing Modal Kerja Sale & Leaseback
//                            )
//                        // End alter - Anthony - 20220520
//                        {
//                            if (Convert.ToInt16(advarr) == 1)
//                            {
//                                switch (RetrieveAmt)
//                                {
//                                    case "CFC": // Porsi Perusahaan Pembiayaan
//                                                // Alter - Anthony - 20220520
//                                                //strSQL = "select S.Rental-SJF.Rental As Rental from Schedule as S ";
//                                                //strSQL += "Inner Join ScheduleJF as SJF on S.LeaseNo=SJF.LeaseNo AND S.Period=SJF.Period ";
//                                                //strSQL += "where S.LeaseNo='" + vLeNo + "' and S.Period=1";
//                                        strSQL = "SELECT ISNULL(Rental, 0) As Rental FROM SchedulePP WHERE LeaseNo='" + vLeNo + "' AND Period=1";
//                                        // End alter - Anthony - 20220520

//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);

//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        break;
//                                    // Add - Anthony - 20220520
//                                    case "CFCJF": // Rental Porsi Funder
//                                        strSQL = "SELECT ISNULL(Rental, 0) As Rental FROM ScheduleJF WHERE LeaseNo='" + vLeNo + "' AND Period=1";

//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);

//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        break;
//                                    case "CFCFL": // Rental Porsi Full
//                                        strSQL = "SELECT ISNULL(Rental, 0) As Rental FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=1";

//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);

//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        break;
//                                }
//                            }

//                            switch (RetrieveAmt)
//                            {
//                                case "CFD": // Porsi Perusahan Pembiayaan
//                                            // Alter - Anthony - 20220520
//                                            //strSQL = "select (S.LeReceivable-SJF.LeReceivable) as LeReceivable from Schedule as S ";
//                                            //strSQL += "Inner Join ScheduleJF as SJF on S.LeaseNo=SJF.LeaseNo And S.Period=SJF.Period ";
//                                            //strSQL += "where S.LeaseNo='" + vLeNo + "' and S.Period=0 And SJF.Period=0";
//                                    strSQL = "SELECT ISNULL(LeReceivable, 0) AS LeReceivable FROM SchedulePP WHERE LeaseNo='" + vLeNo + "' AND Period=0";
//                                    // End alter - Anthony - 20220520

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    break;
//                                // Add - Anthony - 20220520
//                                case "CFDJF": // Porsi Funder / JF
//                                    strSQL = "SELECT ISNULL(LeReceivable, 0) AS LeReceivable FROM ScheduleJF WHERE LeaseNo='" + vLeNo + "' AND Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    break;
//                                case "CFDFL": // Porsi Full
//                                    strSQL = "SELECT ISNULL(LeReceivable, 0) AS LeReceivable FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    break;
//                                // End add - Anthony - 20220520

//                                case "UCF": // Porsi Perusahan Pembiayaan
//                                            // Alter - Anthony - 20220520
//                                            //strSQL = "select (S.UnLeIncome-SJF.UnLeIncome) as UnLeIncome from Schedule as S ";
//                                            //strSQL += "Inner Join ScheduleJF as SJF on S.LeaseNo=SJF.LeaseNo And S.Period=SJF.Period ";
//                                            //strSQL += "where S.LeaseNo='" + vLeNo + "' and S.Period=0 And SJF.Period=0";
//                                    strSQL = "SELECT ISNULL(UnLeIncome, 0) AS UnLeIncome FROM SchedulePP WHERE LeaseNo='" + vLeNo + "' AND Period=0";
//                                    // End alter - Anthony - 20220520

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;
//                                // Add - Anthony - 20220520
//                                case "UCFJF": // Porsi Funder
//                                    strSQL = "SELECT ISNULL(UnLeIncome, 0) AS UnLeIncome FROM ScheduleJF WHERE LeaseNo='" + vLeNo + "' AND Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;
//                                case "UCFFL":
//                                    strSQL = "SELECT ISNULL(UnLeIncome, 0) AS UnLeIncome FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;
//                                // End add - Anthony - 20220520


//                                case "AFC":
//                                    strSQL = "select ISNULL(AdminFee, 0) AS AdminFee from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AdminFee"]);
//                                    break;

//                                // For Lasing & CF
//                                case "LFT":
//                                    strSQL = "select ISNULL(Notary, 0) AS Notary from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Notary"]);
//                                    break;

//                                case "IPT":
//                                    strSQL = "select (InsurAmt + InsurIntRate)-InsDisc As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["InsurAmt"]);
//                                    break;

//                                case "IPS":
//                                    strSQL = "select ISNULL(Instosupplier, 0) AS Instosupplier from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosupplier"]);
//                                    break;

//                                case "ISM":
//                                    strSQL = "select ISNULL(Instosalesman, 0) AS Instosalesman from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosalesman"]);
//                                    break;

//                                case "IPC":
//                                    strSQL = "select ISNULL(Instocust, 0) AS Instocust from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
//                                    break;

//                                case "IPD":
//                                    strSQL = "select ISNULL(Instocust, 0) AS Instocust from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
//                                    break;
//                                // Add - Anthony - 20220520
//                                case "IPDC": // Insurance Discount
//                                    strSQL = "SELECT ISNULL(InsDisc, 0) - (ISNULL(Instosupplier, 0) + ISNULL(Instosalesman, 0) + ISNULL(Instocust, 0)) AS InsDisc from Lease where LeaseNo='" + vLeNo + "'";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["InsDisc"]);
//                                    break;
//                                // End add - Anthon - 20220520
//                                case "P":
//                                    object SisaIns;
//                                    SisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                            "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
//                                            "Lease",
//                                            "LeaseNo='" + vLeNo + "'"
//                                        );

//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]) + Convert.ToDouble(SisaIns);
//                                    break;
//                                case "PAF": // Provision Fee + Admin Fee
//                                    strSQL = "SELECT (ISNULL(ProvisionFee, 0) + ISNULL(AdminFee, 0)) AS ProvisionFee FROM Lease WHERE LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);

//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["ProvisionFee"]);
//                                    break;
//                                // Add - Anthony - 20220520
//                                case "B": // Biaya Blokir
//                                    strSQL = "SELECT ISNULL(BIBlokir, 0) AS BIBlokir FROM Lease WHERE LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["BIBlokir"]);
//                                    break;
//                                case "S":
//                                    strSQL = "SELECT ISNULL(BISurvei, 0) AS BISurvei FROM Lease WHERE LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["BISurvei"]);
//                                    break;
//                                // End add - Anthony - 20220520

//                                case "HOP":
//                                case "BPH": // Branch Payable to Holding


//                                    LReceivable = 0;
//                                    UnLeIncome = 0;
//                                    AdminFee = 0;
//                                    Instosupplier = 0;
//                                    Instosales = 0;
//                                    Instocust = 0;
//                                    InsurAmt = 0;
//                                    InsAmtDisc = 0;
//                                    provision = 0;
//                                    Notary = 0;
//                                    Rental = 0;
//                                    TD = 0;
//                                    TC = 0;

//                                    object BCFSisaIns;
//                                    BCFSisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                            "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
//                                            "Lease",
//                                            "LeaseNo='" + vLeNo + "'"
//                                        );

//                                    // Biaya Asuransi
//                                    double IPT = 0;
//                                    strSQL = "SELECT ISNULL(ISNULL(InsurAmt, 0) + ISNULL(InsurIntRate, 0), 0) AS IPT FROM Lease WHERE LeaseNo = '" + vLeNo + "'";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);

//                                    if (dtSchedule.Rows.Count == 0)
//                                    {
//                                        ErrCode = 2;
//                                    }
//                                    else
//                                    {
//                                        IPT = Convert.ToDouble(dtSchedule.Rows[0]["IPT"]);
//                                    }

//                                    advarr = LeaseTools.GetINSTANCE().GetFieldValue(
//                                        "AdvArr",
//                                        "Lease",
//                                        "LeaseNo='" + vLeNo + "'"
//                                        );

//                                    if (Convert.ToInt16(advarr) == 1)
//                                    {
//                                        strSQL = "SELECT ISNULL(Rental, 0) AS Rental FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=1";

//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);

//                                        if (dtSchedule.Rows.Count == 0)
//                                        {
//                                            ErrCode = 2;
//                                        }
//                                        else
//                                        {
//                                            Rental = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        }
//                                    }

//                                    double surveyFee = 0;

//                                    strSQL = "SELECT s.LeReceivable as LeReceivable, s.UnLeIncome as UnLeIncome, l.Notary, ISNULL(l.BISurvei, 0) AS BiSurvei, ";
//                                    strSQL += "ISNULL(l.AdminFee, 0) AS AdminFee, ";
//                                    strSQL += "l.Instosupplier, l.Instosalesman, ";
//                                    strSQL += "l.Instocust, (l.InsurAmt + l.InsurIntRate)-l.Instocust As InsurAmt, l.Provision, l.ProvisionFee, l.Residual, l.InsDisc As InsDisc ";
//                                    strSQL += "FROM Lease AS l ";
//                                    strSQL += "INNER JOIN Schedule s ON s.LeaseNo = l.LeaseNo ";
//                                    strSQL += "WHERE l.LeaseNo = '" + vLeNo + "' AND s.Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);

//                                    if (dtLease.Rows.Count == 0)
//                                    {
//                                        ErrCode = 1;
//                                    }
//                                    else
//                                    {
//                                        LReceivable = Convert.ToDouble(dtLease.Rows[0]["LeReceivable"]);
//                                        UnLeIncome = Convert.ToDouble(dtLease.Rows[0]["UnLeIncome"]);
//                                        AdminFee = Convert.ToDouble(dtLease.Rows[0]["AdminFee"]);
//                                        Instosupplier = Convert.ToDouble(dtLease.Rows[0]["Instosupplier"]);
//                                        Instosales = Convert.ToDouble(dtLease.Rows[0]["Instosalesman"]);
//                                        Instocust = Convert.ToDouble(dtLease.Rows[0]["Instocust"]);
//                                        InsurAmt = (Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]) - Convert.ToDouble(dtLease.Rows[0]["InsDisc"]));
//                                        provision = Convert.ToDouble(dtLease.Rows[0]["Provision"]) + Convert.ToDouble(BCFSisaIns);
//                                        provisionFee = Convert.ToDouble(dtLease.Rows[0]["ProvisionFee"]);
//                                        Residual = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                        Notary = Convert.ToDouble(dtLease.Rows[0]["Notary"]);
//                                        InsAmtDisc = Convert.ToDouble(dtLease.Rows[0]["InsDisc"]);
//                                        surveyFee = Convert.ToDouble(dtLease.Rows[0]["BISurvei"]);
//                                    }

//                                    TD = LReceivable;
//                                    TC = UnLeIncome + AdminFee + provisionFee + surveyFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Notary + Rental;
//                                    vAmt = TD - TC;


//                                    break;
//                            }
//                        }
//                        else // Normal
//                        {
//                            //Add by Anthony - 20151201
//                            if (Convert.ToInt16(advarr) == 1)
//                            {
//                                switch (RetrieveAmt)
//                                {
//                                    case "CFC":
//                                        strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);

//                                        //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=1";
//                                        //da = new SqlDataAdapter(strSQL, conn);
//                                        //dtScheduleIns = new DataTable();
//                                        //da.Fill(dtScheduleIns);
//                                        //if (dtScheduleIns.Rows.Count == 0)
//                                        //    ErrCode = 0;
//                                        //else
//                                        //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["Rental"]);
//                                        break;
//                                    case "LRC":
//                                        if (Convert.ToInt16(advarr) == 1)
//                                        {
//                                            strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
//                                            da = new SqlDataAdapter(strSQL, conn);
//                                            dtSchedule = new DataTable();
//                                            da.Fill(dtSchedule);
//                                            if (dtSchedule.Rows.Count == 0)
//                                                ErrCode = 2;
//                                            else
//                                                vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        }
//                                        break;
//                                }
//                            }
//                            // End of add by Anthony - 20151201

//                            #region Hasil Pindahan untuk dipakai leasing dan CF - OJK - Anthony 20151202
//                            switch (RetrieveAmt)
//                            {
//                                // Leasing 
//                                case "LRD":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);

//                                    //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtScheduleIns = new DataTable();
//                                    //da.Fill(dtScheduleIns);
//                                    //if (dtScheduleIns.Rows.Count == 0)
//                                    //    ErrCode = 0;
//                                    //else
//                                    //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["LeReceivable"]);
//                                    break;

//                                case "ULI":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);


//                                    break;
//                                #region KMF
//                                //Facility Amount
//                                case "FA":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeAmount"]);
//                                    break;
//                                //LeAmount After Advance
//                                case "LMT":
//                                    vAmt = IDSTools.GetLeAmt(vLeNo);
//                                    break;
//                                //Other Payable apabila fee tidak dipotong / tdk advance
//                                case "OPT":
//                                    vAmt = IDSTools.GetAmtNonAdvanceFee(vLeNo);
//                                    break;
//                                //PROVISION NON ADVANCE UNTUK LEASING 
//                                case "PNA":
//                                    vAmt = IDSTools.GetAmtNonAdvanceFee(vLeNo);
//                                    break;
//                                //Admin Fee
//                                case "AF":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AdminFee"]);
//                                    break;
//                                //Provision Fee
//                                case "PF":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["ProvisionFee"]);
//                                    break;
//                                //provision fee untuk selain multiguna
//                                case "PFL":
//                                    vAmt = IDSTools.GetAmtProvisionLeasing(vLeNo);
//                                    break;
//                                //Interest
//                                case "INF":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and period = 0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;
//                                //Legal Fee
//                                case "LF":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Notary"]);
//                                    break;
//                                //Other Fee
//                                case "OF":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OtherFee"]);
//                                    break;
//                                //Blokir Fee
//                                case "BF":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["BIBlokir"]);
//                                    break;
//                                //Survey Fee
//                                case "SF":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["BISurvei"]);
//                                    break;
//                                //Customer Deposit
//                                case "CD":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["DebtorDeposit"]);
//                                    break;
//                                //Thrid Fee
//                                case "TF":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["ThirdPartyFee"]);
//                                    break;
//                                //Insur
//                                case "IF":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["InsurAmt"]);
//                                    break;
//                                case "AK":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AsuransiKerugian"]);
//                                    break;
//                                case "AJ":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AsuransiJiwa"]);
//                                    break;
//                                #endregion



//                                case "DOL":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Residual"]);
//                                    break;

//                                case "AFL":
//                                    strSQL = "select ISNULL(AdminFee, 0) AS AdminFee from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AdminFee"]);
//                                    break;

//                                case "RV":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                    break;

//                                // CF 
//                                case "CFD":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);

//                                    //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtScheduleIns = new DataTable();
//                                    //da.Fill(dtScheduleIns);
//                                    //if (dtScheduleIns.Rows.Count == 0)
//                                    //    ErrCode = 0;
//                                    //else
//                                    //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["LeReceivable"]);
//                                    break;

//                                case "UCF":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);

//                                    //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtScheduleIns = new DataTable();
//                                    //da.Fill(dtScheduleIns);
//                                    //if (dtScheduleIns.Rows.Count == 0)
//                                    //    ErrCode = 0;
//                                    //else
//                                    //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "AFC":
//                                    strSQL = "select ISNULL(AdminFee, 0) AS AdminFee from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AdminFee"]);
//                                    break;

//                                // Add by Anthony - 20160321
//                                // Provision (Pengganti Admin Fee semenjak OJK)
//                                case "APF":
//                                    strSQL = "SELECT ISNULL(ProvisionFee, 0) AS ProvisionFee FROM Lease WHERE LeaseNo = '" + vLeNo + "';";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    DataTable dtProvision = new DataTable();
//                                    da.Fill(dtProvision);

//                                    if (dtProvision.Rows.Count > 0)
//                                        vAmt = Convert.ToDouble(dtProvision.Rows[0]["ProvisionFee"]);
//                                    else
//                                        vAmt = 0;
//                                    break;
//                                // End of add by Anthony - 20160321

//                                // Add by Anthony - 20160225 - Biaya Survei
//                                case "SFC":
//                                    using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
//                                    {
//                                        db.CommandText = "SELECT ISNULL(BISurvei, 0) FROM Lease WHERE LeaseNo = @leaseNo";
//                                        db.AddParameter("@leaseNo", SqlDbType.VarChar, vLeNo);
//                                        db.CommandType = CommandType.Text;
//                                        db.Open();

//                                        object biayaSurvei = db.ExecuteScalar();

//                                        if (biayaSurvei == null)
//                                        {
//                                            ErrCode = 2;
//                                        }
//                                        else
//                                        {
//                                            vAmt = Convert.ToDouble(biayaSurvei);
//                                        }
//                                    }
//                                    break;
//                                // End of add by Anthony - 20160225

//                                // For Lasing & CF

//                                case "LFT":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Notary"]);
//                                    break;

//                                case "IPT":
//                                    //strSQL = "select ((InsurAmt - InsDisc) + InsurIntRate) - InsDisc As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
//                                    // Modify by Yusuf (21 JUN 2011)
//                                    //double NProvision=0;
//                                    //object SisaHslIns;
//                                    //SisaHslIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                    //        "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
//                                    //        "Lease",
//                                    //        "LeaseNo='" + vLeNo + "'"
//                                    //    );

//                                    //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtSchedule = new DataTable();
//                                    //da.Fill(dtSchedule);
//                                    //if (dtSchedule.Rows.Count == 0)
//                                    //    ErrCode = 2;
//                                    //else
//                                    //    NProvision = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]) + Convert.ToDouble(SisaHslIns);

//                                    strSQL = "select (InsurAmt + InsurIntRate)-InsDisc As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["InsurAmt"]);
//                                    break;

//                                case "IPS":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosupplier"]);
//                                    break;

//                                case "ISM":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosalesman"]);
//                                    break;

//                                case "IPC":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
//                                    break;

//                                case "IPD":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
//                                    break;

//                                case "P":
//                                    // Alter by Anthony - 20160427
//                                    #region OLD - Sebelum Perubahan OJK
//                                    //object SisaIns;
//                                    //SisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                    //        "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
//                                    //        "Lease",
//                                    //        "LeaseNo='" + vLeNo + "'"
//                                    //    );

//                                    //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtSchedule = new DataTable();
//                                    //da.Fill(dtSchedule);
//                                    //if (dtSchedule.Rows.Count == 0)
//                                    //    ErrCode = 2;
//                                    //else
//                                    //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]) + Convert.ToDouble(SisaIns);
//                                    // End of alter by Anthony - 20160427
//                                    #endregion

//                                    strSQL = "SELECT ISNULL(Provision, 0) AS Provision FROM Lease WHERE LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]);
//                                    // End of alter by Anthony - 20160427
//                                    break;

//                                case "CTL":
//                                    LReceivable = 0;
//                                    UnLeIncome = 0;
//                                    AdminFee = 0;
//                                    Instosupplier = 0;
//                                    Instosales = 0;
//                                    Instocust = 0;
//                                    InsurAmt = 0;
//                                    provision = 0;

//                                    InsAmtDisc = 0;
//                                    Residual = 0;
//                                    Rental = 0;
//                                    Notary = 0;

//                                    TD = 0;
//                                    TC = 0;

//                                    // Add by Anthony - 20160321
//                                    provisionFee = 0;
//                                    // End of add by Anthony - 20160321

//                                    object BLSisaIns;
//                                    BLSisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                            "(ISNULL(InsDisc, 0) -(ISNULL(Instosupplier, 0) + ISNULL(Instosalesman, 0) + ISNULL(Instocust, 0))) As TotalSisaIns",
//                                            "Lease",
//                                            "LeaseNo='" + vLeNo + "'"
//                                        );

//                                    advarr = LeaseTools.GetINSTANCE().GetFieldValue(
//                                        "AdvArr",
//                                        "Lease",
//                                        "LeaseNo='" + vLeNo + "'"
//                                        );

//                                    if (Convert.ToInt16(advarr) == 1)
//                                    {
//                                        strSQL = "";
//                                        strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                        {
//                                            ErrCode = 2;
//                                        }
//                                        else
//                                        {
//                                            Rental = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                        }
//                                    }

//                                    strSQL = "";
//                                    strSQL = "select s.LeReceivable, s.UnLeIncome, l.Notary, ";
//                                    strSQL += "ISNULL(l.AdminFee, 0) AS AdminFee, l.Instosupplier, l.Instosalesman, ";
//                                    strSQL += "l.Instocust, (l.InsurAmt + l.InsurIntRate)-l.Instocust As InsurAmt, l.Provision, l.Residual, l.InsDisc As InsDisc ";
//                                    // Add by Anthony - 20160304
//                                    strSQL += ", BISurvei, ProvisionFee ";
//                                    // End of add by Anthony - 20160304
//                                    strSQL += "from Lease as l ";
//                                    strSQL += "Inner Join Schedule as s on l.LeaseNo=s.LeaseNo  ";
//                                    strSQL += "where l.LeaseNo='" + vLeNo + "' And s.Period=0";

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                    {
//                                        ErrCode = 1;
//                                    }
//                                    else
//                                    {
//                                        LReceivable = Convert.ToDouble(dtLease.Rows[0]["LeReceivable"]);
//                                        UnLeIncome = Convert.ToDouble(dtLease.Rows[0]["UnLeIncome"]);
//                                        AdminFee = Convert.ToDouble(dtLease.Rows[0]["AdminFee"]);
//                                        Instosupplier = Convert.ToDouble(dtLease.Rows[0]["Instosupplier"]);
//                                        Instosales = Convert.ToDouble(dtLease.Rows[0]["Instosalesman"]);
//                                        Instocust = Convert.ToDouble(dtLease.Rows[0]["Instocust"]);
//                                        InsurAmt = (Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]) - Convert.ToDouble(dtLease.Rows[0]["InsDisc"]));
//                                        provision = Convert.ToDouble(dtLease.Rows[0]["Provision"]) + Convert.ToDouble(BLSisaIns);
//                                        Residual = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                        Notary = Convert.ToDouble(dtLease.Rows[0]["Notary"]);
//                                        InsAmtDisc = Convert.ToDouble(dtLease.Rows[0]["InsDisc"]);
//                                        BiSurvei = Convert.ToDouble(dtLease.Rows[0]["BISurvei"]);

//                                        // Add by Anthony - 20160321
//                                        provisionFee = Convert.ToDouble(dtLease.Rows[0]["ProvisionFee"]);
//                                        // End of add by Anthony - 20160321
//                                    }

//                                    TD = LReceivable + Residual;

//                                    // Alter by Anthony - 20160321
//                                    //TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Residual + Notary + BiSurvei + Rental;
//                                    TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Residual + Notary + BiSurvei + Rental + provisionFee - Convert.ToDouble(BLSisaIns);
//                                    // End of alter by Anthony - 20160321
//                                    //TC = UnLeIncome + AdminFee + InsurAmt + Notary + Rental;
//                                    vAmt = TD - TC;
//                                    break;

//                                case "CTC":
//                                    // Alter - Anthony - 20160321
//                                    //if (ContractType != "1C2") // Pakar
//                                    if (ContractType != "1CIB2"
//                                        || ContractType != "1CPB2"
//                                        || ContractType != "1CGB2"
//                                        || ContractType != "1CPJ2"
//                                        || ContractType != "1CGJ2"
//                                        || ContractType != "1CMU2"
//                                        || ContractType != "1CGG2"
//                                        || ContractType != "1CGS2") // Pakar
//                                                                    // End alter - Anthony - 20160321
//                                    {
//                                        LReceivable = 0;
//                                        UnLeIncome = 0;
//                                        AdminFee = 0;
//                                        Instosupplier = 0;
//                                        Instosales = 0;
//                                        Instocust = 0;
//                                        InsurAmt = 0;
//                                        InsAmtDisc = 0;
//                                        provision = 0;
//                                        Notary = 0;
//                                        Rental = 0;
//                                        TD = 0;
//                                        TC = 0;

//                                        // Add by Anthony - 20160321
//                                        provisionFee = 0;
//                                        // End of add by Anthony - 20160321

//                                        object BCFSisaIns;
//                                        BCFSisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
//                                                "(ISNULL(InsDisc, 0) -(ISNULL(Instosupplier, 0) + ISNULL(Instosalesman, 0) + ISNULL(Instocust, 0))) As TotalSisaIns",
//                                                "Lease",
//                                                "LeaseNo='" + vLeNo + "'"
//                                            );

//                                        advarr = LeaseTools.GetINSTANCE().GetFieldValue(
//                                            "AdvArr",
//                                            "Lease",
//                                            "LeaseNo='" + vLeNo + "'"
//                                            );

//                                        if (Convert.ToInt16(advarr) == 1)
//                                        {
//                                            strSQL = "";
//                                            strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
//                                            da = new SqlDataAdapter(strSQL, conn);
//                                            dtSchedule = new DataTable();
//                                            da.Fill(dtSchedule);
//                                            if (dtSchedule.Rows.Count == 0)
//                                            {
//                                                ErrCode = 2;
//                                            }
//                                            else
//                                            {
//                                                Rental = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
//                                            }
//                                        }

//                                        strSQL = "";
//                                        strSQL = "select s.LeReceivable, s.UnLeIncome, l.Notary, ";
//                                        strSQL += "ISNULL(l.AdminFee, 0) AS AdminFee, l.Instosupplier, l.Instosalesman, ";
//                                        strSQL += "l.Instocust, (l.InsurAmt + l.InsurIntRate)-l.Instocust As InsurAmt, l.Provision, l.Residual, l.InsDisc As InsDisc ";
//                                        strSQL += ", BISurvei, ProvisionFee ";
//                                        strSQL += "from Lease as l ";
//                                        strSQL += "Inner Join Schedule as s on l.LeaseNo=s.LeaseNo  ";
//                                        strSQL += "where l.LeaseNo='" + vLeNo + "' And s.Period=0";

//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtLease = new DataTable();
//                                        da.Fill(dtLease);
//                                        if (dtLease.Rows.Count == 0)
//                                        {
//                                            ErrCode = 1;
//                                        }
//                                        else
//                                        {
//                                            LReceivable = Convert.ToDouble(dtLease.Rows[0]["LeReceivable"]);
//                                            UnLeIncome = Convert.ToDouble(dtLease.Rows[0]["UnLeIncome"]);
//                                            AdminFee = Convert.ToDouble(dtLease.Rows[0]["AdminFee"]);
//                                            Instosupplier = Convert.ToDouble(dtLease.Rows[0]["Instosupplier"]);
//                                            Instosales = Convert.ToDouble(dtLease.Rows[0]["Instosalesman"]);
//                                            Instocust = Convert.ToDouble(dtLease.Rows[0]["Instocust"]);
//                                            InsurAmt = (Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]) - Convert.ToDouble(dtLease.Rows[0]["InsDisc"]));
//                                            provision = Convert.ToDouble(dtLease.Rows[0]["Provision"]) + Convert.ToDouble(BCFSisaIns);
//                                            Residual = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                            Notary = Convert.ToDouble(dtLease.Rows[0]["Notary"]);
//                                            InsAmtDisc = Convert.ToDouble(dtLease.Rows[0]["InsDisc"]);
//                                            BiSurvei = Convert.ToDouble(dtLease.Rows[0]["BISurvei"]);

//                                            // Add by Anthony - 20160321
//                                            provisionFee = Convert.ToDouble(dtLease.Rows[0]["ProvisionFee"]);
//                                            // End of add by Anthony - 20160321
//                                        }

//                                        TD = LReceivable;
//                                        // Alter by Anthony - 20160321
//                                        //TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Notary + BiSurvei + Rental;
//                                        TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Notary + BiSurvei + Rental + provisionFee - Convert.ToDouble(BCFSisaIns);
//                                        // End of add by Anthony - 20160321
//                                        //TC = UnLeIncome + AdminFee + InsurAmt + Notary + Rental;
//                                        vAmt = TD - TC;
//                                    }
//                                    break;
//                                // End

//                                case "LRI":
//                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count == 0)
//                                        ErrCode = 0;
//                                    else
//                                        vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["LeReceivable"]);
//                                    break;

//                                case "LRL":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    break;

//                                case "PRL":
//                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count == 0)
//                                        ErrCode = 0;
//                                    else
//                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["OutPrinc"]);

//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = vAmt + Convert.ToDouble(dtSchedule.Rows[0]["OutPrinc"]);
//                                    break;

//                                case "UEL":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "UEI":
//                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count == 0)
//                                        ErrCode = 0;
//                                    else
//                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "AP":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["LeAmount"]) + Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]);
//                                    break;

//                                case "APL":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["LeAmount"]);
//                                    break;

//                                case "API":
//                                    strSQL = "select ((InsurAmt - InsDisc) + InsurIntRate) As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]);
//                                    break;

//                                case "OPL":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["LeAmount"]);
//                                    break;

//                                case "SD":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Security"]);
//                                    break;

//                                case "BL":
//                                    vAmt = (TotCr + TotDb) * (-1);
//                                    break;

//                                case "UNI":
//                                    vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutIncome_Int", "Lease", "LeaseNo='" + vLeNo + "'"));
//                                    break;

//                                case "UT":
//                                    vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutIncome", "Lease", "LeaseNo='" + vLeNo + "'"));
//                                    if (!string.IsNullOrEmpty(LeaseTools.GetINSTANCE().GetMaxFieldValue("Period", "dbo.Schedule", "LeaseNo='" + vLeNo + "' and Payment<>0").ToString()))
//                                        vAmt = vAmt + Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("Accrual2", "dbo.Schedule", "LeaseNo='" + vLeNo + "' and Period=" + LeaseTools.GetINSTANCE().GetMaxFieldValue("Period", "dbo.Schedule", "LeaseNo='" + vLeNo + "' and Payment<>0").ToString()));
//                                    if (Convert.ToInt16(LeaseTools.GetINSTANCE().GetFieldValue("LeStatus", "Lease", "LeaseNo='" + vLeNo + "'")) == 7)
//                                    {
//                                        //StopAccrueDate = Convert.ToDateTime(GetFieldValue("ValueDate", "Trans", "EntityNo='" + vLeNo + "' and TransCode='LL13'"));
//                                        //StopAccrueDate = Convert.ToDateTime(GetFieldValue("DueDate", "Schedule", "LeaseNo='" + vLeNo + "' and datediff(month,DueDate,'" + StopAccrueDate + "')=1"));
//                                        vAmt = vAmt + Convert.ToDouble(LeaseTools.GetINSTANCE().GetSumFieldValue("Accrual1", "Schedule", "LeaseNo='10-LS-0003183-001' and datediff(month,DueDate,'6/30/2004')>=0 and datediff(month,DueDate,'6/30/2004')<=2")) + Convert.ToDouble(LeaseTools.GetINSTANCE().GetSumFieldValue("Accrual2", "Schedule", "LeaseNo='10-LS-0003183-001' and datediff(month,DueDate,'6/30/2004')>=0 and datediff(month,DueDate,'6/30/2004')<=2"));
//                                    }
//                                    break;

//                                case "UTL":
//                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "UTI":
//                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
//                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count == 0)
//                                        ErrCode = 0;
//                                    else
//                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "RT":
//                                    vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutLR", "Lease", "LeaseNo='" + vLeNo + "'"));
//                                    break;

//                                //case "ADT":
//                                //    'Accumulated depreciation-equipment for lease
//                                //    'Operating Lease

//                                case "ARL":
//                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
//                                    break;

//                                case "AUL":
//                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
//                                    break;

//                                case "CFI":
//                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "LRT":
//                                case "UIT":
//                                case "LIT":
//                                case "OIT":
//                                case "PT":
//                                    ErrCode = 6;
//                                    break;

//                                case "OP":
//                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutPrinc"]);
//                                    break;

//                                case "UI":
//                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;

//                                case "OLR":
//                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    break;

//                                case "RLI":
//                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
//                                        nPeriod = 0;
//                                    else
//                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                    {
//                                        int x = 0;
//                                        for (byte I = Convert.ToByte(rowTrans["InnerCode"]); I < Convert.ToByte(rowTrans["OuterCode"]); I++)
//                                        {
//                                            vAmt = vAmt + Convert.ToDouble(dtSchedule.Rows[x]);
//                                            x++;
//                                        }
//                                    }
//                                    break;

//                                case "GL":
//                                    if (Convert.ToInt16(LeaseTools.GetINSTANCE().GetFieldValue("NovasiStatus", "dbo.Lease", "LeaseNo='" + vLeNo + "'")) == 0)
//                                        vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("GainLoss", "dbo.tTermination", "LeaseNo='" + vLeNo + "' and year(ProcessDate)='" + Convert.ToDateTime(rowTrans["ValueDate"]).ToString("yyyy") + "' and month(ProcessDate)='" + Convert.ToDateTime(rowTrans["ValueDate"]).ToString("mm") + "' and day(ProcessDate)='" + Convert.ToDateTime(rowTrans["ValueDate"]).ToString("dd") + "'"));
//                                    break;

//                                case "GLN":
//                                    vAmt = Convert.ToDouble(rowTrans["Amount"]);
//                                    vAmt = vAmt + Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutIncome_Int", "Lease", "LeaseNo='" + vLeNo + "'"));
//                                    vAmt = vAmt - Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutLR_Int", "Lease", "LeaseNo='" + vLeNo + "'"));
//                                    break;

//                                case "LRS":
//                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0) as sumLR from Schedule where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count > 0)
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["sumLR"]);

//                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0)) as sumLR from ScheduleInsur where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count > 0)
//                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["sumLR"]);
//                                    break;

//                                case "UES":
//                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0) as sumLR from Schedule where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count > 0)
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["sumLR"]);

//                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0)) as sumLR from ScheduleInsur where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtScheduleIns = new DataTable();
//                                    da.Fill(dtScheduleIns);
//                                    if (dtScheduleIns.Rows.Count > 0)
//                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["sumLR"]);
//                                    vAmt = vAmt - Convert.ToDouble(rowTrans["Amount"]);
//                                    break;

//                                case "UER":
//                                    vAmt = Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescAfter2"].ToString()) ? "0" : rowTrans["DescAfter2"].ToString()) -
//                                         Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescBefore2"].ToString()) ? "0" : rowTrans["DescBefore2"].ToString());
//                                    break;

//                                case "LRR":
//                                    vAmt = Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescAfter3"].ToString()) ? "0" : rowTrans["DescAfter3"].ToString()) -
//                                         Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescBefore3"].ToString()) ? "0" : rowTrans["DescBefore3"].ToString());
//                                    break;

//                                case "OPR":
//                                    vAmt = Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescAfter1"].ToString()) ? "0" : rowTrans["DescAfter1"].ToString()) -
//                                         Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescBefore1"].ToString()) ? "0" : rowTrans["DescBefore1"].ToString());
//                                    break;

//                                default:
//                                    //vAmt = vAmt - Convert.ToDouble(rowTrans["Amount"]);
//                                    break;
//                            }
//                            #endregion




//                            //Add By : Tomy (20 Agust 2011)
//                            //For Receivable Assignment
//                            if (ContractType == "1L6" || ContractType == "1C6")
//                            {
//                                switch (RetrieveAmt)
//                                {
//                                    case "HOP":
//                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutPrincipal"]);
//                                        break;

//                                    case "DOL":
//                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(0);
//                                        break;

//                                    case "ULI":
//                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutIncome"]);
//                                        break;

//                                    case "LR":
//                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutLR"]);
//                                        break;

//                                    case "RV":
//                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(0);
//                                        break;
//                                }
//                            }
//                            //End Add
//                            //}
//                        }
//                    }
//                    #endregion

//                    #region Partial Termination
//                    // Partial Termination
//                    else if (Status == 2)
//                    {
//                        object period, BungaDenda, hslp;
//                        period = xPer;
//                        hslp = Convert.ToInt32(xPer - 1);

//                        BungaDenda = LeaseTools.GetINSTANCE().GetFieldValue(
//                                "CurrInt",
//                                "LetTermination",
//                                "LeaseNo='" + vLeNo + "' AND Type=0 AND Period='" + Convert.ToInt32(period) + "'"
//                            );

//                        switch (RetrieveAmt)
//                        {
//                            // Leasing
//                            case "ULI":
//                                //Commented By Marsolim 6 December 2011    
//                                //strSQL = "select abs(Adjustment) As Adjustment From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                //da = new SqlDataAdapter(strSQL, conn);
//                                //dtSchedule = new DataTable();
//                                //da.Fill(dtSchedule);
//                                //if (dtSchedule.Rows.Count == 0)
//                                //    ErrCode = 2;
//                                //else
//                                //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
//                                //End Comment
//                                //Added By Marsolim 6 Dec 2011
//                                vAmt = Math.Abs(Convert.ToDouble(rowTrans["DescAfter3"]));
//                                //End Add
//                                break;

//                            case "LRL":
//                                #region Commented
//                                //if (Convert.ToDouble(BungaDenda) == 0)
//                                //{
//                                //    //strSQL  = "SELECT (SELECT DescBefore4 From LeTrans ";
//                                //    //strSQL += "WHERE EntityNo='" + vLeNo + "' AND SubEntityNo='" + Convert.ToInt32(period) + "') ";
//                                //    ////strSQL += " - (LeReceivable) + (Rental) ";
//                                //    //strSQL += " - (LeReceivable + (Rental-Payment) ";
//                                //    //strSQL += "As Hsl ";
//                                //    //strSQL += "From Schedule ";
//                                //    //strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

//                                //    strSQL = "SELECT ((SELECT LeReceivable From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period ='" + (Convert.ToInt32(period) - 1) + "') - LeReceivable) - Payment As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

//                                //    da = new SqlDataAdapter(strSQL, conn);
//                                //    dtSchedule = new DataTable();
//                                //    da.Fill(dtSchedule);
//                                //    if (dtSchedule.Rows.Count == 0)
//                                //        ErrCode = 2;
//                                //    else
//                                //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                //}
//                                //else
//                                //{
//                                //    //strSQL = "SELECT (LeReceivable - (SELECT DescBefore4 From LeTrans WHERE EntityNo='" + vLeNo + "' AND SubEntityNo='" + Convert.ToInt32(period) + "')) * -1 As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                //    strSQL  = "SELECT (SELECT DescBefore4 From LeTrans ";
//                                //    strSQL += "WHERE EntityNo='" + vLeNo + "' AND SubEntityNo='" + Convert.ToInt32(period) + "') - (LeReceivable) ";
//                                //    strSQL += "As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

//                                //    da = new SqlDataAdapter(strSQL, conn);
//                                //    dtSchedule = new DataTable();
//                                //    da.Fill(dtSchedule);
//                                //    if (dtSchedule.Rows.Count == 0)
//                                //        ErrCode = 2;
//                                //    else
//                                //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                //}
//                                #endregion

//                                //Altered By Yusuf 24 Okt 2011
//                                //strSQL = "SELECT ((SELECT LeReceivable From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period ='" + (Convert.ToInt32(period) - 1) + "') - LeReceivable) - Payment As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

//                                //Altered By Marsolim 6 December 2011
//                                strSQL = "SELECT (" + Convert.ToDouble(rowTrans["DescBefore4"]) + " - LeReceivable) As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                //End Alter

//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);

//                                break;

//                            // CF
//                            case "LIC":
//                                //Commented By Marsolim 6 Dec 2011
//                                //strSQL = "select abs(Adjustment) As Adjustment From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                //da = new SqlDataAdapter(strSQL, conn);
//                                //dtSchedule = new DataTable();
//                                //da.Fill(dtSchedule);
//                                //if (dtSchedule.Rows.Count == 0)
//                                //    ErrCode = 2;
//                                //else
//                                //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
//                                //End Comment

//                                //Added By Marsolim 6 Dec 2011
//                                vAmt = Math.Abs(Convert.ToDouble(rowTrans["DescAfter3"]));
//                                //End Add
//                                break;

//                            case "CFR":


//                                //Altered By Yusuf 24 Okt 2011
//                                //strSQL = "SELECT ((SELECT LeReceivable From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period ='" + (Convert.ToInt32(period) - 1) + "') - LeReceivable) - Payment As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

//                                //Altered By Marsolim 6 December 2011
//                                strSQL = "SELECT (" + Convert.ToDouble(rowTrans["DescBefore4"]) + " - LeReceivable) As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                //End Alter

//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                break;

//                            // For All
//                            case "DOL":
//                                //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                strSQL = "Select (AssetOutPrinc / TotalPrincipal) * ";
//                                strSQL += "(Select Security From Lease Where LeaseNo='" + vLeNo + "') As S ";
//                                strSQL += "From LetTermination ";
//                                strSQL += "Where LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["S"]);
//                                break;

//                            case "RV":
//                                //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                strSQL = "Select (AssetOutPrinc / TotalPrincipal) * ";
//                                strSQL += "(Select Residual From Lease Where LeaseNo='" + vLeNo + "') As D ";
//                                strSQL += "From LetTermination ";
//                                strSQL += "Where LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtLease = new DataTable();
//                                da.Fill(dtLease);
//                                if (dtLease.Rows.Count == 0)
//                                    ErrCode = 1;
//                                else
//                                    vAmt = Convert.ToDouble(dtLease.Rows[0]["D"]);
//                                break;

//                            case "OVI":
//                                strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=0 and Period='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);
//                                break;

//                            case "GLS":
//                                strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=0 and Period='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["GainLoss"]);
//                                break;

//                            case "CTL":
//                                strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=0 ";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                break;

//                            case "CTC":
//                                strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=0 ";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                break;
//                        }
//                    }
//                    #endregion

//                    #region Early Termination
//                    // Early Terminate
//                    else if (Status == 3)
//                    {
//                        object period, hslp, BungaDenda;
//                        int TotPeriod;

//                        period = xPer;
//                        hslp = Convert.ToInt32(xPer - 1);

//                        BungaDenda = LeaseTools.GetINSTANCE().GetFieldValue(
//                                "CurrInt",
//                                "LetTermination",
//                                "LeaseNo='" + vLeNo + "' AND Type=1 AND Period='" + Convert.ToInt32(period) + "'"
//                            );

//                        TotPeriod = Convert.ToInt32(LeaseTools.GetINSTANCE().GetFieldValue(
//                                "COUNT(*)-3",
//                                "Schedule",
//                                "LeaseNo='" + vLeNo + "'"
//                            ));

//                        if (TotPeriod == Convert.ToInt32(period))
//                        {
//                            double vCurrIntAmt = 0;
//                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                            da = new SqlDataAdapter(strSQL, conn);
//                            dtSchedule = new DataTable();
//                            da.Fill(dtSchedule);
//                            if (dtSchedule.Rows.Count == 0)
//                                ErrCode = 2;
//                            else
//                                vCurrIntAmt = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);

//                            double vCFLeaseIncomeAmt = 0;
//                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                            da = new SqlDataAdapter(strSQL, conn);
//                            dtSchedule = new DataTable();
//                            da.Fill(dtSchedule);
//                            if (dtSchedule.Rows.Count == 0)
//                                ErrCode = 2;
//                            else
//                                vCFLeaseIncomeAmt = IDS.Tool.GeneralHelper.NullToDouble(dtSchedule.Rows[0]["CFLeaseIncome"], 0);

//                            switch (RetrieveAmt)
//                            {
//                                // Leasing
//                                case "ULI":
//                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    //Altered By Marsolim 24 Okt 2011
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt;
//                                    vAmt = 0;
//                                    break;

//                                case "LRL":
//                                    //if (Convert.ToDouble(BungaDenda) == 0)
//                                    //{
//                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(hslp) + "'";
//                                    //    da = new SqlDataAdapter(strSQL, conn);
//                                    //    dtSchedule = new DataTable();
//                                    //    da.Fill(dtSchedule);
//                                    //    if (dtSchedule.Rows.Count == 0)
//                                    //        ErrCode = 2;
//                                    //    else
//                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    //}
//                                    //else
//                                    //{
//                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                    //    da = new SqlDataAdapter(strSQL, conn);
//                                    //    dtSchedule = new DataTable();
//                                    //    da.Fill(dtSchedule);
//                                    //    if (dtSchedule.Rows.Count == 0)
//                                    //        ErrCode = 2;
//                                    //    else
//                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    //}
//                                    if (Convert.ToDouble(BungaDenda) == 0)
//                                    {
//                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    }
//                                    else
//                                    {
//                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]) - (Convert.ToDouble(BungaDenda));
//                                    }
//                                    break;

//                                case "LIL":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        //vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCurrIntAmt + vCFLeaseIncomeAmt;
//                                        //Altered By Marsolim 24 Okt 2011
//                                        vAmt = vCurrIntAmt + vCFLeaseIncomeAmt;
//                                    //vAmt = 0;
//                                    break;

//                                //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                //da = new SqlDataAdapter(strSQL, conn);
//                                //dtSchedule = new DataTable();
//                                //da.Fill(dtSchedule);
//                                //if (dtSchedule.Rows.Count == 0)
//                                //    ErrCode = 2;
//                                //else
//                                //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeIncome"]);

//                                // CF
//                                case "LIC":
//                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    //Altered By Marsolim 24 Okt 2011
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt;
//                                    //vAmt = 0;
//                                    break;

//                                case "CFR":
//                                    //if (Convert.ToDouble(BungaDenda) == 0)
//                                    //{
//                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(hslp) + "'";
//                                    //    da = new SqlDataAdapter(strSQL, conn);
//                                    //    dtSchedule = new DataTable();
//                                    //    da.Fill(dtSchedule);
//                                    //    if (dtSchedule.Rows.Count == 0)
//                                    //        ErrCode = 2;
//                                    //    else
//                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    //}
//                                    //else
//                                    //{
//                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                    //    da = new SqlDataAdapter(strSQL, conn);
//                                    //    dtSchedule = new DataTable();
//                                    //    da.Fill(dtSchedule);
//                                    //    if (dtSchedule.Rows.Count == 0)
//                                    //        ErrCode = 2;
//                                    //    else
//                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
//                                    //}
//                                    if (Convert.ToDouble(BungaDenda) == 0)
//                                    {
//                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    }
//                                    else
//                                    {
//                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]) - (Convert.ToDouble(BungaDenda));
//                                    }

//                                    break;

//                                case "CFI":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        //vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCurrIntAmt + vCFLeaseIncomeAmt;
//                                        //Altered by Marsolim 24 Okt 2011
//                                        vAmt = vCurrIntAmt + vCFLeaseIncomeAmt;
//                                    break;
//                                ////strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                ////da = new SqlDataAdapter(strSQL, conn);
//                                ////dtSchedule = new DataTable();
//                                ////da.Fill(dtSchedule);
//                                ////if (dtSchedule.Rows.Count == 0)
//                                ////    ErrCode = 2;
//                                ////else
//                                ////    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeIncome"]);
//                                //vAmt = 0;
//                                //break;

//                                // For All
//                                case "DOL":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Security"]);
//                                    break;

//                                case "RV":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                    break;

//                                case "OVI":
//                                    vAmt = 0;
//                                    //strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtSchedule = new DataTable();
//                                    //da.Fill(dtSchedule);
//                                    //if (dtSchedule.Rows.Count == 0)
//                                    //    ErrCode = 2;
//                                    //else
//                                    //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);
//                                    break;

//                                case "GLS":
//                                    //strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                                    //da = new SqlDataAdapter(strSQL, conn);
//                                    //dtSchedule = new DataTable();
//                                    //da.Fill(dtSchedule);
//                                    //if (dtSchedule.Rows.Count == 0)
//                                    //    ErrCode = 2;
//                                    //else
//                                    //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["GainLoss"]);
//                                    vAmt = 0;
//                                    break;

//                                case "ADF":
//                                    strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AllowancedblfullAmt"]);
//                                    break;

//                                case "CTL":
//                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    break;

//                                case "CTC":
//                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    break;
//                            }
//                        }
//                        else
//                        {
//                            double vCurrIntAmt1 = 0;
//                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                            da = new SqlDataAdapter(strSQL, conn);
//                            dtSchedule = new DataTable();
//                            da.Fill(dtSchedule);
//                            if (dtSchedule.Rows.Count == 0)
//                                ErrCode = 2;
//                            else
//                                vCurrIntAmt1 = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);


//                            double vCFLeaseIncomeAmt1 = 0;
//                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                            da = new SqlDataAdapter(strSQL, conn);
//                            dtSchedule = new DataTable();
//                            da.Fill(dtSchedule);
//                            if (dtSchedule.Rows.Count == 0)
//                                ErrCode = 2;
//                            else
//                                vCFLeaseIncomeAmt1 = IDS.Tool.GeneralHelper.NullToDouble(dtSchedule.Rows[0]["CFLeaseIncome"], 0);
//                            DateTime datePelunasan = IDS.Tool.GeneralHelper.NullToDateTime(dtSchedule.Rows[0]["ProcessDate"], DateTime.MinValue);


//                            int periodadd1 = Convert.ToInt32(period) + 1;
//                            int periodadd2 = Convert.ToInt32(period) + 2;
//                            DateTime dateDueSc = DateTime.Now;


//                            strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + periodadd1 + "'";
//                            da = new SqlDataAdapter(strSQL, conn);
//                            dtSchedule = new DataTable();
//                            da.Fill(dtSchedule);
//                            if (dtSchedule.Rows.Count == 0)
//                                ErrCode = 2;
//                            else
//                                dateDueSc = Convert.ToDateTime(dtSchedule.Rows[0]["DueDate"]);


//                            switch (RetrieveAmt)
//                            {
//                                #region KMF
//                                //Accrual 1 / bunga sampai tanggal due date / Acrrual Due Date
//                                case "ADT":
//                                    if (datePelunasan.Date == dateDueSc.Date)
//                                    {
//                                        strSQL = "select Accrual1 from Schedule where LeaseNo='" + vLeNo + "' and Period =" + periodadd1 + "";
//                                        da = new SqlDataAdapter(strSQL, conn);
//                                        dtSchedule = new DataTable();
//                                        da.Fill(dtSchedule);
//                                        if (dtSchedule.Rows.Count == 0)
//                                            ErrCode = 2;
//                                        else
//                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Accrual1"]);
//                                    }
//                                    else
//                                    {
//                                        vAmt = 0;
//                                    }

//                                    break;
//                                //Piutang unearned bunga
//                                case "PUB":
//                                    strSQL = "select UnLeIncome from Schedule where LeaseNo='" + vLeNo + "' and Period =" + Convert.ToInt32(period) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
//                                    break;
//                                //OFT = OUTSTANDING FROM TERMINATION
//                                case "OFT":
//                                    strSQL = "SELECT OutPrinc As Hsl From LetTermination ";
//                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    break;
//                                //Penalty Amount udah ada == "GLS"
//                                //Bunga dr termination / Income From Termination
//                                case "IFT":
//                                    strSQL = "SELECT CurrInt As Hsl From LetTermination ";
//                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    break;
//                                //Total nilai yang dibayar udah ada == "CTC"
//                                #endregion  
//                                // Leasing
//                                case "ULI":
//                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    //Altered By Marsolim 24 Okt 2011
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt1;
//                                    break;

//                                case "LRL":

//                                    strSQL = "Select (LeReceivable + Rental - Payment) as LeRec From Schedule Where LeaseNo='" + vLeNo +
//                                        "' And Period='" + (period) + "'";
//                                    //End Alter

//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeRec"]);

//                                    break;

//                                case "LIL":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else

//                                        vAmt = vCurrIntAmt1 + vCFLeaseIncomeAmt1;
//                                    break;

//                                // CF
//                                case "LIC":
//                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
//                                    //Altered By Marsolim 24 Okt 2011
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt1;
//                                    break;

//                                case "CFR":
//                                    strSQL = "select LeReceivable-(Select Payment From Schedule Where LeaseNo='" + vLeNo +
//                                    "' And Period='" + (period) + "') As LeRec from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(hslp) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeRec"]);

//                                    break;

//                                case "CFI":
//                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else

//                                        vAmt = vCurrIntAmt1 + vCFLeaseIncomeAmt1;
//                                    break;

//                                // For All
//                                case "DOL":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Security"]);
//                                    break;

//                                case "RV":
//                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtLease = new DataTable();
//                                    da.Fill(dtLease);
//                                    if (dtLease.Rows.Count == 0)
//                                        ErrCode = 1;
//                                    else
//                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
//                                    break;

//                                case "OVI":
//                                    vAmt = 0;

//                                    break;

//                                case "GLS":
//                                    strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["GainLoss"]);
//                                    break;

//                                case "ADF":
//                                    strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AllowancedblfullAmt"]);
//                                    break;

//                                case "CTL":
//                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    break;

//                                case "CTC":
//                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
//                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
//                                    da = new SqlDataAdapter(strSQL, conn);
//                                    dtSchedule = new DataTable();
//                                    da.Fill(dtSchedule);
//                                    if (dtSchedule.Rows.Count == 0)
//                                        ErrCode = 2;
//                                    else
//                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
//                                    break;
//                            }
//                        }
//                    }
//                    #endregion

//                    #region Floating
//                    // Floating
//                    else if (Status == 4)
//                    {
//                        // Leasing
//                        object period;
//                        period = xPer;

//                        switch (RetrieveAmt)
//                        {
//                            case "LRL":
//                            case "ULI":
//                            case "CRC":
//                            case "UCF":
//                                strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = (dtSchedule.Rows[0]["Adjustment"] == DBNull.Value ? 0D : Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]));
//                                break;
//                        }
//                    }
//                    #endregion

//                    #region Rescheduling
//                    // Rescheduling
//                    else if (Status == 21)
//                    {
//                        // Leasing
//                        object period;
//                        period = xPer;

//                        switch (RetrieveAmt)
//                        {
//                            case "LRL":
//                            case "ULI":
//                            case "CRC":
//                            case "UCF":
//                                strSQL = "select RescAdjust from LeRescheduling where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["RescAdjust"]);


//                                break;

//                            // Add by Anthony - 20160321
//                            case "APF":
//                                strSQL = "SELECT ISNULL(ProvisionFee, 0) AS ProvisionFee FROM Lease WHERE LeaseNo = @leaseno;";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);

//                                if (dtSchedule.Rows.Count > 0)
//                                {
//                                    vAmt = 0;
//                                }
//                                else
//                                {
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["ProvisionFee"]);

//                                }
//                                break;
//                                // End of add by Anthony - 20160321

//                        }
//                    }
//                    #endregion

//                    #region Contract Interest Accrued For JF
//                    // Contract Interest Accrued
//                    else if (Status == 24)
//                    {

//                        if (rowTrans["Amount"] == null)
//                            ErrCode = 2;
//                        else
//                            vAmt = Convert.ToDouble(rowTrans["Amount"]);
//                    }
//                    #endregion

//                    #region Write Off
//                    // Contract WriteOff
//                    else if (Status == 29)
//                    {

//                        object period;
//                        period = xPer;

//                        switch (RetrieveAmt)
//                        {
//                            //LAST OS PRINCIPAL
//                            case "OUP":
//                                strSQL = "select top 1 * from leoutstanding where LeaseNo='" + vLeNo + "' order by period desc";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = (dtSchedule.Rows[0]["OutPrincipal"] == DBNull.Value ? 0D : Convert.ToDouble(dtSchedule.Rows[0]["OutPrincipal"]));
//                                break;
//                            // LAST OS INCOME
//                            case "OUI":
//                                strSQL = "select top 1 * from leoutstanding where LeaseNo='" + vLeNo + "' order by period desc";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = (dtSchedule.Rows[0]["OutIncome"] == DBNull.Value ? 0D : Convert.ToDouble(dtSchedule.Rows[0]["OutIncome"]));
//                                break;
//                            //SISA ACCRUAL
//                            case "UEI":
//                                strSQL = "select sum(accrual1+accrual2) as total from Schedule where rental - payment > 0 and LeaseNo='" + vLeNo + "' and RescAdjustOld = 1";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = (dtSchedule.Rows[0]["total"] == DBNull.Value ? 0D : Convert.ToDouble(dtSchedule.Rows[0]["total"]));
//                                break;
//                            //TOTAL OUTSTANDING
//                            case "TOU":
//                                strSQL = "select top 1 * from leoutstanding where LeaseNo='" + vLeNo + "' order by period desc";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                    ErrCode = 2;
//                                else
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutPrincipal"]) + Convert.ToDouble(dtSchedule.Rows[0]["OutIncome"]);
//                                break;
//                        }
//                    }
//                    #endregion

//                    #region Principal Termination
//                    // Rescheduling
//                    else if (Status == 30)
//                    {
//                        // Leasing
//                        object period;
//                        period = xPer;

//                        switch (RetrieveAmt)
//                        {
//                            case "PTB"://POKOK PEMBIAYAAN / Yang dibayar Principal nya berapa
//                                strSQL = "select PrincRepaymentAMT from PrincTermination where tkey='" + Tkey + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                {
//                                    ErrCode = 2;
//                                    vAmt = 0;
//                                }
//                                else
//                                {
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["PrincRepaymentAMT"]);
//                                }
//                                break;
//                            case "PTC"://PIUTANG BUNGA /Interest Rec Baru/ULI baru + rescAdjust
//                                double UlIPeriod0 = 0;
//                                UlIPeriod0 = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "UnLeIncome",
//                                                        "Schedule",
//                                                        "leaseno='" + vLeNo + "' and Period ='" + Convert.ToInt16(period) + "'"));

//                                strSQL = "select RescAdjust from SChedule where Leaseno='" + vLeNo + "' and Period ='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                {
//                                    ErrCode = 2;
//                                    vAmt = 0;
//                                }
//                                else
//                                {
//                                    vAmt = UlIPeriod0 + Convert.ToDouble(dtSchedule.Rows[0]["RescAdjust"]);
//                                }
//                                break;
//                            case "PTD"://UNEARNED BARU / ULI PERIOD Bayar

//                                strSQL = "select UnLeincome from SChedule where Leaseno='" + vLeNo + "' and Period ='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                {
//                                    ErrCode = 2;
//                                    vAmt = 0;
//                                }
//                                else
//                                {
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeincome"]);
//                                }
//                                break;
//                            case "PTE"://Unearned Lama//Uli lama period Bayar + AdjustIntPeriodBayar
//                                double UlIPeriodBayar = 0;
//                                UlIPeriod0 = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue(
//                                                        "UnLeIncome",
//                                                        "Schedule",
//                                                        "leaseno='" + vLeNo + "' and Period ='" + Convert.ToInt16(period) + "'"));
//                                strSQL = "select rescAdjust from SChedule where Leaseno='" + vLeNo + "' and Period ='" + Convert.ToInt32(period) + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                {
//                                    ErrCode = 2;
//                                    vAmt = 0;
//                                }
//                                else
//                                {
//                                    vAmt = UlIPeriodBayar + Convert.ToDouble(dtSchedule.Rows[0]["rescAdjust"]);
//                                }
//                                break;


//                            case "PTF"://JUMALH INTEREST LAMA / Interest Rec yang lama/ULI period 0
//                                strSQL = "select UnLeincome from SChedule where LeaseNo='" + vLeNo + "' and Period='0'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                {
//                                    ErrCode = 2;
//                                    vAmt = 0;
//                                }
//                                else
//                                {
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["RescAdjust"]);
//                                }
                                    


//                                break;
//                            case "PTG"://JUMALH Penalty
//                                strSQL = "select PrincRepaymentPenaltyAMT from SChedule where tkey='" + Tkey + "'";
//                                da = new SqlDataAdapter(strSQL, conn);
//                                dtSchedule = new DataTable();
//                                da.Fill(dtSchedule);
//                                if (dtSchedule.Rows.Count == 0)
//                                {
//                                    ErrCode = 2;
//                                    vAmt = 0;
//                                }
//                                else
//                                {
//                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["PrincRepaymentPenaltyAMT"]);
//                                }
//                                break;

//                        }
//                    }
//                    #endregion
//                }
//                catch (Exception ex)
//                {

//                }
//                finally
//                {
//                    conn.Close();
//                }
//            }
//        }

       
//    }
//}
