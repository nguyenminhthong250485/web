using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeFollowLiteForex
{
    public class order
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

        public static int GetSecondOpenPrice(order neworder)
        {
            string str_datetme = neworder.datetime;
            long tickorder = DateTime.Parse(str_datetme).Ticks;
            string[] arr_datetime = str_datetme.Split(':');
            int result = int.Parse(arr_datetime[1]) * 60 + int.Parse(arr_datetime[2]);
            return result;
        }
        public static bool ExistNewOrder(List<order> orders,int delaycon=10)
        {
            order lastorder = orders[0];
            int secondorder = GetSecondOpenPrice(lastorder);
            if (secondorder < 11) secondorder = 60 * 60 + secondorder;
            int secondnow = DateTime.Now.Minute * 60 + DateTime.Now.Second;
            int delay = secondnow - secondorder;
            if (delay <= delaycon) return true;
            else return false;
        }
    }
}
