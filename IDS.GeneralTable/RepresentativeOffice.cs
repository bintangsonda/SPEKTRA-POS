using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Tool;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class RepresentativeOffice
    {
        [Display(Name = "Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Area code is required")]
        [MaxLength(5), StringLength(5)]
        public string RepCode { get; set; }

        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Area name is required")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Display(Name = "BranchCode")]
        [MaxLength(5)]
        public string BranchCode { get; set; }

        [Display(Name = "Created By")]
        public string EntryUser { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        [Display(Name = "Created Date")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Operator ID")]
        public string OperatorID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; }

        public RepresentativeOffice()
        {

        }



        
        public static RepresentativeOffice GetDataKarep(string BranchCode, string Karep)
        {
            RepresentativeOffice area = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select * from lekarep where BranchCode='"+ BranchCode + "' and KarepCode='"+ Karep + "'";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        area = new RepresentativeOffice();
                        area.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                        area.RepCode = IDS.Tool.GeneralHelper.NullToString(dr["KaRepCode"]);

                        area.Name = IDS.Tool.GeneralHelper.NullToString(dr["KarepName"]);
                        area.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["UpdatedBy"]);
                        area.LastUpdate = IDS.Tool.GeneralHelper.NullToDateTime(dr["UpdatedDate"], DateTime.Now);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return area;
        }


        public static List<RepresentativeOffice> GetRepOff()
        {
            List<IDS.GeneralTable.RepresentativeOffice> list = new List<RepresentativeOffice>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select * from lekarep";
                db.CommandType = System.Data.CommandType.Text;
             
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            RepresentativeOffice area = new RepresentativeOffice();
                            area.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            area.RepCode = IDS.Tool.GeneralHelper.NullToString(dr["KaRepCode"]);

                            area.Name = IDS.Tool.GeneralHelper.NullToString(dr["KarepName"]);
                            area.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["UpdatedBy"]);
                            area.LastUpdate = IDS.Tool.GeneralHelper.NullToDateTime(dr["UpdatedDate"],DateTime.Now);

                            list.Add(area);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public int InsUpDelArea(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * FROM LeKarep where BranchCode ='" + BranchCode + "' and KarepCode='"+RepCode+"' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "LeUpdKaRep";
                    cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar,BranchCode );
                    cmd.AddParameter("@Code", System.Data.SqlDbType.VarChar, RepCode);
                    cmd.AddParameter("@Name", System.Data.SqlDbType.VarChar, Name);
                    cmd.AddParameter("@Operator", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if (ExecCode != 3)
                    {
                        newData = log.GenLogArray("SELECT * FROM LeKarep where BranchCode ='" + BranchCode + "' and KarepCode='" + RepCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "LeKarep", BranchCode+","+RepCode, OperatorID, "INSERT");
                        }
                        else if (ExecCode == 2)
                        {
                            log.SaveSysLog1(oldData, newData, "LeKarep", BranchCode + "," + RepCode, OperatorID, "UPDATE");
                        }
                        
                    }
                    else
                    {
                        log.SaveSysLog1(oldData, "", "LeKarep", BranchCode + "," + RepCode, OperatorID, "DELETE");

                    }
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Area code is already exists. Please choose other area code.");
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

    }
}
