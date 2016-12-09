<%@ WebHandler Language="C#" Class="PLHandler" %>

using System;
using System.Web;
using Service;
using Service.ViewModel;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using Model.enums;

public class PLHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "getmarkets")
        {
            var mobile = context.Request["mobile"] ?? "";
            var marketSvc = new MarketService();
            var list = marketSvc.GetMarkets(mobile,(int)EnumMarketFlag.WXMarket);
            var str = GetMarketJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
        else if (method == "getproducts")
        {
            var plSvc = new PinglunService();
            var marketId = int.Parse(context.Request["marketId"]);
            var mobile = context.Request["mobile"];
            var list = plSvc.GetPinglunByMarkets(marketId, mobile);
            var str = GetPLJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
        else if (method == "getCommentByMobile")
        {
            var mobile = context.Request["mobile"];
            var plSvc = new PinglunService();
            var start = DateTime.Today.AddDays(-1);
            var end = DateTime.Today.AddDays(1);
            var list = plSvc.GetPinglunByMobile(mobile, start, end);
            var str = GetPLWithMarketJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
        else if (method == "getComByAction")
        {
            var mobile = context.Request["mobile"];
            var actionStr = context.Request["actionStr"];
            IFormatProvider culture = new System.Globalization.CultureInfo("zh-CN", true);
            string[] expectedFormats = { "yyyy-MM-dd HH:mm:ss fff" };
            DateTime date = DateTime.ParseExact(context.Request["dateStr"], expectedFormats, culture, System.Globalization.DateTimeStyles.AllowInnerWhite);
            var plSvc = new PinglunService();
            var list = plSvc.GetPinglunByAction(mobile, date, actionStr);
            var str = GetPLWithMarketJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
        else if (method == "groupChannel")
        {
            var mobile = context.Request["mobile"];
            var strGroups = context.Request["groups"];
            new MarketService().saveGroupChannel(mobile, strGroups, (int)Model.enums.EnumMarketFlag.WXMarket);
            context.Response.End();
        }
        else if (method == "getTodayComByProductId")
        {
            var mobile = context.Request["mobile"];
            var productId = int.Parse(context.Request["productId"]);
            var plSvc = new PinglunService();
            var start = DateTime.Today.AddDays(0);
            var end = DateTime.Today.AddDays(1);
            var list = plSvc.GetPinglunByProductId(mobile,productId, start, end);
            var str = GetPLWithMarketJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
    }

    private string GetMarketJson(List<MarketGroupVM> list)
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
                    if (list[i].Market[j].OrderPL!=null && list[i].Market[j].OrderPL.Count > 0)
                    {
                        writer.WritePropertyName("pinglun");
                        writer.WriteStartArray();

                        for (int k = 0; k < list[i].Market[j].OrderPL.Count; k++)
                        {
                            var orderPl = list[i].Market[j].OrderPL[k];
                            writer.WriteStartObject();
                            writer.WritePropertyName("id");
                            writer.WriteValue(orderPl.Id);
                            writer.WritePropertyName("title");
                            writer.WriteValue(orderPl.Title);
                            writer.WritePropertyName("date");
                            writer.WriteValue(orderPl.Date);
                            writer.WritePropertyName("url");
                            writer.WriteValue(orderPl.Url);
                            writer.WriteEndObject();
                        }
                        
                        writer.WriteEndArray();
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

    private string GetPLJson(List<AppPingLunVM> list)
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
                writer.WriteValue(list[i].Id.ToString());
                writer.WritePropertyName("productname");
                writer.WriteValue(list[i].ProductName.ToString());
                writer.WritePropertyName("title");
                writer.WriteValue(list[i].Title);
                writer.WritePropertyName("avatar");
                writer.WriteValue(list[i].Icon);
                writer.WritePropertyName("url");
                writer.WriteValue(list[i].Url);
                writer.WritePropertyName("date");
                writer.WriteValue(list[i].Date);
                writer.WritePropertyName("isOrder");
                writer.WriteValue(list[i].IsOrder);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.Flush();
            sw.Close();
        }
        return sw.GetStringBuilder().ToString();
    }

    private string GetPLWithMarketJson(List<AppPingLunVM> list)
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
                writer.WriteValue(list[i].Id.ToString());
                writer.WritePropertyName("marketname");
                writer.WriteValue(list[i].MarketName.ToString());
                writer.WritePropertyName("productname");
                writer.WriteValue(list[i].ProductName.ToString());
                writer.WritePropertyName("title");
                writer.WriteValue(list[i].Title);
                writer.WritePropertyName("avatar");
                writer.WriteValue(list[i].Icon);
                writer.WritePropertyName("url");
                writer.WriteValue(list[i].Url);
                writer.WritePropertyName("date");
                writer.WriteValue(list[i].Date);
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