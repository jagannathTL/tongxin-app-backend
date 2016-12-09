using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Service.ViewModel;
using Model.enums;

namespace Service
{
    public class MarketService
    {
        public List<MarketGroupVM> GetMarkets(string mobile, int flag, string fla1)
        {
            var list = new List<MarketGroupVM>();
            using (var ctx = new ShtxSms2008Entities())
            {
                //change://
                //var groups = ctx.XHMarketGroups.Where(o => o.Flag == flag && o.IsForApp);
                var groups = CacheService.GetMarketGroupForApp(flag);
                foreach (var group in groups)
                {
                    var groupId = group.GroupID;
                    var vm = new MarketGroupVM() { Id = groupId, Name = group.GroupName };
                    //change://
                    //vm.Market = ctx.Markets.Where(o => o.GroupID == groupId).Select(o => new MarketVM { Id = o.MarketId, Name = o.MarketName, Order = (int)o.DisplayOrder }).OrderBy(o => o.Order).ToList();
                    vm.Market = CacheService.GetMarketByGroupId(groupId).Select(o => new MarketVM { Id = o.MarketId, Name = o.MarketName, Order = (int)o.DisplayOrder }).OrderBy(o => o.Order).ToList();
                    list.Add(vm);
                }
            }
            return list;
        }

