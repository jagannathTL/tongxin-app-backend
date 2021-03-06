﻿using Model;
using Service.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class CacheService
    {
        private static Dictionary<int, List<SmsProduct>> smsProductCache = new Dictionary<int, List<SmsProduct>>();
        public static List<SmsProduct> GetSmsProductByMarketId(int marketId)
        {
            if (smsProductCache.ContainsKey(marketId))
            {
                return smsProductCache[marketId];
            }
            else
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var products = ctx.SmsProducts.Where(o => o.MarketId == marketId).OrderBy(p => p.DisplayOrder).ToList();
                    smsProductCache.Add(marketId, products);
                    return products;
                }
            }
        }

        private static Dictionary<int, List<CataLogVM>> cataLogCache = new Dictionary<int, List<CataLogVM>>();
        public static List<CataLogVM> GetCataLogByChannelId(int channelId)
        {
            if (cataLogCache.ContainsKey(channelId))
            {
                return cataLogCache[channelId];
            }
            else
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var catalogs = ctx.CataLogs.Where(o => o.ChannelId == channelId).Select(o => new CataLogVM
                    {
                        Id = o.ID,
                        Name = o.KeyName,
                        Desc = o.Description
                    }).ToList();
                    //cataLogCache[channelId] = catalogs;
                    cataLogCache.Add(channelId, catalogs);
                    return catalogs;
                }
            }
        }


        private static List<ChannelVM> channelCache = null;
        public static List<ChannelVM> GetChannel()
        {
            if (channelCache != null)
            {
                return channelCache;
            }
            else
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var list = ctx.Channels.Select(o => new ChannelVM
                    {
                        Id = o.ID,
                        Name = o.Name
                    }).ToList();
                    channelCache = list;
                    return list;
                }
            }

        }

        private static Dictionary<int, List<int>> defaultAppGroupsCache = new Dictionary<int, List<int>>();
        public static List<int> GetGroupsForApp(int flag)
        {
            if (defaultAppGroupsCache.ContainsKey(flag))
            {
                return defaultAppGroupsCache[flag];
            }
            else
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var list = ctx.XHMarketGroups.Where(o => o.Flag == flag && o.IsForApp && (o.IsDefault ?? false) && o.GroupID != 102 && o.GroupID != 103).Select(o => o.GroupID).ToList();
                    //defaultAppGroupsCache[flag] = list;
                    defaultAppGroupsCache.Add(flag, list);
                    return list;
                }
            }
        }

        private static Dictionary<int, List<XHMarketGroup>> marketGroupForAppCache = new Dictionary<int, List<XHMarketGroup>>();
        public static List<XHMarketGroup> GetMarketGroupForApp(int flag)
        {
            if (marketGroupForAppCache.ContainsKey(flag))
            {
                return marketGroupForAppCache[flag];
            }
            else
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var groups = ctx.XHMarketGroups.Where(o => o.Flag == flag && o.IsForApp && o.GroupID != 102 && o.GroupID != 103).ToList();
                    marketGroupForAppCache.Add(flag, groups);
                    //marketGroupForAppCache[flag] = groups;
                    return groups;
                }
            }
        }

        private static Dictionary<int, List<Market>> marketCache = new Dictionary<int, List<Market>>();
        public static List<Market> GetMarketByGroupId(int groupId)
        {
            if (marketCache.ContainsKey(groupId))
            {
                return marketCache[groupId];
            }
            else
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var markets = ctx.Markets.Where(o => o.GroupID == groupId).ToList();
                    //marketCache[groupId] = markets;
                    marketCache.Add(groupId, markets);
                    return markets;
                }
            }
        }

        private static Dictionary<int, List<SmsProduct>> productCache = new Dictionary<int, List<SmsProduct>>();
        public static List<SmsProduct> GetProductByMarketId(int marketId)
        {
            if (productCache.ContainsKey(marketId))
            {
                return productCache[marketId];
            }
            else
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var list = ctx.SmsProducts.Where(o => o.MarketId == marketId).ToList();
                    productCache.Add(marketId, list);
                    return list;
                }
            }
        }
        private static Dictionary<int, string> marketNameCache = new Dictionary<int, string>();
        public static string GetMarketNameById(int id)
        {
            if (marketNameCache.ContainsKey(id))
            {
                return marketNameCache[id];
            }
            else
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var name = ctx.Markets.Single(o => o.MarketId == id).MarketName;
                    marketNameCache.Add(id, name);
                    return name;
                }
            }
        }

        private static Dictionary<int, Tuple<int, string, int, string>> productNameCache = new Dictionary<int, Tuple<int, string, int, string>>();
        public static string GetProductNameById(int id)
        {
            if (productNameCache.ContainsKey(id))
            {
                return productNameCache[id].Item2;
            }
            else
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var obj = GetProductAndMarketByProductId(id);
                    var name = obj.Item2;
                    productNameCache.Add(id, obj);
                    return name;
                }
            }
        }
        public static Tuple<int, string> GetMarketByProductId(int id)
        {
            if (productNameCache.ContainsKey(id))
            {
                return Tuple.Create(productNameCache[id].Item3, productNameCache[id].Item4);
            }
            else
            {
                using (var ctx = new ShtxSms2008Entities())
                {
                    var obj = GetProductAndMarketByProductId(id);
                    var tuple = Tuple.Create(obj.Item3, obj.Item4);
                    productNameCache.Add(id, obj);
                    return tuple;
                }
            }
        }

        private static Tuple<int, string, int, string> GetProductAndMarketByProductId(int productId)
        {
            using (var ctx = new ShtxSms2008Entities())
            {
                var product = ctx.SmsProducts.FirstOrDefault(o => o.ProductId == productId);
                if (product == null)
                {
                    return Tuple.Create(0, "", 0, "");
                }
                var marketId = product.MarketId.Value;
                var marketName = GetMarketNameById(marketId);

                return Tuple.Create(productId, product.ProductName, marketId, marketName);
            }
        }
    }
}
