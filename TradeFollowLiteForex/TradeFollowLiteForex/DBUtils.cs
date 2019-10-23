using System;
using MySql.Data.MySqlClient;

namespace TradeFollowLiteForex
{
    class DBUtils
    {
        public MySqlConnection conn;
        public MySqlConnection connclient;
        public string error_mes;
        public DBUtils(string username)
        {
            //string host = "localhost";
            string host = "103.89.88.150";
            int port = 3306;
            string database = "trademt4";
            //string username = "client";
            string password = "Meyeu150458@";

            conn = DBMySQLUtils.GetDBConnection(host, port, database, username, password);
        }

        private void CloseClient()
        {
            if (connclient.State != System.Data.ConnectionState.Closed)
            {
                connclient.Close();
            }
        }
        private void ConnectDB()
        {
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
        }
        private void CloseDB()
        {
            if (conn.State != System.Data.ConnectionState.Closed)
            {
                conn.Close();
            }
        }
        public bool InsertOrder(order neworder,string sourcedata,string username)
        {
            ConnectDB();
            string sql = "INSERT INTO mastertrader (sourcedata,username,symbol,typeorder,size,datetime,openprice,currentprice,stoploss,takeprofit,profit)"
                + " VALUES (@sourcedata,@username,@symbol,@typeorder,@size,@datetime,@openprice,@currentprice,@stoploss,@takeprofit,@profit)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sourcedata", sourcedata);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@symbol", neworder.symbol);
            
            
            cmd.Parameters.AddWithValue("@size", double.Parse(neworder.size));
            cmd.Parameters.AddWithValue("@datetime", neworder.datetime);
            double openprice = double.Parse(neworder.openprice);
            cmd.Parameters.AddWithValue("@openprice", openprice);
            double currentprice = double.Parse(neworder.currentprice);
            cmd.Parameters.AddWithValue("@currentprice", currentprice);
            cmd.Parameters.AddWithValue("@stoploss", double.Parse(neworder.stoploss));
            cmd.Parameters.AddWithValue("@takeprofit", double.Parse(neworder.takeprofit));
            cmd.Parameters.AddWithValue("@profit", double.Parse(neworder.profit));

            int typeorder = 0;
            if (neworder.typeorder == "Mua") typeorder = 0;
            else if (neworder.typeorder == "Bán") typeorder = 1;
            else if (neworder.typeorder == "Muatại" && openprice < currentprice) typeorder = 2;
            else if (neworder.typeorder == "Bántại" && openprice > currentprice) typeorder = 3;
            else if (neworder.typeorder == "Muatại" && openprice > currentprice) typeorder = 4;
            else if (neworder.typeorder == "Bántại" && openprice < currentprice) typeorder = 5;
            else typeorder = 6;
            cmd.Parameters.AddWithValue("@typeorder", typeorder);
            try
            {
                cmd.ExecuteNonQuery();
                CloseDB();
                error_mes = null;
                return true;
            }
            catch (MySqlException error)
            {
                CloseDB();
                error_mes = "InsertOrder: " + error.Message;
                return false;
            }
        }
    }
}
