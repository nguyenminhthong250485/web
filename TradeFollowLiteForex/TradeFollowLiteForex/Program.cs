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
            Thread SendSignal;
            SendSignal = new Thread(delegate ()
            {
                string OldMD5 = db_SendSignal.GetOldMD5();
                try
                {
                    while (true)
                    {
                        order neworder = new order();
                        neworder = db_SendSignal.GetNewOrder(OldMD5);
                        if (neworder == null) continue;
                        if (neworder.username == "RUBI"|| neworder.username == "Kudji")
                        {
                            string signal = db_SendSignal.SendSignalOrder(neworder, "l", "scalping");
                            OldMD5 += neworder.md5 + "|";
                            Bot.SendTextMessageAsync(635860813, signal);
                        }
                        Thread.Sleep(500);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("SendSignal: "+ex.Message);
                    Console.WriteLine("SendSignal: " + ex.StackTrace);
                }
            });
            //SendSignal.Start();

            Thread SendOrderToTrade;
            SendOrderToTrade = new Thread(delegate ()
            {
                string OldMD5 = db_SendOrderToTrade.GetOldMD5();
                try
                {
                    while (true)
                    {
                        order neworder = new order();
                        neworder = db_SendOrderToTrade.GetNewOrder(OldMD5);
                        if (neworder == null) continue;
                        if (neworder.username == "anhsangfx")
                        {
                            nonlive newordertrade = new nonlive();
                            newordertrade.chatid = 635860813;
                            newordertrade.username = "thong250485";
                            string command = db_SendOrderToTrade.SendOrderToTrade(neworder, "thong250485@l", "scalping");
                            OldMD5 += neworder.md5 + "|";
                            string keyword = command.Split(' ')[0];
                            newordertrade.keyword = keyword;
                            newordertrade.name_mt4 = "thong250485@l";
                            newordertrade.command = command;
                            newordertrade.datetime = DateTime.Now.ToString();
                            db_SendOrderToTrade.InsertNonLive(newordertrade);
                        }
                        if(neworder.username == "Alexngo_vn" || neworder.username == "Happytrader")
                        {

                        }
                        if (neworder.username == "RUBI" || neworder.username == "Kudji")
                        {
                            nonlive newordertrade = new nonlive();
                            newordertrade.chatid = 635860813;
                            newordertrade.username = "thong250485";
                            string command = db_SendOrderToTrade.SendOrderToTrade(neworder, "thong250485@l");
                            OldMD5 += neworder.md5 + "|";
                            string keyword = command.Split(' ')[0];
                            newordertrade.keyword = keyword;
                            newordertrade.name_mt4 = "thong250485@l";
                            newordertrade.command = command;
                            newordertrade.datetime = DateTime.Now.ToString();
                            db_SendOrderToTrade.InsertNonLive(newordertrade);
                        }
                        Thread.Sleep(500);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SendOrderToTrade: " + ex.Message);
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
                    var client = new RestClient("https://my.liteforex.com");
                    var request = new RestRequest("vi/traders/trades?id=458257", Method.GET);//@Kudji
                    IRestResponse response = client.Execute(request);
                    string html = response.Content;
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        if (div_content_rows.Count == 0) continue;
                        List<order> orders = new List<order>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            order neworder = new order();
                            neworder.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            neworder.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.currentprice = currentprice;
                            if (typeorder == "Mua") neworder.typeorder = 0;
                            else if (typeorder == "Bán") neworder.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) neworder.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) neworder.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) neworder.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) neworder.typeorder = 5;
                            else neworder.typeorder = 6;
                            neworder.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            orders.Add(neworder);
                            string info_InsertOrder = db_Kudji.InsertOrder(orders[0], "Lite", "Kudji");
                            if (info_InsertOrder == "duplicate")
                            {
                                break;
                            }
                            else if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@Kudji");
                                runKudji = false;
                                break;
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
                    var client = new RestClient("https://my.liteforex.com");
                    var request = new RestRequest("vi/traders/trades?id=793606", Method.GET);//@RUBI
                    IRestResponse response = client.Execute(request);
                    string html = response.Content;
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        if (div_content_rows.Count == 0) continue;
                        List<order> orders = new List<order>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            order neworder = new order();
                            neworder.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder= div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            neworder.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice= double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.openprice = openprice;
                            double currentprice= double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.currentprice = currentprice;
                            if (typeorder == "Mua") neworder.typeorder = 0;
                            else if (typeorder == "Bán") neworder.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) neworder.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) neworder.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) neworder.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) neworder.typeorder = 5;
                            else neworder.typeorder = 6;
                            neworder.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            orders.Add(neworder);
                            string info_InsertOrder = db_RUBI.InsertOrder(orders[0], "Lite", "RUBI");
                            if (info_InsertOrder == "duplicate")
                            {
                                break;
                            }
                            else if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@RUBI");
                                runRUBI = false;
                                break;
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Bot.SendTextMessageAsync(635860813, "Lite@RUBI: " + ex.StackTrace);
                        runRUBI = false;
                    }
                    Thread.Sleep(500);
                }
            });
            RUBI.Start();
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
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        List<order> orders = new List<order>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            order neworder = new order();
                            neworder.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            neworder.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.currentprice = currentprice;
                            if (typeorder == "Mua") neworder.typeorder = 0;
                            else if (typeorder == "Bán") neworder.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) neworder.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) neworder.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) neworder.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) neworder.typeorder = 5;
                            else neworder.typeorder = 6;
                            neworder.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            orders.Add(neworder);
                            string info_InsertOrder = db_anhsangfx.InsertOrder(orders[0], "Lite", "anhsangfx");
                            if (info_InsertOrder == "duplicate")
                            {
                                break;
                            }
                            else if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@anhsangfx");
                                runanhsangfx = false;
                                break;
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
            anhsangfx.Start();
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
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        if (div_content_rows.Count == 0) continue;
                        List<order> orders = new List<order>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            order neworder = new order();
                            neworder.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            neworder.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.currentprice = currentprice;
                            if (typeorder == "Mua") neworder.typeorder = 0;
                            else if (typeorder == "Bán") neworder.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) neworder.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) neworder.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) neworder.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) neworder.typeorder = 5;
                            else neworder.typeorder = 6;
                            neworder.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            orders.Add(neworder);
                            string info_InsertOrder = db_Alexngo_vn.InsertOrder(orders[0], "Lite", "Alexngo_vn");
                            if (info_InsertOrder == "duplicate")
                            {
                                break;
                            }
                            else if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@Alexngo_vn");
                                runAlexngo_vn = false;
                                break;
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
            Alexngo_vn.Start();
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
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        if (div_content_rows.Count == 0) continue;
                        List<order> orders = new List<order>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            order neworder = new order();
                            neworder.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            neworder.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.currentprice = currentprice;
                            if (typeorder == "Mua") neworder.typeorder = 0;
                            else if (typeorder == "Bán") neworder.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) neworder.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) neworder.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) neworder.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) neworder.typeorder = 5;
                            else neworder.typeorder = 6;
                            neworder.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            orders.Add(neworder);
                            string info_InsertOrder = db_Happytrader.InsertOrder(orders[0], "Lite", "Happytrader");
                            if (info_InsertOrder == "duplicate")
                            {
                                break;
                            }
                            else if (info_InsertOrder == "error")
                            {
                                Console.WriteLine("Lite@Happytrader");
                                runHappytrader = false;
                                break;
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
            Happytrader.Start();
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
                    if (html.Contains("Nhà giao dịch không có giao dịch mở hiện tại")) continue;
                    try
                    {
                        HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                        List<order> orders = new List<order>();
                        foreach (HtmlNode div_content_row in div_content_rows)
                        {
                            string html_div_content_row = div_content_row.InnerHtml;
                            HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                            order neworder = new order();
                            neworder.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                            string typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                            neworder.size = double.Parse(div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                            double openprice = double.Parse(div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.openprice = openprice;
                            double currentprice = double.Parse(div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.currentprice = currentprice;
                            if (typeorder == "Mua") neworder.typeorder = 0;
                            else if (typeorder == "Bán") neworder.typeorder = 1;
                            else if (typeorder == "Muatại" && openprice < currentprice) neworder.typeorder = 2;
                            else if (typeorder == "Bántại" && openprice > currentprice) neworder.typeorder = 3;
                            else if (typeorder == "Muatại" && openprice > currentprice) neworder.typeorder = 4;
                            else if (typeorder == "Bántại" && openprice < currentprice) neworder.typeorder = 5;
                            else neworder.typeorder = 6;
                            neworder.stoploss = double.Parse(div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.takeprofit = double.Parse(div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", ""));
                            neworder.profit = double.Parse(div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "").Replace("USD", ""));
                            orders.Add(neworder);
                            if (db_HerryDuc.InsertOrder(orders[0], "Lite", "HerryDuc") == "duplicate")
                            {
                                break;
                            }
                            if (db_HerryDuc.InsertOrder(orders[0], "Lite", "HerryDuc") == "error")
                            {
                                Console.WriteLine("Lite@HerryDuc");
                                runHerryDuc = false;
                                break;
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
