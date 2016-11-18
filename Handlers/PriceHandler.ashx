<%@ WebHandler Language="C#" Class="PriceHandler" %>

using System;
using System.Web;
using Service;
using Service.ViewModel;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class PriceHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "getPrices")
        {
            var mobile = context.Request["mobile"];
            var marketId = context.Request["marketId"];
            var groupId = context.Request["groupId"] ?? "-1";
            if (!string.IsNullOrEmpty(mobile) && !string.IsNullOrEmpty(marketId))
            {
                var priceSvc = new ProductPriceService();
                List<ProductPriceVM> list = new List<ProductPriceVM>();
                if (groupId == "0")
                {
                    list = priceSvc.GetOrderPricesByMobileAndMarketId(mobile, int.Parse(marketId));
                }
                else
                {
                    list = priceSvc.GetPricesByMobileAndMarketId(mobile, int.Parse(marketId));
                }
                var str = GetJson(list);
                context.Response.Write(str);
            }
            context.Response.Flush();
            context.Response.End();
        }
        else if (method == "getHistoryPrices")
        {
            var productId = int.Parse(context.Request["productId"]);
            var start = DateTime.Parse(context.Request["start"]);
            var end = DateTime.Parse(context.Request["end"]);
            var priceSvc = new ProductPriceService();
            var list = priceSvc.GetHistoryPrices(productId, start, end);
            var str = GetHistoryPriceJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
    }

    private string GetJson(List<ProductPriceVM> list)
    {
        StringWriter sw = new StringWriter();
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();
            for (int i = 0; i < list.Count; i++)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("ProductId");
                writer.WriteValue(list[i].ProductId);
                writer.WritePropertyName("ProductName");
                writer.WriteValue(list[i].ProductName);
                writer.WritePropertyName("LPrice");
                writer.WriteValue(list[i].LPrice);
                writer.WritePropertyName("HPrice");
                writer.WriteValue(list[i].HPrice);
                writer.WritePropertyName("Date");
                writer.WriteValue(list[i].Date);
                writer.WritePropertyName("Change");
                writer.WriteValue(list[i].Change);
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

    private string GetHistoryPriceJson(List<ProductPriceVM> list)
    {
        StringWriter sw = new StringWriter();
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();
            for (int i = 0; i < list.Count; i++)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("LPrice");
                writer.WriteValue(list[i].LPrice);
                writer.WritePropertyName("HPrice");
                writer.WriteValue(list[i].HPrice);
                writer.WritePropertyName("Date");
                writer.WriteValue(list[i].Date);
                writer.WritePropertyName("Change");
                writer.WriteValue(list[i].Change);
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