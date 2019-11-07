using System;
using MySql.Data.MySqlClient;

namespace TradeFollowLiteForex
{
    struct nonlive
    {
        public ushort id;
        public uint chatid;
        public string username;
        public string keyword;
        public string command;
        public string name_mt4;
        public string datetime;
    }
    class DBUtils
    {
        public MySqlConnection conn;
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

        private void ConnectDB()
        {
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
        }
        //@Anthony_Khai(Hoang)
        //@Alexngo_vn(Quan)
        //@Happytrader(Phuoc Giao Chu)

        //@Nupacachi
        //@gi0_d0ng
        private void CloseDB()
        {
            if (conn.State != System.Data.ConnectionState.Closed)
            {
                conn.Close();
            }
        }
        public string InsertOrder(order neworder,string sourcedata,string username)
        {
            string sql = "INSERT INTO mastertrader (sourcedata,username,symbol,typeorder,size,datetime,openprice,currentprice,stoploss,takeprofit,profit,md5)"
                + " VALUES (@sourcedata,@username,@symbol,@typeorder,@size,@datetime,@openprice,@currentprice,@stoploss,@takeprofit,@profit,@md5)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sourcedata", sourcedata);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@symbol", neworder.symbol);
            cmd.Parameters.AddWithValue("@size", neworder.size);
            cmd.Parameters.AddWithValue("@datetime", neworder.datetime);
            cmd.Parameters.AddWithValue("@openprice", neworder.openprice);
            cmd.Parameters.AddWithValue("@currentprice", neworder.currentprice);
            cmd.Parameters.AddWithValue("@stoploss", neworder.stoploss);
            cmd.Parameters.AddWithValue("@takeprofit", neworder.takeprofit);
            cmd.Parameters.AddWithValue("@profit", neworder.profit);
            cmd.Parameters.AddWithValue("@typeorder", neworder.typeorder);

