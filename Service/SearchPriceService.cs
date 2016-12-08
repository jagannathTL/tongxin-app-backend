using Service.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Service
{
    public class SearchPriceService
    {
        public List<SearchPriceVM> GetSearchResult(string key, string mobile)
        {
            var list = new List<SearchPriceVM>();
            List<int> xhmarketIds = new List<int>();
            using (var ctx = new ShtxSms2008Entities())
            {
                //change://
                //var groups = ctx.XHMarketGroups.Where(o => o.Flag == 1 && o.IsForApp).Select(o => o.GroupID).ToList();
                var groups = CacheService.GetMarketGroupForApp(1).Select(o => o.GroupID).ToList();
                CustomerBase cb = ctx.CustomerBases.FirstOrDefault(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                if (cb != null)
                {
                    mobile = cb.Tel;
                    var orderIds = ctx.Gps.Where(o => o.Tel == mobile).Select(o => o.ProductID.Value).ToList();
                    foreach (var group in groups)
                    {
                        //change://
                        //xhmarketIds.AddRange(ctx.Markets.Where(o => o.GroupID == group).Select(o => o.MarketId));
                        xhmarketIds.AddRange(CacheService.GetMarketByGroupId(group).Select(o => o.MarketId));
                    }

                    var result = ctx.SmsProducts.Where(o => o.ProductName.Contains(key) && xhmarketIds.Contains(o.MarketId.Value))
                        .Select(o => new searchProduct { MarketId = o.MarketId.Value, ProductId = o.ProductId, ProductName = o.ProductName }).ToList();

                    var markets = result.Select(o => o.MarketId).Distinct();

                    foreach (var market in markets)
                    {
                        var m = ctx.Markets.FirstOrDefault(o => o.MarketId == market);
                        if (m != null)
                        {
                            SearchPriceVM priceVm = new SearchPriceVM();
                            priceVm.MarketId = m.MarketId;
                            priceVm.MarketName = m.MarketName;
                            priceVm.prices = new List<ProductPriceVM>();

                            var productList = result.Where(o => o.MarketId == market);
                            foreach (var product in productList)
                            {
                                var vm = new ProductPriceVM
                                {
                                    ProductId = product.ProductId,
                                    ProductName = product.ProductName
                                };
                                if (orderIds.Contains(product.ProductId))
                                {
                                    vm.IsOrder = "YES";
                                }
                                else
                                {
                                    vm.IsOrder = "NO";
                                }
                                //mongo:
                                //Price price = ctx.Prices.OrderByDescending(o => o.AddDate).FirstOrDefault(o => o.ProductID == product.ProductId);
                                Price price = MongoDBService.GetLastPricesByProductId(product.ProductId);
                                if (price != null)
                                {
                                    vm.LPrice = price.LPrice;
                                    vm.HPrice = string.IsNullOrEmpty(price.HPrice) ? price.LPrice : price.HPrice;
                                    vm.Date = price.AddDate.Value.ToString("yyyy-MM-dd");
                                    vm.Change = CommonService.ChangePrice(price.PriceChange);
                                    vm.AddDate = price.AddDate.Value;
                                    priceVm.prices.Add(vm);
                                }
                            }
                            if(priceVm.prices.Count>0)
                                list.Add(priceVm);
                        }
                    }
                }
            }
            return list;
        }
    }

    public class searchProduct
    {
        public int MarketId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
