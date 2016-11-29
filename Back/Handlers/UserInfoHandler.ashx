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
        else if (method == "updateUserInfo")
        {
            var mobile = context.Request["mobile"];
            var pics = context.Request["pics"] == null ? "" : context.Request["pics"].ToString();
            //pics = @"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASwAAACWCAYAAABkW7XSAAAAAXNSR0IArs4c6QAAAylJREFUeAHt0DEBAAAAwqD1T20IX4hAYcCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYOAdGL/UAAEPpnR6AAAAAElFTkSuQmCC|||data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASwAAACWCAYAAABkW7XSAAAAAXNSR0IArs4c6QAAAylJREFUeAHt0DEBAAAAwqD1T20IX4hAYcCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGDBgwYOAdGL/UAAEPpnR6AAAAAElFTkSuQmCC|||";
            var companyName = context.Request["companyName"] == null ? "" : context.Request["companyName"].ToString();
            var contact = context.Request["contact"] == null ? "" : context.Request["contact"].ToString();
            var tel = context.Request["tel"] == null ? "" : context.Request["tel"].ToString();
            var industry = context.Request["industry"] == null ? "" : context.Request["industry"].ToString();
            var product = context.Request["product"] == null ? "" : context.Request["product"].ToString();
            var industryDesc = context.Request["industryDesc"] == null ? "" : context.Request["industryDesc"].ToString();
            var provinceName = context.Request["provinceName"] == null ? "" : context.Request["provinceName"].ToString();
            var cityName = context.Request["cityName"] == null ? "" : context.Request["cityName"].ToString();
            var addressDesc = context.Request["addressDesc"] == null ? "" : context.Request["addressDesc"].ToString();
            var isOpenMsg = context.Request["isOpenMsg"] == null ? "" : context.Request["isOpenMsg"].ToString();

            var picName = new List<string>();
            if (!string.IsNullOrWhiteSpace(pics))
            {
                var arr = pics.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var a in arr)
                {
                    var pic = a.Replace("data:image/png;base64,","");
                    var fileName = Guid.NewGuid()+".jpg";
                    var path = context.Server.MapPath("~/upload")+"/"+fileName;
                    Byte[] streamByte = Convert.FromBase64String(pic);
                    System.IO.File.WriteAllBytes(path, streamByte);
                    picName.Add(fileName);
                }
            }

            UserCompanyInfoVM company = new UserCompanyInfoVM();
            company.AppAddressDesc = addressDesc;
            company.AppBusinessDesc = industryDesc;
            company.AppCity = cityName;
            company.AppCompanyName = companyName;
            company.AppCustomerName = contact;
            company.AppIndustry = industry;
            company.AppProduct = product;
            company.AppProvince = provinceName;
            company.AppTel = tel;
            company.IsOpenMsg = isOpenMsg.ToLower() == "true" ? true : false;
            var flag = new UserInfoService().SaveUserCompany(mobile,company, picName);
            if (flag)
            {
                context.Response.Write("{\"result\":\"ok\"}");
            }
            else
            {
                context.Response.Write("{\"result\":\"error\"}");
            }
        }
        else if (method == "uploadCmpPic")
        { 
            
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
                    var arrPic = company.AppCompanyPics.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
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