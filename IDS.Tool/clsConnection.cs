using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Summary description for clsConnection
/// </summary>
namespace IDS.Tool
{
    public class clsConnection  // Add by Anthony - 20140415 - IDisposable
    {
        SqlConnection sConn;
        string stringConn;

        public string buildConnection()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuation = builder.Build();
            return configuation.GetSection("ConnectionStrings").GetSection("Conn").Value;
        }

        public SqlConnection Open()
        {
            stringConn = buildConnection();
            sConn = new SqlConnection(stringConn);
            sConn.Open();
            return sConn;
        }

        #region Untuk Koneksi database JF
        public string buildConnection(bool isJFSQLConnection)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuation = builder.Build();
            return configuation.GetSection("ConnectionStrings").GetSection("ConnJF").Value;
        }

        public SqlConnection Open(bool isJFSQLConnection)
        {
            if (isJFSQLConnection)
                stringConn = buildConnection(true);
            else
                stringConn = buildConnection();

            sConn = new SqlConnection(stringConn);
            sConn.Open();
            return sConn;
        }
        #endregion

        public SqlConnection Close()
        {
            //string stringConn = buildConnection();
            //SqlConnection sConn = new SqlConnection(stringConn);
            sConn.Close();
            sConn.Dispose();
            GC.WaitForPendingFinalizers();
            return sConn;
        }

        // Add - Anthony - 20180828
        public SqlConnection GetConnection
        {
            get { return sConn; }
        }
        // End add - Anthony - 20180828 

        #region IDisposable Members

        public void Dispose()
        {
            sConn.Dispose();
            GC.WaitForPendingFinalizers();
        }

        #endregion
    }
}
