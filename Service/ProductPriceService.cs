using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Service.ViewModel;

namespace Service
{
    public class ProductPriceService
    {
        private List<int> GetProductIdsByMobileAndMarketId(string mobile, int marketId)
        {
            using (var ctx = new ShtxSms2008Entities())
            {
                return ctx.Gps.Where(o => o.Tel.Contains(mobile) && o.MarketID == marketId).Select(o => o.ProductID.Value).ToList();
            }
        }

        public List<ProductPriceVM> GetPricesByMobileAndMarketId(string mobile, int marketId)
        {
            var listOrder = new List<ProductPriceVM>();
            using (var ctx = new ShtxSms2008Entities())
            {
                CustomerBase cb = ctx.CustomerBases.FirstOrDefault(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                if (cb != null)
                {
                    mobile = cb.Tel;
                    var products = ctx.SmsProducts.Where(o => o.MarketId == marketId).OrderBy(p => p.DisplayOrder).ToList();
                    var orderIds = ctx.Gps.Where(o => o.Tel == mobile && o.MarketID == marketId).Select(o => o.ProductID.Value).ToList();
                    foreach (var product in products)
                    {
                        var vm = new ProductPriceVM
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            ParentName = product.parentName,
                            Comment = product.comment,
                            Spec = product.spec
                        };
                        if (orderIds.Contains(product.ProductId))
                        {
                            vm.IsOrder = "YES";
                        }
                        else
                        {
                            vm.IsOrder = "NO";
                        }
                        Price price = ctx.Prices.OrderByDescending(o => o.AddDate).FirstOrDefault(o => o.ProductID == product.ProductId);
                        if (price != null)
                        {
                            vm.LPrice = price.LPrice;
                            vm.HPrice = string.IsNullOrEmpty(price.HPrice) ? price.LPrice : price.HPrice;
                            vm.Date = price.AddDate.Value.ToString("yyyy-MM-dd");
                            vm.Change = CommonService.ChangePrice(price.PriceChange);
                            vm.AddDate = price.AddDate.Value;
                            listOrder.Add(vm);
                        }
                    }
                }
                return listOrder;
            }
        }

        //获得所有现货商品最近更新价格信息
        //public List<ProductPriceVM> GetAllMarketLastPrice()
        public Dictionary<int, ProductPriceVM> GetAllMarketLastPrice()
        {
            //var listOrder = new List<ProductPriceVM>();
            var listDic = new Dictionary<int, ProductPriceVM>();
            using (var ctx = new ShtxSms2008Entities())
            {//ctx
                //inner join 
                var listPrice = from f in
                    (from p in ctx.Prices
                        where !ctx.Prices.Any(es => (es.MarketID == p.MarketID) && (es.ProductID == p.ProductID) && (es.AddDate > p.AddDate) )
                        orderby p.AddDate descending
                        select p)
                join dist in ctx.SmsProducts on new { mid=(int)f.MarketID, pid=(int)f.ProductID } equals new { mid = (int)dist.MarketId, pid=dist.ProductId }
                select new 
                {
                    dist.ProductName,
                    dist.SName,
                    f.ProductID,
                    f.LPrice,
                    f.HPrice,
                    f.APrice,
                    f.AddDate,
                    f.PriceChange,
                    f.MarketID
                };

                //IEnumerable<IGrouping<int?, ProductPriceVM>> grouplist = listPrice.ToList().GroupBy(c => c.MarketID);
                //var g_Mapid = new Dictionary<int, int>();
                foreach (var product in listPrice)
                {
                    if (!listDic.ContainsKey((int)product.MarketID))
                    {
                        var vm = new ProductPriceVM
                        {
                            ProductId = (int)product.ProductID,
                            ProductName = product.ProductName,

                        };
                        vm.AddDate = (DateTime)product.AddDate;
                        vm.LPrice = product.LPrice;
                        vm.HPrice = product.HPrice;
                        vm.Change = product.PriceChange;
                        vm.MarketID = (int)product.MarketID;
                        listDic.Add((int)product.MarketID,vm);
                        //listOrder.Add(vm);
                    }

                }

                return listDic;
            }//ctx
        }

        public List<ProductPriceVM> GetOrderPricesByMobileAndMarketId(string mobile, int marketId)
        {
            var listOrder = new List<ProductPriceVM>();
            using (var ctx = new ShtxSms2008Entities())
            {
                CustomerBase cb = ctx.CustomerBases.FirstOrDefault(o => o.Tel.Contains(mobile) && o.SendInterFace == 102);
                if (cb != null)
                {
                    mobile = cb.Tel;
                    var products = ctx.SmsProducts.Where(o => o.MarketId == marketId).OrderBy(p => p.DisplayOrder).ToList();
                    var orderIds = ctx.Gps.Where(o => o.Tel == mobile && o.MarketID == marketId).Select(o => o.ProductID.Value).ToList();
                    foreach (var product in products)
                    {
                        var vm = new ProductPriceVM
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            ParentName = product.parentName,
                            Comment = product.comment,
                            Spec = product.spec
                        };
                        if (orderIds.Contains(product.ProductId))
                        {
                            vm.IsOrder = "YES";
                        }
                        else
                        {
                            continue;
                        }
                        Price price = ctx.Prices.OrderByDescending(o => o.AddDate).FirstOrDefault(o => o.ProductID == product.ProductId);
                        if (price != null)
                        {
                            vm.LPrice = price.LPrice;
                            vm.HPrice = string.IsNullOrEmpty(price.HPrice) ? price.LPrice : price.HPrice;
                            vm.Date = price.AddDate.Value.ToString("yyyy-MM-dd");
                            vm.Change = CommonService.ChangePrice(price.PriceChange);
                            vm.AddDate = price.AddDate.Value;
                            listOrder.Add(vm);
                        }
                    }
                }
                return listOrder;
            }
        }

        public List<ProductPriceVM> GetHistoryPrices(int productId, DateTime start, DateTime end)
        {
            var list = new List<ProductPriceVM>();
            using (var ctx = new ShtxSms2008Entities())
            {
                while (start <= end)
                {
                    DateTime endTime = start.AddDays(1);
                    var price = ctx.Prices.Where(o => (o.ProductID ?? 0) == productId && o.AddDate < endTime && o.AddDate >= start).OrderByDescending(o => o.AddDate).FirstOrDefault();
                    if (price != null && (!string.IsNullOrEmpty(price.LPrice) || !string.IsNullOrEmpty(price.HPrice)))
                    {
                        ProductPriceVM vm = new ProductPriceVM();
                        vm.Date = start.ToString("yyyy-MM-dd");
                        vm.HPrice = string.IsNullOrEmpty(price.HPrice) ? price.LPrice : price.HPrice;
                        vm.LPrice = string.IsNullOrEmpty(price.LPrice) ? price.HPrice : price.LPrice;
                        vm.Change = CommonService.ChangePrice(price.PriceChange);
                        list.Add(vm);
                    }
                    start = start.AddDays(1);
                }
            }
            return list.OrderByDescending(o => o.Date).ToList();
        }
    }
}
