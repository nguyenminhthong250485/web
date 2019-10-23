using HtmlAgilityPack;
using RestSharp;
using System.Collections.Generic;
using System.Threading;
namespace TradeFollowLiteForex
{
    class Program
    {
        private static DBUtils db_mastertrader = new DBUtils("thong250485");
        static void Main(string[] args)
        {
            Thread RUBI;
            RUBI = new Thread(delegate ()
            {
                while (true)
                {
                    var client = new RestClient("https://my.liteforex.com");
                    var request = new RestRequest("vi/traders/trades?id=793606", Method.GET);//@RUBI
                    IRestResponse response = client.Execute(request);
                    string html = response.Content;
                    HtmlNodeCollection div_content_rows = Util.HtmlGetNodeCollection(html, "//div[@class='content_row']");
                    List<order> orders = new List<order>();
                    foreach (HtmlNode div_content_row in div_content_rows)
                    {
                        string html_div_content_row = div_content_row.InnerHtml;
                        HtmlNodeCollection div_content_cols = Util.HtmlGetNodeCollection(html_div_content_row, "//div[@class='content_col']");
                        order neworder = new order();
                        neworder.symbol = div_content_cols[0].InnerText.Replace(" ", "").Replace("\n", "");
                        neworder.typeorder = div_content_cols[1].InnerText.Replace(" ", "").Replace("\n", "");
                        neworder.size = div_content_cols[2].InnerText.Replace(" ", "").Replace("\n", "");
                        neworder.datetime = div_content_cols[3].InnerText.Replace(" ", "").Replace("\n", "");
                        neworder.openprice = div_content_cols[4].InnerText.Replace(" ", "").Replace("\n", "");
                        neworder.currentprice = div_content_cols[5].InnerText.Replace(" ", "").Replace("\n", "");
                        neworder.stoploss = div_content_cols[6].InnerText.Replace(" ", "").Replace("\n", "");
                        neworder.takeprofit = div_content_cols[7].InnerText.Replace(" ", "").Replace("\n", "");
                        neworder.profit = div_content_cols[8].InnerText.Replace(" ", "").Replace("\n", "").Replace("Lợinhuận", "");
                        orders.Add(neworder);
                        db_mastertrader.InsertOrder(neworder, "Lite", "RUBI");
                    }
                    Thread.Sleep(1000);
                }
            });
            RUBI.Start();
        }
    }
}
