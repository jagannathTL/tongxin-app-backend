<%@ WebHandler Language="C#" Class="XHMarketHandler" %>

using System;
using System.Web;
using Service;
using Service.ViewModel;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;


public class XHMarketHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        //var mobile = context.Request["mobile"];
        if (method == "getmarkets")
        {
            var mobile = context.Request["mobile"] ?? "";
            
            var marketSvc = new MarketService();
            var list = marketSvc.GetMarkets(mobile, (int)Model.enums.EnumMarketFlag.XHMarket);
            var str = GetJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
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