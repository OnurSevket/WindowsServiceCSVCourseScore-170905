using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteScoreWinService
{
    public class SqlHelper
    {
        //Connection String buradan Değiştirilmesi gerekiyor!!!!!
        private const string connectionString = "Server=.;Database=DBLutegSchool;Integrated Security=true;";

        public static SqlCommand createSqlCommand()
        {
            SqlConnection sqlConn = new SqlConnection();
            sqlConn.ConnectionString = connectionString;

            SqlCommand sqlComm = new SqlCommand();
            sqlComm.Connection = sqlConn;

            return sqlComm;
        }
    }
}
