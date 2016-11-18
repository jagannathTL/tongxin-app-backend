<%@ WebHandler Language="C#" Class="SearchHandler" %>

using System;
using System.Web;
using Service;
using Service.ViewModel;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class SearchHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "getSearchResult")
        {
            var key = context.Request["searchKey"].Trim();
            var mobile = context.Request["mobile"];
            if (!string.IsNullOrEmpty(key))
            {
                var svc = new SearchPriceService();
                var list = svc.GetSearchResult(key, mobile);
                var str = GetJson(list);
                context.Response.Write(str);
            }
            else
            {
                context.Response.Write("[]");
            }
            context.Response.Flush();
            context.Response.End();
        }
    }

    private string GetJson(List<SearchPriceVM> markets)
    {
        StringWriter sw = new StringWriter();
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();
            foreach (var market in markets)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("id");
                writer.WriteValue(market.MarketId);
                writer.WritePropertyName("name");
                writer.WriteValue(market.MarketName);
                writer.WritePropertyName("products");
                writer.WriteStartArray();
                foreach (var price in market.prices)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("ProductId");
                    writer.WriteValue(price.ProductId);
                    writer.WritePropertyName("ProductName");
                    writer.WriteValue(price.ProductName);
                    writer.WritePropertyName("LPrice");
                    writer.WriteValue(price.LPrice);
                    writer.WritePropertyName("HPrice");
                    writer.WriteValue(price.HPrice);
                    writer.WritePropertyName("Date");
                    writer.WriteValue(price.Date);
                    writer.WritePropertyName("Change");
                    writer.WriteValue(price.Change);
                    writer.WritePropertyName("isOrder");
                    writer.WriteValue(price.IsOrder);
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
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}