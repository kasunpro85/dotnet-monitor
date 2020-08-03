
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public class DataService 
    {

        IDataService _dataService;

        public DataService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public bool PostList(Task<MonitoVal> content)
        {
            
            try
            {

                DbParameter[] dbParameters = new DbParameter[2];

                dbParameters[0] = DataServiceBuilder.CreateDBParameter("@Register", System.Data.DbType.String, System.Data.ParameterDirection.Input, content.Result.register);
                dbParameters[1] = DataServiceBuilder.CreateDBParameter("@Polling", System.Data.DbType.String, System.Data.ParameterDirection.Input, content.Result.polling);
                // _dataService.ExecuteNonQuery("[dbo].[AddTest]", dbParameters);
            
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
    }
}
