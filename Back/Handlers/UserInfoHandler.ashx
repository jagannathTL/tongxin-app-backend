<%@ WebHandler Language="C#" Class="UserInfoHandler" %>

using System;
using System.Web;
using Service;

public class UserInfoHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        var method = context.Request["method"];
        if (method == "getUserInfo")
        {
            var mobile = context.Request["mobile"];
            Service.ViewModel.UserInfoVM userInfo = new UserInfoService().GetUserInfoByMobile(mobile);
            context.Response.Write("{\"endDate\":\"" + userInfo.EndDate + "\",\"isSound\":\"" + userInfo.IsSound.ToString().ToLower() + "\"}");
        }
        else if (method == "setUserInfo")
        {
            var mobile = context.Request["mobile"];
            var isSound = context.Request["isSound"] == "0" ? false : true;
            var flag = new UserInfoService().SetUserInfoSound(mobile, isSound);
            if (flag)
            {
                context.Response.Write("{\"result\":\"ok\"}");
            }
            else
            {
                context.Response.Write("{\"result\":\"error\"}");
            }
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