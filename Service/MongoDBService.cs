using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Model;
using MongoDB.Driver.Builders;
using Service.ViewModel;

namespace Service
{
    public static class MongoDBService
    {
        static IMongoDatabase db;
        static MongoDBService()
        {
            var set = new MongoClientSettings();
            var cred = new List<MongoCredential>() { MongoCredential.CreateCredential("shtx", "shtx", "shyr021191") };
            var server1 = new MongoServerAddress("172.20.67.110", 27017);
            //var server2 = new MongoServerAddress("172.20.67.111", 27017);
            //var server3 = new MongoServerAddress("172.20.67.111", 27018);
            //var server4 = new MongoServerAddress("172.20.67.111", 27019);
            var servers = new List<MongoServerAddress>() { server1 };
            //var servers = new List<MongoServerAddress>() { server1, server2, server3, server4 };
            set.Credentials = cred;
            set.Servers = servers;
            set.ConnectionMode = ConnectionMode.ReplicaSet;
            set.ReplicaSetName = "tongxin";
            set.ReadPreference = new ReadPreference(ReadPreferenceMode.SecondaryPreferred);
            var client = new MongoClient(set);
            //var client = new MongoClient(conn);
            db = client.GetDatabase("shtx");

            
        }
        //private static string conn = @"mongodb://shtx:shyr021191@172.20.67.110:27017/shtx?readPreference=secondary";

        public static Price GetLastPricesByProductId(int productId)
        {
            Price price = null;
            
            //var collection = db.GetCollection<BsonDocument>("prices");
            
            //var filter = Builders<BsonDocument>.Filter.Eq("productId", productId);
            //var data = collection.Find(filter).Sort(Builders<BsonDocument>.Sort.Descending("addDate")).FirstOrDefault();

            //var data = collection.WithReadPreference(ReadPreference.SecondaryPreferred)
            //    .Find(filter).Sort(Builders<BsonDocument>.Sort.Descending("addDate")).ToList();

            var collection = db.GetCollection<MgPrice>("prices");

            var data = collection.Find(o => o.productId == productId).SortByDescending(o => o.addDate).FirstOrDefault();

            if (data != null)
            {
                price = new Price();
                price.MarketID = data.marketId;
                price.ProductID = data.productId;
                price.HPrice = data.high;
                price.LPrice = data.low;
                price.APrice = data.average;
                price.PriceChange = data.change ;
                price.AddDate = data.addDate.ToLocalTime();
            }

            return price;
        }

        public static List<Price> GetHostoryPrices(int productId, DateTime start, DateTime end)
        {
            end = end.AddDays(1);
            var list = new List<Price>();

            //var collection = db.GetCollection<BsonDocument>("prices");

            //var filterBuilder = Builders<BsonDocument>.Filter;

            //var filter = filterBuilder.Gt("addDate", start) & filterBuilder.Lte("addDate", end) & filterBuilder.Eq("productId", productId);
            //var data = collection.Find(filter)
            //    .Sort(Builders<BsonDocument>.Sort.Descending("addDate")).ToList();

            var collection = db.GetCollection<MgPrice>("prices");
            var data = collection.Find(o => o.addDate >= start && o.addDate <= end && o.productId == productId)
                .SortByDescending(o => o.addDate).ToList();

            foreach (var d in data)
            {
                var price = new Price();
                price.MarketID = d.marketId;
                price.ProductID = d.productId;
                price.HPrice = d.high;
                price.LPrice = d.low;
                price.APrice = d.average;
                price.PriceChange = d.change;
                price.AddDate = d.addDate.ToLocalTime();
                
                list.Add(price);
            }

            return list;
        }

        public static Dictionary<int, ProductPriceVM> GetAllLastPrice()
        {
            var list = new Dictionary<int, ProductPriceVM>();
            var collection = db.GetCollection<MgPrice>("prices");
            var data = collection.Find(o => o.isLatest).SortByDescending(o=>o.addDate).ToList();
            //var l = new List<MgPrice>();
            //var g = data.GroupBy(o => o.productId);
            //foreach (var gr in g)
            //{
            //    var c = data.Where(o => o.productId == gr.Key);
            //    if (c.Count() > 1)
            //    {
            //        l.AddRange(c);
            //    }
            //}
            var marketGroup = data.GroupBy(o => o.marketId);
            foreach (var g in marketGroup)
            {
                var productPrice = g.OrderByDescending(o => o.addDate).First();
                var vm = new ProductPriceVM();
                vm.AddDate = productPrice.addDate.ToLocalTime();
                vm.Change = productPrice.change;
                vm.LPrice = productPrice.low;
                vm.HPrice = productPrice.high;
                vm.ProductId = productPrice.productId;
                vm.MarketID = productPrice.marketId;
                vm.ProductName = CacheService.GetProductNameById(productPrice.productId);
                list.Add(g.Key, vm);
            }

            return list;
        }
    }

    public class MgLastPrice
    {
        public int ProductId { get; set; }
        public MgPrice Price { get; set; }
        public DateTime AddDate { get; set; }
    }

    public class MgPrice
    {
        public ObjectId Id { get; set; }
        public int marketId { get; set; }
        public int productId { get; set; }
        public string low { get; set; }
        public string high { get; set; }
        public string average { get; set; }
        public string change { get; set; }
        public DateTime addDate { get; set; }
        public bool isLatest { get; set; }
    }
}
