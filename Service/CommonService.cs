using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CommonService
    {
        public static string ChangePrice(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";
            str = str.Trim();
            if (str == "跌0" || str == "涨0")
                return "0";
            if (str == "***")
                return str;
            str = str.Replace("跌", "-").Replace("涨", "");
            return str;
        }
    }
}
