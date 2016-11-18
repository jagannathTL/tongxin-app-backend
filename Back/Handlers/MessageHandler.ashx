<%@ WebHandler Language="C#" Class="MessageHandler" %>

using System;
using System.Web;

public class MessageHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "clearMessage")
        {
            var mobile = context.Request["mobile"];
            var msgSvc = new Service.MessageService();
            bool flag = msgSvc.ClearSaltByMobile(mobile);
            if (flag)
                context.Response.Write("{\"result\":\"ok\"}");
            else
                context.Response.Write("{\"result\":\"error\"}");
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}