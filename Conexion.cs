using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    class Conexion
    {

        SqlConnection conn;
        string Server;
        string User;
        string Passw;
        string DataBase;
 
        public Conexion(string server, string user, string passw, string database)
        {
            Server = server;
            User = user;
            Passw = passw;
            DataBase = database;
 
            conn = new SqlConnection(GetConnectionString());
        }
 
        private string GetConnectionString()
        {
            return "server=" + Server + ";database=" + DataBase + ";user=" + User + ";password=" + Passw;
        }
 
        public void Insertar(string SQL)
        {
            SqlCommand cmd = new SqlCommand();
            conn.Open();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SQL;
                cmd.Connection = conn;
 
                cmd.Parameters.Clear();
                cmd.ExecuteNonQuery();
 
 
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close();
                    cmd.Dispose();
                }
            }
        }
 
        public DataTable consultar(string sql)
        {
            //Creo el DataTable
            DataTable dt = new DataTable();
 
            using (SqlDataAdapter Adapter = new SqlDataAdapter(sql, conn))
            {
                //Relleno el adaptador con los datos en memoria
                Adapter.Fill(dt);
            }
 
            return dt;
          }
 
        public DataTable Consulta(string SQL, SqlParameter[] param)
        {
            SqlCommand cmd = null;
            SqlDataAdapter sda = null;
            DataSet ds = null;
 
            try
            {
                cmd = new SqlCommand();
 
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SQL;
                cmd.Connection = conn;
 
                cmd.Parameters.Clear();
 
                if (param != null)
                {
                    foreach (SqlParameter p in param)
                        cmd.Parameters.Add(p);
                }
 
                sda = new SqlDataAdapter(cmd);
                ds = new DataSet();
                sda.Fill(ds);
 
                return ds.Tables.Count > 0 ? ds.Tables[0] : null;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close();
                    cmd.Dispose();
                }
 
                if (ds != null) ds.Dispose();
                if (sda != null) sda.Dispose();
            }
        }
 
        public bool IsDbConnectionOK()
        {
            string sqlQuery = "SELECT NAME FROM SYS.DATABASES where NAME='" + DataBase + "'";
 
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                    {
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        return sqlDataReader != null || sqlDataReader.HasRows;
                    }
                }
            }
            catch { return false; }
        }
 
        public void ExecNonProcedure(string ProcedureName, SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand();
 
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = ProcedureName;
                cmd.Connection = conn;
 
                cmd.Parameters.Clear();
 
                if (param != null)
                {
                    foreach (SqlParameter p in param)
                        cmd.Parameters.Add(p);
                }
 
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
 
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Parameters.Clear();
                    if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close();
                    cmd.Dispose();
                }
            }
        }
    


    }

