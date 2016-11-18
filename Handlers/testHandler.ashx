<%@ WebHandler Language="C#" Class="testHandler" %>

using System;
using System.Web;
using System.IO;

public class testHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        //不可以删除
        if (method == "health")
        {
            context.Response.Write("ok");
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}