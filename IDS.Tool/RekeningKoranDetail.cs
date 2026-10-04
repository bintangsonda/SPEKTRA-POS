using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Web;
using Microsoft.VisualBasic.FileIO;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace IDS.Tool
{
    /// <summary>
    /// Class helper untuk membaca file Rekening Koran (.csv)
    /// </summary>
    public class RekeningKoranDetail
    {


        public string Description { get; set; }

        /// <summary>
        /// Tanggal transaksi di rekening koran
        /// </summary>
        public DateTime TransDate { get; set; }

        /// <summary>
        /// Debit atau Credit
        /// </summary>
        public string DBCR { get; set; }

        /// <summary>
        /// Nilai transaksi
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Ending balance per transaksi
        /// </summary>
        public double EndingBalance { get; set; }

        public RekeningKoranDetail()
        {
            Amount = 0;
            EndingBalance = 0;
        }

        /// <summary>
        /// Cek lokasi file dan membaca file rekening korang dan disimpan ke variable Original File Content
        /// </summary>
        /// <param name="CSVFilePath"></param>
        /// <param name="bankName"></param>
        public void CollectData()
        {



            string conn = string.Empty;
            string fileName = "CorpAcctStmt202211325617466.csv";
            string rootPath = Directory.GetCurrentDirectory(); // lokasi base project (bukan wwwroot)
            string filePath = Path.Combine(rootPath, "wwwroot", fileName);
            string fileExt = "csv";

            Excel.Application _app = new Excel.Application();
            Excel.Workbooks _workbooks = _app.Workbooks;

            _workbooks.OpenText(filePath, Comma: true);
        }
    }
}
