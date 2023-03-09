using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace Project2AzureFunc.Data
{
    internal interface IDataProvider
    {
        DataSet HandleSQL(string sql);
        Task<DataSet> HandleSQLAsync(string sql);
    }

    internal class DataAccessHelper : IDataProvider
    {
        private readonly string connString;

        internal DataAccessHelper(string connString)
        {
            this.connString = connString;
        }

        internal DataAccessHelper() //read from ENV or default setup
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            connString = @$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={currentDirectory}\Data\Database.mdf;Integrated Security=True";
        }

        public DataSet HandleSQL(string sql)
        {
            return ExecuteNonScalarQuery(sql);
        }

        public async Task<DataSet> HandleSQLAsync(string sql)
        {
            return await ExecuteNonScalarQueryAsync(sql);
        }

        private DataSet ExecuteNonScalarQuery(string sql)
        {
            SqlConnection sqlConnection = new(connString);
            try
            {
                SqlCommand sqlCommand = new()
                {
                    Connection = sqlConnection,
                    CommandText = sql,
                    CommandTimeout = 9000
                };

                SqlDataAdapter dataAdapter = new(sqlCommand);
                sqlConnection.Open();

                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                return dataSet;
            }
            catch (SqlException e)
            {
                var test = sql;
                throw e;
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
        }

        private async Task<DataSet> ExecuteNonScalarQueryAsync(string sql) //can be for reads
        {
            SqlConnection sqlConnection = new(connString);
            try
            {
                SqlCommand sqlCommand = new()
                {
                    Connection = sqlConnection,
                    CommandText = sql,
                    CommandTimeout = 9000
                };

                SqlDataAdapter dataAdapter = new(sqlCommand);
                await sqlConnection.OpenAsync();

                DataSet dataSet = new();
                dataAdapter.Fill(dataSet);
                return dataSet;
            }
            catch (SqlException e)
            {
                var test = sql;
                throw e;
            }
            finally
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
        }
    }
}
