<%@ WebHandler Language="C#" Class="InboxMsgHandler" %>

using System;
using System.Web;
using Service;
using Service.ViewModel;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


public class InboxMsgHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "getInboxMsg")
        {
            var mobile = context.Request["mobile"];
            var inboxSvc = new InboxMsgService();
            var list = inboxSvc.GetInBoxMsgByMobile(mobile);
            var str = GetJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
        else if (method == "getMsgByAction")
        {
            var mobile = context.Request["mobile"];
            var action = context.Request["actionStr"];
            IFormatProvider culture = new System.Globalization.CultureInfo("zh-CN", true);
            string[] expectedFormats = {"yyyy-MM-dd HH:mm:ss fff"}; 
            DateTime date = DateTime.ParseExact(context.Request["dateStr"],expectedFormats,culture, System.Globalization.DateTimeStyles.AllowInnerWhite);
            var inboxSvc = new InboxMsgService();
            var list = inboxSvc.GetInBoxMsgByAction(mobile,action,date);
            var str = GetJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
    }

    private string GetJson(List<InboxVM> list)
    {
        StringWriter sw = new StringWriter();
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;
            writer.WriteStartArray();
            for (int i = 0; i < list.Count; i++)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("msg");
                writer.WriteValue(list[i].Msg);
                writer.WritePropertyName("url");
                writer.WriteValue(list[i].Url);
                writer.WritePropertyName("date");
                writer.WriteValue(list[i].Date.Value.ToString("yyyy-MM-dd HH:mm:ss fff"));
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