            string str_hashmd5 = neworder.datetime + username + sourcedata;
            string hashmd5 = Util.MD5(str_hashmd5);
            cmd.Parameters.AddWithValue("@md5", hashmd5);
            if (CheckExistOrder(hashmd5)) return "duplicate";
            ConnectDB();
            try
            {
                cmd.ExecuteNonQuery();
                CloseDB();
                error_mes = null;
                return "success";
            }
            catch (MySqlException error)
            {
                CloseDB();
                error_mes = "InsertOrder: " + error.Message;
                return "error";
            }
        }
        public bool CheckExistOrder(string md5)
        {
            string result = "";
            ConnectDB();
            string sql = "SELECT * FROM mastertrader WHERE md5=@md5";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@md5", md5);
            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result += reader.GetString("md5");
                    }
                }
                error_mes = null;
            }
            catch (MySqlException error)
            {
                error_mes = "CheckExistOrder: " + error.Message;
            }
            CloseDB();
            if (result == "") return false;
            else return true;
        }
        public order GetNewOrder(string oldmd5)
        {
            order result = new order();
            ConnectDB();
            string sql = "SELECT * FROM mastertrader";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string md5hash = reader.GetString("md5");
                        if (oldmd5.Contains(md5hash) == false)
                        {
                            result.sourcedata = reader.GetString("sourcedata");
                            result.username = reader.GetString("username");
                            result.symbol = reader.GetString("symbol");
                            result.typeorder = reader.GetInt16("typeorder");
                            result.size = reader.GetDouble("size");
                            result.datetime = reader.GetString("datetime");
                            result.openprice = reader.GetDouble("openprice");
                            result.currentprice = reader.GetDouble("currentprice");
                            result.stoploss = reader.GetDouble("stoploss");
                            result.takeprofit = reader.GetDouble("takeprofit");
                            result.profit = reader.GetDouble("profit");
                            result.md5 = reader.GetString("md5");
                            return result;
                        }
                    }
                    result = null;
                }
                error_mes = null;
            }
            catch (MySqlException error)
            {
                error_mes = "GetNewOrder: " + error.Message;
            }
            return result;
        }
        public string GetOldMD5()
        {
            string result = "";
            ConnectDB();
            string sql = "SELECT * FROM mastertrader";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result += reader.GetString("md5") + "|";
                    }
                }
                error_mes = null;
            }
            catch (MySqlException error)
            {
                error_mes = "GetOldMD5: " + error.Message;
            }
            CloseDB();
            return result;
        }
        public string SendSignalOrder(order neworder,string namemt4,string mode= "absolute")
        {
            string result = "";//buy sb xauusd size 0.01 price 0 pricesl 1500 pricetp 1400 comment mastertrader mt4 l
            string typeorder = "typewrong";
            if (neworder.typeorder == 0) typeorder = "buy";
            else if (neworder.typeorder == 1) typeorder = "sell";
            else if (neworder.typeorder == 2) typeorder = "buylimit";
            else if (neworder.typeorder == 3) typeorder = "selllimit";
            else if (neworder.typeorder == 4) typeorder = "buystop";
            else if (neworder.typeorder == 5) typeorder = "sellstop";
            if (mode == "absolute")
                result += typeorder + " sb " + neworder.symbol + " size " + neworder.size + " price " + neworder.openprice + " pricesl " + neworder.stoploss + " pricetp " + neworder.takeprofit + " comment " + neworder.username + " mt4 " + namemt4;
            else if (mode == "scalping")
                result += typeorder + " sb " + neworder.symbol + " size " + neworder.size + " price " + neworder.openprice + " pricesl -1 " + " pricetp -1 " + " comment " + neworder.username + " mt4 " + namemt4;
            return result;
        }
        public bool InsertNonLive(nonlive NewNonlive)
        {
            ConnectDB();
            string sql = "INSERT INTO nonlive (chatid,username,keyword,command,name_mt4,datetime)" + " VALUES (@chatid,@username,@keyword,@command,@name_mt4,@datetime)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@chatid", NewNonlive.chatid);
            cmd.Parameters.AddWithValue("@username", NewNonlive.username);
            cmd.Parameters.AddWithValue("@keyword", NewNonlive.keyword);
            cmd.Parameters.AddWithValue("@command", NewNonlive.command);
            cmd.Parameters.AddWithValue("@name_mt4", NewNonlive.name_mt4);
            cmd.Parameters.AddWithValue("@datetime", NewNonlive.datetime);


            try
            {
                cmd.ExecuteNonQuery();
                CloseDB();
                error_mes = null;
            }
            catch (MySqlException error)
            {
                CloseDB();
                error_mes = "InsertNonLive: " + error.Message;
                return false;
            }
            return true;
        }
        public string SendOrderToTrade(order neworder, string namemt4, string mode = "absolute")
        {
            string result = "";//buy sb xauusd size 0.01 price 0 pricesl 1500 pricetp 1400 comment mastertrader mt4 l
            string typeorder = "typewrong";
            if (neworder.typeorder == 0) typeorder = "buy";
            else if (neworder.typeorder == 1) typeorder = "sell";
            else if (neworder.typeorder == 2) typeorder = "buylimit";
            else if (neworder.typeorder == 3) typeorder = "selllimit";
            else if (neworder.typeorder == 4) typeorder = "buystop";
            else if (neworder.typeorder == 5) typeorder = "sellstop";
            if (mode == "absolute")
                result += typeorder + " " + neworder.symbol + " " + neworder.size + " " + neworder.openprice + " " + neworder.stoploss + " " + neworder.takeprofit + " " + neworder.username + " " + namemt4;
            else if (mode == "scalping")
                result += typeorder + " " + neworder.symbol + " " + neworder.size + " " + neworder.openprice + " -1 -1 " + neworder.username + " " + namemt4;
            return result;
        }
    }
}
