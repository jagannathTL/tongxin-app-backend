using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Service.ViewModel;

namespace Service
{
    public class OrderService
    {
        static object obj = new object();
        public bool addGps(string mobile, int productId, bool isOrder)
        {
            bool flag = false;
            lock (obj)
            {

                using (var ctx = new ShtxSms2008Entities())
                {
                    try
                    {
                        CustomerBase cb = ctx.CustomerBases.FirstOrDefault(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                        if (cb != null)
                        {
                            string tel = cb.Tel;
                            if (isOrder)
                            {
                                //订阅
                                var count = ctx.Gps.Count(o => o.Tel == tel && o.ProductID == productId);
                                if (count > 0)
                                {
                                    return true;
                                }
                                else
                                {
                                    var product = ctx.SmsProducts.FirstOrDefault(o => o.ProductId == productId);
                                    if (product != null)
                                    {
                                        Gp gps = new Gp();
                                        gps.MarketID = product.MarketId;
                                        gps.Tel = tel;
                                        gps.ProductID = productId;
                                        gps.RoleID = 15;
                                        gps.ProductNum = product.ProductNum;
                                        gps.ProductSum = product.ProductSum;
                                        gps.ProductKind = product.ProductKind;
                                        ctx.Gps.Add(gps);
                                        ctx.SaveChanges();
                                        flag = true;
                                    }
                                }
                            }
                            else
                            {
                                //取消
                                Gp gps = ctx.Gps.FirstOrDefault(o => o.Tel == tel && o.ProductID == productId);
                                if (gps != null)
                                {
                                    ctx.Gps.Remove(gps);
                                    ctx.SaveChanges();
                                    flag = true;
                                }
                                else
                                    return true;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
            return flag;
        }

        public List<OrderVM> GetMyOrder(string mobile)
        {
            var list = new List<OrderVM>();
            List<int> xhmarketIds = new List<int>();
            var xhMarkets = new Dictionary<int, string>();
            using (var ctx = new ShtxSms2008Entities())
            {
                var groups = ctx.XHMarketGroups.Where(o => (o.Flag == 1 || o.Flag == 5) && o.IsForApp).Select(o => o.GroupID).ToList();
                CustomerBase cb = ctx.CustomerBases.FirstOrDefault(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                if (cb != null)
                {
                    mobile = cb.Tel;

                    foreach (var group in groups)
                    {
                        var markets = ctx.Markets.Where(o => o.GroupID == group).ToList();
                        foreach (var m in markets)
                        {
                            xhmarketIds.Add(m.MarketId);
                            xhMarkets.Add(m.MarketId, m.MarketName);
                        }
                    }

                    var orderIds = ctx.Gps.Where(o => o.Tel == mobile && xhmarketIds.Contains(o.MarketID.Value)).Select(o => o.ProductID.Value).ToList();

                    foreach (var productId in orderIds)
                    {
                        var product = ctx.SmsProducts.FirstOrDefault(o => o.ProductId == productId);
                        if (product != null)
                        {
                            OrderVM order = new OrderVM();
                            order.ProductId = product.ProductId;
                            order.ProductName = product.ProductName;
                            order.MarketName = xhMarkets[product.MarketId.Value];
                            list.Add(order);
                        }
                    }
                }
            }
            return list.OrderBy(o => o.MarketName).ToList();
        }



        public bool renew(string mobile)
        {
            bool flag = false;
            using (var ctx = new ShtxSms2008Entities())
            {
                CustomerExtend ce = ctx.CustomerExtend.FirstOrDefault(o => o.SendInterFace == 102 && o.Tel.Contains(mobile));
                if (ce != null)
                {
                    DateTime endTime = ce.EndDate.Value;
                    ce.FirstDate = endTime;
                    ce.EndDate = endTime.AddYears(1);
                    ce.Valid = true;
                    ce.CusKind = 1;
                    ctx.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }
    }
}
