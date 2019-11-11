using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
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
    struct mastertrader
    {
        public string sourcedata;
        public string username;
        public string symbol;
        public int typeorder;
        public double size;
        public string datetime;
        public double openprice;
        public double currentprice;
        public double stoploss;
        public double takeprofit;
        public double profit;
        public string md5;
    }

    struct copytrade
    {
        public int id;
        public string broker;
        public int numbermt4;
        public string namemt4;
        public string symbol;
        public int typeorder;
        public double size;
        public string opentime;
        public string closetime;
        public double openprice;
        public double closeprice;
        public double stoploss;
        public double takeprofit;
        public string comment;
        public double profit;
        public int pip;
        public int ticket;
        public int status;
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
        private void CloseDB()
        {
            if (conn.State != System.Data.ConnectionState.Closed)
            {
                conn.Close();
            }
        }
        public string InsertMastertrader(mastertrader newmastertrader,string sourcedata,string username)
        {
            string sql = "INSERT INTO mastertrader (sourcedata,username,symbol,typeorder,size,datetime,openprice,currentprice,stoploss,takeprofit,profit,md5)"
                + " VALUES (@sourcedata,@username,@symbol,@typeorder,@size,@datetime,@openprice,@currentprice,@stoploss,@takeprofit,@profit,@md5)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sourcedata", sourcedata);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@symbol", newmastertrader.symbol);
            cmd.Parameters.AddWithValue("@size", newmastertrader.size);
            cmd.Parameters.AddWithValue("@datetime", newmastertrader.datetime);
            cmd.Parameters.AddWithValue("@openprice", newmastertrader.openprice);
            cmd.Parameters.AddWithValue("@currentprice", newmastertrader.currentprice);
            cmd.Parameters.AddWithValue("@stoploss", newmastertrader.stoploss);
            cmd.Parameters.AddWithValue("@takeprofit", newmastertrader.takeprofit);
            cmd.Parameters.AddWithValue("@profit", newmastertrader.profit);
            cmd.Parameters.AddWithValue("@typeorder", newmastertrader.typeorder);

            string str_hashmd5 = newmastertrader.datetime + username + sourcedata;
            string hashmd5 = Util.MD5(str_hashmd5);
            cmd.Parameters.AddWithValue("@md5", hashmd5);
            if (CheckExistMastertrader(hashmd5)) return "duplicate";
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
                error_mes = "Insertmastertrader: " + error.Message;
                return "error";
            }
        }
        public List<mastertrader> GetMastertraderByUsername(string username)
        {
            List<mastertrader> result = new List<mastertrader>();
            ConnectDB();
            string sql = "SELECT * FROM mastertrader WHERE username=@username";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);
            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mastertrader tam = new mastertrader();
                        tam.sourcedata = reader.GetString("sourcedata");
                        tam.username = reader.GetString("username");
                        tam.symbol = reader.GetString("symbol");
                        tam.typeorder = reader.GetInt16("typeorder");
                        tam.size = reader.GetDouble("size");
                        tam.datetime = reader.GetString("datetime");
                        tam.openprice = reader.GetDouble("openprice");
                        tam.currentprice = reader.GetDouble("currentprice");
                        tam.stoploss = reader.GetDouble("stoploss");
                        tam.takeprofit = reader.GetDouble("takeprofit");
                        tam.profit = reader.GetDouble("profit");
                        tam.md5 = reader.GetString("md5");
                        result.Add(tam);
                    }
                }
                error_mes = null;
            }
            catch (MySqlException error)
            {
                error_mes = "GetMastertraderByUsername: " + error.Message;
            }
            CloseDB();
            return result;
        }
        public bool CheckExistMastertrader(string md5)
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
                error_mes = "CheckExistmastertrader: " + error.Message;
            }
            CloseDB();
            if (result == "") return false;
            else return true;
        }
        public bool DeleteClosemastertrader(string md5,string str_md5_orderopen)
        {
            if(str_md5_orderopen.Contains(md5)==false)
            {
                ConnectDB();
                string sql = "DELETE FROM mastertrader WHERE md5=@md5";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@md5", md5);
                try
                {
                    cmd.ExecuteNonQuery();
                    error_mes = null;
                }
                catch (MySqlException error)
                {
                    CloseDB();
                    error_mes = "DeleteClosemastertrader: " + error.Message;
                    return false;
                }
                sql = "ALTER TABLE mastertrader AUTO_INCREMENT = 1";
                cmd = new MySqlCommand(sql, conn);
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
                    error_mes = "ALTER TABLE mastertrader: " + error.Message;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public List<mastertrader> GetNewMastertrader(string oldmd5)
        {
            List<mastertrader> result = new List<mastertrader>();
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
                            mastertrader tam = new mastertrader();
                            tam.sourcedata = reader.GetString("sourcedata");
                            tam.username = reader.GetString("username");
                            tam.symbol = reader.GetString("symbol");
                            tam.typeorder = reader.GetInt16("typeorder");
                            tam.size = reader.GetDouble("size");
                            tam.datetime = reader.GetString("datetime");
                            tam.openprice = reader.GetDouble("openprice");
                            tam.currentprice = reader.GetDouble("currentprice");
                            tam.stoploss = reader.GetDouble("stoploss");
                            tam.takeprofit = reader.GetDouble("takeprofit");
                            tam.profit = reader.GetDouble("profit");
                            tam.md5 = reader.GetString("md5");
                            result.Add(tam);
                        }
                    }
                }
                error_mes = null;
            }
            catch (MySqlException error)
            {
                error_mes = "Getnewmastertrader: " + error.Message;
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
        public string SendSignalOrder(mastertrader newmastertrader,string namemt4,string mode= "absolute")
        {
            string result = "";//buy sb xauusd size 0.01 price 0 pricesl 1500 pricetp 1400 comment mastertrader mt4 l
            string typeorder = "typewrong";
            if (newmastertrader.typeorder == 0) typeorder = "buy";
            else if (newmastertrader.typeorder == 1) typeorder = "sell";
            else if (newmastertrader.typeorder == 2) typeorder = "buylimit";
            else if (newmastertrader.typeorder == 3) typeorder = "selllimit";
            else if (newmastertrader.typeorder == 4) typeorder = "buystop";
            else if (newmastertrader.typeorder == 5) typeorder = "sellstop";
            if (mode == "absolute")
                result += typeorder + " sb " + newmastertrader.symbol + " size " + newmastertrader.size + " price " + newmastertrader.openprice + " pricesl " + newmastertrader.stoploss + " pricetp " + newmastertrader.takeprofit + " comment " + newmastertrader.username + " mt4 " + namemt4;
            else if (mode == "scalping")
                result += typeorder + " sb " + newmastertrader.symbol + " size " + newmastertrader.size + " price " + newmastertrader.openprice + " pricesl -1 " + " pricetp -1 " + " comment " + newmastertrader.username + " mt4 " + namemt4;
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
        public string SendOrderToTrade(mastertrader newmastertrader, string namemt4, string mode = "absolute")
        {
            string result = "";//buy sb xauusd size 0.01 price 0 pricesl 1500 pricetp 1400 comment mastertrader mt4 l
            string typeorder = "typewrong";
            if (newmastertrader.typeorder == 0) typeorder = "buy";
            else if (newmastertrader.typeorder == 1) typeorder = "sell";
            else if (newmastertrader.typeorder == 2) typeorder = "buylimit";
            else if (newmastertrader.typeorder == 3) typeorder = "selllimit";
            else if (newmastertrader.typeorder == 4) typeorder = "buystop";
            else if (newmastertrader.typeorder == 5) typeorder = "sellstop";
            if (mode == "absolute")
                result += typeorder + " " + newmastertrader.symbol + " " + newmastertrader.size + " " + newmastertrader.openprice + " " + newmastertrader.stoploss + " " + newmastertrader.takeprofit + " " + newmastertrader.md5 + " " + namemt4;
            else if (mode == "scalping")
                result += typeorder + " " + newmastertrader.symbol + " " + newmastertrader.size + " " + newmastertrader.openprice + " -1 -1 " + newmastertrader.username + " " + namemt4;
            return result;
        }
        public List<copytrade> GetCopytradeByComment(string comment)
        {
            List<copytrade> copytrades = new List<copytrade>();
            ConnectDB();
            string sql = "SELECT * FROM copytrade WHERE comment=@comment";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@comment", comment);
            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        copytrade result = new copytrade();
                        result.id = reader.GetInt16("id");
                        result.broker = reader.GetString("broker");
                        result.numbermt4 = reader.GetInt32("numbermt4");
                        result.namemt4 = reader.GetString("namemt4");
                        result.symbol = reader.GetString("symbol");
                        result.typeorder = reader.GetInt16("typeorder");
                        result.size = reader.GetDouble("size");
                        result.opentime = reader.GetString("opentime");
                        result.closetime = reader.GetString("closetime");
                        result.openprice = reader.GetDouble("openprice");
                        result.closeprice = reader.GetDouble("closeprice");
                        result.stoploss = reader.GetDouble("stoploss");
                        result.takeprofit = reader.GetDouble("takeprofit");
                        result.comment = reader.GetString("comment");
                        result.profit = reader.GetDouble("profit");
                        result.pip = reader.GetInt32("pip");
                        result.ticket = reader.GetInt32("ticket");
                        result.status = reader.GetInt16("status");
                        copytrades.Add(result);
                    }
                }
                error_mes = null;
            }
            catch (MySqlException error)
            {
                error_mes = "GetMastertraderByUsername: " + error.Message;
            }
            CloseDB();
            return copytrades;
        }
        public string SendOrderToClose(int ticket,string namemt4)
        {
            string result = "closeticket " + ticket.ToString() + " " + namemt4;
            return result;
        }
    }
}
