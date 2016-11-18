<%@ WebHandler Language="C#" Class="DownloadCountHandler" %>

using System;
using System.Web;

public class DownloadCountHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        
        var method = context.Request["method"];
        if (method == "addCount")
        {
            new Service.DownloadCountService().addCount();
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }
}