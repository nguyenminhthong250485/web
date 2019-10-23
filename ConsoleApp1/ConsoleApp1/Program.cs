using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("https://my.liteforex.com");
            var request = new RestRequest("vi/traders/trades?id=793606", Method.GET);
            IRestResponse response = client.Execute(request);
            string html = response.Content;
        }
    }
}
