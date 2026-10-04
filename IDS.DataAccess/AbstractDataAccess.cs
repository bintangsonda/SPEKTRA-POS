using System;
using System.Data;

namespace IDS.DataAccess
{
    public abstract class AbstractDataAccess : IDisposable
    {
        private string connectionString;
        private IDbConnection dbConnection;
        private IDbCommand dbCommand;
        private IDbTransaction transaction;
        private IDataReader dbDataReader;
        private IDbDataAdapter dbDataAdapter;
        private DataSet dataSet;
        public DataSet DataSet
        {
            get { return dataSet; }
            protected set { dataSet = value; }
        }
        public IDbDataAdapter DbDataAdapter
        {
            get { return dbDataAdapter; }
            set { dbDataAdapter = value; }
        }

        public IDataReader DbDataReader
        {
            get { return dbDataReader; }
            set { dbDataReader = value; }
        }

        public IDbTransaction Transaction
        {
            get { return transaction; }
            set { transaction = value; }
        }

        public IDbCommand DbCommand
        {
            get { return dbCommand; }
            set { dbCommand = value; }
        }

        public IDbConnection DbConnection
        {
            get { return dbConnection; }
            set { dbConnection = value; }
        }

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }
        public abstract void Open();
        public abstract void BeginTransaction();
        public abstract void CommitTransaction();
        public abstract void RollbackTransaction();

        public abstract DataSet GetDataSet();
        public abstract DataSet GetDataSet(string dataSetName);
        public abstract DataTable GetDataTable();
        public abstract DataTable GetDataTable(string dataTableName);

        public void Close()
        {
            if (DbConnection.State == ConnectionState.Open || DbConnection.State == ConnectionState.Fetching)
                DbConnection.Close();
        }



        public void Dispose()
        {
            dbConnection.Close();
            dbConnection.Dispose();
            dbCommand.Dispose();

            if (dbDataReader != null)
            {
                dbDataReader.Close();
                dbDataReader.Dispose();
            }

            if (dbDataAdapter != null)
            {
                dbDataAdapter = null;
            }

            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
