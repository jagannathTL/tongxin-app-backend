using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Service.ViewModel;

namespace Service
{
    public class PinglunService
    {
        public List<AppPingLunVM> GetPinglunByMarkets(int marketId, string mobile)
        {
            var list = new List<AppPingLunVM>();
            using (var ctx = new ShtxSms2008Entities())
            {
                //var products = ctx.SmsProducts.Where(o => o.MarketId == marketId).OrderBy(o => o.DisplayOrder);
                //foreach (var product in products)
                //{
                //    var vm = new AppPingLunVM();
                //    vm.Id = product.ProductId;
                //    vm.ProductName = product.ProductName;

                //    var pl = ctx.Weixin_Pinglun.Where(o => o.productId == product.ProductId).OrderByDescending(o => o.date).FirstOrDefault();
                //    if (pl != null)
                //    {
                //        vm.Title = pl.title;
                //        vm.Icon = pl.icon;
                //        vm.Date = pl.create.Value.ToString("yyyy-MM-dd");
                //        vm.Url = "http://app.shtx.com.cn/StaticHtml/WeixinPingLun.html?mobile="+mobile+"&id=" + pl.id;
                //        list.Add(vm);
                //    }
                //}

                CustomerBase cb = ctx.CustomerBases.FirstOrDefault(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                if (cb != null)
                {
                    mobile = cb.Tel;
                    var now = DateTime.Today;
                    //DateTime start = now.AddDays(-6);
                    //DateTime end = now.AddDays(1);
                    var products = ctx.SmsProducts.Where(o => o.MarketId == marketId).Select(o => o.ProductId).Distinct().ToList();
                    var orders = ctx.Gps.Where(o => o.Tel == mobile).Select(o => o.ProductID).Distinct().ToList();
                    //var pingluns = new List<AppPingLunVM>();

                    foreach (var productId in products)
                    {
                        var product = ctx.SmsProducts.FirstOrDefault(o => o.ProductId == productId);
                        var productPingluns = ctx.Weixin_Pinglun.Where(o => o.productId == productId).OrderByDescending(o => o.create).Take(50).ToList();
                        if (productPingluns.Count > 0)
                        {
                            foreach (var productPinglun in productPingluns)
                            {
                                var vm = new AppPingLunVM();
                                vm.Id = product.ProductId;
                                vm.ProductName = product.ProductName;
                                vm.Title = productPinglun.title;
                                vm.Icon = productPinglun.icon;
                                vm.Date = productPinglun.create.Value.ToString("yyyy-MM-dd");
                                vm.Url = "http://app.shtx.com.cn/StaticHtml/WeixinPingLun.html?mobile=" + mobile + "&id=" + productPinglun.id;
                                if (orders.Contains(productId))
                                {
                                    vm.IsOrder = "YES";
                                }
                                else
                                {
                                    vm.IsOrder = "NO";
                                }
                                list.Add(vm);
                            }
                        }
                    }


                    //var pls = ctx.Weixin_Pinglun.Where(o => o.create > start && o.create < end).OrderByDescending(o => o.create);
                    //foreach (var pl in pls)
                    //{
                    //    var pId = pl.productId;
                    //    if (!products.Contains(pId))
                    //        continue;
                    //    var product = ctx.SmsProducts.FirstOrDefault(o => o.ProductId == pl.productId);
                    //    var vm = new AppPingLunVM();
                    //    vm.Id = product.ProductId;
                    //    vm.ProductName = product.ProductName;
                    //    vm.Title = pl.title;
                    //    vm.Icon = pl.icon;
                    //    vm.Date = pl.create.Value.ToString("yyyy-MM-dd");
                    //    vm.Url = "http://app.shtx.com.cn/StaticHtml/WeixinPingLun.html?mobile=" + mobile + "&id=" + pl.id;
                    //    if (orders.Contains(pId))
                    //    {
                    //        vm.IsOrder = "YES";
                    //    }
                    //    else
                    //    {
                    //        vm.IsOrder = "NO";
                    //    }
                    //    list.Add(vm);
                    //}
                }
            }
            return list.OrderByDescending(o => o.Date).ToList();
        }

        public List<AppPingLunVM> GetPinglunByMobile(string mobile, DateTime start, DateTime end)
        {
            var list = new List<AppPingLunVM>();

            using (var ctx = new ShtxSms2008Entities())
            {
                var orderdProductIds = ctx.Gps.Where(o => o.Tel.Contains(mobile)).Select(o => o.ProductID).ToList();
                var pls = ctx.Weixin_Pinglun.Where(o => o.create < end && o.create > start && orderdProductIds.Contains(o.productId));

                foreach (var pl in pls)
                {
                    var product = ctx.SmsProducts.FirstOrDefault(o => o.ProductId == pl.productId);
                    var market = ctx.Markets.FirstOrDefault(o => o.MarketId == product.MarketId);
                    var vm = new AppPingLunVM();
                    vm.Id = pl.productId;
                    vm.Title = pl.title;
                    vm.MarketName = (market == null ? "" : market.MarketName);
                    vm.ProductName = (product == null ? "" : product.ProductName);
                    vm.Icon = pl.icon;
                    vm.Date = pl.create.Value.ToString("yyyy-MM-dd HH:mm:ss fff");
                    vm.Url = "http://app.shtx.com.cn/StaticHtml/WeixinPingLun.html?mobile=" + mobile + "&id=" + pl.id;
                    list.Add(vm);
                }
            }
            return list;
        }

        //下拉，取全部，大于该日期的
        private List<AppPingLunVM> GetPinglunByMobilePushDown(string mobile, DateTime date)
        {
            var list = new List<AppPingLunVM>();
            using (var ctx = new ShtxSms2008Entities())
            {
                var orderdProductIds = ctx.Gps.Where(o => o.Tel.Contains(mobile)).Select(o => o.ProductID).ToList();
                var pls = ctx.Weixin_Pinglun.Where(o => orderdProductIds.Contains(o.productId) && o.create > date).OrderByDescending(o => o.create);
                foreach (var pl in pls)
                {
                    var product = ctx.SmsProducts.FirstOrDefault(o => o.ProductId == pl.productId);
                    var market = ctx.Markets.FirstOrDefault(o => o.MarketId == product.MarketId);
                    var vm = new AppPingLunVM();
                    vm.Id = pl.productId;
                    vm.Title = pl.title;
                    vm.Icon = pl.icon;
                    vm.MarketName = (market == null ? "" : market.MarketName);
                    vm.ProductName = (product == null ? "" : product.ProductName);
                    vm.Date = pl.create.Value.ToString("yyyy-MM-dd HH:mm:ss fff");
                    vm.Url = "http://app.shtx.com.cn/StaticHtml/WeixinPingLun.html?mobile=" + mobile + "&id=" + pl.id;
                    list.Add(vm);
                }
            }
            return list;
        }

        //上拉，取20条，小于该日期的
        private List<AppPingLunVM> GetPinglunByMobilePushUp(string mobile, DateTime date)
        {
            var list = new List<AppPingLunVM>();
            using (var ctx = new ShtxSms2008Entities())
            {
                var orderdProductIds = ctx.Gps.Where(o => o.Tel.Contains(mobile)).Select(o => o.ProductID).ToList();
                var pls = ctx.Weixin_Pinglun.Where(o => orderdProductIds.Contains(o.productId) && o.create < date).OrderByDescending(o => o.create).Take(20);
                foreach (var pl in pls)
                {
                    var product = ctx.SmsProducts.FirstOrDefault(o => o.ProductId == pl.productId);
                    var market = ctx.Markets.FirstOrDefault(o => o.MarketId == product.MarketId);
                    var vm = new AppPingLunVM();
                    vm.Id = pl.productId;
                    vm.Title = pl.title;
                    vm.Icon = pl.icon;
                    vm.MarketName = (market == null ? "" : market.MarketName);
                    vm.ProductName = (product == null ? "" : product.ProductName);
                    vm.Date = pl.create.Value.ToString("yyyy-MM-dd HH:mm:ss fff");
                    vm.Url = "http://app.shtx.com.cn/StaticHtml/WeixinPingLun.html?mobile=" + mobile + "&id=" + pl.id;
                    list.Add(vm);
                }
            }
            return list;
        }

        public List<AppPingLunVM> GetPinglunByAction(string mobile, DateTime date, string action)
        {
            var list = new List<AppPingLunVM>();
            if (action == "pullUp")
            {
                list = GetPinglunByMobilePushUp(mobile, date);
            }
            else if (action == "pullDown")
            {
                list = GetPinglunByMobilePushDown(mobile, date);
            }
            return list;
        }
    }
}
