<%@ WebHandler Language="C#" Class="LoginHandler" %>

using System;
using System.Web;
using Service;

public class LoginHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "signin")
        {
            var mobile = context.Request["mobile"];
            var pwd = context.Request["password"];
            var token = context.Request["token"];
            var phoneType = Convert.ToInt32(context.Request["phoneType"]);
            string result = new CustomerBaseService().Login(mobile.Trim(), pwd.Trim(), token,phoneType) ? "ok" : "error";
            context.Response.Write("{\"result\":\"" + result + "\"}");
        }
        else if (method == "checkuser")
        {
            var mobile = context.Request["mobile"];
            var pwd = context.Request["password"];
            var token = context.Request["token"];
            string result = new CustomerBaseService().CheckUser(mobile.Trim(), pwd.Trim(), token) ? "ok" : "error";
            context.Response.Write("{\"result\":\"" + result + "\"}");
        }
        else if (method == "checkToken")
        {
            var tel = context.Request["mobile"];
            var token = context.Request["token"];
            bool result = new CustomerBaseService().CheckTokenByMobile(tel, token);
            string res = "";
            if (result)
            {
                res = "ok";
            }
            else
            {
                res = "error";
            }
            context.Response.Write("{\"result\":\"" + res + "\"}");
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}