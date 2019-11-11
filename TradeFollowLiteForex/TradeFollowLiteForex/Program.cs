using HtmlAgilityPack;
using RestSharp;
using System.Collections.Generic;
using System.Threading;
using System;
using Telegram.Bot;
namespace TradeFollowLiteForex
{
    class Program
    {
        public static string token = "722735016:AAE9duoIYHuREaHQuO_KmNBv6j-tzpugcS8";
        public static TelegramBotClient Bot;
        private static DBUtils db_program = new DBUtils("program");
        private static DBUtils db_Kudji = new DBUtils("Kudji");
        private static DBUtils db_RUBI = new DBUtils("RUBI");
        private static DBUtils db_anhsangfx = new DBUtils("anhsangfx");
        private static DBUtils db_Alexngo_vn = new DBUtils("Alexngo_vn");//Happytrader
        private static DBUtils db_Happytrader = new DBUtils("Happytrader");
        private static DBUtils db_SendOrderToTrade = new DBUtils("SendOrderToTrade");
        private static DBUtils db_SendSignal = new DBUtils("SendSignal");
        private static DBUtils db_HerryDuc = new DBUtils("thong250485");
        //Kudji,458257
        static void Main(string[] args)
        {
            Bot = new TelegramBotClient(token);

            Thread SendOrderToTrade;
            SendOrderToTrade = new Thread(delegate ()
            {
                Thread.Sleep(1000 * 60);
                string OldMD5 = db_SendOrderToTrade.GetOldMD5();
                try
                {
                    while (true)
                    {
                        List<mastertrader> newmastertraders = db_SendOrderToTrade.GetNewMastertrader(OldMD5);
                        if (newmastertraders.Count == 0) continue;
                        foreach (mastertrader newmastertrader in newmastertraders)
                        {
                            if (newmastertrader.username == "Alexngo_vn" || newmastertrader.username == "Happytrader")
                            {
                                nonlive newmastertradertrade = new nonlive();
                                newmastertradertrade.chatid = 635860813;
                                newmastertradertrade.username = "thong250485";
                                string command = db_SendOrderToTrade.SendOrderToTrade(newmastertrader, "thong250485@l");
                                OldMD5 += newmastertrader.md5 + "|";
                                string keyword = command.Split(' ')[0];
                                newmastertradertrade.keyword = keyword;
                                newmastertradertrade.name_mt4 = "thong250485@l";
                                newmastertradertrade.command = command;
                                newmastertradertrade.datetime = DateTime.Now.ToString();
                                db_SendOrderToTrade.InsertNonLive(newmastertradertrade);
                            }
                            if (newmastertrader.username == "RUBI" || newmastertrader.username == "Kudji")
                            {
                                nonlive newmastertradertrade = new nonlive();
                                newmastertradertrade.chatid = 635860813;
                                newmastertradertrade.username = "thong250485";
                                string command = db_SendOrderToTrade.SendOrderToTrade(newmastertrader, "thong250485@l");
                                OldMD5 += newmastertrader.md5 + "|";
                                string keyword = command.Split(' ')[0];
                                newmastertradertrade.keyword = keyword;
                                newmastertradertrade.name_mt4 = "thong250485@l";
                                newmastertradertrade.command = command;
                                newmastertradertrade.datetime = DateTime.Now.ToString();
                                db_SendOrderToTrade.InsertNonLive(newmastertradertrade);
                            }
                        }
                        Thread.Sleep(500);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SendOrderToTrade: " + ex.StackTrace);
                }
            });
            SendOrderToTrade.Start();

            Thread Kudji;
            bool runKudji = true;
            Kudji = new Thread(delegate ()
            {
                while (true)
                {
                    string str_hashmd5 = "";
                    var client = new RestClient("https://my.liteforex.com");
                    var request = new RestRequest("vi/traders/trades?id=458257", Method.GET);//@Kudji
                    IRestResponse response = client.Execute(request);
                    string html = response.Content;
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        if (div_content_rows.Count == 0) continue;
                        List<mastertrader> newmastertraders = new List<mastertrader>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            mastertrader newmastertrader = new mastertrader();
                            newmastertrader.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            newmastertrader.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.currentprice = currentprice;
                            if (typeorder == "Mua") newmastertrader.typeorder = 0;
                            else if (typeorder == "Bán") newmastertrader.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) newmastertrader.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) newmastertrader.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) newmastertrader.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) newmastertrader.typeorder = 5;
                            else newmastertrader.typeorder = 6;
                            newmastertrader.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            newmastertraders.Add(newmastertrader);

                            str_hashmd5 += Util.MD5(newmastertrader.datetime + "Kudji" + "Lite") + ",";

                        }
                        foreach(mastertrader newmastertrader in newmastertraders)
                        {
                            string info_InsertOrder = db_Kudji.InsertMastertrader(newmastertrader, "Lite", "Kudji");
                            if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@Kudji");
                                return;
                            }
                        }
                        List<mastertrader> MastertraderKudjis = db_Kudji.GetMastertraderByUsername("Kudji");
                        foreach (mastertrader MastertraderKudji in MastertraderKudjis)
                        {
                            if(db_Kudji.DeleteClosemastertrader(MastertraderKudji.md5,str_hashmd5))
                            {
                                string md5del = MastertraderKudji.md5;
                                List<copytrade> CopyTrades = db_Kudji.GetCopytradeByComment(md5del);
                                foreach(copytrade CopyTrade in CopyTrades)
                                {
                                    int ticketclose = CopyTrade.ticket;
                                    string namemt4close = CopyTrade.namemt4;
                                    db_Kudji.SendOrderToClose(ticketclose, namemt4close);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Bot.SendTextMessageAsync(635860813, "Lite@Kudji: "+ ex.StackTrace);
                        runKudji = false;
                    }
                    Thread.Sleep(500);
                }
            });
            Kudji.Start();
            if (runKudji == false)
                Kudji.Abort();

            Thread RUBI; 
            bool runRUBI = true;
            RUBI = new Thread(delegate ()
            {
                while (true)
                {
                    string str_hashmd5 = "";
                    var client = new RestClient("https://my.liteforex.com");
                    var request = new RestRequest("vi/traders/trades?id=793606", Method.GET);//@RUBI
                    IRestResponse response = client.Execute(request);
                    string html = response.Content;
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        if (div_content_rows.Count == 0) continue;
                        List<mastertrader> newmastertraders = new List<mastertrader>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            mastertrader newmastertrader = new mastertrader();
                            newmastertrader.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            newmastertrader.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.currentprice = currentprice;
                            if (typeorder == "Mua") newmastertrader.typeorder = 0;
                            else if (typeorder == "Bán") newmastertrader.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) newmastertrader.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) newmastertrader.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) newmastertrader.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) newmastertrader.typeorder = 5;
                            else newmastertrader.typeorder = 6;
                            newmastertrader.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            newmastertraders.Add(newmastertrader);
                            string info_InsertOrder = db_RUBI.InsertMastertrader(newmastertraders[0], "Lite", "");

                            str_hashmd5 += Util.MD5(newmastertrader.datetime + "RUBI" + "Lite") + ",";

                            if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@RUBI");
                                runRUBI = false;
                                break;
                            }
                        }
                        List<mastertrader> MastertraderRUBIs = db_RUBI.GetMastertraderByUsername("RUBI");
                        foreach (mastertrader MastertraderRUBI in MastertraderRUBIs)
                        {
                            if (db_RUBI.DeleteClosemastertrader(MastertraderRUBI.md5, str_hashmd5))
                            {
                                string md5del = MastertraderRUBI.md5;
                                List<copytrade> CopyTrades = db_RUBI.GetCopytradeByComment(md5del);
                                foreach (copytrade CopyTrade in CopyTrades)
                                {
                                    int ticketclose = CopyTrade.ticket;
                                    string namemt4close = CopyTrade.namemt4;
                                    db_RUBI.SendOrderToClose(ticketclose, namemt4close);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Bot.SendTextMessageAsync(635860813, "Lite@RUBI: " + ex.StackTrace);
                        runRUBI = false;
                    }
                    Thread.Sleep(500);
                }
            });
            //RUBI.Start();
            if (runRUBI == false)
                RUBI.Abort();


            Thread anhsangfx;
            bool runanhsangfx = true;
            anhsangfx = new Thread(delegate ()
            {
                while (true)
                {
                    var client = new RestClient("https://my.liteforex.com");
                    var request = new RestRequest("vi/traders/trades?id=846465", Method.GET);//@Happytrader
                    IRestResponse response = client.Execute(request);
                    string html = response.Content;
                    string str_hashmd5 = "";
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        if (div_content_rows.Count == 0) continue;
                        List<mastertrader> newmastertraders = new List<mastertrader>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            mastertrader newmastertrader = new mastertrader();
                            newmastertrader.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            newmastertrader.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.currentprice = currentprice;
                            if (typeorder == "Mua") newmastertrader.typeorder = 0;
                            else if (typeorder == "Bán") newmastertrader.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) newmastertrader.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) newmastertrader.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) newmastertrader.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) newmastertrader.typeorder = 5;
                            else newmastertrader.typeorder = 6;
                            newmastertrader.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            newmastertraders.Add(newmastertrader);
                            string info_InsertOrder = db_anhsangfx.InsertMastertrader(newmastertraders[0], "Lite", "");

                            str_hashmd5 += Util.MD5(newmastertrader.datetime + "anhsangfx" + "Lite") + ",";

                            if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@anhsangfx");
                                runanhsangfx = false;
                                break;
                            }
                        }
                        List<mastertrader> Mastertraderanhsangfxs = db_anhsangfx.GetMastertraderByUsername("anhsangfx");
                        foreach (mastertrader Mastertraderanhsangfx in Mastertraderanhsangfxs)
                        {
                            if (db_anhsangfx.DeleteClosemastertrader(Mastertraderanhsangfx.md5, str_hashmd5))
                            {
                                string md5del = Mastertraderanhsangfx.md5;
                                List<copytrade> CopyTrades = db_anhsangfx.GetCopytradeByComment(md5del);
                                foreach (copytrade CopyTrade in CopyTrades)
                                {
                                    int ticketclose = CopyTrade.ticket;
                                    string namemt4close = CopyTrade.namemt4;
                                    db_anhsangfx.SendOrderToClose(ticketclose, namemt4close);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Bot.SendTextMessageAsync(635860813, "Lite@anhsangfx: " + ex.StackTrace);
                        runanhsangfx = false;
                    }
                    Thread.Sleep(500);
                }
            });
            //anhsangfx.Start();
            if (runanhsangfx == false)
                anhsangfx.Abort();

            Thread Alexngo_vn;
            bool runAlexngo_vn = true;
            Alexngo_vn = new Thread(delegate ()
            {
                while (true)
                {
                    var client = new RestClient("https://my.liteforex.com");
                    var request = new RestRequest("vi/traders/trades?id=805126", Method.GET);//@Anthony_Khai
                    IRestResponse response = client.Execute(request);
                    string html = response.Content;
                    string str_hashmd5 = "";
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        if (div_content_rows.Count == 0) continue;
                        List<mastertrader> newmastertraders = new List<mastertrader>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            mastertrader newmastertrader = new mastertrader();
                            newmastertrader.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            newmastertrader.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.currentprice = currentprice;
                            if (typeorder == "Mua") newmastertrader.typeorder = 0;
                            else if (typeorder == "Bán") newmastertrader.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) newmastertrader.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) newmastertrader.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) newmastertrader.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) newmastertrader.typeorder = 5;
                            else newmastertrader.typeorder = 6;
                            newmastertrader.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            newmastertraders.Add(newmastertrader);
                            string info_InsertOrder = db_Alexngo_vn.InsertMastertrader(newmastertraders[0], "Lite", "");

                            str_hashmd5 += Util.MD5(newmastertrader.datetime + "Alexngo_vn" + "Lite") + ",";

                            if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@Alexngo_vn");
                                runAlexngo_vn = false;
                                break;
                            }
                        }
                        List<mastertrader> MastertraderAlexngo_vns = db_Alexngo_vn.GetMastertraderByUsername("Alexngo_vn");
                        foreach (mastertrader MastertraderAlexngo_vn in MastertraderAlexngo_vns)
                        {
                            if (db_Alexngo_vn.DeleteClosemastertrader(MastertraderAlexngo_vn.md5, str_hashmd5))
                            {
                                string md5del = MastertraderAlexngo_vn.md5;
                                List<copytrade> CopyTrades = db_Alexngo_vn.GetCopytradeByComment(md5del);
                                foreach (copytrade CopyTrade in CopyTrades)
                                {
                                    int ticketclose = CopyTrade.ticket;
                                    string namemt4close = CopyTrade.namemt4;
                                    db_Alexngo_vn.SendOrderToClose(ticketclose, namemt4close);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Bot.SendTextMessageAsync(635860813, "Lite@Alexngo_vn: " + ex.StackTrace);
                        runAlexngo_vn = false;
                    }
                    Thread.Sleep(500);
                }
            });
            //Alexngo_vn.Start();
            if (runAlexngo_vn == false)
                Alexngo_vn.Abort();

            Thread Happytrader;
            bool runHappytrader = true;
            Happytrader = new Thread(delegate ()
            {
                while (true)
                {
                    var client = new RestClient("https://my.liteforex.com");
                    var request = new RestRequest("vi/traders/trades?id=743781", Method.GET);//@Happytrader
                    IRestResponse response = client.Execute(request);
                    string html = response.Content;
                    string str_hashmd5 = "";
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        if (div_content_rows.Count == 0) continue;
                        List<mastertrader> newmastertraders = new List<mastertrader>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            mastertrader newmastertrader = new mastertrader();
                            newmastertrader.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            newmastertrader.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.currentprice = currentprice;
                            if (typeorder == "Mua") newmastertrader.typeorder = 0;
                            else if (typeorder == "Bán") newmastertrader.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) newmastertrader.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) newmastertrader.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) newmastertrader.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) newmastertrader.typeorder = 5;
                            else newmastertrader.typeorder = 6;
                            newmastertrader.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            newmastertraders.Add(newmastertrader);
                            string info_InsertOrder = db_Happytrader.InsertMastertrader(newmastertraders[0], "Lite", "");

                            str_hashmd5 += Util.MD5(newmastertrader.datetime + "Happytrader" + "Lite") + ",";

                            if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@Happytrader");
                                runHappytrader = false;
                                break;
                            }
                        }
                        List<mastertrader> MastertraderHappytraders = db_Happytrader.GetMastertraderByUsername("Happytrader");
                        foreach (mastertrader MastertraderHappytrader in MastertraderHappytraders)
                        {
                            if (db_Happytrader.DeleteClosemastertrader(MastertraderHappytrader.md5, str_hashmd5))
                            {
                                string md5del = MastertraderHappytrader.md5;
                                List<copytrade> CopyTrades = db_Happytrader.GetCopytradeByComment(md5del);
                                foreach (copytrade CopyTrade in CopyTrades)
                                {
                                    int ticketclose = CopyTrade.ticket;
                                    string namemt4close = CopyTrade.namemt4;
                                    db_Happytrader.SendOrderToClose(ticketclose, namemt4close);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Bot.SendTextMessageAsync(635860813, "Lite@Happytrader: " + ex.StackTrace);
                        runHappytrader = false;
                    }
                    Thread.Sleep(500);
                }
            });
            //Happytrader.Start();
            if (runHappytrader == false)
                Happytrader.Abort();

            Thread HerryDuc;
            bool runHerryDuc = true;
            HerryDuc = new Thread(delegate ()
            {
                while (true)
                {
                    var client = new RestClient("https://my.liteforex.com");
                    var request = new RestRequest("vi/traders/trades?id=758051", Method.GET);//@HerryDuc
                    IRestResponse response = client.Execute(request);
                    string html = response.Content;
                    string str_hashmd5 = "";
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        if (div_content_rows.Count == 0) continue;
                        List<mastertrader> newmastertraders = new List<mastertrader>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            mastertrader newmastertrader = new mastertrader();
                            newmastertrader.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            newmastertrader.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.currentprice = currentprice;
                            if (typeorder == "Mua") newmastertrader.typeorder = 0;
                            else if (typeorder == "Bán") newmastertrader.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) newmastertrader.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) newmastertrader.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) newmastertrader.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) newmastertrader.typeorder = 5;
                            else newmastertrader.typeorder = 6;
                            newmastertrader.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            newmastertrader.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            newmastertraders.Add(newmastertrader);
                            string info_InsertOrder = db_HerryDuc.InsertMastertrader(newmastertraders[0], "Lite", "");

                            str_hashmd5 += Util.MD5(newmastertrader.datetime + "HerryDuc" + "Lite") + ",";

                            if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@HerryDuc");
                                runHerryDuc = false;
                                break;
                            }
                        }
                        List<mastertrader> MastertraderHerryDucs = db_HerryDuc.GetMastertraderByUsername("HerryDuc");
                        foreach (mastertrader MastertraderHerryDuc in MastertraderHerryDucs)
                        {
                            if (db_HerryDuc.DeleteClosemastertrader(MastertraderHerryDuc.md5, str_hashmd5))
                            {
                                string md5del = MastertraderHerryDuc.md5;
                                List<copytrade> CopyTrades = db_HerryDuc.GetCopytradeByComment(md5del);
                                foreach (copytrade CopyTrade in CopyTrades)
                                {
                                    int ticketclose = CopyTrade.ticket;
                                    string namemt4close = CopyTrade.namemt4;
                                    db_HerryDuc.SendOrderToClose(ticketclose, namemt4close);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("HerryDuc: " + ex.Message);
                    }
                    Thread.Sleep(500);
                }
            });
            //HerryDuc.Start();
            if (runHerryDuc == false)
                HerryDuc.Abort();
        }
    }
}
