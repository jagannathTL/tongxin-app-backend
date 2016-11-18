<%@ WebHandler Language="C#" Class="ChannelHandler" %>

using System;
using System.Web;
using Service;
using Service.ViewModel;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using Model.enums;

public class ChannelHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "getchannel")
        {
            var channelSvc = new ChannelService();
            var list = channelSvc.GetAllChannels();
            var str = GetChannelJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
        else if (method == "getcatalog")
        {
            var channelId = int.Parse(context.Request["channelId"]);
            var cataLogSvc = new CataLogService();
            var list = cataLogSvc.GetCataLogByChannelId(channelId);
            var str = GetCataLogJson(list);
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }
    }

    private string GetChannelJson(List<ChannelVM> list)
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
                writer.WritePropertyName("Name");
                writer.WriteValue(list[i].Name.ToString().Trim());
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.Flush();
            sw.Close();
        }
        return sw.GetStringBuilder().ToString();
    }

    private string GetCataLogJson(List<CataLogVM> list)
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
                writer.WritePropertyName("Name");
                writer.WriteValue(list[i].Name.ToString().Trim());
                writer.WritePropertyName("Desc");
                writer.WriteValue(list[i].Desc.ToString().Trim());
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