        /// <summary>
        /// type: 1行情，5评论
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<MarketVM> GetMyOrderMarkets(string mobile, int type, Dictionary<int, IEnumerable<Weixin_Pinglun>> threePl)
        {

            var list = new List<MarketVM>();
            List<int> xhmarketIds = new List<int>();
            var xhMarkets = new Dictionary<int, string>();

            using (var ctx = new ShtxSms2008Entities())
            {
                //change://
                //var groups = ctx.XHMarketGroups.Where(o => (o.Flag == type) && o.IsForApp).ToList();
                var groups = CacheService.GetMarketGroupForApp(type);
                CustomerBase cb = ctx.CustomerBases.FirstOrDefault(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                if (cb != null)
                {
                    mobile = cb.Tel;

                    foreach (var group in groups)
                    {
                        //change://
                        //var markets = ctx.Markets.Where(o => o.GroupID == group.GroupID).ToList();
                        var markets = CacheService.GetMarketByGroupId(group.GroupID);
                        foreach (var m in markets)
                        {
                            xhmarketIds.Add(m.MarketId);
                            xhMarkets.Add(m.MarketId, group.GroupName + " - " + m.MarketName);
                        }
                    }

                    
                    var marketIds = ctx.Gps.Where(o => o.Tel == mobile && xhmarketIds.Contains(o.MarketID.Value)).Select(o => o.MarketID.Value).Distinct().ToList();

                    //评论，显示三条，价格不显示
                    if (type == (int)EnumMarketFlag.WXMarket)
                    {
                        var orderProducts = ctx.Gps.Where(o => o.Tel == mobile).Select(o => o.ProductID).Distinct().ToList();
                        foreach (var marketId in marketIds)
                        {
                            MarketVM order = new MarketVM();
                            order.Id = marketId;
                            order.Name = xhMarkets[marketId];
                            //change://
                            //var hasProducts = ctx.SmsProducts.Where(o => o.MarketId == marketId && orderProducts.Contains(o.ProductId)).Select(o => o.ProductId).ToList();
                            //var pls = ctx.Weixin_Pinglun.OrderByDescending(o => o.create).Where(o => hasProducts.Contains(o.productId)).Take(3).ToList();
                            var hasProducts = CacheService.GetProductByMarketId(marketId).Where(o => orderProducts.Contains(o.ProductId)).Select(o => o.ProductId).ToList();
                            var pls = threePl.Where(o => hasProducts.Contains(o.Key)).SelectMany(o => o.Value).OrderByDescending(o => o.create).Take(3).ToList();
                            List<OrderPLVM> orderPls = new List<OrderPLVM>();
                            foreach (var pl in pls)
                            {
                                OrderPLVM vm = new OrderPLVM();
                                vm.Id = pl.id;
                                vm.Title = pl.title;
                                vm.Date = pl.date.ToString("yyyy-MM-dd");
                                vm.Url = "http://app.shtx.com.cn/StaticHtml/WeixinPingLun.html?mobile=" + mobile + "&id=" + pl.id;
                                orderPls.Add(vm);
                            }
                            order.OrderPL = orderPls;
                            list.Add(order);
                        }
                    }
                    else
                    {

                        foreach (var marketId in marketIds)
                        {
                            MarketVM order = new MarketVM();
                            order.Id = marketId;
                            order.Name = xhMarkets[marketId];
                            list.Add(order);
                        }
                    }
                }
            }
            return list;
        }

        public Dictionary<int, IEnumerable<Weixin_Pinglun>> TakeNPLEachGroup(int n)
        {
            var list = new Dictionary<int, IEnumerable<Weixin_Pinglun>>();
            using (var ctx = new ShtxSms2008Entities())
            {
                var query = ctx.Weixin_Pinglun.GroupBy(o => o.productId).Select(g => new { id = g.Key, list = g.OrderByDescending(p => p.create).Take(n) }).ToList();
                foreach (var q in query)
                {
                    list.Add(q.id, q.list);
                }
            }
            return list;
        }

        public List<MarketGroupVM> GetMarkets(string mobile, int flag)
        {
            var list = new List<MarketGroupVM>();
            var listOrder = new List<int>();
            var defaultOrder = new List<int>();
            //var threePl = TakeNPLEachGroup(3);
            var threePl = new Dictionary<int,IEnumerable<Weixin_Pinglun>>();
            //判断是否是评论或价格
            if (flag == (int)EnumMarketFlag.WXMarket)
                threePl = TakeNPLEachGroup(3);
            using (var ctx = new ShtxSms2008Entities())
            {
                if (!string.IsNullOrWhiteSpace(mobile))
                {
                    listOrder.AddRange(ctx.MyAppGroups.Where(o => o.tel == mobile && o.Flag == flag).OrderByDescending(o => o.DisplayOrder).Select(o => o.GroupID));
                    if (listOrder.Count == 0)
                    {
                        //change://
                        //defaultOrder.AddRange(ctx.XHMarketGroups.Where(o => o.Flag == flag && o.IsForApp && (o.IsDefault ?? false)).Select(o => o.GroupID));
                        defaultOrder.AddRange(CacheService.GetGroupsForApp(flag));
                    }
                }
                //change://
                //var groups = ctx.XHMarketGroups.Where(o => o.Flag == flag && o.IsForApp);
                var groups = CacheService.GetMarketGroupForApp(flag);
                foreach (var group in groups)
                {
                    var groupId = group.GroupID;
                    var vm = new MarketGroupVM() { Id = groupId, Name = group.GroupName };
                    //change://
                    //var markets = ctx.Markets.Where(o => o.GroupID == groupId).Select(o => new MarketVM { Id = o.MarketId, Name = o.MarketName, Order = (int)o.DisplayOrder }).OrderBy(o => o.Order).ToList();
                    var markets = CacheService.GetMarketByGroupId(groupId).Select(o => new MarketVM { Id = o.MarketId, Name = o.MarketName, Order = (int)o.DisplayOrder }).OrderBy(o => o.Order).ToList();
                    //评论显示，显示三条最新的。
                    if (flag == (int)EnumMarketFlag.WXMarket)
                    {
                        foreach (var market in markets)
                        {
                            //change://
                            //var hasProducts = ctx.SmsProducts.Where(o => o.MarketId == market.Id).Select(o => o.ProductId).ToList();
                            //var pls = ctx.Weixin_Pinglun.OrderByDescending(o => o.create).Where(o => hasProducts.Contains(o.productId)).Take(3).ToList();
                            var hasProducts = CacheService.GetProductByMarketId(market.Id).Select(o => o.ProductId).ToList();
                            var pls = threePl.Where(o => hasProducts.Contains(o.Key)).SelectMany(o => o.Value).OrderByDescending(o => o.create).Take(3).ToList();
                            List<OrderPLVM> orderPls = new List<OrderPLVM>();
                            foreach (var pl in pls)
                            {
                                OrderPLVM orderPlVM = new OrderPLVM();
                                orderPlVM.Id = pl.id;
                                orderPlVM.Title = pl.title;
                                orderPlVM.Date = pl.date.ToString("yyyy-MM-dd");
                                orderPlVM.Url = "http://app.shtx.com.cn/StaticHtml/WeixinPingLun.html?mobile=" + mobile + "&id=" + pl.id;
                                orderPls.Add(orderPlVM);
                            }
                            market.OrderPL = orderPls;
                        }
                    }


                    
                    vm.Market = markets;
                    vm.inBucket = "false";
                    if (string.IsNullOrWhiteSpace(mobile))
                    {
                        vm.inBucket = "true";
                    }
                    else
                    {
                        if (listOrder.Count == 0 && defaultOrder.Count == 0)
                        {
                            vm.inBucket = "true";
                            list.Add(vm);
                            continue;
                        }

                        if (listOrder.Count != 0)
                        {
                            if (listOrder.Contains(groupId))
                            {
                                vm.inBucket = "true";
                                list.Add(vm);
                                continue;
                            }
                        }
                        else
                        {
                            if (defaultOrder.Count != 0)
                            {
                                if (defaultOrder.Contains(groupId))
                                {
                                    vm.inBucket = "true";
                                }
                            }
                        }
                    }

                    list.Add(vm);
                }

                if (listOrder.Count != 0)
                {
                    //排序订阅的
                    for (int i = 0; i < listOrder.Count; i++)
                    {
                        var group = list.FirstOrDefault(o => o.Id == listOrder[i]);
                        if (group != null)
                        {
                            list.Remove(group);
                            list.Insert(0, group);
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(mobile))
            {
                //插入我的关注
                MarketGroupVM root = new MarketGroupVM();
                root.Id = 0;
                root.Name = "我的关注";
                root.inBucket = "true";
                var myOrder = GetMyOrderMarkets(mobile, flag, threePl);
                root.Market = myOrder;
                list.Insert(0, root);
            }
            return list;
        }

        public bool saveGroupChannel(string mobile, string groupChannels, int flag)
        {
            bool ret = false;
            List<int> groups = new List<int>();

            string[] strGroups = groupChannels.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strGroups.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(strGroups[i]))
                {
                    int groupId = Convert.ToInt32(strGroups[i]);
                    if (groupId != 0)
                    {
                        groups.Add(groupId);
                    }
                }
            }
            try
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var myAppGroups = ctx.MyAppGroups.Where(o => o.tel == mobile && o.Flag == flag).ToList();
                    ctx.MyAppGroups.RemoveRange(myAppGroups);

                    int index = 1;
                    foreach (var groupId in groups)
                    {
                        MyAppGroup group = new MyAppGroup();
                        group.tel = mobile;
                        group.GroupID = groupId;
                        group.DisplayOrder = index++;
                        group.Flag = flag;
                        ctx.MyAppGroups.Add(group);
                    }
                    ctx.SaveChanges();
                    ret = true;
                }
            }
            catch (Exception)
            {

            }

            return ret;
        }
    }
}
