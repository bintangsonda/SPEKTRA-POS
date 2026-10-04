using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Tool
{
    public abstract class RekeningKoranHeader
    {
        // Header
        protected string Title { get; set; }
        protected string BankAccountNo { get; set; }
        protected string AccountOwnerName { get; set; }
        protected DateTime PeriodFrom { get; set; }
        protected DateTime PeriodTo { get; set; }
        protected string Currency { get; set; }

        // Footer
        protected decimal BeginningBalance { get; set; }
        protected decimal DebitMutation { get; set; }
        protected decimal CreditMutation { get; set; }
        protected decimal EndingBalance { get; set; } 


        // Konfigurasi sesuai dengan file CSV Bank, default berdasarkan Bank BCA
        /// <summary>
        /// Apakah file CSV terdapat header atau tidak
        /// </summary>
        protected bool hasHeader { get; set; }
        /// <summary>
        /// Batas line terakhir header
        /// </summary>
        protected int headerEndLine { get; set; }

        /// <summary>
        /// Apakah file CSV terdapat footer atau tidak
        /// </summary>
        protected bool hasFooter { get; set; }
        /// <summary>
        /// Jumlah line footer
        /// </summary>
        protected int footerLineCount { get; set; }

        /// <summary>
        /// Apakah sebelum data detail pada file csv ada judul data detil atau tidak
        /// </summary>
        protected bool detailHasTitle { get; set; }
        /// <summary>
        /// Batas line terakhir judul data detil
        /// </summary>
        protected int detailDataTitleEndLine { get; set; }

        /// <summary>
        /// Line bagian detil mulai
        /// </summary>
        protected int detailDataStartFrom { get; set; }
        
        protected string[] DetailTitle { get; set; }

        /// <summary>
        /// Separator untuk data
        /// </summary>
        protected char[] separator { get; set; }

        public List<RekeningKoranDetail> Detail;

        public RekeningKoranHeader()
        {
            hasHeader = true;
            headerEndLine = 7;

            detailHasTitle = true;
            detailDataTitleEndLine = 8;

            hasFooter = true;
            detailDataStartFrom = 9;
            separator = new char[] { ',' };
            Detail = new List<RekeningKoranDetail>();
        }

        public virtual void ReadCSVFile(string filePath)
        {
            string line = string.Empty;

            using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath, Encoding.UTF8))
            {
                int count = 1;
                StringBuilder sb = new StringBuilder();
                string[] temp;
                char[] dustSeparator = new char[] { '-' };
                char[] slashSeparator = new char[] { '/' };

                RekeningKoranDetail lastItem = null;
                RekeningKoranDetail item = null;

                while ((line = reader.ReadLine()) != null)
                {
                    #region Header
                    if (hasHeader && count <= headerEndLine)
                    {
                        temp = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                        if (temp.Length == 0)
                        {
                            count++;
                            continue;
                        }                            

                        if (temp[0].StartsWith("No. rekening :"))
                        {
                            BankAccountNo = temp[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1];
                            count++;
                            continue;
                        }

                        if (temp[0].StartsWith("Nama :"))
                        {
                            AccountOwnerName = temp[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1];
                            count++;
                            continue;
                        }

                        if (temp[0].StartsWith("Periode :"))
                        {
                            string[] period = temp[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                            if (!string.IsNullOrWhiteSpace(period[1]))
                            {
                                string[] tempPeriod = period[1].Split(dustSeparator).Select(x => x.Trim()).ToArray();

                                PeriodFrom = DateTime.ParseExact(tempPeriod[0], "dd/MM/yy",System.Globalization.CultureInfo.InvariantCulture);
                                PeriodTo = DateTime.ParseExact(tempPeriod[1], "dd/MM/yy", System.Globalization.CultureInfo.InvariantCulture);
                            }

                            count++;
                            continue;
                        }

                        if (temp[0].StartsWith("Kode Mata Uang :"))
                        {
                            Currency = temp[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1];
                            count++;
                            continue;
                        }

                        count++;
                        continue;
                    }
                    #endregion
                    
                    #region Detail Title
                    if (detailHasTitle && count > headerEndLine && count <= detailDataTitleEndLine)
                    {
                        DetailTitle = line.Split(separator);
                        count++;
                        continue;
                    }
                    #endregion

                    #region Detail
                    if (Detail == null)
                        Detail = new List<RekeningKoranDetail>();

                    temp = line.Split(separator);

                    if (lastItem == null)
                    {
                        item = new RekeningKoranDetail();
                    }
                    else
                    {
                        item = lastItem;
                    }


                    if (temp != null)
                    {
                        if (temp[0] != ""
                            && !temp[0].StartsWith("\"Saldo Awal :")
                            && !temp[0].StartsWith("\"Mutasi Debet :")
                            && !temp[0].StartsWith("\"Mutasi Kredit :")
                            && !temp[0].StartsWith("\"Saldo Akhir :"))
                        {

                            //string[] datePart;

                            if (lastItem == null)
                            {
                                //if (temp[0].Contains('-'))
                                //{
                                //    datePart = temp[0].Split(dustSeparator, StringSplitOptions.RemoveEmptyEntries);
                                //}
                                //else
                                //{
                                //    datePart = temp[0].Split(slashSeparator, StringSplitOptions.RemoveEmptyEntries);
                                //}

                                //if (datePart.Length > 1)
                                //{
                                //    switch (datePart[1].ToUpper())
                                //    {
                                //        case "JAN":
                                //        case "FEB":
                                //        case "MAR":
                                //        case "APR":
                                //        case "MAY":
                                //        case "JUN":
                                //        case "JUL":
                                //        case "AUG":
                                //        case "SEPT":
                                //        case "OCT":
                                //        case "NOV":
                                //        case "DEC":
                                //            item.TransDate = Convert.ToDateTime(datePart[0] + "-" + datePart[1] + "-" + PeriodFrom.ToString("yyyy")); //MM-DD-YYYY
                                //            break;
                                //        default:
                                //            item.TransDate = Convert.ToDateTime(datePart[1] + "-" + datePart[0] + "-" + PeriodFrom.ToString("yyyy"));
                                //            break;
                                //    }
                                //}

                                item.TransDate = Convert.ToDateTime(temp[0]);

                                item.Description = temp[1];
                            }
                            else
                            {
                                item.Description = temp[1];
                            }

                            if (temp[temp.Length - 1].EndsWith("\""))
                            {
                                string amount = GetAmountValue(line).Replace("\"", "");
                                string endingBalance = GetEndingBalance(line).Replace("\"", "");

                                if (amount.ToUpper().EndsWith("DB"))
                                {
                                    item.DBCR = "DB";
                                }
                                else if (amount.ToUpper().EndsWith("CR"))
                                {
                                    item.DBCR = "CR";
                                }

                                amount = amount.Substring(0, amount.Length - 2);
                                amount = amount.Replace(",", "");
                                item.Amount = Convert.ToDouble(amount);

                                if (item.DBCR == "DB")
                                    item.Amount = (item.Amount * (-1));

                                item.EndingBalance = Convert.ToDouble(endingBalance.Replace(",", ""));

                                Detail.Add(item);
                                lastItem = null;
                            }
                            else
                            {
                                lastItem = item;
                            }
                        }
                        else
                        {
                            if (!temp[0].StartsWith("\"Saldo Awal :")
                            && !temp[0].StartsWith("\"Mutasi Debet :")
                            && !temp[0].StartsWith("\"Mutasi Kredit :")
                            && !temp[0].StartsWith("\"Saldo Akhir :"))
                            {
                                item.Description = item.Description + ", " + temp[1];

                                if (temp[temp.Length - 1].EndsWith("\""))
                                {
                                    string amount = GetAmountValue(line).Replace("\"", "");
                                    string endingBalance = GetEndingBalance(line).Replace("\"", "");

                                    if (amount.ToUpper().EndsWith("DB"))
                                    {
                                        item.DBCR = "DB";
                                    }
                                    else if (amount.ToUpper().EndsWith("CR"))
                                    {
                                        item.DBCR = "CR";
                                    }

                                    amount = amount.Substring(0, amount.Length - 2);
                                    amount = amount.Replace(",", "");
                                    item.Amount = Convert.ToDouble(amount);

                                    if (item.DBCR == "DB")
                                        item.Amount = (item.Amount * (-1));

                                    item.EndingBalance = Convert.ToDouble(endingBalance.Replace(",", ""));

                                    Detail.Add(item);
                                    lastItem = null;
                                }
                                else
                                {
                                    lastItem = item;
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                    #endregion
                }
            }   
        }

        public virtual void InsertToDB(string branchCode, string AccNo, string ccy, string period)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                try
                {
                    db.CommandText = "GLUpdAcfBankStat";
                    db.CommandType = System.Data.CommandType.StoredProcedure;
                    db.Open();
                    db.BeginTransaction();

                    if (Detail != null && Detail.Count > 0)
                    {
                        foreach (RekeningKoranDetail d in Detail)
                        {
                            db.AddParameter("@Type", System.Data.SqlDbType.Int, 1);
                            db.AddParameter("@AccNo", System.Data.SqlDbType.VarChar, AccNo);
                            db.AddParameter("@CCy", System.Data.SqlDbType.VarChar, ccy);
                            db.AddParameter("@Period", System.Data.SqlDbType.VarChar, period);
                            db.AddParameter("@DocBank", System.Data.SqlDbType.VarChar, DBNull.Value);
                            db.AddParameter("@AmountBank", System.Data.SqlDbType.VarChar, d.Amount);
                            db.AddParameter("@SaldoBank", System.Data.SqlDbType.Money, d.EndingBalance);
                            db.AddParameter("@Remark", System.Data.SqlDbType.VarChar, d.Description);
                            db.AddParameter("@TransDate", System.Data.SqlDbType.DateTime, d.TransDate);
                            db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, branchCode);
                            db.AddParameter("@MatchStatus", System.Data.SqlDbType.Bit, 0);
                            result = db.ExecuteNonQuery();
                        }



                    }
                    db.CommitTransaction();


                }
                catch (Exception)
                {
                    if (db.Transaction != null)
                        db.RollbackTransaction();

                    throw;
                }
                finally
                {
                    db.Close();
                }
            }
        }

        private string GetAmountValue(string line)
        {
            int dQuoteIndex = line.IndexOf("\"", 0);
            int dQuoteIndex2 = line.IndexOf("\"", dQuoteIndex + 1);

            string amount = line.Substring(dQuoteIndex, dQuoteIndex2 - dQuoteIndex);

            return amount;
        }

        private string GetEndingBalance(string line)
        {
            int dQuoteIndex = line.LastIndexOf("\"");
            int dQuoteIndex2 = line.LastIndexOf("\"", line.Length - 2);

            string amount = line.Substring(dQuoteIndex2, dQuoteIndex - dQuoteIndex2);

            return amount;
        }
    }
}
