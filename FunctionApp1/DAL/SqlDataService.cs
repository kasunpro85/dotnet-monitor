using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;

namespace FunctionApp1
{
    internal class SqlDataService : IDataService
    {
       
        private SqlConnection _sqlConnection;
        private SqlTransaction _sqlTransaction;
        private SqlDataReader _sqlDataReader;
        private static IConfigurationRoot Configuration { get; set; }

        public SqlDataService(string ConnectionString)
        {
            _sqlConnection = new SqlConnection();
            _sqlConnection.ConnectionString = ConnectionString;
            _sqlConnection.Open();
        }
            

        public override void BeginTransaction()
        {
            if (_sqlConnection != null)
            {
                if (_sqlConnection.State == ConnectionState.Closed)
                    _sqlConnection.Open();
                _sqlTransaction = _sqlConnection.BeginTransaction();
            }
        }

        public override void CloseConnection()
        {
            if (_sqlConnection != null)
            {
                if (_sqlTransaction != null)
                    _sqlTransaction.Rollback();
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    _sqlConnection.Close();
                    _sqlConnection.Dispose();
                }
            }
        }

        public override void CommitTransaction()
        {
            if (_sqlConnection != null)
            {
                if (_sqlTransaction != null)
                {
                    _sqlTransaction.Commit();
                    _sqlTransaction = null;
                }
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    _sqlConnection.Close();
                    _sqlConnection.Dispose();
                }
            }
        }

        public override void RollbackTransaction()
        {
            if (_sqlConnection != null)
            {
                if (_sqlTransaction != null)
                    _sqlTransaction.Rollback();
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    _sqlConnection.Close();
                    _sqlConnection.Dispose();
                }
            }
        }

        public override int ExecuteNonQuery(string spName, DbParameter[] sqlParameters)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = _sqlConnection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            command.Parameters.Clear();
            AddParameters(sqlParameters, command);
            if (_sqlTransaction != null)
                command.Transaction = _sqlTransaction;
            int result = command.ExecuteNonQuery();
            return result;
        }

        public override DbDataReader ExecuteReader(string spName, DbParameter[] sqlParameters)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = _sqlConnection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            command.Parameters.Clear();
            AddParameters(sqlParameters, command);
            if (_sqlTransaction != null)
                command.Transaction = _sqlTransaction;
            _sqlDataReader = command.ExecuteReader();
            return _sqlDataReader;
        }

        public override object ExecuteScalar(string spName, DbParameter[] sqlParameters)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = _sqlConnection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            command.Parameters.Clear();
            AddParameters(sqlParameters, command);
            if (_sqlTransaction != null)
                command.Transaction = _sqlTransaction;
            object result = command.ExecuteScalar();
            return result;
        }

        private void AddParameters(DbParameter[] dbParameters, SqlCommand command)
        {
            if (dbParameters != null)
            {
                foreach (DbParameter param in dbParameters)
                {
                    command.Parameters.Add(param);
                }
            }
        }

        public override void Dispose()
        {
            if (_sqlDataReader != null && !_sqlDataReader.IsClosed)
            {
                _sqlDataReader.Dispose();
            }
            if (_sqlConnection != null && _sqlConnection.State == System.Data.ConnectionState.Open)
            {
                _sqlConnection.Close();
            }
            if (_sqlConnection != null)
                _sqlConnection.Dispose();
        }
    }
}

