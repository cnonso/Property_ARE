using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace PropertyManagerWeb.Models
{
    public class DB
    {
        private SqlConnection serverConnector = new SqlConnection();
        public string DBError;
        public DB()
        {
            serverConnector = new SqlConnection(WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }

        
        public DataTable GetTableColumns(string tableName)
        {

            return ReadTable(tableName + "Columns", "select * from information_schema.columns where table_name = '" + tableName + "'");
        }

        public string ReadData(string strQuery)
        {
            string result = string.Empty;
            try
            {
                serverConnector.Open();
            }
            catch (Exception ex)
            {
                //AppHandler.MsgBox(ex.Message);
                return "";
            }
            SqlCommand sqlCmd = new SqlCommand(strQuery, serverConnector);
            SqlDataReader sdr = sqlCmd.ExecuteReader();
            if (sdr.Read())
                result = sdr.GetValue(0).ToString();
            sdr.Close();
            serverConnector.Close();
            return result;
        }

        public DataTable ReadTable(string tableName)
        {
            DataTable tableData = new DataTable(tableName);
            try
            {
                serverConnector.Open();
            }
            catch (Exception ex)
            {
                DBError = ex.Message;
                return tableData;
            }
            SqlCommand sqlCmd = new SqlCommand("SELECT * FROM " + tableName + "", serverConnector);
            tableData.Load(sqlCmd.ExecuteReader());
            serverConnector.Close();
            return tableData;
        }

        public DataTable ReadTable(string tableName, string customQuery)
        {
            DataTable tableData = new DataTable(tableName);
            try
            {
                serverConnector.Open();
            }
            catch (Exception ex)
            {
                DBError = ex.Message;
                return tableData;
            }
            SqlCommand sqlCmd = new SqlCommand(customQuery, serverConnector);
            tableData.Load(sqlCmd.ExecuteReader());
            serverConnector.Close();
            return tableData;
        }

        public bool ExecuteQuery(string query)
        {
            try
            {
                serverConnector.Open();
            }
            catch (Exception ex) { return false; }
            SqlCommand sqlCmd = new SqlCommand(query, serverConnector);
            bool executed = sqlCmd.ExecuteNonQuery() > 0;
            serverConnector.Close();
            return executed;
        }

        public bool BatchExecution(string query)
        {
            try
            {
                serverConnector.Open();
            }
            catch (Exception ex) { }
            SqlCommand sqlCmd = new SqlCommand(query, serverConnector);
            return sqlCmd.ExecuteNonQuery() > 0;
        }

        public void StopBatchExecution()
        {
            try
            {
                serverConnector.Close();
            }
            catch (Exception ex) { }
        }

        
        
        public bool CreateTableFromAnotherDbTable(string sourceDB, string sourceTable, string destDB, string destTable)
        {
            try
            {
                if (ExecuteQuery("SELECT * INTO [" + destDB + "].[dbo].[" + destTable + "] FROM [" + sourceDB + "].[dbo].[" + sourceTable + "]"))
                    return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public bool BackupDB(string dest)
        {
            return ExecuteQuery("BACKUP DATABASE " + serverConnector.Database + " TO disk = '" + dest + "'");
        }

        public void DeleteAllTables()
        {
            ExecuteQuery("EXEC sp_MSforeachtable @command1 = \"DROP TABLE ?\"");
        }

        public DataTable SortDataTable(DataTable table, string columnName, string order)
        {
            table.DefaultView.Sort = columnName + " " + order;
            table = table.DefaultView.ToTable();
            return table;
        }
        public DateTime IgnoreDBNullDate(object arg)
        {
            DateTime extremeDate = new DateTime(9999, 12, 31);
            try
            {
                return Convert.ToDateTime(arg.ToString());
            }
            catch (Exception ex) { }
            return extremeDate;
        }

        public string IgnoreDBNullString(object arg)
        {
            try
            {
                return arg.ToString();
            }
            catch (Exception ex) { }
            return "";
        }

        //public bool AddBook(Book book)
        //{
        //    bool added = false;
        //    try
        //    {
        //        serverConnector.Open();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    SqlCommand bookCmd = serverConnector.CreateCommand();
        //    //SqlTransaction transaction = serverConnector.BeginTransaction("PixelsBankPay");
        //    string strQuery = "INSERT INTO [BookDetails](BookTitle, BookID, Category, Author_Publisher, Status, Shelf_No) " +
        //                      " VALUES(@BookTitle, @BookID, @Category, @Author_Publisher, @Status, @Shelf_No" +
        //                      ") SET @Identity = SCOPE_IDENTITY()";
        //    bookCmd.Connection = serverConnector;
        //    //bookCmd.Transaction = transaction;
        //    bookCmd.CommandText = strQuery;
        //    SqlParameter identityParam = new SqlParameter("@Identity", SqlDbType.Int, 0, "BookID");
        //    identityParam.Direction = ParameterDirection.Output;
        //    bookCmd.Parameters.AddWithValue("@BookTitle", book.BookTitle);
        //    bookCmd.Parameters.AddWithValue("@BookID", book.BookID);
        //    bookCmd.Parameters.AddWithValue("@Category", book.Category);
        //    bookCmd.Parameters.AddWithValue("@Author_Publisher", book.Author_Publisher);
        //    bookCmd.Parameters.AddWithValue("@Status", "Available");
        //    bookCmd.Parameters.AddWithValue("@Shelf_No", book.Shelf_No);
        //    bookCmd.Parameters.Add(identityParam);
        //    added = bookCmd.ExecuteNonQuery() > 0;
        //    //transaction.Commit();
        //    serverConnector.Close();
        //    return added;
        //}

        //public bool UpdateBook(Book book)
        //{
        //    bool updated = false;
        //    try
        //    {
        //        serverConnector.Open();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    SqlCommand schoolCmd = serverConnector.CreateCommand();
        //    string strQuery = "UPDATE [dbo].[BookDetails]SET [BookTitle] = @BookTitle,[BookID] = @BookID,[Category] = @Category,[Author_Publisher] = @Author_Publisher,[Status] = @Status,[Shelf_No] = @Shelf_No WHERE BookID=@BookID";
        //    schoolCmd.Connection = serverConnector;
        //    schoolCmd.CommandText = strQuery;
        //    schoolCmd.Parameters.AddWithValue("@BookTitle", book.BookTitle);
        //    schoolCmd.Parameters.AddWithValue("@BookID", book.BookID);
        //    schoolCmd.Parameters.AddWithValue("@Category", book.Category);
        //    schoolCmd.Parameters.AddWithValue("@Author_Publisher", book.Author_Publisher);
        //    schoolCmd.Parameters.AddWithValue("@BorrowDate", book.BorrowDate);
        //    schoolCmd.Parameters.AddWithValue("@ReturnDue", book.ReturnDue);
        //    schoolCmd.Parameters.AddWithValue("@Status", book.Status);
        //    schoolCmd.Parameters.AddWithValue("@Shelf_No", book.Shelf_No);
        //    updated = schoolCmd.ExecuteNonQuery() > 0;
        //    serverConnector.Close();
        //    return updated;
        //}

        public bool CartContainsProduct(string product_ID)
        {
            return ReadData("select ProductID from Cart where ProductID = '" + product_ID + "' ").Trim() != string.Empty;
        }
    }
}