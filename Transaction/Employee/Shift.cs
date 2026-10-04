using IDS.DataAccess;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Transaction.Employee
{
    public class Shift
    {
        public int ShiftID { get; set; }
        public string Branch { get; set; }
        public string ShiftUser { get; set; }
        public DateTime StartShift { get; set; }
        public string StartShiftString { get; set; }
        public DateTime CloseShift { get; set; }
        public string CloseShiftString { get; set; }
        public decimal KasAwal { get; set; }
        public decimal KasAkhir { get; set; }
        public string OperatorID { get; set; }
        public DateTime LastUpdate { get; set; }

        public Shift()
        {

        }
        
        public static List<Shift> GetDataShift(DateTime Dates, string Outlet)
        {
            List<Shift> data = new List<Shift>();

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select Shift.*,tblbranch.branchcode+' - '+tblBranch.BranchName as branchname from Shift inner join tblbranch on tblbranch.BranchCode=shift.Branch where Shift.Branch=@Outlet and CAST(Shift.startShift AS DATE)=CAST(@StartShift AS DATE)";
                db.AddParameter("@Outlet", System.Data.SqlDbType.VarChar, Outlet);
                db.AddParameter("@StartShift", System.Data.SqlDbType.DateTime, Dates);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Shift details = new Shift();
                            details.ShiftID = IDS.Tool.GeneralHelper.NullToInt(dr["ShiftID"],0);
                            details.Branch = IDS.Tool.GeneralHelper.NullToString(dr["branchname"]);
                            details.ShiftUser = IDS.Tool.GeneralHelper.NullToString(dr["ShiftUser"]);
                            details.StartShift = IDS.Tool.GeneralHelper.NullToDateTime(dr["StartShift"],DateTime.MinValue);
                            details.CloseShift = IDS.Tool.GeneralHelper.NullToDateTime(dr["CloseShift"], DateTime.MinValue);
                            if (details.StartShift == DateTime.MinValue)
                            {
                                details.StartShiftString = "";
                            }
                            else
                            {
                                details.StartShiftString = details.StartShift.ToString("dd-MMM-yyyy");
                            }
                            if (details.CloseShift == DateTime.MinValue)
                            {
                                details.CloseShiftString = "";
                            }
                            else
                            {
                                details.CloseShiftString = details.CloseShift.ToString("dd-MMM-yyyy");
                            }
                            details.KasAwal = IDS.Tool.GeneralHelper.NullToDecimal(dr["KasAwal"],0);
                            details.KasAkhir = IDS.Tool.GeneralHelper.NullToDecimal(dr["KasAkhir"], 0);
                            data.Add(details);
                        }


                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return data;
        }

        public int InsUpDel(DateTime Dates,int Type, string User, string Outlet,decimal KasAwal,decimal KasAkhir)
        {
            int result = 0;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    DateTime finalDate = Dates.Date.Add(DateTime.Now.TimeOfDay);

                    cmd.Open();
                    cmd.CommandText = "InsShift";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@Dates", System.Data.SqlDbType.DateTime, finalDate);

                    cmd.AddParameter("@Outlet", System.Data.SqlDbType.VarChar, Outlet);
                    cmd.AddParameter("@KasAwal", System.Data.SqlDbType.Money, KasAwal);

                    cmd.AddParameter("@KasAkhir", System.Data.SqlDbType.Money, KasAkhir);
                    cmd.AddParameter("@UserID", System.Data.SqlDbType.VarChar, User);
                    cmd.AddParameter("@Type", System.Data.SqlDbType.Int, Type);


                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
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

        public static bool CekShift(DateTime Dates, string Outlet, int type)
        {
            bool result = false;
            int allow = 0;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.Open();
                    cmd.CommandText = "CekShift";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    
                    cmd.AddParameter("@Dates", System.Data.SqlDbType.DateTime, Dates);
                    cmd.AddParameter("@Type", System.Data.SqlDbType.Int, type);
                    cmd.AddParameter("@Outlet", System.Data.SqlDbType.VarChar, Outlet);

                    cmd.ExecuteReader();
                    using (Microsoft.Data.SqlClient.SqlDataReader dr = cmd.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {

                                allow = IDS.Tool.GeneralHelper.NullToInt(dr["Result"], 0);
                            }


                        }

                        if (!dr.IsClosed)
                            dr.Close();
                    }
                    cmd.Close();
                }
                catch
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    throw;
                }
               
            }
            if (allow == 1)
            {
                result = true;
            }

            return result;
        }

    }
}
