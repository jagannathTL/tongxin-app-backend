<%@ WebHandler Language="C#" Class="CustomerHandler" %>

using System;
using System.Web;
using Service;

public class CustomerHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "trial")
        {
            var mobile = context.Request["mobile"];
            TrialError result = new CustomerService().addTrial(mobile);
            string rtn = "ok";
            
            if (result == TrialError.Invalid)
            {
                rtn = "手机号码不合法，请重新输入!";
            }
            else if (result == TrialError.Registered)
            {
                rtn = "该手机号已被注册，请使用其他号码!";
            }
            else if (result == TrialError.SystemError)
            {
                rtn = "系统错误，请重试";
            }
            else if (result == TrialError.TooManyTrial)
            {
                rtn = "已超过试用次数限制!";
            }
            
            context.Response.Write("{\"result\":\"" + rtn + "\"}");
        }
        else if (method == "send")
        {
            var mobile = context.Request["mobile"];
            var result = new CustomerService().sendPwd(mobile);
            context.Response.Write("{\"result\":\"" + (result ? "ok" : "error") + "\"}");
            
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