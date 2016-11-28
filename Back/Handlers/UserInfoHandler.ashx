<%@ WebHandler Language="C#" Class="UserInfoHandler" %>

using System;
using System.Web;
using Service;
using Service.ViewModel;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        else if(method == "getUserCompanyInfo")
        {
            var mobile = context.Request["mobile"];
            var company = new UserInfoService().GetUserComponyInfoByMobile(mobile);
            var str = GetJson(company);
            context.Response.Write(str);
        }
    }

    private string GetJson(UserCompanyInfoVM company)
    { 
        StringWriter sw = new StringWriter();
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.None;
            writer.WriteStartObject();
            if (company != null)
            {
                writer.WritePropertyName("companyName");
                writer.WriteValue(company.AppCompanyName);

                writer.WritePropertyName("companyPics");

                writer.WriteStartArray();

                if (!string.IsNullOrWhiteSpace(company.AppCompanyPics))
                {
                    var arrPic = company.AppCompanyPics.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var pic in arrPic)
                    {
                        writer.WriteValue("/upload/"+pic);
                    }
                }

                writer.WriteEndArray();

                writer.WritePropertyName("customerName");
                writer.WriteValue(company.AppCustomerName);
                writer.WritePropertyName("tel");
                writer.WriteValue(company.AppTel);
                writer.WritePropertyName("industry");
                writer.WriteValue(company.AppIndustry);
                writer.WritePropertyName("product");
                writer.WriteValue(company.AppProduct);
                writer.WritePropertyName("businessDesc");
                writer.WriteValue(company.AppBusinessDesc);
                writer.WritePropertyName("province");
                writer.WriteValue(company.AppProvince);
                writer.WritePropertyName("city");
                writer.WriteValue(company.AppCity);
                writer.WritePropertyName("addressDesc");
                writer.WriteValue(company.AppAddressDesc);
                writer.WritePropertyName("isOpenMsg");
                writer.WriteValue(company.IsOpenMsg ? "true" : "false");
            }
            writer.WriteEndObject();
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