<%@ WebHandler Language="C#" Class="orderHandler" %>

using System;
using System.Web;
using Service;
using Service.ViewModel;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class orderHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "order")
        {
            var productId = int.Parse(context.Request["productId"]);
            var mobile = context.Request["mobile"];
            var isOrder = context.Request["isOrder"] == "YES" ? true : false;
            bool flag = new OrderService().addGps(mobile, productId, isOrder);
            context.Response.Write("{\"result\":\"" + (flag ? "ok" : "error") + "\"}");
        }
        else if (method == "myorder")
        {
            var mobile = context.Request["mobile"];
            var list = new OrderService().GetMyOrder(mobile);
            var str = GetJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
        else if (method == "renew")
        {
            var mobile = context.Request["mobile"];
            bool flag = new OrderService().renew(mobile);
            context.Response.Write("{\"result\":\"" + (flag ? "ok" : "error") + "\"}");
        }
    }

    private string GetJson(List<OrderVM> list)
    {
        StringWriter sw = new StringWriter();
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();
            for (int i = 0; i < list.Count; i++)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("marketname");
                writer.WriteValue(list[i].MarketName);
                writer.WritePropertyName("productid");
                writer.WriteValue(list[i].ProductId.ToString());
                writer.WritePropertyName("productname");
                writer.WriteValue(list[i].ProductName);
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