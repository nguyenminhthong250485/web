using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeFollowLiteForex
{
    public class order
    {
        public string symbol;
        public string typeorder;
        public string size;
        public string datetime;
        public string openprice;
        public string currentprice;
        public string stoploss;
        public string takeprofit;
        public string profit;

        public static int GetSecondOpenPrice(order neworder)
        {
            string str_datetme = neworder.datetime;
            string[] arr_datetime = str_datetme.Split(':');
            int result = int.Parse(arr_datetime[1]) * 60 + int.Parse(arr_datetime[2]);
            return result;
        }
        public static bool ExistNewOrder(List<order> orders,int delay=10)
        {
            order lastorder = orders[0];
            int seconddelay = GetSecondOpenPrice(lastorder);
            if (seconddelay <= delay) return true;
            else return false;
        }
    }
}
