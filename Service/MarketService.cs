using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Service.ViewModel;

namespace Service
{
    public class MarketService
    {
        public List<MarketGroupVM> GetMarkets(string mobile, int flag, string fla1)
        {
            var list = new List<MarketGroupVM>();
            using (var ctx = new ShtxSms2008Entities())
            {
                var groups = ctx.XHMarketGroups.Where(o => o.Flag == flag && o.IsForApp);
                foreach (var group in groups)
                {
                    var groupId = group.GroupID;
                    var vm = new MarketGroupVM() { Id = groupId, Name = group.GroupName };
                    vm.Market = ctx.Markets.Where(o => o.GroupID == groupId).Select(o => new MarketVM { Id = o.MarketId, Name = o.MarketName, Order = (int)o.DisplayOrder }).OrderBy(o => o.Order).ToList();
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
        private List<MarketVM> GetMyOrderMarkets(string mobile, int type)
        {

            var list = new List<MarketVM>();
            List<int> xhmarketIds = new List<int>();
            var xhMarkets = new Dictionary<int, string>();
            using (var ctx = new ShtxSms2008Entities())
            {
                var groups = ctx.XHMarketGroups.Where(o => (o.Flag == type) && o.IsForApp).ToList();
                CustomerBase cb = ctx.CustomerBases.FirstOrDefault(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                if (cb != null)
                {
                    mobile = cb.Tel;

                    foreach (var group in groups)
                    {
                        var markets = ctx.Markets.Where(o => o.GroupID == group.GroupID).ToList();
                        foreach (var m in markets)
                        {
                            xhmarketIds.Add(m.MarketId);
                            xhMarkets.Add(m.MarketId, group.GroupName + " - " + m.MarketName);
                        }
                    }

                    var marketIds = ctx.Gps.Where(o => o.Tel == mobile && xhmarketIds.Contains(o.MarketID.Value)).Select(o => o.MarketID.Value).Distinct().ToList();

                    foreach (var marketId in marketIds)
                    {
                        MarketVM order = new MarketVM();
                        order.Id = marketId;
                        order.Name = xhMarkets[marketId];
                        list.Add(order);
                    }
                }
            }
            return list;
        }

        public List<MarketGroupVM> GetMarkets(string mobile, int flag)
        {
            var list = new List<MarketGroupVM>();
            var listOrder = new List<int>();
            var defaultOrder = new List<int>();

            using (var ctx = new ShtxSms2008Entities())
            {
                if (!string.IsNullOrWhiteSpace(mobile))
                {
                    listOrder.AddRange(ctx.MyAppGroups.Where(o => o.tel == mobile && o.Flag == flag).OrderByDescending(o => o.DisplayOrder).Select(o => o.GroupID));
                    if (listOrder.Count == 0)
                    {
                        defaultOrder.AddRange(ctx.XHMarketGroups.Where(o => o.Flag == flag && o.IsForApp && (o.IsDefault ?? false)).Select(o => o.GroupID));
                    }
                }
                var groups = ctx.XHMarketGroups.Where(o => o.Flag == flag && o.IsForApp);
                foreach (var group in groups)
                {
                    var groupId = group.GroupID;
                    var vm = new MarketGroupVM() { Id = groupId, Name = group.GroupName };
                    var markets = ctx.Markets.Where(o => o.GroupID == groupId).Select(o => new MarketVM { Id = o.MarketId, Name = o.MarketName, Order = (int)o.DisplayOrder }).OrderBy(o => o.Order).ToList();
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
                var myOrder = GetMyOrderMarkets(mobile, flag);
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
