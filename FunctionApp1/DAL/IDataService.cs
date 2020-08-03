using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FunctionApp1
{
    public abstract class IDataService : IDisposable
    {
        public abstract void BeginTransaction();
        public abstract void CommitTransaction();
        public abstract void RollbackTransaction();
        public abstract void CloseConnection();
        public abstract int ExecuteNonQuery(string spName, DbParameter[] sqlParameters);
        public abstract DbDataReader ExecuteReader(string spName, DbParameter[] sqlParameters);
        public abstract object ExecuteScalar(string spName, DbParameter[] sqlParameters);

        public abstract void Dispose();

    }
}