﻿<%@ WebHandler Language="C#" Class="XHMarketHandler" %>

using System;
using System.Web;
using Service;
using Service.ViewModel;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using Model;
using System.Linq;

public class XHMarketHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        //var mobile = context.Request["mobile"];
        if (method == "getmarkets")
        {
            using (var ctx = new ShtxSms2008Entities())
            {//entities
                var mobile = context.Request["mobile"] ?? "";

                var priceSvc = new ProductPriceService();
                var marketSvc = new MarketService();

                //mongo://
                //var priceDic = priceSvc.GetAllMarketLastPrice();
                var marketPriceDic = MongoDBService.GetAllLastPrice();
                //var orderList = ctx.AppGetOrderMarketPrice(mobile);

                //价格，不需要显示三条最新评论
                var list = marketSvc.GetMarkets(mobile, (int)Model.enums.EnumMarketFlag.XHMarket);

                foreach (var tGroup in list)
                {
                    if (tGroup.Market != null) //.inBucket == "false")
                    {//1
                        if (tGroup.Name == "我的关注")
                        {
                            var orderIds = ctx.Gps.Where(o => o.Tel == mobile && o.SendTime == "").Select(o => o.ProductID).ToList();
                            foreach (var tMarket in tGroup.Market)
                            {

                                if (marketPriceDic.ContainsKey(tMarket.Id))
                                {
                                    if (tMarket.NewPrices == null)
                                    {
                                        tMarket.NewPrices = new List<ProductPriceVM>();
                                    }
                                    var newPrice = new ProductPriceVM();
                                    newPrice = marketPriceDic[tMarket.Id];
                                    tMarket.NewPrices.Add(newPrice);
                                    //Console.WriteLine(priceDic[tMarket.Id]);
                                }
                                
                                
                                //change://
                                //var list1 = ctx.SmsProducts.Where(o => o.MarketId == tMarket.Id && orderIds.Contains(o.ProductId)).Select(o => o.ProductId).ToList();
                                //var list1 = CacheService.GetSmsProductByMarketId(tMarket.Id).Where(o => orderIds.Contains(o.ProductId)).Select(o => o.ProductId).ToList();
                                //if (list1.Count > 0)
                                //{
                                //    //mongo://
                                //    //var orderPrice = ctx.Prices.Where(o => list1.Contains(o.ProductID.Value)).OrderByDescending(o => o.AddDate).FirstOrDefault();
                                //    var count = priceDic.Count(o => list1.Contains(o.Key));
                                //    if (count > 0)
                                //    {
                                //        var keyValue = priceDic.Where(o => list1.Contains(o.Key)).OrderByDescending(o => o.Value.AddDate).FirstOrDefault();
                                //        var orderPrice = keyValue.Value;
                                //        var orderProducts = ctx.SmsProducts.FirstOrDefault(o => o.ProductId == orderPrice.ProductId);
                                //        if (tMarket.NewPrices == null)
                                //        {
                                //            tMarket.NewPrices = new List<ProductPriceVM>();
                                //        }
                                //        var newPrice = new ProductPriceVM();
                                //        newPrice.AddDate = orderPrice.AddDate;
                                //        newPrice.Change = CommonService.ChangePrice(orderPrice.Change);
                                //        newPrice.Comment = orderProducts.comment;
                                //        newPrice.HPrice = orderPrice.HPrice;
                                //        newPrice.LPrice = orderPrice.LPrice;
                                //        newPrice.ProductId = orderPrice.ProductId;
                                //        newPrice.ProductName = orderProducts.ProductName;

                                //        tMarket.NewPrices.Add(newPrice);
                                //    }
                                //    else
                                //    {
                                //        //没有最新价
                                //        var newPrice = new ProductPriceVM();
                                //        newPrice.AddDate = DateTime.Now;
                                //        newPrice.Change = "";
                                //        newPrice.Comment = "";
                                //        newPrice.HPrice = "";
                                //        newPrice.LPrice = "";
                                //        newPrice.ProductName = "";
                                //        if (tMarket.NewPrices == null)
                                //        {
                                //            tMarket.NewPrices = new List<ProductPriceVM>();
                                //        }
                                //        tMarket.NewPrices.Add(newPrice);
                                //    }
                                //}
                            }
                        }
                        else
                            foreach (var tMarket in tGroup.Market)
                            {//for

                                if (marketPriceDic.ContainsKey(tMarket.Id))
                                {
                                    if (tMarket.NewPrices == null)
                                    {
                                        tMarket.NewPrices = new List<ProductPriceVM>();
                                    }
                                    var newPrice = new ProductPriceVM();
                                    newPrice = marketPriceDic[tMarket.Id];
                                    tMarket.NewPrices.Add(newPrice);
                                    //Console.WriteLine(priceDic[tMarket.Id]);
                                }
                            }//for
                    }//1
                }

                var str = GetJson(list);
                context.Response.Write(str);
                context.Response.Flush();
                context.Response.End();
            }//entities
        }
        else if (method == "groupChannel")
        {
            var mobile = context.Request["mobile"];
            var strGroups = context.Request["groups"];
            new MarketService().saveGroupChannel(mobile, strGroups, (int)Model.enums.EnumMarketFlag.XHMarket);
            context.Response.End();
        }
    }

    private string GetJson(List<MarketGroupVM> list)
    {
        StringWriter sw = new StringWriter();
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();
            for (int i = 0; i < list.Count; i++)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("id");
                writer.WriteValue(list[i].Id);
                writer.WritePropertyName("name");
                writer.WriteValue(list[i].Name);
                writer.WritePropertyName("inBucket");
                writer.WriteValue(list[i].inBucket);
                writer.WritePropertyName("markets");
                writer.WriteStartArray();
                for (int j = 0; j < list[i].Market.Count; j++)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("id");
                    writer.WriteValue(list[i].Market[j].Id);
                    writer.WritePropertyName("name");
                    writer.WriteValue(list[i].Market[j].Name);


                    if (list[i].Market[j].NewPrices != null)
                        for (int k = 0; k < list[i].Market[j].NewPrices.Count; k++)
                        {
                            writer.WritePropertyName("ProductId");
                            writer.WriteValue(list[i].Market[j].NewPrices[k].ProductId);
                            writer.WritePropertyName("ProductName");
                            writer.WriteValue(list[i].Market[j].NewPrices[k].ProductName);
                            writer.WritePropertyName("LPrice");
                            writer.WriteValue(list[i].Market[j].NewPrices[k].LPrice);
                            writer.WritePropertyName("HPrice");
                            writer.WriteValue(list[i].Market[j].NewPrices[k].HPrice);
                            writer.WritePropertyName("Date");
                            writer.WriteValue(list[i].Market[j].NewPrices[k].AddDate.ToString("yyyy-MM-dd"));
                            writer.WritePropertyName("Change");
                            writer.WriteValue(list[i].Market[j].NewPrices[k].Change ?? "");
                        }

                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.Flush();
            sw.Close();
        }
        return sw.GetStringBuilder().ToString();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}